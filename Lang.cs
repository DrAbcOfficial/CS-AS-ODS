using Newtonsoft.Json;
using System;
using System.IO;

namespace CsAsODS
{
    public class Lang
    {
        public LangGeneral General { get; set; } = new LangGeneral();
        public LangGeoIP GeoIP { get; set; } = new LangGeoIP();
        public LangSQL SQL { get; set; } = new LangSQL();
    }
    public class LangGeneral
    {
        public string Exit { get; set; } = "按任意键退出....";
        public string EmptyInput { get; set; } = "输入的内容为空";
        public string ReadingFile { get; set; } = "正在读取...";
        public string ReadingFailed { get; set; } = "读取失败咯";
        public string ReadingStream { get; set; } = "正在读取流...";
        public string WriteEmpty { get; set; } = "空白,不写出";
        public string Writted { get; set; } = "写入成功！";
        public string CreateWrite { get; set; } = "创建新文件...";
        public string WriteFailed { get; set; } = "写出失败咯";
        public string WriteAcupied { get; set; } = "被占用，不写入";
        public string ReadingJson { get; set; } = "正在读取配置文件";
        public string JsonReadFailed { get; set; } = "配置文件读取失败！";
        public string Running { get; set; } = "成功！程序正在运行中！";
        public string QuestFinish { get; set; } = "所有任务已完成！";
        public string ThreadEnd { get; set; } = "线程已终止";
        public string Retrying { get; set; } = "正在重试第 {0} 次...线程编号：{1}";

    }
    public class LangSQL
    {
        public string Running { get; set; } = "正在运行SQL服务";
        public string Connected { get; set; } = "SQL服务器链接成功！";
        public string ConError { get; set; } = "SQL服务器链接失败！请检查服务器或设置";
        public string ConNetError { get; set; } = "SQL服务器网络链接失败！请检查服务器或设置";
        public string RequstError { get; set; } = "SQL查询失败！";
        public string Changed { get; set; } = "SQL临时文件已改变，开始发送信息！";
        public string Empty { get; set; } = "查询的数据为空！";
        public string FirstRun { get; set; } = "第一次运行，开始进行初始化";
        public string Insert { get; set; } = "向SQL添加记录";
        public string Inserted { get; set; } = "添加记录成功！";
        public string Update { get; set; } = "向SQL更新记录！";
        public string Updated { get; set; } = "更新记录成功！";
        public string UpdateFailed { get; set; } = "更新记录失败！";
        public string Remain { get; set; } = "剩余 {0} 个记录";
    }

    public class LangGeoIP
    {
        public string Geoing { get; set; } = "GeoIP开始监听文件...";
        public string Changed { get; set; } = "文件被改变,开始查找IP库";
        public string EmptyIP { get; set; } = "空白ip,不获取地址";
        public string NetError { get; set; } = "发生网络错误！";
        public string SendingAdd { get; set; } = "正在向服务器发送地址";
        public string Error { get; set; } = "发生错误";
        public string IPAddSucc { get; set; } = "成功！获取了地址！";
        public string EmptyRespond { get; set; } = "响应为空！";
    }

    public class LangData
    {
        public static Lang lg = new Lang();
        public static bool LangReader()
        {
            string json = Reader.g_Reader.JsonReader(Program.FileDir + "lang/" + ConfData.conf.General.Lang + ".json");
            if (string.IsNullOrEmpty(json))
            {
                CCUtility.g_Utility.CritWarn(
                    "语言文件为空，将使用默认语言.\n" +
                    "The language file is empty and the default language will be used.\n");
                if (!File.Exists(Program.FileDir + "lang/zh-CN.json"))
                    CreateJson();
                return true;
            }
            else
            {
                try
                {
                    lg = JsonConvert.DeserializeObject<Lang>(json);
                    return true;
                }
                catch (Exception e)
                {
                    CCUtility.g_Utility.CritError(
                        "文件格式不正确！无法读取语言文件！请检查json文件拼写！\n" +
                        "The file format is incorrect! Unable to read the language file! Please check the spelling of JSON file!\n",
                        "错误代码/Error Code: " + e.Message.ToString() + "\n");
                    return false;
                }
            }

            void CreateJson()
            {
                CCUtility.g_Utility.Warn("将生成默认语言文件\nThe default language file will be generated");
                string zhcn = JsonConvert.SerializeObject(lg);
                if (!Directory.Exists(Program.FileDir + "lang"))
                    Directory.CreateDirectory(Program.FileDir + "lang");
                CCWriter.g_Writer.Writer(Program.FileDir + "lang/zh-CN.json", zhcn);
                CCUtility.g_Utility.Succ("默认语言文件输出完毕\nDefault Language File generated");
            }
        }
    }
}