using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ComputerMonitor.Helper
{
    class IniFile
    {
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
}
