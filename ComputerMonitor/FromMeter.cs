using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Lon.Dev;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;

namespace ComputerMonitor
{
    public partial class FromMeter : DockContent
    {
        Meter meter = null;
        float val=0;
        Thread threadReadMeter = null;
        delegate void DelegateShowText(Control ctl, string str);
        public FromMeter()
        {
            InitializeComponent();
        }
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
      
        private void FromMeter_Load(object sender, EventArgs e)
        {
            meter = FormMain.meter;
            if (threadReadMeter != null)
            {
                threadReadMeter.Abort();
            }
            threadReadMeter = new Thread(new ThreadStart(ProcReadMeterVal));
            threadReadMeter.IsBackground = true;
            threadReadMeter.Start();
            
        }

    

        private void button1_Click(object sender, EventArgs e)
        {
            meter.SetAc();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            meter.SetDc();
        }
        private void ProcReadMeterVal()
        {
            float val;
            while (true)
            {
                meter.ReadValue(out val, 70000);
                ShowText(lblMeter, val.ToString());
            }
        }
    }
}
