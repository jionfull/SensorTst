using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComputerMonitor.Protocol;
using System.Threading;
using WeifenLuo.WinFormsUI.Docking;

namespace ComputerMonitor
{
    public partial class FormReceiveData : DockContent
    {
        byte[] dataBuffer=new byte[64*1024]; //64K的缓存
        StringBuilder sb = new StringBuilder();
        Thread threadRcv = null;
        public FormReceiveData()
        {
            InitializeComponent();
        }

        private delegate void ShowResultCallback(string r);
        private void ShowResult(string r)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new ShowResultCallback(ShowResult), new object[] { r });
            }
            else
            {
                textBox1.AppendText(r);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void FormReceiveData_Load(object sender, EventArgs e)
        {
            threadRcv = new Thread(new ThreadStart(ShowProc));
            threadRcv.IsBackground = true;
            threadRcv.Start();
        }
        
        private void FormReceiveData_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadRcv != null && threadRcv.IsAlive)
            {
                threadRcv.Abort();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Protocol.FrameReceive.checkCRC = !Protocol.FrameReceive.checkCRC;
            toolStripButton2.Checked = Protocol.FrameReceive.checkCRC;
            toolStripButton2.Text = Protocol.FrameReceive.checkCRC ? "校验数据" : "不校验数据";
        }

        private void ShowProc()
        {
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, -1); //接收任何数据帧

            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    
                    sb.Length = 0;
                    for (int i = 0; i < frameUnit.FrameLength; i++)
                    {
                        sb.Append(dataBuffer[i].ToString("X2") + " ");
                    }
                    sb.Append("\r\n");
                    
                    ShowResult(sb.ToString());
                    

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

    }
}