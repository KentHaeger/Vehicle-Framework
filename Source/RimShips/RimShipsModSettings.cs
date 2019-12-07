﻿using UnityEngine;
using Verse;

namespace RimShips
{
    public class RimshipsModSettings : ModSettings
    {
        public float beachMultiplier;
        public bool forceFactionCoastOption;
        public int forceFactionCoastRadius;

        public bool shuffledCannonFire;

        public bool debugDrawRegions;
        public bool debugDrawRegionLinks;
        public bool debugDrawRegionThings;
        public int coastRadius => forceFactionCoastOption ? forceFactionCoastRadius : 0;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref beachMultiplier, "beachMultiplier");
            Scribe_Values.Look(ref forceFactionCoastRadius, "forceFactionCoastRadius", 1);
            Scribe_Values.Look(ref forceFactionCoastOption, "forceFactionCoastOption", true);
            Scribe_Values.Look(ref shuffledCannonFire, "shuffledCannonFire", true);
            base.ExposeData();
        }
    }

    public class RimShipMod : Mod
    {
        public RimshipsModSettings settings;
        public static RimShipMod mod;

        public RimShipMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<RimshipsModSettings>();
            mod = this;
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            bool beachLarge = settings.beachMultiplier > 150f;
            listingStandard.Label(beachLarge ? "BeachGenMultiplierLarge".Translate(Mathf.Round(settings.beachMultiplier)) : "BeachGenMultiplier".Translate(Mathf.Round(settings.beachMultiplier)),
                -1f, beachLarge ? "BeachGenMultiplierLargeTooltip".Translate() : "BeachGenMultiplierTooltip".Translate());
            settings.beachMultiplier = listingStandard.Slider(settings.beachMultiplier, 0f, 200f);
            listingStandard.GapLine(16f);

            listingStandard.CheckboxLabeled("ForceSettlementCoastOption".Translate(), ref settings.forceFactionCoastOption, "ForceSettlementCoastTooltip".Translate());
            if(settings.forceFactionCoastOption)
            {
                listingStandard.Label("ForceSettlementCoast".Translate(Mathf.Round(settings.forceFactionCoastRadius)));
                settings.forceFactionCoastRadius = (int)listingStandard.Slider((float)settings.forceFactionCoastRadius, 0f, 10f);  
            }
            listingStandard.GapLine(16f);

            listingStandard.CheckboxLabeled("ShuffledCannonFire".Translate(), ref settings.shuffledCannonFire, "ShuffledCannonFireTooltip".Translate());

            if(Prefs.DevMode)
            {
                listingStandard.CheckboxLabeled("DebugDrawRegions".Translate(), ref settings.debugDrawRegions);
                listingStandard.CheckboxLabeled("DebugDrawRegionLinks".Translate(), ref settings.debugDrawRegionLinks);
                listingStandard.CheckboxLabeled("DebugDrawRegionThings".Translate(), ref settings.debugDrawRegionThings);
            }

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "RimShips".Translate();
        }
    }
}
