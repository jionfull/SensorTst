using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ComputerMonitor.Helper;

namespace ComputerMonitor.Protocol
{
    class CANFrameUnit
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
        public CANFrameUnit(byte[] buffer, int cmdValue)
        {
            frameData = buffer;
            cmd = cmdValue;
        }

        public int Write(byte[] buffer, int count)
        {
            if (cmd < 0 || (buffer[3] >> 5) == cmd) //Æ¥ÅäËùÓÐÃüÁî
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

    class CANFrameManager
    {
        static Thread threadCANRec = null;
        static AutoResetEvent eventCAN = new AutoResetEvent(false);
        static AutoResetEvent eventCANSend = new AutoResetEvent(false);
        private static List<CANFrameUnit> listFrame = new List<CANFrameUnit>();

        private static CAN.CANDevice CANDev = new ComputerMonitor.CAN.CANDevice();

        static CANFrameManager()
        {
            CAN.CANCardLib.CANCARD_LibInit();
            CANDev.OpenDevice();
            CANDev.IRQEvent += new EventHandler(CANDev_IRQEvent);
            CANDev.WriteData(0x02, 0x00);
        }
        public static void WriteData(uint offset, ushort data)
        {
            CANDev.WriteData(offset, data);
        }

        public static ushort ReadData(uint offset)
        {
            return CANDev.ReadData(offset);
        }

        public static void StartRecvData()
        {
            if (threadCANRec == null || threadCANRec.IsAlive == false)
            {
                threadCANRec = new Thread(new ThreadStart(ProcCANRev));
                threadCANRec.IsBackground = true;
                threadCANRec.Start();
            }
        }

        private static void ProcCANRev()
        {
            ushort[] buffer = new ushort[6];
            byte[] rcvData=new byte[12];
            ushort val;
            int index = 0;
            int count = 0;
            while (true)
            {
                eventCAN.WaitOne();
                index = 0;
                count = 0;
                CANDev.ReadData(0x06); //¿ªÊ¼¶ÁÐ´

                while (true)
                {
                    val = CANDev.ReadData(0x04);
                    buffer[index] = val;
                    index++;
                    if (val == 0xffff)
                    {
                        count++;
                    }
                    if (index == 6)
                    {

                        if (count == 6)
                        {
                            //Í£Ö¹¶ÁÐ´
                            CANDev.WriteData(0x02, 0);
                            break;
                        }
                        else
                        {


                            for (int i = 0; i < 6; i++)
                            {
                                byte tmp = (byte)(buffer[i] >> 8);
                                rcvData[i * 2] = tmp;

                                tmp = (byte)(buffer[i] & 0xff);
                                rcvData[i * 2 + 1] = tmp;
                            }
                            CheckFrame(rcvData, 12);
                            Thread.Sleep(10); //µÈ´ýÒ»ÏÂ
                        }
                        index = 0;
                        count = 0;
                    }


                }
            }
        }

        static void CANDev_IRQEvent(object sender, EventArgs e)
        {
            if (CANDev.CanRead)
            {
                eventCAN.Set();
            }
            else
            {
                eventCANSend.Set();
            }
            
        }

        public static bool WaitEvent()
        {
            return eventCANSend.WaitOne();
        }

        public static CANFrameUnit CreateFrame(byte[] buffer, int cmd)
        {
            CANFrameUnit frameUnit = new CANFrameUnit(buffer, cmd);

            lock (listFrame)
            {
                listFrame.Add(frameUnit);
            }
            return frameUnit;
        }

        public static void DeleteFrame(CANFrameUnit frameUnit)
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
                foreach (CANFrameUnit frameUnit in listFrame)
                {
                    frameUnit.Write(originFrame, frameLength);
                }
            }
        }
    }

    class CANFrameTransmit
    {
        static byte[] txData = new byte[11];
        private static Queue<byte[]> sendQueue = new Queue<byte[]>();
        private static AutoResetEvent eventSend = new AutoResetEvent(false);
        static Thread threadSend = null;
        public static void Start()
        {
            if (threadSend == null || threadSend.IsAlive == false)
            {
                threadSend = new Thread(new ThreadStart(ProcSend));
                threadSend.IsBackground = true;
                threadSend.Start();
            }
        }
        public static void Send(byte CANAddr,byte Cmd,byte[] Data,int Length)
        {
            if(Length>8) return;
            byte[] data=new byte[12];
            Array.Clear(data, 0, data.Length);
            data[0] = 0xff;
            data[1] = 0xff;
            data[2] = CANAddr;
            data[3] = (byte)(Cmd << 5 | Length);
            for (int i = 0; i < Length; i++)
            {
                data[4 + i] =Data[i];
            }
            lock (sendQueue)
            {
                sendQueue.Enqueue(data);
                eventSend.Set();
            }
            GlobalVar.ShowSendData(data, 0, data.Length); 
            //SerialManager.CommPort.WriteData(txData, 0, 11);
        }

        private static void ProcSend()
        {
            try
            {
                while (true)
                {
                    eventSend.WaitOne();
                    CANFrameManager.WriteData(0x02, 0);//Í£Ö¹¶ÁÐ´

                    CANFrameManager.WriteData(0x00, 0); //¿ªÊ¼¶ÁÐ´
                    CANFrameManager.WaitEvent();
                    CANFrameManager.WriteData(0x06, 0); //¿ªÊ¼Ð´²Ù×÷
                    
                    
                    lock (sendQueue)
                    {
                        int c=sendQueue.Count;
                        for (int i = 0; i < c; i++)
                        {
                            byte[] sendData = sendQueue.Dequeue();
                            for (int j = 0; j < 6; j++)
                            {
                               ushort tmp = (ushort)((sendData[j * 2] << 8) + sendData[j * 2 + 1]);
                                CANFrameManager.WriteData(0x08, tmp);

                            }
                           
                        }
                    }
                    //Ð´Èëff ff ff ff
                    for (int i = 0; i < 6; i++)
                    {
                        CANFrameManager.WriteData(0x08, 0xffff);
                    }

                    CANFrameManager.WriteData(0x02, 0); //Í£Ö¹¶ÁÐ´

                }
            }
            catch (ThreadAbortException)
            {
                
            }
        }
    }

}
