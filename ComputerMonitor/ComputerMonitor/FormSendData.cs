using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComputerMonitor.Helper;
using System.Threading;

namespace ComputerMonitor
{
    public partial class FormSendData : Form
    {
        public FormSendData()
        {
            InitializeComponent();
        }

        private void FormSendData_Load(object sender, EventArgs e)
        {
            GlobalVar.SendData += new EventHandler(GlobalVar_SendData);
        }

        bool showingData = false;
        private delegate void ShowResultCallback(string r);
        private void ShowResult(string r)
        {
            showingData = true;
            if (this.InvokeRequired)
            {
                
                this.Invoke(new ShowResultCallback(ShowResult), new object[] { r });
            }
            else
            {
                    
                    textBox1.AppendText(r);
                    
            }
            showingData = false;
        }
        void GlobalVar_SendData(object sender, EventArgs e)
        {
            ShowResult((string)sender);
        }

        private void FormSendData_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalVar.SendData -= new EventHandler(GlobalVar_SendData);
            while (showingData)
            {
                Application.DoEvents();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}