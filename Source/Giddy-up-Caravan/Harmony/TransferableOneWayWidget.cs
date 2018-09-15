using GiddyUpCore;
using GiddyUpCore.Storage;
using GiddyUpCore.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    flag = false;
                }
            }
        }

        public static float addMountSelector(TransferableOneWayWidget widget, float num, Rect rect, TransferableOneWay trad)
        {


            float buttonWidth = 150f;


            Pawn pawn = trad.AnyThing as Pawn;
            if (pawn == null)
            {
                return num; //not an animal, return; 
            }
            
            Rect buttonRect = new Rect(num - buttonWidth, 0f, buttonWidth, rect.height);


            //Reflection is needed here to access the private struct inside TransferableOneWayWidget
            Type sectionType = Traverse.Create(widget).Type("Section").GetValue<Type>();
            IList sections = Traverse.Create(widget).Field("sections").GetValue<IList>();

            object section = sections[0]; //section 0 only yields pawns, which are needed in this case

            List<TransferableOneWay> cachedTransferables = sectionType.GetField("cachedTransferables").GetValue(section) as List<TransferableOneWay>;

            List<Pawn> pawns = new List<Pawn>();
            if (cachedTransferables != null)
            {
                foreach (TransferableOneWay tow in cachedTransferables)
                {
                    //Log.Message(tow.AnyThing.Label);
                    Pawn towPawn = tow.AnyThing as Pawn;
                    if (towPawn != null)
                    {
                        pawns.Add(tow.AnyThing as Pawn);
                    }
                }
                //It quacks like a duck, so it is one!
            }
            setSelectedForCaravan(pawn, trad);
            if (pawn.RaceProps.Animal && pawns.Count > 0)
            {
                handleAnimal(num, buttonRect, pawn, pawns, trad);
            }
            else
            {
                return num;
            }


            return num - buttonWidth;
        }



        private static void setSelectedForCaravan(Pawn pawn, TransferableOneWay trad)
        {

            ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);

            if (trad.CountToTransfer == 0) //unset pawndata when pawn is not selected for caravan. 
            {
                pawnData.selectedForCaravan = false;
                if (pawnData.caravanMount != null)
                {
                    unsetDataForRider(pawnData);
                }
                if (pawnData.caravanRider != null)
                {
                    unsetDataForMount(pawnData);
                }
            }
            if(pawnData.caravanMount != null && (pawnData.caravanMount.Dead || pawnData.caravanMount.Downed))
            {
                unsetDataForRider(pawnData);
            }

            if (trad.CountToTransfer > 0)
            {
                pawnData.selectedForCaravan = true;
            }
        }

        private static void unsetDataForRider(ExtendedPawnData pawnData)
        {
            ExtendedPawnData mountData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawnData.caravanMount);
            mountData.caravanRider = null;
            pawnData.caravanMount = null;
        }

        private static void unsetDataForMount(ExtendedPawnData pawnData)
        {
            ExtendedPawnData riderData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawnData.caravanRider);
            riderData.caravanMount = null;
            pawnData.caravanRider = null;
        }




        private static void handleAnimal(float num, Rect buttonRect, Pawn animal, List<Pawn> pawns, TransferableOneWay trad)
        {
            ExtendedPawnData animalData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(animal);
            Text.Anchor = TextAnchor.MiddleLeft;

            List<FloatMenuOption> list = new List<FloatMenuOption>();

            string buttonText = "GU_Car_Set_Rider".Translate();

            bool canMount = true;



            if (!animalData.selectedForCaravan)
            {
                buttonText = "GU_Car_AnimalNotSelected".Translate();
                canMount = false;
            }

            bool isMountable = IsMountableUtility.isMountable(animal, out IsMountableUtility.Reason reason);
            if (!isMountable)
            {
                if (reason == IsMountableUtility.Reason.NotFullyGrown)
                {
                    buttonText = "GU_Car_NotFullyGrown".Translate();
                    canMount = false;
                }
                if (reason == IsMountableUtility.Reason.NeedsObedience)
                {
                    buttonText = "GU_Car_NeedsObedience".Translate();
                    canMount = false;
                }
                if (reason == IsMountableUtility.Reason.NotInModOptions)
                {
                    buttonText = "GU_Car_NotInModOptions".Translate();
                    canMount = false;
                }
            }

            if (animalData.caravanRider != null)
            {
                ExtendedPawnData riderData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(animalData.caravanRider);
                if (riderData.selectedForCaravan)
                {
                    buttonText = animalData.caravanRider.Name.ToStringShort;
                }
            }
            if (!canMount)
            {
                Widgets.ButtonText(buttonRect, buttonText, false, false, false);
            }
            else if (Widgets.ButtonText(buttonRect, buttonText, true, false, true))
            {
                foreach (Pawn pawn in pawns)
                {
                    if (pawn.IsColonist)
                    {
                        ExtendedPawnData pawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(pawn);
                        if (!pawnData.selectedForCaravan)
                        {
                            list.Add(new FloatMenuOption(pawn.Name.ToStringShort + " (" + "GU_Car_PawnNotSelected".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                            continue;
                        }

                        if (pawnData.caravanMount != null)
                        {
                            continue;
                        }
                        list.Add(new FloatMenuOption(pawn.Name.ToStringShort, delegate
                        {
                            {
                                if (animalData.caravanRider != null)
                                {
                                    ExtendedPawnData riderData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(animalData.caravanRider);
                                    riderData.caravanMount = null;
                                }

                                pawnData.caravanMount = animal;
                                animalData.caravanRider = pawn;
                                Traverse.Create(trad).Property("CountToTransfer").SetValue(-1); //Setting this to -1 will make sure total weight is calculated again. it's set back to 1 shortly after
                                Log.Message("setting CountToTransfer to -1");
                                animalData.selectedForCaravan = true;
                            }
                        }, MenuOptionPriority.High, null, null, 0f, null, null));
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
                        Traverse.Create(trad).Property("CountToTransfer").SetValue(-1); //Setting this to -1 will make sure total weight is calculated again. it's set back to 1 shortly after
                        Log.Message("setting CountToTransfer to -1");
                        animalData.selectedForCaravan = true;

                    }
                }, MenuOptionPriority.Low, null, null, 0f, null, null));
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }


    }

    
    //This code makes sure total pack weight is refreshed after a rider is set for an animal. 
    //CountToTransfer with -1 is used as a flag here, indicating that weight should be recalculated. Unfortunately I couldn't come up with a cleaner way to do this without completely disabling caching. 
    [HarmonyPatch(typeof(TransferableOneWayWidget), "FillMainRect")]
    static class TransferableOneWayWidget_FillMainRect
    {
        static void Postfix(TransferableOneWayWidget __instance, ref bool anythingChanged)
        {
            //Reflection is needed here to access the private struct inside TransferableOneWayWidget
            Type sectionType = Traverse.Create(__instance).Type("Section").GetValue<Type>();
            IList sections = Traverse.Create(__instance).Field("sections").GetValue<IList>();
            foreach (object s in sections)
            {
                List<TransferableOneWay> tf = sectionType.GetField("cachedTransferables").GetValue(s) as List<TransferableOneWay>;
            }
            if (sections.Count < 4)
            {
                return;
            }
            object section = sections[3]; //section 3 only yields animals, which are needed in this case

            List<TransferableOneWay> cachedTransferables = sectionType.GetField("cachedTransferables").GetValue(section) as List<TransferableOneWay>;
            if (cachedTransferables != null)
            {
                foreach (TransferableOneWay tow in cachedTransferables)
                {
                    Pawn towPawn = tow.AnyThing as Pawn;
                    if (towPawn == null)
                    {
                        continue;
                    }
                    if (tow.CountToTransfer == -1)
                    {
                        ExtendedPawnData PawnData = GiddyUpCore.Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(towPawn);
                        if (PawnData.selectedForCaravan == true)
                        {
                            anythingChanged = true;
                            Traverse.Create(tow).Property("CountToTransfer").SetValue(1);
                        }
                    }
                }
            }
        }
    }
}
