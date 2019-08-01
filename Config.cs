using Newtonsoft.Json;
using System;

namespace CsAsODS
{
    public class General
    {
        public string Save { get; set; } = "Ods";
        public string Lang { get; set; } = "zh-CN";
        public bool Hex { get; set; } = false;
        public bool ShowTime { get; set; } = true;
        public bool ShowID { get; set; } = true;
        public bool LogIO { get; set; } = true;
        public int Log { get; set; } = 1;
        public int Retry { get; set; } = 6;
        public int RetryTime { get; set; } = 1;
    }

    public class GeoData
    {
        public bool Enable { get; set; } = true;
        public string IPInput { get; set; } = "IPInput.txt";
        public string IPOutput { get; set; } = "IPOutput.txt";
        public string IPDoneput { get; set; } = "IPDoneput";
        public string[] IPBackFormat { get; set; } = {
            "countryCode",
            "country",
            "regionName",
            "city"
        };
    }
    public class SQLJsonConfig
    {
        public string FileName { get; set; } = "ODS";
    }
    public class MySQLConfig
    {
        public bool OldGUID { get; set; } = true;
        public bool Persist { get; set; } = true;
        public bool Unicode { get; set; } = true;
        public string SSL { get; set; } = "none";
        public string Encode { get; set; } = "utf-8";
        public string[] Structure { get; set; } = {
            "UID",
            "SteamID",
            "UserName",
            "Ecco",
            "Addition"
        };
    }
    public class SQLExtra
    {
        public string Input = "exInput.txt";
        public string Output = "exOutput.txt";
        public string Changeput = "exChangeput.txt";
        public string Finish = "exFinish";
        public string Sheet = "exSheet";
        public string[] Structure { get; set; } = {
            "UID",
            "SteamID",
            "UserName",
            "Content",
            "Addition"
        };
    }
    public class SQLServerConfig
    {
        public int TimeOut { get; set; } = 7;
        public string Server { get; set; } = "localhost";
        public string Port { get; set; } = "3306";
        public string Prefix { get; set; } = "SvenCoop";
        public string Suffix { get; set; } = "ecco";
        public string Database { get; set; } = "Ecco";
        public string Account { get; set; } = "root";
        public string Password { get; set; } = "secret";
        public MySQLConfig MySQL { get; set; } = new MySQLConfig();
    }
    public class SQLData
    {
        public bool Enable { get; set; } = false;
        public string SQLType { get; set; } = "MySql";
        //MySql MariaDB MongoDB Json | Fuck H2
        public SQLServerConfig SQLNet { get; set; } = new SQLServerConfig();
        public SQLJsonConfig SQLJson { get; set; } = new SQLJsonConfig();
        public string SQLInput { get; set; } = "SQLInput.txt";
        public string SQLOutput { get; set; } = "SQLOutput.txt";
        public string SQLChangeput { get; set; } = "SQLChangeput.txt";
        public string SQLFinish { get; set; } = "SQLFinish";
        public bool ExtraEnable { get; set; } = false;
        public SQLExtra[] ExtraList { get; set; } = { new SQLExtra() };
    }
    public class Config
    {
        public General General { get; set; } = new General();
        public GeoData GeoData { get; set; } = new GeoData();
        public SQLData SQLData { get; set; } = new SQLData();
    }
    public class ConfData
    {
        public static Config conf = new Config();
        public static bool JsReader()
        {
            string json = Reader.g_Reader.JsonReader(Program.FileDir + ConfData.conf.General.Save + "/config.json");
            if (string.IsNullOrEmpty(json))
            {
                CCUtility.g_Utility.CritWarn(
                    "不存在配置文件，将生成默认配置文件！\n" +
                    "There is no configuration file, the default configuration file will be generated!\n");
                CreateJson();
                return true;

            }
            else
            {
                try
                {
                    conf = JsonConvert.DeserializeObject<Config>(json);
                    return true;
                }
                catch (Exception e)
                {
                    CCUtility.g_Utility.CritError(
                        "文件格式不正确！无法读取配置文件！请检查json文件拼写！\n" +
                        "The file format is incorrect! Unable to read the configuration file! Please check the spelling of JSON file!\n",
                        "错误代码/Error Code: ", e);
                    return false;
                }
            }
        }
        public static void CreateJson()
        {
            CCUtility.g_Utility.Warn("将生成默认配置文件\nDefault Configuration files will be generated");
            string dconf = JsonConvert.SerializeObject(conf);
            CCWriter.g_Writer.Writer(Program.FileDir + conf.General.Save + "/config.json", dconf);
            CCUtility.g_Utility.Succ("默认配置文件生成完毕\nThe default configuration file has been generated");
        }
    }
}