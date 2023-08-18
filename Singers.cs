using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTVProject
{
    public class Singers
    {
        private int singerId;

        public int SingerId
        {
            get { return singerId; }
            set { singerId = value; }
        }
        private string singerName;

        public string SingerName
        {
            get { return singerName; }
            set { singerName = value; }
        }
        private string singerImg;

        public string SingerImg
        {
            get { return singerImg; }
            set { singerImg = value; }
        }
        private string singerType;

        public string SingerType
        {
            get { return singerType; }
            set { singerType = value; }
        }
    }
}
