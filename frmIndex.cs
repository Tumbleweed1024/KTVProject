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
    public partial class frmIndex : Form
    {
        //窗体构造函数
        public frmIndex()
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
        private frmSinger fs;
        public List<Music> musics = new List<Music>();//存放已点的曲目
        //控制静音
        private bool mute = false;
        //控制暂停
        private bool paused = false;
        //控制伴奏
        private bool accompany = false;
        //存储音量
        private int volume = 50;
        //控制大屏
        private bool fullScreen = false;
        //控制首页按钮是否生效
        private bool index = true;
        //分钟计时器
        private int minute = 0;
        //已点
        int yidianpage = 1;
        int rows = 4;
        int zong = 0;
        //主窗体加载
        private void frmIndex_Load(object sender, EventArgs e)
        {
            #region 隐藏主页面所有边框
            this.panelTop.BorderStyle = BorderStyle.None;
            this.panelLogo.BorderStyle = BorderStyle.None;
            this.panelScroll.BorderStyle = BorderStyle.None;
            this.panelPlayList.BorderStyle = BorderStyle.None;
            this.panelCountdown.BorderStyle = BorderStyle.None;
            this.panelFill.BorderStyle = BorderStyle.None;
            this.panelCenter.BorderStyle = BorderStyle.None;
            this.panelIndex.BorderStyle = BorderStyle.None;
            this.panelSinger.BorderStyle = BorderStyle.None;
            this.panelTopList.BorderStyle = BorderStyle.None;
            this.panelMV.BorderStyle = BorderStyle.None;
            this.panelMusicType.BorderStyle = BorderStyle.None;
            this.panelItem.BorderStyle = BorderStyle.None;
            this.panelBottom.BorderStyle = BorderStyle.None;
            this.panelMute.BorderStyle = BorderStyle.None;
            this.panelVolumeDown.BorderStyle = BorderStyle.None;
            this.panelVolumeUp.BorderStyle = BorderStyle.None;
            this.panelSearch.BorderStyle = BorderStyle.None;
            this.panelPaused.BorderStyle = BorderStyle.None;
            this.panelReplay.BorderStyle = BorderStyle.None;
            this.panelAccompany.BorderStyle = BorderStyle.None;
            this.panelFullScreen.BorderStyle = BorderStyle.None;
            this.panelNextMusic.BorderStyle = BorderStyle.None;
            this.panelToIndex.BorderStyle = BorderStyle.None;
            #endregion
            #region 布置默认图片，图片在Properties.Resoutces
            this.BackgroundImage = Properties.Resources.indexBg;
            this.panelCenter.BackgroundImage = Properties.Resources.半透明白色背景;
            this.panelToIndex.BackgroundImage = Properties.Resources.首页;
            this.panelMute.BackgroundImage = Properties.Resources.静音;
            this.panelVolumeDown.BackgroundImage = Properties.Resources.音量减;
            this.panelVolumeUp.BackgroundImage = Properties.Resources.音量加;
            this.panelSearch.BackgroundImage = Properties.Resources.点歌;
            this.panelPaused.BackgroundImage = Properties.Resources.暂停;
            this.panelReplay.BackgroundImage = Properties.Resources.重唱;
            this.panelAccompany.BackgroundImage = Properties.Resources.伴奏;
            this.panelFullScreen.BackgroundImage = Properties.Resources.大屏;
            this.panelNextMusic.BackgroundImage = Properties.Resources.切歌;
            this.panelSinger.BackgroundImage = Properties.Resources.歌手;
            this.panelTopList.BackgroundImage = Properties.Resources.榜单;
            this.panelMusicType.BackgroundImage = Properties.Resources.歌曲分类;
            this.panelItem.BackgroundImage = Properties.Resources.酒水零食;
            this.panelLogo.BackgroundImage = Properties.Resources.LOGOtext;
            this.panelPlayList.BackgroundImage = Properties.Resources.已点;
            this.panelCountdown.BackgroundImage = Properties.Resources.剩余时间;
            #endregion
            //同步音量
            this.awmp_mv.settings.volume = volume;
            this.awmp_bz.settings.volume = volume;
            //取消MV窗口UI
            this.awmp_mv.uiMode = "none";
            //禁止MV右键菜单
            this.awmp_mv.enableContextMenu = false;
            this.awmp_mv.settings.volume = volume;
            //静音伴奏
            this.awmp_bz.settings.mute = true;
            //禁止自动播放
            this.awmp_mv.settings.autoStart = false;
            this.awmp_bz.settings.autoStart = false;
            //同步时间
            if (!Convert.ToString(DateTime.Now.Hour + ":" + DateTime.Now.Minute).Equals(this.labelTime.Text))
            {
                if (Convert.ToString(DateTime.Now.Hour).Length == 1)
                {
                    if (Convert.ToString(DateTime.Now.Minute).Length == 1)
                    {
                        if (Convert.ToString(DateTime.Now.Second).Length == 1)
                        {
                            this.labelTime.Text = Convert.ToString("0" + DateTime.Now.Hour + ":0" + DateTime.Now.Minute + ":0" + DateTime.Now.Second);
                        }
                        else
                        {
                            this.labelTime.Text = Convert.ToString("0" + DateTime.Now.Hour + ":0" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                        }
                    }
                    else
                    {
                        if (Convert.ToString(DateTime.Now.Second).Length == 1)
                        {
                            this.labelTime.Text = Convert.ToString("0" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":0" + DateTime.Now.Second);
                        }
                        else
                        {
                            this.labelTime.Text = Convert.ToString("0" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                        }
                    }
                }
                else
                {
                    if (Convert.ToString(DateTime.Now.Minute).Length == 1)
                    {
                        if (Convert.ToString(DateTime.Now.Second).Length == 1)
                        {
                            this.labelTime.Text = Convert.ToString(DateTime.Now.Hour + ":0" + DateTime.Now.Minute + ":0" + DateTime.Now.Second);
                        }
                        else
                        {
                            this.labelTime.Text = Convert.ToString(DateTime.Now.Hour + ":0" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                        }
                    }
                    else
                    {
                        if (Convert.ToString(DateTime.Now.Second).Length == 1)
                        {
                            this.labelTime.Text = Convert.ToString(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":0" + DateTime.Now.Second);
                        }
                        else
                        {
                            this.labelTime.Text = Convert.ToString(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                        }
                    }
                }
            }
            //归位滚动条
            this.panelScrollText1.Left = this.panelScroll.Width;
            this.panelScrollText2.Left = this.panelScroll.Width;
            //播放
            play();
        }
        #region 连接数据库
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connstring = "Data Source=.;Initial Catalog=KTVProject;User ID=sa;Password=123456";
        #endregion

        #region 鼠标动作

        #region 静音鼠标动作
        private void panelMute_MouseEnter(object sender, EventArgs e)
        {
            if (mute)
            {
                this.panelMute.BackgroundImage = Properties.Resources.声音hover;
            }
            else
            {
                this.panelMute.BackgroundImage = Properties.Resources.静音hover;
            }
        }
        private void panelMute_MouseLeave(object sender, EventArgs e)
        {
            if (mute)
            {
                this.panelMute.BackgroundImage = Properties.Resources.声音;
            }
            else
            {
                this.panelMute.BackgroundImage = Properties.Resources.静音;
            }
        }

        private void panelMute_MouseDown(object sender, MouseEventArgs e)
        {
            if (mute)
            {
                this.panelMute.BackgroundImage = Properties.Resources.声音down;
            }
            else
            {
                this.panelMute.BackgroundImage = Properties.Resources.静音down;
            }
        }

        private void panelMute_MouseUp(object sender, MouseEventArgs e)
        {
            if (mute)
            {
                //判断是否为伴奏，是则不取消MV静音
                if (accompany)
                {
                    //取消静音伴奏
                    this.awmp_bz.settings.mute = false;
                    //同步伴奏音量
                    this.awmp_bz.settings.volume = volume;
                }
                else
                {
                    //取消静音MV
                    this.awmp_mv.settings.mute = false;
                    //同步MV音量
                    this.awmp_mv.settings.volume = volume;
                }

                this.panelMute.BackgroundImage = Properties.Resources.静音hover;
            }
            else
            {
                //判断是否为伴奏进行对应静音
                if (accompany)
                {
                    //静音伴奏
                    this.awmp_bz.settings.mute = true;
                }
                else
                {
                    //静音MV
                    this.awmp_mv.settings.mute = true;
                }

                this.panelMute.BackgroundImage = Properties.Resources.声音hover;
            }
            mute = !mute;
        }
        #endregion

        #region 音量减鼠标动作
        private void panelVolumeDown_MouseEnter(object sender, EventArgs e)
        {
            this.panelVolumeDown.BackgroundImage = Properties.Resources.音量减hover;
        }

        private void panelVolumeDown_MouseLeave(object sender, EventArgs e)
        {
            this.panelVolumeDown.BackgroundImage = Properties.Resources.音量减;
        }

        private void panelVolumeDown_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelVolumeDown.BackgroundImage = Properties.Resources.音量减down;
        }

        private void panelVolumeDown_MouseUp(object sender, MouseEventArgs e)
        {
            //更改音量
            if (volume>=5)
            {
                volume -= 5;
                volume -= 5;
            }
            //按照静音和伴奏状态同步相应音量
            if (!(mute || accompany))
            {
                this.awmp_mv.settings.volume = volume;
            }
            else if(accompany&&!mute){
                this.awmp_bz.settings.volume = volume;
            }
            //显示音量，计时器开启
            this.labelVolumnValue.Text = Convert.ToString(volume);
            this.panelVolume.Visible = true;
            //置顶音量
            this.panelVolume.BringToFront();
            //如果计时器已经启动，则先停止
            if (this.timerVolume.Enabled)
            {
                this.timerVolume.Stop();
                this.timerVolume.Start();
            }
            else
            {
                this.timerVolume.Start();
            }

            this.panelVolumeDown.BackgroundImage = Properties.Resources.音量减hover;
        }
        #endregion

        #region 音量加鼠标动作
        private void panelVolumeUp_MouseEnter(object sender, EventArgs e)
        {
            this.panelVolumeUp.BackgroundImage = Properties.Resources.音量加hover;
        }

        private void panelVolumeUp_MouseLeave(object sender, EventArgs e)
        {
            this.panelVolumeUp.BackgroundImage = Properties.Resources.音量加;
        }

        private void panelVolumeUp_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelVolumeUp.BackgroundImage = Properties.Resources.音量加down;
        }

        private void panelVolumeUp_MouseUp(object sender, MouseEventArgs e)
        {
            //更改音量
            if (volume <= 95)
            {
                volume += 5;
                volume += 5;
            }
            //按照静音和伴奏状态同步相应音量
            if (!(mute || accompany))
            {
                this.awmp_mv.settings.volume = volume;
            }
            else if (accompany && !mute)
            {
                this.awmp_bz.settings.volume = volume;
            }
            //显示音量，计时器开启
            this.labelVolumnValue.Text = Convert.ToString(volume);
            this.panelVolume.Visible = true;
            //置顶音量
            this.panelVolume.BringToFront();
            //如果计时器已经启动，则先停止
            if (this.timerVolume.Enabled)
            {
                this.timerVolume.Stop();
                this.timerVolume.Start();
            }
            else
            {
                this.timerVolume.Start();
            }

            this.panelVolumeUp.BackgroundImage = Properties.Resources.音量加hover;
        }
        #endregion

        #region 点歌鼠标动作
        private void panelSearch_MouseEnter(object sender, EventArgs e)
        {
            this.panelSearch.BackgroundImage = Properties.Resources.点歌hover;
        }

        private void panelSearch_MouseLeave(object sender, EventArgs e)
        {
            this.panelSearch.BackgroundImage = Properties.Resources.点歌;
        }

        private void panelSearch_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelSearch.BackgroundImage = Properties.Resources.点歌down;
        }

        private void panelSearch_MouseUp(object sender, MouseEventArgs e)
        {
            this.panelSearch.BackgroundImage = Properties.Resources.点歌hover;
        }
        #endregion

        #region 暂停鼠标动作
        private void panelPaused_MouseEnter(object sender, EventArgs e)
        {
            if (paused)
            {
                this.panelPaused.BackgroundImage = Properties.Resources.播放hover;
            }
            else
            {
                this.panelPaused.BackgroundImage = Properties.Resources.暂停hover;
            }
        }

        private void panelPaused_MouseLeave(object sender, EventArgs e)
        {
            if (paused)
            {
                this.panelPaused.BackgroundImage = Properties.Resources.播放;
            }
            else
            {
                this.panelPaused.BackgroundImage = Properties.Resources.暂停;
            }
        }

        private void panelPaused_MouseDown(object sender, MouseEventArgs e)
        {
            if (paused)
            {
                this.panelPaused.BackgroundImage = Properties.Resources.播放down;
            }
            else
            {
                this.panelPaused.BackgroundImage = Properties.Resources.暂停down;
            }
        }

        private void panelPaused_MouseUp(object sender, MouseEventArgs e)
        {
            if (paused)
            {
                //播放
                this.awmp_mv.Ctlcontrols.play();
                this.awmp_bz.Ctlcontrols.play();
                this.panelPaused.BackgroundImage = Properties.Resources.暂停hover;
            }
            else
            {
                //暂停
                this.awmp_mv.Ctlcontrols.pause();
                this.awmp_bz.Ctlcontrols.pause();
                this.panelPaused.BackgroundImage = Properties.Resources.播放hover;
            }
            paused = !paused;
        }
        #endregion

        #region 重唱鼠标动作
        private void panelReplay_MouseEnter(object sender, EventArgs e)
        {
            this.panelReplay.BackgroundImage = Properties.Resources.重唱hover;
        }

        private void panelReplay_MouseLeave(object sender, EventArgs e)
        {
            this.panelReplay.BackgroundImage = Properties.Resources.重唱;
        }

        private void panelReplay_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelReplay.BackgroundImage = Properties.Resources.重唱down;
        }

        private void panelReplay_MouseUp(object sender, MouseEventArgs e)
        {
            //播放进度回退至0
            this.awmp_mv.Ctlcontrols.currentPosition = 0;
            this.awmp_bz.Ctlcontrols.currentPosition = 0;

            this.panelReplay.BackgroundImage = Properties.Resources.重唱hover;
        }
        #endregion

        #region 伴奏鼠标动作
        private void panelAccompany_MouseEnter(object sender, EventArgs e)
        {
            if (accompany)
            {
                this.panelAccompany.BackgroundImage = Properties.Resources.原唱hover;
            }
            else
            {
                this.panelAccompany.BackgroundImage = Properties.Resources.伴奏hover;
            }
        }

        private void panelAccompany_MouseLeave(object sender, EventArgs e)
        {
            if (accompany)
            {
                this.panelAccompany.BackgroundImage = Properties.Resources.原唱;
            }
            else
            {
                this.panelAccompany.BackgroundImage = Properties.Resources.伴奏;
            }
        }

        private void panelAccompany_MouseDown(object sender, MouseEventArgs e)
        {
            if (accompany)
            {
                this.panelAccompany.BackgroundImage = Properties.Resources.原唱down;
            }
            else
            {
                this.panelAccompany.BackgroundImage = Properties.Resources.伴奏down;
            }
        }

        private void panelAccompany_MouseUp(object sender, MouseEventArgs e)
        {
            if (accompany)
            {
                //判断是否静音，静音不恢复
                if (!mute)
                {
                    //静音伴奏
                    this.awmp_bz.settings.mute = true;
                    //恢复原唱
                    this.awmp_mv.settings.mute = false;
                    //同步原唱音量
                    this.awmp_mv.settings.volume = volume;
                }

                this.panelAccompany.BackgroundImage = Properties.Resources.伴奏hover;
            }
            else
            {
                //判断是否静音，静音不修改
                if (!mute)
                {
                    //静音原唱
                    this.awmp_mv.settings.mute = true;
                    //恢复伴奏
                    this.awmp_bz.settings.mute = true;
                    //同步伴奏音量
                    this.awmp_bz.settings.volume = volume;
                }
                this.panelAccompany.BackgroundImage = Properties.Resources.原唱hover;
            }
            accompany = !accompany;
        }
        #endregion

        #region 大屏鼠标动作
        private void panelFullScreen_MouseEnter(object sender, EventArgs e)
        {
            if (fullScreen)
            {
                this.panelFullScreen.BackgroundImage = Properties.Resources.小屏hover;
            }
            else
            {
                this.panelFullScreen.BackgroundImage = Properties.Resources.大屏hover;
            }
        }

        private void panelFullScreen_MouseLeave(object sender, EventArgs e)
        {
            if (fullScreen)
            {
                this.panelFullScreen.BackgroundImage = Properties.Resources.小屏;
            }
            else
            {
                this.panelFullScreen.BackgroundImage = Properties.Resources.大屏;
            }
        }

        private void panelFullScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (fullScreen)
            {
                this.panelFullScreen.BackgroundImage = Properties.Resources.小屏down;
            }
            else
            {
                this.panelFullScreen.BackgroundImage = Properties.Resources.大屏down;
            }
        }

        private void panelFullScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (fullScreen)
            {
                this.panelMV.Dock = DockStyle.None;
                this.panelFullScreen.BackgroundImage = Properties.Resources.大屏hover;
            }
            else
            {
                this.panelMV.Dock = DockStyle.Fill;
                this.panelFullScreen.BackgroundImage = Properties.Resources.小屏hover;
            }
            if (!index)
            {
                fs.mvDock(!fullScreen);
            }
            fullScreen = !fullScreen;
        }
        #endregion

        #region 切歌鼠标动作
        private void panelNextMusic_MouseEnter(object sender, EventArgs e)
        {
            this.panelNextMusic.BackgroundImage = Properties.Resources.切歌hover;
        }

        private void panelNextMusic_MouseLeave(object sender, EventArgs e)
        {
            this.panelNextMusic.BackgroundImage = Properties.Resources.切歌;
        }

        private void panelNextMusic_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelNextMusic.BackgroundImage = Properties.Resources.切歌down;
        }

        private void panelNextMusic_MouseUp(object sender, MouseEventArgs e)
        {
            this.awmp_mv.Ctlcontrols.next();
            this.panelNextMusic.BackgroundImage = Properties.Resources.切歌hover;
        }
        #endregion

        #region 首页鼠标动作
        private void panelToIndex_MouseEnter(object sender, EventArgs e)
        {
            this.panelToIndex.BackgroundImage = Properties.Resources.首页hover;
        }

        private void panelToIndex_MouseLeave(object sender, EventArgs e)
        {
            this.panelToIndex.BackgroundImage = Properties.Resources.首页;
        }

        private void panelToIndex_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelToIndex.BackgroundImage = Properties.Resources.首页down;
        }

        private void panelToIndex_MouseUp(object sender, MouseEventArgs e)
        {
            this.panelToIndex.BackgroundImage = Properties.Resources.首页hover;
            if (!index)
            {
                toIndex();
            }
        }
        #endregion

        #region 歌手鼠标动作
        private void panelSinger_MouseEnter(object sender, EventArgs e)
        {
            this.panelSinger.BackgroundImage = Properties.Resources.歌手hover;
        }

        private void panelSinger_MouseLeave(object sender, EventArgs e)
        {
            this.panelSinger.BackgroundImage = Properties.Resources.歌手;
        }

        private void panelSinger_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelSinger.BackgroundImage = Properties.Resources.歌手down;
        }

        private void panelSinger_MouseUp(object sender, MouseEventArgs e)
        {
            this.panelSinger.BackgroundImage = Properties.Resources.歌手hover;
            singer();
        }
        #endregion

        #region 榜单鼠标动作
        private void panelTopList_MouseEnter(object sender, EventArgs e)
        {
            this.panelTopList.BackgroundImage = Properties.Resources.榜单hover;
        }

        private void panelTopList_MouseLeave(object sender, EventArgs e)
        {
            this.panelTopList.BackgroundImage = Properties.Resources.榜单;
        }

        private void panelTopList_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelTopList.BackgroundImage = Properties.Resources.榜单down;
        }

        private void panelTopList_MouseUp(object sender, MouseEventArgs e)
        {
            this.panelTopList.BackgroundImage = Properties.Resources.榜单hover;
        }
        #endregion

        #region 歌曲分类鼠标动作
        private void panelMusicType_MouseEnter(object sender, EventArgs e)
        {
            this.panelMusicType.BackgroundImage = Properties.Resources.歌曲分类hover;
        }

        private void panelMusicType_MouseLeave(object sender, EventArgs e)
        {
            this.panelMusicType.BackgroundImage = Properties.Resources.歌曲分类;
        }

        private void panelMusicType_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelMusicType.BackgroundImage = Properties.Resources.歌曲分类down;
        }

        private void panelMusicType_MouseUp(object sender, MouseEventArgs e)
        {
            this.panelMusicType.BackgroundImage = Properties.Resources.歌曲分类hover;
        }
        #endregion

        #region 酒水零食鼠标动作
        private void panelItem_MouseEnter(object sender, EventArgs e)
        {
            this.panelItem.BackgroundImage = Properties.Resources.酒水零食hover;
        }

        private void panelItem_MouseLeave(object sender, EventArgs e)
        {
            this.panelItem.BackgroundImage = Properties.Resources.酒水零食;
        }

        private void panelItem_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelItem.BackgroundImage = Properties.Resources.酒水零食down;
        }

        private void panelItem_MouseUp(object sender, MouseEventArgs e)
        {
            this.panelItem.BackgroundImage = Properties.Resources.酒水零食hover;
        }
        #endregion

        #region LOGO鼠标动作
        private void panelLogo_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void panelLogo_MouseEnter(object sender, EventArgs e)
        {
            this.panelLogo.BackgroundImage = Properties.Resources.LOGOtextY;
        }
        private void panelLogo_MouseLeave(object sender, EventArgs e)
        {
            this.panelLogo.BackgroundImage = Properties.Resources.LOGOtext;
        }
        #endregion

        #region 已点歌曲鼠标动作
        private void panelPlayList_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.pn_yidian.Visible == false)
            {
                this.pn_yidian.Visible = true;
            }
            else
            {
                this.pn_yidian.Visible = false;
            }
            getZongRow();
            showSong();
        }
        private void panelPlayList_MouseEnter(object sender, EventArgs e)
        {
            this.panelPlayList.BackgroundImage = Properties.Resources.已点Y;
            this.labelSongCount.ForeColor = Color.Yellow;
        }
        private void panelPlayList_MouseLeave(object sender, EventArgs e)
        {
            this.panelPlayList.BackgroundImage = Properties.Resources.已点;
            this.labelSongCount.ForeColor = Color.White;
        }

        private void labelSongCount_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.pn_yidian.Visible == false)
            {
                this.pn_yidian.Visible = true;
            }
            else
            {
                this.pn_yidian.Visible = false;
            }
            getZongRow();
            showSong();
        }

        private void labelSongCount_MouseEnter(object sender, EventArgs e)
        {
            this.panelPlayList.BackgroundImage = Properties.Resources.已点Y;
            this.labelSongCount.ForeColor = Color.Yellow;
        }

        private void labelSongCount_MouseLeave(object sender, EventArgs e)
        {
            this.panelPlayList.BackgroundImage = Properties.Resources.已点;
            this.labelSongCount.ForeColor = Color.White;
        }
        #endregion

        #endregion
        //播放歌曲
        public void play()
        {
            //无歌曲默认放歌
                if (musics.Count > 0)
                {
                    this.awmp_mv.URL = musics[0].MusicMVPath;
                    this.awmp_bz.URL = musics[0].MusicAccompanyPath;
                }
                else
                {
                    this.awmp_mv.URL = @"D:\KTVData\LoadPlay\砂之惑星MV_降音量.mp4";
                    this.awmp_bz.URL = @"D:\KTVData\LoadPlay\砂之惑星伴奏.mp3";
                    //this.awmp_mv.URL = @"D:\KTVData\LoadPlay\秘密人偶剧.mp4";
                    //this.awmp_bz.URL = @"D:\KTVData\LoadPlay\秘密人偶剧伴奏.wav";
            }
            //等待播放器准备就绪
            while (true)
            {
                if(this.awmp_mv.playState == WMPLib.WMPPlayState.wmppsReady && this.awmp_bz.playState == WMPLib.WMPPlayState.wmppsReady)
                {
                    this.awmp_mv.Ctlcontrols.play();
                    this.awmp_bz.Ctlcontrols.play();
                    break;
                }
            }
            
        }
        //延时三秒关闭音量显示
        private void timerVolume_Tick(object sender, EventArgs e)
        {
            panelVolume.Visible = false;
        }

        //每秒动作
        private void timer1s_Tick(object sender, EventArgs e)
        {
            //同步时间
            if (!Convert.ToString(DateTime.Now.Hour + ":" + DateTime.Now.Minute).Equals(this.labelTime.Text))
            {
                if(Convert.ToString(DateTime.Now.Hour).Length == 1)
                {
                    if (Convert.ToString(DateTime.Now.Minute).Length == 1)
                    {
                        if (Convert.ToString(DateTime.Now.Second).Length == 1)
                        {
                            this.labelTime.Text = Convert.ToString("0" + DateTime.Now.Hour + ":0" + DateTime.Now.Minute + ":0" + DateTime.Now.Second);
                        }
                        else
                        {
                            this.labelTime.Text = Convert.ToString("0" + DateTime.Now.Hour + ":0" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                        }
                    }
                    else
                    {
                        if (Convert.ToString(DateTime.Now.Second).Length == 1)
                        {
                            this.labelTime.Text = Convert.ToString("0" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":0" + DateTime.Now.Second);
                        }
                        else
                        {
                            this.labelTime.Text = Convert.ToString("0" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                        }
                    }
                }
                else
                {
                    if (Convert.ToString(DateTime.Now.Minute).Length == 1)
                    {
                        if (Convert.ToString(DateTime.Now.Second).Length == 1)
                        {
                            this.labelTime.Text = Convert.ToString(DateTime.Now.Hour + ":0" + DateTime.Now.Minute + ":0" + DateTime.Now.Second);
                        }
                        else
                        {
                            this.labelTime.Text = Convert.ToString(DateTime.Now.Hour + ":0" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                        }
                    }
                    else
                    {
                        if (Convert.ToString(DateTime.Now.Second).Length == 1)
                        {
                            this.labelTime.Text = Convert.ToString(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":0" + DateTime.Now.Second);
                        }
                        else
                        {
                            this.labelTime.Text = Convert.ToString(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                        }
                    }
                }
            }
            //连续播放
            if (this.awmp_mv.playState == WMPLib.WMPPlayState.wmppsStopped && this.awmp_bz.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                play();
            }
            //同步剩余时间
            if (!this.labelCountDown.Text.Equals("10"))
            {
                this.labelCountDown.Text = "10";
            }
            ////减时间
            //minute++;
            //if(minute >= 60)
            //{
            //    this.labelCountDown.Text = Convert.ToString(Convert.ToInt32(this.labelCountDown.Text) - 1);
            //}
            //余额不足十分钟标为红色
            if (this.labelCountDown.Text.Length < 2)
            {
                this.labelCountDown.ForeColor = Color.Red;
            }
            //检查是否还有余额

            //替换滚动条歌名
            if (musics.Count > 0)
            {
                if (this.lbl_dqbf.Text != musics[0].MusicName)
                {
                    this.lbl_dqbf.Text = musics[0].MusicName;
                }
            }
            else
            {
                if (this.lbl_dqbf.Text != "暂无歌曲！")
                {
                    this.lbl_dqbf.Text = "暂无歌曲！";
                }
            }
            if (musics.Count > 1)
            {
                if (this.lbl_xys.Text != musics[1].MusicName)
                {
                    this.lbl_xys.Text = musics[1].MusicName;
                }
            }
            else
            {
                if (this.lbl_xys.Text != "暂无歌曲！")
                {
                    this.lbl_xys.Text = "暂无歌曲！";
                }
            }
            //剩余X首
            if(this.labelSongCount.Text != Convert.ToString(musics.Count))
            {
                this.labelSongCount.Text = Convert.ToString(musics.Count);
            }
            if (this.labelSongCount.Left != 19 - ((this.labelSongCount.Width - 41) / 2))
            {
                this.labelSongCount.Left = 19 - ((this.labelSongCount.Width - 41) / 2);
            }
        }

        //返回首页
        private void toIndex()
        {
            //改变首页布尔值
            index = true;

            this.p_show.Controls.Clear();
            this.p_show.Visible = false;
            this.panelIndex.Visible = true;
            this.panelMV.Controls.Add(this.awmp_mv);
        }
        //歌手页
        private void singer()
        {
            //改变首页布尔值
            index = false;
            //第一步清空
            this.p_show.Controls.Clear();
            //创建点歌窗体对象
            fs = new frmSinger();
            //把当前窗体mv传给点歌窗体
            fs.TopLevel = true;
            fs.pn_mv.Controls.Add(this.awmp_mv);
            this.awmp_mv.Dock = DockStyle.Fill;
            fs.frmindex = this;
            fs.TopLevel = false;
            this.p_show.Controls.Add(fs);
            fs.Dock = DockStyle.Fill;
            fs.Show();
            this.p_show.Dock = DockStyle.Fill;
            this.panelIndex.Visible = false;
            this.p_show.Visible = true;
        }
        //滚动条专属计时器
        private void timerScorll_Tick(object sender, EventArgs e)
        {
            //滚动
            //如果1和2未到达左边界，则1滚动
            if(!(this.panelScrollText2.Left < 0 - this.panelScrollText2.Width) && !(this.panelScrollText1.Left < 0 - this.panelScrollText1.Width))
            {
                this.panelScrollText1.Left -= 10;
                //如果1到达左边界2未达到，则2滚动
            }else if(this.panelScrollText1.Left < 0 - this.panelScrollText1.Width && !(this.panelScrollText2.Left < 0 - this.panelScrollText2.Width))
            {
                this.panelScrollText2.Left -= 10;
                //如果都到了，挪到右边界
            }
            else if(this.panelScrollText1.Left < 0 - this.panelScrollText1.Width && this.panelScrollText2.Left < 0 - this.panelScrollText2.Width)
            {
                //归位滚动条
                this.panelScrollText1.Left = this.panelScroll.Width;
                this.panelScrollText2.Left = this.panelScroll.Width;
            }

            //if (this.panelScorllText1.Left < 0 - this.panelScorllText1.Width)
            //{

            //    this.panelScorllText1.Left = this.panelScorllText1.Width;
            //}
            //this.panelScorllText2.Left -= 9;
            //if (this.panelScorllText2.Left < 0 - this.panelScorllText2.Width)
            //{

            //    this.panelScorllText2.Left = this.panelScorllText2.Width;
            //}

            //改变panel长度
            if (this.panelScrollText1.Width != 220 + this.lbl_dqbf.Text.Length * 18)
            {
                this.panelScrollText1.Width = 220 + this.lbl_dqbf.Text.Length * 18;
            }
            if (this.panelScrollText2.Width != 220 + this.lbl_xys.Text.Length * 18)
            {
                this.panelScrollText2.Width = 220 +  this.lbl_xys.Text.Length * 18;
            }
        }
        //获取总页数
        public void getZongRow()
        {
            //获取集合总长度
            int count = musics.Count;
            if (count % rows == 0)
            {
                zong = count / rows;
            }
            else
            {
                zong = (count / rows) + 1;
            }
            this.lbl_dqy.Text = 1.ToString();
            this.lbl_zys.Text = zong.ToString();
        }
        //点歌  参数
        public void showSong()
        {
            for (int i = 0; i < this.panel10.Controls.Count; i++)
            {
                int xiabiao = (yidianpage - 1) * rows + i;
                if (xiabiao < musics.Count)
                {
                    this.panel10.Controls[i].Controls[1].Text = "置顶";
                    this.panel10.Controls[i].Controls[0].Text = "删除";
                    this.panel10.Controls[i].Controls[2].Text = musics[xiabiao].SingerName;
                    this.panel10.Controls[i].Controls[2].Text = musics[xiabiao].MusicName;
                    this.panel10.Controls[i].Controls[0].Tag = xiabiao.ToString();
                    this.panel10.Controls[i].Controls[1].Tag = xiabiao.ToString();
                }
                else
                {
                    this.panel10.Controls[i].Controls[2].Text = "";
                    this.panel10.Controls[i].Controls[1].Text = "";
                    this.panel10.Controls[i].Controls[0].Text = "";
                    this.panel10.Controls[i].Controls[2].Text = "";
                }
            }
        }
        //上下页禁用方法
        public void jinyong()
        {
            yidianpage = int.Parse(this.lbl_dqy.Text);
            zong = int.Parse(this.lbl_zys.Text);
            if (yidianpage == 1)
            {
                this.label18.Enabled = false;
            }
            else
            {
                this.label18.Enabled = true;
            }

            if (yidianpage == zong)
            {
                this.label22.Enabled = false;
            }
            else
            {
                this.label22.Enabled = true;
            }
        }
        //点歌
        public void diange(int musicID)
        {

            conn.Open();
            //根据传过来的歌曲编号查询歌曲信息，并且添加到存放已点列表的集合中
            string sql = "select m.MusicID,m.MusicName,s.SingerName,m.MusicAccompanyPath,m.MusicAccompanyPath from Music m,Singer s where MusicDisplay=True and m.SingerID=s.SingerID and m.MusicID=" + musicID;
            comm = new SqlCommand(sql, conn);
            reader = comm.ExecuteReader();
            if (reader.Read())
            {
                Music music = new Music();
                music.MusicID = (int)reader["MusicID"];
                music.MusicName = reader["MusicName"].ToString();
                music.MusicAccompanyPath = reader["MusicAccompanyPath"].ToString();
                music.MusicMVPath = reader["MusicMVPath"].ToString();
                music.SingerName = reader["SingerName"].ToString();
                musics.Add(music);//添加到集合
            }
            reader.Close();
            conn.Close();
            getydCount();//每次点歌完对已点数目进行更新

            if (musics.Count == 1)
            {
                play();
            }
        }
        //已点的数量变化
        public void getydCount()
        {
            this.labelSongCount.Text = musics.Count.ToString();
        }

        public string MusicID = "";
        //更改歌曲的播放次数
        public void countjia()
        {
            conn.Open();
            string count = "";
            //查询次数
            string sql = "select MusicViews from Music where MusicID='" + MusicID + "' ";
            comm = new SqlCommand(sql, conn);
            reader = comm.ExecuteReader();
            if (reader.Read())
            {
                count = reader["MusicViews"].ToString();
            }
            int count2 = (Convert.ToInt32(count)) + 1;//count 次数加1
            reader.Close();
            string sql2 = "update Music set MusicViews='" + count2 + "' where MusicID='" + MusicID + "' ";
            comm = new SqlCommand(sql2, conn);
            comm.ExecuteNonQuery();
            conn.Close();
        }
        //已点
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (this.pn_yidian.Visible == false)
            {
                this.pn_yidian.Visible = true;
            }
            else
            {
                this.pn_yidian.Visible = false;
            }
            getZongRow();
            showSong();
        }
        //下一页
        private void label22_Click(object sender, EventArgs e)
        {
            yidianpage++;
            showSong();
            this.lbl_dqy.Text = yidianpage.ToString();
            jinyong();
        }
        //上一页
        private void label18_Click(object sender, EventArgs e)
        {
            yidianpage--;
            showSong();
            this.lbl_dqy.Text = yidianpage.ToString();
            jinyong();
        }
        //删除
        private void label14_Click(object sender, EventArgs e)
        {
            string xiabiao = ((Label)sender).Tag.ToString();
            MessageBox.Show(xiabiao);
            musics.RemoveAt(Convert.ToInt32(xiabiao));

            //重新刷新
            getZongRow();//调用获取集合总长度方法
            showSong();
        }
        //置顶
        private void label15_Click(object sender, EventArgs e)
        {
            string xiabiao = ((Label)sender).Tag.ToString();
            Music music = musics[Convert.ToInt32(xiabiao)];
        }
    }
}
