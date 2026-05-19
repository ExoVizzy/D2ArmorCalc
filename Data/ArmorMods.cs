/*
*   FILE          : ArmorMods.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Static definitions for all general slot armor mods,
*                   organized by armor slot.
*/
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Data {
    public static class ArmorMods {
        //=====================================================================
        //Helmet Mods.
        //=====================================================================
        public static readonly ArmorMod SpecialFinder = new("Special Finder", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod SpecialScout = new("Special Scout", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod HeavyFinder = new("Heavy Finder", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod HeavyScout = new("Heavy Scout", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod Dynamo = new("Dynamo", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod AshesToAssets = new("Ashes to Assets", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod HandsOn = new("Hands-On", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod PowerPreservation = new("Power Preservation", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod RadiantLight = new("Radiant Light", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod PowerfulFriends = new("Powerful Friends", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod HarmonicSiphon = new("Harmonic Siphon", 1, ArmorSlot.Helmet);
        public static readonly ArmorMod KineticSiphon = new("Kinetic Siphon", 2, ArmorSlot.Helmet);
        public static readonly ArmorMod ArcSiphon = new("Arc Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod SolarSiphon = new("Solar Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod VoidSiphon = new("Void Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod StrandSiphon = new("Strand Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod StasisSiphon = new("Stasis Siphon", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod InFlightCompensator = new("In-Flight Compensator", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod HarmonicTargeting = new("Harmonic Targeting", 2, ArmorSlot.Helmet);
        public static readonly ArmorMod KineticTargeting = new("Kinetic Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod ArcTargeting = new("Arc Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod SolarTargeting = new("Solar Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod VoidTargeting = new("Void Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod StrandTargeting = new("Strand Targeting", 3, ArmorSlot.Helmet);
        public static readonly ArmorMod StasisTargeting = new("Stasis Targeting", 3, ArmorSlot.Helmet);
        //=====================================================================
        //Arms Mods.
        //=====================================================================
        public static readonly ArmorMod Fastball = new("Fastball", 1, ArmorSlot.Arms);
        public static readonly ArmorMod Firepower = new("Firepower", 3, ArmorSlot.Arms);
        public static readonly ArmorMod ImpactInduction = new("Impact Induction", 2, ArmorSlot.Arms);
        public static readonly ArmorMod BolsteringDetonation = new("Bolstering Detonation", 2, ArmorSlot.Arms);
        public static readonly ArmorMod GrenadeKickstart = new("Grenade Kickstart", 3, ArmorSlot.Arms);
        public static readonly ArmorMod HeavyHanded = new("Heavy Handed", 3, ArmorSlot.Arms);
        public static readonly ArmorMod MomentumTransfer = new("Momentum Transfer", 2, ArmorSlot.Arms);
        public static readonly ArmorMod FocusingStrike = new("Focusing Strike", 2, ArmorSlot.Arms);
        public static readonly ArmorMod MeleeKickstart = new("Melee Kickstart", 3, ArmorSlot.Arms);
        public static readonly ArmorMod ShieldBreakCharge = new("Shield Break Charge", 4, ArmorSlot.Arms);
        public static readonly ArmorMod HarmonicLoader = new("Harmonic Loader", 2, ArmorSlot.Arms);
        public static readonly ArmorMod KineticLoader = new("Kinetic Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod ArcLoader = new("Arc Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod SolarLoader = new("Solar Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod VoidLoader = new("Void Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod StrandLoader = new("Strand Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod StasisLoader = new("Stasis Loader", 3, ArmorSlot.Arms);
        public static readonly ArmorMod HarmonicDexterity = new("Harmonic Dexterity", 2, ArmorSlot.Arms);
        public static readonly ArmorMod KineticDexterity = new("Kinetic Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod ArcDexterity = new("Arc Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod SolarDexterity = new("Solar Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod VoidDexterity = new("Void Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod StrandDexterity = new("Strand Dexterity", 3, ArmorSlot.Arms);
        public static readonly ArmorMod StasisDexterity = new("Stasis Dexterity", 3, ArmorSlot.Arms);
        //=====================================================================
        //Chestplate Mods.
        //=====================================================================
        public static readonly ArmorMod HarmonicResist = new("Harmonic Resist", 1, ArmorSlot.Chestplate);
        public static readonly ArmorMod ArcResist = new("Arc Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod SolarResist = new("Solar Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod VoidResist = new("Void Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod StrandResist = new("Strand Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod StasisResist = new("Stasis Resist", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod MeleeResist = new("Melee Resist", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod SniperResist = new("Sniper Resist", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod ConcussiveDampener = new("Concussive Dampener", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod EmergencyReinforcement = new("Emergency Reinforcement", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod ChargedUp = new("Charged Up", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod LucentBlades = new("Lucent Blades", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingHarmonicAim = new("Unflinching Harmonic Aim", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingKineticAim = new("Unflinching Kinetic Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingArcAim = new("Unflinching Arc Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingSolarAim = new("Unflinching Solar Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingVoidAim = new("Unflinching Void Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingStrandAim = new("Unflinching Strand Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod UnflinchingStasisAim = new("Unflinching Stasis Aim", 3, ArmorSlot.Chestplate);
        public static readonly ArmorMod HarmonicAmmoGeneration = new("Harmonic Ammo Generation",1, ArmorSlot.Chestplate);
        public static readonly ArmorMod KineticAmmoGeneration = new("Kinetic Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod ArcAmmoGeneration = new("Arc Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod SolarAmmoGeneration = new("Solar Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod VoidAmmoGeneration = new("Void Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod StrandAmmoGeneration = new("Strand Ammo Generation", 2, ArmorSlot.Chestplate);
        public static readonly ArmorMod StasisAmmoGeneration = new("Stasis Ammo Generation", 2, ArmorSlot.Chestplate);
        //=====================================================================
        //Boots Mods.
        //=====================================================================
        public static readonly ArmorMod Recuperation = new("Recuperation", 1, ArmorSlot.Boots);
        public static readonly ArmorMod BetterAlready = new("Better Already", 1, ArmorSlot.Boots);
        public static readonly ArmorMod Innervation = new("Innervation", 1, ArmorSlot.Boots);
        public static readonly ArmorMod Invigoration = new("Invigoration", 1, ArmorSlot.Boots);
        public static readonly ArmorMod Insulation = new("Insulation", 1, ArmorSlot.Boots);
        public static readonly ArmorMod Absolution = new("Absolution", 3, ArmorSlot.Boots);
        public static readonly ArmorMod OrbsOfRestoration = new("Orbs of Restoration", 2, ArmorSlot.Boots);
        public static readonly ArmorMod EnhancedAthletics = new("Enhanced Athletics", 1, ArmorSlot.Boots);
        public static readonly ArmorMod StacksOnStacks = new("Stacks on Stacks", 4, ArmorSlot.Boots);
        public static readonly ArmorMod ElementalCharge = new("Elemental Charge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod KineticWeaponSurge = new("Kinetic Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod ArcWeaponSurge = new("Arc Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod SolarWeaponSurge = new("Solar Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod VoidWeaponSurge = new("Void Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod StrandWeaponSurge = new("Strand Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod StasisWeaponSurge = new("Stasis Weapon Surge", 3, ArmorSlot.Boots);
        public static readonly ArmorMod HarmonicHolster = new("Harmonic Holster", 1, ArmorSlot.Boots);
        public static readonly ArmorMod KineticHolster = new("Kinetic Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod ArcHolster = new("Arc Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod SolarHolster = new("Solar Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod VoidHolster = new("Void Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod StrandHolster = new("Strand Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod StasisHolster = new("Stasis Holster", 2, ArmorSlot.Boots);
        public static readonly ArmorMod HarmonicScavenger = new("Harmonic Scavenger", 2, ArmorSlot.Boots);
        public static readonly ArmorMod KineticScavenger = new("Kinetic Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod ArcScavenger = new("Arc Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod SolarScavenger = new("Solar Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod VoidScavenger = new("Void Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod StrandScavenger = new("Strand Scavenger", 3, ArmorSlot.Boots);
        public static readonly ArmorMod StasisScavenger = new("Stasis Scavenger", 3, ArmorSlot.Boots);
        //=====================================================================
        //Class Item Mods.
        //=====================================================================
        public static readonly ArmorMod Distribution = new("Distribution", 3, ArmorSlot.ClassItem);
        public static readonly ArmorMod Outreach = new("Outreach", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod Bomber = new("Bomber", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod UtilityKickstart = new("Utility Kickstart", 3, ArmorSlot.ClassItem);
        public static readonly ArmorMod TimeDilation = new("Time Dilation", 3, ArmorSlot.ClassItem);
        public static readonly ArmorMod PowerfulAttraction = new("Powerful Attraction", 2, ArmorSlot.ClassItem);
        public static readonly ArmorMod ProximityWard = new("Proximity Ward", 2, ArmorSlot.ClassItem);
        public static readonly ArmorMod RestorativeFinisher = new("Restorative Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod SpecialFinisher = new("Special Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod OneTwoFinisher = new("One-Two Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod BulwarkFinisher = new("Bulwark Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod HealthyFinisher = new("Healthy Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod SnaploadFinisher = new("Snapload Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod ExplosiveFinisher = new("Explosive Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod UtilityFinisher = new("Utility Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod BenevolentFinisher = new("Benevolent Finisher", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod EmpoweredFinish = new("Empowered Finish", 1, ArmorSlot.ClassItem);
        public static readonly ArmorMod Reaper = new("Reaper", 3, ArmorSlot.ClassItem);
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
        public static readonly ArmorMod[] All = [
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
        ];
    }
}