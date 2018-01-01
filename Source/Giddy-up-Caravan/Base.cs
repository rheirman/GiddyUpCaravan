using GiddyUpCore.Storage;
using HugsLib;
using HugsLib.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace GiddyUpCaravan
{
    public class Base : ModBase
    {
        internal static SettingHandle<int> completeCaravanBonus;
        internal static SettingHandle<int> incompleteCaravanBonusCap;


        internal static SettingHandle<int> visitorMountChance;
        internal static SettingHandle<int> visitorMountChanceTribal;

        internal static SettingHandle<int> inBiomeWeight;
        internal static SettingHandle<int> outBiomeWeight;
        internal static SettingHandle<int> nonWildWeight;

        private int minPercentage = 0;
        private int maxPercentage = 100;

        public override string ModIdentifier
        {
            get { return "GiddyUpCaravan"; }
        }

        public static ExtendedDataStorage GetExtendedDataStorage()
        {
            return GiddyUpCore.Base.Instance.GetExtendedDataStorage();
        }
        public override void DefsLoaded()
        {
            completeCaravanBonus = Settings.GetHandle<int>("completeCaravanBonus", "GU_Car_CompleteCaravanBonus_Title".Translate(), "GU_Car_CompleteCaravanBonus_Description".Translate(), 80, Validators.IntRangeValidator(0, 200));
            incompleteCaravanBonusCap = Settings.GetHandle<int>("incompleteCaravanBonusCap", "GU_Car_incompleteCaravanBonusCap_Title".Translate(), "GU_Car_incompleteCaravanBonusCap_Description".Translate(), 25, Validators.IntRangeValidator(0, 200));

            visitorMountChance = Settings.GetHandle<int>("visitorMountChance", "GU_Car_visitorMountChance_Title".Translate(), "GU_Car_visitorMountChance_Description".Translate(), 20, Validators.IntRangeValidator(minPercentage, maxPercentage));
            visitorMountChanceTribal = Settings.GetHandle<int>("visitorMountChanceTribal", "GU_Car_visitorMountChanceTribal_Title".Translate(), "GU_Car_visitorMountChanceTribal_Description".Translate(), 40, Validators.IntRangeValidator(minPercentage, maxPercentage));

            inBiomeWeight = Settings.GetHandle<int>("inBiomeWeight", "GU_Car_InBiomeWeight_Title".Translate(), "GU_Car_InBiomeWeight_Description".Translate(), 70, Validators.IntRangeValidator(minPercentage, maxPercentage));
            outBiomeWeight = Settings.GetHandle<int>("outBiomeWeight", "GU_Car_OutBiomeWeight_Title".Translate(), "GU_Car_OutBiomeWeight_Description".Translate(), 15, Validators.IntRangeValidator(minPercentage, maxPercentage));
            nonWildWeight = Settings.GetHandle<int>("nonWildWeight", "GU_Car_NonWildWeight_Title".Translate(), "GU_Car_NonWildWeight_Description".Translate(), 15, Validators.IntRangeValidator(minPercentage, maxPercentage));

        }
    }


}
