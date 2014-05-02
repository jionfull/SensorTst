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
    public partial class FormFuncBoardData : Form
    {
        int boardType = 1;
        int boardAddr = 2;
        byte ChannelNumKRB = 1;
        byte[] ChannelDataKRB=new byte[256];
        int DataCountKRB = 0;
        byte[] ChannelData1 = new byte[256];
        int DataCount1 = 0;
        int warningTime = 5000;

        byte ChannelNumMRB = 1;
        byte[] ChannelDataMRB = new byte[256];
        int DataCountMRB = 0;
        public FormFuncBoardData()
        {
            InitializeComponent();
        }

        Thread threadKRBSwitch = null; //开入板开关量

        private void button1_Click(object sender, EventArgs e)
        {
            if (threadKRBSwitch == null || threadKRBSwitch.IsAlive == false)
            {
                ChannelNumKRB = byte.Parse(textBox1.Text, System.Globalization.NumberStyles.AllowHexSpecifier);

                string[] txtValues = textBox2.Text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                DataCountKRB = txtValues.Length;
                ChannelDataKRB[0] = ChannelNumKRB;
                for (int i = 0; i < DataCountKRB; i++)
                {
                    ChannelDataKRB[i + 1] = byte.Parse(txtValues[i], System.Globalization.NumberStyles.AllowHexSpecifier);
                }

                threadKRBSwitch = new Thread(new ThreadStart(ProcKRBSwitch));
                threadKRBSwitch.IsBackground = true;
                threadKRBSwitch.Start();
                button1.Text = "停止";
            }
            else
            {
                threadKRBSwitch.Abort();
                button1.Text = "开始";
            }

        }
        int cmdType = 0x20;
        private void ProcKRBSwitch()
        {
            byte[] frameData = new byte[16];
            FrameUnit frameUnit = FrameManager.CreateFrame(frameData, cmdType);
            
            try
            {
                while (true)
                {
                    frameUnit.WaitData(-1);

                    FrameTransmit.Send(boardAddr, 1, cmdType, ChannelDataKRB, DataCountKRB + 1);
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

        private void FormFuncBoardData_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            boardAddr = int.Parse(comboBox1.Text);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            boardType = comboBox2.SelectedIndex + 1;
        }

        Thread threadKRBWarning = null; //开入板开关量
        private void button2_Click(object sender, EventArgs e)
        {
            if (threadKRBWarning == null || threadKRBWarning.IsAlive == false)
            {
                

                string[] txtValues = textBox3.Text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                DataCount1 = txtValues.Length;
                for (int i = 0; i < DataCount1; i++)
                {
                    ChannelData1[i ] = byte.Parse(txtValues[i], System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                warningTime = int.Parse(textBox4.Text);
                threadKRBWarning = new Thread(new ThreadStart(ProcKRBWarning));
                threadKRBWarning.IsBackground = true;
                threadKRBWarning.Start();
                button2.Text = "停止";
            }
            else
            {
                threadKRBWarning.Abort();
                button2.Text = "开始";
            }
        }

        /// <summary>
        /// 开入板报警数据
        /// </summary>
        private void ProcKRBWarning()
        {
            byte[] frameData = new byte[16];
            FrameUnit frameUnit = FrameManager.CreateFrame(frameData, 0x40);

            try
            {
                while (true)
                {
                    if (warningTime < 0)
                    {
                        frameUnit.WaitData(-1);
                    }
                    else
                    {
                        Thread.Sleep(warningTime);
                    }

                    FrameTransmit.Send(boardAddr, 1, 0x40, ChannelData1, DataCount1);
                }
            }
            catch (ThreadAbortException)
            {

            }
            finally
            {
                
            }
        }

        

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                cmdType = int.Parse(button.Tag.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
        }
        int type = 0;
        private void button3_Click(object sender, EventArgs e)
        {
            int chanNum = int.Parse(textBox6.Text);
            byte[] data=new byte[7];

            data[0] = (byte)((type << 4) | (boardAddr & 0x0f));
            data[1] = (byte)(chanNum & 0xff);

            data[2] = (byte)(((chanNum >> 8) & 0x03) << 2);
            string[] txtVals = textBox5.Text.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            data[3] = byte.Parse(txtVals[0], System.Globalization.NumberStyles.AllowHexSpecifier);
            data[4] = byte.Parse(txtVals[1], System.Globalization.NumberStyles.AllowHexSpecifier);
            data[5] = byte.Parse(txtVals[2], System.Globalization.NumberStyles.AllowHexSpecifier);
            data[6] = byte.Parse(txtVals[3], System.Globalization.NumberStyles.AllowHexSpecifier);
            FrameTransmit.Send(boardAddr, 1, 0xa1, data, 7);
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                type = int.Parse(button.Tag.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] txtVals = textBox8.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            byte[] data = new byte[txtVals.Length];
            for (int i = 0; i < txtVals.Length; i++)
            {
                data[i] = byte.Parse(txtVals[i], System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            FrameTransmit.Send(boardAddr, 1, byte.Parse(textBox7.Text, System.Globalization.NumberStyles.AllowHexSpecifier), data, data.Length);
        }
    }
}