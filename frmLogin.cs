using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTVProject
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            //缓存机制，防止加载闪烁
            this.DoubleBuffered = true;//设置本窗体
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            textUpdate();
            reloadTime();
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

        int roomStatus;
        int roomId;

        //获取主机名
        string hostName = SystemInformation.ComputerName;
        //更新主机名和房间号
        public void textUpdate()
        {
            label2.Text = "主机名称："+hostName;
            //主机名查房间号
            reload();
        }
        //修改房间号
        public void updateRoomID(int roomID)
        {
            DBHelper.OpenConnection();
            string sql = "UPDATE Room SET HostName = '" + hostName + "' WHERE  RoomID = " + roomID;
            DBHelper.GetExecuteNonQuery(sql);
            DBHelper.CloseConnection();
        }
        //刷新一下
        public void reload()
        {
            DBHelper.OpenConnection();
            string sql = "SELECT RoomID FROM Room WHERE HostName = '" + hostName + "'";
            roomId = DBHelper.GetExecuteScalar(sql);
            if (roomId != 0)
            {
                label1.Text = "包厢编号：" + roomId.ToString();
                panel2.Visible = true;
                label2.Visible = false;
                panel1.Visible = false;
            }
            else
            {
                label1.Text = "包厢编号：没有找到该设备的数据";
                panel2.Visible = false;
                label2.Visible = true;
                panel1.Visible = true;
            }
            string sql2 = "SELECT RoomStatus FROM Room WHERE RoomID = " + roomId + "";
            roomStatus = DBHelper.GetExecuteScalar(sql2);
            if (roomStatus == 0)
            {
                label4.Text = "包厢已关闭";
                label4.ForeColor = Color.Red;
                button2.Visible = false;
                reloadTime();
            }
            else if (roomStatus == 1)
            {
                label4.Text = "包厢已预约";
                label4.ForeColor = Color.LimeGreen;
                button2.Visible = true;
                reloadTime();
            }
            else if (roomStatus == 2)
            {
                label4.Text = "包厢已开启";
                label4.ForeColor = Color.DeepSkyBlue;
                button2.Visible = true;
                reloadTime();
            }
            DBHelper.CloseConnection();
        }
        //确定房间是否过期
        public bool reloadTime()
        {
            bool end = false;
            DBHelper.OpenConnection();
            string sql = "SELECT RoomCloseTime,RoomStatus FROM Room WHERE RoomID = " + roomId + "";
            SqlDataReader reader = DBHelper.GetExecuteReader(sql);
            if(reader.Read())
            {
                string readRoomCloseTime = reader["RoomCloseTime"].ToString();
                int readRoomStatus = Convert.ToInt32(reader["RoomStatus"]);
                reader.Close();
                if (!readRoomCloseTime.Equals(string.Empty))
                {
                    DateTime RoomCloseTime = DateTime.Parse(readRoomCloseTime);
                    if (DateTime.Now >= RoomCloseTime||readRoomStatus == 0)
                    {
                        //过期就初始化房间状态和时间数据
                        string sql2 = "UPDATE Room SET RoomStatus = 0 WHERE RoomID =" + roomId;
                        DBHelper.GetExecuteNonQuery(sql2);
                        string sql3 = "UPDATE Room SET RoomCloseTime = NULL WHERE RoomID =" + roomId;
                        DBHelper.GetExecuteNonQuery(sql3);
                        string sql4 = "UPDATE Room SET RoomTimeMinutes = NULL WHERE RoomID =" + roomId;
                        DBHelper.GetExecuteNonQuery(sql4);
                    }
                    else
                    {
                        end = true;
                    }
                }
                else { end = true; }
            }
            DBHelper.CloseConnection();
            return end;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1 == null)
            {
                MessageBox.Show("未选择房间号");
            }
            else
            {
                updateRoomID(Convert.ToInt32(comboBox1.Text));
                reload();
            }
        }
        //进入房间按钮
        private void button2_Click(object sender, EventArgs e)
        {
            //进入前验一下有没有过期
            if (reloadTime())
            {
                frmIndex fi = new frmIndex();
                //如果是预约状态就设置房间结束时间,然后把房间状态改成打开
                if (roomStatus == 1)
                {
                    DBHelper.OpenConnection();
                    string sql = "SELECT RoomID FROM Room WHERE HostName = '" + hostName + "'";
                    roomId = DBHelper.GetExecuteScalar(sql);
                    string sql2 = "SELECT RoomTimeMinutes FROM Room WHERE RoomID = " + roomId;
                    TimeSpan roomTimeMinutes = TimeSpan.FromMinutes(DBHelper.GetExecuteScalar(sql2));
                    DateTime nowTime = DateTime.Now;
                    DateTime endTime = nowTime + roomTimeMinutes;
                    string endTimeString = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                    string sql3 = "UPDATE Room SET RoomCloseTime = '" + endTimeString + "' WHERE RoomID =" + roomId;
                    DBHelper.GetExecuteNonQuery(sql3);
                    //改打开
                    string sql4 = "UPDATE Room SET RoomStatus = 2 WHERE RoomID =" + roomId;
                    DBHelper.GetExecuteNonQuery(sql4);
                    DBHelper.CloseConnection();
                    fi.Show();
                }
                else if (roomStatus == 2)
                {
                    fi.Show();
                }
            }
        }
        //每秒刷新和检测房间状态
        private void timer1_Tick(object sender, EventArgs e)
        {
            reload();
        }

    }
}
