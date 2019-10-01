using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CisaNet.Entity;

namespace CISAKafkaConnector
{
    public class NFC
    {
        int retCode;
        int hCard;
        int hContext;
        int Protocol;
        public bool connActive = false;
        //string readername = "ACS ACR122 0"; // Change depending on reader
        string readername = ""; // Will change depending on connected reader/writer device
        public byte[] SendBuff = new byte[263];
        public byte[] RecvBuff = new byte[263];
        public int SendLen, RecvLen, nBytesRet, reqType, Aprotocol, dwProtocol, cbPciLength;
        public Card.SCARD_READERSTATE RdrState;
        public Card.SCARD_IO_REQUEST pioSendRequest;
        public static VBHelpers vbHelper = new VBHelpers();

        public void SelectDevice()
        {
            List<string> availableReaders = this.ListReaders();
            this.RdrState = new Card.SCARD_READERSTATE();
            readername = availableReaders[0].ToString();//selecting first device
            this.RdrState.RdrName = readername;
        }

        public List<string> ListReaders()
        {
            int ReaderCount = 0;
            List<string> AvailableReaderList = new List<string>();

            // Make sure a context has been established before 
            // retrieving the list of smartcard readers.
            retCode = Card.SCardListReaders(hContext, null, null, ref ReaderCount);
            if (retCode != Card.SCARD_S_SUCCESS)
            {
                Console.WriteLine(Card.GetScardErrMsg(retCode));
                //connActive = false;
            }

            byte[] ReadersList = new byte[ReaderCount];

            // Get the list of reader present again but this time add sReaderGroup, retData as 2rd & 3rd parameter respectively.
            retCode = Card.SCardListReaders(hContext, null, ReadersList, ref ReaderCount);
            if (retCode != Card.SCARD_S_SUCCESS)
            {
                Console.WriteLine(Card.GetScardErrMsg(retCode));
            }

            string rName = "";
            int indx = 0;
            if (ReaderCount > 0)
            {
                // Convert reader buffer to string
                while (ReadersList[indx] != 0)
                {

                    while (ReadersList[indx] != 0)
                    {
                        rName = rName + (char)ReadersList[indx];
                        indx = indx + 1;
                    }

                    // Add reader name to list
                    AvailableReaderList.Add(rName);
                    rName = "";
                    indx = indx + 1;

                }
            }
            return AvailableReaderList;

        }

        internal void establishContext()
        {
            retCode = Card.SCardEstablishContext(Card.SCARD_SCOPE_SYSTEM, 0, 0, ref hContext);
            if (retCode != Card.SCARD_S_SUCCESS)
            {
                Console.WriteLine("Check your device and please restart again");
                connActive = false;
                return;
            }
        }

        /*
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (connectCard())
            {
                string cardUID = getcardUID();
                textBlock1.Text = cardUID; //displaying on text block
            }
        }*/

        public bool connectCard()
        {
            connActive = true;

            retCode = Card.SCardConnect(hContext, readername, Card.SCARD_SHARE_SHARED,
                      Card.SCARD_PROTOCOL_T0 | Card.SCARD_PROTOCOL_T1, ref hCard, ref Protocol);

            if (retCode != Card.SCARD_S_SUCCESS)
            {
                Console.WriteLine("Card not available: " + Card.GetScardErrMsg(retCode));
                connActive = false;
                return false;
            }
            return true;
        }

        public string getcardUID() // Only for Mifare 1k cards
        {
            string cardUID = "";
            byte[] receivedUID = new byte[256];
            Card.SCARD_IO_REQUEST request = new Card.SCARD_IO_REQUEST();
            request.dwProtocol = Card.SCARD_PROTOCOL_T1;
            request.cbPciLength = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Card.SCARD_IO_REQUEST));
            byte[] sendBytes = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 }; // Get UID command for Mifare cards
            int outBytes = receivedUID.Length;
            int status = Card.SCardTransmit(hCard, ref request, ref sendBytes[0], sendBytes.Length, ref request, ref receivedUID[0], ref outBytes);

            if (status != Card.SCARD_S_SUCCESS)
            {
                cardUID = "Error";
            }
            else
            {
                cardUID = BitConverter.ToString(receivedUID.Take(4).ToArray()).Replace("-", string.Empty).ToLower();
            }

            return cardUID;
        }       

        // Write Hotel File
        public void writeHotelFile (string file) // Hotel file is 36*2 chars long
        {
            string block4data, block5data, block6data; // wavemodebufCard for Mifare 

            // The Hotel file must be written in the first dedicated Mifare Classic sector (sector 1 by default)
            // the 36 bytes obtained from the function must be transformed into a 48 bytes buffer (corresponding to 3 Mifare blocks, 16 bytes each) as described below:

            block4data = file.Substring(0,32); // Get first 16 bytes (32 chars)
            block5data = file.Substring(32,24) + "00000000"; // Get following 12 bytes (24 chars) padded to 16 bytes with zeroes (4 x 0x00)
            block6data = file.Substring(56) + "0000000000000000"; // Get resting 8 bytes (16 chars) padded to 16 bytes with zeroes (8 x 0x00)

            submitHex(block4data.Hex2Byte(), "4"); // Sector 1 - data block 4 is the data block we start writing Hotel File data on the card
            submitHex(block5data.Hex2Byte(), "5"); // data block 5
            submitHex(block6data.Hex2Byte(), "6"); // data block 6
        }

        // Read Hotel File
        public string readHotelFile() // Hotel file is 36*2 chars long
        {
            string file; // wavemodebufCard for Mifare 

            // The Hotel file must be written in the first dedicated Mifare Classic sector (sector 1 by default)
            // the 36 bytes obtained from the function must be transformed into a 48 bytes buffer (corresponding to 3 Mifare blocks, 16 bytes each) as described below:
            
            /*byte[] bloque4 = Encoding.ASCII.GetBytes(verifyCard("4"));
            byte[] bloque5 = Encoding.ASCII.GetBytes(verifyCard("5"));
            byte[] bloque6 = Encoding.ASCII.GetBytes(verifyCard("6"));
            */
            file = verifyCard("4").Substring(0, 32); // file.Substring(0, 32); // Get first 16 bytes (32 chars)
            file = file + verifyCard("5").Substring(0,24); // file.Substring(32, 24) + "00000000"; // Get following 12 bytes (24 chars) padded to 16 bytes with zeroes (4 x 0x00)
            file = file + verifyCard("6").Substring(0,16); // file.Substring(56) + "0000000000000000"; // Get resting 8 bytes (16 chars) padded to 16 bytes with zeroes (8 x 0x00)

            return file;
        }

        // Write Keyplan File
        public void writeKeyplanFile(string file) // Keyplan file is 192*2 chars long
        {
            // The Keyplan file must be written from the following dedicated Mifare Classic sector(sector 2 by default), without any padding.
            //byte[] test = new byte[] { 0x22, 0x00, 0x25, 0x00, 0x9E, 0x04, 0x7C, 0x45, 0x0C, 0x69, 0x00, 0x20, 0x00, 0xFF, 0x89, 0x017 };

            submitHex(file.Substring(0, 32).Hex2Byte(), "8"); // Sector 2 - data block 8 is the data block we start writing Keyplan File data on the card
            submitHex(file.Substring(32, 32).Hex2Byte(), "9"); // data block 9
            submitHex(file.Substring(64, 32).Hex2Byte(), "10"); // data block 10
            submitHex(file.Substring(96, 32).Hex2Byte(), "12"); // Sector 3 - data block 12
            submitHex(file.Substring(128, 32).Hex2Byte(), "13"); // data block 13
            submitHex(file.Substring(160, 32).Hex2Byte(), "14"); // data block 14            
            submitHex(file.Substring(192, 32).Hex2Byte(), "16"); // Sector 4 - data block 16
            submitHex(file.Substring(224, 32).Hex2Byte(), "17"); // data block 17
            submitHex(file.Substring(256, 32).Hex2Byte(), "18"); // data block 18
            submitHex(file.Substring(288, 32).Hex2Byte(), "20"); // Sector 5 - data block 20
            submitHex(file.Substring(320, 32).Hex2Byte(), "21"); // data block 21
            submitHex(file.Substring(352, 32).Hex2Byte(), "22"); // data block 22
            //submitHex(test, "22"); // data block 22
        }

        // Read Keyplan File
        public string readKeyplanFile() // Hotel file is 36*2 chars long
        {
            string file;

            // Keyplan file must be written from the following dedicated Mifare Classic sector(sector 2 by default), without any padding.
            //byte[] test = new byte[] { 0x22, 0x00, 0x25, 0x00, 0x9E, 0x04, 0x7C, 0x45, 0x0C, 0x69, 0x00, 0x20, 0x00, 0xFF, 0x89, 0x017 };

            /*
            byte[] bloque8 = Encoding.ASCII.GetBytes(verifyCard("8"));
            byte[] bloque9 = Encoding.ASCII.GetBytes(verifyCard("9"));
            byte[] bloque10 = Encoding.ASCII.GetBytes(verifyCard("10"));
            byte[] bloque12 = Encoding.ASCII.GetBytes(verifyCard("12"));
            byte[] bloque13= Encoding.ASCII.GetBytes(verifyCard("13"));
            byte[] bloque14 = Encoding.ASCII.GetBytes(verifyCard("14"));
            byte[] bloque16 = Encoding.ASCII.GetBytes(verifyCard("16"));
            byte[] bloque17 = Encoding.ASCII.GetBytes(verifyCard("17"));
            byte[] bloque18 = Encoding.ASCII.GetBytes(verifyCard("18"));
            byte[] bloque20 = Encoding.ASCII.GetBytes(verifyCard("20"));
            byte[] bloque21 = Encoding.ASCII.GetBytes(verifyCard("21"));
            byte[] bloque22 = Encoding.ASCII.GetBytes(verifyCard("22"));
            */

            file = verifyCard("8").Substring(0, 32); // Sector 2 - data block 8 is the data block we start writing Keyplan File data on the card
            file = file + verifyCard("9").Substring(0, 32); // data block 9
            file = file + verifyCard("10").Substring(0, 32); // data block 10
            file = file + verifyCard("12").Substring(0, 32); // Sector 3 - data block 12
            file = file + verifyCard("13").Substring(0, 32); // data block 13
            file = file + verifyCard("14").Substring(0, 32); // data block 14            
            file = file + verifyCard("16").Substring(0, 32); // Sector 4 - data block 16
            file = file + verifyCard("17").Substring(0, 32); // data block 17
            file = file + verifyCard("18").Substring(0, 32); // data block 18
            file = file + verifyCard("20").Substring(0, 32); // Sector 5 - data block 20
            file = file + verifyCard("21").Substring(0, 32); // data block 21
            file = file + verifyCard("22").Substring(0, 32); // data block 22

            return file;
        }

        // Write card - Submit Text method
        public void submitText(String Text, String Block)
        {

            String tmpStr = Text;
            int indx;
            if (authenticateBlock(Block))
            {
                ClearBuffers();
                SendBuff[0] = 0xFF;                             // CLA
                SendBuff[1] = 0xD6;                             // INS
                SendBuff[2] = 0x00;                             // P1
                SendBuff[3] = (byte)int.Parse(Block);           // P2 : Starting Block No.
                SendBuff[4] = (byte)int.Parse("16");            // P3 : Data length - Maximum 48 bytes for MIFARE 1K. (Multiple Blocks Mode; 3 consecutive blocks)                

                for (indx = 0; indx <= (tmpStr).Length - 1; indx++)
                {
                    SendBuff[indx + 5] = (byte)tmpStr[indx]; 
                }
                SendLen = SendBuff[4] + 5;
                RecvLen = 0x02;

                retCode = SendAPDUandDisplay(2);

                if (retCode != Card.SCARD_S_SUCCESS)
                {
                   Console.WriteLine("fail write");
                }
                else
                {
                    Console.WriteLine("write success");
                }
            }
            else
            {
                Console.WriteLine("FailAuthentication");
            }
        }

        // Write card - Submit Hex byte[] method
        public void submitHex(byte[] data, String Block)
        {

            int indx;
            if (authenticateBlock(Block))
            {
                ClearBuffers();
                SendBuff[0] = 0xFF;                             // CLA
                SendBuff[1] = 0xD6;                             // INS
                SendBuff[2] = 0x00;                             // P1
                SendBuff[3] = (byte)int.Parse(Block);           // P2 : Starting Block No.
                SendBuff[4] = (byte)int.Parse("16");            // P3 : Data length - Maximum 48 bytes for MIFARE 1K. (Multiple Blocks Mode; 3 consecutive blocks)

                //byte[] test = new byte[] { 0x01, 0x00, 0x25, 0x00, 0x9E, 0x04, 0x7C, 0x45, 0x0C, 0x69, 0x00, 0x20, 0x00, 0xFF, 0x89, 0x017 };
                
                for (indx = 0; indx <= (data).Length - 1; indx++)
                {
                    SendBuff[indx + 5] = (byte)data[indx];
                }
                SendLen = SendBuff[4] + 5;
                RecvLen = 0x02;

                retCode = SendAPDUandDisplay(2);

                if (retCode != Card.SCARD_S_SUCCESS)
                {
                    Console.WriteLine("block write fail");
                }
                else
                {
                    Console.WriteLine("block write success");
                }
            }
            else
            {
                Console.WriteLine("FailAuthentication");
            }
        }

        // Block authentication
        private bool authenticateBlock(String block)
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[2] = 0x00;                         // P1: same for all source types 
            SendBuff[1] = 0x86;                         // INS: for stored key input
            SendBuff[3] = 0x00;                         // P2 : Memory location;  P2: for stored key input
            SendBuff[4] = 0x05;                         // P3: for stored key input
            SendBuff[5] = 0x01;                         // Byte 1: version number
            SendBuff[6] = 0x00;                         // Byte 2
            SendBuff[7] = (byte)int.Parse(block);       // Byte 3: sectore no. for stored key input
            SendBuff[8] = 0x60;                         // Byte 4 : Key A for stored key input
            SendBuff[9] = (byte)int.Parse("1");         // Byte 5 : Session key for non-volatile memory

            SendLen = 0x0A;
            RecvLen = 0x02;

            retCode = SendAPDUandDisplay(0);

            if (retCode != Card.SCARD_S_SUCCESS)
            {
                // MessageBox.Show("FAIL Authentication!");
                return false;
            }

            return true;
        }

        // Clear memory buffers
        private void ClearBuffers()
        {
            long indx;

            for (indx = 0; indx <= 262; indx++)
            {
                RecvBuff[indx] = 0;
                SendBuff[indx] = 0;
            }
        }

        // Send application protocol data unit : communication unit between a smart card reader and a smart card
        private int SendAPDUandDisplay(int reqType)
        {
            int indx;
            string tmpStr = "";

            pioSendRequest.dwProtocol = Aprotocol;
            pioSendRequest.cbPciLength = 8;

            // Display APDU In
            for (indx = 0; indx <= SendLen - 1; indx++)
            {
                tmpStr = tmpStr + " " + string.Format("{0:X2}", SendBuff[indx]);
            }

            retCode = Card.SCardTransmit(hCard, ref pioSendRequest, ref SendBuff[0],
                                 SendLen, ref pioSendRequest, ref RecvBuff[0], ref RecvLen);

            if (retCode != Card.SCARD_S_SUCCESS)
            {
                return retCode;
            }

            else
            {
                try
                {
                    tmpStr = "";
                    switch (reqType)
                    {
                        case 0:
                            for (indx = (RecvLen - 2); indx <= (RecvLen - 1); indx++)
                            {
                                tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                            }

                            if ((tmpStr).Trim() != "90 00")
                            {
                                // MessageBox.Show("Return bytes are not acceptable.");
                                return -202;
                            }

                            break;

                        case 1:

                            for (indx = (RecvLen - 2); indx <= (RecvLen - 1); indx++)
                            {
                                tmpStr = tmpStr + string.Format("{0:X2}", RecvBuff[indx]);
                            }

                            if (tmpStr.Trim() != "90 00")
                            {
                                tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                            }

                            else
                            {
                                tmpStr = "ATR : ";
                                for (indx = 0; indx <= (RecvLen - 3); indx++)
                                {
                                    tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                                }
                            }

                            break;

                        case 2:

                            for (indx = 0; indx <= (RecvLen - 1); indx++)
                            {
                                tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                            }

                            break;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    return -200;
                }
            }
            return retCode;
        }

        // Disconnect card reader connection
        public void Close()
        {
            if (connActive)
            {
                retCode = Card.SCardDisconnect(hCard, Card.SCARD_UNPOWER_CARD);
            }
            //retCode = Card.SCardReleaseContext(hCard);
        }

        // Read Card
        public string verifyCard(String Block)
        {
            string value = "";
            if (connectCard())
            {
                value = readBlock(Block);
            }

            // value = value.Split(new char[] { '\0' }, 2, StringSplitOptions.None)[0].ToString(); // not valid for CISA encoder
            // Remove 0x90 0x00 (SendAPDUandDisplay command) - remove last 4 chars from the string
            value = value.Substring(0, value.Length - 4);

            return value;
        }

        public string readBlock(String Block)
        {
            string tmpStr = "";
            int indx;

            if (authenticateBlock(Block))
            {
                ClearBuffers();
                SendBuff[0] = 0xFF; // CLA 
                SendBuff[1] = 0xB0; // INS
                SendBuff[2] = 0x00; // P1
                SendBuff[3] = (byte)int.Parse(Block); // P2 : Block No.
                SendBuff[4] = (byte)int.Parse("16"); // Bytes Lenght

                SendLen = 5;
                RecvLen = SendBuff[4] + 2;

                retCode = SendAPDUandDisplay(2);

                if (retCode == -200)
                {
                    return "outofrangeexception";
                }

                if (retCode == -202)
                {
                    return "BytesNotAcceptable";
                }

                if (retCode != Card.SCARD_S_SUCCESS)
                {
                    return "FailRead";
                }

                /* Display data in text format
                for (indx = 0; indx <= RecvLen - 1; indx++)
                {
                    tmpStr = tmpStr + Convert.ToChar(RecvBuff[indx]);
                }*/

                for (indx = 0; indx <= RecvLen - 1; indx++)
                {
                    tmpStr = tmpStr + string.Format("{0:X2}", RecvBuff[indx]);
                }

                return (tmpStr);
            }
            else
            {
                return "FailAuthentication";
            }
        }
    }
}
