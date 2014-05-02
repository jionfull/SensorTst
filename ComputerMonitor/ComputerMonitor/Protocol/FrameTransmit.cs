using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerMonitor.Protocol
{
    class FrameTransmit
    {
       
        private static byte[] txBuffer=new byte[64*1024];
        /// <summary>
        /// 从串口发送数据
        /// </summary>
        /// <param name="srcAddr">源地址</param>
        /// <param name="destAddr">目的地址</param>
        /// <param name="cmd">命令</param>
        /// <param name="data">数据</param>
        /// <param name="count">数据长度</param>
        public static void Send(int srcAddr, int destAddr, int cmd, byte[] data, int count)
        {
            int txIndex=0;
            int checkSum=0;
            txBuffer[0] = 0x10;
            txBuffer[1] = 0x02;
            txIndex=2;
            //长度部分
            int length = 5 + count;
            
            byte tmp=(byte)(length & 0xff);
            checkSum += tmp;
            if(tmp==0x10)
            {
                txBuffer[txIndex]=0x10;
                txIndex++;
                txBuffer[txIndex]=0x10;
                txIndex++;
            }
            else
            {
                txBuffer[txIndex]=tmp;
                txIndex++;
            }
            tmp = (byte)((length >> 8) & 0xff);
            checkSum += tmp;
            if (tmp == 0x10)
            {
                txBuffer[txIndex] = 0x10;
                txIndex++;
                txBuffer[txIndex] = 0x10;
                txIndex++;
            }
            else
            {
                txBuffer[txIndex] = tmp;
                txIndex++;
            }
            //源地址
            checkSum += srcAddr;
            tmp = (byte)(srcAddr);
            if (tmp == 0x10)
            {
                txBuffer[txIndex] = 0x10;
                txIndex++;
                txBuffer[txIndex] = 0x10;
                txIndex++;
            }
            else
            {
                txBuffer[txIndex] = tmp;
                txIndex++;
            }
            //目的地址
            checkSum += destAddr;
            tmp = (byte)(destAddr);
            if (tmp == 0x10)
            {
                txBuffer[txIndex] = 0x10;
                txIndex++;
                txBuffer[txIndex] = 0x10;
                txIndex++;
            }
            else
            {
                txBuffer[txIndex] = tmp;
                txIndex++;
            }

            //命令
            checkSum += cmd;
            tmp = (byte)(cmd);
            if (tmp == 0x10)
            {
                txBuffer[txIndex] = 0x10;
                txIndex++;
                txBuffer[txIndex] = 0x10;
                txIndex++;
            }
            else
            {
                txBuffer[txIndex] = tmp;
                txIndex++;
            }

            //数据部分
            for (int i = 0; i < count; i++)
            {
                tmp = (byte)(data[i]);
                checkSum += data[i];
                if (tmp == 0x10)
                {
                    txBuffer[txIndex] = 0x10;
                    txIndex++;
                    txBuffer[txIndex] = 0x10;
                    txIndex++;
                }
                else
                {
                    txBuffer[txIndex] = tmp;
                    txIndex++;
                }
            }

            //校验部分
            tmp = (byte)(checkSum & 0xff);
            if (tmp == 0x10)
            {
                txBuffer[txIndex] = 0x10;
                txIndex++;
                txBuffer[txIndex] = 0x10;
                txIndex++;
            }
            else
            {
                txBuffer[txIndex] = tmp;
                txIndex++;
            }
            tmp = (byte)((checkSum >> 8) & 0xff);
            if (tmp == 0x10)
            {
                txBuffer[txIndex] = 0x10;
                txIndex++;
                txBuffer[txIndex] = 0x10;
                txIndex++;
            }
            else
            {
                txBuffer[txIndex] = tmp;
                txIndex++;
            }

            // 数据帧尾
            txBuffer[txIndex] = 0x10;
            txIndex++;
            txBuffer[txIndex] = 0x03;
            txIndex++;


            SerialManager.CommPort.WriteData(txBuffer, 0, txIndex);


        }
    }
  
}
