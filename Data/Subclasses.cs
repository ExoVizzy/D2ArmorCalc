/*
*   FILE          : Subclasses.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Static definitions for all subclasses across all three
*                   classes, including fragments, aspects, supers, melees,
*                   grenades, class abilities, & jumps.
*/
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Data {
    //Static definitions for all subclass data.
    public static class Subclasses {
        //=====================================================================
        //Shared Data.
        //=====================================================================
        private static readonly string[] WarlockJumps = ["Strafe Glide", "Burst Glide", "Balanced Glide"];
        private static readonly string[] WarlockBlink = ["Strafe Glide", "Burst Glide", "Balanced Glide", "Blink"];
        private static readonly string[] WarlockClassAbilities = ["Healing Rift", "Empowering Rift"];
        private static readonly string[] WarlockSolarClassAbilities = ["Healing Rift", "Empowering Rift", "Phoenix Dive"];
        private static readonly string[] TitanJumps = ["High Lift", "Strafe Lift", "Catapult Lift"];
        private static readonly string[] TitanClassAbilities = ["Rally Barricade", "Towering Barricade"];
        private static readonly string[] TitanArcClassAbilities = ["Rally Barricade", "Towering Barricade", "Thruster"];
        private static readonly string[] HunterJumps = ["High Jump", "Strafe Jump", "Triple Jump"];
        private static readonly string[] HunterBlink = ["High Jump", "Strafe Jump", "Triple Jump", "Blink"];
        private static readonly string[] HunterClassAbilities = ["Marksmans Dodge", "Gamblers Dodge"];
        private static readonly string[] HunterSolarClassAbilities = ["Marksmans Dodge", "Gamblers Dodge", "Acrobats Dodge"];
        //=====================================================================
        //Arc Fragments.
        //=====================================================================
        private static Fragment[] GetArcFragments(PlayerClass playerClass){
            Stat focusStat = playerClass switch {
                PlayerClass.Titan => Stat.Health,
                PlayerClass.Hunter => Stat.Weapons,
                _ => Stat.Class //Warlock.
            };
            return [
                new Fragment("Brilliance", "Arc", new StatChange(Stat.Super, +10)),
                new Fragment("Discharge", "Arc", new StatChange(Stat.Melee, -10)),
                new Fragment("Feedback", "Arc", new StatChange(Stat.Health, +10)),
                new Fragment("Focus", "Arc", new StatChange(focusStat, -10)),
                new Fragment("Protection", "Arc", new StatChange(Stat.Melee, +10)),
                new Fragment("Shock", "Arc", new StatChange(Stat.Grenade, -10)),
                new Fragment("Volts", "Arc", new StatChange(Stat.Class, +10)),
                new Fragment("Amplitude", "Arc"), new Fragment("Beacons", "Arc"),
                new Fragment("Frequency", "Arc"), new Fragment("Haste", "Arc"),
                new Fragment("Instinct", "Arc"), new Fragment("Ions", "Arc"),
                new Fragment("Magnitude", "Arc"), new Fragment("Momentum", "Arc"),
                new Fragment("Recharge", "Arc")];
        }
        //=====================================================================
        //Solar Fragments.
        //=====================================================================
        private static readonly Fragment[] SolarFragments = [
            new Fragment("Beams", "Solar", new StatChange(Stat.Super, +10)),
            new Fragment("Benevolence", "Solar", new StatChange(Stat.Grenade, -10)),
            new Fragment("Char", "Solar", new StatChange(Stat.Grenade, +10)),
            new Fragment("Combustion", "Solar", new StatChange(Stat.Melee, +10)),
            new Fragment("Empyrean", "Solar", new StatChange(Stat.Health, -10)),
            new Fragment("Eruption", "Solar", new StatChange(Stat.Melee, +10)),
            new Fragment("Mercy", "Solar", new StatChange(Stat.Health, +10)),
            new Fragment("Searing", "Solar", new StatChange(Stat.Class, +10)),
            new Fragment("Tempering", "Solar", new StatChange(Stat.Class, -10)),
            new Fragment("Torches", "Solar", new StatChange(Stat.Grenade, -10)),
            new Fragment("Wonder", "Solar", new StatChange(Stat.Health, +10)),
            new Fragment("Ashes", "Solar"), new Fragment("Blistering", "Solar"),
            new Fragment("Resolve", "Solar"), new Fragment("Singing", "Solar"),
            new Fragment("Solace", "Solar")];
        //=====================================================================
        //Void Fragments.
        //=====================================================================
        private static Fragment[] GetVoidFragments(PlayerClass playerClass){
            Stat persistenceStat = playerClass switch {
                PlayerClass.Titan => Stat.Health,
                PlayerClass.Hunter => Stat.Weapons,
                _ => Stat.Class //Warlock.
            };
            return [
                new Fragment("Dilation", "Void", new StatChange(Stat.Weapons, +10), new StatChange(Stat.Super, +10)),
                new Fragment("Domineering", "Void", new StatChange(Stat.Grenade, +10)),
                new Fragment("Exchange", "Void", new StatChange(Stat.Melee, +10)),
                new Fragment("Expulsion", "Void", new StatChange(Stat.Super, +10)),
                new Fragment("Instability", "Void", new StatChange(Stat.Melee, +10)),
                new Fragment("Leeching", "Void", new StatChange(Stat.Health, +10)),
                new Fragment("Obscurity", "Void", new StatChange(Stat.Class, +10)),
                new Fragment("Persistence", "Void", new StatChange(persistenceStat, -10)),
                new Fragment("Provision", "Void", new StatChange(Stat.Grenade, +10)),
                new Fragment("Starvation", "Void", new StatChange(Stat.Class, -10)),
                new Fragment("Undermining", "Void", new StatChange(Stat.Grenade, -10)),
                new Fragment("Cessation", "Void"), new Fragment("Harvest", "Void"),
                new Fragment("Remnants", "Void"), new Fragment("Reprisal", "Void"),
                new Fragment("Vigilance", "Void")];
        }
        //=====================================================================
        //Stasis Fragments.
        //=====================================================================
        private static readonly Fragment[] StasisFragments = [
            new Fragment("Bonds", "Stasis", new StatChange(Stat.Super, -10)),
            new Fragment("Chains", "Stasis", new StatChange(Stat.Class, +10)),
            new Fragment("Conduction", "Stasis", new StatChange(Stat.Health, +10), new StatChange(Stat.Super, +10)),
            new Fragment("Durance", "Stasis", new StatChange(Stat.Melee, +10)),
            new Fragment("Hunger", "Stasis", new StatChange(Stat.Melee, -20)),
            new Fragment("Impetus", "Stasis", new StatChange(Stat.Health, +10)),
            new Fragment("Torment", "Stasis", new StatChange(Stat.Grenade, -10)),
            new Fragment("Chill", "Stasis"), new Fragment("Fissures", "Stasis"),
            new Fragment("Fractures", "Stasis"), new Fragment("Hedrons", "Stasis"),
            new Fragment("Refraction", "Stasis"), new Fragment("Rending", "Stasis"),
            new Fragment("Reversal", "Stasis"), new Fragment("Rime", "Stasis"),
            new Fragment("Shards", "Stasis")];
        //=====================================================================
        //Strand Fragments.
        //=====================================================================
        private static readonly Fragment[] StrandFragments = [
            new Fragment("Ascent", "Strand", new StatChange(Stat.Weapons, +10)),
            new Fragment("Binding", "Strand", new StatChange(Stat.Health, +10)),
            new Fragment("Evolution", "Strand", new StatChange(Stat.Super, +10)),
            new Fragment("Finality", "Strand", new StatChange(Stat.Class, +10)),
            new Fragment("Fury", "Strand", new StatChange(Stat.Melee, -10)),
            new Fragment("Generation", "Strand", new StatChange(Stat.Grenade, -10)),
            new Fragment("Propagation", "Strand", new StatChange(Stat.Melee, +10)),
            new Fragment("Transmutation", "Strand", new StatChange(Stat.Melee, +10)),
            new Fragment("Warding", "Strand", new StatChange(Stat.Health, -10)),
            new Fragment("Continuity", "Strand"), new Fragment("Isolation", "Strand"),
            new Fragment("Mind", "Strand"), new Fragment("Redbirth", "Strand"),
            new Fragment("Wisdom", "Strand")];
        //=====================================================================
        //Prismatic Fragments.
        //=====================================================================
        private static readonly Fragment[] PrismaticFragments = [
            new Fragment("Awakening", "Prismatic", new StatChange(Stat.Health, +10)),
            new Fragment("Courage", "Prismatic", new StatChange(Stat.Grenade, +10)),
            new Fragment("Dawn", "Prismatic", new StatChange(Stat.Melee, -10)),
            new Fragment("Defiance", "Prismatic", new StatChange(Stat.Class, +10)),
            new Fragment("Devotion", "Prismatic", new StatChange(Stat.Melee, +10)),
            new Fragment("Dominance", "Prismatic", new StatChange(Stat.Grenade, -10)),
            new Fragment("Grace", "Prismatic", new StatChange(Stat.Health, -10)),
            new Fragment("Honor", "Prismatic", new StatChange(Stat.Melee, +10)),
            new Fragment("Justice", "Prismatic", new StatChange(Stat.Super, +10)),
            new Fragment("Protection", "Prismatic", new StatChange(Stat.Melee, +10)),
            new Fragment("Purpose", "Prismatic", new StatChange(Stat.Class, -10)),
            new Fragment("Ruin", "Prismatic", new StatChange(Stat.Weapons, +10)),
            new Fragment("Sacrifice", "Prismatic", new StatChange(Stat.Grenade, +10)),
            new Fragment("Balance", "Prismatic"), new Fragment("Blessing", "Prismatic"),
            new Fragment("Bravery", "Prismatic"), new Fragment("Command", "Prismatic"),
            new Fragment("Generosity", "Prismatic"), new Fragment("Hope", "Prismatic"),
            new Fragment("Mending", "Prismatic"), new Fragment("Solitude", "Prismatic")];
        //=====================================================================
        //Grenades.
        //=====================================================================
        private static readonly string[] ArcGrenades = ["Lightning", "Storm", "Flashbang", "Pulse", "Skip", "Flux", "Arcbolt"];
        private static readonly string[] SolarGrenades = ["Tripmine", "Thermite", "Incendiary", "Solar", "Swarm", "Fusion", "Firebolt", "Healing"];
        private static readonly string[] VoidGrenades = ["Void Spike", "Void Wall", "Suppressor", "Vortex", "Scatter", "Magnetic", "Axion Bolt"];
        private static readonly string[] StasisGrenades = ["Glacier", "Duskfield", "Coldsnap"];
        private static readonly string[] StrandGrenades = ["Shackle", "Threadling", "Grapple"];

        private static readonly string[] PrismaticWarlockGrenades = ["Vortex", "Healing", "Storm", "Coldsnap", "Threadling"];
        private static readonly string[] PrismaticTitanGrenades = ["Shackle", "Glacier", "Pulse", "Thermite", "Suppressor"];
        private static readonly string[] PrismaticHunterGrenades = ["Grapple", "Swarm", "Magnetic", "Arcbolt", "Duskfield"];
        //=====================================================================
        //Warlock Subclasses.
        //=====================================================================
        public static readonly Subclass WarlockArc = new(
            name: "Arc",
            playerClass: PlayerClass.Warlock,
            fragments: GetArcFragments(PlayerClass.Warlock),
            aspects: [
                new Aspect("Arc Soul", 2), new Aspect("Lightning Surge", 2),
                new Aspect("Electrostatic Mind", 2), new Aspect("Ionic Sentry", 2)],
            supers: ["Stormtrance", "Chaos Reach"],
            melees: ["Chain Lightning", "Ball Lightning"],
            grenades: ArcGrenades,
            classAbilities: WarlockClassAbilities,
            jumps: WarlockJumps
        );
        public static readonly Subclass WarlockSolar = new(
            name: "Solar",
            playerClass: PlayerClass.Warlock,
            fragments: SolarFragments,
            aspects: [
                new Aspect("Icarus Dash", 2), new Aspect("Heat Rises", 2),
                new Aspect("Touch of Flame", 2), new Aspect("Hellion", 2)],
            supers: ["Daybreak", "Well of Radiance", "Song of Flame"],
            melees: ["Incinerator Snap", "Celestial Fire"],
            grenades: SolarGrenades,
            classAbilities: WarlockSolarClassAbilities,
            jumps: WarlockJumps
        );
        public static readonly Subclass WarlockVoid = new(
            name: "Void",
            playerClass: PlayerClass.Warlock,
            fragments: GetVoidFragments(PlayerClass.Warlock),
            aspects: [
                new Aspect("Chaos Accelerant", 2), new Aspect("Feed the Void", 2), 
                new Aspect("Child of the Old Gods", 2)],
            supers: ["Nova Warp", "Nova Bomb Vortex", "Nova Bomb Cataclysm"],
            melees: ["Pocket Singularity"],
            grenades: VoidGrenades,
            classAbilities: WarlockClassAbilities,
            jumps: WarlockBlink
        );
        public static readonly Subclass WarlockStasis = new(
            name: "Stasis",
            playerClass: PlayerClass.Warlock,
            fragments: StasisFragments,
            aspects: [
                new Aspect("Iceflare Bolts", 2), new Aspect("Frostpulse", 2),
                new Aspect("Bleak Watcher", 2), new Aspect("Glacial Harvest", 2)],
            supers: ["Winters Wrath"],
            melees: ["Penumbral Blast"],
            grenades: StasisGrenades,
            classAbilities: WarlockClassAbilities,
            jumps: WarlockJumps
        );
        public static readonly Subclass WarlockStrand = new(
            name: "Strand",
            playerClass: PlayerClass.Warlock,
            fragments: StrandFragments,
            aspects: [
                new Aspect("Weavers Call", 2), new Aspect("Mindspun Invocation", 2),
                new Aspect("Wanderer", 2), new Aspect("Weavewalk", 3)],
            supers: ["Needlestorm"],
            melees: ["Arcane Needle"],
            grenades: StrandGrenades,
            classAbilities: WarlockClassAbilities,
            jumps: WarlockJumps
        );
        public static readonly Subclass WarlockPrismatic = new(
            name: "Prismatic",
            playerClass: PlayerClass.Warlock,
            fragments: PrismaticFragments,
            aspects: [
                new Aspect("Hellion", 2), new Aspect("Feed the Void", 2),
                new Aspect("Lightning Surge", 3), new Aspect("Bleak Watcher", 2),
                new Aspect("Weavers Call", 3)],
            supers: ["Song of Flame", "Nova Bomb Cataclysm", "Stormtrance", "Winters Wrath", "Needlestorm"],
            melees: ["Arcane Needle", "Pocket Singularity", "Incinerator Snap", "Chain Lightning", "Penumbral Blast"],
            grenades: PrismaticWarlockGrenades,
            classAbilities: WarlockSolarClassAbilities,
            jumps: WarlockBlink
        );
        //=====================================================================
        //Titan Subclasses.
        //=====================================================================
        public static readonly Subclass TitanArc = new(
            name: "Arc",
            playerClass: PlayerClass.Titan,
            fragments: GetArcFragments(PlayerClass.Titan),
            aspects: [
                new Aspect("Storms Keep", 2), new Aspect("Touch of Thunder",2),
                new Aspect("Juggernaut", 2), new Aspect("Knockout", 2)],
            supers: ["Fists of Havoc", "Thundercrash"],
            melees: ["Thunderclap", "Seismic Strike", "Ballistic Slam"],
            grenades: ArcGrenades,
            classAbilities: TitanArcClassAbilities,
            jumps: TitanJumps
        );
        public static readonly Subclass TitanSolar = new(
            name: "Solar",
            playerClass: PlayerClass.Titan,
            fragments: SolarFragments,
            aspects: [
                new Aspect("Sol Invictus", 2), new Aspect("Roaring Flames", 2),
                new Aspect("Consecration", 3)],
            supers: ["Hammer of Sol", "Burning Maul"],
            melees: ["Hammer Strike", "Throwing Hammer"],
            grenades: SolarGrenades,
            classAbilities: TitanClassAbilities,
            jumps: TitanJumps
        );
        public static readonly Subclass TitanVoid = new(
            name: "Void",
            playerClass: PlayerClass.Titan,
            fragments: GetVoidFragments(PlayerClass.Titan),
            aspects: [
                new Aspect("Controlled Demolition", 2), new Aspect("Bastion", 2),
                new Aspect("Offensive Bulwark", 2), new Aspect("Unbreakable", 3)],
            supers: ["Sentinel Shield", "Ward of Dawn", "Twilight Arsenal"],
            melees: ["Shield Bash", "Shield Throw"],
            grenades: VoidGrenades,
            classAbilities: TitanClassAbilities,
            jumps: TitanJumps
        );
        public static readonly Subclass TitanStasis = new(
            name: "Stasis",
            playerClass: PlayerClass.Titan,
            fragments: StasisFragments,
            aspects: [
                new Aspect("Cryoclasm", 3), new Aspect("Tectonic Harvest", 2),
                new Aspect("Howl of the Storm", 2), new Aspect("Diamond Lance", 3)
            ],
            supers: ["Glacial Quake"],
            melees: ["Shiver Strike"],
            grenades: StasisGrenades,
            classAbilities: TitanClassAbilities,
            jumps: TitanJumps
        );
        public static readonly Subclass TitanStrand = new(
            name: "Strand",
            playerClass: PlayerClass.Titan,
            fragments: StrandFragments,
            aspects: [
                new Aspect("Into the Fray", 2), new Aspect("Drengrs Lash", 2),
                new Aspect("Flechette Storm", 2), new Aspect("Banner of War", 2)],
            supers: ["Bladefury"],
            melees: ["Frenzied Blade"],
            grenades: StrandGrenades,
            classAbilities: TitanClassAbilities,
            jumps: TitanJumps
        );
        public static readonly Subclass TitanPrismatic = new(
            name: "Prismatic",
            playerClass: PlayerClass.Titan,
            fragments: PrismaticFragments,
            aspects: [
                new Aspect("Unbreakable", 3), new Aspect("Consecration", 2),
                new Aspect("Knockout", 2), new Aspect("Diamond Lance", 3),
                new Aspect("Drengrs Lash", 3)],
            supers: ["Thundercrash", "Bladefury", "Twilight Arsenal", "Hammer of Sol", "Glacial Quake"],
            melees: ["Thunderclap", "Shield Throw", "Shiver Strike", "Hammer Strike", "Frenzied Blade"],
            grenades: PrismaticTitanGrenades,
            classAbilities: TitanArcClassAbilities,
            jumps: TitanJumps
        );
        //=====================================================================
        //Hunter Subclasses.
        //=====================================================================
        public static readonly Subclass HunterArc = new(
            name: "Arc",
            playerClass: PlayerClass.Hunter,
            fragments: GetArcFragments(PlayerClass.Hunter),
            aspects: [
                new Aspect("Flow State", 2), new Aspect("Tempest Strike", 2),
                new Aspect("Lethal Current", 2), new Aspect("Ascension", 3)],
            supers: ["Gathering Storm", "Arc Staff", "Storms Edge"],
            melees: ["Combination Blow", "Disorienting Blow"],
            grenades: ArcGrenades,
            classAbilities: HunterClassAbilities,
            jumps: HunterBlink
        );
        public static readonly Subclass HunterSolar = new(
            name: "Solar",
            playerClass: PlayerClass.Hunter,
            fragments: SolarFragments,
            aspects: [
                new Aspect("Knock em Down", 2), new Aspect("On Your Mark", 3),
                new Aspect("Gunpowder Gamble", 2)],
            supers: ["Golden Gun Deadshot", "Golden Gun Marksman", "Blade Barrage"],
            melees: ["Knife Trick", "Lightweight Knife", "Weighted Throwing Knife", "Proximity Explosive Knife"],
            grenades: SolarGrenades,
            classAbilities: HunterSolarClassAbilities,
            jumps: HunterJumps
        );
        public static readonly Subclass HunterVoid = new(
            name: "Void",
            playerClass: PlayerClass.Hunter,
            fragments: GetVoidFragments(PlayerClass.Hunter),
            aspects: [
                new Aspect("Trappers Ambush", 2), new Aspect("Vanishing Step", 2),
                new Aspect("Stylish Executioner", 2), new Aspect("On the Prowl", 3)],
            supers: ["Shadowshot Moebius Quiver", "Shadowshot Deadfall", "Spectral Blades"],
            melees: ["Snare Bomb"],
            grenades: VoidGrenades,
            classAbilities: HunterClassAbilities,
            jumps: HunterJumps
        );
        public static readonly Subclass HunterStasis = new(
            name: "Stasis",
            playerClass: PlayerClass.Hunter,
            fragments: StasisFragments,
            aspects: [
                new Aspect("Winters Shroud", 2), new Aspect("Shatterdive", 2),
                new Aspect("Grim Harvest", 3), new Aspect("Touch of Winter", 2)],
            supers: ["Silence and Squall"],
            melees: ["Withering Blade"],
            grenades: StasisGrenades,
            classAbilities: HunterClassAbilities,
            jumps: HunterJumps
        );
        public static readonly Subclass HunterStrand = new(
            name: "Strand",
            playerClass: PlayerClass.Hunter,
            fragments: StrandFragments,
            aspects: [
                new Aspect("Widows Silk", 2), new Aspect("Ensnaring Slam", 2),
                new Aspect("Threaded Specter", 3), new Aspect("Whirling Maelstrom", 2)],
            supers: ["Silkstrike"],
            melees: ["Threaded Spike"],
            grenades: StrandGrenades,
            classAbilities: HunterClassAbilities,
            jumps: HunterJumps
        );
        public static readonly Subclass HunterPrismatic = new(
            name: "Prismatic",
            playerClass: PlayerClass.Hunter,
            fragments: PrismaticFragments,
            aspects: [
                new Aspect("Ascension", 2), new Aspect("Stylish Executioner",2),
                new Aspect("Gunpowder Gamble", 3), new Aspect("Winters Shroud", 2),
                new Aspect("Threaded Specter", 3)],
            supers: ["Golden Gun Marksman", "Silence and Squall", "Storms Edge", "Shadowshot Deadfall", "Silkstrike"],
            melees: ["Threaded Strike", "Withering Blade", "Snare Bomb", "Knife Trick", "Combination Blow"],
            grenades: PrismaticHunterGrenades,
            classAbilities: HunterSolarClassAbilities,
            jumps: HunterBlink
        );
        //=====================================================================
        //All Subclasses lookup.
        //=====================================================================
        public static readonly Dictionary<PlayerClass, Subclass[]> All = new(){
            {PlayerClass.Warlock, new[] {WarlockArc, WarlockSolar, WarlockVoid, WarlockStasis, WarlockStrand, WarlockPrismatic}},
            {PlayerClass.Titan, new[] {TitanArc, TitanSolar, TitanVoid, TitanStasis, TitanStrand, TitanPrismatic}},
            {PlayerClass.Hunter, new[] {HunterArc, HunterSolar, HunterVoid, HunterStasis, HunterStrand, HunterPrismatic}}
        };
        /*
        Method        : GetSubclass
        Description   : Returns subclass matching given class & name.
        Parameters    : PlayerClass playerClass : Player class to look up.
                        string name             : Subclass name (e.g. "Arc").
        Return Values : Subclass                : Matching subclass, or null if not found.
        */
        public static Subclass? GetSubclass(PlayerClass playerClass, string name){
            foreach (Subclass subclass in All[playerClass]){
                if (subclass.Name == name) return subclass;
            }
            return null;
        }
    }
}