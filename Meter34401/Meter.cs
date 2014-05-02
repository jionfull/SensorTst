using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Collections;

namespace Lon.Dev
{
    public class ValueListener
    {
        public AutoResetEvent WaitEvent = new AutoResetEvent(false);
        public float Val;
    }

    public class Meter
    {

        SerialPort sp = new SerialPort();
        Thread sampleThread = null;
        ArrayList valueListeners = new ArrayList();
        int WaitCount = 0;
        String cmdStr= null;

        public Meter():this("COM2")
        {
            
        }
        public string PortName
        {
            get
            {
                return sp.PortName;
            }
            set
            {
                sp.PortName = value;
            }
        }
        public Meter(string port)
        {
            sp.PortName = port;
        }
        void sampleProc()
        {
            //   sp.RtsEnable = true;
            sp.DtrEnable = true;
            sp.ReadTimeout = 2000;

            try
            {
                sp.Open();
                sp.WriteLine("");
                sp.WriteLine("*CLS");
                sp.WriteLine("SYST:REM");
                Thread.Sleep(500);
                sp.WriteLine("CONF:VOLT:AC");
                Thread.Sleep(500);
                while (true)
                {
                    sp.WriteLine("*CLS");
                    Thread.Sleep(100);
                    if (cmdStr == null)
                    {
                        sp.WriteLine("READ?");
                        Thread.Sleep(100);
                        String readStr;
                        try
                        {
                            readStr = sp.ReadLine();
                            try
                            {
                                float val;
                                val = float.Parse(readStr);
                                lock (valueListeners)
                                {
                                    foreach (ValueListener listener in valueListeners)
                                    {

                                        listener.Val = val;
                                        listener.WaitEvent.Set();

                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                        catch
                        {
                            sp.WriteLine("");
                            Thread.Sleep(100);
                            sp.WriteLine("*CLS");
                            Thread.Sleep(200);
                        }
                    }
                    else
                    {
                        sp.WriteLine(cmdStr);
                        Thread.Sleep(500);
                        cmdStr = null;
                    }
                }
            }
            catch (System.Exception ex)
            {

            }

        }
        public bool ReadValue(out float val, int outTime)
        {
            ValueListener listener = new ValueListener();
            bool retVal;

            lock (valueListeners)
            {
                valueListeners.Add(listener);
            }
            if (listener.WaitEvent.WaitOne(outTime, false))
            {
                val = listener.Val;
                
                retVal=true;
                goto End;
                
            }
            else
            {
                val = 0;
                retVal = false;
                goto End;
            }
            End:
            lock (valueListeners)
            {
                valueListeners.Remove(listener);
            }
            return retVal;
        }
        public bool IsOpen()
        {
            return sp.IsOpen;
        }
        public void Stop()
        {
            sampleThread.Abort();
            sp.WriteLine("");
            sp.WriteLine("*CLS");
            sp.WriteLine("SYSTem:LOC");
          
            sp.Close();
        }
        public void WriteCmd(String str)
        {
            if(cmdStr==null)
            {
                
                   cmdStr = str;
               
               
            }
        }
          
        public void SetAc()
        {
            WriteCmd("CONF:VOLT:AC");
        }
        public void SetDc()
        {
            WriteCmd("CONF:VOLT:DC");
        }
        public void Run()
        {
            if (sampleThread != null&&sampleThread.IsAlive)
            {
                Stop();
            }
            sampleThread = new Thread(new ThreadStart(sampleProc));
            sampleThread.IsBackground = true;
            sampleThread.Start();
        }

    }





}
