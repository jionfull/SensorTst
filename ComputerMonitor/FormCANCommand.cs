using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ComputerMonitor.Protocol;
using ComputerMonitor.Helper;

namespace ComputerMonitor
{
    public partial class FormCANCommand : Form
    {
        DataSet setParam = new DataSet();

        byte[] canParamData=new byte[4642];
        public FormCANCommand()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "INI 文件|*.ini";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DataTable table = null;
                    setParam.Reset();
                    using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs, ASCIIEncoding.Default))
                        {
                            while (true)
                            {
                                string fileLine = sr.ReadLine();
                                if (string.IsNullOrEmpty(fileLine))
                                {
                                    if (table != null)
                                    {
                                        setParam.Tables.Add(table);
                                    }
                                    break;
                                }
                                fileLine = fileLine.Trim();
                                if (fileLine.StartsWith("["))
                                {
                                    if (table != null)
                                    {
                                        setParam.Tables.Add(table);
                                    }
                                    string[] names = fileLine.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                                    table = new DataTable(names[0]);
                                }
                                else if (fileLine.StartsWith("//")) //注释开头
                                {
                                    continue;
                                }
                                else
                                {
                                    string[] names = fileLine.Split(new char[] { '=', '/', }, StringSplitOptions.RemoveEmptyEntries);
                                    if (table != null)
                                    {
                                        DataColumn col = table.Columns.Add(names[0], typeof(string));
                                        col.DefaultValue = names[1];
                                    }
                                }

                            }

                        }
                    }
                    button2.Enabled = true;
                    //buttonExport.Enabled = true;
                }
            }
        }

        private void FormCANCommand_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int boardCount = setParam.Tables.Count - 1;
            canParamData[0] = (byte)boardCount;
            canParamData[1] = 0;
            int index = 2;
            for (int i = 0; i < setParam.Tables.Count; i++)
            {
                if (setParam.Tables[i].TableName.StartsWith("功能板"))
                {
                    byte boardAddr = byte.Parse(setParam.Tables[i].Columns["板地址"].DefaultValue.ToString());
                    canParamData[index] = boardAddr;
                    index++;
                    byte boardType = byte.Parse(setParam.Tables[i].Columns["板类型"].DefaultValue.ToString());
                    canParamData[index] = boardType;
                    index++;
                    byte channelNum = byte.Parse(setParam.Tables[i].Columns["通道数"].DefaultValue.ToString());
                    canParamData[index] = channelNum;
                    index++;
                    canParamData[index] = 0;//保留
                    index++;



                    for (int j = 0; j < setParam.Tables[i].Columns.Count; j++)
                    {
                        if (setParam.Tables[i].Columns[j].ColumnName.StartsWith("CH"))//通道号
                        {
                            string[] txtVal = setParam.Tables[i].Columns[j].DefaultValue.ToString().Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries);
                            canParamData[index] = (byte)Tool.FromString(txtVal[0]); 
                            index++;
                            canParamData[index] = (byte)Tool.FromString(txtVal[1]); 
                            index++;
                            canParamData[index] = (byte)Tool.FromString(txtVal[2]); 
                            index++;
                            canParamData[index] = (byte)Tool.FromString(txtVal[3]); 
                            index++;
                            canParamData[index] = (byte)Tool.FromString(txtVal[4]); 
                            index++;
                            canParamData[index] = (byte)Tool.FromString(txtVal[5]); 
                            index++;
                            ushort s = (ushort)Tool.FromString(txtVal[6]); 
                            canParamData[index] = (byte)(s & 0xff);
                            index++;
                            canParamData[index] = (byte)(s >>8);
                            index++;
                        }
                    }

                   

                }
            }

            byte canAddr = byte.Parse(textBox1.Text);

            byte[] CanData = new byte[8];
            int sendLength = index;
            int startIndex = 0;
            while (sendLength > 0)
            {
                if (sendLength <= 7)
                {
                    Array.Copy(canParamData, startIndex, CanData, 1, sendLength);
                    CanData[0] = 0x30; //结束帧
                    CANFrameTransmit.Send(canAddr, 1, CanData, sendLength + 1);
                    sendLength = 0;


                }
                else
                {
                    Array.Copy(canParamData, startIndex, CanData, 1, 7);
                    if (startIndex == 0)
                    {

                        CanData[0] = 0x10; //开始帧
                    }
                    else
                    {
                        CanData[0] = 0x20;//为中间帧
                    }
                    CANFrameTransmit.Send(canAddr, 1, CanData, 8);
                    sendLength -= 7;
                    startIndex += 7;
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte canAddr = byte.Parse(textBox1.Text);
            byte[] CanData = new byte[8];
            CanData[0] = 0;
            CanData[1] = 0;
            CANFrameTransmit.Send(canAddr, 0, CanData, 2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte canAddr = (byte)Tool.FromString(textBox1.Text);
            byte[] CanData = new byte[8];
            byte cmd = (byte)Tool.FromString(textBox3.Text);
            string[] txtVal = textBox2.Text.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
            int c = 0;
            for (int i = 0; i < txtVal.Length; i++)
            {
                if (i >= 8) break;
                CanData[c] = (byte)Tool.FromString(txtVal[i]);
                c++;
            }

            CANFrameTransmit.Send(canAddr, cmd, CanData, c);

        }
    }
}