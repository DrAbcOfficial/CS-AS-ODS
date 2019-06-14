using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CsAsODS
{
    public class GeoIP
    {
        FileSystemWatcher fsw = null;
        public void Stop()
        {
            fsw.EnableRaisingEvents = false;
            fsw.Dispose();
        }
        public void GeoWatcher()
        {
            CCUtility.g_Utility.Succ(LangData.lg.GeoIP.Geoing);
            //监视文件
            fsw = new FileSystemWatcher
            {
                //获取程序路径
                Path = Program.FileDir,
                //获取或设置要监视的更改类型
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                //要监视的文件
                Filter = ConfData.conf.GeoData.IPInput,
                //设置是否级联监视指定路径中的子目录
                IncludeSubdirectories = false
            };

            //添加事件
            fsw.Changed += OnChanged;
            // 开始监听
            fsw.EnableRaisingEvents = true;
            //改变时
            void OnChanged(object source, FileSystemEventArgs e)
            {
                CCUtility.g_Utility.FileWatcherLog(e.Name + LangData.lg.GeoIP.Changed);
                try { Write(GeoIt(Reader.g_Reader.ReadIt(e.FullPath).Split(',')[1].Split(':')[0]), Reader.g_Reader.ReadIt(e.FullPath).Split(',')[0]); }
                catch (Exception ex) { CCUtility.g_Utility.Error(LangData.lg.GeoIP.Error + ": " + ex.Message.ToString()); }
            }
            //写
            void Write(in string output, in string ID)
            {
                string outPath = Program.FileDir + ConfData.conf.GeoData.IPOutput;
                //开始读取
                string str = Reader.g_Reader.ReadIt(outPath);
                bool IsExs = false;

                string[] cache = str.Split('\n');
                for (int i = 0; i < cache.Length; i++)
                {
                    string[] zj = cache[i].Split(',');
                    if (zj[0] == ID)
                    {
                        cache[i] = ID + "," + FormatIpBack(output);
                        IsExs = true;
                    }
                }

                string op = "";
                for (int i = 0; i < cache.Length; i++)
                {
                    if (!string.IsNullOrEmpty(cache[i]))
                        op = op + cache[i] + "\n";
                }
                if (!IsExs)
                    op = op + ID + "," + FormatIpBack(output);

                CCWriter.g_Writer.Writer(outPath, op);
            }

            //规范格式
            string FormatIpBack(in string Input)
            {
                if (string.IsNullOrEmpty(Input))
                {
                    CCUtility.g_Utility.Error(LangData.lg.General.EmptyInput);
                }
                try
                {
                    IPData ipd = JsonConvert.DeserializeObject<IPData>(Input);
                    Dictionary<string, string> dic = new Dictionary<string, string>{
                        {"city",ipd.city},
                        {"country",ipd.country },
                        {"countryCode",ipd.countryCode },
                        {"isp",ipd.isp },
                        {"lat",ipd.lat },
                        {"lon",ipd.lon },
                        {"org",ipd.org },
                        {"query",ipd.query },
                        {"region",ipd.region },
                        {"regionName",ipd.regionName },
                        {"status",ipd.status },
                        {"timezone",ipd.timezone },
                        {"zip",ipd.zip } };
                    string output = "";
                    for (int i = 0; i < ConfData.conf.GeoData.IPBackFormat.Length; i++)
                    {
                        output = output + dic[ConfData.conf.GeoData.IPBackFormat[i]] + (i == ConfData.conf.GeoData.IPBackFormat.Length - 1 ? "" : ",");
                    }
                    return output;
                }
                catch (Exception e)
                {
                    CCUtility.g_Utility.Error(e.Message);
                }
                return "";
            }


            string GeoIt(in string ipAdd)
            {
                if (string.IsNullOrEmpty(ipAdd))
                {
                    CCUtility.g_Utility.Warn(LangData.lg.GeoIP.EmptyIP);
                    return "";
                }
                else
                    CCUtility.g_Utility.Dialog(LangData.lg.GeoIP.SendingAdd + ": " + ipAdd + "...");

                //发送地址
                string url = "http://ip-api.com/json/" + ipAdd + "?lang=" + ConfData.conf.General.Lang;
                string str = "";
                WebRequest wRequest = WebRequest.Create(url);
                wRequest.Method = "GET";
                wRequest.ContentType = "text/html;charset=UTF-8";
                wRequest.Timeout = 50000; //设置超时时间
                WebResponse wResponse = null;
                try
                {
                    wResponse = wRequest.GetResponse();
                }
                catch (WebException e)
                {
                    //发生网络错误时,获取错误响应信息
                    CCUtility.g_Utility.Error(LangData.lg.GeoIP.NetError + " " + e.Message + ".");
                }
                catch (Exception e)
                {
                    //发生异常时把错误信息当作错误信息返回
                    CCUtility.g_Utility.Error(LangData.lg.GeoIP.Error + ": " + e.Message);

                }
                finally
                {
                    if (wResponse != null)
                    {
                        //获得网络响应流
                        Stream stream = wResponse.GetResponseStream();
                        //url返回的值  
                        str = Reader.g_Reader.StreamReader(stream);
                        wResponse.Close();
                        CCUtility.g_Utility.Succ(LangData.lg.GeoIP.IPAddSucc);
                    }
                    else
                        CCUtility.g_Utility.Warn(LangData.lg.GeoIP.EmptyRespond);
                }
                return str;
            }
        }
    }
    public class IPData
    {
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string isp { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string org { get; set; }
        public string query { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string status { get; set; }
        public string timezone { get; set; }
        public string zip { get; set; }
    }
}
