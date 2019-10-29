using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CisaNet.Entity;

namespace CISAKafkaConnector
{
    public static class Helpers
    {
        // Check if a string is a valid JSON object
        public static bool IsValidJSON(this string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Write entrty to Log file
        public static void WriteLog(string strLog)
        {
            string m_exePath = string.Empty;
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string logFilePath = m_exePath + "\\" + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            FileInfo logFileInfo = new FileInfo(logFilePath);
            DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
            {
                using (StreamWriter log = new StreamWriter(fileStream))
                {
                    log.Write("\r\n");
                    log.WriteLine("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToLongTimeString());
                    log.WriteLine("{0}", strLog);
                    log.WriteLine("-------------------------------");
                }
            }
        }

        // Return Substring
        public static string Mid(string s, int a, int b)

        {

            string temp = s.Substring(a - 1, b);

            return temp;

        }

        // Convert Hex string to ASCII
        public static string Hex2Ascii(this string hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }

        // Convert ASCII string to Hex
        public static string Ascii2Hex(this byte[] bytes)
        {
            var builder = new StringBuilder();

            var hexCharacters = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            for (var i = 0; i < bytes.Length; i++)
            {
                int firstValue = (bytes[i] >> 4) & 0x0F;
                int secondValue = bytes[i] & 0x0F;

                char firstCharacter = hexCharacters[firstValue];
                char secondCharacter = hexCharacters[secondValue];

                //builder.Append("0x");
                builder.Append(firstCharacter);
                builder.Append(secondCharacter);
                //builder.Append(' ');
            }

            return builder.ToString().Trim(' ');
        }

        // Convert HEX string to Byte of Hex Values
        public static byte[] Hex2Byte(this string hexstring)
        {
            byte[] data = new byte[hexstring.Length / 2];

            for (int i = 0; i < hexstring.Length; i += 2)
            {
                data[i / 2] = Convert.ToByte(hexstring.Substring(i, 2), 16);
            }

            return data;
        }
        // Adjust CISA CSEReadDateTime date year value
        public static int YearCisa(this byte cvyear)
        {
            int cvyearint;
            cvyearint = cvyear + 88;
            if (cvyearint >= 100)
                cvyearint = cvyearint - 100;
            return cvyearint;
        }

        public static int YearPC2Cisa(int cvyear)
        {
            int cvyearint;
            cvyearint = cvyear;
            if (cvyearint <= 51)
                cvyearint = cvyearint + 100;
            return cvyearint - 88;
        }

        // EPOCH to Date
        public static DateTime epoch2date (double epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime().AddSeconds(epoch);
        }

        // Date to EPOCH        
        public static string date2epoch (DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds).ToString();
        }

        public static DateTime csdt2Dt(Csemks32.csdate cdate, Csemks32.cstime ctime)
        {
            DateTime DT = new DateTime(cdate.year_Renamed, cdate.month_Renamed, cdate.day_Renamed, ctime.hours, ctime.minutes, 0);
            return DT;
        }

        // Convert char[]  to string
        public static string char2String(char[] chr)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < chr.Length; i++)
                {
                    char hs = chr[i];
                    if (hs == 0) break;
                    ascii += hs;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }

    }
}
