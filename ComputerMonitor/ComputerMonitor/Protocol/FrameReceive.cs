using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;

namespace ComputerMonitor.Protocol
{
    class FrameReceive
    {
        
        private static Thread threadFrameReceive;

        static FrameReceive()
        {
            
        }

        public static bool checkCRC = false;
        private static void ParseFrame(byte[] frame,int count)
        {
            //改为累加和检验
            if (checkCRC)
            {
                if (count > 8)
                {
                    //先校验长度
                    ushort frameLength = (ushort)(frame[2] + (frame[3] << 8));
                    if (frameLength == count - 6)
                    {
                        ushort checkSum = 0;
                        for (int i = 0; i < count - 6; i++)
                        {
                            checkSum += frame[i + 2];
                        }
                        ushort rCheckSum = (ushort)(frame[count - 4] + (frame[count - 3] << 8));
                        if (checkSum == rCheckSum)
                        {
                            FrameManager.CheckFrame(frame, count);
                        }
                    }
                }
            }
            else
            {
                FrameManager.CheckFrame(frame, count);
            }
            
        }

        private static void FrameReceiveProc()
        {

            byte[] frameData = new byte[64*1024];
            byte[] canFrame = new byte[11];//数据帧格式为0xff +11个数据
            int canFrameLength = 0;
            int frameLength = 0;
            byte[] receiveData=new byte[64*1024];
            int receiveLength = 0;
            bool metHeader = false;
            bool metFrame = false;
            while (true)
            {
                if (SerialManager.CommPort.WaitPortData(500))
                {
                    receiveLength = SerialManager.CommPort.ReadData(receiveData);
                    for (int i = 0; i < receiveLength; i++)
                    {
                        if (receiveData[i] == 0xff)
                        {
                            if (canFrameLength >= 3)
                            {
                       //         CANFrameManager.CheckFrame(canFrame, canFrameLength);
                                canFrameLength = 0;
                            }
                            else
                            {
                                canFrame[0] = 0xff;
                                canFrameLength = 1;
                            }
                            
                        }
                        else
                        {
                            if (canFrameLength > 0 && canFrameLength < 11)
                            {
                                canFrame[canFrameLength] = receiveData[i];
                                canFrameLength++;
                            }
                            else
                            {
                                canFrameLength = 0;
                            }
                        }
                    }
                    for (int i = 0; i < receiveLength; i++)
                    {
                        if (receiveData[i] == 0x10)
                        {
                            if (metHeader == false)
                            {
                                frameData[frameLength] = 0x10;
                                frameLength++;
                                metHeader = true;
                                if (frameLength >= frameData.Length)
                                {
                                    frameLength = 0;
                                    metHeader = false;
                                    metFrame = false;
                                }
                            }
                            else
                            {
                                metHeader = false;
                            }
                        }
                        else if (receiveData[i] == 0x02)
                        {
                            if (metHeader)
                            {
                                frameData[0] = 0x10;
                                frameData[1] = 0x02;
                                frameLength = 2;
                                metHeader = false;
                                metFrame = true;
                            }
                            else
                            {
                                if (!metFrame)
                                {
                                    frameLength = 0;
                                    metHeader = false;
                                    continue;
                                }
                                frameData[frameLength] = 0x02;
                                frameLength++;
                                if (frameLength >= frameData.Length)
                                {
                                    frameLength = 0;
                                    metHeader = false;
                                    metFrame = false;
                                }

                            }
                        }
                        else if (receiveData[i] == 0x03)
                        {
                            if (!metFrame)
                            {
                                frameLength = 0;
                                metHeader = false;
                                continue;
                            }
                            if (metHeader)
                            {
                                frameData[frameLength] = 0x03;
                                frameLength++;

                                ParseFrame(frameData, frameLength); //收到数据帧
                                //Thread.Sleep(50);
                                frameLength = 0;
                                metHeader = false;
                                metFrame = false;
                            }
                            else
                            {

                                frameData[frameLength] = 0x03;
                                frameLength++;
                                if (frameLength >= frameData.Length)
                                {
                                    frameLength = 0;
                                    metHeader = false;
                                    metFrame = false;
                                }
                            }
                        }
                        else
                        {
                            if (!metFrame)
                            {
                                frameLength = 0;
                                metHeader = false;
                                continue;
                            }

                            frameData[frameLength] = receiveData[i];
                            frameLength++;
                            if (frameLength >= frameData.Length)
                            {
                                frameLength = 0;
                                metHeader = false;
                                metFrame = false;
                            }

                        }
                    }
                }
                else
                {
 
                }

                 
               
            }
        }

       
        public static void Start()
        {
            if (threadFrameReceive == null || threadFrameReceive.IsAlive == false)
            {
                threadFrameReceive = new Thread(new ThreadStart(FrameReceiveProc));
                threadFrameReceive.IsBackground = true;
                threadFrameReceive.Start();
            }
        }

        public static void Stop()
        {
 
        }
    }
}
