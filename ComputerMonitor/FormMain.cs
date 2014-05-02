using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using WeifenLuo.WinFormsUI.Docking;
using System.Configuration;
using System.Xml;
using System.IO;
using Lon.Dev;
using System.Threading;
using ComputerMonitor.Protocol;

namespace ComputerMonitor
{
   
    public partial class FormMain : Form
    {
        FormFuncBoardParam formFuncBoardParam = null;
        FormBoardFuncSet formBoardFuncSet = null;
        FormReceiveData formDataReceive = null;
        FormSendData formDataSend = null;
        FormAutoCalc formSimu = null;
        FormFuncBoardData formSimuData = null;
        FormCANCommand formCANCommand = null;
        FormAutoTest formStartTest = null;
        FrmAnalogDisplay frmAnalogDisplay = null;
        FromMeter frmMeter = null;
        public  static  Meter meter = new Meter();
        public static Thread mainWorkThread = null;
        TextBox textBoxBoardAddr;
        int boardAddress = -1;
        static public FormMain Instance;
       
        public FormMain()
        {
            InitializeComponent();
            Instance = this;
        }

        delegate void DelegateShowText(Control ctl, String text);
        void ShowText(Control ctl,String text)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new DelegateShowText(this.ShowText), new object[] { ctl, text });
            }
            else
            {
                ctl.Text = text;
            }
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (formFuncBoardParam == null || formFuncBoardParam.IsDisposed)
            {
                formFuncBoardParam = new FormFuncBoardParam();
                
                formFuncBoardParam.Show(this.dpMain);
            }
            else
            {
                formFuncBoardParam.Activate();
            }
            

        }
        void ReadAddr_Finish(ReadAddrFinishEventArgs res)
        {
            if(res.IsSucess)
            {
                ShowText(txtBoardAddr, res.Addr.ToString());
            }

        }
        void WrAddr_Finish(WrAddrFinishEventArgs res)
        {
            
        }
        private void FormMain_Load(object sender, EventArgs e)
        {

            cmbWorkPort.Items.AddRange(SerialPort.GetPortNames());
            cmbMeterPort.Items.AddRange(SerialPort.GetPortNames());
            if (cmbWorkPort.Items.Count > 0)
            {
                cmbWorkPort.SelectedIndex = 0;
            }
            toolStripComboBox2.Items.AddRange(new string[] { "9600", "38400", "57600","115200","500000" });
            toolStripComboBox2.SelectedIndex = 2;
          
            formDataReceive = new FormReceiveData();
            formDataSend = new FormSendData();
            frmAnalogDisplay = new FrmAnalogDisplay();
            formBoardFuncSet = new FormBoardFuncSet();
            formFuncBoardParam = new FormFuncBoardParam();
            frmMeter = new FromMeter();
            formDataReceive.Show(this.dpMain);
            formDataSend.Show(formDataReceive.Pane,DockAlignment.Right,0.5);
            frmAnalogDisplay.Show(dpMain);
            
            formFuncBoardParam.Show(dpMain);
            formBoardFuncSet.Show(dpMain);
         // frmMeter.Show(dpMain);
            Protocol.FrameReceive.Start();
           
          
            
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(".\\MySetting.XML");
                XmlNode node= document.SelectSingleNode("/Config/Board/BoardAddr");

                txtBoardAddr.Text = node.InnerXml;
                node = document.SelectSingleNode("/Config/Board/BoardStyle");
                cmbCardStyle.SelectedIndex = int.Parse(node.InnerXml);
                node = document.SelectSingleNode("/Config/Board/BoardStyle");
                cmbCardStyle.SelectedIndex = int.Parse(node.InnerXml);
                node = document.SelectSingleNode("/Config/Ports/Work/Port");
                cmbWorkPort.SelectedIndex = int.Parse(node.InnerXml);
                node = document.SelectSingleNode("/Config/Ports/Meter/Port");
                cmbMeterPort.SelectedIndex = int.Parse(node.InnerXml);

            }
            catch (System.Exception ex)
            {
                txtBoardAddr.Text = "3";
                cmbCardStyle.SelectedIndex = 0;
            }

            Thread485.RdAddrFinish += ReadAddr_Finish;
            Thread485.WrAddrFinish += new Thread485.WriteAddrFinish(Thread485_WrAddrFinish);



        }

        void Thread485_WrAddrFinish(WrAddrFinishEventArgs args)
        {
            if (args.IsSucess)
            {
                ShowText(txtBoardAddr, args.Addr.ToString());
                MessageBox.Show("写入地址" + args.Addr.ToString() + "成功");
            }
            else
            {
                MessageBox.Show("写入地址" + args.Addr.ToString() + "失败");
            }
        }
        #region 菜单事件
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (SerialManager.CommPort.IsOpen)
            {
                SerialManager.CommPort.ClosePort();
            }
            else
            {
                SerialManager.CommPort.OpenPort(cmbWorkPort.Text, int.Parse(toolStripComboBox2.Text));

            }

            if (SerialManager.CommPort.IsOpen)
            {
                toolStrip1.SuspendLayout();
                toolStripButton1.Image = Properties.Resources.Close;
                toolStripButton1.Text = "关闭串口";
                toolStripButton1.ToolTipText = "关闭串口";
                cmbWorkPort.Enabled = false;
                toolStripComboBox2.Enabled = false;
                toolStrip1.ResumeLayout(true);
            }
            else
            {
                toolStrip1.SuspendLayout();
                toolStripButton1.Image = Properties.Resources.Port;
                toolStripButton1.Text = "打开串口";
                toolStripButton1.ToolTipText = "打开串口";
                cmbWorkPort.Enabled = true;
                toolStripComboBox2.Enabled = true;
                toolStrip1.ResumeLayout(true);
            }
            
        }

        private void toolStripMenuItemViewSend_Click(object sender, EventArgs e)
        {
            if (formDataSend == null || formDataSend.IsDisposed)
            {
                formDataSend = new FormSendData();
               
                formDataSend.Show(this.dpMain);
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
             
                formDataReceive.Show(this.dpMain);
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
                
                formBoardFuncSet.Show(dpMain);

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
                formSimu = new FormAutoCalc();
               
                formSimu.Show(dpMain,new Rectangle(50,50,918,777));
            }
            else 
            {
                formSimu.Activate();
            }
        }
        FormSignalCurrentCalc formSignal = null;
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (formSignal == null || formSignal.IsDisposed)
            {
                formSignal = new FormSignalCurrentCalc();
                formSignal.MdiParent = this;
                formSignal.Show(dpMain, new Rectangle(50, 50, 918, 777));
            }
            else
            {
                formSignal.Activate();
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

        private void ToolStripMenuItemAutoTest_Click(object sender, EventArgs e)
        {
            if (formStartTest == null || formStartTest.IsDisposed)
            {
                formStartTest = new FormAutoTest();
                formStartTest.MdiParent = this;
                formStartTest.Show();
            }
            else
            {
                formStartTest.Activate();
            }
        }
        #endregion
        private void txtBoardAddr_TextChanged(object sender, EventArgs e)
        {
            
           
            XmlDocument document = new XmlDocument();
            document.LoadXml("<Board></Board>");
            XmlNode root = document.SelectSingleNode("Board");
           
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(".\\MySetting.Xml");
                XmlNode node = doc.SelectSingleNode("/Config/Board/BoardAddr");
                node.InnerXml = txtBoardAddr.Text;
                node = doc.SelectSingleNode("/Config/Board/BoardStyle");
                node.InnerXml = cmbCardStyle.SelectedIndex.ToString();
                node = doc.SelectSingleNode("/Config/Ports/Work/Port");
                node.InnerXml = cmbWorkPort.SelectedIndex.ToString();
                node = doc.SelectSingleNode("/Config/Ports/Meter/Port");
                node.InnerXml = cmbMeterPort.SelectedIndex.ToString();

                doc.Save(".\\MySetting.Xml");
            }
            catch (System.Exception ex)
            {
            	
            }
            
       
        }

        private void cmbCardStyle_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
         
        private void btnOpenMeter_Click(object sender, EventArgs e)
        {
             if (meter.IsOpen()==false)
                {
                    toolStrip1.SuspendLayout();
                    btnOpenMeter.Image = Properties.Resources.Close;
                    btnOpenMeter.Text = "关闭串口";
                    btnOpenMeter.ToolTipText = "关闭串口";
                    cmbMeterPort.Enabled = false;
                    meter.PortName=cmbMeterPort.Text;
                    meter.Run();
                    toolStrip1.ResumeLayout(true);
                }
                else
                {
                    toolStrip1.SuspendLayout();
                    btnOpenMeter.Image = Properties.Resources.Port;
                    btnOpenMeter.Text = "打开串口";
                    btnOpenMeter.ToolTipText = "打开串口";
                    cmbMeterPort.Enabled = true;
                    try
                    { 
                        meter.Stop();
                    }
                    catch (System.Exception ex)
                    {
                    	
                    }
                   
                    toolStrip1.ResumeLayout(true);
                }
        }

        private void dpMain_ActiveContentChanged(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //byte addr = (byte)int.Parse(txtBoardAddr.Text);
            //Funs485.Rst(addr);

            byte[] buffer = new byte[1];
            boardAddress = byte.Parse(txtBoardAddr.Text);
            Protocol.FrameTransmit.Send(SrcAddr, boardAddress, 0x0f, buffer, 0);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //byte addr = (byte)int.Parse(txtBoardAddr.Text);
            //Funs485.StartBoard(addr);


            byte[] buffer = new byte[1];
            boardAddress = byte.Parse(txtBoardAddr.Text);
            Protocol.FrameTransmit.Send(SrcAddr, boardAddress, 5, buffer, 0);

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            boardAddress = byte.Parse(txtBoardAddr.Text);
            Protocol.FrameTransmit.Send(1, boardAddress, 6, buffer, 0);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1];
            boardAddress = byte.Parse(txtBoardAddr.Text);
            Protocol.FrameTransmit.Send(SrcAddr, boardAddress, 0x10, buffer, 0);
        }

        private void btnRdAddr_Click(object sender, EventArgs e)
        {
            Thread485.ReadAddr();
        }

        public byte SrcAddr = 1;
        private void cmbSrcAddr_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }

        private void cmbSrcAddr_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SrcAddr = (byte)int.Parse(cmbSrcAddr.Text);
            }
            catch (System.Exception ex)
            {

            }
        }

       
    }
    class ConfigSectionData : ConfigurationSection
    {
        [ConfigurationProperty("BoardAddr")]
        public int BoardAddr
        {
            get { return (int)this["BoardAddr"]; }
            set { this["BoardAddr"] = value; }
        }

        [ConfigurationProperty("BoardStyle")]
        public int BoardStyle
        {
            get { return (int)this["BoardStyle"]; }
            set { this["BoardStyle"] = value; }
        }
    }
}