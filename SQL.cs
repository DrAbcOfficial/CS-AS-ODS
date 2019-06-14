using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;


namespace CsAsODS
{
    class SQLRequest
    {
        public void SQLWatcher()
        {
            CCUtility.g_Utility.Succ(LangData.lg.SQL.Running);
            List<string[]> empty = new List<string[]>();

            MySqlConnection SQL_con = new MySqlConnection(
                    "server=" + ConfData.conf.SQLData.Server + ";" +
                    "port=" + ConfData.conf.SQLData.Port + ";" +
                    "database=" +  ConfData.conf.SQLData.Database + ";" +
                    "user=" + ConfData.conf.SQLData.Account + ";" +
                    "password=" + ConfData.conf.SQLData.Password );

            if(CCUtility.g_Utility.SQLOpen(SQL_con))
                if (!SQL_con.GetSchema("Tables").AsEnumerable().Any(x => x.Field<string>("TABLE_NAME") == ConfData.conf.SQLData.Prefix + "_Ecco"))
                {
                    CCUtility.g_Utility.Warn(LangData.lg.SQL.FirstRun);
                    SQLFirstRun();
                }
            SQL_con.Close();
            //监视文件
            FileSystemWatcher fsw = new FileSystemWatcher
            {
                //获取程序路径
                Path = Program.FileDir,
                //获取或设置要监视的更改类型
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                //要监视的文件
                Filter = ConfData.conf.SQLData.SQLInput,
                //设置是否级联监视指定路径中的子目录
                IncludeSubdirectories = false
            };
            //添加事件
            fsw.Changed += OnChanged;
            // 开始监听
            fsw.EnableRaisingEvents = true;

            SQLWrite write = new SQLWrite();
            write.SQL_con = SQL_con;
            write.SQL();

            void SQLFirstRun()
            {
                string createStatement = String.Format("CREATE TABLE `{0}`.`{1}_Ecco` ( `{2}` INT NOT NULL AUTO_INCREMENT , `{3}` VARCHAR(24) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL , `{4}` VARCHAR(24) NOT NULL , `{5}` INT NOT NULL , PRIMARY KEY (`{2}`, `{3}`)) ENGINE = InnoDB;",
                    ConfData.conf.SQLData.Database, ConfData.conf.SQLData.Prefix, "ID", "SteamID","NickName", "Ecco");
                // 建表
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(createStatement, SQL_con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    CCUtility.g_Utility.Error(LangData.lg.SQL.ConError + ": " + e.Message.ToString());
                }
            }
            //改变时
            void OnChanged(object source, FileSystemEventArgs e)
            {
                CCUtility.g_Utility.FileWatcherLog(e.Name + LangData.lg.SQL.Changed);
                Search();
            }

            //查询请求
            void Search()
            {
                string inPath = Program.FileDir + ConfData.conf.SQLData.SQLInput;
                string outPath = Program.FileDir + ConfData.conf.SQLData.SQLOutput;
                string[] line = Reader.g_Reader.ReadIt(inPath).Split(',');
                CCUtility.g_Utility.SQLOpen(SQL_con);

                bool IsExs = false;
                string[] outLine = Reader.g_Reader.ReadIt(outPath).Split('\n');
                for (int i = 0; i < outLine.Length; i++)
                {
                    string[] zj = outLine[i].Split(',');
                    if (string.IsNullOrEmpty(outLine[i]))
                        continue;
                    else if (zj[1] == line[0])
                    {
                        outLine[i] = Request(line[0], line[1]);
                        IsExs = true;
                    }
                }

                string op = "";
                for (int i = 0; i < outLine.Length; i++)
                {
                    if (!string.IsNullOrEmpty(outLine[i]))
                        op = op + outLine[i] + "\n";
                }
                if (!IsExs)
                    op = op + Request(line[0], line[1]);
                CCWriter.g_Writer.Writer(outPath, op);

                SQL_con.Close();
                if(empty.Count != 0)
                {
                    CCUtility.g_Utility.SQLOpen(SQL_con);
                    string[][] ary = empty.ToArray();
                    for(int i = 0;i < ary.Length;i ++)
                    {
                        Insert(ary[i][0], ary[i][1], 0);
                    }
                    SQL_con.Close();
                    empty.Clear();
                }
            }

            void Insert(string szID, string szNick, int szEcco)
            {
                CCUtility.g_Utility.Warn(LangData.lg.SQL.Insert + ": [" + szID + "]");
                string str = String.Format("INSERT INTO `{0}_Ecco` (`SteamID`, `NickName`, `Ecco`) VALUES ('{1}', '{2}', '{3}')", ConfData.conf.SQLData.Prefix, szID, szNick, szEcco); ;
                //更新SQL
                MySqlCommand cmd = new MySqlCommand(str, SQL_con);
                if (cmd.ExecuteNonQuery() >0)
                {
                    CCUtility.g_Utility.Succ(LangData.lg.SQL.Inserted);
                }
            }

            string Request(string szID, string szNick)
            {
                string str = String.Format("UPDATE `{0}_Ecco` SET `NickName` = '{2}' WHERE `{0}_Ecco`.`SteamID` = '{1}'; select * from {0}_Ecco where SteamID= '{1}'", ConfData.conf.SQLData.Prefix,szID,szNick);
                //设置查询命令
                MySqlCommand cmd = new MySqlCommand(str, SQL_con);
                MySqlDataReader reader = null;
                //查询结果读取器
                try
                {
                    //执行查询，并将结果返回给读取器
                    reader = cmd.ExecuteReader();
                    string szReturn = "";
                    if (!reader.HasRows)//不存在则加入列表
                    {
                        CCUtility.g_Utility.Warn(LangData.lg.SQL.Empty);
                        string[] a  = { szID, szNick };
                        empty.Add(a);
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            szReturn = reader[0].ToString() + "," + reader[1].ToString() + "," + reader[2].ToString() + "," + reader[3].ToString();
                        }
                    }
                    return szReturn;
                }
                catch (Exception e)
                {
                    CCUtility.g_Utility.Error(LangData.lg.SQL.RequstError + ": " + e.Message.ToString());
                }
                finally
                {
                    reader.Close();
                }
                return null;
            }
        }
    }
}