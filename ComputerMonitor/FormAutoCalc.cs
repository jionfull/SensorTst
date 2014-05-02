using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComputerMonitor.Protocol;
using System.Threading;
using Lon.Dev;
using System.IO;
using JcxhCodeSend;
using WeifenLuo.WinFormsUI.Docking;


namespace ComputerMonitor
{
    public partial class FormAutoCalc : DockContent
    {
        byte[] listenBuffer = new byte[4000];
        FrameUnit chListen;
        delegate void DelegateShowText(Control ctl, string str);
        delegate void DelegateAppendText(TextBox tb, string str);
        byte cardAddr = 4;
        byte ch = 255;
        Thread threadReadCh = null;

        AutoResetEvent waitKey = new AutoResetEvent(false);

        UInt16[] coffList = new UInt16[255];
        ICodeSend sigSource;

        #region 常量定义
        static readonly string[] freqNameList = new String[]{
                
                "1700Hz","2000Hz","2300Hz","2600Hz"
        };
        static readonly float[] freqList = new float[]
        {
               1700,2000,2300,2600
        };
        static readonly float[] interferenceFreqList = new float[]
        {
               2300,2600,1700,2000
        };

        static readonly float[][] chScale = new float[][] {
           // ch0     ch1,  ch2,   ch3,  ch4 ,   ch5,   ch6, ch7 部分为数组对齐补空数据 
           new  float[]{ 10f,   10f,   10f,  625f,  625f,  625f, 625f,  625f},//站内电码化
           new  float[]{ 10f,   10f,   1000, 1000,  625f,  625f, 625f,625f},//有绝缘
           new  float[]{ 10f,   10f,   100f,  1000f,1000f,  1000f, 625f,625f},//无绝缘
           new  float[]{ 10f,   10f,   10f,  1000f, 1000f,1000f,1000f,1000f},//电源屏
           new  float[]{ 100f, 100f,   100f, 100f,  100f,  100f, 100f, 100f},//直流40V
           new  float[]{  10f,  10f,   10f,   10f,   10f,   10f,  10f,  10f},//直流PN200V
           new  float[]{  10f,  10f,   10f,   10f,   10f,   10f,  10f,  10f},//道岔表示交流
           new  float[]{  10f,  10f,   10f,   10f,   10f,   10f,  10f,  10f},//道岔表示交流
           new  float[]{  10f,  10f,   10f,   10f,   10f,   10f,  10f,  10f},//高压不对称
           new  float[]{1000f, 100f,  1000f,1000f, 1000f, 1000f,1000f,1000f}//直流电流
        };
        #endregion


        #region 委托定义
        void ShowText(Control ctl, string str)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateShowText(ShowText), new object[] { ctl, str });
            }
            else
            {
                ctl.Text = str;
            }
        }
        void AppendText(TextBox tb, String str)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateAppendText(AppendText), new object[] { tb, str });
            }
            else
            {
                tb.AppendText(str);
            }
        }

        #endregion

        public FormAutoCalc()
        {
            InitializeComponent();
            sigSource = new CodeSend3022();
        }

        #region 表头常量定义
        const int titleRow = 0;
        const int detailedTitleRow = 1;
        const int chNameCol = 0;
        const int calcedAmpliCol = 1;
        const int calcedFreqCol = 2;
        const int calcedLowFreqCol = 3;
        const int refAmpliCol = 4;
        const int refFreqCol = 5;
        const int refLowFreqCol = 6;
        const int inaccuracyCol = 7;
        const int freq1700CoefCol = 8;
        const int freq2000CoefCol = 9;
        const int freq2300CoefCol = 10;
        const int freq2600CoefCol = 11;
        #endregion

        private void FormAutoCalc_Load(object sender, EventArgs e)
        {

            DataSourceEditor calcedValueEditor = new DataSourceEditor(WaitCalcValues);
            DataSourceEditor RefValueEditor = new DataSourceEditor(RefValues);
            CoefEditor coefValueEditor = new CoefEditor(CoeffValues);
            InaccuracyEditor inaccuracyEditor = new InaccuracyEditor(WaitCalcValues, RefValues);


            this.gridValueView.Redim(10, 12);
            this.gridValueView[titleRow, chNameCol] = new SourceGrid.Cells.ColumnHeader("通道\r\n名称");
            this.gridValueView[titleRow, chNameCol].RowSpan = 2;
            this.gridValueView[titleRow, calcedAmpliCol] = new SourceGrid.Cells.ColumnHeader("待校准板");
            this.gridValueView[titleRow, calcedAmpliCol].ColumnSpan = 3;
            this.gridValueView[titleRow, refAmpliCol] = new SourceGrid.Cells.ColumnHeader("基准板");
            this.gridValueView[titleRow, refAmpliCol].ColumnSpan = 3;
            this.gridValueView[titleRow, freq1700CoefCol] = new SourceGrid.Cells.ColumnHeader("系数");
            this.gridValueView[titleRow, freq1700CoefCol].ColumnSpan = 4;
            this.gridValueView[titleRow, inaccuracyCol] = new SourceGrid.Cells.ColumnHeader("误差");
            this.gridValueView[titleRow, inaccuracyCol].RowSpan = 2;
            this.gridValueView[detailedTitleRow, calcedAmpliCol] = new SourceGrid.Cells.ColumnHeader("幅度");
            this.gridValueView[detailedTitleRow, calcedFreqCol] = new SourceGrid.Cells.ColumnHeader("载频");
            this.gridValueView[detailedTitleRow, calcedLowFreqCol] = new SourceGrid.Cells.ColumnHeader("低频");
            this.gridValueView[detailedTitleRow, refAmpliCol] = new SourceGrid.Cells.ColumnHeader("幅度");
            this.gridValueView[detailedTitleRow, refFreqCol] = new SourceGrid.Cells.ColumnHeader("载频");
            this.gridValueView[detailedTitleRow, refLowFreqCol] = new SourceGrid.Cells.ColumnHeader("低频");

            this.gridValueView[detailedTitleRow, freq1700CoefCol] = new SourceGrid.Cells.ColumnHeader("1700Hz");
            this.gridValueView[detailedTitleRow, freq2000CoefCol] = new SourceGrid.Cells.ColumnHeader("2000Hz");
            this.gridValueView[detailedTitleRow, freq2300CoefCol] = new SourceGrid.Cells.ColumnHeader("2300Hz");
            this.gridValueView[detailedTitleRow, freq2600CoefCol] = new SourceGrid.Cells.ColumnHeader("2600Hz");

            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, chNameCol] = new SourceGrid.Cells.RowHeader("通道" + (i + 1).ToString());

            }


            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, calcedAmpliCol] = new SourceGrid.Cells.Cell(i * 3);
                this.gridValueView[i + detailedTitleRow + 1, calcedAmpliCol].Editor = calcedValueEditor;
            }
            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, calcedFreqCol] = new SourceGrid.Cells.Cell(i * 3 + 1);
                this.gridValueView[i + detailedTitleRow + 1, calcedFreqCol].Editor = calcedValueEditor;

            }
            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, calcedLowFreqCol] = new SourceGrid.Cells.Cell(i * 3 + 2);
                this.gridValueView[i + detailedTitleRow + 1, calcedLowFreqCol].Editor = calcedValueEditor;

            }

            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, refAmpliCol] = new SourceGrid.Cells.Cell(i * 3);
                this.gridValueView[i + detailedTitleRow + 1, refAmpliCol].Editor = RefValueEditor;

            }

            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, refFreqCol] = new SourceGrid.Cells.Cell(i * 3 + 1);
                this.gridValueView[i + detailedTitleRow + 1, refFreqCol].Editor = RefValueEditor;

            }

            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, refLowFreqCol] = new SourceGrid.Cells.Cell(i * 3 + 2);
                this.gridValueView[i + detailedTitleRow + 1, refLowFreqCol].Editor = RefValueEditor;

            }
            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, inaccuracyCol] = new SourceGrid.Cells.Cell(i * 3);
                this.gridValueView[i + detailedTitleRow + 1, inaccuracyCol].Editor = inaccuracyEditor;

            }
            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, freq1700CoefCol] = new SourceGrid.Cells.Cell(i * 4);
                this.gridValueView[i + detailedTitleRow + 1, freq1700CoefCol].Editor = coefValueEditor;

            }

            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, freq2000CoefCol] = new SourceGrid.Cells.Cell(i * 4 + 1);
                this.gridValueView[i + detailedTitleRow + 1, freq2000CoefCol].Editor = coefValueEditor;

            }
            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, freq2300CoefCol] = new SourceGrid.Cells.Cell(i * 4 + 2);
                this.gridValueView[i + detailedTitleRow + 1, freq2300CoefCol].Editor = coefValueEditor;

            }
            for (int i = 0; i < 8; i++)
            {
                this.gridValueView[i + detailedTitleRow + 1, freq2600CoefCol] = new SourceGrid.Cells.Cell(i * 4 + 3);
                this.gridValueView[i + detailedTitleRow + 1, freq2600CoefCol].Editor = coefValueEditor;

            }

            this.gridValueView.AutoSize = true;
            this.gridValueView.AutoSizeCells();
            this.gridValueView.Columns.StretchToFit();
            this.gridValueView.Rows.StretchToFit();


        }





        private bool WriteNorScale()
        {
            return true;
        }


        private void ProcCalc()
        {

            CalcFullValues();
            Int16[] coefValues;
            if (!Funs485.RdScaleList(cardAddr, out coefValues))
            {
                AppendText(this.txtMessage, "读取系数失败");
                ShowText(this.btnStartCalc, "自动校准");
                return;
            }
            for (int i = 0; i < coefValues.Length; i++)
            {
                if (coefValues[i] < 0 || coefValues[i] == 0x0)
                {
                    coefValues[i] = 1024;
                }
            }
            coefValues.CopyTo(CoeffValues, 0);
            InvalidateCtrl(this.gridValueView);
            for (int i = 0; i < 4; i++)
            {
                DoCalcStep(i, CoeffValues);
            }
            if (!Funs485.WriteCoeff(cardAddr, CoeffValues))
            {
                ShowText(this.txtMessage, "系数写入失败");
            }
            WriteFactoryMessage(cardAddr);
            ShowText(this.btnStartCalc, "自动校准");
        }

        private void WriteFactoryMessage(byte cardAddr)
        {
            byte[] message = new byte[16];
            byte[] idStrBuf = Encoding.Default.GetBytes(cardId);
            int coypLen = idStrBuf.Length < 12 ? idStrBuf.Length : 12;
            Array.Copy(idStrBuf, message, coypLen);
            DateTime dt = DateTime.Now;
            message[12] = (byte)(dt.Year);
            message[13] = (byte)(dt.Year >> 8);
            message[14] = (byte)(dt.Month);
            message[15] = (byte)(dt.Day);
            Funs485.WrFactoryMessage(cardAddr, message);
            message = new byte[16];
            byte[] tsterNameBuf = Encoding.Default.GetBytes(testerName);
            coypLen = tsterNameBuf.Length < 16 ? tsterNameBuf.Length : 16;
            Array.Copy(tsterNameBuf, message, coypLen);
            Funs485.WrTsterMessage(cardAddr, message);

        }

        byte refCardNo = 33;

        /// <summary>
        /// 待校准的数值列列表
        /// </summary>
        Int16[] WaitCalcValues = new Int16[8 * 3];
        /// <summary>
        /// 基准板数值列列表
        /// </summary>
        Int16[] RefValues = new Int16[8 * 3];
        Int16[] CoeffValues = new Int16[8 * 4];
        delegate void InvalidateDelegate(Control ctl);

        private void InvalidateCtrl(Control ctl)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new InvalidateDelegate(InvalidateCtrl), new Object[] { ctl });
            }
            else
            {
                ctl.Invalidate();
            }
        }

        private bool DoCalcStep(int step, Int16[] scaleList)
        {
            Int16[] chValues = new Int16[100];
            Int16[] refValues = new Int16[100];
            SetFreq(step);

            DateTime startTime = DateTime.Now;
            AppendText(this.txtMessage, String.Format("开始校准频率{0},读值稳定后按任意键进入下一个频率\r\n", freqNameList[step]));
            waitKey.Reset();
            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                if (!Funs485.RdAllCh(cardAddr, out chValues))
                {
                    AppendText(this.txtMessage, String.Format("读取板{0}模拟量数据失败", cardAddr));
                    return false;
                }
                if (!Funs485.RdAllCh(refCardNo, out refValues))
                {
                    AppendText(this.txtMessage, String.Format("读取板{0}模拟量数据失败", cardAddr));
                    return false;
                }
                if (chValues.Length != refValues.Length)
                {
                    AppendText(this.txtMessage, String.Format("基准板与待校准板不一致"));
                    return false;
                }
                chValues.CopyTo(WaitCalcValues, 0);
                refValues.CopyTo(RefValues, 0);
                InvalidateCtrl(this.gridValueView);
                if (waitKey.WaitOne(0,true)) break;
            }
            for (int i = 0; i < chValues.Length / 3; i++)
            {
                double coef = scaleList[i * 4 + step] * 1.0;
                float refVal = refValues[i * 3];
                float realVal = chValues[i * 3];
                coef = coef * refVal;
                coef = coef / realVal;
                // coef *= coef * refVal /realVal;
                scaleList[i * 4 + step] = (Int16)Math.Round(coef);
            }
            scaleList.CopyTo(CoeffValues, 0);
            InvalidateCtrl(this.gridValueView);
            return true;

        }

        private void SetFreq(int step)
        {
            sigSource.SetFreq(0, freqList[step]);
            sigSource.SetFreq(1, interferenceFreqList[step]);
            sigSource.SetAmpli(freqFullValues[step] * 0.75f);
        }




        private int GetCalcStep()
        {
            return 4;
        }
        /// <summary>
        /// 是否需要写默认系数
        /// </summary>
        /// <returns></returns>
        private bool GetWriteDefaultState()
        {
            return true;
        }
        String cardId;
        String testerName;
        private void btnStartCalc_Click(object sender, EventArgs e)
        {
            cardAddr = (byte)int.Parse(FormMain.Instance.txtBoardAddr.Text);
            cardId = this.txtId.Text;
            testerName = this.txtTster.Text;
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
                btnStartCalc.Text = "开始校准";
            }
            else
            {
                FormMain.mainWorkThread = new Thread(new ThreadStart(ProcCalc));
                FormMain.mainWorkThread.IsBackground = true;
                btnStartCalc.Text = "停止校准";
                FormMain.mainWorkThread.Start();
            }
        }

        private void FormAutoCalc_KeyPress(object sender, KeyPressEventArgs e)
        {
            waitKey.Set();
        }






        public void Destory()
        {

        }

        private void btnRun_Click(object sender, EventArgs e)
        {

        }

        private void buttonExt1_Click(object sender, EventArgs e)
        {
            Int16[] coefValues;
            cardAddr = (byte)int.Parse(FormMain.Instance.txtBoardAddr.Text);
            Funs485.RdScaleList(cardAddr, out coefValues);
            if (coefValues == null) return;
            coefValues.CopyTo(CoeffValues, 0);
            InvalidateCtrl(this.gridValueView);

        }
        int firstChFullVal = 3000;
        float sendFullValue = 3.0f;
        private void btnRun_Click_1(object sender, EventArgs e)
        {
            cardAddr = (byte)int.Parse(FormMain.Instance.txtBoardAddr.Text);
            cardId = this.txtId.Text;
            testerName = this.txtTster.Text;
            firstChFullVal = int.Parse(this.txtChVal.Text);
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
                btnRun.Text = "开始测试";
            }
            else
            {
                FormMain.mainWorkThread = new Thread(new ThreadStart(ProcTest));
                FormMain.mainWorkThread.IsBackground = true;
                btnRun.Text = "停止测试";
                FormMain.mainWorkThread.Start();
            }
        }

        private void ProcTest()
        {

            Int16[] coefValues;
            String id;
            DateTime dt;
            if (!Funs485.RdIdMessage(cardAddr, out id,out dt))
            {
                AppendText(this.txtMessage, "读取板Id失败");
            }
            AppendText(this.txtMessage,"开始测试"+id+"\r\n");

            WriteToFile(DateTime.Now.ToString("F") + id);
            if (!Funs485.RdScaleList(cardAddr, out coefValues))
            {
                AppendText(this.txtMessage, "读取系数失败");
                ShowText(this.btnRun, "开始测试");
                return;
            }

            coefValues.CopyTo(CoeffValues, 0);
            InvalidateCtrl(this.gridValueView);
            CalcFullValues();
            for (int i = 0; i < 4; i++)
            {
                DoFreqTstStep(i, CoeffValues);
            }
            for (int i = 0; i < 4; i++)
            {
                DoAmpliTstStep(i, CoeffValues);
            }
            String ver;
            Funs485.RdVerMessage(cardAddr, out ver);
            AppendText(this.txtMessage,ver+"\r\n");
           // WriteFactoryMessage(cardAddr);
            ShowText(this.btnRun, "开始测试");
        }

        private void GetFullSendAmpli(int freqStep)
        {
            sendFullValue = 1;
            sigSource.SetFreq(freqList[freqStep]);
            sigSource.SetAmpli(sendFullValue);
            AppendText(this.txtMessage, "等待幅度稳定（8S），按任意键退出");

            Int16[] refValues = new Int16[100];
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < 8)
            {
                if (!Funs485.RdAllCh(refCardNo, out refValues))
                {
                    AppendText(this.txtMessage, String.Format("读取板{0}模拟量数据失败\r\n", cardAddr));
                    return;
                }
                //if (waitKey.WaitOne(0,true))
                //{
                //    break;
                //}
            }
            try
            {
                sendFullValue = (float)firstChFullVal / refValues[0];
                freqFullValues[freqStep] = sendFullValue;
            }
            catch
            {
            }

        }


        private bool DoFreqTstStep(int step, Int16[] scaleList)
        {
            Int16[] chValues = new Int16[100];
            Int16[] refValues = new Int16[100];
            SetFreq(step);
            sigSource.SetAmpli(freqFullValues[step] * 0.75f);
            DateTime startTime = DateTime.Now;
            AppendText(this.txtMessage, String.Format("开始频率{0}幅度一致性测试,读值稳定后按任意键进入下一个频率\r\n", freqNameList[step]));
            waitKey.Reset();
            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                if (!Funs485.RdAllCh(cardAddr, out chValues))
                {
                    AppendText(this.txtMessage, String.Format("读取板{0}模拟量数据失败", cardAddr));
                    return false;
                }
                if (!Funs485.RdAllCh(refCardNo, out refValues))
                {
                    AppendText(this.txtMessage, String.Format("读取板{0}模拟量数据失败", cardAddr));
                    return false;
                }
                if (chValues.Length != refValues.Length)
                {
                    AppendText(this.txtMessage, String.Format("基准板与待校准板不一致"));
                    return false;
                }
                chValues.CopyTo(WaitCalcValues, 0);
                refValues.CopyTo(RefValues, 0);
                InvalidateCtrl(this.gridValueView);
                if (waitKey.WaitOne(0,true)) break;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("幅度误差测试{0}:", step);
            for (int i = 0; i < chValues.Length / 3; i++)
            {

                float refVal = refValues[i * 3];
                float realVal = chValues[i * 3];
                sb.AppendFormat("{0}/{1}/{2}%,", realVal, refVal, realVal * 100 / refVal);

            }
            sb.AppendLine();
            AppendText(this.txtMessage, sb.ToString());
            WriteToFile(sb.ToString());
            InvalidateCtrl(this.gridValueView);
            return true;

        }
        float[] ampliTable = new float[] { 0.25f, 0.5f, 0.75f ,1.0f};
        private bool DoAmpliTstStep(int step, Int16[] scaleList)
        {
            Int16[] chValues = new Int16[100];
            Int16[] refValues = new Int16[100];
            sigSource.SetAmpli(ampliTable[step] * freqFullValues[step]);
            DateTime startTime = DateTime.Now;
            AppendText(this.txtMessage, String.Format("读值稳定后按任意键进入下一个幅度\r\n"));
            waitKey.Reset();
            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                if (!Funs485.RdAllCh(cardAddr, out chValues))
                {
                    AppendText(this.txtMessage, String.Format("读取板{0}模拟量数据失败", cardAddr));
                    return false;
                }
                if (!Funs485.RdAllCh(refCardNo, out refValues))
                {
                    AppendText(this.txtMessage, String.Format("读取板{0}模拟量数据失败", cardAddr));
                    return false;
                }
                if (chValues.Length != refValues.Length)
                {
                    AppendText(this.txtMessage, String.Format("基准板与待校准板不一致"));
                    return false;
                }
                chValues.CopyTo(WaitCalcValues, 0);
                refValues.CopyTo(RefValues, 0);
                InvalidateCtrl(this.gridValueView);
                if (waitKey.WaitOne(0,true)) break;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("幅度误差测试{0}:", step);
            for (int i = 0; i < chValues.Length / 3; i++)
            {

                float refVal = refValues[i * 3];
                float realVal = chValues[i * 3];
                sb.AppendFormat("{0}/{1}/{2}%,", realVal, refVal, realVal * 100 / refVal);

            }
            sb.AppendLine();
            AppendText(this.txtMessage, sb.ToString());
            WriteToFile(sb.ToString());
            scaleList.CopyTo(CoeffValues, 0);
            InvalidateCtrl(this.gridValueView);
            return true;

        }
        private void WriteToFile(String str)
        {
            File.AppendAllText(".\\TstData.dat", str);
        }

        private void buttonExt2_Click(object sender, EventArgs e)
        {
            cardAddr = (byte)int.Parse(FormMain.Instance.txtBoardAddr.Text);
            DateTime dt;
            String id;
            Funs485.RdIdMessage(cardAddr, out id, out dt);
            AppendText(this.txtMessage,
                String.Format("板卡ID：{0}，校准日期：{1:F}\r\n", id, dt));


        }

        private void buttonExt3_Click(object sender, EventArgs e)
        {
            cardId = this.txtId.Text;
            testerName = this.txtTster.Text;
            WriteFactoryMessage(cardAddr);
        }

        private void buttonExt4_Click(object sender, EventArgs e)
        {
            cardAddr = (byte)int.Parse(FormMain.Instance.txtBoardAddr.Text);
            String tster;
            Funs485.RdTsterMessage(cardAddr, out tster);
            AppendText(this.txtMessage,
                String.Format("测试人{0}", tster));
        }

        private void FormAutoCalc_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
            }
        }
        public bool fullValueChanged = true;
        public float fullValue = 3000;
        public float[] freqFullValues = new float[4];
        private void CalcFullValues()
        {
            if (fullValueChanged == false) return;
            fullValueChanged = false;
            for (int i = 0; i < 4; i++)
            {
                GetFullSendAmpli(i);
            }
        }
        private void txtChVal_TextChanged(object sender, EventArgs e)
        {
            int val;
            if (int.TryParse(txtChVal.Text, out val))
            {
                firstChFullVal = val;
            }
        }

        private void txtMessage_DoubleClick(object sender, EventArgs e)
        {
            this.txtMessage.Clear();
        }

        private void FormAutoCalc_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Destory();
        }
    }

    public class CoeffView : SourceGrid.Cells.Views.Cell
    {
        Int16[] dataSource;
        public CoeffView(Int16[] dataSource)
            : base()
        {
            this.dataSource = dataSource;
        }


    }
    /// <summary>
    /// 定义设备属性的输出字符串
    /// </summary>
    class DataSourceEditor : SourceGrid.Cells.Editors.EditorBase
    {
        protected Int16[] dataSource;
        public DataSourceEditor(Int16[] dataSource)
            : base(typeof(int))
        {
            this.dataSource = dataSource;
        }
        public override string ValueToDisplayString(object p_Value)
        {
            int index = (int)p_Value;
            if (index < 0) return "NUll";
            if (dataSource == null) return "NULL";
            if (index >= dataSource.Length) return "NULL";
            return dataSource[index].ToString();
        }


    }

    /// <summary>
    /// 定义设备属性的输出字符串
    /// </summary>
    class CoefEditor : DataSourceEditor
    {

        public CoefEditor(Int16[] dataSource)
            : base(dataSource)
        {

        }
        public override string ValueToDisplayString(object p_Value)
        {
            int index = (int)p_Value;
            if (index < 0) return "NUll";
            if (dataSource == null) return "NULL";
            if (index >= dataSource.Length) return "NULL";
            return (dataSource[index] * 1.0 / 1024).ToString("0.000") + "/" + dataSource[index].ToString();
        }


    }

    /// <summary>
    /// 定义设备属性的输出字符串
    /// </summary>
    class InaccuracyEditor : SourceGrid.Cells.Editors.EditorBase
    {
        protected Int16[] dataSource;
        protected Int16[] dataSource2;
        public InaccuracyEditor(Int16[] dataSource, Int16[] dataSource2)
            : base(typeof(int))
        {
            this.dataSource = dataSource;
            this.dataSource2 = dataSource2;
        }
        public override string ValueToDisplayString(object p_Value)
        {
            int index = (int)p_Value;
            if (index < 0) return "NUll";
            if (dataSource == null) return "NULL";
            if (index >= dataSource.Length) return "NULL";
            if (dataSource2 == null) return "NULL";
            if (index >= dataSource2.Length) return "NULL";
            float inaccuracy = (dataSource2[index] - dataSource[index]) * 1.0f /*/ dataSource[index]*/;
            return String.Format("{0}", inaccuracy);
        }


    }
}
