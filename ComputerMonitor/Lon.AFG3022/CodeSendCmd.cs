using System;
using System.Collections.Generic;
using System.Text;
using Lon.TestModel;
using System.Windows.Forms;
using System.Collections;
using LogSave;

namespace Lon.AFG3022
{
    public class CodeSendCmd
    {
        #region AC码型定义
        private static UInt16[][] jljsTime = new UInt16[][]{
                                   new UInt16[]  {350,120,220,120,220,570},//1.6
                                   new UInt16[]  {380,120,380,720},//1.6
                                   new UInt16[]  {230,570,230,570},//1.6
                                   new UInt16[]  {340,120,220,120,220,580},//1.6
                                   new UInt16[]  {440,120,460,580},//1.6
                                   new UInt16[]  {580,120,320,580},//1.6
                                   new UInt16[]  {220,580,220,580},//1.6
                                   new UInt16[]  {350,120,250,120,250,810},//1.9
                                   new UInt16[]  {350,120,620,810},//1.9
                                   new UInt16[]  {300,650,300,650},//1.9
                                   new UInt16[]  {360,120,240,120,240,840},//1.92
                                   new UInt16[]  {480,120,720,600},//1.92
                                   new UInt16[]  {620,120,580,600},//1.92
                                   new UInt16[]  {300,660,300,660},//1.92
                                   new UInt16[]  {220,120,340,120,220,580},//1.6
                                   new UInt16[]  {240,120,360,120,240,840},//1.92
                                   };
        #endregion

        private int _codeStyle;
        private double _freq;
        private double _lowFreq;
        private double _ampli;
        private double _coeffi;
        public  int CodeStyle
        {
            get
            {
                return _codeStyle;
            }
            set
            {
                _codeStyle = value;
            }
        }
        public double Freq 
        { get
          {
              return _freq;
           }
           set
           {
               _freq = value; 
            }
          }
        public double LowFreq
        {
            get
            {
                return _freq;
            }
            set
            {
                _freq = value;
            }
        }
        public double Ampli
        {
            get
            { return _ampli; }
            set
            {_ampli = value;}
        }
        public double Coeffi
        {
            get { return _coeffi;}
            set { _coeffi = value; }
        }
        public CodeSendCmd()
        {
            CreateAFG3022();
           
        }
        public bool Cmd(int codeStyle, double freq, double lowFreq, double ampli, double coeffi)
        {
            _codeStyle = codeStyle;
            _freq = freq;
            _lowFreq = lowFreq;
            _ampli = ampli;
            _coeffi = coeffi;
            return Cmd();
        }
        public bool SineSend(byte SourceNum, double freq, double ampli)
        {
        	if (afg3022.Descriptor == null)
            {
                Log.MessAgeLog("xhtst.log", "AFG3022.Descriptor==null");
                CreateAFG3022();
                
            }
        	if (afg3022.Descriptor == null)
            {
                Log.MessAgeLog("xhtst.log", "AFG3022未找到");
                return false;
                
            }
                afg3022.WriteString("SOURce" + SourceNum + ":FUNCtion:SHAPe SIN");
                afg3022.WriteString("SOURce" + SourceNum + ":FREQuency:FIXed " + freq);
                afg3022.WriteString("SOURce" + SourceNum + ":VOLTage:LEVel:IMMediate:AMPLitude " + ampli);
                afg3022.WriteString("OUTPut ON " );
                Log.MessAgeLog(".\\xhtst.log", afg3022.Descriptor + "  Sine Send");
                return true;
            
        }
    
        public bool ChOFF(byte SourceNum)
        {
            if (afg3022.Descriptor == null)
            {
                Log.MessAgeLog("xhtst.log", "AFG3022.Descriptor==null");
                CreateAFG3022();

            }
            if (afg3022.Descriptor == null)
            {
                Log.MessAgeLog("xhtst.log", "AFG3022未找到");
                return false;

            }
           
            afg3022.WriteString("OUTPut OFF");
            Log.MessAgeLog(".\\xhtst.log", afg3022.Descriptor + "  Sine Send");
            return true;
        }


        public bool Cmd()
        {
            switch (_codeStyle)
            {
                case 0:
                    CreateJljs(1, _freq, _ampli, (byte)_lowFreq);
                    break;
                case 1:
                    CreateFM(1,_freq,_ampli*_coeffi,_lowFreq,55);
                    break;
                case 2:
                    CreateFM(1, _freq, _ampli * _coeffi, _lowFreq, 11);
                    break;
                default:
                    CreateFM(1, _freq, _ampli*_coeffi, _lowFreq, 11);
                    break;


            }
            return true;
        }
        private static string[] memName = new string[] { "EMEMory", "USER1", "USER2", "USER3", "USER4" };

        private static TVCLib.Tvc afg3022 = new TVCLib.Tvc();

        private  TVCLib.Tvc CreateAFG3022()
        {
           
                //刷新设备列表
                TekVRAPI.CRemoteAPI remoteAPI = new TekVRAPI.CRemoteAPI();
                if (!remoteAPI.TekVIsRMUPdateActive())
                {
                    remoteAPI.TekVStartRmUpdate("");
                }
                //寻找AFG3022设备
                string descriptor = null;
                TekVISANet.VISA visa = new TekVISANet.VISA();
                ArrayList deviceList = new ArrayList();
                if (visa.FindResources("USB?*INSTR", out deviceList))
                {
                    descriptor = deviceList[0].ToString();

                }
                afg3022.Descriptor = descriptor;
                if (descriptor != null)
                {
                    Log.MessAgeLog(".\\xhtst.log", "找到USB设备" + descriptor);
                }
                else
                {
                    Log.MessAgeLog(".\\xhtst.log", "未找到设备");
                }
            //    CreateFM(1, 1700, 0.100, 10.3, 11);
                visa.Dispose();
                visa = null;
                
            
            return afg3022;
        }

        public  void CreateFM(byte SourceNum, double CarrierFreq, double Ampl, double FMFreq, double Deviation)
        {
            if (afg3022.Descriptor == null)
            {
                Log.MessAgeLog("xhtst.log", "AFG3022.Descriptor==null");
                CreateAFG3022();
            }
                afg3022.WriteString("SOURce" + SourceNum + ":FUNCtion:SHAPe SIN");
                afg3022.WriteString("SOURce" + SourceNum + ":FREQuency:FIXed " + CarrierFreq);
                afg3022.WriteString("SOURce" + SourceNum + ":VOLTage:UNIT VRMS");
                afg3022.WriteString("SOURce" + SourceNum + ":VOLTage:LEVel:IMMediate:AMPLitude " + Ampl * 0.001);
                afg3022.WriteString("SOURce" + SourceNum + ":FM:INTernal:FUNCtion SQUare");
                afg3022.WriteString("SOURce" + SourceNum + ":FM:INTernal:FREQuency " + FMFreq);
                afg3022.WriteString("SOURce" + SourceNum + ":FM:DEViation " + Deviation);
                afg3022.WriteString("SOURce" + SourceNum + ":FM:STATe ON");
                Log.MessAgeLog(".\\xhtst.log", afg3022.Descriptor + "  FM Send");
            
        }

 #region "生成交流计数"
        public static double GetJLJSRate(byte index)
        {
            if (index > 15) return 1;
            index = 0;
            int onTime = 0, totalTime = 0;
            for (int i = 0; i < jljsTime[index].Length; i++)
            {
                totalTime += jljsTime[index][i];
                if ((i % 2) == 0)
                {
                    onTime += jljsTime[index][i];
                }
            }
            return (double)totalTime / onTime;
        }

       
        public  void CreateJljs(byte SourceNO, double CarrierFreq, double Ampl, byte AcCode)
        {
            if (afg3022 == null || AcCode > 15) return;
            int count = 0;
            int count1 = 0;
            for (int i = 0; i < jljsTime[AcCode].Length; i++)
            {
                if (i % 2 == 0)
                {
                    count1 += jljsTime[AcCode][i];
                }
                count += jljsTime[AcCode][i];

            }
            double Ampl1 = Ampl * 1.0;//Math.Sqrt(8.0 * count / count1);
            byte multiple = (byte)(65536 / count);
            byte[] signalData = new byte[2 * count * multiple];

            int k = 0;
            ushort data;
            for (int i = 0; i < jljsTime[AcCode].Length; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < multiple * jljsTime[AcCode][i]; j++)
                    {
                        data = (ushort)((Math.Sin(2 * Math.PI * j / multiple * CarrierFreq / 1000) + 1) * 8191);
                        signalData[2 * k] = (byte)(data >> 8);
                        signalData[2 * k + 1] = (byte)(data & 0xff);
                        k++;
                    }
                }
                else
                {
                    for (int j = 0; j < multiple * jljsTime[AcCode][i]; j++)
                    {

                        signalData[2 * k] = 0x1f;
                        signalData[2 * k + 1] = 0xff;
                        k++;
                    }
                }
            }
            int totalCount = count * multiple;
            double ScanFreq = 1000.0 / count;
            try
            {

                afg3022.WriteString("SOUR" + SourceNO + ":FM:STATE OFF");
                afg3022.WriteString("TRACe:DEFine EMEMory," + totalCount.ToString());
                totalCount = totalCount * 2;
                afg3022.SendEndEnabled = false;
                afg3022.WriteString("TRACE:DATA EMEMORY,#" + totalCount.ToString().Length + totalCount);
                afg3022.SendEndEnabled = true;
                afg3022.WriteByteArray(signalData);
                afg3022.WriteString("SOUR" + SourceNO + ":FREQ " + ScanFreq);
                afg3022.WriteString("SOUR" + SourceNO + ":FUNC EMEMORY");
                afg3022.WriteString("SOURce" + SourceNO + ":VOLTage:LEVel:IMMediate:AMPLitude " + Ampl1 * 0.002828);
                afg3022.WriteString("OUTPut ON");
                
            }
            catch (Exception ex)
            {
                Log.MessAgeLog("xhtst.log", ex.Message + " " + afg3022.Descriptor);
                this.CreateAFG3022();
                afg3022.WriteString("SOUR" + SourceNO + ":FM:STATE OFF");
                afg3022.WriteString("TRACe:DEFine EMEMory," + totalCount.ToString());
                totalCount = totalCount * 2;
                afg3022.SendEndEnabled = false;
                afg3022.WriteString("TRACE:DATA EMEMORY,#" + totalCount.ToString().Length + totalCount);
                afg3022.SendEndEnabled = true;
                afg3022.WriteByteArray(signalData);
                afg3022.WriteString("SOUR" + SourceNO + ":FREQ " + ScanFreq);
                afg3022.WriteString("SOUR" + SourceNO + ":FUNC EMEMORY");
                afg3022.WriteString("SOURce" + SourceNO + ":VOLTage:LEVel:IMMediate:AMPLitude " + Ampl1 * 0.002828);
                afg3022.WriteString("OUTPut ON");
            }
         //  afg3022.WriteString("OUTPUT2 ON");


        }
       

        public  void CreateJljs(byte SourceNO, double CarrierFreq, double Ampl, byte AcCode, double coeffi)
        {
            if (afg3022 == null || AcCode > 15) return;
            int count = 0;
            int count1 = 0;
            for (int i = 0; i < jljsTime[AcCode].Length; i++)
            {
                if (i % 2 == 0)
                {
                    count1 += jljsTime[AcCode][i];
                }
                count += jljsTime[AcCode][i];

            }
            double Ampl1 = Ampl * 1;//Math.Sqrt(8.0 * count / count1);
            byte multiple = (byte)(65536 / count);
            byte[] signalData = new byte[2 * count * multiple];

            int k = 0;
            ushort data;
            for (int i = 0; i < jljsTime[AcCode].Length; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < multiple * jljsTime[AcCode][i]; j++)
                    {
                        data = (ushort)((Math.Sin(2 * Math.PI * j / multiple * CarrierFreq / 1000) * coeffi + 1) * 8191);
                        signalData[2 * k] = (byte)(data >> 8);
                        signalData[2 * k + 1] = (byte)(data & 0xff);
                        k++;
                    }
                }
                else
                {
                    for (int j = 0; j < multiple * jljsTime[AcCode][i]; j++)
                    {

                        signalData[2 * k] = 0x1f;
                        signalData[2 * k + 1] = 0xff;
                        k++;
                    }
                }
            }
            int totalCount = count * multiple;
            double ScanFreq = 1000.0 / count;
            afg3022.WriteString("SOUR" + SourceNO + ":FM:STATE OFF");
            afg3022.WriteString("TRACe:DEFine EMEMory," + totalCount.ToString());
            totalCount = totalCount * 2;
            afg3022.SendEndEnabled = false;
            afg3022.WriteString("TRACE:DATA EMEMORY,#" + totalCount.ToString().Length + totalCount);
            afg3022.SendEndEnabled = true;
            afg3022.WriteByteArray(signalData);
            afg3022.WriteString("SOUR" + SourceNO + ":FREQ " + ScanFreq);
            afg3022.WriteString("SOUR" + SourceNO + ":FUNC EMEMORY");
            afg3022.WriteString("SOURce" + SourceNO + ":VOLTage:LEVel:IMMediate:AMPLitude " + Ampl1);
        }

        public  void CreateJljsVpp(byte SourceNO, double CarrierFreq, double Ampl, byte AcCode, double coeffi)
        {
            if (afg3022 == null || AcCode > 15) return;
            int count = 0;

            for (int i = 0; i < jljsTime[AcCode].Length; i++)
            {
                count += jljsTime[AcCode][i];

            }

            byte multiple = (byte)(65536 / count);
            byte[] signalData = new byte[2 * count * multiple];

            int k = 0;
            ushort data;
            for (int i = 0; i < jljsTime[AcCode].Length; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < multiple * jljsTime[AcCode][i]; j++)
                    {
                        data = (ushort)((Math.Sin(2 * Math.PI * j / multiple * CarrierFreq / 1000) * coeffi + 1) * 8191);
                        signalData[2 * k] = (byte)(data >> 8);
                        signalData[2 * k + 1] = (byte)(data & 0xff);
                        k++;
                    }
                }
                else
                {
                    for (int j = 0; j < multiple * jljsTime[AcCode][i]; j++)
                    {

                        signalData[2 * k] = 0x1f;
                        signalData[2 * k + 1] = 0xff;
                        k++;
                    }
                }
            }
            int totalCount = count * multiple;
            double ScanFreq = 1000.0 / count;
            afg3022.WriteString("SOUR" + SourceNO + ":FM:STATE OFF");
            afg3022.WriteString("TRACe:DEFine EMEMory," + totalCount.ToString());
            totalCount = totalCount * 2;
            afg3022.SendEndEnabled = false;
            afg3022.WriteString("TRACE:DATA EMEMORY,#" + totalCount.ToString().Length + totalCount);
            afg3022.SendEndEnabled = true;
            afg3022.WriteByteArray(signalData);
            afg3022.WriteString("SOUR" + SourceNO + ":FREQ " + ScanFreq);
            afg3022.WriteString("SOUR" + SourceNO + ":FUNC EMEMORY");
            afg3022.WriteString("SOURce" + SourceNO + ":VOLTage:LEVel:IMMediate:AMPLitude " + Ampl);
        }


        #endregion



    }
}
