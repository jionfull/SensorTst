using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using ComputerMonitor.Protocol;
using WeifenLuo.WinFormsUI.Docking;

namespace ComputerMonitor
{
    public partial class FormAutoTest : DockContent
    {
        public FormAutoTest()
        {
            InitializeComponent();
        }

        int boardAddress = -1;
        byte channelNo = 0;
        Thread threadScan = null;
        string TextMessage = null;
        //bool bStart = false;
        byte[] dataBuffer = new byte[64 * 1024];
        Thread threadReadMeter = null;
        AutoResetEvent waitKey = new AutoResetEvent(false);
        Lon.Dev.Meter meter = new Lon.Dev.Meter("COM2");
        float meterScale = 1;

        delegate void DelegateAppendText(TextBox tb, string str);

        private void btnStartTest_Click(object sender, EventArgs e)
        {
            boardAddress = int.Parse(textBoxBoardAddress.Text);
            channelNo = byte.Parse(textBoxChannelNo.Text);

            if (threadScan == null || !threadScan.IsAlive)
            {
                byte[] buffer = new byte[1];
                Protocol.FrameTransmit.Send(1, boardAddress, 5, buffer, 0);
                //bStart = true;

                threadScan = new Thread(new ThreadStart(ScanProc));
                threadScan.IsBackground = true;
                threadScan.Start();
                btnStartTest.Text = "停止测试";
            }
            else
            {
                byte[] buffer = new byte[1];
                Protocol.FrameTransmit.Send(1, boardAddress, 6, buffer, 0);
                //bStart = false;
                threadScan.Abort();
                btnStartTest.Text = "开始测试";
            }
        }

        private void ScanProc()
        {
            StringBuilder sb = new StringBuilder();
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x30);
            try
            {
                byte[] buffer = new byte[1];
                buffer[0] = channelNo;

                while (true)
                {
                    Protocol.FrameTransmit.Send(1, boardAddress, 0x30, buffer, 1);

                    if (frameUnit.WaitData(3000) == false)
                    {
                        MessageBox.Show("收不到数据");

                        // byte[] buffer1 = new byte[1];
                        // Protocol.FrameTransmit.Send(1, boardAddress, 6, buffer1, 0);

                        ShowText(btnStartTest, "开始测试");
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

                        ShowText(textBox_testMessage, sb.ToString());

                    }
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

        private void btnSaveFiles_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = "D:\\长龙";
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel表格文件|*.xls";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                StreamWriter sw = new StreamWriter(fileName);
                sw.WriteLine(textBox_testMessage.Text);
                sw.Close();
            }
        }


    }
}
    
