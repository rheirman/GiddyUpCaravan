using GiddyUpCore;
using GiddyUpCore.Storage;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(TransferableOneWayWidget), "DoRow")]
    static class TransferableOneWayWidget_DoRow
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Log.Message(typeof(TooltipHandler).GetMethod("TipRegion", new Type[] { typeof(Rect), typeof(TipSignal) }).ToString());
            var instructionsList = new List<CodeInstruction>(instructions);
            bool flag = false;
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                yield return instruction;
                if (instructionsList[i].operand == typeof(TooltipHandler).GetMethod("TipRegion", new Type[] { typeof(Rect), typeof(TipSignal) }))
                {
                    flag = true;
                }
                if (flag && instructionsList[i].opcode == OpCodes.Stloc_0)
                {

                    yield return new CodeInstruction(OpCodes.Ldarg_0);//Load instance
                    yield return new CodeInstruction(OpCodes.Ldloc_0);//load count local variable
                    yield return new CodeInstruction(OpCodes.Ldarg_1);//Load rectangle
                    yield return new CodeInstruction(OpCodes.Ldarg_2);//Load trad
                    yield return new CodeInstruction(OpCodes.Call, typeof(TransferableOneWayWidget_DoRow).GetMethod("addMountSelector"));//Injected code     
                    yield return new CodeInstruction(OpCodes.Stloc_0);//store count local variable

                }
            }
        }
        public static float addMountSelector(TransferableOneWayWidget widget, float num, Rect rect, TransferableOneWay trad)
        {
            float buttonWidth = 150f;
            Pawn animal = trad.AnyThing as Pawn;
            if (animal == null || !animal.RaceProps.Animal)
            {
                return num; //not an animal, return; 
            }

            ExtendedPawnData animalData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(animal);
            Rect buttonRect = new Rect(num-buttonWidth, 0f, buttonWidth, rect.height);
            Text.Anchor = TextAnchor.MiddleLeft;

            List<FloatMenuOption> list = new List<FloatMenuOption>();
            List<Pawn> pawns = Dialog_FormCaravan.AllSendablePawns(Find.VisibleMap, false);
            string buttonText = "GU_Car_Set_Rider".Translate();

            bool canMount = true;
            if (animal.ageTracker.CurLifeStageIndex != animal.RaceProps.lifeStageAges.Count - 1)
            {
                //opts.Add(new FloatMenuOption("BM_NotFullyGrown".Translate(), null, MenuOptionPriority.Low));
                buttonText = "GU_Car_NotFullyGrown".Translate();
                canMount = false;
            }
            if (!(animal.training != null && animal.training.IsCompleted(TrainableDefOf.Obedience)))
            {
                //opts.Add(new FloatMenuOption("BM_NeedsObedience".Translate(), null, MenuOptionPriority.Low));
                buttonText = "GU_Car_NeedsObedience".Translate();
                canMount = false;
            }
            bool found = GiddyUpCore.Base.animalSelecter.Value.InnerList.TryGetValue(animal.def.defName, out AnimalRecord value);
            if (found && !value.isSelected)
            {
                buttonText = "GU_Car_NotInModOptions".Translate();
                canMount = false;
            }
            if (animalData.caravanRider != null)
            {
                buttonText = animalData.caravanRider.Name.ToStringShort;
            }
            if (!canMount)
            {
                Widgets.ButtonText(buttonRect, buttonText, false, false, false);
            }
            else if (Widgets.ButtonText(buttonRect, buttonText, true, false, true))
            {
                foreach (Pawn pawn in pawns)
                {
                    if (pawn.RaceProps.Humanlike)
                    {
                        ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
                        if(pawnData.caravanMount != null)
                        {
                            continue;
                        }
                        list.Add(new FloatMenuOption(pawn.Name.ToStringShort, delegate
                        {
                            {

                                if(animalData.caravanRider != null)
                                {
                                    ExtendedPawnData riderData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(animalData.caravanRider);
                                    riderData.caravanMount = null;
                                }

                                pawnData.caravanMount = animal;
                                animalData.caravanRider = pawn;
                            }
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));

                    }
                }
                list.Add(new FloatMenuOption("GU_Car_No_Rider".Translate(), delegate
                {
                    {
                        if (animalData.caravanRider != null)
                        {
                            ExtendedPawnData riderData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(animalData.caravanRider);
                            riderData.caravanMount = null;
                        }
                        animalData.caravanRider = null;
                    }
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                Find.WindowStack.Add(new FloatMenu(list));
            }

            return num - buttonWidth;
        }
    }

    [HarmonyPatch(typeof(TransferableOneWayWidget), "DrawMass")]
    [HarmonyPatch(new Type[] { typeof(Rect), typeof(TransferableOneWay), typeof(float) })]
    static class TransferableOneWayWidget_DrawMass
    {
        static bool Prefix(TransferableOneWayWidget __instance, Rect rect, TransferableOneWay trad)
        {
            Pawn pawn = trad.AnyThing as Pawn;
            if (pawn == null)
            {
                return true;
            }
            ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
            if (pawnData.caravanRider == null)
            {
                return true;
            }
            else
            {
                float cap = MassUtility.Capacity(pawn);
                float gearMass = MassUtility.GearMass(pawn);
                IgnorePawnsInventoryMode ignore = Traverse.Create(__instance).Field("ignorePawnInventoryMass").GetValue<IgnorePawnsInventoryMode>();
                float invMass = (!InventoryCalculatorsUtility.ShouldIgnoreInventoryOf(pawn, ignore)) ? MassUtility.InventoryMass(pawn) : 0f;
                float num = cap - gearMass - invMass;
                num -= pawnData.caravanRider.GetStatValue(StatDefOf.Mass);
                Log.Message("mass: " + num);
                if(num < 0)
                {
                    num = 0;
                }

                Color oldColor = GUI.color;
                if (num > 0f)
                {
                    GUI.color = Color.green;
                }
                else if (num < 0f)
                {
                    GUI.color = Color.red;
                }
                else
                {
                    GUI.color = Color.gray;
                }
                Widgets.Label(rect, num.ToStringMassOffset());
                object[] types = {typeof(TransferableOneWay), typeof(float), typeof(float), typeof(float), typeof(int)};
                object[] parms = { trad, cap, 0f, gearMass, invMass };
                TooltipHandler.TipRegion(rect, () => Traverse.Create(__instance).Method("GetPawnMassTip", types, parms).GetValue<String>(), trad.GetHashCode() * 59);
                GUI.color = oldColor;
                return false;
            }
        }

    }

 }
