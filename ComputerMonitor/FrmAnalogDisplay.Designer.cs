namespace ComputerMonitor
{
    partial class FrmAnalogDisplay
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
            this.txtAnalog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtAnalog
            // 
            this.txtAnalog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAnalog.Location = new System.Drawing.Point(0, 0);
            this.txtAnalog.Multiline = true;
            this.txtAnalog.Name = "txtAnalog";
            this.txtAnalog.Size = new System.Drawing.Size(292, 273);
            this.txtAnalog.TabIndex = 0;
            // 
            // FrmAnalogDisplay
            // 
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.txtAnalog);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FrmAnalogDisplay";
            this.Text = "FrmAnalogDisplay";
            this.Load += new System.EventHandler(this.FrmAnalogDisplay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAnalog;
    }
}