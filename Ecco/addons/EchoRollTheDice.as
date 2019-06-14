namespace EchoRollTheDice
{
	void Activate()
	{
		e_ScriptParser.Register("gainhelth", CustomMacro(GainHealth));
		e_ScriptParser.Register("gainarmor", CustomMacro(GainArmor));
		e_ScriptParser.Register("gainmeleesecweapons", CustomMacro(GainMeleeSecWeapons));
		e_ScriptParser.Register("gainheavyweapons", CustomMacro(GainHeavyWeapons));
		e_ScriptParser.Register("gainfreekill", CustomMacro(GainFreeKill));
		e_ScriptParser.Register("gaingravity", CustomMacro(GainGravity));
		e_ScriptParser.Register("gainghostmode", CustomMacro(GainGhostMode));
		e_ScriptParser.Register("gainstrip", CustomMacro(GainStrip));
		e_ScriptParser.Register("gainpoints", CustomMacro(GainPoints));
		e_ScriptParser.Register("gainclip", CustomMacro(GainClip));
		e_ScriptParser.Register("gaininvisibility", CustomMacro(GainInvisibility));
		e_ScriptParser.Register("gainpriweapons", CustomMacro(GainPriWeapons));
		e_ScriptParser.Register("gainallweapons", CustomMacro(GainAllWeapons));
		e_ScriptParser.Register("gainnoclip", CustomMacro(GainNoclip));
		e_ScriptParser.Register("gaingod", CustomMacro(GainGod));
		e_ScriptParser.Register("gainfreenuke", CustomMacro(GainFreeNuke));
		CRTDResult::PluginInit();
	}
		
	bool GainHealth(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainHealth(pPlayer);
	   return true;
	}
	bool GainArmor(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainArmor(pPlayer);
	   return true;
	}
	bool GainMeleeSecWeapons(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainMeleeSecWeapons(pPlayer);
	   return true;
	}
	bool GainHeavyWeapons(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainHeavyWeapons(pPlayer);
	   return true;
	}
	bool GainFreeKill(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainFreeKill(pPlayer);
	   return true;
	}
	bool GainGravity(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainGravity(pPlayer);
	   return true;
	}
	bool GainGhostMode(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainGhostMode(pPlayer);
	   return true;
	}
	bool GainStrip(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainStrip(pPlayer);
	   return true;
	}
	bool GainPoints(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainPoints(pPlayer);
	   return true;
	}
	bool GainClip(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainClip(pPlayer);
	   return true;
	}
	bool GainInvisibility(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainInvisibility(pPlayer);
	   return true;
	}
	bool GainPriWeapons(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer))
	   CRTDResult::GainPriWeapons(pPlayer);
	   return true;
	}
	bool GainAllWeapons(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer,true))
	   CRTDResult::GainAllWeapons(pPlayer);
	   return true;
	}
	bool GainNoclip(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer,true))
	   CRTDResult::GainNoclip(pPlayer);
	   return true;
	}
	bool GainGod(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer,true))
	   CRTDResult::GainGod(pPlayer);
	   return true;
	}
	bool GainFreeNuke(CBasePlayer@ pPlayer, array<string>@ args)
	{
	if(CRTDResult::CouldWeRTD(pPlayer,true))
	   CRTDResult::GainFreeNuke(pPlayer);
	   return true;
	}
}

namespace CRTDResult
{
	HUDTextParams RTD_Parameters;
	const uint g_NextRTD = 20000;
	dictionary g_PlayerRTD, g_PlayerALLA, g_PlayerInvis, g_PlayerNoclip, g_PlayerGod, g_PlayerNuke;
	
	bool CouldWeRTD(CBasePlayer@ pPlayer, bool IsADV = false)
	{
		const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
		if (!g_PlayerRTD.exists(szSteamId))
			g_PlayerRTD[szSteamId] = 0;
				
		uint t = uint(g_EngineFuncs.Time()*1000);
		uint d = t - uint(g_PlayerRTD[szSteamId]);

		if (d < g_NextRTD) 
		{
			float w = float(g_NextRTD - d) / 1000.0f;
			g_PlayerFuncs.ClientPrint(pPlayer, HUD_PRINTTALK, "[RTD] STFU and wait " + ceil(w) + " seconds\n");
			e_PlayerInventory.ChangeBalance( pPlayer, IsADV?100:15);
			return false;
		}
		g_PlayerRTD[szSteamId] = t;
		return true;
	}

	const array<string> player_weaponlist = 
	{
		"weapon_357",
		"weapon_9mmar",
		"weapon_9mmhandgun",
		"weapon_crossbow",
		"weapon_crowbar",
		"weapon_displacer",
		"weapon_eagle",
		"weapon_egon",
		"weapon_gauss",
		"weapon_grapple",
		"weapon_handgrenade",
		"weapon_hornetgun",
		"weapon_m16",
		"weapon_m249",
		"weapon_medkit",
		"weapon_minigun",
		"weapon_pipewrench",
		"weapon_rpg",
		"weapon_satchel",
		"weapon_shotgun",
		"weapon_snark",
		"weapon_sniperrifle",
		"weapon_sporelauncher",
		"weapon_tripmine",
		"weapon_uzi"
	};

	void PluginInit()
	{
		RTD_Parameters.x = -0.45;
		RTD_Parameters.y = 0.85;
		RTD_Parameters.r1 = 140;
		RTD_Parameters.g1 = 220;
		RTD_Parameters.b1 = 250;
		RTD_Parameters.effect = 0;
		RTD_Parameters.a1 = 0;
		RTD_Parameters.fadeinTime = 0.0;
		RTD_Parameters.fadeoutTime = 0.0;
		RTD_Parameters.holdTime = 1.5;
		RTD_Parameters.fxTime = 0.0;
		RTD_Parameters.channel = 20;
	}

	void MapInit()
	{
		g_SoundSystem.PrecacheSound( "items/airtank1.wav" );	
		g_SoundSystem.PrecacheSound( "items/medshot4.wav" );
		g_SoundSystem.PrecacheSound( "items/r_item1.wav" );	
		g_SoundSystem.PrecacheSound( "items/suitchargeok1.wav" );	
		g_SoundSystem.PrecacheSound( "items/weapondrop1.wav" );	
		g_SoundSystem.PrecacheSound( "buttons/blip1.wav" );	
		g_SoundSystem.PrecacheSound( "common/launch_deny2.wav" );
		g_SoundSystem.PrecacheSound( "weapons/scock1.wav" );
		g_SoundSystem.PrecacheSound( "weapons/mine_charge.wav" );
		g_SoundSystem.PrecacheSound( "weapons/mortarhit.wav" );
		g_SoundSystem.PrecacheSound( "barney/ba_bring.wav" );
		g_SoundSystem.PrecacheSound( "scientist/whatyoudoing.wav" );	
		g_Game.PrecacheModel( "sprites/fun/laserbeam.spr" );
		g_Game.PrecacheModel( "sprites/zerogxplode.spr" );
		g_PlayerRTD.deleteAll();
		g_PlayerALLA.deleteAll();
		g_PlayerInvis.deleteAll();
		g_PlayerNoclip.deleteAll();
		g_PlayerGod.deleteAll();
	}

	void Uti_Print( CBasePlayer@ pPlayer, string&in Input , string&in Sound , bool IsSound = true )
	{
		if(IsSound)
			g_SoundSystem.EmitSoundDyn( pPlayer.edict(), CHAN_ITEM, Sound, 0.65, ATTN_NORM, 0, PITCH_NORM );
		g_PlayerFuncs.ClientPrintAll(HUD_PRINTTALK, "[RTD] " + string(pPlayer.pev.netname) + Input + ".\n");
	}

	void GainHealth(CBasePlayer@ pPlayer)
	{
		pPlayer.pev.max_health = g_PlayerFuncs.SharedRandomLong( pPlayer.random_seed, 150, 300 );
		pPlayer.pev.health = pPlayer.pev.max_health;
		
		Uti_Print( pPlayer, " got 1, gaining " + pPlayer.pev.max_health + " Max Health" , "items/medshot4.wav" );
	}

	void GainArmor(CBasePlayer@ pPlayer)
	{
		pPlayer.pev.armortype = g_PlayerFuncs.SharedRandomLong( pPlayer.random_seed, 150, 300 );
		pPlayer.pev.armorvalue = pPlayer.pev.armortype;
		
		Uti_Print( pPlayer, " got 2, gaining " + pPlayer.pev.armortype + " Max Armor" , "items/suitchargeok1.wav" );
	}

	void GainMeleeSecWeapons(CBasePlayer@ pPlayer)
	{
		pPlayer.GiveNamedItem("weapon_crowbar", 0, 0);
		pPlayer.GiveNamedItem("weapon_pipewrench", 0, 0);
		pPlayer.GiveNamedItem("weapon_grapple", 0, 0);
		pPlayer.GiveNamedItem("weapon_9mmhandgun", 0, 0);
		pPlayer.GiveNamedItem("weapon_357", 0, 0);
		pPlayer.GiveNamedItem("weapon_eagle", 0, 0);
		pPlayer.GiveNamedItem("weapon_uziakimbo", 0, 0);
		
		Uti_Print( pPlayer, " got 3, gaining melee & secondary weapons", "" , false);
	}
		
	void GainPriWeapons(CBasePlayer@ pPlayer)
	{
		pPlayer.GiveNamedItem("weapon_9mmAR", 0, 0);
		pPlayer.GiveNamedItem("weapon_shotgun", 0, 0);
		pPlayer.GiveNamedItem("weapon_crossbow", 0, 0);
		pPlayer.GiveNamedItem("weapon_m16", 0, 0);
		
		Uti_Print( pPlayer, " got 4, gaining primary weapons" , "" , false);
	}

	void GainHeavyWeapons(CBasePlayer@ pPlayer)
	{
		pPlayer.GiveNamedItem("weapon_rpg", 0, 0);
		pPlayer.GiveNamedItem("weapon_gauss", 0, 0);
		pPlayer.GiveNamedItem("weapon_egon", 0, 0);
		pPlayer.GiveNamedItem("weapon_hornetgun", 0, 0);
		
		Uti_Print( pPlayer, " got 5, gaining heavy weapons" , "" , false);
	}

	void GainFreeKill(CBasePlayer@ pPlayer)
	{
		const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
		g_PlayerALLA[szSteamId] = 10;
		Uti_Print( pPlayer, " got 6, gaining free suicide" , "", false  );
		g_Scheduler.SetInterval( "ALLAHUAKBAR", 1, 10 );
	}
	
	void ALLAHUAKBAR()
	{
		for (int i = 1; i <= g_Engine.maxClients; i++)
		{
			CBasePlayer@ pPlayer = g_PlayerFuncs.FindPlayerByIndex(i);
			if(pPlayer !is null && pPlayer.IsConnected())
			{
				const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
				if(g_PlayerALLA.exists(szSteamId))
				{
					g_PlayerALLA[szSteamId] = int8(g_PlayerALLA[szSteamId]) - 1;
					g_PlayerFuncs.ClientPrintAll( HUD_PRINTCENTER, string(pPlayer.pev.netname) + " is gonna explode in " + int8(g_PlayerALLA[szSteamId]) + " secs.\n" );
					g_SoundSystem.EmitSoundDyn( pPlayer.edict(), CHAN_ITEM, "buttons/blip1.wav", 0.65, ATTN_NORM, 0, PITCH_NORM );
					NetworkMessage m(MSG_BROADCAST, NetworkMessages::SVC_TEMPENTITY, null );
						m.WriteByte(TE_BEAMTORUS);
						m.WriteCoord(pPlayer.pev.origin.x);
						m.WriteCoord(pPlayer.pev.origin.y);
						m.WriteCoord(pPlayer.pev.origin.z);
						m.WriteCoord(pPlayer.pev.origin.x);
						m.WriteCoord(pPlayer.pev.origin.y);
						m.WriteCoord(pPlayer.pev.origin.z + 200);
						m.WriteShort(g_EngineFuncs.ModelIndex("sprites/fun/laserbeam.spr"));
						m.WriteByte(0);
						m.WriteByte(16);
						m.WriteByte(4);
						m.WriteByte(8);
						m.WriteByte(0);
						m.WriteByte(255);
						m.WriteByte(255);
						m.WriteByte(255);
						m.WriteByte(255);
						m.WriteByte(0);
					m.End();
					if( int8(g_PlayerALLA[szSteamId]) == 0 )
					{
					NetworkMessage n(MSG_BROADCAST, NetworkMessages::SVC_TEMPENTITY, null );
						n.WriteByte(TE_EXPLOSION);
						n.WriteCoord(pPlayer.pev.origin.x);
						n.WriteCoord(pPlayer.pev.origin.y);
						n.WriteCoord(pPlayer.pev.origin.z);
						n.WriteShort(g_EngineFuncs.ModelIndex("sprites/zerogxplode.spr"));
						n.WriteByte(128);
						n.WriteByte(15);
						n.WriteByte(0);
						n.End();
						
						g_EntityFuncs.DispatchKeyValue(pPlayer.edict(), "classify", 2 );
						g_WeaponFuncs.RadiusDamage( pPlayer.pev.origin, pPlayer.pev, pPlayer.pev, 9999.0f, 200, 99, DMG_BLAST | DMG_ALWAYSGIB );
						g_SoundSystem.EmitSoundDyn( pPlayer.edict(), CHAN_AUTO, "weapons/explode4.wav", 1.0, ATTN_NORM, 0, 95 + Math.RandomLong( 0,0x1f ) );
						g_PlayerFuncs.ClientPrintAll( HUD_PRINTNOTIFY, string(pPlayer.pev.netname) + " RTD TOO MUCH AND EXPLODED" );
						g_PlayerALLA.delete(szSteamId);
					}
				}
			}
		}
	}

	void GainGravity(CBasePlayer@ pPlayer)
	{
		pPlayer.pev.gravity = 0.3;
		
		Uti_Print( pPlayer, " got 7, feeling lighter" , "items/airtank1.wav" );
	}

	void GainGhostMode(CBasePlayer@ pPlayer)
	{
		pPlayer.pev.rendermode = 4;
		pPlayer.pev.renderamt = 150;
		pPlayer.pev.renderfx = 16;
		
		Uti_Print( pPlayer, " got 8, gaining GHOST-MODE" , "common/launch_deny2.wav" );
	}

	void GainStrip(CBasePlayer@ pPlayer)
	{
		pPlayer.RemoveAllItems(false);
		pPlayer.SetItemPickupTimes(0);
		
		Uti_Print( pPlayer, " got 9, gaining nothing but losing everything", "items/weapondrop1.wav" );
	}

	void GainPoints(CBasePlayer@ pPlayer)
	{
		int gainpoint = g_PlayerFuncs.SharedRandomLong( pPlayer.random_seed, -10, 100 );
		e_PlayerInventory.ChangeBalance( pPlayer, gainpoint);
		
		Uti_Print( pPlayer, " got 10, gaining " + gainpoint + " points" , "items/r_item1.wav" );
	}

	void GainClip(CBasePlayer@ pPlayer)
	{
		CBasePlayerWeapon@ activeItem = cast<CBasePlayerWeapon@>(pPlayer.m_hActiveItem.GetEntity());
		int gainclip = g_PlayerFuncs.SharedRandomLong( pPlayer.random_seed, 2, 5 );
		activeItem.m_iClip = activeItem.m_iClip * gainclip;
		activeItem.m_iClip2 = activeItem.m_iClip2 * gainclip;	
		
		Uti_Print( pPlayer, " got 11, gaining " + gainclip + " multi clips" , "weapons/scock1.wav" );
	}

	void GainInvisibility(CBasePlayer@ pPlayer)
	{
		pPlayer.pev.flags |= FL_NOTARGET;
		pPlayer.pev.rendermode = kRenderTransTexture;
		pPlayer.pev.renderamt = 50;
		
		const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
		g_PlayerInvis[szSteamId] = 20;
		g_Scheduler.SetInterval("InvisWearOff", 1, 20 );
	
		Uti_Print( pPlayer, " got 12, gaining Invisibility for 20 secs\n", "buttons/blip1.wav" );
	}
	
	void InvisWearOff()
	{
		for (int i = 1; i <= g_Engine.maxClients; i++)
		{
			CBasePlayer@ pPlayer = g_PlayerFuncs.FindPlayerByIndex(i);
			if(pPlayer !is null && pPlayer.IsConnected())
			{
				const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
				if(g_PlayerInvis.exists(szSteamId))
				{
					g_PlayerInvis[szSteamId] = int8(g_PlayerInvis[szSteamId]) - 1;
					g_PlayerFuncs.HudMessage( pPlayer, RTD_Parameters,  "Invisibility: " + int8(g_PlayerInvis[szSteamId]) + "\n" );
					if( int8(g_PlayerInvis[szSteamId]) == 0 )
					{					
						pPlayer.pev.flags = FL_CLIENT;
						pPlayer.pev.rendermode = kRenderNormal;
						pPlayer.pev.renderamt = 0;
					 
						g_PlayerFuncs.ClientPrint(pPlayer, HUD_PRINTTALK, "[RTD] Your Invisibility has worn off." );
						g_PlayerInvis.delete(szSteamId);
					}
				}
			}
		}
	}
	
	void GainAllWeapons(CBasePlayer@ pPlayer)
	{
		string activeItem;
		if (pPlayer.m_hActiveItem.GetEntity() !is null)
		activeItem = pPlayer.m_hActiveItem.GetEntity().pev.classname;
		
		for(uint j = 0; j < player_weaponlist.length(); j++)
		{
			pPlayer.GiveNamedItem(player_weaponlist[j], 0, 9999);
		}
		for (int i = 0; i < 64; i++)
		pPlayer.m_rgAmmo(i, 1000000);
		
		Uti_Print( pPlayer, " got 13, gaining all weapons with infinite ammo", "" , false);
	}
	
	void GainNoclip(CBasePlayer@ pPlayer)
	{
		pPlayer.pev.movetype = MOVETYPE_NOCLIP;
		
		const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
		g_PlayerNoclip[szSteamId] = 15;
		g_Scheduler.SetInterval("NoclipWearOff", 1, 15 );
		
		Uti_Print( pPlayer, " got 14, gaining noclip mode" , "scientist/whatyoudoing.wav" );
	}
	
	void NoclipWearOff()
	{
		for (int i = 1; i <= g_Engine.maxClients; i++)
		{
			CBasePlayer@ pPlayer = g_PlayerFuncs.FindPlayerByIndex(i);
			if(pPlayer !is null && pPlayer.IsConnected())
			{
				const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
				if(g_PlayerNoclip.exists(szSteamId))
				{
					g_PlayerNoclip[szSteamId] = int8(g_PlayerNoclip[szSteamId]) - 1;
					g_PlayerFuncs.HudMessage( pPlayer, RTD_Parameters,  "Noclip: " + int8(g_PlayerNoclip[szSteamId]) + "\n" );
					if( int8(g_PlayerNoclip[szSteamId]) == 0 )
					{					
						pPlayer.pev.movetype = MOVETYPE_WALK;
					 
						g_PlayerFuncs.ClientPrint(pPlayer, HUD_PRINTTALK, "[RTD] Your Noclip has worn off." );
						g_PlayerNoclip.delete(szSteamId);
					}
				}
			}
		}
	}
	
	void GainGod(CBasePlayer@ pPlayer)
	{
		pPlayer.pev.flags = FL_GODMODE;
		
		const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
		g_PlayerGod[szSteamId] = 15;
		g_Scheduler.SetInterval("GodWearOff", 1, 15 );
		
		Uti_Print( pPlayer, " got 15, gaining god mode" , "barney/ba_bring.wav" );
	}
	
	void GodWearOff()
	{
		for (int i = 1; i <= g_Engine.maxClients; i++)
		{
			CBasePlayer@ pPlayer = g_PlayerFuncs.FindPlayerByIndex(i);
			if(pPlayer !is null && pPlayer.IsConnected())
			{
				const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
				if(g_PlayerGod.exists(szSteamId))
				{
					g_PlayerGod[szSteamId] = int8(g_PlayerGod[szSteamId]) - 1;
					g_PlayerFuncs.HudMessage( pPlayer, RTD_Parameters,  "God: " + int8(g_PlayerGod[szSteamId]) + "\n" );
					if( int8(g_PlayerGod[szSteamId]) == 0 )
					{					
						pPlayer.pev.flags = FL_CLIENT;
					 
						g_PlayerFuncs.ClientPrint(pPlayer, HUD_PRINTTALK, "[RTD] Your God has worn off." );
						g_PlayerGod.delete(szSteamId);
					}
				}
			}
		}
	}
	
	void GainFreeNuke(CBasePlayer@ pPlayer)
	{
		const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
		g_PlayerNuke[szSteamId] = 4;
		Uti_Print( pPlayer, " got 16, gaining free nuke" , "weapons/mine_charge.wav" );
		g_Scheduler.SetInterval( "Nuke", 1, 4 );
	}
	
	void Nuke()
	{
		for (int i = 1; i <= g_Engine.maxClients; i++)
		{
			CBasePlayer@ pPlayer = g_PlayerFuncs.FindPlayerByIndex(i);
			if(pPlayer !is null && pPlayer.IsConnected())
			{
				const string szSteamId = g_EngineFuncs.GetPlayerAuthId(pPlayer.edict());
				if(g_PlayerNuke.exists(szSteamId))
				{
					g_PlayerNuke[szSteamId] = int8(g_PlayerNuke[szSteamId]) - 1;
					g_PlayerFuncs.ClientPrintAll( HUD_PRINTCENTER, "Tactical nuke " + string(pPlayer.pev.netname) + " incoming in " + int8(g_PlayerNuke[szSteamId]) + " secs.\n" );
					NetworkMessage m(MSG_BROADCAST, NetworkMessages::SVC_TEMPENTITY, null );
						m.WriteByte(TE_BEAMTORUS);
						m.WriteCoord(pPlayer.pev.origin.x);
						m.WriteCoord(pPlayer.pev.origin.y);
						m.WriteCoord(pPlayer.pev.origin.z);
						m.WriteCoord(pPlayer.pev.origin.x);
						m.WriteCoord(pPlayer.pev.origin.y);
						m.WriteCoord(pPlayer.pev.origin.z + 600);
						m.WriteShort(g_EngineFuncs.ModelIndex("sprites/fun/laserbeam.spr"));
						m.WriteByte(0);
						m.WriteByte(16);
						m.WriteByte(4);
						m.WriteByte(8);
						m.WriteByte(15);
						m.WriteByte(255);
						m.WriteByte(255);
						m.WriteByte(255);
						m.WriteByte(255);
						m.WriteByte(0);
					m.End();
					if( int8(g_PlayerNuke[szSteamId]) == 0 )
					{
					NetworkMessage n(MSG_BROADCAST, NetworkMessages::SVC_TEMPENTITY, null );
						n.WriteByte(TE_EXPLOSION);
						n.WriteCoord(pPlayer.pev.origin.x);
						n.WriteCoord(pPlayer.pev.origin.y);
						n.WriteCoord(pPlayer.pev.origin.z);
						n.WriteShort(g_EngineFuncs.ModelIndex("sprites/zerogxplode.spr"));
						n.WriteByte(255);
						n.WriteByte(12);
						n.WriteByte(0);
						n.End();
					NetworkMessage o(MSG_BROADCAST, NetworkMessages::SVC_TEMPENTITY, null );
						o.WriteByte(TE_BEAMTORUS);
						o.WriteCoord(pPlayer.pev.origin.x);
						o.WriteCoord(pPlayer.pev.origin.y);
						o.WriteCoord(pPlayer.pev.origin.z);
						o.WriteCoord(pPlayer.pev.origin.x);
						o.WriteCoord(pPlayer.pev.origin.y);
						o.WriteCoord(pPlayer.pev.origin.z + 600);
						o.WriteShort(g_EngineFuncs.ModelIndex("sprites/fun/laserbeam.spr"));
						o.WriteByte(0);
						o.WriteByte(16);
						o.WriteByte(20);
						o.WriteByte(100);
						o.WriteByte(32);
						o.WriteByte(255);
						o.WriteByte(255);
						o.WriteByte(255);
						o.WriteByte(255);
						o.WriteByte(0);
					o.End();
						
						g_WeaponFuncs.RadiusDamage( pPlayer.pev.origin, pPlayer.pev, pPlayer.pev, 9999.0f, 1200, 99, DMG_BLAST | DMG_ALWAYSGIB );
						g_SoundSystem.EmitSoundDyn( pPlayer.edict(), CHAN_AUTO, "weapons/mortarhit.wav", 1.0, ATTN_NORM, 0, 95 + Math.RandomLong( 0,0x1f ) );
						g_PlayerFuncs.ScreenShakeAll( pPlayer.pev.origin, 200, 50, 6);
						g_PlayerFuncs.ClientPrintAll( HUD_PRINTNOTIFY, string(pPlayer.pev.netname) + " RTD TOO MUCH AND EXPLODED" );
						g_PlayerNuke.delete(szSteamId);
					}
				}
			}
		}
	}
	
}
