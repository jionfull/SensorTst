using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ComputerMonitor
{
    public partial class FormCANReceiveData : Form
    {
        Thread threadRecv = null;
        public FormCANReceiveData()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        delegate void ShowResultCallback(string txt);

        void ShowResult(string txt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowResultCallback(ShowResult), new object[] { txt });
            }
            else
            {
                textBox1.AppendText(txt);
            }
        }

        private void ProcRecv()
        {
            byte[] frameData=new byte[12];
            Protocol.CANFrameUnit frameUnit = Protocol.CANFrameManager.CreateFrame(frameData, -1);
            StringBuilder sb = new StringBuilder();
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    sb.Length = 0;
                    int c = frameUnit.FrameLength;
                    for (int i = 0; i < c; i++)
                    {
                        sb.Append(frameData[i].ToString("X2") + " ");

                    }
                    sb.Append("\r\n");
                    ShowResult(sb.ToString());
                }
            }
            catch (ThreadAbortException)
            {
 
            }
        }

        private void FormCANReceiveData_Load(object sender, EventArgs e)
        {
            threadRecv = new Thread(new ThreadStart(ProcRecv));
            threadRecv.IsBackground = true;
            threadRecv.Start();
        }
    }
}