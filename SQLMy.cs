using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsAsODS
{
    class MySQLRequest : SQLabs
    {
        string SheetState(in string[] structure)
        {
            string szString;
            if (!Exr)
            {
                szString = string.Format(
                   "CREATE TABLE `{0}`.`{1}_{2}` ( " +
                   "`{3}` INT UNSIGNED NOT NULL AUTO_INCREMENT , " +
                   "`{4}` CHAR(32) CHARACTER SET {8} COLLATE {8}_bin NOT NULL , " +
                   "`{5}` CHAR(36) CHARACTER SET {8} COLLATE {8}_bin NOT NULL , " +
                   "`{6}` INT NOT NULL , " +
                   "`{7}` CHAR(36) CHARACTER SET {8} COLLATE {8}_bin NULL DEFAULT NULL , " +
                   "PRIMARY KEY (`{3}`, `{4}`)) ENGINE = InnoDB CHARSET={8} COLLATE {8}_bin CHARSET={8};",
                    Database,
                    Prefix,
                    Suffix,
                    structure[0],
                    structure[1],
                    structure[2],
                    structure[3],
                    structure[4],
                    Encode.Replace("-", "").Replace("_", ""));
            }
            else
            {
                szString = string.Format("CREATE TABLE `{0}`.`{1}_{2}` ( `{3}` INT UNSIGNED NOT NULL AUTO_INCREMENT ,",
                                            Database,
                                            Prefix,
                                            Suffix,
                                            structure[0]);
                for (int i = 1; i < structure.Length; i++)
                {
                    szString += string.Format("`{0}` CHAR(36) CHARACTER SET {1} COLLATE {1}_bin NOT NULL ",
                                            structure[i],
                                            Encode.Replace("-", "").Replace("_", ""));
                    if (i != structure.Length - 1)
                        szString += ",";
                    else
                        szString += string.Format(",PRIMARY KEY (`{0}`, `{1}`))", structure[0], structure[1]);
                }
                szString += string.Format("ENGINE = InnoDB CHARSET={0} COLLATE {0}_bin;",
                                            Encode.Replace("-", "").Replace("_", ""));
            }
            return szString;
        }
        void SQLFirstRun()
        {
            string createStatement = SheetState(structure);
            // 建表
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(createStatement, SQL_con))
                {
                    cmd.ExecuteNonQuery();
                }
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Updated);
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.ConError, e);
            }
        }
        void SQLOpen(MySqlConnection sql)
        {
            sql.Open();
            CCUtility.g_Utility.Succ(LangData.lg.SQL.Connected);
        }
        public override bool Start()
        {
            //佛祖保佑
            lock(lockobj)
            {
                using (SQL_con = new MySqlConnection(szConnection))
                {
                    CCUtility.g_Utility.AutoRetry(() => SQLOpen(SQL_con));//自动重试，防止质量"特别"好的SQL
                    if (!SQL_con.GetSchema("Tables").AsEnumerable().Any(x => x.Field<string>("TABLE_NAME") == Prefix + "_" + Suffix))
                    {
                        CCUtility.g_Utility.Warn(LangData.lg.SQL.FirstRun);
                        SQLFirstRun();
                    }
                    SQL_con.Close();
                }
            }
            if (!Exr)
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Running + ": " + ConfData.conf.SQLData.SQLType);
            return true;
        }
        //改变时
        public override void OnChanged(object source, FileSystemEventArgs e)
        {
            CCUtility.g_Utility.FileWatcherLog(e.Name + LangData.lg.SQL.Changed);
            Search();
        }
        public override void OnUpdate(object source, FileSystemEventArgs e)
        {
            Task t3 = null;
            string changePath = Program.FileDir + Changeput;
            CCUtility.g_Utility.FileWatcherLog(e.Name + LangData.lg.SQL.Changed);
            string str = Reader.g_Reader.ReadIt(changePath);
            if (string.IsNullOrEmpty(str))
                return;
            string[] line = str.Split('\n');
            CCUtility.g_Utility.Dialog(LangData.lg.SQL.Update);
            for (int i = 0; i < line.Length; i++)
            {
                CCUtility.g_Utility.Taskbar(String.Format(LangData.lg.SQL.Remain, line.Length - i));
                if (!string.IsNullOrEmpty(line[i]))
                {
                    string[] sz = line[i].ToString().Split(',');
                    t3 = Task.Factory.StartNew(() => Update(sz[0], sz[1], sz.Length > 2 ? sz[2] : ""));
                    //不越界
                }
            }
            Task.WaitAll(t3);
            CCUtility.g_Utility.Succ(LangData.lg.SQL.Updated);
            CCUtility.g_Utility.Taskbar(LangData.lg.General.QuestFinish);
        }

        public override void Update(in string ID, in string Ecco, in string Add)
        {
            MySqlConnection ThreadPoolCon = new MySqlConnection(szConnection);
            ThreadPoolCon.Open();
            string str;
            if (Add != "")
            {
                str = String.Format("UPDATE `{0}_{1}` SET `{6}` = '{3}' WHERE `{0}_{1}`.`{5}` = '{2}'; UPDATE `{0}_{1}` SET `{7}` = '{4}' WHERE `{0}_{1}`.`{5}` = '{2}'",
                Prefix,
                Suffix,
                ID,
                Ecco,
                Add,
                structure[1],
                structure[3],
                structure[4]);
            }
            else
            {
                str = String.Format("UPDATE `{0}_{1}` SET `{5}` = '{3}' WHERE `{0}_{1}`.`{4}` = '{2}';",
                Prefix,
                Suffix,
                ID,
                Ecco,
                structure[1],
                structure[3]);
            }
            //更新SQL
            MySqlCommand cmd = new MySqlCommand(str, ThreadPoolCon);
            if (cmd.ExecuteNonQuery() < 0)
                CCUtility.g_Utility.Error(LangData.lg.SQL.UpdateFailed + ": " + ID + ":" + Ecco);
            cmd.Dispose();
            ThreadPoolCon.Close();
        }
        //查询请求
        public override void Search()
        {
            string inPath = Program.FileDir + Input;
            string outPath = Program.FileDir + Output;
            string[] line = Reader.g_Reader.ReadIt(inPath).Split(',');
            if (line.Length == 0)
                return;
            bool IsExs = false;
            string[] outLine = Reader.g_Reader.ReadIt(outPath).Split('\n');
            SQLOpen(SQL_con);
            //查找是否存在此项
            for (int i = 0; i < outLine.Length; i++)
            {
                if (string.IsNullOrEmpty(outLine[i]))
                    continue;
                else
                {
                    string[] zj = outLine[i].Split(',');
                    //存在
                    if (zj[1] == line[0])
                    {
                        outLine[i] = Request(line[0], line[1]);
                        IsExs = true;
                    }
                }
            }

            string op = "";
            for (int i = 0; i < outLine.Length; i++)
            {
                if (!string.IsNullOrEmpty(outLine[i]))
                    op = op + outLine[i] + "\n";
            }
            if (!IsExs)
                op += Request(line[0], line[1]);
            CCWriter.g_Writer.Writer(outPath, op);

            SQL_con.Close();
            if (empty.Count != 0)
            {
                SQLOpen(SQL_con);
                string[][] ary = empty.ToArray();
                for (int i = 0; i < ary.Length; i++)
                {
                    Insert(ary[i][0], ary[i][1], 0, "0");
                }
                SQL_con.Close();
                empty.Clear();
            }
        }

        public override void Insert(string szID, string szNick, int szEcco, string szAdd)
        {
            string UniNick = CCUtility.g_Utility.Get_uft8(CCUtility.g_Utility.FormatNick(szNick));
            CCUtility.g_Utility.Warn(LangData.lg.SQL.Insert + ": [" + szID + "]");
            string str = String.Format(
                "INSERT INTO `{0}_{1}` (`{6}`, `{7}`, `{8}`, `{9}`) VALUES ('{2}', '{3}', '{4}', '{5}')",
                Prefix,
                Suffix,
                szID,
                UniNick,
                szEcco,
                szAdd,
                structure[1],
                structure[2],
                structure[3],
                structure[4]);
            //更新SQL
            MySqlCommand cmd = new MySqlCommand(str, SQL_con);
            if (cmd.ExecuteNonQuery() > 0)
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Inserted);
            cmd.Dispose();
        }

        public override string Request(in string szID, in string szNick)
        {
            string UniNick = CCUtility.g_Utility.Get_uft8(CCUtility.g_Utility.FormatNick(szNick));
            string str = String.Format(
                "UPDATE `{0}_{1}` SET `{5}` = '{3}' WHERE `{0}_{1}`.`{4}` = '{2}'; select * from `{0}_{1}` where `{4}`= '{2}'",
                Prefix,
                Suffix,
                szID,
                UniNick,
                structure[1],
                structure[2]);
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
                    string[] a = { szID, UniNick };
                    empty.Add(a);
                }
                else
                {
                    while (reader.Read())
                    {
                        szReturn = reader[0].ToString() + "," + reader[1].ToString() + "," + reader[3].ToString() + "," + reader[4].ToString();
                    }
                }
                return szReturn;
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.RequstError, e);
            }
            finally
            {
                cmd.Dispose();
                reader.Close();
            }
            return null;
        }
    }
}