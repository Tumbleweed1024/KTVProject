using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTVProject
{
    public partial class frmSinger : Form
    {
        public frmIndex frmindex;
        public frmSinger()
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
        //歌手名单集合
        List<Singers> ls = new List<Singers>();
        //歌星名单当前页
        int pageNow = 1;
        //歌星名单每一页数据量
        int pageSize = 6;
        //歌星名单总页数
        int zong = 0;
        //歌星歌曲名单当前页
        int pageNow1 = 1;
        //歌星歌曲名单每一页数据量
        int pageSize1 = 6;
        //歌星歌曲名单总页数
        int pageZong1 = 0;
        string leixing = "0";//性别
        int singerId = 0;//歌手编号
        private void frmSinger_Load(object sender, EventArgs e)
        {
            //MV置顶，防止无法全屏
            pn_mv.BringToFront();
            //先不让歌曲显示
            this.panel9.Visible = false;
            //显示总数据
            this.getZong();
            //显示歌手
            showSinger();
            jinyong();
            //“全部”选项变蓝色
            lblAll.ForeColor = Color.RoyalBlue;
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
                    tiaojian += " and SingerAbbr like '%" + py + "%'";
                }
                if (!leixing.Equals("0"))
                {
                    tiaojian += " and SingerType= " + leixing;
                }

                //if (!diqu.Equals("0"))
                //{
                //    tiaojian += " and languageId=" + diqu;
                //}

                //if (!py.Trim().Equals(""))
                //{
                //    tiaojian += " and Singersuoxie like '" + py + "%'";
                //}

                string sql = "select count(*)  from Singer where SingerDisplay='True'  " + tiaojian + "";
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
        //显示歌手
        //歌星分页显示
        public void showSinger()
        {
            string py = this.label21.Text.Trim();
            try
            {
                DBHelper.OpenConnection();
                string tiaojian = "";

                if (!leixing.Equals("0"))
                {
                    tiaojian += " and SingerType= " + leixing;
                }

                //if (!diqu.Equals("0"))
                //{
                //    tiaojian += " and languageId=" + diqu;
                //}

                if (!py.Trim().Equals(""))
                {
                    tiaojian += " and SingerAbbr like '%" + py + "%'";
                }
                string sql = "select top " + pageSize + " SingerID,SingerName,SingerCoverPath from Singer where SingerDisplay='True' and SingerID not in(select top " + (pageSize * (pageNow - 1)) + " SingerID from Singer where SingerDisplay='True'" + tiaojian + ")" + tiaojian + "";
                SqlDataReader reader = DBHelper.GetExecuteReader(sql);
                for (int i = 0; i < this.panel1.Controls.Count; i++)
                {
                    if (reader.Read())
                    {
                        //0表示歌手名称，1表示图像
                        this.panel1.Controls[i].Controls[0].Text = reader["SingerName"].ToString();
                        ((PictureBox)(this.panel1.Controls[i].Controls[1])).Image = Image.FromFile(reader["SingerCoverPath"].ToString());
                        this.panel1.Controls[i].Controls[1].Tag = reader["SingerID"].ToString();
                    }
                    else
                    {
                        this.panel1.Controls[i].Controls[0].Text = "";
                        ((PictureBox)(this.panel1.Controls[i].Controls[1])).Image = null;
                    }
                    //如果没读到信息就把歌手框隐藏
                    if (this.panel1.Controls[i].Controls[0].Text.Trim().Equals(""))
                    {
                        this.panel1.Controls[i].Visible = false;
                    }
                    else
                    {
                        this.panel1.Controls[i].Visible = true;
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("歌手分页" + ex.Message);
            }
            finally
            {
                DBHelper.CloseConnection();
            }
        }
        //歌手上下页禁用
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

            if (page == zong)
            {
                this.lblxiayiye.Enabled = false;
            }
            else
            {
                this.lblxiayiye.Enabled = true;
                this.lblxiayiye.Cursor = Cursors.Hand;
            }
        }
        //歌手上一页
        private void lblshangyiye_Click(object sender, EventArgs e)
        {
            pageNow--;
            showSinger();//显示下一页歌手列表
            this.lblDangqianye.Text = pageNow.ToString();
            jinyong();
        }
        //歌手下一页
        private void lblxiayiye_Click(object sender, EventArgs e)
        {
            pageNow++;
            showSinger();//显示下一页歌手列表
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

        //男
        private void lblNan_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showSinger();
            jinyong();
            SetColor(sender, e);
        }
        //女
        private void lblNv_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showSinger();
            jinyong();
            SetColor(sender, e);
        }
        //组合
        private void lblZuhe_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showSinger();
            jinyong();
            SetColor(sender, e);
        }
        //全部
        private void lblAll_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showSinger();
            jinyong();
            SetColor(sender, e);
        }
        //显示歌曲
        public void showSong()
        {
            string py = this.label21.Text;
            string tiaojian = "";
            try
            {
                DBHelper.OpenConnection();
                if (!py.Trim().Equals(""))
                {
                    tiaojian += " and m.MusicAbbr like '%" + py + "%'";
                }
                string sql = "select top " + pageSize1 + "     MusicID,MusicName,s.SingerName, '点歌' as dg, MusicID from Music as m,Singer as s where m.SingerID=s.SingerID and m.MusicID not in(select top " + (pageSize1 * (pageNow1 - 1)) + " MusicID from Music where MusicDisplay='True' and SingerID=" + singerId + " " + tiaojian + ") and m.MusicDisplay='True' and s.SingerID=" + singerId + " " + tiaojian + " ";
                SqlDataReader reader = DBHelper.GetExecuteReader(sql);
                for (int i = 0; i < this.panel10.Controls.Count; i++)
                {
                    if (reader.Read())
                    {
                        this.panel10.Controls[i].Controls[2].Text = reader["MusicName"].ToString();
                        this.panel10.Controls[i].Controls[1].Text = reader["SingerName"].ToString();
                        this.panel10.Controls[i].Controls[0].Text = reader["dg"].ToString();
                        this.panel10.Controls[i].Controls[0].Tag = reader["MusicID"].ToString();
                    }
                    else
                    {
                        this.panel10.Controls[i].Controls[2].Text = "";
                        this.panel10.Controls[i].Controls[1].Text = "";
                        this.panel10.Controls[i].Controls[0].Text = "";
                        this.panel10.Controls[i].Controls[2].Tag = 0;
                    }
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DBHelper.CloseConnection();
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            singerId = Convert.ToInt32(((Control)sender).Tag);
            this.panel9.Visible = true;
            this.panel9.BringToFront();
            //this.panel9.Dock = DockStyle.Fill;
            getSongZong();
            this.label21.Text = string.Empty;
            showSong();
            jinyong2();
        }
        //歌曲的上下页
        public void jinyong2()
        {
            int page = int.Parse(this.lblDang.Text);
            int zong = int.Parse(this.lblzong.Text);
            if (page == 1)
            {
                this.lblsyy.Enabled = false;
            }
            else
            {
                this.lblsyy.Enabled = true;
                this.lblsyy.Cursor = Cursors.Hand;
            }

            if (page == pageZong1)
            {
                this.lblxyy.Enabled = false;
            }
            else
            {
                this.lblxyy.Enabled = true;
                this.lblxyy.Cursor = Cursors.Hand;
            }
        }

        //获取总页数歌曲
        public void getSongZong()
        {
            string py = this.label21.Text.ToString();
            try
            {
                DBHelper.OpenConnection();
                string tiaojian = "";
                if (!py.Trim().Equals(""))
                {
                    tiaojian += " and MusicAbbr like '" + py + "'";
                }
                string sql = "select count(*) from Music  where MusicDisplay='True' and SingerID =" + singerId;
                int count = (int)DBHelper.GetExecuteScalar(sql);
                if (count % pageSize1 == 0)
                {
                    pageZong1 = count / pageSize1;
                }
                else
                {
                    pageZong1 = (count / pageSize1) + 1;
                }
                this.lblDang.Text = 1.ToString();
                this.lblzong.Text = pageZong1.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DBHelper.CloseConnection();
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {

            string songId = ((Label)sender).Tag.ToString();
            if (frmindex.diange(songId))
            {
                frmindex.MusicID = songId;//将SongId传给主页SongId
                frmindex.countjia();//执行增加点击次数
                this.label17.Text = "已点";
                //this.panel14.BackgroundImage = Image.FromFile(@"G:\项目\Ktv项目1\图片\ttt1.png");
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            string songId = ((Label)sender).Tag.ToString();
            if (frmindex.diange(songId))
            {
                frmindex.MusicID = songId;//将SongId传给主页SongId
                frmindex.countjia();//执行增加点击次数
                this.label14.Text = "已点";
                //this.panel13.BackgroundImage = Image.FromFile(@"G:\项目\Ktv项目1\图片\ttt1.png");
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            string songId = ((Label)sender).Tag.ToString();
            if (frmindex.diange(songId))
            {
                frmindex.MusicID = songId;//将SongId传给主页SongId
                frmindex.countjia();//执行增加点击次数
                this.label11.Text = "已点";
                //this.panel12.BackgroundImage = Image.FromFile(@"G:\项目\Ktv项目1\图片\ttt1.png");
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {
            string songId = ((Label)sender).Tag.ToString();
            if (frmindex.diange(songId))
            {
                frmindex.MusicID = songId;//将SongId传给主页SongId
                frmindex.countjia();//执行增加点击次数
                this.label10.Text = "已点";
                //this.panel11.BackgroundImage = Image.FromFile(@"G:\项目\Ktv项目1\图片\ttt1.png");
            }
        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {
            pageNow = 1;
            this.label21.Text += ((PictureBox)sender).Tag.ToString();
        }

        private void label21_TextChanged(object sender, EventArgs e)
        {
            getZong();
            showSinger();
        }

        private void label20_Click(object sender, EventArgs e)
        {
            this.panel9.Visible = false;
            this.panel9.SendToBack();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.label21.Text.Trim().Length == 0)
            {
                return;
            }
            this.label21.Text = this.label21.Text.Substring(0, this.label21.Text.Length - 1);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            this.label21.Text = string.Empty;
        }
    }
}
