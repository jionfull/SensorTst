using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerMonitor.Protocol
{
    class FrameTransmit
    {
       
        private static byte[] txBuffer=new byte[64*1024];
        /// <summary>
        /// �Ӵ��ڷ�������
        /// </summary>
        /// <param name="srcAddr">Դ��ַ</param>
        /// <param name="destAddr">Ŀ�ĵ�ַ</param>
        /// <param name="cmd">����</param>
        /// <param name="data">����</param>
        /// <param name="count">���ݳ���</param>
        public static void Send(int srcAddr, int destAddr, int cmd, byte[] data, int count)
        {
            int txIndex=0;
            int checkSum=0;
            txBuffer[0] = 0x10;
            txBuffer[1] = 0x02;
            txIndex=2;
            //���Ȳ���
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
            //Դ��ַ
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
            //Ŀ�ĵ�ַ
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

            //����
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

            //���ݲ���
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

            //У�鲿��
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

            // ����֡β
            txBuffer[txIndex] = 0x10;
            txIndex++;
            txBuffer[txIndex] = 0x03;
            txIndex++;


            SerialManager.CommPort.WriteData(txBuffer, 0, txIndex);


        }
    }
  
}
