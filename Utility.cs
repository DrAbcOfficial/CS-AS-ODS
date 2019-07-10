using System;
using System.IO;
using System.Text;
using System.Threading;

namespace CsAsODS
{
    public class CCUtility
    {
        //实例化
        public static CCUtility g_Utility = new CCUtility();

        public void Dialog(in string szInput)
        {
            TextColor();
            FormatCon(szInput);
        }

        public void WriteLog(in Exception e, string LogAddress = "")
        {
            //都不启用输出个卵log
            if (!ConfData.conf.General.Log)
                return;
            //e为空输出个卵log
            if (e == null)
                return;
            //如果日志文件为空，则默认在Debug目录下新建 YYYY-mm-dd_Log.log文件
            if (LogAddress == "")
                LogAddress = Environment.CurrentDirectory + '/' + ConfData.conf.General.Save + "/Log/";
            string LogName = LogAddress + DateTime.Now.Year + '-' +
                    DateTime.Now.Month + '-' +
                    DateTime.Now.Day + ".log";
            //判断文件夹是否存在
            if (!Directory.Exists(LogAddress))
                Directory.CreateDirectory(LogAddress);
            //把异常信息输出到文件
            StreamWriter fs = new StreamWriter(LogName, true);
            fs.WriteLine("<===================================================================>");
            fs.WriteLine("[1] " + LangData.lg.General.Log.Time + ": " + DateTime.Now.ToString());
            fs.WriteLine("[2] " + LangData.lg.General.Log.Message + ": " + e.Message);
            fs.WriteLine("[3] " + LangData.lg.General.Log.Source + ": " + e.Source);
            fs.WriteLine("[4] " + LangData.lg.General.Log.StackTrace + ": \n" + e.StackTrace.Trim());
            fs.WriteLine("[5] " + LangData.lg.General.Log.TargetSite + ": " + e.TargetSite);
            fs.WriteLine("<===================================================================>");
            fs.WriteLine();
            fs.Close();
        }

        public void Error(in string inPut, in Exception e = null)
        {
            WriteLog(e);
            TextColor("Red");
            string Message = "";
            if (e != null)
                Message = e.Message;
            FormatCon("[ERROR]" + inPut + ": " + Message);
            TextColor();
        }
        public void CritError(in string inPut, in string szReason = "", in Exception e = null)
        {
            WriteLog(e);
            TextColor("Red");
            FormatCon(
                    "\n==========================================================================================================\n" +
                    "　　ＯＯＯＯＯ　　　　　　　ＯＯＯＯ　　　　　　　ＯＯＯＯ　　　　　　　  ＯＯＯ  　　　　　　　ＯＯＯＯ　\n" +
                    "　　Ｏ　　　　　　　　　　　Ｏ　　Ｏ　　　　　　　Ｏ　　Ｏ　　　　　　　ＯＯ　ＯＯ　　　　　　　Ｏ　　Ｏ　\n" +
                    "　　Ｏ　　　　　　　　　　　Ｏ　　Ｏ　　　　　　　Ｏ　　Ｏ　　　　　　　Ｏ  　  Ｏ　　　　　　　Ｏ　　Ｏ　\n" +
                    "　　ＯＯＯＯ　　　　　　　　ＯＯＯＯ　　　　　　　ＯＯＯＯ　　　　　　　Ｏ　　　Ｏ　　　　　　　ＯＯＯＯ　\n" +
                    "　　Ｏ　　　　　　　　　　　ＯＯＯＯ　　　　　　　ＯＯＯＯ　　　　　　　Ｏ　　　Ｏ　　　　　　　ＯＯＯＯ　\n" +
                    "　　Ｏ　　　　　　　　　　　Ｏ　　Ｏ　　　　　　　Ｏ　　Ｏ　　　　　　  Ｏ  　  Ｏ　　　　　　　Ｏ　　Ｏ　\n" +
                    "　　Ｏ　　　　　　　　　　　Ｏ　　　Ｏ　　　　　　Ｏ　　　Ｏ　　　　　　ＯＯ　ＯＯ　　　　　　　Ｏ　　　Ｏ\n" +
                    "　　ＯＯＯＯＯ　　　　　　　Ｏ　　　Ｏ　　　　　　Ｏ　　　Ｏ　　　　　　  ＯＯＯ  　　　　　　　Ｏ　　　Ｏ\n" +
                    "==========================================================================================================\n");
            TextColor("Yellow");
            string Message = "";
            if (e != null)
                Message = e.Message;
            FormatCon("[ERROR]" + inPut + ": " + Message);
            TextColor("Magenta");
            FormatCon(szReason);
            TextColor();
            Program.cts.Cancel();
        }
        public void CritWarn(in string inPut, in Exception e = null)
        {
            WriteLog(e);
            TextColor("Yellow");
            FormatCon(
                    "\n==========================================================================================================\n" +
                    "　      ＯＯ　　　Ｏ　　ＯＯ　　　　　 　Ｏ　　　　　    　 ＯＯＯＯ　　　　　    ＯＯ　　　Ｏ　　　\n" +
                    "　      ＯＯ　　ＯＯ　　ＯＯ　　　　　　ＯＯ　　　　　  　  ＯＯＯＯＯ　　　　    ＯＯＯ　　Ｏ　　　\n" +
                    "　       ＯＯ　ＯＯＯ　ＯＯ　　　　 　 ＯＯＯ　　　　　  　 Ｏ　　　Ｏ　　　　    ＯＯＯ　　Ｏ　　　\n" +
                    "　       ＯＯ　ＯＯＯ　ＯＯ　　　　 　ＯＯＯＯ　　　　　 　 Ｏ　　　Ｏ　　　　　  Ｏ　ＯＯ　Ｏ　　　\n" +
                    "　      　ＯＯＯＯＯＯＯＯ　　　　　 ＯＯ　ＯＯ　　　　  　 ＯＯＯＯ	　　　　 　Ｏ　ＯＯ　Ｏ　　　\n" +
                    "　　      ＯＯＯ　ＯＯＯ　　　　  　ＯＯＯＯＯＯ　　　　　　Ｏ　　ＯＯ　　　　　　Ｏ　　ＯＯＯ　　　\n" +
                    "　　      ＯＯＯ　ＯＯＯ　　　　 　ＯＯ　　ＯＯＯ　　　 　　Ｏ　　　Ｏ　　　　　　Ｏ　　ＯＯＯ　　　\n" +
                    "　　     　ＯＯ　　ＯＯ　　　　　　ＯＯ　　　ＯＯ　　　　 　Ｏ　　　ＯＯ　　　　　Ｏ　　　ＯＯ　　　\n" +
                    "==========================================================================================================\n");
            TextColor("Magenta");
            string Message = "";
            if (e != null)
                Message = e.Message;
            FormatCon("[WARNNING]" + inPut + ": " + Message);
            TextColor();
        }
        public void Warn(in string szInput, in Exception e = null)
        {
            WriteLog(e);
            TextColor("Yellow");
            string Message = "";
            if (e != null)
                Message = e.Message;
            FormatCon("[WARNNING]" + szInput + ": " + Message);
            TextColor();
        }
        public void Succ(in string szInput)
        {
            TextColor("Green");
            FormatCon("[Success]" + szInput);
            TextColor();
        }
        public void Taskbar(in string szInput)
        {
            TextColor();
            FormatCon(szInput);
        }

        public void FileIOLog(in string szInput)
        {
            if (ConfData.conf.General.LogIO)
                FormatCon(szInput);
        }

        public void FileWatcherLog(in string szInput)
        {
            TextColor("Cyan");
            FormatCon(szInput);
            TextColor();
        }

        public void FormatCon(in string szInput)
        {
            Console.WriteLine("==>[" + DateTime.Now.ToString() + "]  " + szInput + ".");
        }
        //彩色字体
        private void TextColor(string colorName = "White")
        {
            try
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName, true);
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.GeoIP.Error, e);
            }
        }
        //占用判断防止报错
        public bool IsFileInUse(in string fileName)
        {
            bool inUse = true;
            if (File.Exists(fileName))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                    inUse = false;
                }
                catch (Exception e)
                {
                    g_Utility.Warn(LangData.lg.General.ReadingFailed, e);
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
                return inUse;
            }
            else
                return false;
        }

        public string get_uft8(in string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        public string FormatNick(in string shitName)
        {
            return shitName.Replace("\"", "").Replace("\'", "").Replace(@"\", "/");
        }
        public void AutoRetry(Action action)
        {
            var tries = ConfData.conf.General.Retry;
            while (true)
            {
                try
                {
                    action();
                    break; // success!
                }
                catch (Exception e)
                {
                    Warn(string.Format(LangData.lg.General.Retrying, ConfData.conf.General.Retry - tries + 1, Thread.CurrentThread.Name), e);
                    if (--tries == 0)
                        CritError(LangData.lg.SQL.ConError, "", e);
                    Thread.Sleep(ConfData.conf.General.RetryTime * 1000);
                }
            }
        }
    }
}