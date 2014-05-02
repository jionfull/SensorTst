using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ComputerMonitor
{
    public partial class MyDraw : UserControl
    {
        private Graphics gp;
        public MyDraw()
        {
            InitializeComponent();
            gp = this.CreateGraphics();
        }

        private void MyDraw_Load(object sender, EventArgs e)
        {
            gp.Clear(Color.Black);

        }

        private void MyDraw_Paint(object sender, PaintEventArgs e)
        {
            
        }
    }
}
