﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTVProject
{
   public class DingDan
    {
        private string jiushuiName;

        public string JiushuiName
        {
            get { return jiushuiName; }
            set { jiushuiName = value; }
        }
        private int jiushuiConut;

        public int JiushuiConut
        {
            get { return jiushuiConut; }
            set { jiushuiConut = value; }
        }

        private int jiushuidianjia;

        public int Jiushuidianjia
        {
            get { return jiushuidianjia; }
            set { jiushuidianjia = value; }
        }
        private int jiushuitype;

        public int Jiushuitype
        {
            get { return jiushuitype; }
            set { jiushuitype = value; }
        }
        private string jiushuiImg;

        public string JiushuiImg
        {
            get { return jiushuiImg; }
            set { jiushuiImg = value; }
        }
        private int jiushuizongjia;

        public int Jiushuizongjia
        {
            get { return jiushuizongjia; }
            set { jiushuizongjia = value; }
        }
    }
}