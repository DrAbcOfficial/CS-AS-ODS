/***
	SQL 杀敌死亡数统计
*****/

namespace CSQLScoreData
{
	const string JoinTitle = "SQL-Score";
	const string FileDir	= "scripts/plugins/store/SQLOutput.txt";
	const string FileOut	= "scripts/plugins/store/SQLInput.txt";
	const string FIleChange = "scripts/plugins/store/SQLChangeput.txt";
	const string DoneFilePath	= "scripts/plugins/store/SQLFinish";
	string ThatDay;
	dictionary SQLDataBase;

	void ReadSQL()
	{
		File @pFile = g_FileSystem.OpenFile( FileDir , OpenFile::READ );
		if ( pFile !is null && pFile.IsOpen() )
		{
			string line;
			while ( !pFile.EOFReached() )
			{
				pFile.ReadLine( line );
				if ( line.IsEmpty() )
					continue;
				array<string>@ buff = line.Split( "," );				//分割
				CCSQLData data; //实例化
				if(buff.length() > 0)
				{	
					data.Kill = atoi(buff[1]);
					data.Death = atoi(buff[2]);
					SQLDataBase[buff[0]] = data;
				}
			}
			pFile.Close();
		}
		else
			FormatLog("IP data No Read!");							//畜生，你中了甚么
	}
	
	void WriteMetaSQL( string MetaIP ,string FilePath = FileOut )
	{
		File @pFile = g_FileSystem.OpenFile( FilePath , OpenFile::WRITE );
		if ( pFile !is null && pFile.IsOpen())
		{
			pFile.Write(MetaIP);	//写出元数据
			pFile.Close();	
		}
		else
			FormatLog("IP data No Write!");				//畜生，你中了甚么
	}

    void WriteSQL( string MetaIP ,string FilePath = FileOut )
	{
		File @pFile = g_FileSystem.OpenFile( FilePath , OpenFile::WRITE );
		if ( pFile !is null && pFile.IsOpen())
		{
			pFile.Write(MetaIP);	//写出数据
			pFile.Close();	
			@pFile = g_FileSystem.OpenFile( DoneFilePath , OpenFile::WRITE );
			if ( pFile !is null && pFile.IsOpen())
			{
				pFile.Write("#wedone#");	//写出结束数据
				pFile.Close();	
			}
		}
		else
			FormatLog("IP data No Write!");				//畜生，你中了甚么
	}

    string GetKD(int&in Kill , int&in Death)
    {
        if(Death == 0)
            return string(Kill);
        else
		{
			string szKD = string(Kill/Death);
			szKD.Resize(szKD.Find(".") + 1,false);
			return szKD;
		}
    }

	void SQLCast( string&in Name, string&in szID )
	{			
		if(SQLDataBase.exists(szID))
        {
            CCSQLData@ data = cast<CCSQLData@>(SQLDataBase[szID]);
		    CSQLScoreData::SayToAll("["+ CSQLScoreData::JoinTitle + "]玩家:" + Name +"|总击杀:" + data.Kill + " 死亡:" + data.Death + " KD:" + GetKD(data.Kill,data.Death) + "|加入了游戏.\n");	//来了
        }
        else
        {
            CCSQLData data;
            data.Kill = 0;
            data.Death = 0;
            SQLDataBase[szID] = data;
            CSQLScoreData::SayToAll("["+ CSQLScoreData::JoinTitle + "]玩家:" + Name +"|总击杀:" + data.Kill + " 死亡:" + data.Death + " KD:" + GetKD(data.Kill,data.Death) + "|加入了游戏.\n");	//来了
        }
	}

	void SayToAll( string&in InPut )
	{
		//发送信息并记录日志
		g_PlayerFuncs.ClientPrintAll( HUD_PRINTTALK, InPut + "\n" );
		FormatLog( InPut + "\n");
	}

	void FormatLog(string&in InPut)
	{
		string szCurrentTime;
		DateTime time;
		time.Format(szCurrentTime, "%Y.%m.%d - %H:%M:%S" );
		g_Game.AlertMessage(at_logged, "==> [" + szCurrentTime + "] "+ InPut + ".\n");
	}
}

class CCSQLData
{
	private int sz_Kill;
	private int sz_Death;

	int Kill
	{
		get const{ return sz_Kill;}
		set{ sz_Kill = value;}
	}

    int Death
	{
		get const{ return sz_Death;}
		set{ sz_Death = value;}
	}
}


void PluginInit()
{
	g_Module.ScriptInfo.SetAuthor("Dr.Abc");
	g_Module.ScriptInfo.SetContactInfo("Bruh.");

	//注册Time
	g_Hooks.RegisterHook( Hooks::Player::ClientConnected, @ClientConnected );
	g_Hooks.RegisterHook( Hooks::Player::ClientDisconnect, @ClientDisconnect );
	g_Hooks.RegisterHook( Hooks::Player::ClientPutInServer, @ClientPutInServer );
}

void MapInit()
{
	string Date;
	DateTime time;
	time.Format(Date, "%d" );

	if( Date != CSQLScoreData::ThatDay )
	{
		CSQLScoreData::SQLDataBase.deleteAll();
		CSQLScoreData::WriteMetaSQL("",CSQLScoreData::FileDir);
		CSQLScoreData::ThatDay = Date;
	}
}

HookReturnCode ClientConnected( edict_t@ pEntity, const string& in szPlayerName, const string& in szIPAddress, bool& out bDisallowJoin, string& out szRejectReason )
{
	const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pEntity);
	if(!CSQLScoreData::SQLDataBase.exists(szSteamId))
		CSQLScoreData::WriteMetaSQL(szSteamId + "," + szPlayerName);
	return HOOK_HANDLED;
}

HookReturnCode ClientPutInServer(CBasePlayer@ pPlayer)
{
	const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
	if(!CSQLScoreData::SQLDataBase.exists(szSteamId))
		CSQLScoreData::ReadSQL();
	CSQLScoreData::SQLCast(pPlayer.pev.netname, szSteamId );
	return HOOK_HANDLED;
}

HookReturnCode ClientDisconnect(CBasePlayer@ pPlayer )
{
    const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
	if(CSQLScoreData::SQLDataBase.exists(szSteamId))
    {
        CCSQLData@ data = cast<CCSQLData@>(CSQLScoreData::SQLDataBase[szSteamId]);
        data.Kill += int(pPlayer.pev.frags);
        data.Death += pPlayer.m_iDeaths;
        CSQLScoreData::SQLDataBase[szSteamId] = data;
        CSQLScoreData::WriteSQL(szSteamId+","+data.Kill+","+data.Death,CSQLScoreData::FIleChange);
    }
	return HOOK_HANDLED;
}
