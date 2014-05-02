namespace ComputerMonitor
{
    partial class FormAutoTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_testMessage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStartTest = new System.Windows.Forms.Button();
            this.btnSaveFiles = new System.Windows.Forms.Button();
            this.textBoxBoardAddress = new System.Windows.Forms.TextBox();
            this.textBoxChannelNo = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_TrueShow = new System.Windows.Forms.Label();
            this.label_TrueValue = new System.Windows.Forms.Label();
            this.label_TestValue = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_testMessage
            // 
            this.textBox_testMessage.BackColor = System.Drawing.SystemColors.Info;
            this.textBox_testMessage.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_testMessage.ForeColor = System.Drawing.SystemColors.InfoText;
            this.textBox_testMessage.Location = new System.Drawing.Point(227, 50);
            this.textBox_testMessage.Multiline = true;
            this.textBox_testMessage.Name = "textBox_testMessage";
            this.textBox_testMessage.ReadOnly = true;
            this.textBox_testMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_testMessage.Size = new System.Drawing.Size(277, 373);
            this.textBox_testMessage.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(13, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "板地址：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(13, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "通道号：";
            // 
            // btnStartTest
            // 
            this.btnStartTest.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStartTest.Location = new System.Drawing.Point(59, 134);
            this.btnStartTest.Name = "btnStartTest";
            this.btnStartTest.Size = new System.Drawing.Size(75, 23);
            this.btnStartTest.TabIndex = 3;
            this.btnStartTest.Text = "开始测试";
            this.btnStartTest.UseVisualStyleBackColor = true;
            this.btnStartTest.Click += new System.EventHandler(this.btnStartTest_Click);
            // 
            // btnSaveFiles
            // 
            this.btnSaveFiles.Font = new System.Drawing.Font("新宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSaveFiles.Location = new System.Drawing.Point(59, 184);
            this.btnSaveFiles.Name = "btnSaveFiles";
            this.btnSaveFiles.Size = new System.Drawing.Size(75, 23);
            this.btnSaveFiles.TabIndex = 7;
            this.btnSaveFiles.Text = "保存文件";
            this.btnSaveFiles.UseVisualStyleBackColor = true;
            this.btnSaveFiles.Click += new System.EventHandler(this.btnSaveFiles_Click);
            // 
            // textBoxBoardAddress
            // 
            this.textBoxBoardAddress.Location = new System.Drawing.Point(89, 38);
            this.textBoxBoardAddress.Name = "textBoxBoardAddress";
            this.textBoxBoardAddress.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBoxBoardAddress.Size = new System.Drawing.Size(100, 21);
            this.textBoxBoardAddress.TabIndex = 8;
            this.textBoxBoardAddress.Text = "3\r\n";
            this.textBoxBoardAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxChannelNo
            // 
            this.textBoxChannelNo.Location = new System.Drawing.Point(89, 77);
            this.textBoxChannelNo.Name = "textBoxChannelNo";
            this.textBoxChannelNo.Size = new System.Drawing.Size(100, 21);
            this.textBoxChannelNo.TabIndex = 9;
            this.textBoxChannelNo.Text = "0";
            this.textBoxChannelNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Controls.Add(this.label_TrueShow);
            this.panel1.Location = new System.Drawing.Point(12, 284);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 10;
            // 
            // label_TrueShow
            // 
            this.label_TrueShow.AutoSize = true;
            this.label_TrueShow.BackColor = System.Drawing.SystemColors.ControlText;
            this.label_TrueShow.Font = new System.Drawing.Font("宋体", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_TrueShow.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label_TrueShow.Location = new System.Drawing.Point(3, 18);
            this.label_TrueShow.Name = "label_TrueShow";
            this.label_TrueShow.Size = new System.Drawing.Size(193, 64);
            this.label_TrueShow.TabIndex = 11;
            this.label_TrueShow.Text = "0.000";
            // 
            // label_TrueValue
            // 
            this.label_TrueValue.AutoSize = true;
            this.label_TrueValue.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_TrueValue.Location = new System.Drawing.Point(24, 259);
            this.label_TrueValue.Name = "label_TrueValue";
            this.label_TrueValue.Size = new System.Drawing.Size(68, 16);
            this.label_TrueValue.TabIndex = 11;
            this.label_TrueValue.Text = "实际值:";
            // 
            // label_TestValue
            // 
            this.label_TestValue.AutoSize = true;
            this.label_TestValue.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_TestValue.Location = new System.Drawing.Point(224, 20);
            this.label_TestValue.Name = "label_TestValue";
            this.label_TestValue.Size = new System.Drawing.Size(76, 16);
            this.label_TestValue.TabIndex = 12;
            this.label_TestValue.Text = "测试值：";
            // 
            // FormAutoTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 443);
            this.Controls.Add(this.label_TestValue);
            this.Controls.Add(this.label_TrueValue);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBoxChannelNo);
            this.Controls.Add(this.textBoxBoardAddress);
            this.Controls.Add(this.btnSaveFiles);
            this.Controls.Add(this.btnStartTest);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_testMessage);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormAutoTest";
            this.Text = "FormAutoTest";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_testMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStartTest;
        private System.Windows.Forms.Button btnSaveFiles;
        private System.Windows.Forms.TextBox textBoxBoardAddress;
        private System.Windows.Forms.TextBox textBoxChannelNo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_TrueShow;
        private System.Windows.Forms.Label label_TrueValue;
        private System.Windows.Forms.Label label_TestValue;
    }
}