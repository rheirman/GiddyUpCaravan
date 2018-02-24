using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    //This patch makes sure that mounted animals won't attempt to follow the caravan after they are parked
    [HarmonyPatch(typeof(TraderCaravanUtility), "GetTraderCaravanRole")]
    static class TraderCaravanUtility_GetTraderCaravanRole
    {
        static bool Prefix(Pawn p, ref TraderCaravanRole __result)
        {
            //Log.Message("GetTraderCaravanRole called");
            if (p.RaceProps.Animal && p.playerSettings != null)
            {
                //Log.Message("animal!");

                if (p.playerSettings.master != null)
                {
                    //Log.Message("animal master set, setting role to guard");

                    __result = TraderCaravanRole.Guard;
                    return false;
                }
            }
            return true;
        }
    }
}
