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

        // RimWorld.Planet.CaravanTicksPerMoveUtility
        public static int GetDefaultTicksPerMove(Caravan caravan)
        {
            if (caravan == null)
            {
                return 2500;
            }
            return GetDefaultTicksPerMove(caravan.PawnsListForReading);
        }


        public static int GetDefaultTicksPerMove(List<Pawn> pawns)
        {
            if (pawns.Any<Pawn>())
            {
                float num = 0f;
                for (int i = 0; i < pawns.Count; i++)
                {
                    int num2 = (!pawns[i].Downed) ? pawns[i].TicksPerMoveCardinal : 450;
                    num += (float)num2 / (float)pawns.Count;
                }
                num *= 190f;
                return Mathf.Max(Mathf.RoundToInt(num), 1);
            }
            return 2500;
        }
        public static int CostToMove(List<Pawn> pawns, int start, int end, float yearPercent)
        {

            //This part makes sure the static tile costs are also decreased by mount usage, proportionally to the dynamic costs
            int tileCost = WorldPathGrid.CalculatedCostAt(end, false, yearPercent);
            tileCost = Mathf.RoundToInt((float)tileCost);

            Log.Message("tile cost: " + tileCost);

            int adjustedTicksPerMove = CaravanTicksPerMoveUtility.GetTicksPerMove(pawns);
            Log.Message("adjustedTicksPerMove: " + adjustedTicksPerMove);
            
            int defaultTicksPerMove = Utilities.CaravanUtility.GetDefaultTicksPerMove(pawns);
            Log.Message("defaultTicksPerMove: " + defaultTicksPerMove);

            int result = adjustedTicksPerMove + tileCost;

            float factor = ((float)defaultTicksPerMove/adjustedTicksPerMove);
            Log.Message("factor: " + factor);
            Log.Message("result was: " + result);
            result -= Mathf.RoundToInt(tileCost * (1f- 1f/factor));
            Log.Message("result now is: " + result);
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
