using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ComputerMonitor.Protocol
{

    class FrameUnit
    {
        private byte[] frameData=null;
        public int frameLength = 0;
        private AutoResetEvent frameEvent = new AutoResetEvent(false);
        int cmd = -1;
        public int Command
        {
            get
            {
                return cmd;
            }
            set
            {
                cmd = value;
            }
        }

        public int FrameLength
        {
            get
            {
                return frameLength;
            }
        }
        public FrameUnit(byte[] buffer, int cmdValue)
        {
            frameData = buffer;
            cmd = cmdValue;
        }

        public int Write(byte[] buffer, int count)
        {

            if (cmd < 0 || buffer[6] == cmd) //匹配所有命令
            {
                if (frameData != null)
                {
                    Array.Copy(buffer, frameData, count > frameData.Length ? frameData.Length : count);
                    frameLength = count > frameData.Length ? frameData.Length : count;
                    frameEvent.Set();
                    return 0;
                }
            }
            return -1;
        }

        public int Read(byte[] buffer, int count)
        {
            if (frameData != null)
            {
                Array.Copy(frameData, buffer, count > frameData.Length ? frameData.Length : count);
                return count > frameData.Length ? frameData.Length : count;
            }
            return 0;
        }

        public bool WaitData(int time)
        {
            if(time<0)
            {
                return frameEvent.WaitOne();
            }

            return frameEvent.WaitOne(time, false);
        }
    }

    class FrameManager
    {

        private static List<FrameUnit> listFrame = new List<FrameUnit>();

        public static FrameUnit CreateFrame(byte[] buffer,int cmd)
        {
            FrameUnit frameUnit = new FrameUnit(buffer, cmd);

            lock (listFrame)
            {
                listFrame.Add(frameUnit);
            }
            return frameUnit;
        }

        public static void DeleteFrame(FrameUnit frameUnit)
        {
            lock (listFrame)
            {
                listFrame.Remove(frameUnit);
            }
        }

       // public static event EventHandler FrameReceived;

        
        public static void CheckFrame(byte[] originFrame,int frameLength)
        {
            lock (listFrame)
            {
                foreach (FrameUnit frameUnit in listFrame)
                {
                    frameUnit.Write(originFrame, frameLength);
                }
            }
        }

    }

    class Funs485
    {
        static bool ReadDebugReg(byte boardAddress,UInt16 addr,out UInt16 val)
        {
            byte[] dataBuffer=new byte[100];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0xf0); 
            bool ret;

            try
            {
                byte[] buffer = new byte[2];
        
                buffer[0] = (byte)(addr & 0xff);
                buffer[1] = (byte)(addr >> 8);

                Protocol.FrameTransmit.Send(1, boardAddress, 0xF0, buffer, 2);
                if (frameUnit.WaitData(3000) == false)
                {
                    val = 0;
                    ret = false;
                }
                else
                {
                    val = dataBuffer[10];
                    val *= 256;
                    val += dataBuffer[9];
                    ret = true;
                }


            }
            catch (ThreadAbortException)
            {
                ret = false;
                val = 0;
            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return ret;
        }
        static bool WriteDebugReg(byte boardAddr, UInt16 addr,  UInt16 val)
        {
           
            bool ret;
            byte[] rxBuf = new byte[1000];
            FrameUnit frameUnit = FrameManager.CreateFrame(rxBuf, 0xf1);
            try
            {
                byte[] buffer = new byte[4];



                buffer[0] = (byte)(addr & 0xff);
                buffer[1] = (byte)(addr >> 8);
                buffer[2] = (byte)(val & 0xff);
                buffer[3] = (byte)(val >> 8);
                Protocol.FrameTransmit.Send(1, boardAddr, 0xf1, buffer, 4);

                if (frameUnit.WaitData(3000) == false)
                {
                    ret = false;

                }
                else
                {
                    UInt16 temp;
                    temp = (UInt16)((UInt16)rxBuf[9] + (UInt16)(rxBuf[10] * 256));
                    if (temp == val)
                    {

                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
            }
            catch
            {
                ret = false;
            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);

            }
            return ret;
        }
        static  bool ReadAnalogCh(byte boardAddr,byte ch, out Int16 val)
        {
            byte[] dataBuffer = new byte[1000];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x30); //接收任何数据帧
            bool ret;

            try
            {
                byte[] buffer = new byte[1];
                buffer[0] = ch;


                Protocol.FrameTransmit.Send(1, boardAddr, 0x30, buffer, 1);
                if (frameUnit.WaitData(3000) == false)
                {
                    
                    val = 0;
                    ret = false;
                }
                else
                {
                    val = dataBuffer[9];
                    val *= 256;
                    val += dataBuffer[8];
                    ret = true;
                }


            }
            catch (ThreadAbortException)
            {
                ret = false;
                val = 0;
            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return ret;
        }
        static int GetLimitValue(byte boardStyle)
        {
            int LimitValue = 0;
            const int LimitVal1 = 80;
            const int LimitVal2 = 8192;
            const int LimitVal3 = 32768;
            const int LimitVal4 = 512;
            if (boardStyle < 3)
            {
                LimitValue = LimitVal1;
            }
            else if (boardStyle < 4 || boardStyle == 6 || boardStyle == 7)
            {

                LimitValue = LimitVal2;
            }
            else if(boardStyle==9||boardStyle==11)
            {

                LimitValue = LimitVal4;
            }
            else
            {
                LimitValue=LimitVal3;
            }
            return LimitValue;
        }
        /*
        static bool SendParamFrm( byte[] paramArray,int startIndex, int len)
        {
            
            int LimitValue;
            byte boardAddr=paramArray[0];
            LimitValue = GetLimitValue(paramArray[1]);
            byte[] dataBuffer = new byte[1000];            
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 1);
            byte[] frameBuffer = new byte[len + 7];
            try
            {
                frameBuffer[0] = (byte)(startIndex & 0xff);
                frameBuffer[1] = (byte)((startIndex >> 8) & 0xff); //帧序号
                frameBuffer[2] = paramArray[1];
                frameBuffer[3] = 0;
                frameBuffer[4] = 0;
                for (int i = 0; i < len; i++)
                {
                    frameBuffer[5 + i] = paramArray[startIndex * LimitValue + i];
                }
                ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, LimitValue + 5);
            }
            catch
            {

            }
            finally
            {
 
            }
            
        }
         * */
         
        /*
        static bool SendParam(byte boardAddr, byte[] paramAyy)
        {
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 1);
            try
            {
                if (paramLength > 0)
                {
                    int startIndex = 0;
                    while (paramLength > 0)
                    {
                        if (paramLength > LimitValue)
                        {

                            frameBuffer[0] = (byte)(startIndex & 0xff);
                            frameBuffer[1] = (byte)((startIndex >> 8) & 0xff); //帧序号
                            frameBuffer[2] = paramValue[1];
                            frameBuffer[3] = 0;
                            frameBuffer[4] = 0;
                            for (int i = 0; i < LimitValue; i++)
                            {
                                frameBuffer[5 + i] = paramValue[startIndex * LimitValue + i];
                            }
                            ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, LimitValue + 5);
                            frameBuffer[LimitValue + 5] = (byte)(crc & 0xff);
                            frameBuffer[LimitValue + 6] = (byte)((crc >> 8) & 0xff);
                            //CPU板地址为1
                            Protocol.FrameTransmit.Send(1, paramValue[0], 0x01, frameBuffer, LimitValue + 7);
                            startIndex++;
                            frameUnit.WaitData(500);
                        }
                        else
                        {
                            frameBuffer[0] = (byte)(startIndex & 0xff);
                            frameBuffer[1] = (byte)(((startIndex >> 8) & 0xff) | 0x80); //帧序号,bit15为1代表最后一帧
                            frameBuffer[2] = paramValue[1];
                            frameBuffer[3] = 0;
                            frameBuffer[4] = 0;
                            for (int i = 0; i < paramLength; i++)
                            {
                                frameBuffer[5 + i] = paramValue[startIndex * LimitValue + i];
                            }
                            ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, paramLength + 5);
                            frameBuffer[paramLength + 5] = (byte)(crc & 0xff);
                            frameBuffer[paramLength + 6] = (byte)((crc >> 8) & 0xff);
                            //CPU板地址为1
                            Protocol.FrameTransmit.Send(1, paramValue[0], 0x01, frameBuffer, paramLength + 7);
                            //发送完毕
                            startIndex++;
                            frameUnit.WaitData(500);
                        }
                        paramLength -= LimitValue;
                    }
                }
                else
                {
                    frameBuffer[0] = (byte)(0 & 0xff);
                    frameBuffer[1] = (byte)(((0 >> 8) & 0xff) | 0x80); //帧序号,bit15为1代表最后一帧
                    frameBuffer[2] = paramValue[1];
                    frameBuffer[3] = 0;
                    frameBuffer[4] = 0;
                    frameBuffer[5] = paramValue[0];
                    frameBuffer[6] = paramValue[1];
                    ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, 7);
                    frameBuffer[7] = (byte)(crc & 0xff);
                    frameBuffer[8] = (byte)((crc >> 8) & 0xff);
                    Protocol.FrameTransmit.Send(1, paramValue[0], 0x01, frameBuffer, 9);
                }
            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
        }
    */

    } 

}
