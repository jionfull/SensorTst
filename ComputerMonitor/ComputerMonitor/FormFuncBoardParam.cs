using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComputerMonitor.Helper;
using System.IO;
using System.Threading;
using ComputerMonitor.Protocol;

namespace ComputerMonitor
{
    public partial class FormFuncBoardParam : Form
    {

        const int LimitVal1 = 80;
        const int LimitVal2 = 8192;
        const int LimitVal3 = 32768;
        byte[] frameBuffer1 = new byte[LimitVal1 + 7];
        byte[] frameBuffer2 = new byte[LimitVal2 + 7];
        byte[] frameBuffer3 = new byte[LimitVal3 + 7];

        byte[] paramValue=new byte[4+72*8];

        DataSet setParam = new DataSet();

        public FormFuncBoardParam()
        {
            InitializeComponent();
        }

        string[] colCaption = new string[] { "序号", "通道类型", "上传数据数量","关联通道" ,"功能板地址", "通道号", "功能板地址", "通道号","通道参数" };

        int[] rowIndex = new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 0, };
        int[] colIndex = new int[] { 0, 1, 2, 3, 3, 4, 5, 6, 7 };
        int[] rowSpan = new int[] { 2, 2, 2, 1, 1, 1, 1, 1, 2, };
        int[] colSpan = new int[] { 1, 1, 1, 4, 1, 1, 1, 1, 1, };

        private void SetGrid()
        {
            grid1.Redim(74,8);
            grid1.FixedRows = 2;
            grid1.FixedColumns = 1;
            
            for (int i = 0; i < colCaption.Length; i++)
            {
                grid1[rowIndex[i], colIndex[i]] = new SourceGrid.Cells.Header(colCaption[i]);
                grid1[rowIndex[i], colIndex[i]].RowSpan = rowSpan[i];
                grid1[rowIndex[i], colIndex[i]].ColumnSpan = colSpan[i];
                grid1[rowIndex[i], colIndex[i]].View = GridView.HeaderView;
            }
            grid1.Rows[0].Height = 25;
            grid1.Rows[1].Height = 25;
            grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;

            for (int i = 0; i < 72; i++)
            {
                grid1[i + 2, 0] = new SourceGrid.Cells.RowHeader(i+1);
                grid1[i + 2, 1] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[i + 2, 2] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[i + 2, 3] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[i + 2, 4] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[i + 2, 5] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[i + 2, 6] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[i + 2, 7] = new SourceGrid.Cells.Cell("", typeof(string));
            }

            grid1.AutoSizeCells();
            grid1.AutoStretchColumnsToFitWidth = true;
            grid1.Columns.StretchToFit();
        }


        private void FormFuncBoardParam_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            //buttonExport.Enabled = false;
            SetGrid();
            comboBox2.SelectedIndexChanged += new EventHandler(comboBox2_SelectedIndexChanged);
            
        }

        void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string addr = comboBox2.Text;
            bool match = false;
            for (int i = 0; i < setParam.Tables.Count; i++)
            {
                DataTable table = setParam.Tables[i];
                if (table.TableName.Contains("功能板"))
                {
                    if (table.Columns.Contains("板地址"))
                    {
                        table.Columns["板地址"].DefaultValue = addr;
                        if (table.Columns["板地址"].DefaultValue.ToString() == addr)
                        {
                            match = true;
                            if (table.Columns.Contains("板类型"))
                            {
                                int type = int.Parse(table.Columns["板类型"].DefaultValue.ToString());
                                if (type >= 1)
                                {
                                    comboBox1.SelectedIndex = type - 1;
                                }

                            }
                            for (int j = 0; j < 72; j++)
                            {
                                if (table.Columns.Contains("CH" + j))
                                {
                                    string[] vals = table.Columns["CH" + j].DefaultValue.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (vals.Length >= 7)
                                    {
                                        for (int k = 0; k < 7; k++)
                                        {
                                            grid1[j + 2, k + 1].Value = vals[k];
                                        }
                                    }
                                }
                                else
                                {
                                    for (int k = 0; k < 7; k++)
                                    {
                                        grid1[j + 2, k + 1].Value = "";
                                    }
                                }
                            }
                            break;
                        }

                    }
                }
            }
            if (match == false)
            {
                for (int j = 0; j < 72; j++)
                {
                    for (int k = 0; k < 7; k++)
                    {
                        grid1[j + 2, k + 1].Value = "";
                    }
                }
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "INI 文件|*.ini";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, ASCIIEncoding.Default))
                        {
                            sw.WriteLine("[功能板1]");
                            sw.WriteLine("板地址="+comboBox2.Text);
                            sw.WriteLine("板类型=" + (comboBox1.SelectedIndex+1));
                            int chCount = 0;
                            for (int i = 0; i < 72; i++)
                            {
                                if (grid1[i + 2, 1].Value == null)
                                {
                                    break;
                                }
                                chCount++;
                            }
                            sw.WriteLine("通道数=" + chCount);
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < chCount; i++)
                            {
                                sb.Length = 0;
                                sb.Append("CH" + i + "=");
                                for (int j = 0; j < 7; j++)
                                {
                                    sb.Append(grid1[i+2,j+1].DisplayText);
                                    if (j != 6)
                                    {
                                        sb.Append(",");
                                    }
                                }
                                sw.WriteLine(sb.ToString());
                            }

                            sw.Flush();
                        }
                    }


                    MessageBox.Show("数据导出完毕!","",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }

            }
        }

        int paramLength = 0;
        byte[] frameBuffer = null;
        int LimitValue = LimitVal1;
        byte[] dataBuffer=new byte[1024];
        Thread threadSend;
        private void ProcSend()
        {
            FrameUnit frameUnit = FrameManager.CreateFrame(dataBuffer, 1);
            try
            {
                if (paramLength > 0)
                {
                    int startIndex = 0;
                    while (paramLength > 0)
                    {
                        if (paramLength > LimitValue)
                        {

                            frameBuffer[0] = (byte)(startIndex & 0xff);
                            frameBuffer[1] = (byte)((startIndex >> 8) & 0xff); //帧序号
                            frameBuffer[2] = paramValue[1];
                            frameBuffer[3] = 0;
                            frameBuffer[4] = 0;
                            for (int i = 0; i < LimitValue; i++)
                            {
                                frameBuffer[5 + i] = paramValue[startIndex * LimitValue + i];
                            }
                            ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, LimitValue + 5);
                            frameBuffer[LimitValue + 5] = (byte)(crc & 0xff);
                            frameBuffer[LimitValue + 6] = (byte)((crc >> 8) & 0xff);
                            //CPU板地址为1
                            Protocol.FrameTransmit.Send(1, paramValue[0], 0x01, frameBuffer, LimitValue + 7);
                            startIndex++;
                            frameUnit.WaitData(500);
                        }
                        else
                        {
                            frameBuffer[0] = (byte)(startIndex & 0xff);
                            frameBuffer[1] = (byte)(((startIndex >> 8) & 0xff) | 0x80); //帧序号,bit15为1代表最后一帧
                            frameBuffer[2] = paramValue[1];
                            frameBuffer[3] = 0;
                            frameBuffer[4] = 0;
                            for (int i = 0; i < paramLength; i++)
                            {
                                frameBuffer[5 + i] = paramValue[startIndex * LimitValue + i];
                            }
                            ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, paramLength + 5);
                            frameBuffer[paramLength + 5] = (byte)(crc & 0xff);
                            frameBuffer[paramLength + 6] = (byte)((crc >> 8) & 0xff);
                            //CPU板地址为1
                            Protocol.FrameTransmit.Send(1, paramValue[0], 0x01, frameBuffer, paramLength + 7);
                            //发送完毕
                            startIndex++;
                            frameUnit.WaitData(500);
                        }
                        paramLength -= LimitValue;
                    }
                }
                else
                {
                    frameBuffer[0] = (byte)(0 & 0xff);
                    frameBuffer[1] = (byte)(((0 >> 8) & 0xff) | 0x80); //帧序号,bit15为1代表最后一帧
                    frameBuffer[2] = paramValue[1];
                    frameBuffer[3] = 0;
                    frameBuffer[4] = 0;
                    frameBuffer[5] = paramValue[0];
                    frameBuffer[6] = paramValue[1];
                    ushort crc = Helper.CRC16.ComputeCRC16(frameBuffer, 0, 7);
                    frameBuffer[7] = (byte)(crc & 0xff);
                    frameBuffer[8] = (byte)((crc >> 8) & 0xff);
                    Protocol.FrameTransmit.Send(1, paramValue[0], 0x01, frameBuffer, 9);
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

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            
            
            paramValue[0] = byte.Parse(comboBox2.Text); //板地址
            paramValue[1] = (byte)(comboBox1.SelectedIndex + 1);//功能板类型
            paramValue[3] = 0;
            for (int i = 0; i < 72; i++)
            {
                if (grid1[i + 2, 1].Value == null) break;
                paramValue[4 + i * 8 + 0] = (byte)Tool.FromString(grid1[i + 2, 1].Value.ToString());
                paramValue[4 + i * 8 + 1] = (byte)Tool.FromString(grid1[i + 2, 2].Value.ToString());
                paramValue[4 + i * 8 + 2] = (byte)Tool.FromString(grid1[i + 2, 3].Value.ToString());
                paramValue[4 + i * 8 + 3] = (byte)Tool.FromString(grid1[i + 2, 4].Value.ToString());
                paramValue[4 + i * 8 + 4] = (byte)Tool.FromString(grid1[i + 2, 5].Value.ToString());
                paramValue[4 + i * 8 + 5] = (byte)Tool.FromString(grid1[i + 2, 6].Value.ToString());
                int param = Tool.FromString(grid1[i + 2, 7].Value.ToString());
                paramValue[4 + i * 8 + 6] = (byte)(param & 0xff);
                paramValue[4 + i * 8 + 7] = (byte)((param >> 8) & 0xff);

                paramLength = 4 + i * 8 + 8;
                paramValue[2] = (byte)(i + 1);
            }
            
            
            if (comboBox1.SelectedIndex < 2)
            {
                frameBuffer = frameBuffer1;
                LimitValue = LimitVal1;
            }
            else if (comboBox1.SelectedIndex < 3)
            {
                frameBuffer = frameBuffer2;
                LimitValue = LimitVal2;
            }
            else
            {
                frameBuffer = frameBuffer3;
                LimitValue = LimitVal3;
            }
            if (threadSend != null && threadSend.IsAlive)
            {
                threadSend.Abort();
            }
            threadSend = new Thread(new ThreadStart(ProcSend));
            threadSend.IsBackground = true;
            threadSend.Start();


        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd=new OpenFileDialog())
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

                    buttonExport.Enabled = true;
                }
            }

            comboBox2_SelectedIndexChanged(null, null);
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            if (setParam.Tables.Count > 0)
            {
                for (int i = 0; i < setParam.Tables.Count; i++)
                {
                    if (setParam.Tables[i].Columns.Contains("板地址") && setParam.Tables[i].Columns["板地址"].DefaultValue.ToString() == comboBox2.Text)
                    {
                        setParam.Tables[i].Columns["板类型"].DefaultValue = (comboBox1.SelectedIndex + 1).ToString();
                        break;
                    }
                }
            }
             */
        }

        




    }
}