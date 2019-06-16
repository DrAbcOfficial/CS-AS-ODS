﻿using MySql.Data.MySqlClient;
using System;
using System.Runtime.InteropServices;

namespace CsAsODS
{
    public class CCUtility
    {
        //引用DLL
        [DllImport("kernel32.dll")]
        static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        const int OF_READWRITE = 2;
        const int OF_SHARE_DENY_NONE = 0x40;
        static readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        //实例化
        public static CCUtility g_Utility = new CCUtility();

        public void Dialog(in string szInput)
        {
            TextColor();
            FormatCon(szInput);
        }
        public void Error(in string szInput)
        {
            TextColor("Red");
            FormatCon("[ERROR]" + szInput);
            TextColor();
        }
        public void CritError(in string szInput, in string szReason = "")
        {
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
            FormatCon("[ERROR]" + szInput);
            TextColor("Magenta");
            FormatCon(szReason);
            TextColor();
            Program.cts.Cancel();
        }
        public void CritWarn(in string szInput)
        {
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
            FormatCon("[WARNNING]" + szInput);
            TextColor();
        }
        public void Warn(in string szInput)
        {
            TextColor("Yellow");
            FormatCon("[WARNNING]" + szInput);
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
                CCUtility.g_Utility.Error(LangData.lg.GeoIP.Error + ": " + e.Message.ToString());
            }
        }
        //占用判断防止报错
        public bool IsFileInUse(in string fileName)
        {
            IntPtr vHandle = _lopen(fileName, OF_READWRITE | OF_SHARE_DENY_NONE);
            CloseHandle(vHandle);
            return vHandle == HFILE_ERROR ? true : false;
        }
    }
}