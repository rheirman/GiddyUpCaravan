using GiddyUpCore.Storage;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Utilities
{
    static class CaravanUtility
    {
        public static bool ridingCaravanMount(this Pawn pawn)
        {
            ExtendedPawnData pawndata = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
            if (pawn.IsColonist && pawndata != null && pawndata.caravanMount != null)
            {
                Log.Message("pawn is colonis, has pawndata and has caravan mount");
                Log.Message("Pawn is caravan member: " + pawn.IsCaravanMember());
                Log.Message("CaravanMount is caravan member: " + pawndata.caravanMount.IsCaravanMember());
                Log.Message("pawn has same caravan as mount: " + (pawn.GetCaravan() == pawndata.caravanMount.GetCaravan()));
                //ExtendedPawnData mountData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawndata.mount);

                // if (mountData.selectedForCaravan)
                //{
                //     return true;
                //}
                return true;

            }
            return false;
        }
    }
}
