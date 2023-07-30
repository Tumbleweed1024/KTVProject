using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTVProject
{
    public class Music
    {
        private int musicID;

        public int MusicID
        {
            get { return musicID; }
            set { musicID = value; }
        }
        private string musicName;

        public string MusicName
        {
            get { return musicName; }
            set { musicName = value; }
        }
        private string singerName;//歌手姓名

        public string SingerName
        {
            get { return singerName; }
            set { singerName = value; }
        }

        private int singerID;

        public int SingerID
        {
            get { return singerID; }
            set { singerID = value; }
        }
        private int languageId;

        public int LanguageId
        {
            get { return languageId; }
            set { languageId = value; }
        }
        private int musicTypeID;

        public int MusicTypeID
        {
            get { return musicTypeID; }
            set { musicTypeID = value; }
        }
        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        private int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        private int songzhuangtai;

        public int Songzhuangtai
        {
            get { return songzhuangtai; }
            set { songzhuangtai = value; }
        }
        private string musicAccompanyPath;

        public string MusicAccompanyPath
        {
            get { return musicAccompanyPath; }
            set { musicAccompanyPath = value; }
        }
        private string musicAbbr;

        public string MusicAbbr
        {
            get { return musicAbbr; }
            set { musicAbbr = value; }
        }
        private string musicMVPath;

        public string MusicMVPath
        {
            get { return musicMVPath; }
            set { musicMVPath = value; }
        }
        private int songlength;

        public int Songlength
        {
            get { return songlength; }
            set { songlength = value; }
        }
        private int deletezhuangtai;

        public int Deletezhuangtai
        {
            get { return deletezhuangtai; }
            set { deletezhuangtai = value; }
        }

    }
}
