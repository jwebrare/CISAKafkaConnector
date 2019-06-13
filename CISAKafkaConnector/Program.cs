using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Confluent.Kafka;
using CisaNet.Entity;
using Newtonsoft.Json;

namespace CISAKafkaConnector
{
    class Program
    {
        public static string CardUID = "‭‬"; // Card UID for testing ‭0000000092312066‬

        public static VBHelpers vbHelper = new VBHelpers();
        public static List<string> kafkaTopics;

        /*The default procedure to encode a guest card (the most common use case) could be as follows:
         CSEReadDateTime, to check if the communication with the encoding system is active and to retrieve the current date/time to be set as beginning validity date/time
         CSECard2Buffer, where all details about the card have to be inputted (validity date/time, room number, etc.)
         CSEWaveModeEncode, which needs as inputs the buffer obtained above and the physical support’s UID read by the RFID reader/writer, and gives as output the raw data string to be encoded on the card.

        Calling the CSECard2Buffer you retrieve from the Cisa encoder the Buffer string that has to be written into a card.
        */
        [DllImport("csemks32")]
        public static extern short CSEBuffer2Card([In, Out] byte[] bufCard, string accesstname1, string accesstname2, string accesstname3, string accesstname4, string accesstname5, ref Csemks32.card card, string warning);
        //Declare Function CSEBuffer2Card Lib "csemks32" (ByVal bufCard() As Byte, ByVal accesstname1 As String, ByVal accesstname2 As String, ByVal accesstname3 As String, ByVal accesstname4 As String, ByVal accesstname5 As String, ByRef card As card, ByVal warning As String) As Short

        [DllImport("csemks32")]
        public static extern short CSECard2Buffer(ref Csemks32.card card, string accesstname1, string accesstname2, string accesstname3, string accesstname4, string accesstname5, [In, Out] byte[] bufCard);
        //Declare Function CSECard2Buffer Lib "csemks32" (ByRef card As card, ByVal accesstname1 As String, ByVal accesstname2 As String, ByVal accesstname3 As String, ByVal accesstname4 As String, ByVal accesstname5 As String, ByVal bufCard() As Byte) As Short

        [DllImport("csemks32")]
        public static extern short CSECheckOutCard(ref Csemks32.card card);
        //Declare Function CSECheckOutCard Lib "csemks32" (ByRef card As card) As Short

        [DllImport("csemks32")]
        public static extern short CSEConvertAccessTarget(string accesstname, string cardtype, ref Csemks32.accesstarget accestarget);
        //Declare Function CSEConvertAccessTarget Lib "csemks32" (ByVal accesstname As String, ByVal cardtype As String, ByRef accesstarget As accesstarget) As Short

        [DllImport("csemks32")]
        public static extern short CSECopy2Buffer(string accesstname, [In, Out] byte[] bufCard);
        //Declare Function CSECopy2Buffer Lib "csemks32" (ByVal accesstname As String, ByVal bufCard() As Byte) As Short

        [DllImport("csemks32")]
        public static extern short CSECreateCardToDo(ref Csemks32.card card, ref Csemks32.card cardOp, ref Csemks32.card cardToRecode);
        //Declare Function CSECreateCardToDo Lib "csemks32" (ByRef card As card, ByRef cardOp As card, ByRef cardToRecode As card) As Short

        [DllImport("csemks32")]
        public static extern short CSECreateCategory(string accesstname, ref Csemks32.categoryparams categoryparams, ref Csemks32.accesstarget accesstarget);
        //Declare Function CSECreateCategory Lib "csemks32" (ByVal accesstname As String, ByRef categoryparams As categoryparams, ByRef accesstarget As accesstarget) As Short

        [DllImport("csemks32")]
        public static extern short CSEDeleteCategory(ref Csemks32.accesstarget accesstarget);
        //Declare Function CSEDeleteCategory Lib "csemks32" (ByRef accesstarget As accesstarget) As Short

        [DllImport("csemks32")]
        public static extern short CSEExit();
        //Declare Function CSEExit Lib "csemks32" () As Short

        [DllImport("csemks32")]
        public static extern short CSEFreeEncoder(ref Gn32.GPA gpaEncoder);
        //Declare Function CSEFreeEncoder Lib "csemks32" (ByRef gpaEncoder As GPA) As Short

        [DllImport("csemks32")]
        public static extern short CSEIsUsedCategory(ref Csemks32.accesstarget accesstarget, ref short fIsUsed);
        //Declare Function CSEIsUsedCategory Lib "csemks32" (ByRef accesstarget As accesstarget, ByRef fIsUsed As Short) As Short

        [DllImport("csemks32")]
        public static extern short CSELoadErrNo();
        //Declare Function CSEErrNo Lib "csemks32" Alias "CSELoadErrNo" () As Short

        [DllImport("csemks32")]
        public static extern short CSEReadAccessTarget(ref Csemks32.accesstarget accesstarget, short fNext, string accesstname, string bufParams);
        //Declare Function CSEReadAccessTarget Lib "csemks32" (ByRef accesstarget As accesstarget, ByVal fNext As Short, ByVal accesstname As String, ByVal bufParams As String) As Short

        [DllImport("csemks32")]
        public static extern short CSEReadDateTime(ref Csemks32.csdate csdate, ref Csemks32.cstime cstime, out int seconds);
        //Declare Function CSEReadDateTime Lib "csemks32" (ByRef csdate As csdate, ByRef cstime As cstime, ByVal seconds As String) As Short

        [DllImport("csemks32")]
        public static extern short CSESearchCard(ref Csemks32.card cardOp, ref Csemks32.card card, ref Csemks32.cardsearch cardsearch);
        // ??? Declare Function CSESearchCard Lib "csemks32" (ByRef cardOp As Long, ByRef card As card, ByRef cardsearch As cardsearch) As Short

        [DllImport("csemks32")]
        public static extern short CSEUseEncoder(ref Gn32.GPA gpaEncoder);
        //Declare Function CSEUseEncoder Lib "csemks32" (ByRef gpaEncoder As GPA) As Short

        [DllImport("csemks32")]
        public static extern short CSEWaveModeDecode([In, Out] byte[] wavemodebufCard, string wavemodecarduid, [In, Out] byte[] bufCard);

        [DllImport("csemks32")]
        public static extern short CSEWaveModeEncode([In, Out] byte[] bufCard, string wavemodecarduid, byte wavemodecardheader, [In, Out] byte[] wavemodebufCard);
        //Declare Function CSEWaveModeEncode Lib "csemks32" (ByVal bufCard() As Byte, ByVal wavemodecarduid As String, ByVal wavemodecardheader As Byte, ByVal wavemodebufCard() As Byte) As Short
        // The wavemodebufCard output is ready to be written into an ST SRI512 or SRI4K (ISO 14443-B) card, always starting from block 07.
        // In order to adapt this card format to the Mifare blocks, a simple manipulation must be done. Read page 35 "ukdll16.pdf" doc.
        
        public class CISAResult
        {
            public short rc { get; set; }
            public string errordesc { get; set; }
        }
        
        public class Payload
        {
            public string accessType { get; set; }
            public string accessId { get; set; }
            public string addressType { get; set; }
            public string checkoutHours { get; set; }
            public List<string> extraSpaces { get; set; }
            public List<string> groups { get; set; }
            public string hotelName { get; set; }
            public string id { get; set; }
            public string mac { get; set; }
            public string room { get; set; }
            public string zone { get; set; }
            public string deviceId { get; set; }
        }

        public class KafkaMessage
        {
            public string code { get; set; }
            public Payload payload { get; set; }
        }

        public static void Main(string[] args)
        {
            string encoderID = Properties.Settings.Default.EncoderId;

            Console.WriteLine("Sarting up application.");
            Console.WriteLine("EncoderID: " + encoderID);
            Helpers.WriteLog("Starting up application.");

            /* Reading App Configuration Values */
            string bootstrapServers = Properties.Settings.Default.BootstrapServers;
            int autoOffsetReset = Properties.Settings.Default.AutoOffsetReset;

            var jsonsettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            // Preparing NFC Device
            NFC NFCDevice = new NFC();
            NFCDevice.SelectDevice();
            NFCDevice.establishContext();
            if (NFCDevice.connectCard())
            {
                //Console.WriteLine("Card UID bytes lenght: " + System.Text.ASCIIEncoding.ASCII.GetByteCount(NFCDevice.getcardUID());
                CardUID = NFCDevice.getcardUID();
                //CardUID = "C15A13FF" + NFCDevice.getcardUID(); // Mifare Classic cards have a 4-byte UID, which must be padded with 0xC15A13FF to reach an 8-byte length
                Console.WriteLine("Card UID: " + CardUID);
            }


            // Keep the console window open in debug mode.
            //Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();


            //Kafka Consumer
            Random rand = new Random();
            int rvalue = rand.Next(1000);

            kafkaTopics = new List<string>() { "write_request", "card_readers_request" };
            KafkaMessage kmessage;

            Action<DeliveryReportResult<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            var confC = new ConsumerConfig
                {
                    GroupId = "testingCISA" + rvalue.ToString("000"),
                    BootstrapServers = bootstrapServers,
                    AutoOffsetReset = (AutoOffsetResetType)autoOffsetReset // By default 1 == AutoOffsetResetType.Earliest
                    //SecurityProtocol = SecurityProtocolType.Sasl_Plaintext,
                    //SaslMechanism = SaslMechanismType.ScramSha256,
                    //SaslPassword = "90ZRaduaUyRfNbzIJnXVIRlmkbTgfFn8",
                    //SaslUsername = "y6bxsy8c",
                    //
                    // Note: The AutoOffsetReset property determines the start offset in the event
                    // there are not yet any committed offsets for the consumer group for the
                    // topic/partitions of interest. By default, offsets are committed
                    // automatically, so in this example, consumption will only start from the
                    // earliest message in the topic 'write_request' the first time you run the program.
                };

                using (var c = new Consumer<Ignore, string>(confC))
                {
                    c.Subscribe(kafkaTopics);

                    bool consuming = true;
                    // The client will automatically recover from non-fatal errors. You typically
                    // don't need to take any action unless an error is marked as fatal.
                    //c.OnError += (_, e) => consuming = !e.IsFatal;

                    // Raised on critical errors, e.g. connection failures or all brokers down.
                    c.OnError += (_, error)
                    => {
                        Console.WriteLine($"Kafka error occured: {error.Reason}");
                        Helpers.WriteLog($"Kafka error occured: {error.Reason}");
                        consuming = !error.IsFatal;
                    };

                    while (consuming)
                    {
                        try
                        {
                            var cr = c.Consume();
                            var p = new Producer<Null, string>(confC);
                            string resultmessage = "";
                            var Topic = cr.Topic;

                            if (cr.Value.IsValidJSON())
                            {
                                if (cr.Topic == "write_request")
                                { // Write Request
                                    kmessage = JsonConvert.DeserializeObject<KafkaMessage>(cr.Value, jsonsettings);
                                    if (kmessage.payload.accessType == "CISA" && kmessage.payload.deviceId == encoderID)
                                    { // Only Kafka messages tagged with "CISA" accessType are processed
                                        string extraSpaces = (kmessage.payload.extraSpaces != null ? string.Join(",", kmessage.payload.extraSpaces) : null);
                                        string groups = (kmessage.payload.groups != null ? string.Join(",", kmessage.payload.groups) : null);

                                        Console.WriteLine($"Code: '{kmessage.code}' Guest/Staff: '{kmessage.payload.accessId}' Room: '{kmessage.payload.room}' Zone: '{kmessage.payload.zone}' ExtraSpaces: '{extraSpaces}' Groups: '{groups}' at: '{cr.TopicPartitionOffset}'.");
                                        Helpers.WriteLog($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");

                                        CISAResult result = CISACheck(kmessage, NFCDevice);
                                        if (result.rc == Csemks32.CSE_SUCCESS)
                                        {
                                            resultmessage = "{\"code\":\"write_result\",\"payload\":{\"requestId\":" + kmessage.payload.id + "}}";
                                        }
                                        else
                                        {
                                            resultmessage = "{\"code\":\"write_result\",\"payload\":{\"requestId\":" + kmessage.payload.id + ",\"error\":" + result.errordesc.ToString() + "}}";
                                        }

                                        p.BeginProduce("write_result", new Message<Null, string> { Value = resultmessage }, handler);
                                        p.Flush(TimeSpan.FromSeconds(1));

                                        Helpers.WriteLog($"Response '{resultmessage}' sent to write_result topic.");
                                        Console.WriteLine($"Response '{resultmessage}' sent to write_result topic.");
                                    }
                                }

                                if(cr.Topic == "card_readers_request")
                                { // Card Reader Request
                                    kmessage = JsonConvert.DeserializeObject<KafkaMessage>(cr.Value, jsonsettings);
                                    if (kmessage.payload.id != null)
                                    {
                                        resultmessage = "{\"code\":\"card_readers_response\",\"payload\":{\"id\":" + kmessage.payload.id + ",\"deviceId\":" + encoderID + ",\"id\":\"CISA\"}}";
                                        p.BeginProduce("card_readers_response", new Message<Null, string> { Value = resultmessage }, handler);
                                        p.Flush(TimeSpan.FromSeconds(1));

                                        Helpers.WriteLog($"Response '{resultmessage}' sent to card_readers_response topic.");
                                        Console.WriteLine($"Response '{resultmessage}' sent to card_readers_response topic.");
                                    }
                                }
                            } else
                            {
                                Console.WriteLine($"ERROR Invalid JSON format - Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                                Helpers.WriteLog($"ERROR Invalid JSON format - Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                            Helpers.WriteLog($"Error occured: {e.Error.Reason}");
                        }
                    }

                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
                //KAFKA

        }

        public static CISAResult CISACheck (KafkaMessage kmessage, NFC NFCDevice)
        {
            /* CISA DLL Testing  */
            CISAResult result = new CISAResult();        
            short rc = 0;

            byte[] bufCard = new byte[400];  // 221
            byte[] bufCardWavemode = new byte[400]; // 228

            Csemks32.csdate csdateNow;
            Csemks32.csdate csdateEnd;

            Csemks32.cstime cstimeNow;
            Csemks32.cstime cstimeEnd;

            int secNow;

            csdateNow.day_Renamed = 0;
            csdateNow.month_Renamed = 0;
            csdateNow.year_Renamed = 0;

            cstimeNow.hours = 0;
            cstimeNow.minutes = 0;

            string serialnumberC2B = string.Format("{0,7}", ""); // 8 Chars fixed length string
            serialnumberC2B = vbHelper.Hex2Bin(CardUID + "C15A13FF");

            /* Calling CSEReadDateTime */
            rc = CSEReadDateTime(ref csdateNow, ref cstimeNow, out secNow);

            if (rc == Csemks32.CSE_SUCCESS)
            {
                Console.WriteLine("CSEReadDateTime OK");
                Console.WriteLine("Now is {0}/{1}/{2} {3}:{4}:{5}", csdateNow.day_Renamed.ToString("D2"), csdateNow.month_Renamed.ToString("D2"), csdateNow.year_Renamed.YearCisa(), cstimeNow.hours.ToString("D2"), cstimeNow.minutes.ToString("D2"), secNow.ToString("D2"));
            }
            else
            {
                result.rc = CSELoadErrNo();
                result.errordesc = "\"CSEReadDateTime - ErrNo " + result.rc + "\"";
                Console.WriteLine("CSEReadDateTime Failed");
                Console.WriteLine("ErrNo: " + result.rc.ToString());

                return result;
            }

            /* Calling CSECard2Buffer */
            // 6 Chars fixed length strings
            string accesstname1 = string.Format("{0,6}", (kmessage.payload.room != null) ? kmessage.payload.room : kmessage.payload.zone); // room/zone - first access target name
            string accesstname2 = string.Format("{0,6}", (kmessage.payload.extraSpaces != null && kmessage.payload.extraSpaces.Count > 0) ? kmessage.payload.extraSpaces[0] : ""); // extraSpaces - second access target name
            string accesstname3 = string.Format("{0,6}", (kmessage.payload.extraSpaces != null && kmessage.payload.extraSpaces.Count > 1) ? kmessage.payload.extraSpaces[1] : ""); // extraSpaces - third access target name
            string accesstname4 = string.Format("{0,6}", (kmessage.payload.extraSpaces != null && kmessage.payload.extraSpaces.Count > 2) ? kmessage.payload.extraSpaces[2] : ""); // extraSpaces - fourth access target name
            string accesstname5 = string.Format("{0,6}", (kmessage.payload.groups != null && kmessage.payload.groups.Count > 0) ? kmessage.payload.groups[0] : ""); // groups - category name

            Csemks32.card guestcard = new Csemks32.card();

            if (kmessage.payload.room != null)
            {
                guestcard.cardtype = 0; // 0 - Guest Card
            } else
            {
                guestcard.cardtype = 4; // 4 - Staff Card
            }

            // Provisional data for testing
            csdateEnd.year_Renamed = csdateNow.year_Renamed;
            csdateEnd.month_Renamed = csdateNow.month_Renamed;
            csdateEnd.day_Renamed = 25;


            guestcard.accessid = kmessage.payload.accessId; // Guest/Employee name
            guestcard.accesstime_Renamed.dateStart = csdateNow; // Checkin date
            guestcard.accesstime_Renamed.dateEnd = csdateEnd; // Checkout date
            guestcard.accesstime_Renamed.timeStart = cstimeNow; // checkin time
            guestcard.accesstime_Renamed.timeEnd.hours = 23; // checkout hour
            guestcard.accesstime_Renamed.timeEnd.minutes = 59; // checkout minutes
            guestcard.credits = 0; // credits

            guestcard.cardinfo_Renamed.icopy = 1;
            guestcard.cardinfo_Renamed.ncopy = 1;
            guestcard.fOvrPrivacy = 1;
            guestcard.fOffice = 0;
            guestcard.fCardHistory = 0;
            guestcard.nhoursvalid = 0;


            
            if (guestcard.cardtype == 4) { // Staff's Card

                /*
                guestcard.accesstime_Renamed.Initialize();
                
                guestcard.accesstime_Renamed.mtimeshift_Renamed[0] = new Csemks32.mtimeshift();
                guestcard.accesstime_Renamed.mtimeshift_Renamed[1] = new Csemks32.mtimeshift();
                guestcard.accesstime_Renamed.mtimeshift_Renamed[2] = new Csemks32.mtimeshift();
                guestcard.accesstime_Renamed.mtimeshift_Renamed[3] = new Csemks32.mtimeshift();

                
                guestcard.accesstime_Renamed.mtimeshift_Renamed[0].start1 = 0;
                guestcard.accesstime_Renamed.mtimeshift_Renamed[0].start2 = 0;
                guestcard.accesstime_Renamed.mtimeshift_Renamed[0].end1 = 95;
                guestcard.accesstime_Renamed.mtimeshift_Renamed[0].end2 = 0;
                guestcard.accesstime_Renamed.mtimeshift_Renamed[0].days = 255;
                */

            }

            rc = CSECard2Buffer(ref guestcard,
                                        String.Format(accesstname1, "!@@@@@@"),
                                        String.Format(accesstname2, "!@@@@@@"),
                                        String.Format(accesstname3, "!@@@@@@"),
                                        String.Format(accesstname4, "!@@@@@@"),
                                        String.Format(accesstname5, "!@@@@@@"),
                                        bufCard);

            if (rc == Csemks32.CSE_SUCCESS)
            {
                Console.WriteLine("CSECard2Buffer OK");
                Console.WriteLine(vbHelper.Ascii2Hex(bufCard));
            }
            else
            {
                result.rc = CSELoadErrNo();
                result.errordesc = "\"CSECard2Buffer - ErrNo " + result.rc + "\"";
                Console.WriteLine("CSECard2Buffer Failed");
                Console.WriteLine("ErrNo: " + result.rc.ToString());

                return result;
            }

            /* Calling CSEWaveModeEncode */

            const byte IRversion = 1;

            rc = CSEWaveModeEncode(bufCard, serialnumberC2B, IRversion, bufCardWavemode);

            if (rc == Csemks32.CSE_SUCCESS)
            {
                Console.WriteLine("CSEWaveModeEncode OK");
                Console.WriteLine("* bufCardWavemode Content: " + bufCardWavemode);
                Console.WriteLine("* bufCardWavemode Lenght: " + bufCardWavemode.Length);
                Console.WriteLine("* HOTEL File Lenght: " + vbHelper.MidVB(vbHelper.Ascii2Hex(bufCardWavemode), 1, 32 * 2).Length);
                Console.WriteLine("* HOTEL FILE: " + vbHelper.MidVB(vbHelper.Ascii2Hex(bufCardWavemode), 1, 32 * 2));
                Console.WriteLine("* HOTEL FILE: " + Helpers.Mid(bufCardWavemode.Ascii2Hex(), 1, 32 * 2));
                Console.WriteLine("* KEYPLAN File Lenght: " + vbHelper.MidVB(vbHelper.Ascii2Hex(bufCardWavemode), 32 * 2 + 1, 188 * 2).Length);
                Console.WriteLine("* KEYPLAN FILE: " + vbHelper.MidVB(vbHelper.Ascii2Hex(bufCardWavemode), 32 * 2 + 1, 188 * 2));
                Console.WriteLine("* KEYPLAN FILE: " + Helpers.Mid(bufCardWavemode.Ascii2Hex(), 32 * 2 + 1, 188 * 2));

                if (NFCDevice.connectCard())// establish connection to the card: you've declared this from previous post
                {
                    Console.WriteLine("Writing to Card ...");
                    NFCDevice.writeHotelFile(vbHelper.MidVB(vbHelper.Ascii2Hex(bufCardWavemode), 1, 36 * 2)); // Write Hotel File - 36 bytes
                    NFCDevice.writeKeyplanFile(vbHelper.MidVB(vbHelper.Ascii2Hex(bufCardWavemode), 36 * 2 + 1, 192 * 2)); // Write Keyplan File - 192 bytes
                    NFCDevice.Close();                    
                } else
                {
                    Console.WriteLine("Card not availale.");
                    result.rc = -1;
                    result.errordesc = "\"Writing card failed - Card not availale.\"";

                    return result;
                }
                
            }
            else
            {
                result.rc = CSELoadErrNo();
                result.errordesc = "\"CSEWaveModeEncode Failed - ErrNo " + result.rc + "\"";
                Console.WriteLine("CSEWaveModeEncode Failed");
                Console.WriteLine("ErrNo: " + result.rc.ToString());

                return result;
            }

            return result;
        }
    }
}
