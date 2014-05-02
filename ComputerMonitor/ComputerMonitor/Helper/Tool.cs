using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerMonitor.Helper
{
    class Tool
    {
        public static int FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            str = str.ToUpper();
            if (str.StartsWith("0X")||str.EndsWith("H"))
            {
                str = str.TrimEnd(new char[] { 'H' });
                str = str.TrimStart(new char[] { '0' });
                str = str.TrimStart(new char[] { 'X' });
                if (str == "")
                {
                    return 0;
                }
                else
                {
                    return int.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
                }
            }

            return int.Parse(str);
        }
    }
}
