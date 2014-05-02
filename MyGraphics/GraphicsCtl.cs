using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MyGraphics
{
    public partial class GraphicsCtl : UserControl
    {
        Graphics gp;
        public GraphicsCtl()
        {
            InitializeComponent();
            gp = this.CreateGraphics();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            gp.Clear(Color.Black);
        }
    }
}