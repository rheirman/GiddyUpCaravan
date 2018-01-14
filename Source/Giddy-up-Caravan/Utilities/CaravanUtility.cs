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

        public static int applySpeedBonus(int ticksPerMove, List<Pawn> pawns)
        {
            int pawnsWithMount = 0;
            int pawnsWithoutMount = 0;

            ExtendedDataStorage store = GiddyUpCore.Base.Instance.GetExtendedDataStorage();
            if (store == null)
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

            if (pawnsWithoutMount == 0) //no pawns without mount, apply full speed bonus
            {
                Log.Message("result before bonus");
                ticksPerMove = Mathf.RoundToInt(ticksPerMove / ((100f + Base.completeCaravanBonus.Value) / 100));
                Log.Message("result after bonus");

            }
            else //otherwise apply small per mount bonus
            {
                //Log.Message("pawnsWithoutMount: " + pawnsWithoutMount);

                int total = pawnsWithMount + pawnsWithoutMount;
                int adjustedTotal = total > 1 ? total - 1 : 1; //adjusted total makes sure incompleteCaravanBonusCap is achievable and prevents div/0. 
                float isMountedFraction = (float)pawnsWithMount / adjustedTotal;
                //Log.Message("isMountedFraction: " + isMountedFraction);
                ticksPerMove = Mathf.RoundToInt(ticksPerMove / ((100f + isMountedFraction * Base.incompleteCaravanBonusCap.Value) / 100f));

            }
            return ticksPerMove;
        }

        public static int CostToMove(List<Pawn> pawns, int start, int end, float yearPercent)
        {

            //This part makes sure the static tile costs are also decreased by mount usage
            int tileCost = WorldPathGrid.CalculatedCostAt(end, false, yearPercent);
            tileCost = Mathf.RoundToInt((float)tileCost);

            int adjustedTicksPerMove = CaravanTicksPerMoveUtility.GetTicksPerMove(pawns);
            Log.Message("tileCost: " + tileCost);
            Log.Message("adjustedTileCost: " + Utilities.CaravanUtility.applySpeedBonus(tileCost, pawns));
            int result = adjustedTicksPerMove + Utilities.CaravanUtility.applySpeedBonus(tileCost, pawns);
            result = Mathf.RoundToInt((float)result * Find.WorldGrid.GetRoadMovementMultiplierFast(start, end));
            Log.Message("result after road multiplier is: " + result);

            return result;
        }

        //Almost literal copy from vanilla, except Caravan object exposed here. 
        public static int EstimatedTicksToArrive(int from, int to, WorldPath path, float nextTileCostLeft, Caravan caravan, int curTicksAbs)
        {
            return EstimatedTicksToArrive(from, to, path, nextTileCostLeft, caravan.PawnsListForReading, curTicksAbs);
        }



        public static int EstimatedTicksToArrive(int from, int to, WorldPath path, float nextTileCostLeft, List<Pawn> pawns, int curTicksAbs)
        {

            Log.Message("modified estimated ticks called instead of vanilla one");
            int num = 0;
            int num2 = from;
            int num3 = 0;
            int num4 = Mathf.CeilToInt(20000f) - 1;
            int num5 = 60000 - num4;
            int num6 = 0;
            int num7 = 0;
            int num8;
            if (CaravanRestUtility.WouldBeRestingAt(from, (long)curTicksAbs))
            {
                num += CaravanRestUtility.LeftRestTicksAt(from, (long)curTicksAbs);
                num8 = num5;
            }
            else
            {
                num8 = CaravanRestUtility.LeftNonRestTicksAt(from, (long)curTicksAbs);
            }
            while (true)
            {
                num7++;
                if (num7 >= 10000)
                {
                    break;
                }
                if (num6 <= 0)
                {
                    if (num2 == to)
                    {
                        return num;
                    }
                    bool flag = num3 == 0;
                    int start = num2;
                    num2 = path.Peek(num3);
                    num3++;
                    float num9;
                    if (flag)
                    {
                        num9 = nextTileCostLeft;
                    }
                    else
                    {
                        int num10 = curTicksAbs + num;
                        float yearPercent = (float)GenDate.DayOfYear((long)num10, 0f) / 60f;
                        num9 = (float)Utilities.CaravanUtility.CostToMove(pawns, start, num2, yearPercent); //replaced caravanTicksPerMove with caravan here. 
                    }
                    num6 = Mathf.CeilToInt(num9 / 1f);
                }
                if (num8 < num6)
                {
                    num += num8;
                    num6 -= num8;
                    num += num4;
                    num8 = num5;
                }
                else
                {
                    num += num6;
                    num8 -= num6;
                    num6 = 0;
                }
            }
            Log.ErrorOnce("Could not calculate estimated ticks to arrive. Too many iterations.", 1837451324);
            return num;
        }


    }



}
