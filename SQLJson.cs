using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CsAsODS
{
    class JsonSQL
    {
        string JsonFile = String.Format(Program.FileDir + "SqlJson/{0}.json", ConfData.conf.SQLData.SQLJson.FileName);
        Dictionary<string, JsonCollection> JsonData = new Dictionary<string, JsonCollection>();
        public bool Start()
        {
            if (!Directory.Exists(Program.FileDir + "SqlJson") || !File.Exists(JsonFile))
            {
                CCUtility.g_Utility.Warn(LangData.lg.SQL.FirstRun);
                Directory.CreateDirectory(Program.FileDir + "/SqlJson");
                CCWriter.g_Writer.Writer(JsonFile, JsonConvert.SerializeObject(JsonData, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
            }
            else
                JsonData = JsonConvert.DeserializeObject<Dictionary<string, JsonCollection>>(Reader.g_Reader.ReadIt(JsonFile), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
            CCUtility.g_Utility.Succ(LangData.lg.SQL.Running + ": " + ConfData.conf.SQLData.SQLType);
            return true;
        }
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            CCUtility.g_Utility.FileWatcherLog(e.Name + LangData.lg.SQL.Changed);
            Search();
        }

        public void OnUpdate(object source, FileSystemEventArgs e)
        {
            string changePath = Program.FileDir + ConfData.conf.SQLData.SQLChangeput;
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

        void Update(in string szID, in string szEcco, in string szAdd)
        {
            if (JsonData.ContainsKey(szID))
            {
                JsonCollection data = JsonData[szID];
                data.Ecco = Convert.ToInt32(szEcco);
                if(szAdd != "")
                    data.Addition = szAdd;
                JsonData[szID] = data;
            }
            SaveIt();
        }

        //查询请求
        void Search()
        {
            string inPath = Program.FileDir + ConfData.conf.SQLData.SQLInput;
            string outPath = Program.FileDir + ConfData.conf.SQLData.SQLOutput;
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
                op = op + Request(line[0], line[1]);
            CCWriter.g_Writer.Writer(outPath, op);
        }

        string Request(in string szID, in string szNick)
        {
            try
            {
                string szReturn = "";
                if (JsonData.ContainsKey(szID))
                {
                    JsonCollection data = JsonData[szID];
                    data.Nick = CCUtility.g_Utility.FormatNick(szNick);
                    JsonData[szID] = data;
                    szReturn = data.ID + "," + data.SteamID + "," + data.Nick + "," + data.Ecco + data.Addition;
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
                    szReturn = data.ID + "," + data.SteamID + "," + data.Nick + "," + data.Ecco + "," + data.Addition;
                }
                SaveIt();
                return szReturn;
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.RequstError + ": " + e.Message.ToString());
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
                CCUtility.g_Utility.Error(LangData.lg.SQL.UpdateFailed + ": " + e.Message.ToString());
            }
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