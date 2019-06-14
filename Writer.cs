using System;
using System.IO;

namespace CsAsODS
{
    public class CCWriter
    {
        public static CCWriter g_Writer = new CCWriter();
        public void Writer(in string outPath, in string outContent)
        {
            if (string.IsNullOrEmpty(outContent))
            {
                CCUtility.g_Utility.Warn(LangData.lg.General.WriteEmpty);
                return;
            }
            if (!File.Exists(outPath))
            {
                try
                {
                    FileStream fs = new FileStream(outPath, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    //开始写入
                    sw.Write( outContent);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                        CCUtility.g_Utility.FileIOLog(LangData.lg.General.CreateWrite + ": " + outPath + "....");
                }
                catch (Exception e)
                {
                    CCUtility.g_Utility.Error(LangData.lg.General.WriteFailed + ": " + e.Message);
                }
            }
            if (!CCUtility.g_Utility.IsFileInUse(outPath))
            {
                try
                {
                    FileStream fs = new FileStream(outPath, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    //开始写入
                    sw.Write(outContent);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                        CCUtility.g_Utility.FileIOLog(LangData.lg.General.Writted + ": " + outPath + "....");
                }
                catch (Exception e)
                {
                    CCUtility.g_Utility.Error(LangData.lg.General.WriteFailed + ": " + e.Message);
                }
            }
            else
                CCUtility.g_Utility.Warn(LangData.lg.General.WriteAcupied);
        }
    }
}