#include "../../../custom_weapons/weapon_ied"
#include "../../../custom_weapons/weapon_allahuakbar"
#include "../../../custom_weapons/item_w33d"
#include "../../../custom_weapons/normal_weapon_effect"


namespace CSwepRegister
{
bool g_ExtraPrecache = false;
void MapInit() 
{
	NormalPreCache();
	MapWeaponRegister();
}

void MapWeaponRegister()
{
	if (!g_EchoDynamic.IsOnList(g_Engine.mapname)) 
	{
		g_ExtraPrecache = true;
		//额外的
		//RegisterSentryv3();
		RegisterIED();
	}
	else
		g_ExtraPrecache = false;
	//基础的
	RegisterAllahuakbar();
	RegisterW33d();
}
}