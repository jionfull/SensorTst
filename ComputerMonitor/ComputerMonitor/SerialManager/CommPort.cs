using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using ComputerMonitor.Helper;

namespace ComputerMonitor.SerialManager
{
    class CommPort
    {
        private static SerialPort serialPort = new SerialPort();
        private static AutoResetEvent serialPortEvent = new AutoResetEvent(false);

        static CommPort()
        {
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }

        static void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            serialPortEvent.Set();
        }
        public static void WaitPortData()
        {
            serialPortEvent.WaitOne();
        }
        public static bool WaitPortData(int time)
        {
            if (time < 0)
            {
                return serialPortEvent.WaitOne();
            }
            return serialPortEvent.WaitOne(time,false);
        }

        public static int ReadData(byte[] buffer)
        {
            int receiveLength = 0;
            lock (serialPort)
            {
                receiveLength = serialPort.BytesToRead;
                if (serialPort.IsOpen)
                {
                    serialPort.Read(buffer, 0, receiveLength);
                }
            }
            return receiveLength;
        }

       
        /// <summary>
        /// 将数据写入串口
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns>成功返回true,否则false</returns>
        public static bool WriteData(byte[] buffer, int offset, int count)
        {
            bool success = false;
            if (serialPort.IsOpen)
            {
                serialPort.Write(buffer, offset, count);
                GlobalVar.ShowSendData(buffer, offset, count);
                success = true;
            }
            return success;
        }

        public static string PortName
        {
            get
            {
                return serialPort.PortName;
            }
        }

        public static void OpenPort(string portName)
        {
            OpenPort(portName, 9600);
        }
        public static void OpenPort(string portName,int baudRate)
        {
            if (serialPort.IsOpen == false)
            {
                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.Open();

            }
        }

        public static bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }


        public static void ClosePort()
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception)
            {
 
            }
        }

    }
}
