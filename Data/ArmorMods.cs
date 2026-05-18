/*
*   FILE          : ArmorMods.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Static definitions for all general slot armor mods,
*                   organized by armor slot.
*/
namespace D2ArmorCalc {
    public static class ArmorMods {
        //=====================================================================
        //Helmet Mods.
        //=====================================================================
        public static readonly ArmorMod SpecialFinder = new ArmorMod("Special Finder", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod SpecialScout = new ArmorMod("Special Scout", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod HeavyFinder = new ArmorMod("Heavy Finder", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod HeavyScout = new ArmorMod("Heavy Scout", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod Dynamo = new ArmorMod("Dynamo", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod AshesToAssets = new ArmorMod("Ashes to Assets", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod HandsOn = new ArmorMod("Hands-On", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod PowerPreservation = new ArmorMod("Power Preservation", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod RadiantLight = new ArmorMod("Radiant Light", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod PowerfulFriends = new ArmorMod("Powerful Friends", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod HarmonicSiphon = new ArmorMod("Harmonic Siphon", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod KineticSiphon = new ArmorMod("Kinetic Siphon", 2, ArmorSlot.Helmet);
        public static readonly ArmorMod ArcSiphon = new ArmorMod("Arc Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod SolarSiphon = new ArmorMod("Solar Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod VoidSiphon = new ArmorMod("Void Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod StrandSiphon = new ArmorMod("Strand Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod StasisSiphon = new ArmorMod("Stasis Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod InFlightCompensator = new ArmorMod("In-Flight Compensator", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod HarmonicTargeting = new ArmorMod("Harmonic Targeting", 2, ArmorSlot.Helmet);
        public static readonly ArmorMod KineticTargeting = new ArmorMod("Kinetic Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod ArcTargeting = new ArmorMod("Arc Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod SolarTargeting = new ArmorMod("Solar Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod VoidTargeting = new ArmorMod("Void Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod StrandTargeting = new ArmorMod("Strand Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod StasisTargeting = new ArmorMod("Stasis Targeting", 3, ArmorSlot.Helmet);
        //=====================================================================
        //Arms Mods.
        //=====================================================================
        public static readonly ArmorMod Fastball = new ArmorMod("Fastball", 1, ArmorSlot.Arms);
        public static readonly ArmorMod Firepower = new ArmorMod("Firepower", 3, ArmorSlot.Arms);
        public static readonly ArmorMod ImpactInduction = new ArmorMod("Impact Induction", 2, ArmorSlot.Arms);
        public static readonly ArmorMod BolsteringDetonation = new ArmorMod("Bolstering Detonation", 2, ArmorSlot.Arms);
        public static readonly ArmorMod GrenadeKickstart = new ArmorMod("Grenade Kickstart", 3, ArmorSlot.Arms);
        public static readonly ArmorMod HeavyHanded = new ArmorMod("Heavy Handed", 3, ArmorSlot.Arms);
        public static readonly ArmorMod MomentumTransfer = new ArmorMod("Momentum Transfer", 2, ArmorSlot.Arms);
        public static readonly ArmorMod FocusingStrike = new ArmorMod("Focusing Strike", 2, ArmorSlot.Arms);
        public static readonly ArmorMod MeleeKickstart = new ArmorMod("Melee Kickstart", 3, ArmorSlot.Arms);
        public static readonly ArmorMod ShieldBreakCharge = new ArmorMod("Shield Break Charge", 4, ArmorSlot.Arms);
        public static readonly ArmorMod HarmonicLoader = new ArmorMod("Harmonic Loader", 2, ArmorSlot.Arms);
        public static readonly ArmorMod KineticLoader = new ArmorMod("Kinetic Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod ArcLoader = new ArmorMod("Arc Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod SolarLoader = new ArmorMod("Solar Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod VoidLoader = new ArmorMod("Void Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod StrandLoader = new ArmorMod("Strand Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod StasisLoader = new ArmorMod("Stasis Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod HarmonicDexterity = new ArmorMod("Harmonic Dexterity", 2, ArmorSlot.Arms);
        public static readonly ArmorMod KineticDexterity = new ArmorMod("Kinetic Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod ArcDexterity = new ArmorMod("Arc Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod SolarDexterity = new ArmorMod("Solar Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod VoidDexterity = new ArmorMod("Void Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod StrandDexterity = new ArmorMod("Strand Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod StasisDexterity = new ArmorMod("Stasis Dexterity", 3, ArmorSlot.Arms);
        //=====================================================================
        //Chestplate Mods.
        //=====================================================================
        public static readonly ArmorMod HarmonicResist = new ArmorMod("Harmonic Resist", 1, ArmorSlot.Chestplate);
        public static readonly ArmorMod ArcResist = new ArmorMod("Arc Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod SolarResist = new ArmorMod("Solar Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod VoidResist = new ArmorMod("Void Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod StrandResist = new ArmorMod("Strand Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod StasisResist = new ArmorMod("Stasis Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod MeleeResist = new ArmorMod("Melee Resist", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod SniperResist = new ArmorMod("Sniper Resist", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod ConcussiveDampener = new ArmorMod("Concussive Dampener", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod EmergencyReinforcement = new ArmorMod("Emergency Reinforcement", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod ChargedUp = new ArmorMod("Charged Up", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod LucentBlades = new ArmorMod("Lucent Blades", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingHarmonicAim = new ArmorMod("Unflinching Harmonic Aim", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingKineticAim = new ArmorMod("Unflinching Kinetic Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingArcAim = new ArmorMod("Unflinching Arc Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingSolarAim = new ArmorMod("Unflinching Solar Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingVoidAim = new ArmorMod("Unflinching Void Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingStrandAim = new ArmorMod("Unflinching Strand Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingStasisAim = new ArmorMod("Unflinching Stasis Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod HarmonicAmmoGeneration = new ArmorMod("Harmonic Ammo Generation",1, ArmorSlot.Chestplate);
        public static readonly ArmorMod KineticAmmoGeneration = new ArmorMod("Kinetic Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod ArcAmmoGeneration = new ArmorMod("Arc Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod SolarAmmoGeneration = new ArmorMod("Solar Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod VoidAmmoGeneration = new ArmorMod("Void Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod StrandAmmoGeneration = new ArmorMod("Strand Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod StasisAmmoGeneration = new ArmorMod("Stasis Ammo Generation", 2, ArmorSlot.Chestplate);
        //=====================================================================
        //Boots Mods.
        //=====================================================================
        public static readonly ArmorMod Recuperation = new ArmorMod("Recuperation", 1, ArmorSlot.Boots);
        public static readonly ArmorMod BetterAlready = new ArmorMod("Better Already", 1, ArmorSlot.Boots);
        public static readonly ArmorMod Innervation = new ArmorMod("Innervation", 1, ArmorSlot.Boots);
        public static readonly ArmorMod Invigoration = new ArmorMod("Invigoration", 1, ArmorSlot.Boots);
        public static readonly ArmorMod Insulation = new ArmorMod("Insulation", 1, ArmorSlot.Boots);
        public static readonly ArmorMod Absolution = new ArmorMod("Absolution", 3, ArmorSlot.Boots);
        public static readonly ArmorMod OrbsOfRestoration = new ArmorMod("Orbs of Restoration", 2, ArmorSlot.Boots);
        public static readonly ArmorMod EnhancedAthletics = new ArmorMod("Enhanced Athletics", 1, ArmorSlot.Boots);
        public static readonly ArmorMod StacksOnStacks = new ArmorMod("Stacks on Stacks", 4, ArmorSlot.Boots);
        public static readonly ArmorMod ElementalCharge = new ArmorMod("Elemental Charge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod KineticWeaponSurge = new ArmorMod("Kinetic Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod ArcWeaponSurge = new ArmorMod("Arc Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod SolarWeaponSurge = new ArmorMod("Solar Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod VoidWeaponSurge = new ArmorMod("Void Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod StrandWeaponSurge = new ArmorMod("Strand Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod StasisWeaponSurge = new ArmorMod("Stasis Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod HarmonicHolster = new ArmorMod("Harmonic Holster", 1, ArmorSlot.Boots);
        public static readonly ArmorMod KineticHolster = new ArmorMod("Kinetic Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod ArcHolster = new ArmorMod("Arc Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod SolarHolster = new ArmorMod("Solar Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod VoidHolster = new ArmorMod("Void Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod StrandHolster = new ArmorMod("Strand Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod StasisHolster = new ArmorMod("Stasis Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod HarmonicScavenger = new ArmorMod("Harmonic Scavenger", 2, ArmorSlot.Boots);
        public static readonly ArmorMod KineticScavenger = new ArmorMod("Kinetic Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod ArcScavenger = new ArmorMod("Arc Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod SolarScavenger = new ArmorMod("Solar Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod VoidScavenger = new ArmorMod("Void Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod StrandScavenger = new ArmorMod("Strand Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod StasisScavenger = new ArmorMod("Stasis Scavenger", 3, ArmorSlot.Boots);
        //=====================================================================
        //Class Item Mods.
        //=====================================================================
        public static readonly ArmorMod Distribution = new ArmorMod("Distribution", 3, ArmorSlot.ClassItem);
        public static readonly ArmorMod Outreach = new ArmorMod("Outreach", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod Bomber = new ArmorMod("Bomber", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod UtilityKickstart = new ArmorMod("Utility Kickstart", 3, ArmorSlot.ClassItem);
        public static readonly ArmorMod TimeDilation = new ArmorMod("Time Dilation", 3, ArmorSlot.ClassItem);
        public static readonly ArmorMod PowerfulAttraction = new ArmorMod("Powerful Attraction", 2, ArmorSlot.ClassItem);
        public static readonly ArmorMod ProximityWard = new ArmorMod("Proximity Ward", 2, ArmorSlot.ClassItem);
        public static readonly ArmorMod RestorativeFinisher = new ArmorMod("Restorative Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod SpecialFinisher = new ArmorMod("Special Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod OneTwoFinisher = new ArmorMod("One-Two Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod BulwarkFinisher = new ArmorMod("Bulwark Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod HealthyFinisher = new ArmorMod("Healthy Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod SnaploadFinisher = new ArmorMod("Snapload Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod ExplosiveFinisher = new ArmorMod("Explosive Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod UtilityFinisher = new ArmorMod("Utility Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod BenevolentFinisher = new ArmorMod("Benevolent Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod EmpoweredFinish = new ArmorMod("Empowered Finish", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod Reaper = new ArmorMod("Reaper", 3, ArmorSlot.ClassItem);
        //=====================================================================
        //Slot Lookup.
        //=====================================================================
        /*
        Method        : GetModsBySlot
        Description   : Returns all armor mods available for given armor slot.
        Parameters    : ArmorSlot slot : Armor slot to filter by.
        Return Values : ArmorMod[]     : Array of all mods available in slot.
        */
        public static ArmorMod[] GetModsBySlot(ArmorSlot slot){
            return Array.FindAll(All, m => m.Slot == slot);
        }
        //All armor mods as a flat array for iteration.
        public static readonly ArmorMod[] All = {
            //Helmet.
            SpecialFinder, SpecialScout, HeavyFinder, HeavyScout, Dynamo,
            AshesToAssets, HandsOn, PowerPreservation, RadiantLight, PowerfulFriends,
            HarmonicSiphon, KineticSiphon, ArcSiphon, SolarSiphon, VoidSiphon,
            StrandSiphon, StasisSiphon, InFlightCompensator, HarmonicTargeting,
            KineticTargeting, ArcTargeting, SolarTargeting, VoidTargeting,
            StrandTargeting, StasisTargeting,
            //Arms.
            Fastball, Firepower, ImpactInduction, BolsteringDetonation, GrenadeKickstart,
            HeavyHanded, MomentumTransfer, FocusingStrike, MeleeKickstart, ShieldBreakCharge,
            HarmonicLoader, KineticLoader, ArcLoader, SolarLoader, VoidLoader,
            StrandLoader, StasisLoader, HarmonicDexterity, KineticDexterity, ArcDexterity,
            SolarDexterity, VoidDexterity, StrandDexterity, StasisDexterity,
            //Chestplate.
            HarmonicResist, ArcResist, SolarResist, VoidResist, StrandResist,
            StasisResist, MeleeResist, SniperResist, ConcussiveDampener, EmergencyReinforcement,
            ChargedUp, LucentBlades, UnflinchingHarmonicAim, UnflinchingKineticAim,
            UnflinchingArcAim, UnflinchingSolarAim, UnflinchingVoidAim, UnflinchingStrandAim,
            UnflinchingStasisAim, HarmonicAmmoGeneration, KineticAmmoGeneration,
            ArcAmmoGeneration, SolarAmmoGeneration, VoidAmmoGeneration,
            StrandAmmoGeneration, StasisAmmoGeneration,
            //Boots.
            Recuperation, BetterAlready, Innervation, Invigoration, Insulation,
            Absolution, OrbsOfRestoration, EnhancedAthletics, StacksOnStacks,
            ElementalCharge, KineticWeaponSurge, ArcWeaponSurge, SolarWeaponSurge,
            VoidWeaponSurge, StrandWeaponSurge, StasisWeaponSurge, HarmonicHolster,
            KineticHolster, ArcHolster, SolarHolster, VoidHolster, StrandHolster,
            StasisHolster, HarmonicScavenger, KineticScavenger, ArcScavenger,
            SolarScavenger, VoidScavenger, StrandScavenger, StasisScavenger,
            //Class Item.
            Distribution, Outreach, Bomber, UtilityKickstart, TimeDilation,
            PowerfulAttraction, ProximityWard, RestorativeFinisher, SpecialFinisher,
            OneTwoFinisher, BulwarkFinisher, HealthyFinisher, SnaploadFinisher,
            ExplosiveFinisher, UtilityFinisher, BenevolentFinisher, EmpoweredFinish, Reaper
        };
    }
}