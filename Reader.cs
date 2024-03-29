﻿using System;
using System.IO;
using System.Text;

namespace CsAsODS
{
    public class Reader
    {
        public static Reader g_Reader = new Reader();
        public string ReadIt(in string GeoFile)
        {
            if (!CCUtility.g_Utility.IsFileInUse(GeoFile))
            {
                try
                {
                    StreamReader sr = new StreamReader(GeoFile, Encoding.GetEncoding(ConfData.conf.SQLData.SQLNet.MySQL.Encode));
                    //开始读取
                    string line = sr.ReadToEnd();
                    //关闭流
                    sr.Close();
                    CCUtility.g_Utility.FileIOLog(LangData.lg.General.ReadingFile + ":" + GeoFile + "...");
                    //去分隔符
                    return line;
                }
                catch (Exception e)
                {
                    CCUtility.g_Utility.Error(LangData.lg.General.ReadingFailed, e);
                    return null;
                }
            }
            else return null;
        }

        public string StreamReader(in Stream str)
        {
            try
            {
                StreamReader sr = new StreamReader(str, Encoding.GetEncoding(ConfData.conf.SQLData.SQLNet.MySQL.Encode));
                //开始读取
                string line = sr.ReadToEnd();
                //关闭流
                sr.Close();
                CCUtility.g_Utility.FileIOLog(LangData.lg.General.ReadingStream);
                //去分隔符
                return @line;
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.General.ReadingFailed, e);
                return null;
            }
        }
        public string JsonReader(in string json)
        {
            try
            {
                StreamReader sr = new StreamReader(json, Encoding.GetEncoding(ConfData.conf.SQLData.SQLNet.MySQL.Encode));
                //开始读取
                string line = sr.ReadToEnd();
                //关闭流
                sr.Close();
                CCUtility.g_Utility.Dialog(LangData.lg.General.ReadingJson);
                //去分隔符
                return @line;
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.General.JsonReadFailed, e);
                return null;
            }
        }
    }
}