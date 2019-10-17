using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CsAsODS
{
    class MySqlSync : MySQLRequest
    {
        protected DataSet g_dataSet = new DataSet();

        DataRow GetRowFromId(in string ID, in DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row[structure[1]].ToString() == ID)
                    return row;
            }
            return null;
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
            for (int i = 0; i < line.Length; i++)
            {
                if (!string.IsNullOrEmpty(line[i]))
                {
                    string[] sz = line[i].ToString().Split(',');
                    t3 = Task.Factory.StartNew(() => Update(sz[0], sz[1], sz.Length > 2 ? sz[2] : ""));
                    //不越界
                }
            }
            Task.WaitAll(t3);
            CCUtility.g_Utility.Succ(LangData.lg.SQL.SqlDataSet.ChangeDataSucc);
        }
        public override void Update(in string ID, in string Ecco, in string Add)
        {
            try
            {
                using (DataTable table = g_dataSet.Tables[0])
                {
                    DataRow row = GetRowFromId(ID, table);
                    if (row != null)
                    {
                        row[structure[1]] = ID;
                        row[structure[3]] = Ecco;
                        row[structure[4]] = Add;
                    }
                    else
                        return;

                    table.ImportRow(row);
                }
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.SqlDataSet.ChangeDataFail, e);
                return;
            }
        }

        public override string Request(in string szID, in string szNick)
        {
            string UniNick = CCUtility.g_Utility.Get_uft8(CCUtility.g_Utility.FormatNick(szNick));
            //查询结果读取器
            try
            {
                //执行查询，并将结果返回给读取器
                string szReturn = "";
                DataRow dtrow = GetRowFromId(szID, g_dataSet.Tables[0]);
                if (dtrow == null)//不存在则加入列表
                {
                    CCUtility.g_Utility.Warn(LangData.lg.SQL.SqlDataSet.SearchData);
                    string[] a = { szID, UniNick };
                    empty.Add(a);
                }
                else
                    szReturn = dtrow[0].ToString() + "," + dtrow[1].ToString() + "," + dtrow[3].ToString() + "," + dtrow[4].ToString();
                CCUtility.g_Utility.Succ(LangData.lg.SQL.SqlDataSet.SearchSucc);
                return szReturn;
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.SqlDataSet.SearchFail, e);
            }
            return null;
        }

        public override void Search()
        {
            string inPath = Program.FileDir + Input;
            string outPath = Program.FileDir + Output;
            string[] line = Reader.g_Reader.ReadIt(inPath).Split(',');
            if (line.Length == 0)
                return;
            bool IsExs = false;
            string[] outLine = Reader.g_Reader.ReadIt(outPath).Split('\n');
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
            if (empty.Count != 0)
            {
                string[][] ary = empty.ToArray();
                for (int i = 0; i < ary.Length; i++)
                {
                    Insert(ary[i][0], ary[i][1], 0, "0");
                }
                empty.Clear();
            }
        }

        bool InsertDataSet(in string ID, in string Content1, in string Content2, in string Add)
        {
            using (DataTable table = g_dataSet.Tables[ConfData.conf.SQLData.SQLNet.Prefix + "_" + Suffix])
            {
                DataRow row = table.NewRow();
                row[0] = table.Rows.Count + 1;
                row[1] = ID;
                row[2] = Content1;
                row[3] = Content2;
                row[4] = Add;
                table.Rows.Add(row);
            }
            return true;
        }

        public override void Insert(string szID, string szNick, int szEcco, string szAdd)
        {
            try
            {
                InsertDataSet(szID, szNick, szEcco.ToString(), szAdd);
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.SqlDataSet.InsertDataFail, e);
                return;
            }
            finally
            {
                CCUtility.g_Utility.Succ(LangData.lg.SQL.SqlDataSet.InsertDataSucc);
            }
        }
        public override bool Start()
        {
            base.Start();
            IntitalDataSet();
            SyncSql();
            return true;
        }

        void SyncSql()
        {
            Thread Syncthread = new Thread(() =>
            {
                while (true)
                {
                    if (Program.cts.Token.IsCancellationRequested)
                        break;
                    else
                    {
                        using (MySqlConnection SQL_con = new MySqlConnection(szConnection))
                        {
                            string szSearchstr = string.Format("SELECT * FROM {0}_{1}", ConfData.conf.SQLData.SQLNet.Prefix, Suffix);
                            using (MySqlCommand cmd = new MySqlCommand(szSearchstr, SQL_con))
                            {
                                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                                {
                                    MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
                                    SQL_con.Open();
                                    try
                                    {
                                        adapter.FillSchema(g_dataSet, SchemaType.Mapped);
                                        adapter.Update(g_dataSet, ConfData.conf.SQLData.SQLNet.Prefix + "_" + Suffix);
                                        CCUtility.g_Utility.Succ(LangData.lg.SQL.Updated);
                                    }
                                    catch (Exception e)
                                    {
                                        CCUtility.g_Utility.Error(LangData.lg.SQL.UpdateFailed, e);
                                    }
                                }
                            }
                            SQL_con.Close();
                        }
                        //同步上传SQL的代码
                    }
                    Thread.Sleep(ConfData.conf.SQLData.SQLNet.MySQL.DataSetSyncTime * 1000);
                }
            });
            Syncthread.Start();
        }
        bool IntitalDataSet()
        {
            using (MySqlConnection SQL_con = new MySqlConnection(szConnection))
            {
                SQL_con.Open();
                using (MySqlCommand cmd = new MySqlCommand(string.Format("SELECT * FROM {0}_{1}", ConfData.conf.SQLData.SQLNet.Prefix, Suffix), SQL_con))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.FillSchema(g_dataSet, SchemaType.Mapped);
                            adapter.Fill(g_dataSet, ConfData.conf.SQLData.SQLNet.Prefix + "_" + Suffix);
                            CCUtility.g_Utility.Succ(LangData.lg.SQL.SqlDataSet.GetDataSetSucc);
                        }
                        catch (Exception e)
                        {
                            CCUtility.g_Utility.CritError(LangData.lg.SQL.SqlDataSet.GetDataSetFail, "", e);
                            return false;
                        }
                    }
                }
                SQL_con.Close();
            }
            return true;
        }
        
    }
}
