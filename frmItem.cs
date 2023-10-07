using System;
using System.Collections;
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
    public partial class frmItem : Form
    {
        public frmIndex frmindex;
        public frmItem()
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
        int pageNow = 1;
        int pageSize = 6;
        int zong = 0;
        string leixing = "4";
        public int roomId = 0;
        //加载
        private void frmjiushui_Load(object sender, EventArgs e)
        {
            //MV置顶，防止无法全屏
            pn_mv.BringToFront();
            leixing = "4";
            pageNow = 1;
            getZong();//获取总页数
            showJiushui();
            jinyong();
            bangding();
            label6.ForeColor = Color.RoyalBlue;
        }
        public void getZong()
        {
            // string py = this.textBox1.Text;
            try
            {
                DBHelper.OpenConnection();
                string tiaojian = "";
                if (!leixing.Equals("4"))
                {
                    tiaojian += " and ItemTypeID= " + leixing;
                }
                string sql = "select count(*)  from Items j where 1=1" + tiaojian + "";

                int count = (int)DBHelper.GetExecuteScalar(sql);
                if (count % pageSize == 0)
                {
                    zong = count / pageSize;
                }
                else
                {
                    zong = (count / pageSize) + 1;
                }
                this.lbl_dqy.Text = 1.ToString();
                this.lblzong.Text = zong.ToString();
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

        //酒水分页显示
        public void showJiushui()
        {
            // string py = this.textBox1.Text.Trim();
            try
            {
                DBHelper.OpenConnection();
                string tiaojian = "";

                if (!leixing.Equals("4"))
                {
                    tiaojian += " and ItemTypeID= " + leixing;
                }
                string sql = "select top " + pageSize + " ItemID,ItemName,ItemImagePath,ItemPrices from Items where 1=1 and ItemID not in(select top " + (pageSize * (pageNow - 1)) + " ItemID from Items where 1=1" + tiaojian + ")" + tiaojian + "";

                SqlDataReader reader = DBHelper.GetExecuteReader(sql);
                for (int i = 0; i < this.panel3.Controls.Count; i++)
                {
                    if (reader.Read())
                    {
                        this.panel3.Controls[i].Controls[0].Text = "￥";
                        ((PictureBox)(this.panel3.Controls[i].Controls[3])).Image = Image.FromFile(reader["ItemImagePath"].ToString());
                        this.panel3.Controls[i].Controls[3].Tag = reader["ItemName"].ToString();
                        this.panel3.Controls[i].Controls[2].Text = reader["ItemName"].ToString();
                        this.panel3.Controls[i].Controls[1].Text = reader["ItemPrices"].ToString();
                        this.panel3.Controls[i].Visible = true;
                    }
                    else
                    {
                        this.panel3.Controls[i].Controls[0].Text = "";
                        this.panel3.Controls[i].Controls[1].Text = "";
                        this.panel3.Controls[i].Controls[2].Text = "";
                        ((PictureBox)(this.panel3.Controls[i].Controls[3])).Image = null;
                        this.panel3.Controls[i].Visible = false;
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("酒水分页" + ex.Message);
            }
            finally
            {
                DBHelper.CloseConnection();
            }
        }

        //全部
        private void label6_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showJiushui();
            jinyong();
            SetColor(sender, e);
        }
        //小吃
        private void label7_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showJiushui();
            jinyong();
            SetColor(sender, e);
        }
        //饮料
        private void label8_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showJiushui();
            jinyong();
            SetColor(sender, e);
        }
        //酒类
        private void label9_Click(object sender, EventArgs e)
        {
            leixing = ((Control)sender).Tag.ToString();
            pageNow = 1;
            getZong();//获取总页数
            showJiushui();
            jinyong();
            SetColor(sender, e);
        }
        public void jinyong()
        {
            int page = int.Parse(this.lbl_dqy.Text);
            int zong = int.Parse(this.lblzong.Text);
            if (page == 1)
            {
                this.label1.Enabled = false;
            }
            else
            {
                this.label1.Enabled = true;
            }

            if (page == zong)
            {
                this.label5.Enabled = false;
            }
            else
            {
                this.label5.Enabled = true;
            }

        }

        public void SetColor(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            for (int i = 0; i < c.Parent.Controls.Count; i++)
            {
                c.Parent.Controls[i].ForeColor = Color.Black;
            }
            c.ForeColor = Color.RoyalBlue;
        }
        //下一页
        private void label5_Click(object sender, EventArgs e)
        {
            pageNow++;
            showJiushui();
            this.lbl_dqy.Text = pageNow.ToString();
            jinyong();
        }
        //上一页
        private void label1_Click(object sender, EventArgs e)
        {
            pageNow--;
            showJiushui();
            this.lbl_dqy.Text = pageNow.ToString();
            jinyong();
        }
        //List<DingDan> js = new List<DingDan>();
        
        DingDan[] js = new DingDan[6];
        string JiushuiName;
        public int fangfa(string name)
        {
            int index = -1;
            for (int i = 0; i < js.Length; i++)
            {
                if (js[i] != null && js[i].JiushuiName.Equals(name))
                {
                    index = i;
                    break;
                }
            }
            //MessageBox.Show("index"+index);
            return index;
        }
        //下单
        public DingDan xiangdd()
        {
            DingDan jsdb = null;
            try
            {
                DBHelper.OpenConnection();
                string sql = "select ItemName,ItemPrices,ItemTypeID,ItemImagePath from Items where ItemName='" + JiushuiName + "'";
                SqlDataReader sdr = DBHelper.GetExecuteReader(sql);
                if (sdr.Read())
                {
                    jsdb = new DingDan();
                    jsdb.JiushuiName = sdr["ItemName"].ToString();
                    jsdb.Jiushuidianjia = Convert.ToDouble(sdr["ItemPrices"]);
                    jsdb.Jiushuitype = Convert.ToInt32(sdr["ItemTypeID"]);
                    jsdb.JiushuiImg = sdr["ItemImagePath"].ToString();
                    jsdb.JiushuiConut = 1;
                    if (jsdb.JiushuiConut == 0)
                    {
                        jsdb.JiushuiConut = 1;
                    }
                    jsdb.Jiushuizongjia = jsdb.Jiushuidianjia * jsdb.JiushuiConut;
                    return jsdb;
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
            finally
            {
                DBHelper.CloseConnection();
            }
            return jsdb;
        }
        //将订单往集合中添加
        public void fz(DingDan s)
        {
            for (int i = 0; i < js.Length; i++)
            {
                if (js[i] == null)
                {
                    js[i] = s;
                    break;
                }
                // MessageBox.Show(js[i].JiushuiName);
            }
            //js.Add(s);
        }
        public void bangding()
        {
            for (int i = 0; i < this.panel10.Controls.Count; i++)
            {
                if (js[i] != null)
                {

                    //this.panel10.Controls[i].Controls[1].Text = js[i].JiushuiName.ToString();
                    //this.panel10.Controls[i].Controls[1].Tag= js[i].JiushuiName.ToString();
                    //this.panel10.Controls[i].Controls[0].Text = js[i].Jiushuidianjia.ToString();
                    //this.panel10.Controls[i].Controls[0].Tag = js[i].JiushuiName.ToString();

                    this.panel10.Controls[i].Controls[1].Text = js[i].JiushuiName.ToString();
                    this.panel10.Controls[i].Controls[1].Tag = js[i].JiushuiName.ToString();
                    this.panel10.Controls[i].Controls[0].Tag = js[i].JiushuiName.ToString();
                    this.panel10.Controls[i].Controls[0].Text = js[i].Jiushuidianjia.ToString();
                    this.panel10.Controls[i].Controls[2].Tag = js[i].JiushuiName.ToString();
                    this.panel10.Controls[i].Controls[3].Tag = js[i].JiushuiName.ToString();
                    this.panel10.Controls[i].Controls[2].Text = js[i].JiushuiConut.ToString();
                    this.panel10.Controls[i].Visible = true;
                }
                else
                {
                    this.panel10.Controls[i].Controls[0].Text = string.Empty;
                    this.panel10.Controls[i].Visible = false;

                }
            }
            button1.Enabled = false;
            for (int i = 0; i < js.Length; i++)
            {
                if (js[i]!=null)
                {
                    button1.Enabled = true;
                }
            }
        }
        //获得总价
        double zongjiage = 0;
        public void zongjia()
        {
            zongjiage = 0;
            for (int i = 0; i < js.Length; i++)
            {
                if (js[i] != null)
                {
                    zongjiage += js[i].Jiushuizongjia;
                }
                else
                {
                    break;
                }
            }
            this.lblzongjia.Text = zongjiage.ToString();
            // MessageBox.Show(this.lblzongjia.Text);
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Tag != null)
            {
                bool isNull = false;
                for (int i = 0; i < this.panel10.Controls.Count; i++)
                {
                    if (js[i] == null)
                    {
                        isNull = true;
                    }
                }
                JiushuiName = ((PictureBox)sender).Tag.ToString();
                int liang = fangfa(JiushuiName);
                if (liang < 0)
                {
                    //检查还有没有空位
                    if (isNull)
                    {
                        DingDan jsdb = xiangdd();//点击图片进行下单
                        fz(jsdb);
                    }
                    else
                    {
                        MessageBox.Show("购物车已满，请先进行结算！");
                    }
                }
                else
                {
                    js[liang].JiushuiConut++;
                    js[liang].Jiushuizongjia = js[liang].Jiushuidianjia * js[liang].JiushuiConut;
                }
                
            }
            bangding();
            zongjia();
        }



        #region 酒水数量的加减
        //数量加
        private void label56_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox6.Text);
            this.textBox6.Text = (liang + 1).ToString();


        }
        //数量减
        private void label55_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox6.Text);
            if (liang > 0)
            {
                this.textBox6.Text = (liang - 1).ToString();
            }
        }

        private void label51_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox5.Text);
            this.textBox5.Text = (liang + 1).ToString();
        }

        private void label50_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox5.Text);
            if (liang > 0)
            {
                this.textBox5.Text = (liang - 1).ToString();
            }
        }

        private void label46_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox4.Text);
            this.textBox4.Text = (liang + 1).ToString();
        }

        private void label45_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox4.Text);
            if (liang > 0)
            {
                this.textBox4.Text = (liang - 1).ToString();
            }
        }

        private void label41_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox3.Text);
            this.textBox3.Text = (liang + 1).ToString();
        }

        private void label40_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox3.Text);
            if (liang > 0)
            {
                this.textBox3.Text = (liang - 1).ToString();
            }
        }

        private void label35_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox2.Text);
            this.textBox2.Text = (liang + 1).ToString();
        }

        private void label34_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox2.Text);
            if (liang > 0)
            {
                this.textBox2.Text = (liang - 1).ToString();
            }
        }

        private void label26_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox1.Text);
            if (liang > 0)
            {
                this.textBox1.Text = (liang - 1).ToString();
            }
        }
        private void label27_Click(object sender, EventArgs e)
        {
            int liang = Convert.ToInt32(this.textBox1.Text);
            this.textBox1.Text = (liang + 1).ToString();

        }
        #endregion

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            string name = ((Control)sender).Tag.ToString();
            //MessageBox.Show(name);
            for (int i = 0; i < js.Length; i++)
            {
                if (js[i] != null)
                {
                    if (js[i].JiushuiName.Equals(name))
                    {
                        js[i].JiushuiConut = Convert.ToInt32(((Control)sender).Text);
                        js[i].Jiushuizongjia = js[i].JiushuiConut * js[i].Jiushuidianjia;
                    }
                }
            }
            //如果减少到0就删除这一项并重排数组
            int jIndex = fangfa(name);
            if (js[jIndex].JiushuiConut==0)
            {
                for(int i = jIndex; i<js.Length-1; i++)
                {
                    js[i] = js[i + 1];
                }
                js[js.Length - 1] = null;
            }
            zongjia();
            bangding();
        }
        //下单结算
        private void button1_Click(object sender, EventArgs e)
        {
            changru();
        }
        public void changru()
        {
            try
            {
                DBHelper.OpenConnection();
                for (int i = 0; i < this.panel10.Controls.Count; i++)
                {
                    if (js[i] != null)
                    {
                        String shopTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string sql = "insert into ItemShopList values("+roomId+ ",'"+ js[i].JiushuiName + "'," + js[i].JiushuiConut + "," + js[i].Jiushuidianjia + "," + js[i].Jiushuizongjia + ",'未结算','"+ shopTime +"')";
                        DBHelper.GetExecuteNonQuery(sql);
                    }
                }
                MessageBox.Show("结算完成");
                for (int i = 0;i < this.panel10.Controls.Count; i++)
                {
                    js[i] = null;
                }
                lblzongjia.Text = "购物车没有商品";
                bangding();
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
            finally
            {
                DBHelper.CloseConnection();
            }
        }
    }
}
