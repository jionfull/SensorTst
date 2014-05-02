using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Lon.IO.Ports
{
    public class FunGen
    {
        static ArrayList deviceList = new ArrayList();
        static public ArrayList GetDevDescList()
        {
            deviceList = null;
            TekVRAPI.CRemoteAPI remoteAPI = new TekVRAPI.CRemoteAPI();
            if (!remoteAPI.TekVIsRMUPdateActive())
            {
                remoteAPI.TekVStartRmUpdate("");
            }

            string descriptor = null;
            TekVISANet.VISA visa = new TekVISANet.VISA();
            if (visa.FindResources("USB?*INSTR", out deviceList))
            {
                descriptor = deviceList[0].ToString();
            }
            visa.Dispose();
            visa = null;
            return deviceList;
        }
        private static TVCLib.Tvc afg3022 = new TVCLib.Tvc();
        public FunGen(string descriptor)
        {
            afg3022.Descriptor = descriptor;

        }
        static String desc = null;
        public FunGen()
        {
            if (desc != null)
            {
               // afg3022.Descriptor = desc;
                return;
            }
            if (GetDevDescList() != null && deviceList.Count != 0)
            {
                afg3022.Descriptor = deviceList[0].ToString();
                desc = afg3022.Descriptor;
            }
        }

        public bool SetFreq(int chNum, float freq)
        {

            afg3022.WriteString("SOURce" + (chNum + 1).ToString() + ":FREQuency:FIXed " + freq);
            Trace.WriteLine("SOURce" + (chNum + 1).ToString() + ":FREQuency:FIXed " + freq);
            return true;

        }
        public bool SetAmpli(int chNum, float ampli)
        {

            Thread.Sleep(300);
            afg3022.WriteString("SOURce" + (chNum + 1).ToString() + ":VOLTage:LEVel:IMMediate:AMPLitude " + ampli  + "\r\n");
            return true;

        }
        public bool SetSinMode(int chNum)
        {

            Thread.Sleep(100);
            afg3022.WriteString("SOURce" + (chNum + 1).ToString() + ":FUNCtion:SHAPe SIN");
            return true;

        }
        private bool WaitDevRdy(int waitTime)
        {
            int i = 0;
            while (afg3022.Descriptor == null || deviceList.Count == 0)
            {
                Thread.Sleep(100);
                if (GetDevDescList() != null && deviceList.Count != 0)
                {
                    afg3022.Descriptor = deviceList[0].ToString();
                }
                if (i > waitTime)
                {
                    return false;
                }
                i++;
            }
            return true;
        }
        public bool SetFmMode(int chNum)
        {

            Thread.Sleep(100);
            afg3022.WriteString("SOURce" + (chNum + 1).ToString() + ":FM:STATe ON");
            return true;

        }
        public bool SetFmSquare(int chNum)
        {

            Thread.Sleep(100);
            afg3022.WriteString("SOURce" + (chNum + 1).ToString() + ":FM:STATe ON");
            return true;


        }
        public bool SetFmFreq(int chNum, float freq)
        {
            bool ret;
         
                afg3022.WriteString("SOURce" + (chNum + 1).ToString() + ":FM:INTernal:FREQuency " + freq.ToString());
                ret = true;

       

            return ret;
        }

        public bool SetFmDevia(int chNum, float freq)
        {
              
                afg3022.WriteString("SOURce" + (chNum + 1).ToString() + ":FM:Deviation " + freq.ToString() + "Hz");
                return true;
         
        }
    }
}
