using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ComputerMonitor.Protocol;

namespace ComputerMonitor
{
    public partial class FormFuncBoardSimulator : Form
    {
        Thread threadParam = null;
        Thread threadFirmware = null;
        Thread threadUpdateStatus = null;
        byte[] frameBuffer1=new byte[32*1024];
        byte[] frameBuffer2 = new byte[32*1024];
        byte[] frameBuffer3=new byte[16];
        int boardAddr = 2;
        int boardType = 1;
        int paramResponse = 0;
        int firmwareResponse = 0;
        int updateStatus = 0;

        public FormFuncBoardSimulator()
        {
            InitializeComponent();
        }

        private void FormFuncBoardSimulator_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        /// <summary>
        /// 应答参数设置
        /// </summary>
        private void ProcResponseParam()
        {
            FrameUnit frameUnit = FrameManager.CreateFrame(frameBuffer1, 0x01);
            byte[] data = new byte[4];
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    data[0]=frameBuffer1[7];
                    data[1] = frameBuffer1[8];
                    data[2] = (byte)(boardType);
                    data[3] = (byte)(paramResponse);
                    Thread.Sleep(100);
                    FrameTransmit.Send(boardAddr, 1, 1, data, 4);
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

        /// <summary>
        /// 应答固件升级
        /// </summary>
        private void ProcResponseFirmware()
        {
            FrameUnit frameUnit = FrameManager.CreateFrame(frameBuffer2, 0x02);
            byte[] data = new byte[4];
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    data[0] = frameBuffer2[7];
                    data[1] = frameBuffer2[8];
                    data[2] = frameBuffer2[9];// (byte)(boardType);
                    data[3] = (byte)(firmwareResponse);

                    FrameTransmit.Send(boardAddr, 1, 0x02, data, 4);
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            boardAddr = int.Parse(comboBox1.Text);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            boardType = comboBox2.SelectedIndex + 1;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                paramResponse = int.Parse(button.Tag.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (threadParam == null || threadParam.IsAlive == false)
            {
                threadParam = new Thread(new ThreadStart(ProcResponseParam));
                threadParam.IsBackground = true;
                threadParam.Start();
                button1.Text = "停止";
            }
            else
            {
                threadParam.Abort();
                button1.Text = "开始";
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                firmwareResponse = int.Parse(button.Tag.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (threadFirmware == null || threadFirmware.IsAlive == false)
            {
                threadFirmware = new Thread(new ThreadStart(ProcResponseFirmware));
                threadFirmware.IsBackground = true;
                threadFirmware.Start();
                button2.Text = "停止";
            }
            else
            {
                threadFirmware.Abort();
                button2.Text = "开始";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (threadUpdateStatus == null || threadUpdateStatus.IsAlive == false)
            {
                threadUpdateStatus = new Thread(new ThreadStart(ProcUpdateStatus));
                threadUpdateStatus.IsBackground = true;
                threadUpdateStatus.Start();
                button3.Text = "停止";
            }
            else
            {
                threadUpdateStatus.Abort();
                button3.Text = "开始";
            }
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                updateStatus = int.Parse(button.Tag.ToString());
            }
        }
        private void ProcUpdateStatus()
        {
            FrameUnit frameUnit = FrameManager.CreateFrame(frameBuffer3, 0x03);
            byte[] data = new byte[1];
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    data[0] = (byte)updateStatus;

                    FrameTransmit.Send(boardAddr, 1, 0x03, data, 1);
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

        Thread threadStartBoard = null;
        int statusStart = 0;
        private void button4_Click(object sender, EventArgs e)
        {
            if (threadStartBoard == null || threadStartBoard.IsAlive == false)
            {
                threadStartBoard = new Thread(new ThreadStart(ProcStartBoard));
                threadStartBoard.IsBackground = true;
                threadStartBoard.Start();
                button4.Text = "停止";
            }
            else
            {
                threadStartBoard.Abort();
                button4.Text = "开始";
            }
        }

        private void ProcStartBoard()
        {
            byte[] frameData=new byte[16];
            FrameUnit frameUnit = FrameManager.CreateFrame(frameData, 0x05);
            byte[] data = new byte[1];
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    data[0] = (byte)statusStart;

                    FrameTransmit.Send(boardAddr, 1, 0x05, data, 1);
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

        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                statusStart = int.Parse(button.Tag.ToString());
            }
        }

        Thread threadStopBoard = null;
        int statusStop = 0;
        private void button5_Click(object sender, EventArgs e)
        {
            if (threadStopBoard == null || threadStopBoard.IsAlive == false)
            {
                threadStopBoard = new Thread(new ThreadStart(ProcStopBoard));
                threadStopBoard.IsBackground = true;
                threadStopBoard.Start();
                button5.Text = "停止";
            }
            else
            {
                threadStopBoard.Abort();
                button5.Text = "开始";
            }
        }

        private void ProcStopBoard()
        {
            byte[] frameData = new byte[16];
            FrameUnit frameUnit = FrameManager.CreateFrame(frameData, 0x06);
            byte[] data = new byte[1];
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    data[0] = (byte)statusStop;

                    FrameTransmit.Send(boardAddr, 1, 0x06, data, 1);
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

        private void radioButton18_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                statusStop = int.Parse(button.Tag.ToString());
            }
        }

        Thread threadVersion = null;
        int version1 = 1;
        int version2 = 0;
        private void button6_Click(object sender, EventArgs e)
        {
            if (threadVersion == null || threadVersion.IsAlive==false)
            {
                version1 = int.Parse(textBox1.Text);
                version2 = int.Parse(textBox2.Text);
                threadVersion = new Thread(new ThreadStart(ProcVersion));
                threadVersion.IsBackground = true;
                threadVersion.Start();
                button6.Text = "停止";
            }
            else
            {
                threadVersion.Abort();
                button6.Text = "启动";
            }
        }

        private void ProcVersion()
        {
            byte[] frameData = new byte[16];
            FrameUnit frameUnit = FrameManager.CreateFrame(frameData, 0x07);
            byte[] data = new byte[2];
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    data[0] = (byte)version1;
                    data[1] = (byte)version2;
                    FrameTransmit.Send(boardAddr, 1, 0x07, data, 2);
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
        Thread threadBoardStatus = null;
        int boardStatus = 0;
        private void button7_Click(object sender, EventArgs e)
        {
            if (threadBoardStatus == null || threadBoardStatus.IsAlive == false)
            {
                threadBoardStatus = new Thread(new ThreadStart(ProcBoardStatus));
                threadBoardStatus.IsBackground = true;
                threadBoardStatus.Start();
                button7.Text = "停止";
            }
            else
            {
                threadBoardStatus.Abort();
                button7.Text = "开始";
            }
        }

        private void ProcBoardStatus()
        {
            byte[] frameData = new byte[16];
            FrameUnit frameUnit = FrameManager.CreateFrame(frameData, 0x10);
            byte[] data = new byte[1];
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);
                    data[0] = (byte)boardStatus;
                    FrameTransmit.Send(boardAddr, 1, 0x10, data, 1);
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

        private void radioButton19_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                boardStatus = int.Parse(button.Tag.ToString());
            }
        }

    }
}