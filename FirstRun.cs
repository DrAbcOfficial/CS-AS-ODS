using System;
using System.IO;
using System.Text;

namespace CsAsODS
{
    class FirstInit
    {
        public void SQLCreate(in string Changeput, in string Input, in string Output, in string Finish)
        {
            if (!File.Exists(Program.FileDir + Changeput))
                FirstWriter(Program.FileDir + Changeput);
            if (!File.Exists(Program.FileDir + Input))
                FirstWriter(Program.FileDir + Input);
            if (!File.Exists(Program.FileDir + Output))
                FirstWriter(Program.FileDir + Output);
            if (!File.Exists(Program.FileDir + Finish))
                FirstWriter(Program.FileDir + Finish);
        }
        private void IPCreate(in string Finish, in string Input, in string Output)
        {
            if (!File.Exists(Program.FileDir + Finish))
                FirstWriter(Program.FileDir + Finish);
            if (!File.Exists(Program.FileDir + Input))
                FirstWriter(Program.FileDir + Input);
            if (!File.Exists(Program.FileDir + Output))
                FirstWriter(Program.FileDir + Output);
        }
        public void FirstRun()
        {
            if (ConfData.conf.GeoData.Enable)
                IPCreate(
                    ConfData.conf.GeoData.IPDoneput,
                    ConfData.conf.GeoData.IPInput,
                    ConfData.conf.GeoData.IPOutput);

            if (ConfData.conf.SQLData.Enable)
                SQLCreate(
                    ConfData.conf.SQLData.SQLChangeput,
                    ConfData.conf.SQLData.SQLInput,
                    ConfData.conf.SQLData.SQLOutput,
                    ConfData.conf.SQLData.SQLFinish);
            if (ConfData.conf.SQLData.ExtraEnable && ConfData.conf.SQLData.ExtraList.Length > 0)
            {
                for (int i = 0; i < ConfData.conf.SQLData.ExtraList.Length; i++)
                {
                    SQLCreate(
                        ConfData.conf.SQLData.ExtraList[i].Changeput,
                        ConfData.conf.SQLData.ExtraList[i].Input,
                        ConfData.conf.SQLData.ExtraList[i].Output,
                        ConfData.conf.SQLData.ExtraList[i].Finish);
                }
            }
        }
        private void FirstWriter(in string outPath)
        {
            try
            {
                FileStream fs = new FileStream(outPath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(ConfData.conf.SQLData.SQLNet.MySQL.Encode));
                sw.Write("");
                sw.Flush();
                sw.Close();
                fs.Close();
                CCUtility.g_Utility.FileIOLog(LangData.lg.General.CreateWrite + ": " + outPath + "....");
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.General.WriteFailed, e);
            }
        }
    }
}