using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDataAdapter
{
    class Program
    {
        private static string connSql = ConfigurationManager.ConnectionStrings["connSql"].ConnectionString;
        static void Main(string[] args)
        {
            //InsertCommand
            //DeleteCommand
            //IpdateCommand
            //SelectCommand
            {
                string sql = "select * from student;select * from teacher;";
                SqlConnection conn = new SqlConnection(connSql);
                //1.SelectCommand创建
                //SqlDataAdapter sdad = new SqlDataAdapter();

                //sdad.SelectCommand = new SqlCommand(sql, conn);
                ////2.
                //SqlCommand scmd = new SqlCommand(sql, conn);
                //SqlDataAdapter adapter = new SqlDataAdapter(scmd);
                ////3.sql语句和连接对象
                //SqlDataAdapter adapter1 = new SqlDataAdapter(sql, conn);
                ////4.sql语句和connSql连接字符串构建adpater对象
                //SqlDataAdapter adapter2 = new SqlDataAdapter(sql, connSql);

                /*如果是T-sql 查询语句，优先使用第3种方式，如果带参数，添加参数建议使用第2种或第1种方式*/
                //此处conn没有open()，也可以进行
                //SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                //DataSet ds = new DataSet();
                //da.TableMappings.Add("Table", "student");//表面映射名称
                //da.TableMappings.Add("Table1", "teacher");
                //da.Fill(ds);
            }
            {
                //填充数据针对单个结果集
                string sql0 = "select * from student;";
                SqlConnection conn = new SqlConnection(connSql);
                
                SqlDataAdapter da = new SqlDataAdapter(sql0, conn);
                DataTable dt = new DataTable("stu");
                conn.Open();
                da.Fill(dt);
                conn.Close();//断开式的机制：Fill的前后conn的状态都是Closed，只有在执行的过程中才会Open(),这是由DataAdpater自己来执行
                //连接式的机制：需要在da.Fill(dt)之前添加conn.Open()，不管Fill的状态如何都是Open,所以需要之后添加conn.Closed();优先使用显示连接式的机制
            }

            {
                //填充数据针对单个结果集
                string sql0 = "select * from student;";
                SqlConnection conn = new SqlConnection(connSql);

                SqlDataAdapter da = new SqlDataAdapter(sql0, conn);
                DataTable dt = new DataTable("stu");
                da.Fill(dt);
                dt.Rows[1]["age"] = "14";
                DataRow dr = dt.NewRow();
                dr["code"] = "1000004";
                dr["name"] = "胡喜文";
                dr["sex"] = 0;
                dr["address"] = "长沙望城金星北";
                dr["age"] = 35;
                dr["interesting"] = "test1";
                dr["classteacher"] = "10000000";
                dt.Rows.Add(dr);
                //dt.Rows[0].Delete();//删除一行数据
                //1.会自动生成对应的command命令
                //SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(da);
                //2.手动配置对应command命令
                SqlCommand delCmd = new SqlCommand("delete from student where code=@Scode", conn);
                SqlCommand updateCmd = new SqlCommand("update student set" +
                    "values(name=@Sname,sex=@Ssex,address=@Saddress,age=@Sage,interesting=@Sinteresting,classteacher=@Sclassteacher" +
                    "where code=@Scode", conn);

                SqlCommand insertCommand = new SqlCommand("insert into student(code,name,sex,address,age,interesting,classteacher) " +
                    "values(@Scode,@Sname,@Ssex,@Saddress,@Sage,@Sinteresting,@Sclassteacher)",conn);
                SqlParameter[] param =
                {
                    new SqlParameter("@Scode",SqlDbType.VarChar,50,"code"),
                    new SqlParameter("@Sname", SqlDbType.VarChar, 50, "name"),
                    new SqlParameter("@Ssex", SqlDbType.Int,10, "sex"),
                    new SqlParameter("@Saddress", SqlDbType.VarChar, 50, "address"),
                    new SqlParameter("@Sage", SqlDbType.Int, 10,"age"),
                    new SqlParameter("@Sinteresting", SqlDbType.VarChar, 50, "interesting"),
                    new SqlParameter("@Sclassteacher", SqlDbType.VarChar, 50, "classteacher")
                };
                SqlParameter[] param1=
                {
                    new SqlParameter("@Scode",SqlDbType.VarChar,50,"code")
                };
                insertCommand.Parameters.Clear();
                insertCommand.Parameters.AddRange(param);
                updateCmd.Parameters.Clear();
                updateCmd.Parameters.AddRange(param);
                delCmd.Parameters.Clear();
                delCmd.Parameters.AddRange(param1);
                da.DeleteCommand = delCmd;
                da.InsertCommand = insertCommand;
                conn.Open();
                da.Update(dt);
                //da.Fill(dt);
                conn.Close();//断开式的机制：Fill的前后conn的状态都是Closed，只有在执行的过程中才会Open(),这是由DataAdpater自己来执行
                //连接式的机制：需要在da.Fill(dt)之前添加conn.Open()，不管Fill的状态如何都是Open,所以需要之后添加conn.Closed();优先使用显示连接式的机制
            }
            /*
                    SqlDataReader                SqlDataAdapter
               速度:  快                           慢(数据量小的情况不明显)         
           适用数据量:小                           大
               内存:  小                           大
               连接:一直占用                      断开与连接式
           读取方式:从头读到尾                   一次性加载到内存中通过FILL(一个或多个)，通过UPDATE一次性更新到数据库中
                    .Read(),所以要记得关闭连接
                    一条条的读取                 灵活、可读可写
                    读一条丢一条
                    只向前不能后退的读取
                    即时存储
                    不灵活
                    只读不能修改
            不要求随意读取，不修改，数据量小的情况下，可以使用SqlDataReader
            可以随意读取，可以修改，数据量大的情况下，可以使用SqlDataAdapter
             */
            Console.ReadKey();
        }
    }
}
