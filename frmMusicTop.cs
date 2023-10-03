using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTVProject
{
    public partial class frmMusicTop : Form
    {
        public frmIndex frmindex;
        public frmSinger frmsinger;
        public frmMusicTop()
        {
            InitializeComponent();
            //缓存机制，防止加载闪烁
            this.DoubleBuffered = true;//设置本窗体
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
        }
        #region 防止闪屏
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;

            base.WndProc(ref m);
        }
        #endregion
        //歌曲当前页
        int pageNow = 1;
        //歌曲每一页数据量
        int pageSize = 9;
        //歌曲名单总页数
        int zong = 0;
        string leixing = "desc";//类型

        private void frmMusicType_Load(object sender, EventArgs e)
        {
            //MV置顶，防止无法全屏
            pn_mv.BringToFront();
            //显示总数据
            this.getZong();
            //显示歌手
            showMusic();
            jinyong();
            //“全部”选项变蓝色
            lblAll.ForeColor = Color.RoyalBlue;
        }
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
        //远程输入简写
        public void openAbbr(string rAbbr)
        {
            label21.Text = rAbbr;
        }
        //获取总页数
        public void getZong()
        {
            string py = this.label21.Text;
            try
            {
                DBHelper.OpenConnection();
                string tiaojian = "";
                if (!py.Trim().Equals(""))
                {
                    tiaojian += " and MusicAbbr like '%" + py + "%'";
                }
                

                string sql = "select count(*)  from Music where MusicDisplay='True'  " + tiaojian;
                int count = (int)DBHelper.GetExecuteScalar(sql);
                if (count % pageSize == 0)
                {
                    zong = count / pageSize;
                }
                else
                {
                    zong = (count / pageSize) + 1;
                }
                this.lblDangqianye.Text = 1.ToString();
                this.lblZongYe.Text = zong.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("总数据" + ex.Message);
            }
            finally
            {
                DBHelper.CloseConnection();
            }
        }
        //歌曲分页显示
        public void showMusic()
        {
            string py = this.label21.Text.Trim();
            try
            {
                DBHelper.OpenConnection();
                string tiaojian = "";
                if (!py.Trim().Equals(""))
                {
                    tiaojian += " and MusicAbbr like '%" + py + "%'";
                }
                
                string sql = "select top " + pageSize + " MusicID,MusicName,Singer.SingerName,Music.SingerID,MusicTypeTable.MusicTypeName,Music.MusicTypeID,MusicViews,'点歌' as dg from Music  INNER JOIN Singer ON Music.SingerID = Singer.SingerID INNER JOIN MusicTypeTable ON Music.MusicTypeID = MusicTypeTable.MusicTypeID where SingerDisplay='True' and MusicID not in(select top " + (pageSize * (pageNow - 1)) + " MusicID from Music where MusicDisplay='True'" + tiaojian + " ORDER BY MusicViews " + leixing+")" + tiaojian + " ORDER BY MusicViews " + leixing;
                SqlDataReader reader = DBHelper.GetExecuteReader(sql);
                for (int i = this.panel10.Controls.Count - 1; i >= 0; i--)
                {
                    if (reader.Read())
                    {
                        this.panel10.Controls[i].Controls[4].Text = reader["MusicName"].ToString();
                        this.panel10.Controls[i].Controls[3].Text = reader["SingerName"].ToString();
                        this.panel10.Controls[i].Controls[3].Tag = reader["SingerID"].ToString();
                        this.panel10.Controls[i].Controls[2].Text = reader["MusicTypeName"].ToString();
                        this.panel10.Controls[i].Controls[2].Tag = reader["MusicTypeID"].ToString();
                        this.panel10.Controls[i].Controls[1].Text = reader["MusicViews"].ToString();
                        this.panel10.Controls[i].Controls[0].Text = reader["dg"].ToString();
                        this.panel10.Controls[i].Controls[0].Tag = reader["MusicID"].ToString();
                    }
                    else
                    {
                        this.panel10.Controls[i].Controls[4].Text = "";
                        this.panel10.Controls[i].Controls[3].Text = "";
                        this.panel10.Controls[i].Controls[3].Tag = "";
                        this.panel10.Controls[i].Controls[2].Text = "";
                        this.panel10.Controls[i].Controls[2].Tag = "";
                        this.panel10.Controls[i].Controls[1].Text = "";
                        this.panel10.Controls[i].Controls[0].Text = "";
                        this.panel10.Controls[i].Controls[0].Tag = 0;
                    }
                    foreach (var item1 in frmindex.musics)
                    {
                        if (item1.MusicID == Convert.ToInt32(this.panel10.Controls[i].Controls[0].Tag))
                        {
                            this.panel10.Controls[i].Controls[0].Text = "已点";
                        }
                    }
                    //如果没读到信息就把歌框隐藏
                    if (this.panel10.Controls[i].Controls[4].Text.Trim().Equals(""))
                    {
                        this.panel10.Controls[i].Visible = false;
                    }
                    else
                    {
                        this.panel10.Controls[i].Visible = true;
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("歌曲分页" + ex.Message);
            }
            finally
            {
                DBHelper.CloseConnection();
            }
        }
        //歌曲上下页禁用
        public void jinyong()
        {
            int page = int.Parse(this.lblDangqianye.Text);
            int zong = int.Parse(this.lblZongYe.Text);
            if (page == 1)
            {
                this.lblshangyiye.Enabled = false;
            }
            else
            {
                this.lblshangyiye.Enabled = true;
                this.lblshangyiye.Cursor = Cursors.Hand;
            }

            if (page >= zong)
            {
                this.lblxiayiye.Enabled = false;
            }
            else
            {
                this.lblxiayiye.Enabled = true;
                this.lblxiayiye.Cursor = Cursors.Hand;
            }
        }
        //上一页
        private void lblshangyiye_Click(object sender, EventArgs e)
        {
            pageNow--;
            showMusic();//显示上一页
            this.lblDangqianye.Text = pageNow.ToString();
            jinyong();
        }
        //下一页
        private void lblxiayiye_Click(object sender, EventArgs e)
        {
            pageNow++;
            showMusic();//显示下一页
            this.lblDangqianye.Text = pageNow.ToString();
            jinyong();
        }
        //变色
        public void SetColor(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            for (int i = 0; i < c.Parent.Controls.Count; i++)
            {
                c.Parent.Controls[i].ForeColor = Color.Black;
            }
            c.ForeColor = Color.RoyalBlue;
        }
        //设置类型和调用点击事件
        private void Type_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showMusic();
            jinyong();
            SetColor(sender, e);
        }
        //传值点歌
        private void diange_Click(object sender, EventArgs e)
        {
            string MusicId = ((Label)sender).Tag.ToString();
            if (frmindex.diange(MusicId))
            {
                frmindex.MusicID = MusicId;//将MusicId传给主页MusicId
                frmindex.countjia();//执行增加点击次数
                ((Label)sender).Text = "已点";
            }
        }
        //跳歌手
        private void singer_Click(object sender, EventArgs e)
        {
            frmindex.singer(((Label)sender).Tag.ToString());
        }
        //跳种类
        private void type_Click(object sender, EventArgs e)
        {
            frmindex.musicType(((Label)sender).Tag.ToString());
        }

        //键盘相关
        private void label21_TextChanged(object sender, EventArgs e)
        {

            pageNow = 1;
            getZong();
            showMusic();
            jinyong();
        }
        private void key_Click(object sender, EventArgs e)
        {
            if (label21.Text.Length > 15)
            {
                return;
            }
            this.label21.Text += ((PictureBox)sender).Tag.ToString();
        }

        private void remove_Click(object sender, EventArgs e)
        {
            if (this.label21.Text.Trim().Length == 0)
            {
                return;
            }
            this.label21.Text = this.label21.Text.Substring(0, this.label21.Text.Length - 1);
        }

        private void clear_Click(object sender, EventArgs e)
        {
            if (this.label21.Text.Trim().Length == 0)
            {
                return;
            }
            this.label21.Text = string.Empty;
        }
    }
}
