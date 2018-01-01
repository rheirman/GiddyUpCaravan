using GiddyUpCore.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Utilities
{
    class VisitorMountUtility
    {
        public static void mountAnimals(List<Pawn> list, IncidentParms parms)
        {
            if (list.Count == 0 || !(parms.raidArrivalMode == PawnsArriveMode.EdgeWalkIn || parms.raidArrivalMode == PawnsArriveMode.Undecided) || (parms.raidStrategy != null && parms.raidStrategy.workerClass == typeof(RaidStrategyWorker_Siege)))
            {
                return;
            }
            NPCMountUtility.generateMounts(list, parms, Base.inBiomeWeight, Base.outBiomeWeight, Base.nonWildWeight, Base.visitorMountChance, Base.visitorMountChanceTribal);
        }
    }
}
