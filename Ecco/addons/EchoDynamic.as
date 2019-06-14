#include "EchoSwepregister"

class CEchoDynamic
{
	array<string> azMapList;
	void PluginInIt()
	{
		File@ file = g_FileSystem.OpenFile("scripts/plugins/Ecco/addons/DynamicMaps.txt", OpenFile::READ);
			if(file !is null && file.IsOpen())
			{
				string sLine;
				while(!file.EOFReached())
				{
					file.ReadLine(sLine);
					azMapList.insertLast(sLine);
				}
			}
			file.Close();
	}
	
	bool IsOnList( string&in szMapName )
	{
		if(szMapName != "")
		{
			if(IsOnArray(azMapList,szMapName))
			{
				WeaponList();
				return true;
			}
		}
		return false;
	}
	
	bool IsOnArray ( array<string>&in array , string&in wtf )
	{
		for(uint i = 0; i < array.length();i++)
		{
			if( array[i] == wtf)
				return true;
		}
		return false;
	}
	
	void WeaponList()
	{
		string ConfigPath = "scripts/plugins/Ecco/addons/DynamicScripts.txt";
		File@ file = g_FileSystem.OpenFile(ConfigPath, OpenFile::READ);
		if(file !is null && file.IsOpen())
		{
			while(!file.EOFReached())
			{
				string sLine;
				file.ReadLine(sLine);
        
				if(sLine != "")
					EccoBuyMenu::MenuTextAndScriptName.delete(sLine);
			}
			file.Close();
		}
		else
			g_Game.AlertMessage(at_console, "[ERROR - Ecco] Cannot read the config file, check if it exists and SCDS has the permission to access it!\n");
	}
}

CEchoDynamic g_EchoDynamic;