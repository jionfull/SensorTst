using ComputerMonitor.Protocol;
namespace ComputerMonitor
{
    partial class FormSignalCurrentCalc
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {

                threadReadCh.Abort();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTster = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gridValueView = new SourceGrid.Grid();
            this.txtChVal = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonExt4 = new ComputerMonitor.ButtonExt();
            this.buttonExt3 = new ComputerMonitor.ButtonExt();
            this.buttonExt2 = new ComputerMonitor.ButtonExt();
            this.buttonExt1 = new ComputerMonitor.ButtonExt();
            this.btnRun = new ComputerMonitor.ButtonExt();
            this.btnStartCalc = new ComputerMonitor.ButtonExt();
            this.buttonExt5 = new ComputerMonitor.ButtonExt();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.SystemColors.Info;
            this.txtMessage.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMessage.Location = new System.Drawing.Point(12, 276);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(504, 411);
            this.txtMessage.TabIndex = 0;
            this.txtMessage.DoubleClick += new System.EventHandler(this.txtMessage_DoubleClick);
            this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormAutoCalc_KeyPress);
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(664, 313);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(110, 21);
            this.txtId.TabIndex = 15;
            this.txtId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormAutoCalc_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(584, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "板卡Id：";
            // 
            // txtTster
            // 
            this.txtTster.Location = new System.Drawing.Point(664, 359);
            this.txtTster.Name = "txtTster";
            this.txtTster.Size = new System.Drawing.Size(110, 21);
            this.txtTster.TabIndex = 17;
            this.txtTster.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormAutoCalc_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(584, 362);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "测试人工号：";
            // 
            // gridValueView
            // 
            this.gridValueView.Location = new System.Drawing.Point(12, 12);
            this.gridValueView.Name = "gridValueView";
            this.gridValueView.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridValueView.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridValueView.Size = new System.Drawing.Size(885, 184);
            this.gridValueView.TabIndex = 21;
            this.gridValueView.TabStop = true;
            this.gridValueView.ToolTipText = "";
            // 
            // txtChVal
            // 
            this.txtChVal.Location = new System.Drawing.Point(170, 215);
            this.txtChVal.Name = "txtChVal";
            this.txtChVal.Size = new System.Drawing.Size(183, 21);
            this.txtChVal.TabIndex = 26;
            this.txtChVal.Text = "300";
            this.txtChVal.TextChanged += new System.EventHandler(this.txtChVal_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 218);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 27;
            this.label3.Text = "当前实际电流值：";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // buttonExt4
            // 
            this.buttonExt4.Location = new System.Drawing.Point(744, 587);
            this.buttonExt4.Name = "buttonExt4";
            this.buttonExt4.Size = new System.Drawing.Size(85, 23);
            this.buttonExt4.TabIndex = 25;
            this.buttonExt4.Text = "读测试人信息";
            this.buttonExt4.UseVisualStyleBackColor = true;
            this.buttonExt4.Click += new System.EventHandler(this.buttonExt4_Click);
            // 
            // buttonExt3
            // 
            this.buttonExt3.Location = new System.Drawing.Point(744, 532);
            this.buttonExt3.Name = "buttonExt3";
            this.buttonExt3.Size = new System.Drawing.Size(85, 23);
            this.buttonExt3.TabIndex = 24;
            this.buttonExt3.Text = "手动写出厂信息";
            this.buttonExt3.UseVisualStyleBackColor = true;
            this.buttonExt3.Click += new System.EventHandler(this.buttonExt3_Click);
            // 
            // buttonExt2
            // 
            this.buttonExt2.Location = new System.Drawing.Point(744, 485);
            this.buttonExt2.Name = "buttonExt2";
            this.buttonExt2.Size = new System.Drawing.Size(85, 23);
            this.buttonExt2.TabIndex = 23;
            this.buttonExt2.Text = "读取出厂信息";
            this.buttonExt2.UseVisualStyleBackColor = true;
            this.buttonExt2.Click += new System.EventHandler(this.buttonExt2_Click);
            // 
            // buttonExt1
            // 
            this.buttonExt1.Location = new System.Drawing.Point(576, 587);
            this.buttonExt1.Name = "buttonExt1";
            this.buttonExt1.Size = new System.Drawing.Size(85, 23);
            this.buttonExt1.TabIndex = 22;
            this.buttonExt1.Text = "读取系数";
            this.buttonExt1.UseVisualStyleBackColor = true;
            this.buttonExt1.Click += new System.EventHandler(this.buttonExt1_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(576, 532);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(85, 23);
            this.btnRun.TabIndex = 20;
            this.btnRun.Text = "自动测试";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click_1);
            this.btnRun.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormAutoCalc_KeyPress);
            // 
            // btnStartCalc
            // 
            this.btnStartCalc.Location = new System.Drawing.Point(576, 485);
            this.btnStartCalc.Name = "btnStartCalc";
            this.btnStartCalc.Size = new System.Drawing.Size(85, 23);
            this.btnStartCalc.TabIndex = 5;
            this.btnStartCalc.Text = "自动校准";
            this.btnStartCalc.UseVisualStyleBackColor = true;
            this.btnStartCalc.Click += new System.EventHandler(this.btnStartCalc_Click);
            this.btnStartCalc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormAutoCalc_KeyPress);
            // 
            // buttonExt5
            // 
            this.buttonExt5.Location = new System.Drawing.Point(576, 644);
            this.buttonExt5.Name = "buttonExt5";
            this.buttonExt5.Size = new System.Drawing.Size(85, 23);
            this.buttonExt5.TabIndex = 28;
            this.buttonExt5.Text = "写入默认系数";
            this.buttonExt5.UseVisualStyleBackColor = true;
            this.buttonExt5.Click += new System.EventHandler(this.buttonExt5_Click);
            // 
            // FormSignalCurrentCalc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 746);
            this.Controls.Add(this.buttonExt5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtChVal);
            this.Controls.Add(this.buttonExt4);
            this.Controls.Add(this.buttonExt3);
            this.Controls.Add(this.buttonExt2);
            this.Controls.Add(this.buttonExt1);
            this.Controls.Add(this.gridValueView);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTster);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.btnStartCalc);
            this.Controls.Add(this.txtMessage);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.KeyPreview = true;
            this.Name = "FormSignalCurrentCalc";
            this.Text = "FormAutoCalc";
            this.Load += new System.EventHandler(this.FormAutoCalc_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAutoCalc_FormClosed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormAutoCalc_KeyPress);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAutoCalc_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessage;
        private ButtonExt btnStartCalc;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTster;
        private System.Windows.Forms.Label label2;
        private ButtonExt btnRun;
        private SourceGrid.Grid gridValueView;
        private ButtonExt buttonExt1;
        private ButtonExt buttonExt2;
        private ButtonExt buttonExt3;
        private ButtonExt buttonExt4;
        private System.Windows.Forms.TextBox txtChVal;
        private System.Windows.Forms.Label label3;
        private ButtonExt buttonExt5;
    }
}