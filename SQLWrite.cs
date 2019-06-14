using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace CsAsODS
{
    class SQLWrite
    {
        public MySqlConnection SQL_con = new MySqlConnection();
        public void SQL()
        {
            //监视文件
            FileSystemWatcher fsw = new FileSystemWatcher
            {
                //获取程序路径
                Path = Program.FileDir,
                //获取或设置要监视的更改类型
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                //要监视的文件
                Filter = ConfData.conf.SQLData.SQLChangeput,
                //设置是否级联监视指定路径中的子目录
                IncludeSubdirectories = false
            };
            //添加事件
            fsw.Changed += OnChanged;
            // 开始监听
            fsw.EnableRaisingEvents = true;

            void OnChanged(object source, FileSystemEventArgs e)
            {
                string changePath = Program.FileDir + ConfData.conf.SQLData.SQLChangeput;
                CCUtility.g_Utility.FileWatcherLog(e.Name + LangData.lg.SQL.Changed);
                string str = Reader.g_Reader.ReadIt(changePath);
                string[] line = str.Split('\n');
                CCUtility.g_Utility.SQLOpen(SQL_con);
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Update);
                for (int i =0;i < line.Length;i++)
                {
                    CCUtility.g_Utility.Taskbar(String.Format(LangData.lg.SQL.Remain, line.Length - i));
                    if (!string.IsNullOrEmpty( line[i]))
                    {
                        string[] sz = line[i].Split(',');
                        Update(sz[0], sz[1]);
                    }
                }
                CCUtility.g_Utility.Taskbar(LangData.lg.General.QuestFinish);
                SQL_con.Close();
            }

            void Update(in string ID,in string Ecco)
            {
                string str = String.Format("UPDATE `{0}_Ecco` SET `Ecco` = '{2}' WHERE `{0}_Ecco`.`SteamID` = '{1}'",ConfData.conf.SQLData.Prefix, ID,Ecco);
                //更新SQL
                MySqlCommand cmd = new MySqlCommand(str, SQL_con);
                if (cmd.ExecuteNonQuery() > 0)
                    CCUtility.g_Utility.Succ(LangData.lg.SQL.Updated);
                else
                    CCUtility.g_Utility.Error(LangData.lg.SQL.UpdateFailed + ": " + ID + ":" + Ecco);
            }
        }
    }
}