using System;
using System.Diagnostics;
using System.Threading;

namespace CsAsODS
{
    class Program
    {
        public static readonly string FileDir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public static CancellationTokenSource cts = new CancellationTokenSource();
        static void Main(string[] Arg)
        {
            //骚气的标题
            Console.Title = "[" + DateTime.Now.ToShortDateString().ToString() + "]" + "Sven-Coop CS-AS ODS";
            Console.WriteLine(
                "======          Sven-Coop C#-AngelScripts Object Detector System        ======\n" +
                "====== ODS aka Ostade-Cylsilane if u wanna glue and detector something  ======");
            //实例化
            GeoIP g_GeoIP = null;
            object SQL = null;
            if (ConfData.JsReader() && LangData.LangReader())
            {
                FirstInit fit = new FirstInit();
                fit.FirstRun();
                if (ConfData.conf.GeoData.Enable)
                {
                    g_GeoIP = new GeoIP();
                    Thread GeoIPThread = new Thread(new ThreadStart(g_GeoIP.GeoWatcher));
                    GeoIPThread.Name = "GeoIP";
                    GeoIPThread.Start();
                }
                if (ConfData.conf.SQLData.Enable)
                {
<<<<<<< HEAD

                    Thread SQLThread = new Thread(new ThreadStart(FileWa.ThreadMethod));
=======
                    switch (ConfData.conf.SQLData.SQLType)
                    {
                        default: SQL = new SQLRequest(); break;
                        case "MySql": SQL = new SQLRequest(); break;
                        case "MariaDB": SQL = new SQLRequest(); break;
                        case "MongoDB": SQL = new MongoSQL(); break;
                        case "Json": SQL = new JsonSQL(); break;
                    }
                    Thread SQLThread = new Thread(new ThreadStart(SQLMethod));
>>>>>>> parent of 426089a... hahaha
                    SQLThread.Name = "SQLService";
                    SQLThread.Start();
                }
                //紧急刹车
                Thread TBreakWatcher = new Thread(() =>
                {
                    while (true)
                    {
                        //踩了就刹
                        if (cts.Token.IsCancellationRequested)
                        {
                            if (ConfData.conf.SQLData.Enable)
                            {
                                switch (ConfData.conf.SQLData.SQLType)
                                {
                                    default: ((SQLRequest)SQL).Stop(); break;
                                    case "MySql": ((SQLRequest)SQL).Stop(); break;
                                    case "MariaDB": ((SQLRequest)SQL).Stop(); break;
                                    case "MongoDB": ((MongoSQL)SQL).Stop(); break;
                                    case "Json": ((JsonSQL)SQL).Stop(); break;
                                }
                                Console.WriteLine(LangData.lg.General.ThreadEnd + ": " + SQL.ToString());
                            }
                            if (ConfData.conf.GeoData.Enable)
                            {
                                g_GeoIP.Stop();
                                Console.WriteLine(LangData.lg.General.ThreadEnd + ": " + g_GeoIP.ToString());
                            }
                            //退出
                            Console.WriteLine("LangData.lg.General.Exit");
                            Console.ReadKey();
                            Process.GetCurrentProcess().Kill();
                            break;
                        }
                        else
                            GC.Collect();//不踩给劳资扫地
                        Thread.Sleep(1000);
                    }
                });
                TBreakWatcher.Start();
                //提示
                CCUtility.g_Utility.Succ(LangData.lg.General.Running);
            }
            else
            {
                Console.WriteLine("按任意键退出...\nPress any key to exit....\n");
                Console.ReadKey(true);
            }

            void SQLMethod()
            {
                switch (ConfData.conf.SQLData.SQLType)
                {
                    default: ((SQLRequest)SQL).SQLWatcher(); break;
                    case "MySql": ((SQLRequest)SQL).SQLWatcher(); break;
                    case "MariaDB": ((SQLRequest)SQL).SQLWatcher(); break;
                    case "MongoDB": ((MongoSQL)SQL).SQLWatcher(); break;
                    case "Json": ((JsonSQL)SQL).SQLWatcher(); break;
                }
            }
        }

    }
}