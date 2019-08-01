using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CsAsODS
{
    class JsonSQL : SQLabs
    {
        //私密参数
        private string JsonFile = string.Format("{0}/Sql/", Program.FileDir + ConfData.conf.General.Save);

        Dictionary<string, JsonCollection> JsonData = new Dictionary<string, JsonCollection>();
        public override bool Start()
        {
            JsonFile += string.Format("{0}.json", Suffix);

            if (!Directory.Exists(Program.FileDir + ConfData.conf.General.Save + "/Sql") || !File.Exists(JsonFile))
            {
                CCUtility.g_Utility.Warn(LangData.lg.SQL.FirstRun);
                Directory.CreateDirectory(Program.FileDir + ConfData.conf.General.Save + "/Sql");
                CCWriter.g_Writer.Writer(JsonFile, JsonConvert.SerializeObject(JsonData, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
            }
            else
                JsonData = JsonConvert.DeserializeObject<Dictionary<string, JsonCollection>>(Reader.g_Reader.ReadIt(JsonFile), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
            if (!Exr)
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Running + ": " + ConfData.conf.SQLData.SQLType);
            return true;
        }
        public override void OnChanged(object source, FileSystemEventArgs e)
        {
            CCUtility.g_Utility.FileWatcherLog(e.Name + LangData.lg.SQL.Changed);
            Search();
        }

        public override void OnUpdate(object source, FileSystemEventArgs e)
        {
            string changePath = Program.FileDir + Changeput;
            CCUtility.g_Utility.FileWatcherLog(e.Name + LangData.lg.SQL.Changed);
            string str = Reader.g_Reader.ReadIt(changePath);
            string[] line = str.Split('\n');
            for (int i = 0; i < line.Length; i++)
            {
                CCUtility.g_Utility.Taskbar(String.Format(LangData.lg.SQL.Remain, line.Length - i));
                if (!string.IsNullOrEmpty(line[i]))
                {
                    string[] sz = line[i].Split(',');
                    Update(sz[0], sz[1], sz.Length > 2 ? sz[2] : "");
                }
            }
            CCUtility.g_Utility.Taskbar(LangData.lg.General.QuestFinish);
        }
        public override void Update(in string szID, in string szEcco, in string szAdd)
        {
            if (JsonData.ContainsKey(szID))
            {
                JsonCollection data = JsonData[szID];
                data.Ecco = Convert.ToInt32(szEcco);
                if (szAdd != "")
                    data.Addition = szAdd;
                JsonData[szID] = data;
            }
            SaveIt();
        }

        //查询请求
        public override void Search()
        {
            string inPath = Program.FileDir + Input;
            string outPath = Program.FileDir + Output;
            string[] line = Reader.g_Reader.ReadIt(inPath).Split(',');
            bool IsExs = false;
            string[] outLine = Reader.g_Reader.ReadIt(outPath).Split('\n');
            if (line.Length == 0)
                return;
            for (int i = 0; i < outLine.Length; i++)
            {

                if (string.IsNullOrEmpty(outLine[i]))
                    continue;
                else
                {
                    string[] zj = outLine[i].Split(',');
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
        }

        public override string Request(in string szID, in string szNick)
        {
            try
            {
                string szReturn = "";
                if (JsonData.ContainsKey(szID))
                {
                    JsonCollection data = JsonData[szID];
                    data.Nick = CCUtility.g_Utility.FormatNick(szNick);
                    JsonData[szID] = data;
                    szReturn = data.ID + "," + data.SteamID + "," + data.Ecco + "," + data.Addition;
                }
                else
                {
                    JsonCollection data = new JsonCollection()
                    {
                        ID = JsonData.Count + 1,
                        SteamID = szID,
                        Nick = szNick,
                        Ecco = 0,
                        Addition = ""
                    };
                    JsonData[szID] = data;
                    szReturn = data.ID + "," + data.SteamID + "," + data.Ecco + "," + data.Addition;
                }
                SaveIt();
                return szReturn;
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.RequstError, e);
            }
            return null;
        }

        void SaveIt()
        {
            try
            {
                CCWriter.g_Writer.Writer(JsonFile, JsonConvert.SerializeObject(JsonData));
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Updated);
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.UpdateFailed, e);
            }
        }
        public override void Insert(string szID, string szNick, int szEcco, string szAdd)
        {
            throw new NotImplementedException();
        }
    }
    class JsonCollection
    {
        public int ID { get; set; } = 0;
        public string SteamID { get; set; } = "";
        public string Nick { get; set; } = "";
        public int Ecco { get; set; } = 0;
        public string Addition { get; set; } = "";
    }
}