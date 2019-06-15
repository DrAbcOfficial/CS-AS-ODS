#include "Include"

#include "core/ScoreToBalance"
#include "core/LoadInventory"
#include "core/BuyMenu"

bool IsMapAllowed;
CScheduledFunction@ pSchedu = null;
dictionary BalanceData;

void PluginInit(){
	g_Module.ScriptInfo.SetAuthor("Paranoid_AF");
	g_Module.ScriptInfo.SetContactInfo("Please Don't.");
	g_Hooks.RegisterHook(Hooks::Player::ClientPutInServer, @onJoin);
	g_Hooks.RegisterHook(Hooks::Player::ClientConnected, @ClientConnected);
	g_Hooks.RegisterHook(Hooks::Game::MapChange, @MapChange);
  InitEcco();
}

void MapInit(){

  Precache();
  EccoScoreBuffer::RegisterTimer();
  IsMapAllowed = true;
  CSwepRegister::MapInit();//我得找个地方把东西放下
  CRTDResult::MapInit();//我得找个地方把东西放下
  File@ file = g_FileSystem.OpenFile("scripts/plugins/Ecco/BannedMaps.txt", OpenFile::READ);
  if(file !is null && file.IsOpen()){
    while(!file.EOFReached()){
      string sLine;
      file.ReadLine(sLine);
      if(sLine == g_Engine.mapname){
        IsMapAllowed = false;
        continue;
      }
    }
    file.Close();
  }
  g_Hooks.RegisterHook(Hooks::Player::ClientSay, @onChat);
  if(IsMapAllowed){
    EccoBuyMenu::ReadScriptList();
	
	g_EchoDynamic.IsOnList(g_Engine.mapname);//我得找个地方把东西放下
	
    EccoBuyMenu::InitializeBuyMenu();
    IsMapAllowed = !EccoBuyMenu::IsEmpty();
  }
}

void Precache(){
  g_Game.PrecacheGeneric("sprites/misc/dollar.spr");
  g_Game.PrecacheGeneric("sprites/misc/deduct.spr");
  g_Game.PrecacheGeneric("sprites/misc/add.spr");
}


HookReturnCode onChat(SayParameters@ pParams){
  CBasePlayer@ pPlayer = pParams.GetPlayer();
  const CCommand@ cArgs = pParams.GetArguments();
  if(pPlayer !is null && (cArgs[0].ToLowercase() == "!buy" || cArgs[0].ToLowercase() == "/buy")){
    pParams.ShouldHide = true;
    if(IsMapAllowed){
      EccoBuyMenu::OpenBuyMenu(pPlayer);
      return HOOK_HANDLED;
    }else{
      g_PlayerFuncs.ClientPrint(pPlayer, HUD_PRINTTALK, string(EccoConfig["BuyMenuName"]) + " " + string(EccoConfig["LocaleNotAllowed"]) +"\n");
    }
  }
  return HOOK_CONTINUE;
}

HookReturnCode onJoin(CBasePlayer@ pPlayer){
string PlayerId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
  SQLDic(PlayerId);
  if(IsMapAllowed){
    EccoInventoryLoader::LoadPlayerInventory(pPlayer);
    EccoScoreBuffer::ResetPlayerBuffer(pPlayer);
  }
  e_PlayerInventory.RefreshHUD(pPlayer);
  return HOOK_HANDLED;
}

HookReturnCode ClientConnected(edict_t@ pEntity, const string& in szPlayerName, const string& in szIPAddress, bool& out bDisallowJoin, string& out szRejectReason)
{
  SQLQuery(g_EngineFuncs.GetPlayerAuthId(pEntity)+ "," + szPlayerName);
  return HOOK_HANDLED;
}

HookReturnCode MapChange()
{
	e_PlayerInventory.WriteSQLData();
	  BalanceData.deleteAll();
	return HOOK_HANDLED;
}

void SQLQuery(string szID)
  {
    File@ file = g_FileSystem.OpenFile("scripts/plugins/store/SQLInput.txt", OpenFile::WRITE);
    if(file !is null && file.IsOpen())
    {
      file.Write(szID);
      file.Close();
    }
  }

void SQLDic( string szID )
{
  File@ file = g_FileSystem.OpenFile("scripts/plugins/store/SQLOutput.txt", OpenFile::READ);
    if(file !is null && file.IsOpen())
    {
      while(!file.EOFReached())
      {
        string sLine;
        file.ReadLine(sLine);
		array<string> aryline = sLine.Split(",");
        if(sLine != "" && aryline[1] == szID && !BalanceData.exists(szID))
        {
          BalanceData[szID] = atoi(aryline[3]);
		      break;
        }
      }
      file.Close();
    }
}
