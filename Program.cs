using System;
using System.Threading;

namespace CsAsODS
{
    class Program
    {
        public static readonly string FileDir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public static CancellationTokenSource cts = new CancellationTokenSource();
        static void Main(string[] Arg)
        {
            FileWatcher FileWa = new FileWatcher();
            //骚气的标题
            Console.Title = "[" + DateTime.Now.ToShortDateString().ToString() + "]" + "Sven-Coop CS-AS ODS";
            Console.WriteLine(
                "==============================================================================\n" +
                "======          Sven-Coop C#-AngelScripts Object Detector System        ======\n" +
                "======           Dr.Abc Contact:https://github.com/DrAbcrealone         ======\n" +
                "======  ODS aka Ostade-Cylsilane if u wanna glue and detecte something  ======\n" +
                "==============================================================================");
            //实例化
            GeoIP g_GeoIP = null;
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
                else
                    CCUtility.g_Utility.Warn(LangData.lg.GeoIP.Disable);
                if (ConfData.conf.SQLData.Enable)
                {

                    Thread SQLThread = new Thread(new ThreadStart(FileWa.ThreadMethod));
                    SQLThread.Name = "SQLService";
                    SQLThread.Start();
                }
                else
                    CCUtility.g_Utility.Warn(LangData.lg.SQL.Disable);
                ThreadBreak();
                //提示
                CCUtility.g_Utility.Succ(LangData.lg.General.Running);
            }
            else
            {
                Console.WriteLine("按任意键退出...\nPress any key to exit....\n");
                Console.ReadKey(true);
            }

            void ThreadBreak()
            {
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
                                FileWa.Stop();
                                Console.WriteLine(LangData.lg.General.ThreadEnd + ": " + FileWa.ToString());
                            }
                            if (ConfData.conf.GeoData.Enable)
                            {
                                g_GeoIP.Stop();
                                Console.WriteLine(LangData.lg.General.ThreadEnd + ": " + g_GeoIP.ToString());
                            }
                            //退出
                            Console.WriteLine(LangData.lg.General.Exit);
                            Environment.Exit(0);
                            break;
                        }
                        else
                            GC.Collect();//不踩给劳资扫地
                        Thread.Sleep(1000);
                    }
                });
                TBreakWatcher.Start();
            }
        }
    }
}