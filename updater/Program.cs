using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace ServiceInstaller
{
    internal class Program
    {
        static System.Timers.Timer timer = new System.Timers.Timer();
        static System.Timers.Timer timerStartService = new System.Timers.Timer();

        static void Main(string[] args)
        {
            Console.WriteLine("#################################");
            Console.WriteLine("###           IPCheck         ###");
            Console.WriteLine("##           Installer         ##");
            Console.WriteLine("##             v1.0            ##");
            Console.WriteLine("###       Made by Ambrus L.   ###");
            Console.WriteLine("#################################");


            Directory.CreateDirectory("C:\\IPCheck\\");

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "curl.exe";
            startInfo.Arguments = "-o C:\\IPCheck\\IPCheck.exe https://ipcheck.aml.one/IPCheck.exe";
            process.StartInfo = startInfo;
            process.Start();

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;

            timerStartService.Elapsed += new ElapsedEventHandler(StartService);
            timerStartService.Interval = 6000; //number in milisecinds  
            timerStartService.Enabled = false;

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" - Downloading necessary files..");
            Console.WriteLine("          (please wait)");
            Console.WriteLine("");
            Console.ReadLine();
        }

        private static void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe";
            startInfo.Arguments = " C:\\IPCheck\\IPCheck.exe";
            process.StartInfo = startInfo;
            process.Start();
            Console.WriteLine("###################################");
            Console.WriteLine(" - Installing service..");
            Console.WriteLine("###################################");
            Console.WriteLine("");
            Console.WriteLine("");


            startInfo.FileName = "attrib.exe";
            startInfo.Arguments = "+h C:\\IPCheck";
            process.StartInfo = startInfo;
            process.Start();

            timerStartService.Enabled = true;

            Console.WriteLine("");
            Console.WriteLine("###################################");
            Console.WriteLine(" - Firing up service..");
            Console.WriteLine("###################################");
            Console.WriteLine("");
        }

        private static void StartService(object source, ElapsedEventArgs e)
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = "IPCheck";
            sc.Start();
            sc.WaitForStatus(ServiceControllerStatus.Running);

            Console.WriteLine("");
            Console.WriteLine("###################################");
            Console.WriteLine(" - Installation complete ^^");
            Console.WriteLine("###################################");
            Console.WriteLine("");            

            Console.WriteLine(" - You may close this window.. ;)");
            Console.ReadLine();
        }
    }
}
