﻿using GiddyUpCore.Storage;
using HarmonyLib;
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
        static void Postfix(Pawn p, ref TraderCaravanRole __result)
        {
            //Log.Message("GetTraderCaravanRole called");
            if (p.RaceProps.Animal)
            {
                //Log.Message("animal!");
                ExtendedPawnData pawnData = Base.GetExtendedDataStorage().GetExtendedDataFor(p);

                if (pawnData.ownedBy != null)
                {
                    //Log.Message("animal master set, setting role to guard");
                    __result = TraderCaravanRole.Guard;
                }
            }
        }
    }
}
