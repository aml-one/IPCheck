using System;
using System.IO;
using System.Web;
using System.Net;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Timers;
using static IPCheck.Helper;
using System.Linq;

namespace IPCheck
{
    public partial class IPCheck : ServiceBase
    {
        Timer timer = new Timer();         

        public static String LocalConfigFolderHelper = AppDomain.CurrentDomain.BaseDirectory;        
        public String LastKnownIP;
        public bool Debug = false;
        private static Random random = new Random();

        public IPCheck()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
            Directory.CreateDirectory(LocalConfigFolderHelper);

            timer.Elapsed += new ElapsedEventHandler(GettingIPAddress);
            timer.Interval = 120000; 
            timer.Enabled = true;

            bool.TryParse(IniFileManager.IniReadValue("General", "Debug"), out Debug);            
            if (Debug)
                WriteToFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] START");
            GettingIPAddress(null, null);
        }

        protected override void OnStop()
        {       
            if (Debug)
                WriteToFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Service STOP");
        }


        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxzy0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private void GettingIPAddress(object source, ElapsedEventArgs e)
        {
            String userName;
            String postAddress;            
            String timestamp = DateTime.UtcNow.ToString("U");
            LastKnownIP = IniFileManager.IniReadValue("Address", "PublicIP");

            postAddress = IniFileManager.IniReadValue("General", "PostAddress");
            if (postAddress == "")
            {
                IniFileManager.IniWriteValue("General", "PostAddress", "");
            }

            WebClient webc = new WebClient();
            String webData = webc.DownloadString(postAddress);
            String ipaddress = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(webData)[0].ToString();

            userName = IniFileManager.IniReadValue("General", "User");
            if (userName == "")
            {
                IniFileManager.IniWriteValue("General", "User", "");
            }
            
            String debugStr = IniFileManager.IniReadValue("General", "Debug");
            if (debugStr == "")
            {
                IniFileManager.IniWriteValue("General", "Debug", "False");
            }

            if (Debug)
                WriteToFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] TASK perform");
                           

            try
            {
                // sending ip + username to the selected address
                if (userName != "" && postAddress != "" && LastKnownIP != ipaddress)
                {
                    LastKnownIP = ipaddress;
                    IniFileManager.IniWriteValue("Address", "PublicIP", ipaddress);
                    String json = "{\"user\":\"" + userName +"\", \"ip\":\"" + ipaddress +"\", \"timestamp\":\"" + timestamp +"\"}";

                    byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(json);
                    String enc = Convert.ToBase64String(encbuff);
                    String urlenc = HttpUtility.UrlEncode(enc);

                    String dataPart1 = urlenc.Substring(0, 13);
                    String dataPart2 = urlenc.Substring(13);
                    String randomString = RandomString(13);
                    String dhash = Reverse(dataPart1 + randomString + dataPart2);

                    if (Debug)
                        WriteToFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] HASH=" + dhash);

                    using (var client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var data = "hash=" + dhash;
                        var result = client.UploadString(postAddress, "POST", data);                        

                        if (Debug)
                        {
                            WriteToFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] POST to webaddress");
                            WriteToFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] r! " + result);
                        }
                            
                    }
                }   
            }
            catch (Exception ex) 
            {
                if (Debug)
                {
                    WriteToFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ERROR - " + ex.Message.ToString());
                }                    
            }
        }


        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
