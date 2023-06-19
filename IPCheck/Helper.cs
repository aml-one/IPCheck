using System.Runtime.InteropServices;
using System.Text;
using static IPCheck.IPCheck;

namespace IPCheck
{
    internal class Helper
    {
        //
        // ini file read/write class
        //
        public static class IniFileManager
        {


            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section,
                string key, string val, string filePath);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section,
                     string key, string def, StringBuilder retVal,
                int size, string filePath);
            [DllImport("kernel32.dll")]
            private static extern int GetPrivateProfileSection(string lpAppName,
                     byte[] lpszReturnBuffer, int nSize, string lpFileName);

            public static void IniWriteValue(string Section, string Key, string Value)
            {
                WritePrivateProfileString(Section, Key, Value, LocalConfigFolderHelper + "Config.ini");
            }

            public static string IniReadValue(string Section, string Key)
            {
                StringBuilder temp = new StringBuilder(255);                
                GetPrivateProfileString(Section, Key, "", temp, 255, LocalConfigFolderHelper + "Config.ini");
                
                return temp.ToString();
            }
        }


        //
        // END
        //
    }
}
