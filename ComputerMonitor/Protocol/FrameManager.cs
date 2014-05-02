using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ComputerMonitor.Protocol
{

    class FrameUnit
    {
        private byte[] frameData = null;
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
            if (time < 0)
            {
                return frameEvent.WaitOne();
            }

            return frameEvent.WaitOne(time, false);
        }
    }

    class FrameManager
    {

        private static List<FrameUnit> listFrame = new List<FrameUnit>();

        public static FrameUnit CreateFrame(byte[] buffer, int cmd)
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


        public static void CheckFrame(byte[] originFrame, int frameLength)
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


    public class ParamCh
    {
        byte style;
        byte upDataNum;
        byte refBrdNo1;
        byte refChNo1;
        byte refBrdNo2;
        byte refChNo2;
        Int16 otherParam;
        public ParamCh(byte style)
        {
            this.style = style;
            this.upDataNum = styLenTable[style];
            refBrdNo1 = 0xff;
            refChNo1 = 0xff;
            refBrdNo2 = 0xff;
            refChNo2 = 0xff;
            otherParam = 0x00;
        }
        public byte[] ToByteAry()
        {
            byte[] ret = new byte[8];
            ret[0] = style;
            ret[1] = upDataNum;
            ret[2] = refBrdNo1;
            ret[3] = refChNo1;
            ret[4] = refBrdNo2;
            ret[5] = refChNo2;
            ret[6] = (byte)((otherParam >> 8) & 0xff);
            ret[6] = (byte)(otherParam & 0xff);
            return ret;
        }

        public byte[] styLenTable ={
											//	0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F,//行号
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x00
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x2b,0x2b,0x02,0x02,0x02,0x02,//0x01
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x04,0x02,0x06,0x02,0x02,//0x02
												0x02,0x06,0x06,0x06,0x06,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x03
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x04
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x05
												0x06,0x06,0x06,0x06,0x06,0x06,0x06,0x06,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x06
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x07
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x08
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x09
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x0A
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x0B
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x0C
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x0D
												0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x0E
												0x00,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,0x02,//0x0F

                                                };
    }
    public class Board
    {
        byte id;
        byte style;
        byte chNum;//通道数量
        byte rvse1;

        ParamCh[] chAry = new ParamCh[96];
        public Board(byte brdNum, byte style)
        {
            id = brdNum;
            this.style = style;
            chNum = 0;
            rvse1 = 0;
        }
        public bool AddCh(ParamCh ch)
        {
            chAry[chNum++] = ch;
            return true;
        }
        public byte[] ToByteAry()
        {
            byte[] ret = new byte[chNum * 8 + 4];
            byte[] temp;
            ret[0] = id;
            ret[1] = style;
            ret[2] = chNum;
            ret[3] = rvse1;
            for (int i = 0; i < chNum; i++)
            {
                temp = chAry[i].ToByteAry();
                for (int j = 0; j < 8; j++)
                {
                    ret[4 + i * 8 + j] = temp[j];
                }
            }
            return ret;
        }
    }
    class Funs485
    {
        static public bool ReadDebugReg(byte boardAddress, UInt16 addr, out UInt16 val)
        {
            byte[] dataBuffer = new byte[100];
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
            Thread.Sleep(20);
            return ret;
        }
        static public bool Rst(byte boardAddr)
        {
            byte[] buffer = new byte[1];
            Protocol.FrameTransmit.Send(1, boardAddr, 0x0f, buffer, 0);
            Thread.Sleep(1000);
            return true;
        }
        static public bool WriteDebugReg(byte boardAddr, UInt16 addr, UInt16 val)
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
            Thread.Sleep(100);
            return ret;
        }
        static public bool ReadAnalogCh(byte boardAddr, byte ch, out Int16 val)
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
        static public bool ReadAnalogCh(byte boardAddr, byte ch, out Int16 val, out float freq, out float lowFreq)
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
                    freq = 0;
                    lowFreq = 0;
                }
                else
                {
                    val = dataBuffer[9];
                    val *= 256;
                    val += dataBuffer[8];
                    ret = true;
                    freq = dataBuffer[11];
                    freq *= 256;
                    freq += dataBuffer[10];
                    freq *= 0.1f;
                    lowFreq = dataBuffer[13];
                    lowFreq *= 256;
                    lowFreq += dataBuffer[12];
                    lowFreq *= 0.1f;
                }


            }
            catch (ThreadAbortException)
            {
                ret = false;
                val = 0;
                freq = 0;
                lowFreq = 0;
            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return ret;
        }
        static public int GetLimitValue(byte boardStyle)
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
            else if (boardStyle == 9 || boardStyle == 11)
            {

                LimitValue = LimitVal4;
            }
            else
            {
                LimitValue = LimitVal3;
            }
            return LimitValue;
        }

        static public bool ReadChVal(byte cardNo, byte chNo, int timeOut, out UInt16 chVal)
        {
            byte[] dataBuffer = new byte[4000];
            FrameUnit chListen;
            chListen = FrameManager.CreateFrame(dataBuffer, 0x30);
            StringBuilder sb = new StringBuilder();
            bool ret;

            try
            {
                byte[] buffer = new byte[1];
                buffer[0] = chNo;

                Protocol.FrameTransmit.Send(1, cardNo, 0x30, buffer, 1);
                if (chListen.WaitData(timeOut))
                {
                    chVal = (UInt16)(dataBuffer[8] + dataBuffer[9] * 256);

                    ret = true;
                }
                else
                {
                    chVal = 0;
                    ret = false;
                }



            }
            catch
            {
                chVal = 0;
                ret = false;
            }
            finally
            {
                FrameManager.DeleteFrame(chListen);
            }
            return ret;
        }

        static public bool UnLock(byte boardAddr)
        {
            if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0x55aa) == false)
            {
                return false;
            }
            if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0xaa55) == false)
            {
                return false;
            }
            return true;
        }

        static public bool WrMessage(byte boardAddr, String str)
        {

            // StringBuilder sb = new StringBuilder();
            UInt16 val;
           
            UInt16 addr = 0;
            Encoding coding = ASCIIEncoding.GetEncoding("GB2312");
            byte[] assiiBytes = coding.GetBytes(str);


            addr = 0x0100;
            for (int i = 0; i * 2 < (assiiBytes.Length + 1) && i < 0x255; i++)
            {
                addr = (UInt16)(0x100 + i);


                if (i * 2 + 1 < assiiBytes.Length)
                {
                    val = (UInt16)(assiiBytes[i * 2] + (assiiBytes[i * 2 + 1] << 8));

                }
                else
                {
                    if (i * 2 == assiiBytes.Length)
                    {
                        val = 0;
                    }
                    else
                    {
                        val = assiiBytes[i * 2];

                    }
                }
                if (WriteDebugReg(boardAddr, addr, val) == false)
                {
                    return false;
                }

            }
            return true;

        }

        static public bool RdMessage(byte boardAddr, out String str)
        {
            UInt16 addr;
            StringBuilder sb = new StringBuilder();
            byte[] rxBuf = new byte[512];
            UInt16 tempVal;
            bool retVal = true;
            Encoding coding = ASCIIEncoding.GetEncoding("GB2312");
            str = null;
            int i=0;

            for ( i = 0; i < 255; i++)
            {
                addr = (UInt16)(0x100 + i);
                if (ReadDebugReg(boardAddr, addr,out tempVal))
                {
                    if (tempVal != 0xffff&&tempVal!=0)
                    {
                        rxBuf[i * 2] = (byte)(tempVal & 0xff);
                        rxBuf[i * 2 + 1] = (byte)(tempVal >> 8);
                    }
                    else
                    {
                       
                        break;
                    }
                }
                else
                {
                    retVal = true;
                }
            }
            #region 移除字符串中间“\0”
            for (int k = 0; k < rxBuf.Length-1; k++)
            {
                if (rxBuf[k] == 0x00)
                {
                    for (int j = k; j < rxBuf.Length - 1; j++)
                    {
                        rxBuf[j] = rxBuf[j + 1];
                    }
                }
            }
            #endregion
            str = coding.GetString(rxBuf, 0, i * 2);
            return retVal;


        }

        static public bool SendParam(byte[] paramAry)
        {
            bool ret = true;
            byte[] dataBuffer = new byte[40000];
            byte[] frameBuffer = new byte[40000];
            int LimitValue, paramLength;
            byte boardAddr = paramAry[0];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 1);
            LimitValue = GetLimitValue(paramAry[1]);
            try
            {
                paramLength = paramAry.Length;
                if (paramAry.Length > 0)
                {
                    int startIndex = 0;
                    while (paramLength > 0)
                    {
                        if (paramLength > LimitValue)
                        {

                            frameBuffer[0] = (byte)(startIndex & 0xff);
                            frameBuffer[1] = (byte)((startIndex >> 8) & 0xff); //帧序号
                            frameBuffer[2] = paramAry[1];
                            frameBuffer[3] = 0;
                            frameBuffer[4] = 0;
                            for (int i = 0; i < LimitValue; i++)
                            {
                                frameBuffer[5 + i] = paramAry[startIndex * LimitValue + i];
                            }
                            ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, LimitValue + 5);
                            frameBuffer[LimitValue + 5] = (byte)(crc & 0xff);
                            frameBuffer[LimitValue + 6] = (byte)((crc >> 8) & 0xff);
                            //CPU板地址为1
                            Protocol.FrameTransmit.Send(1, paramAry[0], 0x01, frameBuffer, LimitValue + 7);
                            startIndex++;
                            if (frameUnit.WaitData(500) == false)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            frameBuffer[0] = (byte)(startIndex & 0xff);
                            frameBuffer[1] = (byte)(((startIndex >> 8) & 0xff) | 0x80); //帧序号,bit15为1代表最后一帧
                            frameBuffer[2] = paramAry[1];
                            frameBuffer[3] = 0;
                            frameBuffer[4] = 0;
                            for (int i = 0; i < paramLength; i++)
                            {
                                frameBuffer[5 + i] = paramAry[startIndex * LimitValue + i];
                            }
                            ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, paramLength + 5);
                            frameBuffer[paramLength + 5] = (byte)(crc & 0xff);
                            frameBuffer[paramLength + 6] = (byte)((crc >> 8) & 0xff);
                            //CPU板地址为1
                            Protocol.FrameTransmit.Send(1, paramAry[0], 0x01, frameBuffer, paramLength + 7);
                            //发送完毕
                            startIndex++;
                            if (frameUnit.WaitData(500) == false)
                            {
                                return false;
                            }
                        }
                        paramLength -= LimitValue;
                    }
                }
                else
                {
                    frameBuffer[0] = (byte)(0 & 0xff);
                    frameBuffer[1] = (byte)(((0 >> 8) & 0xff) | 0x80); //帧序号,bit15为1代表最后一帧
                    frameBuffer[2] = paramAry[1];
                    frameBuffer[3] = 0;
                    frameBuffer[4] = 0;
                    frameBuffer[5] = paramAry[0];
                    frameBuffer[6] = paramAry[1];
                    ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, 7);
                    frameBuffer[7] = (byte)(crc & 0xff);
                    frameBuffer[8] = (byte)((crc >> 8) & 0xff);
                    Protocol.FrameTransmit.Send(1, paramAry[0], 0x01, frameBuffer, 9);
                    if (frameUnit.WaitData(500) == false)
                    {
                        return false;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                ret = false;
            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return ret;

        }

        static public bool StartBoard(byte addr)
        {
            byte[] buffer = new byte[1];

            Protocol.FrameTransmit.Send(1, addr, 5, buffer, 0);
            Thread.Sleep(1000);
            return true;
        }
        static public  bool RdAllCh(byte addr, out Int16[] chValues)
        {
           byte[] dataBuffer = new byte[4000];
           FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x30);
           byte[] buffer = new byte[1];
           buffer[0] = 0xff;
           Protocol.FrameTransmit.Send(0x01, addr, 0x30, buffer, 1);
           chValues = null;
           try
           {
                   if (frameUnit.WaitData(3000) == false)
                   {
                        return false;
                   }
                   else
                   {
                       if (frameUnit.FrameLength < 20) return false;
                       chValues = new Int16[(frameUnit.FrameLength - 4 - 8)/2];
                       for (int i = 8; i < frameUnit.FrameLength - 4; i += 2)
                       {  
                           chValues[(i-8)/2] = (Int16)(dataBuffer[i] + dataBuffer[i + 1] * 256);
                       }
                       return true;

                   }

         

           }
           catch (ThreadAbortException)
           {

           }
           finally
           {
               FrameManager.DeleteFrame(frameUnit);
           }
            return false;
        }


        internal static bool RdScaleList(byte cardAddr,out Int16[] chValues)
        {
            byte[] dataBuffer = new byte[4000];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0xE4);
            byte[] buffer = new byte[1];
            buffer[0] = 0x52;
            Protocol.FrameTransmit.Send(0x01, cardAddr, 0xE4, buffer, 1);
            chValues = null;
            try
            {
                if (frameUnit.WaitData(3000) == false)
                {
                    return false;
                }
                else
                {
                    if (frameUnit.FrameLength < 20) return false;
                    chValues = new Int16[(frameUnit.FrameLength - 4 - 8)/2];
                    for (int i = 8; i < frameUnit.FrameLength - 4; i += 2)
                    {
                       
                        chValues[(i - 8) / 2] = (Int16)(dataBuffer[i] + dataBuffer[i + 1] * 256);
                    }
                    return true;

                }


            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return false;
        }

        internal static bool WriteCoeff(byte cardAddr, short[] CoeffValues)
        {
            byte[] dataBuffer = new byte[4000];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0xE2);
            byte[] buffer = new byte[1+CoeffValues.Length*2];
            buffer[0] = 0x52;
            for (int i = 0; i < CoeffValues.Length; i++)
            {
                Int16 val;
               
                    val = CoeffValues[i];
                
              
                buffer[i * 2 + 1] = (byte)(val & 0xff);
                buffer[i * 2 + 2] = (byte)((val>>8) & 0xff);
            }
            Protocol.FrameTransmit.Send(0x01, cardAddr, 0xE2, buffer, buffer.Length);
           
            try
            {
                if (frameUnit.WaitData(3000) == false)
                {
                    return false;
                }
                else
                {
                  
                    return true;

                }


            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return false;
        }

        internal static bool WrFactoryMessage(byte cardAddr, byte[] message)
        {
            byte[] dataBuffer = new byte[4000];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0xE2);
            byte[] buffer = new byte[1 + 16];
            buffer[0] = 0x50;
            Array.Copy(message, 0, buffer, 1, 16);

            Protocol.FrameTransmit.Send(0x01, cardAddr, 0xE2, buffer, buffer.Length);

            try
            {
                if (frameUnit.WaitData(3000) == false)
                {
                    return false;
                }
                else
                {

                    return true;

                }


            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return false;
        }

        internal static bool WrTsterMessage(byte cardAddr, byte[] message)
        {
            byte[] dataBuffer = new byte[4000];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0xE2);
            byte[] buffer = new byte[1 + 16];
            buffer[0] = 0x51;
            Array.Copy(message, 0, buffer, 1, 16);

            Protocol.FrameTransmit.Send(0x01, cardAddr, 0xE2, buffer, buffer.Length);

            try
            {
                if (frameUnit.WaitData(3000) == false)
                {
                    return false;
                }
                else
                {

                    return true;

                }


            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return false;
        }

        internal static bool RdIdMessage(byte cardAddr, out String id,out DateTime calcTime)
        {
            id = String.Empty;
            calcTime = DateTime.MinValue;
            byte[] dataBuffer = new byte[4000];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0xE4);
            byte[] buffer = new byte[1];
            buffer[0] = 0x50;
            Protocol.FrameTransmit.Send(0x01, cardAddr, 0xE4, buffer, 1);
         
            try
            {
                if (frameUnit.WaitData(3000) == false)
                {
                    return false;
                }
                else
                {
                    if (frameUnit.FrameLength < 8+16+4) return false;
                    int charCount = 12;
                    for (int i = 8; i < frameUnit.FrameLength - 4 &&i<(8+12); i ++)
                    {
                        if (dataBuffer[i] == 0x00 || dataBuffer[i] == 0xff)
                        {
                           charCount = i-8;
                            break;
                        }
                    }
                    try
                    {
                        id = Encoding.Default.GetString(dataBuffer, 8, charCount);

                    }
                    catch
                    {
 
                    }
                    try
                    {
                        int year = dataBuffer[8 + 12] + (dataBuffer[8 + 13] << 8);
                         calcTime= calcTime.AddYears(year-1);
                         calcTime=calcTime.AddMonths(dataBuffer[8+ 14]-1);
                         calcTime= calcTime.AddDays(dataBuffer[8 + 15]-1);
                    }
                    catch
                    {
                        calcTime=calcTime.AddYears(2000);
                    }
                    return true;

                }


            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return false;
        }

        internal static bool RdTsterMessage(byte cardAddr,  out string tster)
        {
            tster = String.Empty;
          
            byte[] dataBuffer = new byte[4000];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0xE4);
            byte[] buffer = new byte[1];
            buffer[0] = 0x51;
            Protocol.FrameTransmit.Send(0x01, cardAddr, 0xE4, buffer, 1);

            try
            {
                if (frameUnit.WaitData(3000) == false)
                {
                    return false;
                }
                else
                {
                    if (frameUnit.FrameLength < 8 + 16 + 4) return false;
                    int charCount = 16;
                    for (int i = 8; i < frameUnit.FrameLength - 4 && i < (8 + 16); i++)
                    {
                        if (dataBuffer[i] == 0x00 || dataBuffer[i] == 0xff)
                        {
                            charCount = i - 8;
                            break;
                        }
                    }
                    try
                    {
                      tster= Encoding.Default.GetString(dataBuffer, 8, charCount);

                    }
                    catch
                    {

                    }
                  
                    return true;

                }


            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
            return false;
        }
        internal static bool RdVerMessage(byte cardAddr,out string verMessage)
        {
           
          
            byte[] buffer = new byte[1];
            buffer[0] = (byte)0x01;
            Protocol.FrameTransmit.Send(1, cardAddr, 7, buffer, 1);
            StringBuilder sb = new StringBuilder();
            byte[] dataBuffer = new byte[40];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x07);

            if (frameUnit.WaitData(2000) == false)
            {
                verMessage="查询失败";


            }
            else
            {
                verMessage=("版本号：" + dataBuffer[8].ToString() + "." + dataBuffer[9].ToString() + "\r\n"
                    + "版本日期：" + dataBuffer[10].ToString() + "年" + dataBuffer[11].ToString() + "月" + dataBuffer[12].ToString());
            }
            return true;
        }
     
    }
    class ReadAddrFinishEventArgs:EventArgs
    {
        private bool readSucess;
        private byte addr;
        public bool IsSucess
        {
            get { return readSucess; }
        }
        public byte Addr
        {
            get { return addr; }
        }
        public ReadAddrFinishEventArgs(bool isOk,byte addr)
        {
            this.readSucess = isOk;
            this.addr = addr;
        }
        
    }

    class WrAddrFinishEventArgs:EventArgs
    {
          private bool readSucess;
        private byte addr;
        public bool IsSucess
        {
            get { return readSucess; }
        }
        public byte Addr
        {
            get { return addr; }
        }
        public WrAddrFinishEventArgs(bool isOk,byte addr)
        {
            this.readSucess = isOk;
            this.addr = addr;
        }
    }
   
    class Thread485
    {
        static Thread dealThread;
        static byte addr;
        public delegate void ReadAddrFinish(ReadAddrFinishEventArgs args);
        public delegate void WriteAddrFinish(WrAddrFinishEventArgs args);
        public static  event ReadAddrFinish RdAddrFinish;
        public static  event  WriteAddrFinish WrAddrFinish ;
    
        static void ProcRdAddrAddr()
        {
            Funs485.Rst(255);
            Funs485.UnLock(255);
            ReadAddrFinish tempResult = RdAddrFinish;
            UInt16 outVal;
            if(Funs485.ReadDebugReg(255, 0xf0, out outVal))
            {
                addr = (byte)outVal;
                if(tempResult!=null)
                {
                    tempResult(new ReadAddrFinishEventArgs(true, addr));
                }
            
            }
            else
            {
                if (tempResult != null)
                {
                    tempResult(new ReadAddrFinishEventArgs(false, addr));
                }
            }
            
        }
        public static void ReadAddr()
        {
            if(dealThread!=null&&dealThread.IsAlive)
            {
                dealThread.Abort();
            }
            dealThread = new Thread(new ThreadStart(ProcRdAddrAddr));
            dealThread.IsBackground = true;
            dealThread.Start();
        }
        static byte newAddr;
        static  void ProcWrAddr()
        {
            Funs485.Rst(addr);
            Funs485.UnLock(addr);
            WriteAddrFinish tempDelegate= WrAddrFinish;
            UInt16 writeVal = (UInt16)(newAddr << 8);
            writeVal += newAddr;

           if (Funs485.WriteDebugReg(addr,0xf0,writeVal))
            {
                addr = newAddr;
                if (tempDelegate != null)
                {
                    tempDelegate(new WrAddrFinishEventArgs(true, addr));
                }

            }
            else
            {
                if (tempDelegate != null)
                {
                    tempDelegate(new WrAddrFinishEventArgs(false, newAddr));
                }
            }
        }
        public static void WriteAddr(byte oldAddr,byte newAddress)
        {
            addr = oldAddr;
            newAddr = newAddress;
            if (dealThread != null && dealThread.IsAlive)
            {
                dealThread.Abort();
            }
            dealThread = new Thread(new ThreadStart(ProcWrAddr));
            dealThread.IsBackground = true;
            dealThread.Start();
        }

        
    }

}
