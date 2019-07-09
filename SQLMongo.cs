using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace CsAsODS
{
    class MongoSQL
    {
        List<string[]> empty = new List<string[]>();
        IMongoCollection<BsonDocument> collection = null;
        IMongoDatabase database = null;
        public bool Start()
        {
            string MongoConnect = String.Format(
                "mongodb://{0}:{1}@]{2}:{3}/{4}",
                ConfData.conf.SQLData.SQLNet.Account, ConfData.conf.SQLData.SQLNet.Password, ConfData.conf.SQLData.SQLNet.Server, ConfData.conf.SQLData.SQLNet.Port, ConfData.conf.SQLData.SQLNet.Database);
            //建立连接
            try
            {
                MongoUrl url = new MongoUrl(MongoConnect); // url
                MongoClientSettings settings = MongoClientSettings.FromUrl(url); // 从url 中获取setting
                settings.ServerSelectionTimeout = new TimeSpan(0, 0, ConfData.conf.SQLData.SQLNet.TimeOut); // 设置寻找服务器的时间为10秒
                MongoClient client = new MongoClient(settings); //创建数据库连接 

                client.ListDatabases();
                //建立数据库
                database = client.GetDatabase(ConfData.conf.SQLData.SQLNet.Database);
                string[] arycollection = database.ListCollectionNames().ToList<string>().ToArray();
                bool Foundit = false;
                for (int i = 0; i < arycollection.Length; i++)
                {
                    if (arycollection[i] == ConfData.conf.SQLData.SQLNet.Prefix + "_" + ConfData.conf.SQLData.SQLNet.Suffix)
                    {
                        Foundit = true;
                        break;
                    }
                }
                if (Foundit)
                {
                    //建立collection
                    collection = database.GetCollection<BsonDocument>(ConfData.conf.SQLData.SQLNet.Prefix + "_" + ConfData.conf.SQLData.SQLNet.Suffix);
                }
                else
                {
                    CCUtility.g_Utility.Warn(LangData.lg.SQL.FirstRun);
                    SQLFirstRun();
                }
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Running + ": " + ConfData.conf.SQLData.SQLType);
                return true;

            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.ConError, e);
                return false;
            }
        }

        void SQLFirstRun()
        {
            // 建表
            try
            {
                database.CreateCollection(ConfData.conf.SQLData.SQLNet.Prefix + "_" + ConfData.conf.SQLData.SQLNet.Suffix);
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Updated);
                collection = database.GetCollection<BsonDocument>(ConfData.conf.SQLData.SQLNet.Prefix + "_" + ConfData.conf.SQLData.SQLNet.Suffix);
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.ConError, e);
            }
        }
        //改变时
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
            CCUtility.g_Utility.Succ(LangData.lg.SQL.Update);
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

        void Update(in string ID, in string Ecco, in string Add)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("SteamID", "ID");
                var update = Builders<BsonDocument>.Update.Set("Ecco", Convert.ToInt32(Ecco));
                collection.UpdateMany(filter, update);
                if (Add != "")
                {
                    update = Builders<BsonDocument>.Update.Set("Addition", Add);
                    collection.UpdateMany(filter, update);
                }
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Updated);
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.UpdateFailed + ": " + ID + ":" + Ecco + "|Reason: ", e);
            }
        }

        //查询请求
        void Search()
        {
            string inPath = Program.FileDir + ConfData.conf.SQLData.SQLInput;
            string outPath = Program.FileDir + ConfData.conf.SQLData.SQLOutput;
            string[] line = Reader.g_Reader.ReadIt(inPath).Split(',');
            bool IsExs = false;
            string[] outLine = Reader.g_Reader.ReadIt(outPath).Split('\n');
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

            if (empty.Count != 0)
            {
                string[][] ary = empty.ToArray();
                for (int i = 0; i < ary.Length; i++)
                {
                    Insert(ary[i][0], ary[i][1], 0, "");
                }
                empty.Clear();
            }
        }

        void Insert(string szID, string szNick, int szEcco, string szAdd)
        {
            CCUtility.g_Utility.Warn(LangData.lg.SQL.Insert + ": [" + szID + "]");
            //插入SQL
            var document = new BsonDocument
                {
                    {"SteamID",szID},
                    {"Nick",CCUtility.g_Utility.FormatNick(szNick)},
                    {"Ecco",szEcco},
                    {"Addition",szAdd}
                };
            try
            {
                collection.InsertOne(document);
                CCUtility.g_Utility.Succ(LangData.lg.SQL.Inserted);
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.ConError, e);
            }
        }

        string Request(string szID, string szNick)
        {
            try
            {
                var document = new BsonDocument
                    {
                        {"SteamID",szID}
                    };

                //执行查询，并将结果返回给读取器
                var document1 = collection.Find(document);
                MongoCollection mc = JsonConvert.DeserializeObject<MongoCollection>(document1.ToJson(), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });

                string szReturn = "";
                if (string.IsNullOrEmpty(mc.Nick) || (string.IsNullOrEmpty(mc.SteamID))) //不存在则加入列表
                {
                    CCUtility.g_Utility.Warn(LangData.lg.SQL.Empty);
                    string[] a = { szID, CCUtility.g_Utility.FormatNick(szNick) };
                    empty.Add(a);
                }
                else
                    szReturn = mc._id + "," + mc.SteamID + "," + mc.Nick + "," + mc.Ecco + "," + mc.Addition;
                return szReturn;
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.SQL.RequstError, e);
            }
            return null;
        }

        class MongoCollection
        {
            public int _id { get; set; } = 0;
            public string SteamID { get; set; } = "";
            public string Nick { get; set; } = "";
            public int Ecco { get; set; } = 0;
            public string Addition { get; set; } = "";
        }
    }
}