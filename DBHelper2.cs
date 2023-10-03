using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace KTVProject
{
    public class DBHelper2
    {
        private static string connStr = "Data Source=.;Initial Catalog=KTVProject;User ID=sa;Password=123456";
        private static SqlConnection connection;

        public static SqlConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new SqlConnection(connStr);
                }
                return connection;
            }
        }

        //定义打开数据库的方法
        public static void OpenConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            else if (Connection.State == ConnectionState.Broken)
            {
                Connection.Close();
                Connection.Open();
            }
        }

        //定义关闭数据库的方法
        public static void CloseConnection()
        {
            if (Connection.State == ConnectionState.Open ||
                Connection.State == ConnectionState.Broken)
            {
                Connection.Close();
            }
        }
        //定义返回查询表结果集方法
        public static SqlDataReader GetExecuteReader(string sql)
        {
            SqlCommand sqlcommand = new SqlCommand(sql, Connection);
            SqlDataReader sdr = sqlcommand.ExecuteReader();
            return sdr;
        }
        //定义返回影响行数的方法
        public static int GetExecuteNonQuery(string sql)
        {
            SqlCommand sqlcommand = new SqlCommand(sql, Connection);
            int num = sqlcommand.ExecuteNonQuery();
            return num;
        }
        //返回聚合函数统计结果
        public static int GetExecuteScalar(string sql)
        {
            SqlCommand sc = new SqlCommand(sql, Connection);
            int num = Convert.ToInt32(sc.ExecuteScalar());
            return num;
        }
    }
}
