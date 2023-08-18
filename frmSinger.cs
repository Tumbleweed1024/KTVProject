using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTVProject
{
    public partial class frmSinger : Form
    {
        public frmSinger()
        {
            InitializeComponent();
        }
        public frmIndex frmindex;

        private void frmSinger_Load(object sender, EventArgs e)
        {
            //MV置顶，防止无法全屏
            pn_mv.BringToFront();
        }
        //MV全屏方法，通过底栏按钮调用
        public void mvDock(bool full)
        {
            if (full)
            {
                this.pn_mv.Dock = DockStyle.Fill;
            }
            else
            {
                this.pn_mv.Dock = DockStyle.None;
            }
        }
    }
}
