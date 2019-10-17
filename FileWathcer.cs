using System.IO;

namespace CsAsODS
{
    class FileWatcher
    {
        #region 公共传参
        public bool Exr = false;
        public string[] structure = ConfData.conf.SQLData.SQLNet.MySQL.Structure;
        public string szSuffix = ConfData.conf.SQLData.SQLNet.Suffix;
        public string Changeput = ConfData.conf.SQLData.SQLChangeput;
        public string Input = ConfData.conf.SQLData.SQLInput;
        public string Output = ConfData.conf.SQLData.SQLOutput;
        public string Finish = ConfData.conf.SQLData.SQLFinish;
        #endregion
        #region 私密参数
        private FileSystemWatcher fsw = null, fswc = null;
        private readonly MySqlSync clMySQLSync =  new MySqlSync();
        private readonly MySQLRequest clMySQL = new MySQLRequest();
        private readonly MongoSQL clMongoSQL = new MongoSQL();
        private readonly JsonSQL clJsonSQL = new JsonSQL();
        #endregion

        private bool MySQLStart()
        {
            if(ConfData.conf.SQLData.SQLNet.MySQL.UseDataSet)
            {
                clMySQLSync.Exr = Exr;
                clMySQLSync.structure = structure;
                clMySQLSync.Suffix = szSuffix;
                clMySQLSync.Input = Input;
                clMySQLSync.Output = Output;
                clMySQLSync.Changeput = Changeput;
                return clMySQLSync.Start();
            }
            else
            {
                clMySQL.Exr = Exr;
                clMySQL.structure = structure;
                clMySQL.Suffix = szSuffix;
                clMySQL.Input = Input;
                clMySQL.Output = Output;
                clMySQL.Changeput = Changeput;
                return clMySQL.Start();
            }
        }
        private bool MongoStart()
        {
            clMongoSQL.Exr = Exr;
            clMongoSQL.Suffix = szSuffix;
            clMongoSQL.Input = Input;
            clMongoSQL.Output = Output;
            clMongoSQL.Changeput = Changeput;
            return clMongoSQL.Start();
        }
        private bool JsonStart()
        {
            clJsonSQL.Exr = Exr;
            clJsonSQL.Suffix = Exr ? szSuffix : ConfData.conf.SQLData.SQLJson.FileName;
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

        void MysqlOnChanged(object source, FileSystemEventArgs e)
        {
            if (ConfData.conf.SQLData.SQLNet.MySQL.UseDataSet)
                clMySQLSync.OnChanged(source, e);
            else
                clMySQL.OnChanged(source, e);
        }
        void OnChanged(object source, FileSystemEventArgs e)
        {
            switch (ConfData.conf.SQLData.SQLType)
            {
                default: MysqlOnChanged(source, e); break;
                case "MySql": MysqlOnChanged(source, e); break;
                case "MariaDB": MysqlOnChanged(source, e); break;
                case "MongoDB": clMongoSQL.OnChanged(source, e); break;
                case "Json": clJsonSQL.OnChanged(source, e); break;
            }
        }
        void MysqlOnUpdate(object source, FileSystemEventArgs e)
        {
            if (ConfData.conf.SQLData.SQLNet.MySQL.UseDataSet)
                clMySQLSync.OnUpdate(source, e);
            else
                clMySQL.OnUpdate(source, e);
        }
        void OnUpdate(object source, FileSystemEventArgs e)
        {
            switch (ConfData.conf.SQLData.SQLType)
            {
                default: MysqlOnUpdate(source, e); break;
                case "MySql": MysqlOnUpdate(source, e); break;
                case "MariaDB": MysqlOnUpdate(source, e); break;
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