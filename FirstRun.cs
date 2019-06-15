using System;
using System.IO;

namespace CsAsODS
{
    class FirstInit
    {
        public void FirstRun()
        {
            if (ConfData.conf.GeoData.Enable)
            {
                if (!File.Exists(Program.FileDir + ConfData.conf.GeoData.IPInput))
                    FirstWriter(Program.FileDir + ConfData.conf.GeoData.IPInput);
                if (!File.Exists(Program.FileDir + ConfData.conf.GeoData.IPOutput))
                    FirstWriter(Program.FileDir + ConfData.conf.GeoData.IPOutput);
            }

            if (ConfData.conf.SQLData.Enable)
            {
                if (!File.Exists(Program.FileDir + ConfData.conf.SQLData.SQLChangeput))
                    FirstWriter(Program.FileDir + ConfData.conf.SQLData.SQLChangeput);
                if (!File.Exists(Program.FileDir + ConfData.conf.SQLData.SQLInput))
                    FirstWriter(Program.FileDir + ConfData.conf.SQLData.SQLInput);
                if (!File.Exists(Program.FileDir + ConfData.conf.SQLData.SQLOutput))
                    FirstWriter(Program.FileDir + ConfData.conf.SQLData.SQLOutput);
                if (!File.Exists(Program.FileDir + ConfData.conf.SQLData.SQLFinish))
                    FirstWriter(Program.FileDir + ConfData.conf.SQLData.SQLFinish);
            }
        }

        private void FirstWriter(in string outPath)
        {
            try
            {
                FileStream fs = new FileStream(outPath, FileMode.Create);
                fs.Close();
                CCUtility.g_Utility.FileIOLog(LangData.lg.General.CreateWrite + ": " + outPath + "....");
            }
            catch (Exception e)
            {
                CCUtility.g_Utility.Error(LangData.lg.General.WriteFailed + ": " + e.Message);
            }
        }
    }
}