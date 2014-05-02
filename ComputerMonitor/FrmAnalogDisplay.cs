using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;
using ComputerMonitor.Protocol;

namespace ComputerMonitor
{
    public partial class FrmAnalogDisplay :DockContent
    {
        Thread proc;
        public FrmAnalogDisplay()
        {
            InitializeComponent();
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


        private void FrmAnalogDisplay_Load(object sender, EventArgs e)
        {
           if(proc!=null&&proc.IsAlive==true)
           {
              
           }
           else
           {
               proc = new Thread(new ThreadStart(ProcMR));
               proc.IsBackground = true;
               proc.Start();

           }
        }
        private void ProcMR()
        {
            byte[] dataBuffer = new byte[2048];
            StringBuilder sb = new StringBuilder();
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 0x30);
            try
            {
                while (true)
                {
                    
                    if (frameUnit.WaitData(3000) == false)
                    {
                       
                        ShowText(txtAnalog, "");
                      
                    }
                    else
                    {
                        sb.Length = 0;
                        for (int i = 8; i < frameUnit.FrameLength - 4; i += 2)
                        {
                            Int16 chVal;
                            chVal = (Int16)(dataBuffer[i] + dataBuffer[i + 1] * 256);
                            sb.Append("Í¨µÀ" + ((i - 8) / 2).ToString() + ":   " + chVal.ToString() + " ");
                            sb.Append("\r\n");
                        }



                        ShowText(txtAnalog, sb.ToString());

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

       
    }
}