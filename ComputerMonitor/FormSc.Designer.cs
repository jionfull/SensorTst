namespace ComputerMonitor
{
    partial class FormSc
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
            this.button16 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(301, 12);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(194, 25);
            this.button16.TabIndex = 34;
            this.button16.Text = "一键参数并启动全部采集";
            this.button16.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(301, 52);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(194, 25);
            this.button1.TabIndex = 35;
            this.button1.Text = "停止采集";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FormSc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 427);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button16);
            this.Name = "FormSc";
            this.Text = "FormSc";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button1;
    }
}