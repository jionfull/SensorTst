using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerMonitor.Helper
{
    
    class GlobalVar
    {
        public static event EventHandler SendData = null;
        private static StringBuilder sb = new StringBuilder();
        public static void ShowSendData(byte[] data,int offset,int count)
        {
            if (SendData != null)
            {
                sb.Length = 0;
                for (int i = 0; i < count; i++)
                {
                    sb.Append(data[offset + i].ToString("X2") + " ");
                }
                sb.Append("\r\n");
                SendData(sb.ToString(), null);
            }
        }
    }
}
