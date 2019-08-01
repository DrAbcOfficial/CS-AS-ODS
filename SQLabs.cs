using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;

namespace CsAsODS
{
    abstract class SQLabs
    {
        //公共传参
        public bool Exr = false;
        public string[] structure = ConfData.conf.SQLData.SQLNet.MySQL.Structure;
        public string Suffix = ConfData.conf.SQLData.SQLNet.Suffix;
        public string Changeput = ConfData.conf.SQLData.SQLChangeput;
        public string Input = ConfData.conf.SQLData.SQLInput;
        public string Output = ConfData.conf.SQLData.SQLOutput;
        //mnutable
        public MySqlConnection SQL_con = null;
        public List<string[]> empty = new List<string[]>();
        //线程锁
        public object lockobj = new object();
        //只读参数
        public readonly string Prefix = ConfData.conf.SQLData.SQLNet.Prefix;
        public readonly string Database = ConfData.conf.SQLData.SQLNet.Database;
        public readonly string Encode = ConfData.conf.SQLData.SQLNet.MySQL.Encode;
        public readonly string szConnection = "server=" + ConfData.conf.SQLData.SQLNet.Server + ";" +
                    "port=" + ConfData.conf.SQLData.SQLNet.Port + ";" +
                    "database=" + ConfData.conf.SQLData.SQLNet.Database + ";" +
                    "user=" + ConfData.conf.SQLData.SQLNet.Account + ";" +
                    "password=" + ConfData.conf.SQLData.SQLNet.Password + ";" +
                    "Connect Timeout=" + ConfData.conf.SQLData.SQLNet.TimeOut + ";" +
                    "SslMode=" + ConfData.conf.SQLData.SQLNet.MySQL.SSL + ";" +
                    "persistsecurityinfo=" + ConfData.conf.SQLData.SQLNet.MySQL.Persist + ";" +
                    "charset=" + ConfData.conf.SQLData.SQLNet.MySQL.Encode.Replace("-", "").Replace("_", "") + ";" +
                    "Old Guids=" + ConfData.conf.SQLData.SQLNet.MySQL.OldGUID;
        public abstract bool Start();
        public abstract void OnChanged(object source, FileSystemEventArgs e);
        public abstract void OnUpdate(object source, FileSystemEventArgs e);
        public abstract void Update(in string szID, in string szEcco, in string szAdd);
        public abstract void Search();
        public abstract string Request(in string szID, in string szNick);
        public abstract void Insert(string szID, string szNick, int szEcco, string szAdd);
    }
}
