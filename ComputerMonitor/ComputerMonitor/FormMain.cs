using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace ComputerMonitor
{
    public partial class FormMain : Form
    {
        FormFuncBoardParam formFuncBoardParam = null;
        FormBoardFuncSet formBoardFuncSet = null;
        FormReceiveData formDataReceive = null;
        FormSendData formDataSend = null;
        FormFuncBoardSimulator formSimu = null;
        FormFuncBoardData formSimuData = null;
        FormCANCommand formCANCommand = null;
        public FormMain()
        {
            InitializeComponent();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (formFuncBoardParam == null || formFuncBoardParam.IsDisposed)
            {
                formFuncBoardParam = new FormFuncBoardParam();
                formFuncBoardParam.MdiParent = this;
                formFuncBoardParam.Show();
            }
            else
            {
                formFuncBoardParam.Activate();
            }
            

        }

        private void FormMain_Load(object sender, EventArgs e)
        {

            toolStripComboBox1.Items.AddRange(SerialPort.GetPortNames());
            if (toolStripComboBox1.Items.Count > 0)
            {
                toolStripComboBox1.SelectedIndex = 0;
            }
            toolStripComboBox2.Items.AddRange(new string[] { "9600", "38400", "57600","115200","500000" });
            toolStripComboBox2.SelectedIndex = 2;
            toolStripMenuItemViewSend_Click(null,null);
             toolStripMenuItemViewReceive_Click(null,null);
            Protocol.FrameReceive.Start();
           
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (SerialManager.CommPort.IsOpen)
            {
                SerialManager.CommPort.ClosePort();
            }
            else
            {
                SerialManager.CommPort.OpenPort(toolStripComboBox1.Text, int.Parse(toolStripComboBox2.Text));

            }

            if (SerialManager.CommPort.IsOpen)
            {
                toolStrip1.SuspendLayout();
                toolStripButton1.Image = Properties.Resources.Close;
                toolStripButton1.Text = "关闭串口";
                toolStripButton1.ToolTipText = "关闭串口";
                toolStripComboBox1.Enabled = false;
                toolStripComboBox2.Enabled = false;
                toolStrip1.ResumeLayout(true);
            }
            else
            {
                toolStrip1.SuspendLayout();
                toolStripButton1.Image = Properties.Resources.Port;
                toolStripButton1.Text = "打开串口";
                toolStripButton1.ToolTipText = "打开串口";
                toolStripComboBox1.Enabled = true;
                toolStripComboBox2.Enabled = true;
                toolStrip1.ResumeLayout(true);
            }
            
        }

        private void toolStripMenuItemViewSend_Click(object sender, EventArgs e)
        {
            if (formDataSend == null || formDataSend.IsDisposed)
            {
                formDataSend = new FormSendData();
                formDataSend.MdiParent = this;
                formDataSend.StartPosition = FormStartPosition.Manual;
                formDataSend.Location = new Point(this.ClientSize.Width / 2-100, 0);
                formDataSend.Size = new Size(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
                formDataSend.Show();
            }
            else
            {
                formDataSend.Activate();
            }
        }

        private void toolStripMenuItemViewReceive_Click(object sender, EventArgs e)
        {
            if (formDataReceive == null || formDataReceive.IsDisposed)
            {
                formDataReceive = new FormReceiveData();
                formDataReceive.MdiParent = this;
                formDataReceive.StartPosition = FormStartPosition.Manual;
                formDataReceive.Location = new Point(this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2-100);
                formDataReceive.Size = new Size(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
                formDataReceive.Show();
            }
            else
            {
                formDataReceive.Activate();
            }
        }

        private void toolStripMenuItemFuncSet_Click(object sender, EventArgs e)
        {
            if (formBoardFuncSet == null || formBoardFuncSet.IsDisposed)
            {
                formBoardFuncSet = new FormBoardFuncSet();
                formBoardFuncSet.MdiParent = this;
                formBoardFuncSet.Show();

            }
            else
            {
                formBoardFuncSet.Activate();
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (formSimu == null || formSimu.IsDisposed)
            {
                formSimu = new FormFuncBoardSimulator();
                formSimu.MdiParent = this;
                formSimu.Show();
            }
            else 
            {
                formSimu.Activate();
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (formSimuData == null || formSimuData.IsDisposed)
            {
                formSimuData = new FormFuncBoardData();
                formSimuData.MdiParent = this;
                formSimuData.Show();
            }
            else
            {
                formSimuData.Activate();
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (formCANCommand == null || formCANCommand.IsDisposed)
            {
                formCANCommand = new FormCANCommand();
                formCANCommand.MdiParent = this;
                formCANCommand.Show();
            }
            else
            {
                formCANCommand.Activate();
            }
        }

        FormCANReceiveData formCANDataRec = null;
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (formCANDataRec == null || formCANDataRec.IsDisposed)
            {
                formCANDataRec = new FormCANReceiveData();
                formCANDataRec.MdiParent = this;
                formCANDataRec.Show();
            }
            else
            {
                formCANDataRec.Activate();
            }
        }
    }
}