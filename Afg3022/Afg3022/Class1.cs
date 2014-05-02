using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;

namespace Afg3022
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
            //寻找AFG3022设备
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
        private TVCLib.Tvc afg3022 = new TVCLib.Tvc();
        public FunGen(string descriptor)
        {
            afg3022.Descriptor = descriptor;
         
        }
        public FunGen()
        {
            if(GetDevDescList()!=null&&deviceList.Count!=0)
            {
                afg3022.Descriptor = deviceList[0].ToString();
           }
        }
      
        public bool SetFreq(int chNum, float freq)
        {
            if (afg3022.Descriptor != null)
            {
                afg3022.WriteString("SOURce" + chNum + ":FREQuency:FIXed " + freq);
                return true;
            }
            else 
            {
                return false;
            }
        }
        public bool SetAmpli(int chNum, float ampli)
        {
            if (afg3022.Descriptor != null)
            {
                afg3022.WriteString("SOURce" + chNum + ":VOLTage:UNIT VRMS");
                afg3022.WriteString("SOURce" + chNum + ":VOLTage:LEVel:IMMediate:AMPLitude " + ampli * 0.001);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetSinMode(int chNum)
        {
            if (afg3022.Descriptor != null)
            {
                afg3022.WriteString("SOURce" + chNum + ":FUNCtion:SHAPe SIN");
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool WaitDevRdy(int waitTime)
        { 
            int i=0;
            while (afg3022.Descriptor == null||deviceList.Count == 0)
            {
                Thread.Sleep(1);
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
            
            if (afg3022.Descriptor != null)
            {
                afg3022.WriteString("SOURce" + chNum + ":FM:STATe ON");
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool SetFmSquare(int chNum)
        {
            if (afg3022.Descriptor != null)
            {
                afg3022.WriteString("SOURce" + chNum + ":FM:STATe ON");
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool SetFmFreq(int chNum, float freq)
        {
            bool ret;
            ret=WaitDevRdy(10);
            if (ret)
            { 
                afg3022.WriteString("SOURce" + (chNum+1).ToString() + ":FM:INTernal:FREQuency " + freq);
               
            }

            return ret;
           
        }

    }
}
