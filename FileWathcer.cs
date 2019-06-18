using System.IO;

namespace CsAsODS
{
    class FileWatcher
    {
        FileSystemWatcher fsw = null, fswc = null;
        object SQL = null;
        public void ThreadMethod()
        {
            switch (ConfData.conf.SQLData.SQLType)
            {
                default: SQL = new SQLRequest(); break;
                case "MySql": SQL = new SQLRequest(); break;
                case "MariaDB": SQL = new SQLRequest(); break;
                case "MongoDB": SQL = new MongoSQL(); break;
                case "Json": SQL = new JsonSQL(); break;
            }
            Start();
        }
        public void Stop()
        {
            fsw.EnableRaisingEvents = false;
            fsw.Dispose();
            fswc.EnableRaisingEvents = false;
            fswc.Dispose();
        }

        void Start()
        {
            bool IsSQLStart = false;
            switch (ConfData.conf.SQLData.SQLType)
            {
                default: IsSQLStart = ((SQLRequest)SQL).Start(); break;
                case "MySql": IsSQLStart = ((SQLRequest)SQL).Start(); break;
                case "MariaDB": IsSQLStart = ((SQLRequest)SQL).Start(); break;
                case "MongoDB": IsSQLStart = ((MongoSQL)SQL).Start(); break;
                case "Json": IsSQLStart = ((JsonSQL)SQL).Start(); break;
            }
            if (!IsSQLStart)
            {
                CCUtility.g_Utility.Error(LangData.lg.GeoIP.Error);
                return;
            }


            //监视文件
            fsw = new FileSystemWatcher
            {
                //获取程序路径
                Path = Program.FileDir,
                //获取或设置要监视的更改类型
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                //要监视的文件
                Filter = ConfData.conf.SQLData.SQLInput,
                //设置是否级联监视指定路径中的子目录
                IncludeSubdirectories = false
            };
            //添加事件
            fsw.Changed += OnChanged;
            // 开始监听
            fsw.EnableRaisingEvents = true;

            //监视文件
            fswc = new FileSystemWatcher
            {
                //获取程序路径
                Path = Program.FileDir,
                //获取或设置要监视的更改类型
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                //要监视的文件
                Filter = ConfData.conf.SQLData.SQLFinish,
                //设置是否级联监视指定路径中的子目录
                IncludeSubdirectories = false
            };
            //添加事件
            fswc.Changed += OnUpdate;
            // 开始监听
            fswc.EnableRaisingEvents = true;
        }

        void OnChanged(object source, FileSystemEventArgs e)
        {
            switch (ConfData.conf.SQLData.SQLType)
            {
                default: ((SQLRequest)SQL).OnChanged(source, e); break;
                case "MySql": ((SQLRequest)SQL).OnChanged(source, e); break;
                case "MariaDB": ((SQLRequest)SQL).OnChanged(source, e); break;
                case "MongoDB": ((MongoSQL)SQL).OnChanged(source, e); break;
                case "Json": ((JsonSQL)SQL).OnChanged(source, e); break;
            }
        }

        void OnUpdate(object source, FileSystemEventArgs e)
        {
            switch (ConfData.conf.SQLData.SQLType)
            {
                default: ((SQLRequest)SQL).OnUpdate(source, e); break;
                case "MySql": ((SQLRequest)SQL).OnUpdate(source, e); break;
                case "MariaDB": ((SQLRequest)SQL).OnUpdate(source, e); break;
                case "MongoDB": ((MongoSQL)SQL).OnUpdate(source, e); break;
                case "Json": ((JsonSQL)SQL).OnUpdate(source, e); break;
            }
        }
    }
}