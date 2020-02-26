using GiddyUpCore.Storage;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
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
                return true;

            }
            return false;
        }

        public static int applySpeedBonus(int ticksPerMove, List<Pawn> pawns, StringBuilder explanation)
        {
            int pawnsWithMount = 0;
            int pawnsWithoutMount = 0;

            ExtendedDataStorage store = GiddyUpCore.Base.Instance.GetExtendedDataStorage();
            if (store == null || pawns == null)
            {
                return ticksPerMove;
            }
            foreach (Pawn pawn in pawns)
            {
                ExtendedPawnData pawndata = store.GetExtendedDataFor(pawn);
                if (pawndata != null && pawn.IsColonist && pawn.ridingCaravanMount())
                {
                    pawnsWithMount++;
                }
                else if (pawn.IsColonist)
                {
                    pawnsWithoutMount++;
                }
            }
            if(pawnsWithMount == 0)
            {
                return ticksPerMove;
            }
            if(explanation != null)
            {
                explanation.AppendLine();
                explanation.AppendLine();
                explanation.AppendLine("GU_Car_Explanation_Speed".Translate() + ":");
            }

            if (pawnsWithoutMount == 0) //no pawns without mount, apply full speed bonus
            {
                ticksPerMove = Mathf.RoundToInt(ticksPerMove / ((100f + Base.completeCaravanBonus.Value) / 100));
                if(explanation != null)
                {

                    explanation.AppendLine("  " + "GU_Car_Explanation_EveryoneRiding".Translate());
                    explanation.AppendLine("  " + "GU_Car_Explanation_Bonus".Translate() + ": " + Base.completeCaravanBonus.Value + "%");
                }
            }
            else //otherwise apply small per mount bonus
            {

                int total = pawnsWithMount + pawnsWithoutMount;
                float bestPossible = total -1; // Best possible for incomplete bonus is that total - 1 pawns are riding
                float fractionOfBestPossibleAchieved = pawnsWithMount / bestPossible;

                if(explanation != null && pawnsWithMount > 0)
                {
                    explanation.AppendLine("  " + "GU_Car_Explanation_NotEveryoneRiding".Translate());
                    explanation.AppendLine("  " + "GU_Car_Explanation_MaxNotEveryone".Translate() + ": ");
                    explanation.AppendLine("  " + total + " - 1 = " + bestPossible + " " + "GU_Car_Explanation_OfTotal".Translate());
                    explanation.AppendLine("  " + "GU_Car_Explanation_FractionMax".Translate() + ": " + pawnsWithMount + " / " + bestPossible + " = " + (fractionOfBestPossibleAchieved).ToString("n2"));
                    explanation.AppendLine("  " + "GU_Car_Explanation_Bonus".Translate() + ": " + 
                        Base.incompleteCaravanBonusCap.Value + " * " + 
                        (fractionOfBestPossibleAchieved).ToString("n2") + " = " + 
                        (fractionOfBestPossibleAchieved * Base.incompleteCaravanBonusCap.Value).ToString("n1") + "%");
                }
                ticksPerMove = Mathf.RoundToInt(ticksPerMove / ((100f + fractionOfBestPossibleAchieved* Base.incompleteCaravanBonusCap.Value) / 100));

            }
            return ticksPerMove;
        }

    }



}
