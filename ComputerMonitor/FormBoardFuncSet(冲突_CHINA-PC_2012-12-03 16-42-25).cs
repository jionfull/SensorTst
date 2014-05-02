using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using ComputerMonitor.Protocol;
using WeifenLuo.WinFormsUI.Docking;
using System.Globalization;

namespace ComputerMonitor
{
    public partial class FormBoardFuncSet : DockContent
    {
        TextBox textBoxBoardAddr;
        string firmware = "";
        int boardTypeDetail = 0;
        int boardAddress = -1;
        int boardAddress1 = -1;
        byte majorNum = 0;
        byte minorNum = 0;
        byte KGBChannel = 0;
        int KGBInternal = 1000;
        byte MRChannel = 0;
        int MRInternal = 1000;

       
      
      
    
        Thread threadUnLock = null;

        byte[] dataBuffer = new byte[64 * 1024];
        float k, b;
        public FormBoardFuncSet()
        {
            InitializeComponent();
        }

        private void FormBoardFuncSet_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
           
            comboBox3.SelectedIndex = 0;

            dgvCalcDc.Rows.Add(6);
            dgvCalcYp.Rows.Add(7);
            textBoxBoardAddr = ((FormMain)(this.Parent.Parent)).txtBoardAddr;
            k = 1;
            b = 0;
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = ofd.FileName;
                }
            }
        }
        delegate void UpdateUICallback();
        void UpdateUI()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateUICallback(UpdateUI));
            }
            else
            {
                buttonUpDate.Text = "开始升级";
            }
        }
        Thread threadUpdate = null;

        private void buttonUpDate_Click(object sender, EventArgs e)
        {
            if (threadUpdate != null && threadUpdate.IsAlive)
            {
                threadUpdate.Abort();
                buttonUpDate.Text = "开始升级";
                return;
            }
            else
            {
                firmware = textBox1.Text;
                if (string.IsNullOrEmpty(firmware))
                {
                    MessageBox.Show("请先选择要升级的固件文件！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (File.Exists(firmware) == false)
                {
                    MessageBox.Show("所选的文件不存在！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                boardTypeDetail = comboBox1.SelectedIndex + 1;
                boardAddress = byte.Parse(textBoxBoardAddr.Text);
                majorNum = byte.Parse(textBox2.Text);
                minorNum = byte.Parse(textBox3.Text);

                threadUpdate = new Thread(new ThreadStart(ProcUpDate));
                threadUpdate.IsBackground = true;
                threadUpdate.Start();
                buttonUpDate.Text = "停止升级";
            }

        }

        delegate void ShowProcessCallbac(float rate);
        private void ShowProcess(float rate)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowProcessCallbac(ShowProcess), new object[] { rate });
            }
            else
            {
                if (rate < 0)
                {
                    progressBar1.Value = 0;
                    buttonUpDate.Text = "开始升级";
                    MessageBox.Show("升级完成");


                }
                else
                {
                    progressBar1.Value = (int)(progressBar1.Maximum * rate + 0.5f);
                    if (progressBar1.Value >= progressBar1.Maximum)
                    {
                        buttonUpDate.Enabled = true;
                    }
                }
            }
        }

        delegate void ShowProgrssDelegate(ProgressBar pgb, int val);
        private void ShowProgress(ProgressBar pgb, int val)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowProgrssDelegate(ShowProgress), new object[] { pgb, val });

            }
            else
            {
                pgb.Value = val;
            }
        }

        delegate void DelegateShowText(Control ctl, String txt);
        private void ShowText(Control ctl, String txt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateShowText(ShowText), new object[] { ctl, txt });
            }
            else
            {
                ctl.Text = txt;
            }
        }

        delegate void DelegateAppendText(TextBox txtBox, String txt);
        private void AppendText(TextBox txtBox, String txt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateAppendText(AppendText), new object[] { txtBox, txt });
            }
            else
            {
                txtBox.AppendText(txt);
            }
        }

        private void ProcUpDate()
        {
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 2);
            try
            {

                using (FileStream fs = new FileStream(firmware, FileMode.Open))
                {
                    long length = fs.Length;
                    long totalLeng = length;
                    byte[] buffer = null;
                    int limit = 0;
                    if (boardTypeDetail <= 2)
                    {
                        limit = 80;
                        buffer = new byte[limit + 7];
                    }
                    else if (boardTypeDetail <= 4)
                    {
                        limit = 8192;
                        buffer = new byte[limit + 7];
                    }
                    else if (boardTypeDetail == 9)
                    {
                        limit = 1024;
                        buffer = new byte[limit + 7];
                    }
                    else if (boardTypeDetail == 13)
                    {
                        limit = 512;
                        buffer = new byte[limit + 7];
                    }
                    else if (boardTypeDetail == 14)
                    {
                        limit = 512;
                        buffer = new byte[limit + 7];
                    }
                    else if (boardTypeDetail == 16)
                    {
                        limit = 512;
                        buffer = new byte[limit + 7];
                    }
                    else
                    {
                        limit = 32768;
                        buffer = new byte[limit + 7];
                    }
                    int startIndex = 0;
                    while (length > 0)
                    {
                        if (length > limit)
                        {
                            fs.Read(buffer, 5, limit);
                            length -= limit;
                            buffer[0] = (byte)(startIndex & 0xff);
                            buffer[1] = (byte)((startIndex >> 8) & 0xff);
                            startIndex++;
                            buffer[2] = (byte)boardTypeDetail;
                            buffer[3] = majorNum;
                            buffer[4] = minorNum;
                            ushort crc = Helper.CRC16.ComputeCRC16(buffer, 0, limit + 5);
                            buffer[limit + 5] = (byte)(crc & 0xff);
                            buffer[limit + 6] = (byte)((crc >> 8) & 0xff);

                            Protocol.FrameTransmit.Send(1, boardAddress, 2, buffer, limit + 7);
                            ShowProcess(1 - (length * 1.0f / totalLeng));
                            //Thread.Sleep(50);
                            //等待功能板响应
                            frameUnit.WaitData(-1);
                            Thread.Sleep(10);
                        }
                        else
                        {
                            int rdLen = fs.Read(buffer, 5, limit);
                            length -= rdLen;
                            buffer[0] = (byte)(startIndex & 0xff);
                            buffer[1] = (byte)((startIndex >> 8) & 0xff | 0x80);
                            startIndex++;
                            buffer[2] = (byte)boardTypeDetail;
                            buffer[3] = majorNum;
                            buffer[4] = minorNum;
                            ushort crc = Helper.CRC16.ComputeCRC16(buffer, 0, rdLen + 5);
                            buffer[rdLen + 5] = (byte)(crc & 0xff);
                            buffer[rdLen + 6] = (byte)((crc >> 8) & 0xff);

                            Protocol.FrameTransmit.Send(1, boardAddress, 2, buffer, rdLen + 7);
                            ShowProcess(1 - (length * 1.0f / totalLeng));
                            frameUnit.WaitData(-1);
                            Thread.Sleep(10);

                        }

                    }
                    fs.Close();

                }
                ShowProcess(-1);
            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            boardTypeDetail = comboBox1.SelectedIndex + 1;
            boardAddress = byte.Parse(textBoxBoardAddr.Text);
            byte[] buffer = new byte[1];
            buffer[0] = (byte)boardTypeDetail;
            Protocol.FrameTransmit.Send(1, boardAddress, 3, buffer, 1);
        }

        private void buttonStartBoard_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            boardAddress = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress, 5, buffer, 0);
        }

        private void buttonStopBoard_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            boardAddress = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress, 6, buffer, 0);
        }

        private void buttonQuery_Click(object sender, EventArgs e)
        {
            boardTypeDetail = comboBox1.SelectedIndex + 1;
            boardAddress = byte.Parse(textBoxBoardAddr.Text);
            byte[] buffer = new byte[1];
            buffer[0] = (byte)boardTypeDetail;
            Protocol.FrameTransmit.Send(1, boardAddress, 7, buffer, 1);
               StringBuilder sb = new StringBuilder();
               byte[] dataBuffer = new byte[40];
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x07);
         
            if (frameUnit.WaitData(2000) == false)
            {
                        MessageBox.Show("查询失败");
                       
                       
            }
            else
            {
                MessageBox.Show("版本号：" + dataBuffer[7].ToString() + "." + dataBuffer[8].ToString() + "\r\n"
                    + "版本日期：" + dataBuffer[9].ToString() + "年" + dataBuffer[10].ToString() + "月" + dataBuffer[11].ToString());
            }
        }

        private void buttonQueryStatus_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            boardAddress = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress, 0x10, buffer, 0);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            boardAddress = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress, 0x0f, buffer, 0);
        }

        /// <summary>
        /// 开关板采集
        /// </summary>
        private void ProcKGB()
        {
            try
            {
                byte[] buffer = new byte[1];
                buffer[0] = KGBChannel;

                while (true)
                {
                    Protocol.FrameTransmit.Send(1, boardAddress1, 0x20, buffer, 1);
                    Thread.Sleep(KGBInternal);
                }

            }
            catch (ThreadAbortException)
            {

            }
        }

        private void buttonKGB_Click(object sender, EventArgs e)
        {
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            KGBChannel = byte.Parse(textBox5.Text);
            KGBInternal = int.Parse(textBox6.Text);
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
                buttonKGB.Text = "开始采集";
            }
            else
            {
                if (checkBox1.Checked == false)
                {
                    byte[] buffer = new byte[1];
                    buffer[0] = KGBChannel;
                    Protocol.FrameTransmit.Send(1, boardAddress1, 0x20, buffer, 1);
                }
                else
                {
                    buttonKGB.Text = "停止采集";
                    FormMain.mainWorkThread = new Thread(new ThreadStart(ProcKGB));
                    FormMain.mainWorkThread.IsBackground = true;
                    FormMain.mainWorkThread.Start();
                }
            }
        }

        /// <summary>
        /// 开关板采集
        /// </summary>


        private void ProcMR()
        {
            StringBuilder sb = new StringBuilder();
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x30);
            try
            {
                byte[] buffer = new byte[1];
                buffer[0] = MRChannel;

                while (true)
                {
                    Protocol.FrameTransmit.Send(1, boardAddress1, 0x30, buffer, 1);
                    if (frameUnit.WaitData(3000) == false)
                    {
                        MessageBox.Show("收不到数据");
                        ShowText(buttonMR, "开始采集");
                        break;
                    }
                    else
                    {
                        sb.Length = 0;
                        for (int i = 8; i < frameUnit.FrameLength - 4; i += 2)
                        {
                            Int16 chVal;
                            chVal = (Int16)(dataBuffer[i] + dataBuffer[i + 1] * 256);
                            sb.Append("通道" + ((i - 8) / 2).ToString() + ":   " + chVal.ToString() + " ");
                            sb.Append("\r\n");
                        }



                        ShowText(txtChVal, sb.ToString());

                    }
                    Thread.Sleep(MRInternal);
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

        private bool ReadAnalogCh(byte ch, out Int16 val)
        {

            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x30); //接收任何数据帧
            bool ret;
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            try
            {
                byte[] buffer = new byte[1];
                buffer[0] = ch;


                Protocol.FrameTransmit.Send(1, boardAddress1, 0x30, buffer, 1);
                if (frameUnit.WaitData(3000) == false)
                {
                    MessageBox.Show("收不到数据");
                    ShowText(buttonMR, "开始采集");
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
        private void buttonMR_Click(object sender, EventArgs e)
        {
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            MRChannel = byte.Parse(textBox8.Text);
            MRInternal = int.Parse(textBox7.Text);
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
                buttonMR.Text = "开始采集";
            }
            else
            {
                if (checkBox2.Checked == false)
                {
                    byte[] buffer = new byte[1];
                    buffer[0] = MRChannel;
                    Protocol.FrameTransmit.Send(1, boardAddress1, 0x30, buffer, 1);
                }
                else
                {
                    buttonMR.Text = "停止采集";
                    FormMain.mainWorkThread = new Thread(new ThreadStart(ProcMR));
                    FormMain.mainWorkThread.IsBackground = true;
                    FormMain.mainWorkThread.Start();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            byte[] buffer = new byte[1];
            Protocol.FrameTransmit.Send(1, boardAddress1, 0x40, buffer, 1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            byte[] buffer = new byte[1];
            Protocol.FrameTransmit.Send(1, boardAddress1, 0x50, buffer, 1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] txtVals = textBox9.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] buffer = null;
            int count = txtVals.Length;
            if (count > 0 && count <= 6)
            {
                buffer = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    buffer[i] = byte.Parse(txtVals[i]);
                }
                boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
                Protocol.FrameTransmit.Send(1, boardAddress1, 0x63, buffer, count);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string[] txtVals = textBox9.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] buffer = null;
            int count = txtVals.Length;
            if (count > 0 && count <= 6)
            {
                buffer = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    buffer[i] = byte.Parse(txtVals[i]);
                }
                boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
                Protocol.FrameTransmit.Send(1, boardAddress1, 0x64, buffer, count);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            buffer[0] = byte.Parse(textBox10.Text);
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress1, 0x60, buffer, 1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            buffer[0] = byte.Parse(textBox10.Text);
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress1, 0x61, buffer, 1);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            buffer[0] = byte.Parse(textBox10.Text);
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress1, 0x62, buffer, 1);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            buffer[0] = byte.Parse(textBox10.Text);
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress1, 0x70, buffer, 1);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)((comboBox3.SelectedIndex) << 6);
            int channel = int.Parse(textBox11.Text);
            buffer[1] = (byte)(channel & 0xff);
            buffer[0] = (byte)((buffer[0]) | (((channel >> 8) & 0x3) << 4));
            int interval = int.Parse(textBox12.Text);
            buffer[2] = (byte)(interval & 0xff);

            buffer[3] = (byte)((interval >> 8) & 0xff);
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress1, 0xa0, buffer, 4);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)((comboBox3.SelectedIndex) << 6);
            int channel = int.Parse(textBox11.Text);
            buffer[1] = (byte)(channel & 0xff);
            buffer[0] = (byte)((buffer[0]) | (((channel >> 8) & 0x3) << 4));
            int interval = int.Parse(textBox12.Text);
            buffer[2] = (byte)(interval & 0xff);

            buffer[3] = (byte)((interval >> 8) & 0xff);
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress1, 0xa1, buffer, 4);
        }

        private void ProcScan()
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sbLog = new StringBuilder();
            StringBuilder sbAll = new StringBuilder();
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x30); //接收任何数据帧
            float maxAmpliVal, minAmpliVal;
            float maxFreqVal, minFreqVal;
            float maxModFreqVal, minModFreqVal;



            Lon.IO.Ports.FunGen fGen = new Lon.IO.Ports.FunGen();

            try
            {
                byte[] buffer = new byte[1];
                buffer[0] = MRChannel;
                float tempVal;

                for (int i = 0; i < ((float)(numScanEnd.Value) - (float)(numScanStart.Value) + 0.1f) * 10; i++)
                {
                    while (fGen.SetFmFreq(0, (float)numScanStart.Value + i * 0.1f) == false)
                    {
                        MessageBox.Show("3022未准备好");
                    }

                    sbLog.Length = 0;
                    sbLog.Append("!!!!!!!!!");
                    sbLog.Append(DateTime.Now.ToLongTimeString());
                    sbLog.Append("    3022输出低频：");
                    sbLog.Append(((float)numScanStart.Value + i * 0.1f).ToString());
                    sbLog.Append("//{{{1\r\n");
                    AppendText(txtLog, sbLog.ToString());
                    Thread.Sleep(1000);


                    Protocol.FrameTransmit.Send(1, boardAddress1, 0x30, buffer, 1);
                    if (frameUnit.WaitData(3000) == false)
                    {
                        MessageBox.Show("收不到数据");
                        ShowText(btnLowScan, "开始扫描");
                        break;
                    }
                    else
                    {
                        sb.Length = 0;
                        for (int k = 8; k < frameUnit.FrameLength - 4; k += 2)
                        {
                            float chVal;
                            chVal = dataBuffer[k] + dataBuffer[k + 1] * 256;
                            sb.Append("通道" + ((k - 8) / 2).ToString() + ":   " + chVal.ToString() + " ");

                            sb.Append("\r\n");
                        }
                        ShowText(txtVal, sb.ToString());
                        sbAll.Append(DateTime.Now.ToLongTimeString());
                        sbAll.Append(":\r\n");
                        sbAll.Append(sb.ToString());
                        sbAll.Append(":\r\n");

                        sbLog.Length = 0;
                        sbLog.Append("时间\t电压\t频率\t低频//{{{2\r\n");
                        sbLog.Append(DateTime.Now.ToLongTimeString());
                        sbLog.Append("\t");
                        tempVal = dataBuffer[8 + (int)numScanCh.Value * 2] + dataBuffer[9 + (int)numScanCh.Value * 2] * 256;
                        maxAmpliVal = tempVal;
                        minAmpliVal = tempVal;
                        sbLog.Append(tempVal.ToString());
                        sbLog.Append("\t");
                        tempVal = dataBuffer[10 + (int)numScanCh.Value * 2] + dataBuffer[11 + (int)numScanCh.Value * 2] * 256;
                        maxFreqVal = tempVal;
                        minFreqVal = tempVal;
                        sbLog.Append(tempVal.ToString());
                        sbLog.Append("\t");
                        tempVal = dataBuffer[12 + (int)numScanCh.Value * 2] + dataBuffer[13 + (int)numScanCh.Value * 2] * 256;
                        maxModFreqVal = tempVal;
                        minModFreqVal = tempVal;
                        sbLog.Append(tempVal.ToString());
                        sbLog.Append("\r\n");
                        AppendText(txtLog, sbLog.ToString());

                    }
                    #region 开始采集

                    for (int j = 0; j < (int)(numTstTime.Value * 60); j++)
                    {
                        Thread.Sleep(1000);
                        Protocol.FrameTransmit.Send(1, boardAddress1, 0x30, buffer, 1);
                        if (frameUnit.WaitData(30) == false)
                        {
                            MessageBox.Show("收不到数据");
                            ShowText(btnLowScan, "开始扫描");
                            break;
                        }
                        else
                        {
                            sb.Length = 0;
                            for (int k = 8; k < frameUnit.FrameLength - 4; k += 2)
                            {
                                float chVal;
                                chVal = dataBuffer[k] + dataBuffer[k + 1] * 256;
                                sb.Append("通道" + ((k - 8) / 2).ToString() + ":   " + chVal.ToString() + " ");
                                sb.Append("\r\n");
                            }
                            ShowText(txtVal, sb.ToString());
                            sbAll.Append(DateTime.Now.ToLongTimeString());
                            sbAll.Append(":\r\n");
                            sbAll.Append(sb.ToString());
                            sbAll.Append(":\r\n");

                            sbLog.Length = 0;

                            sbLog.Append(DateTime.Now.ToLongTimeString());
                            sbLog.Append("\t");
                            tempVal = dataBuffer[8 + (int)numScanCh.Value * 2] + dataBuffer[9 + (int)numScanCh.Value * 2] * 256;
                            if (tempVal > maxAmpliVal)
                            {
                                maxAmpliVal = tempVal;
                            }
                            if (tempVal < maxAmpliVal)
                            {
                                minAmpliVal = tempVal;
                            }
                            sbLog.Append(tempVal.ToString());
                            sbLog.Append("\t");
                            tempVal = dataBuffer[10 + (int)numScanCh.Value * 2] + dataBuffer[11 + (int)numScanCh.Value * 2] * 256;
                            if (tempVal > maxFreqVal)
                            {
                                maxFreqVal = tempVal;
                            }
                            if (tempVal < minFreqVal)
                            {
                                minFreqVal = tempVal;
                            }
                            sbLog.Append(tempVal.ToString());
                            sbLog.Append("\t");
                            tempVal = dataBuffer[12 + (int)numScanCh.Value * 2] + dataBuffer[13 + (int)numScanCh.Value * 2] * 256;
                            if (tempVal > maxModFreqVal)
                            {
                                maxModFreqVal = tempVal;
                            }
                            if (tempVal < minModFreqVal)
                            {
                                minModFreqVal = tempVal;
                            }
                            sbLog.Append(tempVal.ToString());
                            sbLog.Append("\r\n");
                            AppendText(txtLog, sbLog.ToString());
                        }

                    }
                    #endregion

                    sbLog.Length = 0;
                    sbLog.Append("//}}}2\r\n");
                    sbLog.Append("!!!!!!!!!");
                    sbLog.Append(DateTime.Now.ToLongTimeString());
                    sbLog.Append("    3022测试低频结束\r\n");
                    sbLog.Append("!!!!!!!!!幅度最大值:");
                    sbLog.Append(maxAmpliVal);
                    sbLog.Append("\r\n");
                    sbLog.Append("!!!!!!!!!幅度最小值:");
                    sbLog.Append(minAmpliVal);
                    sbLog.Append("\r\n");
                    sbLog.Append("!!!!!!!!!频率最大值:");
                    sbLog.Append(maxFreqVal);
                    sbLog.Append("\r\n");
                    sbLog.Append("!!!!!!!!!频率最小值:");
                    sbLog.Append(minFreqVal);
                    sbLog.Append("\r\n");
                    sbLog.Append("!!!!!!!!!调制频率最大值:");
                    sbLog.Append(maxModFreqVal);
                    sbLog.Append("\r\n");
                    sbLog.Append("!!!!!!!!!调制频率最小值:");
                    sbLog.Append(minModFreqVal);
                    sbLog.Append("\r\n");
                    sbLog.Append(((float)numScanStart.Value + i * 0.1f).ToString());
                    sbLog.Append("//}}}1\r\n");
                    AppendText(txtLog, sbLog.ToString());

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
        private void btnLowScan_Click(object sender, EventArgs e)
        {
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            MRChannel = byte.Parse(textBox8.Text);
            MRInternal = int.Parse(textBox7.Text);
         
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
                btnLowScan.Text = "开始扫描";
            }
            else
            {
                btnLowScan.Text = "停止扫描";
                FormMain.mainWorkThread = new Thread(new ThreadStart(ProcScan));
                FormMain.mainWorkThread.IsBackground = true;
                FormMain.mainWorkThread.Start();

            }
        }

        private void btnReadDReg_Click(object sender, EventArgs e)
        {
            Int16 addr;
            StringBuilder sb = new StringBuilder();
            byte[] rxBuf = new byte[1000];
            FrameUnit frameUnit = FrameManager.CreateFrame(rxBuf, 0xf0); //接收任何数据帧
            try
            {
                boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
                addr = Int16.Parse(txtDebugRegAddr.Text, System.Globalization.NumberStyles.HexNumber);
                byte[] buffer = new byte[3];
                buffer[0] = (byte)(addr & 0xff);
                buffer[1] = (byte)(addr >> 8);
                Protocol.FrameTransmit.Send(1, boardAddress1, 0xf0, buffer, 2);
                if (frameUnit.WaitData(3000) == false)
                {
                    MessageBox.Show("收不到数据");

                }
                else
                {
                    ShowText(txtDebugRegVal, (rxBuf[9] + rxBuf[10] * 256).ToString("X2"));

                }

            }
            catch
            {
                MessageBox.Show("数字非法");
            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }


        }
        private bool WriteReg(int boardAddr, UInt16 regAddr, UInt16 val)
        {
            bool ret;
            byte[] rxBuf = new byte[1000];
            FrameUnit frameUnit = FrameManager.CreateFrame(rxBuf, 0xf1);
            try
            {
                byte[] buffer = new byte[4];
                buffer[0] = (byte)(regAddr & 0xff);
                buffer[1] = (byte)(regAddr >> 8);
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
        private void btnWriteDReg_Click(object sender, EventArgs e)
        {
            UInt16 addr, val;
            StringBuilder sb = new StringBuilder();
            byte[] rxBuf = new byte[1000];
            FrameUnit frameUnit = FrameManager.CreateFrame(rxBuf, 0xf1);

            try
            {
                boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
                addr = UInt16.Parse(txtDebugRegAddr.Text, System.Globalization.NumberStyles.HexNumber);
                val = UInt16.Parse(txtDebugRegVal.Text, System.Globalization.NumberStyles.HexNumber);
                //   val &= 0xffff;
                byte[] buffer = new byte[4];
                buffer[0] = (byte)(addr & 0xff);
                buffer[1] = (byte)(addr >> 8);
                buffer[2] = (byte)(val & 0xff);
                buffer[3] = (byte)(val >> 8);
                Protocol.FrameTransmit.Send(1, boardAddress1, 0xf1, buffer, 4);
                if (frameUnit.WaitData(3000) == false)
                {
                    MessageBox.Show("收不到数据");

                }
                else
                {
                    UInt16 temp;
                    temp = (UInt16)((UInt16)rxBuf[9] + (UInt16)(rxBuf[10] * 256));
                    if (temp == val)
                    {

                        MessageBox.Show("写入成功");
                    }

                }

            }
            catch
            {
                MessageBox.Show("数字非法");
            }
            finally
            {
                FrameManager.DeleteFrame(frameUnit);
            }
        }

        private void  ProcWriteMessage()
        {
            Int16 addr;
            StringBuilder sb = new StringBuilder();
            byte[] rxBuf = new byte[1000];
            FrameUnit frameUnit = FrameManager.CreateFrame(rxBuf, 0xf1); //接收任何数据帧
            byte[] txBuf = new byte[4];
            bool isOk = true;

            try
            {
                boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
                addr = 0x0100;
                for (int i = 0; i * 2 < (txtMessage.Text.Length + 1) && i < 0x255; i++)
                {
                    addr = (Int16)(0x100 + i);
                    txBuf[0] = (byte)(addr & 0xff);
                    txBuf[1] = (byte)(addr >> 0x8);

                    if (i * 2 + 1 < txtMessage.Text.Length)
                    {
                        txBuf[2] = (byte)(txtMessage.Text[i * 2]);
                        txBuf[3] = (byte)(txtMessage.Text[i * 2 + 1]);
                    }
                    else
                    {
                        if (i * 2 == txtMessage.Text.Length)
                        {
                            txBuf[2] = (byte)0x00;
                            txBuf[3] = (byte)0x00;
                        }
                        else
                        {
                            txBuf[2] = (byte)(txtMessage.Text[i * 2]);
                            txBuf[3] = (byte)0x00;

                        }
                    }

                    Protocol.FrameTransmit.Send(1, boardAddress1, 0xf1, txBuf, 4);

                    if (frameUnit.WaitData(3000) == false)
                    {
                        MessageBox.Show("等待超时");
                        isOk = false;
                        break;
                    }
                    else
                    {
                        ShowProgress(pgbWriteMessage, (int)((i / txtMessage.Text.Length) * 100));
                    }


                }


            }
            catch
            {
            }
            finally
            {
                if (isOk)
                {
                    MessageBox.Show("写入完成");
                }
                ShowText(btnWriteMessage, "写入");

            }
        }
        private void btnWriteMessage_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
               
            }
           
                ShowText(btnWriteMessage, "停止");
                FormMain.mainWorkThread = new Thread(new ThreadStart(ProcWriteMessage));
                FormMain.mainWorkThread.IsBackground = true;
                FormMain.mainWorkThread.Start();
          
        }
        private void ProcRdMessage()
        {
            Int16 addr;
            StringBuilder sb = new StringBuilder();
            byte[] rxBuf = new byte[1000];
            FrameUnit frameUnit = FrameManager.CreateFrame(rxBuf, 0xf0);
            byte[] txBuf = new byte[4];
            bool isOk = true;
            Encoding myEencoding = Encoding.GetEncoding(936);

            try
            {
                boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
                addr = 0x0100;
                for (int i = 0; i < 255; i++)
                {
                    addr = (Int16)(0x100 + i);
                    txBuf[0] = (byte)(addr & 0xff);
                    txBuf[1] = (byte)(addr >> 0x8);
                    Protocol.FrameTransmit.Send(1, boardAddress1, 0xf0, txBuf, 2);
                    if (frameUnit.WaitData(3000) == false)
                    {
                        MessageBox.Show("等待超时");
                        isOk = false;
                        break;
                    }
                    else
                    {
                        ShowProgress(pgbWriteMessage, (int)(i * 100 / 255));
                        if (rxBuf[9] == 0x00)
                        {
                            break;

                        }
                        sb.Append((char)rxBuf[9]);

                        if (rxBuf[10] == 0x00)
                        {
                            break;
                        }
                        sb.Append((char)rxBuf[10]);


                    }


                }

            }
            catch
            {
            }
            finally
            {
                if (isOk)
                {
                    MessageBox.Show("已读出");
                }
                ShowText(btnRdMessage, "读出");
                ShowText(txtMessage, sb.ToString());

            }
        }

        private void btnRdMessage_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
                ShowText(btnRdMessage, "读出");
            }
            ShowText(btnRdMessage, "停止");
            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcRdMessage));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();

           
        }

        private void ThreadUnLock()
        {

            try
            {
                boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
                if (WriteReg(boardAddress1, (UInt16)0xffff, (UInt16)0x55aa) == false)
                {
                    MessageBox.Show("解锁步骤1失败");
                    return;
                }
                if (WriteReg(boardAddress1, (UInt16)0xffff, (UInt16)0xaa55) == false)
                {
                    MessageBox.Show("解锁步骤2失败");
                    return;
                }
                MessageBox.Show("解锁成功");


            }
            catch
            {
            }
            finally
            {
            }

        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();

            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ThreadUnLock));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();


        }

        private void ThreadLock()
        {
            try
            {
                boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
                if (WriteReg(boardAddress1, (UInt16)0xffff, (UInt16)0xaa55) == false)
                {
                    MessageBox.Show("加锁步骤1失败");
                    return;
                }
                if (WriteReg(boardAddress1, (UInt16)0xffff, (UInt16)0x55aa) == false)
                {
                    MessageBox.Show("加锁步骤2失败");
                    return;
                }
                MessageBox.Show("加锁成功");

            }
            catch
            {
            }
            finally
            {
            }

        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive)
            {
                FormMain.mainWorkThread.Abort();
            }
            else
            {
                ShowText(btnRdMessage, "停止");
                FormMain.mainWorkThread = new Thread(new ThreadStart(ThreadLock));
                FormMain.mainWorkThread.IsBackground = true;
                FormMain.mainWorkThread.Start();
            }

        }

        private void btnSaveScale_Click(object sender, EventArgs e)
        {
            boardAddress1 = byte.Parse(textBoxBoardAddr.Text);
            if (WriteReg(boardAddress1, (UInt16)0x210, (UInt16)0xaa55) == false)
            {
                MessageBox.Show("写入失败");
            }
            else
            {
                MessageBox.Show("写入成功");
            }


        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void label42_Click(object sender, EventArgs e)
        {

        }




        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Int16 val;
            if (e.ColumnIndex == 0)
            {
                for (int i = 0; i < dgvCalcDc.Rows.Count; i++)
                {
                    if (ReadAnalogCh((byte)i, out val) == true)
                    {
                        dgvCalcDc.Rows[i].Cells[0].Value = val.ToString();

                    }
                    else
                    {
                        MessageBox.Show("未读到数据");
                    }
                }
                MessageBox.Show("操作完成");
                dgvCalcDc.EndEdit();
            }
            else if (e.ColumnIndex == 1)
            {
                if (ReadAnalogCh((byte)e.RowIndex, out val) == true)
                {
                    dgvCalcDc.Rows[e.RowIndex].Cells[1].Value = val.ToString();

                }
                else
                {
                    MessageBox.Show("未读到数据");
                }
                MessageBox.Show("操作完成");
                dgvCalcDc.EndEdit();
            }
            dgvCalcDc.EndEdit();
        }

        private void btnCalcDc_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < dgvCalcDc.Rows.Count; i++)
                {
                    dgvCalcDc.Rows[i].Cells[3].Value = (Int16.Parse(dgvCalcDc.Rows[i].Cells[0].Value.ToString())).ToString("X4");
                    int temp = Int16.Parse(dgvCalcDc.Rows[i].Cells[1].Value.ToString()) - Int16.Parse(dgvCalcDc.Rows[i].Cells[0].Value.ToString());
                    dgvCalcDc.Rows[i].Cells[4].Value = (Int16.Parse(dgvCalcDc.Rows[i].Cells[2].Value.ToString()) * 4096
                        / (Int16)(Int16.Parse(dgvCalcDc.Rows[i].Cells[1].Value.ToString()) - Int16.Parse(dgvCalcDc.Rows[i].Cells[0].Value.ToString()))).ToString("X4");
                }
            }
            catch
            {
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvCalcDc.Rows.Count; i++)
            {
                /*
                if (WriteReg(2, (byte)i, (UInt16)UInt16.Parse(dgvCalcDc.Rows[i].Cells[3].Value.ToString(), System.Globalization.NumberStyles.HexNumber)) == false)
                {
                    MessageBox.Show("写入失败");
                    return;
                }
                if (WriteReg(2, (byte)(i + 0x10), (UInt16)UInt16.Parse(dgvCalcDc.Rows[i].Cells[4].Value.ToString(), System.Globalization.NumberStyles.HexNumber)) == false)
                {
                    MessageBox.Show("写入失败");
                    return;
                }
                 * */
                UInt16 val = 0;
                byte board = (byte)UInt16.Parse(textBoxBoardAddr.Text);
                val = UInt16.Parse(dgvCalcDc.Rows[i].Cells[3].Value.ToString(), System.Globalization.NumberStyles.HexNumber);
                if (Funs485.WriteDebugReg(board, (byte)i, val) == false)
                {
                    MessageBox.Show("写入失败");
                    return;
                }
                val = UInt16.Parse(dgvCalcDc.Rows[i].Cells[4].Value.ToString(), System.Globalization.NumberStyles.HexNumber);
                if (Funs485.WriteDebugReg(board, (byte)(i + 0x10), val) == false)
                {
                    MessageBox.Show("写入失败");
                    return;
                }
            }
            MessageBox.Show("写入完成");
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            if (dgvCalcDc.Rows.Count == 6)
            {
                dgvCalcDc.Rows.Add(2);
            }
            else
            {
                dgvCalcDc.Rows.Remove(dgvCalcDc.Rows[dgvCalcDc.Rows.Count - 1]);
                dgvCalcDc.Rows.Remove(dgvCalcDc.Rows[dgvCalcDc.Rows.Count - 1]);
            }
        }

        private void dgvCalcDc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcSetDc66));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();


        }
        void ProcSetDc66()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            Int16 val = 0;
            try
            {
                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                Board brd = new Board(boardAddr, 9);
                for (int i = 0; i < 6; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;

                    }
                    ShowText(txtUpVal, val.ToString());
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }
        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcDcOffCalc));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();

        }

        void ProcDcOffCalc()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            UInt16 oldOffset;
            UInt16 oldCoff;

            Int16 val = 0;
            try
            {
                Board brd = new Board(boardAddr, 9);
                for (int i = 0; i < 6; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                MessageBox.Show("请确认未加信号");
                if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                {
                    Funs485.Rst(boardAddr);
                    Thread.Sleep(1500);

                    Funs485.SendParam(paramAyy);
                    Thread.Sleep(20);
                    Funs485.StartBoard(boardAddr);
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;
                    }
                    ShowText(txtUpVal, val.ToString());
                }

                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                if (Funs485.ReadDebugReg((byte)boardAddr, (UInt16)(0x10 + ch), out oldCoff) != true)
                {
                    MessageBox.Show("读原系数失败");
                }
                if (Funs485.ReadDebugReg((byte)boardAddr, (UInt16)(ch), out oldOffset) != true)
                {
                    MessageBox.Show("读原偏置失败");
                }
                //新零偏= val/ 系数+旧零偏
                Int16 newOff = (Int16)((float)val / ((float)oldCoff / 4096) + oldOffset);
                if (newOff < 0)
                {
                    MessageBox.Show("计算偏置为负");
                    return;
                }
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0x55aa) == false)
                {
                    MessageBox.Show("解锁1失败");
                    return;
                }
                Thread.Sleep(20);
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0xaa55) == false)
                {
                    MessageBox.Show("解锁2失败");
                    return;
                }
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, ch, (UInt16)newOff);
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, 0x210, 0x55);
                Thread.Sleep(20);
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                    }
                    ShowText(txtChVal, val.ToString());
                    
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcDcCoffCalc));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();

        }
        void ProcDcCoffCalc()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            float realValue = float.Parse(txtRealVal2.Text);
            UInt16 oldOffset;
            UInt16 oldCoff;

            Int16 val = 0;
            try
            {
                Board brd = new Board(boardAddr, 9);
                for (int i = 0; i < 6; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                MessageBox.Show("请确认未加信号");
                if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                {
                    Funs485.Rst(boardAddr);
                    Thread.Sleep(1500);

                    Funs485.SendParam(paramAyy);
                    Thread.Sleep(20);
                    Funs485.StartBoard(boardAddr);
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;
                    }
                    ShowText(txtUpVal, val.ToString());
                }

                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                if (Funs485.ReadDebugReg((byte)boardAddr, (UInt16)(0x10 + ch), out oldCoff) != true)
                {
                    MessageBox.Show("读原系数失败");
                }


                Int16 newOff = (Int16)(((float)realValue / val) * ((float)oldCoff));
                if (newOff < 0)
                {
                    MessageBox.Show("计算系数为负");
                    return;
                }
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0x55aa) == false)
                {
                    MessageBox.Show("解锁1失败");
                    return;
                }
                Thread.Sleep(20);
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0xaa55) == false)
                {
                    MessageBox.Show("解锁2失败");
                    return;
                }
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, (UInt16)(ch + 0x10), (UInt16)newOff);
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, 0x210, 0x55);
                Thread.Sleep(20);
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;
                    }
                    ShowText(txtUpVal, val.ToString());
                    Thread.Sleep(1000);
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }

        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcSetGy));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();
        }
        void ProcSetGy()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            Int16 val = 0;
            try
            {
                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                Board brd = new Board(boardAddr, 9);
                brd.AddCh(new ParamCh((byte)88));
                for (int i = 1; i < 3; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                brd.AddCh(new ParamCh((byte)88));
                for (int i = 1; i < 3; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");

                    }
                    ShowText(txtUpVal, val.ToString());
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcCalcGy));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();
        }

        void ProcCalcGy()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            float realValue = float.Parse(txtRealVal2.Text);
            UInt16 oldOffset;
            UInt16 oldCoff;

            Int16 val = 0;
            try
            {

                Board brd = new Board(boardAddr, 9);
                brd.AddCh(new ParamCh((byte)88));
                for (int i = 1; i < 3; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                brd.AddCh(new ParamCh((byte)88));
                for (int i = 1; i < 3; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                MessageBox.Show("请确认信号加入");
                if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                {
                    Funs485.Rst(boardAddr);
                    Thread.Sleep(1500);

                    Funs485.SendParam(paramAyy);
                    Thread.Sleep(20);
                    Funs485.StartBoard(boardAddr);
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;
                    }
                    ShowText(txtUpVal, val.ToString());

                }

                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                if (Funs485.ReadDebugReg((byte)boardAddr, (UInt16)(0x10 + ch), out oldCoff) != true)
                {
                    MessageBox.Show("读原系数失败");
                }


                Int16 newOff = (Int16)(((float)realValue /val ) * ((float)oldCoff));
                if (newOff < 0)
                {
                    MessageBox.Show("计算系数为负");
                    return;
                }
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0x55aa) == false)
                {
                    MessageBox.Show("解锁1失败");
                    return;
                }
                Thread.Sleep(20);
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0xaa55) == false)
                {
                    MessageBox.Show("解锁2失败");
                    return;
                }
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, (UInt16)(ch + 0x10), (UInt16)newOff);
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, 0x210, 0x55);
                Thread.Sleep(20);
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                    }
                    ShowText(txtUpVal, val.ToString());

                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }

        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcSetDcI66));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();
        }
        void ProcSetDcI66()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            Int16 val = 0;
            try
            {
                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                Board brd = new Board(boardAddr, 11);
                for (int i = 0; i < 6; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;

                    }

                    ShowText(txtUpVal, val.ToString());
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcCalcDcI66));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();
        }

        void ProcCalcDcI66()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            UInt16 oldOffset;
            UInt16 oldCoff;

            Int16 val = 0;
            try
            {
                Board brd = new Board(boardAddr, 11);
                for (int i = 0; i < 6; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                MessageBox.Show("请确认未加信号");
                if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                {
                    Funs485.Rst(boardAddr);
                    Thread.Sleep(1500);

                    Funs485.SendParam(paramAyy);
                    Thread.Sleep(20);
                    Funs485.StartBoard(boardAddr);
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;
                    }
                    ShowText(txtUpVal, val.ToString());
                }

                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                if (Funs485.ReadDebugReg((byte)boardAddr, (UInt16)(0x10 + ch), out oldCoff) != true)
                {
                    MessageBox.Show("读原系数失败");
                }
                if (Funs485.ReadDebugReg((byte)boardAddr, (UInt16)(ch), out oldOffset) != true)
                {
                    MessageBox.Show("读原偏置失败");
                }
                //新零偏= val/ 系数+旧零偏
                Int16 newOff = (Int16)((float)val * ((float)oldCoff / 4096) + oldCoff);
                if (newOff < 0)
                {
                    MessageBox.Show("计算偏置为负");
                    return;
                }
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0x55aa) == false)
                {
                    MessageBox.Show("解锁1失败");
                    return;
                }
                Thread.Sleep(20);
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0xaa55) == false)
                {
                    MessageBox.Show("解锁2失败");
                    return;
                }
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, ch, (UInt16)newOff);
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, 0x210, 0x55);
                Thread.Sleep(20);
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }

        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcSetYp7));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();
        }
        void ProcSetYp7()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            float freq;
            float lowFreq;
            Int16 val = 0;
            try
            {
                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                Board brd = new Board(boardAddr, 8);
                for (int i = 0; i < 7; i++)
                {
                    brd.AddCh(new ParamCh((byte)45));
                }
                byte[] paramAyy = brd.ToByteAry();
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val, out freq, out lowFreq) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;

                    }
                    txtUpVal.Text = val.ToString();
                    ShowText(lblUpVal, val.ToString());
                    ShowText(lblUpFreq, freq.ToString());
                    ShowText(lblLowFreq, freq.ToString());
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcSetYp6));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();
        }

        void ProcSetYp6()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            float freq;
            float lowFreq;
            Int16 val = 0;
            try
            {
                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                Board brd = new Board(boardAddr, 8);
                for (int i = 0; i < 6; i++)
                {
                    brd.AddCh(new ParamCh((byte)45));
                }
                byte[] paramAyy = brd.ToByteAry();
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val, out freq, out lowFreq) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;

                    }
                    txtUpVal.Text = val.ToString();
                    ShowText(lblUpVal, val.ToString());
                    ShowText(lblUpFreq, freq.ToString());
                    ShowText(lblLowFreq, freq.ToString());
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcCalcYp6));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();
        }
        Int16 GetFreqClass(float freq)
        {
            if (freq > 2700 || freq < 450)
            {
                return -1;
            }
            else
            {
                if (freq > 2500)
                {
                    return 7;
                }
                if (freq > 2200)
                {
                    return 6;
                }
                if (freq > 1900)
                {
                    return 5;
                }
                if (freq > 1600)
                {
                    return 4;
                }
                if (freq > 800)
                {
                    return 3;
                }
                if (freq > 700)
                {
                    return 2;
                }
                    if(freq>600)
                    {
                        return 1;
                    }
                        
                else
                {

                    return 0;
                }
            }
        }
        void ProcCalcYp6()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));

            UInt16 oldCoff;
            float freq;
            float lowFreq;

            Int16 val = 0;
            try
            {
                Board brd = new Board(boardAddr, 11);
                for (int i = 0; i < 6; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                MessageBox.Show("请确认未加信号");
                if (Funs485.ReadAnalogCh(boardAddr, ch, out val, out freq, out lowFreq) == false)
                {
                    Funs485.Rst(boardAddr);
                    Thread.Sleep(1500);

                    Funs485.SendParam(paramAyy);
                    Thread.Sleep(20);
                    Funs485.StartBoard(boardAddr);
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val,out freq,out lowFreq) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;
                    }
                    ShowText(txtUpVal, val.ToString());
                }

                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                Int16 freqClass;
                freqClass = GetFreqClass(freq);
                if (freqClass < 0)
                {
                    MessageBox.Show("非正常频率");
                    return;
                }
                if (Funs485.ReadDebugReg((byte)boardAddr, (UInt16)(freq * 0x10 + ch), out oldCoff) != true)
                {
                    MessageBox.Show("读原系数失败");
                    return;
                }

                //新零偏= val/ 系数+旧零偏
                Int16 newOff = (Int16)((float)val * ((float)oldCoff / 4096) + oldCoff);
                if (newOff < 0)
                {
                    MessageBox.Show("计算偏置为负");
                    return;
                }
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0x55aa) == false)
                {
                    MessageBox.Show("解锁1失败");
                    return;
                }
                Thread.Sleep(20);
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0xaa55) == false)
                {
                    MessageBox.Show("解锁2失败");
                    return;
                }
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, ch, (UInt16)newOff);
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, 0x210, 0x55);
                Thread.Sleep(20);
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }

        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (FormMain.mainWorkThread != null && FormMain.mainWorkThread.IsAlive == true)
            {
                FormMain.mainWorkThread.Abort();
            }

            FormMain.mainWorkThread = new Thread(new ThreadStart(ProcCalcYp6));
            FormMain.mainWorkThread.IsBackground = true;
            FormMain.mainWorkThread.Start();
        }
        void ProcCalcYp7()
        {
            byte boardAddr = (byte)(int.Parse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text));
            byte ch = (byte)(int.Parse(txtChNo.Text));
            float realVal;

            UInt16 oldCoff;
            float freq;
            float lowFreq;

            Int16 val = 0;
            realVal = float.Parse(txtDspRealVal.Text);
            try
            {
                Board brd = new Board(boardAddr, 11);
                for (int i = 0; i < 7; i++)
                {
                    brd.AddCh(new ParamCh((byte)66));
                }
                byte[] paramAyy = brd.ToByteAry();
                MessageBox.Show("请确认未加信号");
                if (Funs485.ReadAnalogCh(boardAddr, ch, out val, out freq, out lowFreq) == false)
                {
                    Funs485.Rst(boardAddr);
                    Thread.Sleep(1500);

                    Funs485.SendParam(paramAyy);
                    Thread.Sleep(20);
                    Funs485.StartBoard(boardAddr);
                    Thread.Sleep(1000);
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val, out freq, out lowFreq) == false)
                    {
                        MessageBox.Show("采集失败");
                        return;
                    }
                    ShowText(txtUpVal, val.ToString());
                }

                Funs485.Rst(boardAddr);
                Thread.Sleep(1500);
                Int16 freqClass;
                freqClass = GetFreqClass(freq);
                if (freqClass < 0)
                {
                    MessageBox.Show("非正常频率");
                    return;
                }
                if (Funs485.ReadDebugReg((byte)boardAddr, (UInt16)(freq * 0x10 + ch), out oldCoff) != true)
                {
                    MessageBox.Show("读原系数失败");
                    return;
                }

                //新零偏= val/ 系数+旧零偏
                Int16 newOff = (Int16)(realVal /(float)val * ((float)oldCoff) );
                if (newOff < 0)
                {
                    MessageBox.Show("计算偏置为负");
                    return;
                }
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0x55aa) == false)
                {
                    MessageBox.Show("解锁1失败");
                    return;
                }
                Thread.Sleep(20);
                if (Funs485.WriteDebugReg(boardAddr, (UInt16)0xffff, (UInt16)0xaa55) == false)
                {
                    MessageBox.Show("解锁2失败");
                    return;
                }
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, ch, (UInt16)newOff);
                Thread.Sleep(20);
                Funs485.WriteDebugReg(boardAddr, 0x210, 0x55);
                Thread.Sleep(20);
                Funs485.SendParam(paramAyy);
                Thread.Sleep(20);
                Funs485.StartBoard(boardAddr);
                while (true)
                {
                    if (Funs485.ReadAnalogCh(boardAddr, ch, out val) == false)
                    {
                        MessageBox.Show("采集失败");
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {

            }

        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {
            
        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void btnWrAddr_Click(object sender, EventArgs e)
        {
            int val;
            byte oldAddr;
            byte newAddr;
            if(int.TryParse(((FormMain)(this.Parent.Parent)).txtBoardAddr.Text,out val))
            {
                  oldAddr=(byte)val ;
            }
            else
            {
                MessageBox.Show("板地址不合法");
                return;
            }
           
            
            if(int.TryParse(txtNewAddr.Text,out val))
            {
                newAddr = (byte)val;
            }
            else
            {
                MessageBox.Show("新地址不合法");
                return;
            }
            Thread485.WriteAddr(oldAddr, newAddr);
            
        }

        private void btnWriteNormal_Click(object sender, EventArgs e)
        {
            try
            {
                for (int index = 0; index < this.dgvCalcDc.Rows.Count; ++index)
                {
                    this.dgvCalcDc.Rows[index].Cells[3].Value = (object)0;
                    this.dgvCalcDc.Rows[index].Cells[4].Value = (object)1000;
                }
            }
            catch (Exception ex)
            {
            }
            byte boardAddr = (byte)ushort.Parse(this.textBoxBoardAddr.Text);
            Funs485.UnLock(boardAddr);
            for (int index = 0; index < this.dgvCalcDc.Rows.Count; ++index)
            {
                ushort val1 = ushort.Parse(this.dgvCalcDc.Rows[index].Cells[3].Value.ToString(), NumberStyles.HexNumber);
                if (!Funs485.WriteDebugReg(boardAddr, (ushort)(byte)index, val1))
                {
                    int num = (int)MessageBox.Show("写入失败");
                    return;
                }
                else
                {
                    ushort val2 = ushort.Parse(this.dgvCalcDc.Rows[index].Cells[4].Value.ToString(), NumberStyles.HexNumber);
                    if (!Funs485.WriteDebugReg(boardAddr, (ushort)(byte)(index + 16), val2))
                    {
                        int num = (int)MessageBox.Show("写入失败");
                        return;
                    }
                }
            }
            this.boardAddress1 = (int)byte.Parse(this.textBoxBoardAddr.Text);
            if (!this.WriteReg(this.boardAddress1, (ushort)528, (ushort)43605))
            {
                int num1 = (int)MessageBox.Show("写入失败");
            }
            else
            {
                int num2 = (int)MessageBox.Show("写入成功");
            }

        }

        private void btnCalcAndWrite_Click(object sender, EventArgs e)
        {
            try
            {
                for (int index = 0; index < this.dgvCalcDc.Rows.Count; ++index)
                {
                    this.dgvCalcDc.Rows[index].Cells[3].Value = (object)short.Parse(this.dgvCalcDc.Rows[index].Cells[0].Value.ToString()).ToString("X4");
                    int num = (int)short.Parse(this.dgvCalcDc.Rows[index].Cells[1].Value.ToString()) - (int)short.Parse(this.dgvCalcDc.Rows[index].Cells[0].Value.ToString());
                    this.dgvCalcDc.Rows[index].Cells[4].Value = (object)((int)short.Parse(this.dgvCalcDc.Rows[index].Cells[2].Value.ToString()) * 4096 / (int)(short)((int)short.Parse(this.dgvCalcDc.Rows[index].Cells[1].Value.ToString()) - (int)short.Parse(this.dgvCalcDc.Rows[index].Cells[0].Value.ToString()))).ToString("X4");
                }
            }
            catch
            {
            }
            byte boardAddr = (byte)ushort.Parse(this.textBoxBoardAddr.Text);
            Funs485.Rst(boardAddr);
            Funs485.UnLock(boardAddr);
            for (int index = 0; index < this.dgvCalcDc.Rows.Count; ++index)
            {
                ushort val1 = ushort.Parse(this.dgvCalcDc.Rows[index].Cells[3].Value.ToString(), NumberStyles.HexNumber);
                if (!Funs485.WriteDebugReg(boardAddr, (ushort)(byte)index, val1))
                {
                    int num = (int)MessageBox.Show("写入失败");
                    return;
                }
                else
                {
                    ushort val2 = ushort.Parse(this.dgvCalcDc.Rows[index].Cells[4].Value.ToString(), NumberStyles.HexNumber);
                    if (!Funs485.WriteDebugReg(boardAddr, (ushort)(byte)(index + 16), val2))
                    {
                        int num = (int)MessageBox.Show("写入失败");
                        return;
                    }
                }
            }
            this.boardAddress1 = (int)byte.Parse(this.textBoxBoardAddr.Text);
            if (!this.WriteReg(this.boardAddress1, (ushort)528, (ushort)43605))
            {
                int num1 = (int)MessageBox.Show("写入失败");
            }
            else
            {
                int num2 = (int)MessageBox.Show("写入成功");
            }

        }
    }
}
