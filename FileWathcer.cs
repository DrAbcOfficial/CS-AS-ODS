using System.IO;

namespace CsAsODS
{
    class FileWatcher
    {
        //公共传参
        public bool Exr = false;
        public string[] structure = ConfData.conf.SQLData.SQLNet.MySQL.Structure;
        public string Suffix = ConfData.conf.SQLData.SQLNet.Suffix;
        public string Changeput = ConfData.conf.SQLData.SQLChangeput;
        public string Input = ConfData.conf.SQLData.SQLInput;
        public string Output = ConfData.conf.SQLData.SQLOutput;
        public string Finish = ConfData.conf.SQLData.SQLFinish;
        //私密参数
        private FileSystemWatcher fsw = null, fswc = null;
        private MySQLRequest clMySQL = new MySQLRequest();
        private MongoSQL clMongoSQL = new MongoSQL();
        private JsonSQL clJsonSQL = new JsonSQL();

        private bool MySQLStart()
        {
            clMySQL.Exr = Exr;
            clMySQL.structure = structure;
            clMySQL.Suffix = Suffix;
            clMySQL.Input = Input;
            clMySQL.Output = Output;
            clMySQL.Changeput = Changeput;
            return clMySQL.Start();
        }
        private bool MongoStart()
        {
            clMongoSQL.Exr = Exr;
            clMongoSQL.Suffix = Suffix;
            clMongoSQL.Input = Input;
            clMongoSQL.Output = Output;
            clMongoSQL.Changeput = Changeput;
            return clMongoSQL.Start();
        }
        private bool JsonStart()
        {
            clJsonSQL.Exr = Exr;
            clJsonSQL.Suffix = Suffix;
            clJsonSQL.Input = Input;
            clJsonSQL.Output = Output;
            clJsonSQL.Changeput = Changeput;
            return clJsonSQL.Start();
        }
        public void Start()
        {
            bool IsSQLStart = false;
            switch (ConfData.conf.SQLData.SQLType)
            {
                default: IsSQLStart = MySQLStart(); break;
                case "MySql": IsSQLStart = MySQLStart(); break;
                case "MariaDB": IsSQLStart = MySQLStart(); break;
                case "MongoDB": IsSQLStart = MongoStart(); break;
                case "Json": IsSQLStart = JsonStart(); break;
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
                Filter = Input,
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
                Filter = Finish,
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
                default: clMySQL.OnChanged(source, e); break;
                case "MySql": clMySQL.OnChanged(source, e); break;
                case "MariaDB": clMySQL.OnChanged(source, e); break;
                case "MongoDB": clMongoSQL.OnChanged(source, e); break;
                case "Json": clJsonSQL.OnChanged(source, e); break;
            }
        }
        void OnUpdate(object source, FileSystemEventArgs e)
        {
            switch (ConfData.conf.SQLData.SQLType)
            {
                default: clMySQL.OnUpdate(source, e); break;
                case "MySql": clMySQL.OnUpdate(source, e); break;
                case "MariaDB": clMySQL.OnUpdate(source, e); break;
                case "MongoDB": clMongoSQL.OnUpdate(source, e); break;
                case "Json": clJsonSQL.OnUpdate(source, e); break;
            }
        }
        public void Stop()
        {
            if (fsw == null)
                return;
            fsw.EnableRaisingEvents = false;
            fsw.Dispose();
            if (fswc == null)
                return;
            fswc.EnableRaisingEvents = false;
            fswc.Dispose();
        }
    }
}