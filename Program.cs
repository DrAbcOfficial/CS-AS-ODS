using System;
using System.Threading;

namespace CsAsODS
{
    class Program
    {
        public static readonly string FileDir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        static void Main(string[] Arg)
        {
            //骚气的标题
            Console.Title = "[" + DateTime.Now.ToShortDateString().ToString() + "]" + "Sven-Coop CS-AS ODS";
            //退出
            Console.WriteLine(LangData.lg.General.Exit);
            GeoIP g_GeoIP = new GeoIP();
            SQLRequest SQL = new SQLRequest();

            if (ConfData.JsReader() && LangData.LangReader())
            {
                FirstInit fit = new FirstInit();
                fit.FirstRun();
                if (ConfData.conf.GeoData.Enable)
                {
                    Thread GeoIPThread = new Thread(new ThreadStart(g_GeoIP.GeoWatcher));
                    GeoIPThread.Name = "GeoIP";
                    GeoIPThread.Start();
                }
                if (ConfData.conf.SQLData.Enable)
                {
                    Thread SQLThread = new Thread(new ThreadStart(SQL.SQLWatcher));
                    SQLThread.Name = "SQLService";
                    SQLThread.Start();
                }
                //提示
                CCUtility.g_Utility.Succ(LangData.lg.General.Running);
                while (Console.Read() != 'q') ;
            }
            else
            {
                Console.WriteLine("按任意键退出...\nPress any key to exit....\n");
                Console.ReadKey(true);
            }
        }

    }
}