﻿using System;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Timers;
using static IPCheck.Helper;

namespace IPCheck
{
    public partial class IPCheck : ServiceBase
    {
        Timer timer = new Timer();         

        public static String LocalConfigFolderHelper = AppDomain.CurrentDomain.BaseDirectory;        
        public String LastKnownIP;
        public bool Debug = false;

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
            LastKnownIP = IniFileManager.IniReadValue("Address", "PublicIP");
            if (Debug)
                WriteToFile("[" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "] START");
            GettingIPAddress(null, null);
        }
        protected override void OnStop()
        {       
            if (Debug)
                WriteToFile("[" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "] Service STOP");
        }


        private void GettingIPAddress(object source, ElapsedEventArgs e)
        {
            String userName;
            String postAddress;
            String example;
            String timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

            WebClient webc = new WebClient();
            String webData = webc.DownloadString("http://checkip.dyndns.org/");
            String ipaddress = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(webData)[0].ToString();

            if (LastKnownIP != ipaddress)
            {
                LastKnownIP = ipaddress;
                IniFileManager.IniWriteValue("Address", "PublicIP", ipaddress);
            }

            userName = IniFileManager.IniReadValue("General", "User");
            if (userName == "")
            {
                IniFileManager.IniWriteValue("General", "User", "");
            }

            postAddress = IniFileManager.IniReadValue("General", "PostAddress");
            if (postAddress == "")
            {
                IniFileManager.IniWriteValue("General", "PostAddress", "");
            }

            example = IniFileManager.IniReadValue("Examples", "PostAddress");
            if (example == "")
            {
                IniFileManager.IniWriteValue("Examples", "User", "Carlos");
                IniFileManager.IniWriteValue("Examples", "PostAddress", "http://ccdl.co/ipaddress.php");
                IniFileManager.IniWriteValue("Examples", "-", "Final link will look like: <PostAddress>?ip=<IP>&user=<User>&timestamp=<posttimestamp>");
            }

            String debugStr = IniFileManager.IniReadValue("General", "Debug");
            if (debugStr == "")
            {
                IniFileManager.IniWriteValue("General", "Debug", "False");
            }

            if (Debug)
                WriteToFile("[" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "] TASK perform");
                           

            try
            {
                // sending ip + username to the selected address
                if (userName != "" && postAddress != "")
                {
                    using (var client = new WebClient())
                    {                        
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var data = "";
                        var result = client.UploadString(postAddress + "?ip=" + LastKnownIP + "&user=" + userName + "&timestamp=" + timestamp, "POST", data);
                        if (Debug)
                        {
                            WriteToFile("[" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "] POST to webaddress");
                            WriteToFile("[" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "] " + result);
                        }
                            
                    }
                }   
            }
            catch (Exception ex) 
            {
                if (Debug)
                {
                    WriteToFile("link:" + postAddress + "?ip=" + LastKnownIP + "&user=" + userName + "&timestamp=" + timestamp);                    
                    WriteToFile("[" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "] ERROR - " + ex.Message.ToString());
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