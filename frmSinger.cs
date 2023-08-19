﻿using System;
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
    public partial class frmSinger : Form
    {
        public frmIndex frmindex;
        public frmSinger()
        {
            InitializeComponent();
        }
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

            this.panel9.Visible = false;
            //显示总数据
            this.getZong();
            //显示歌手
            showSinger();
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
            string py = this.textBox1.Text;
            try
            {
                DBHelper.OpenConnection();
                string tiaojian = "";
                if (py != string.Empty)
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
            string py = this.textBox1.Text.Trim();
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
                    tiaojian += " and SingerAbbr like '" + py + "%'";
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
            }

            if (page == zong)
            {
                this.lblxiayiye.Enabled = false;
            }
            else
            {
                this.lblxiayiye.Enabled = true;
            }
        }
        //歌手上一页
        private void lblshangyiye_Click(object sender, EventArgs e)
        {

        }
        //歌手下一页
        private void lblxiayiye_Click(object sender, EventArgs e)
        {

        }
        //变色
        public void SetColor(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            for (int i = 0; i < c.Parent.Controls.Count; i++)
            {
                c.Parent.Controls[i].ForeColor = Color.Black;
            }
            c.ForeColor = Color.Red;
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
            pageNow = 1;
            getZong();//获取总页数
            showSinger();
            jinyong();
            SetColor(sender, e);
        }
        //显示歌曲
        public void showSong()
        {
            string py = this.textBox1.Text;
            string tiaojian = "";
            try
            {
                DBHelper.OpenConnection();
                if (!py.Trim().Equals(""))
                {
                    tiaojian += " and m.MusicAbbr like '%" + py + "%'";
                }
                string sql = "select top " + pageSize1 + "     MusicID,MusicName,s.SingerName, '点歌' as dg, MusicID from Music as m,Singer as s where m.SingerID=s.SingerID and s.MusicID not in(select top " + (pageSize1 * (pageNow1 - 1)) + " MusicID from Music where MusicDisplay=True and SingerID=" + singerId + " " + tiaojian + ") and m.MusicDisplay=True and s.SingerID=" + singerId + " " + tiaojian + " ";
                SqlDataReader reader = DBHelper.GetExecuteReader(sql);
                for (int i = 0; i < this.panel10.Controls.Count; i++)
                {
                    if (reader.Read())
                    {
                        this.panel10.Controls[i].Controls[2].Text = reader["SongName"].ToString();
                        this.panel10.Controls[i].Controls[1].Text = reader["SingerName"].ToString();
                        this.panel10.Controls[i].Controls[0].Text = reader["dg"].ToString();
                        this.panel10.Controls[i].Controls[0].Tag = reader["SongId"].ToString();
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
            }

            if (page == pageZong1)
            {
                this.lblxyy.Enabled = false;
            }
            else
            {
                this.lblxyy.Enabled = true;
            }
        }

        //获取总页数歌曲
        public void getSongZong()
        {
            string py = this.textBox1.Text.ToString();
            try
            {
                DBHelper.OpenConnection();
                string tiaojian = "";
                if (py != string.Empty)
                {
                    tiaojian += " and Songnamesuoxie like '" + py + "'";
                }
                string sql = "select count(*) from Song  where deletezhuangtai=0 and SingerId =" + singerId;
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

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            getZong();
            showSinger();
        }
    }
}
