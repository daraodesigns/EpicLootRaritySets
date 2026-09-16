using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EpicLoot;
using EpicLoot.LegendarySystem;
using EpicLoot.MagicItemEffects;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace Fran.EpicLootRaritySets
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("randyknapp.mods.epicloot", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("ishid4.mods.betterarchery", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("NorseDemigods", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("neobotics.valheim_mod.wolfpack", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("radamanto.Bestiary", BepInDependency.DependencyFlags.SoftDependency)]
    public class EpicLootRaritySetsPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "fran.mods.epiclootraritysets";
        public const string PluginName = "Epic Loot Rarity Sets";
        public const string PluginVersion = "0.1.36";

        internal static ManualLogSource Log;
        internal static ConfigFile PluginConfig;
        internal static ConfigEntry<bool> EnableNaturalDrops;
        internal static ConfigEntry<bool> EnableSetActivationBuffs;
        internal static ConfigEntry<bool> EnableFrostbrandAbilities;
        internal static ConfigEntry<bool> EnableHraesvelgrAbilities;
        internal static ConfigEntry<bool> EnableSolomonKaneAbilities;
        internal static ConfigEntry<bool> EnableMoonveinAbilities;
        internal static ConfigEntry<bool> EnableNottAbilities;
        internal static ConfigEntry<bool> EnableRagnarAbilities;
        internal static ConfigEntry<bool> EnableHelveigAbilities;
        internal static ConfigEntry<bool> EnableHeimdallAbilities;
        internal static ConfigEntry<bool> EnableSeidrAbilities;
        internal static ConfigEntry<bool> GenerateManagedConfigFiles;
        internal static ConfigEntry<bool> ReadLegendariesJson;
        internal static ConfigEntry<bool> ReadRaritySetsJson;
        internal static ConfigEntry<bool> ReadBossSetDropRules;
        internal static ConfigEntry<bool> ConfigureWolfPackCompatibility;
        internal static ConfigEntry<string> WolfPackTrainableCreatures;
        internal static ConfigEntry<int> WolfPackDetectionRange;
        internal static ConfigEntry<int> WolfPackAttackRange;
        internal static ConfigEntry<int> WolfPackMaxTames;
        internal static ConfigEntry<KeyboardShortcut> FrostbrandDashHotkey;
        internal static ConfigEntry<KeyboardShortcut> FrostbrandHolyStrikeHotkey;
        internal static ConfigEntry<KeyboardShortcut> FrostbrandWaterSphereHotkey;
        internal static ConfigEntry<bool> FrostbrandHolySunRequiresBlock;
        internal static ConfigEntry<float> FrostbrandDashCooldown;
        internal static ConfigEntry<float> FrostbrandDashEitrUse;
        internal static ConfigEntry<float> FrostbrandDashRange;
        internal static ConfigEntry<float> FrostbrandDashDamage;
        internal static ConfigEntry<float> FrostbrandHolyStrikeCooldown;
        internal static ConfigEntry<float> FrostbrandHolyStrikeEitrUse;
        internal static ConfigEntry<float> FrostbrandHolyStrikeRange;
        internal static ConfigEntry<float> FrostbrandHolyStrikeFireDamage;
        internal static ConfigEntry<float> FrostbrandHolyStrikeSpiritDamage;
        internal static ConfigEntry<float> FrostbrandHolySunCooldown;
        internal static ConfigEntry<float> FrostbrandHolySunEitrUse;
        internal static ConfigEntry<float> FrostbrandHolySunRange;
        internal static ConfigEntry<float> FrostbrandHolySunRadius;
        internal static ConfigEntry<float> FrostbrandHolySunDuration;
        internal static ConfigEntry<float> FrostbrandHolySunFireDamagePerTick;
        internal static ConfigEntry<float> FrostbrandHolySunSpiritDamagePerTick;
        internal static ConfigEntry<float> FrostbrandWaterSphereCooldown;
        internal static ConfigEntry<float> FrostbrandWaterSphereEitrUse;
        internal static ConfigEntry<float> FrostbrandWaterSpherePullRadius;
        internal static ConfigEntry<float> FrostbrandWaterSpherePullForce;
        internal static ConfigEntry<float> FrostbrandWaterSphereMaxDuration;
        internal static ConfigEntry<int> FrostbrandLightningStrikeAttackCount;
        internal static ConfigEntry<float> FrostbrandLightningStrikeRadius;
        internal static ConfigEntry<float> FrostbrandLightningStrikeBaseDamage;
        internal static ConfigEntry<float> FrostbrandLightningStrikeDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> FrostbrandLightningStrikeImpactForce;
        internal static ConfigEntry<KeyboardShortcut> FrostbrandSlashHotkey;
        internal static ConfigEntry<KeyboardShortcut> FrostbrandCrushHotkey;
        internal static ConfigEntry<KeyboardShortcut> FrostbrandElementalShieldHotkey;
        internal static ConfigEntry<float> FrostbrandSlashCooldown;
        internal static ConfigEntry<float> FrostbrandSlashEitrUse;
        internal static ConfigEntry<float> FrostbrandSlashRange;
        internal static ConfigEntry<float> FrostbrandSlashRadius;
        internal static ConfigEntry<float> FrostbrandSlashBaseDamage;
        internal static ConfigEntry<float> FrostbrandSlashDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> FrostbrandCrushCooldown;
        internal static ConfigEntry<float> FrostbrandCrushEitrUse;
        internal static ConfigEntry<float> FrostbrandCrushRange;
        internal static ConfigEntry<float> FrostbrandCrushRadius;
        internal static ConfigEntry<float> FrostbrandCrushBaseDamage;
        internal static ConfigEntry<float> FrostbrandCrushDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> FrostbrandBurningGroundDuration;
        internal static ConfigEntry<float> FrostbrandBurningGroundTickInterval;
        internal static ConfigEntry<float> FrostbrandBurningGroundBaseDamage;
        internal static ConfigEntry<float> FrostbrandBurningGroundDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> FrostbrandElementalShieldCooldown;
        internal static ConfigEntry<float> FrostbrandElementalShieldEitrUse;
        internal static ConfigEntry<float> FrostbrandElementalShieldDuration;
        internal static ConfigEntry<float> FrostbrandElementalShieldBaseMitigation;
        internal static ConfigEntry<float> FrostbrandElementalShieldMitigationPerElementalMagicLevel;
        internal static ConfigEntry<float> FrostbrandElementalShieldMaxMitigation;
        internal static ConfigEntry<KeyboardShortcut> HraesvelgrVolleyHotkey;
        internal static ConfigEntry<KeyboardShortcut> HraesvelgrSummonHotkey;
        internal static ConfigEntry<float> HraesvelgrSneakyNoiseModifier;
        internal static ConfigEntry<float> HraesvelgrSneakyStealthModifier;
        internal static ConfigEntry<float> HraesvelgrSneakySpeedModifier;
        internal static ConfigEntry<float> HraesvelgrVolleyDuration;
        internal static ConfigEntry<float> HraesvelgrVolleyCooldown;
        internal static ConfigEntry<float> HraesvelgrVolleyAttackSpeedMultiplier;
        internal static ConfigEntry<float> HraesvelgrVolleyProjectileSpeedMultiplier;
        internal static ConfigEntry<float> HraesvelgrVolleyStaminaUseMultiplier;
        internal static ConfigEntry<float> HraesvelgrVolleyShotsPerSecond;
        internal static ConfigEntry<float> HraesvelgrVolleyDamageMultiplier;
        internal static ConfigEntry<float> HraesvelgrVolleyProjectileVelocity;
        internal static ConfigEntry<float> HraesvelgrDashCooldown;
        internal static ConfigEntry<float> HraesvelgrDashEitrUse;
        internal static ConfigEntry<float> HraesvelgrSummonCooldown;
        internal static ConfigEntry<float> HraesvelgrSummonDuration;
        internal static ConfigEntry<float> HraesvelgrSummonStaminaUse;
        internal static ConfigEntry<float> HraesvelgrSummonGuardRadius;
        internal static ConfigEntry<KeyboardShortcut> HraesvelgrTrapHotkey;
        internal static ConfigEntry<float> HraesvelgrTrapStaminaUse;
        internal static ConfigEntry<int> HraesvelgrTrapMaxCharges;
        internal static ConfigEntry<float> HraesvelgrTrapRechargeSeconds;
        internal static ConfigEntry<float> HraesvelgrTrapBuffDuration;
        internal static ConfigEntry<float> HraesvelgrTrapNextAttackDamageBonus;
        internal static ConfigEntry<int> HraesvelgrHeadshotAttackCount;
        internal static ConfigEntry<float> HraesvelgrHeadshotDamageMultiplier;
        internal static ConfigEntry<KeyboardShortcut> SolomonKaneInfusedBoltHotkey;
        internal static ConfigEntry<KeyboardShortcut> SolomonKaneBombHotkey;
        internal static ConfigEntry<KeyboardShortcut> SolomonKaneBatFormHotkey;
        internal static ConfigEntry<float> SolomonKaneInfusedBoltStaminaUse;
        internal static ConfigEntry<float> SolomonKaneInfusedBoltCooldown;
        internal static ConfigEntry<float> SolomonKaneInfusedBoltDuration;
        internal static ConfigEntry<float> SolomonKaneInfusedBoltRadius;
        internal static ConfigEntry<float> SolomonKaneInfusedBoltBaseDamage;
        internal static ConfigEntry<float> SolomonKaneInfusedBoltDamagePerCrossbowsLevel;
        internal static ConfigEntry<float> SolomonKaneInfusedBoltSlow;
        internal static ConfigEntry<float> SolomonKaneInfusedBoltSlowDuration;
        internal static ConfigEntry<float> SolomonKaneBombStaminaUse;
        internal static ConfigEntry<float> SolomonKaneBombCooldown;
        internal static ConfigEntry<float> SolomonKaneBombRange;
        internal static ConfigEntry<float> SolomonKaneBombRadius;
        internal static ConfigEntry<float> SolomonKaneBombBaseDamage;
        internal static ConfigEntry<float> SolomonKaneBombDamagePerCrossbowsLevel;
        internal static ConfigEntry<float> SolomonKaneBombImpactForce;
        internal static ConfigEntry<float> SolomonKaneBatFormStaminaUse;
        internal static ConfigEntry<float> SolomonKaneBatFormCooldown;
        internal static ConfigEntry<float> SolomonKaneBatFormDuration;
        internal static ConfigEntry<float> SolomonKaneBatFormFlightSpeed;
        internal static ConfigEntry<float> SolomonKaneBatFormVisualScale;
        internal static ConfigEntry<float> SolomonKaneWitchmarkDuration;
        internal static ConfigEntry<float> SolomonKaneSilverVerdictBaseDamage;
        internal static ConfigEntry<float> SolomonKaneSilverVerdictDamagePerCrossbowsLevel;
        internal static ConfigEntry<int> SolomonKaneSilverVerdictTargetCount;
        internal static ConfigEntry<float> SolomonKaneSilverVerdictSeekRadius;
        internal static ConfigEntry<float> SolomonKaneSilverVerdictDamageLossPerJump;
        internal static ConfigEntry<float> SolomonKaneSilverVerdictStaminaRefund;
        internal static ConfigEntry<KeyboardShortcut> NottWarpHotkey;
        internal static ConfigEntry<float> NottSneakyNoiseModifier;
        internal static ConfigEntry<float> NottSneakyStealthModifier;
        internal static ConfigEntry<float> NottSneakySpeedModifier;
        internal static ConfigEntry<float> NottWarpRange;
        internal static ConfigEntry<float> NottWarpCooldown;
        internal static ConfigEntry<float> NottWarpEitrUse;
        internal static ConfigEntry<float> NottWarpStaminaUse;
        internal static ConfigEntry<float> NottWarpDamageMultiplier;
        internal static ConfigEntry<float> NottWarpResetRadius;
        internal static ConfigEntry<float> NottHitSpeedBonus;
        internal static ConfigEntry<float> NottHitSpeedDuration;
        internal static ConfigEntry<int> NottHitSpeedMaxStacks;
        internal static ConfigEntry<float> NottPoisonBaseDamage;
        internal static ConfigEntry<float> NottPoisonDamagePerKnivesLevel;
        internal static ConfigEntry<KeyboardShortcut> RagnarDecayAuraHotkey;
        internal static ConfigEntry<float> RagnarDecayAuraRadius;
        internal static ConfigEntry<float> RagnarDecayAuraStaminaPerSecond;
        internal static ConfigEntry<float> RagnarDecayAuraBaseDamage;
        internal static ConfigEntry<float> RagnarDecayAuraDamagePerAxesLevel;
        internal static ConfigEntry<float> RagnarDecayAuraTickInterval;
        internal static ConfigEntry<float> RagnarFuryAttackSpeedPerStack;
        internal static ConfigEntry<float> RagnarFuryLifeStealPerStack;
        internal static ConfigEntry<float> RagnarFuryDuration;
        internal static ConfigEntry<int> RagnarFuryMaxStacks;
        internal static ConfigEntry<int> RagnarFuryHealEveryAttacks;
        internal static ConfigEntry<float> RagnarFuryHealMaxHealthFraction;
        internal static ConfigEntry<float> MagicSetDropChance;
        internal static ConfigEntry<float> RareSetDropChance;
        internal static ConfigEntry<float> EpicSetDropChance;
        internal static ConfigEntry<float> AncientSetDropChance;
        internal static ConfigEntry<float> MoonveinMagicBowEitrUse;
        internal static ConfigEntry<float> MoonveinRareBowEitrUse;
        internal static ConfigEntry<float> MoonveinEpicBowEitrUse;
        internal static ConfigEntry<float> MoonveinLegendaryBowEitrUse;
        internal static ConfigEntry<float> MoonveinMythicBowEitrUse;
        internal static ConfigEntry<float> MoonveinAncientBowEitrUse;
        internal static ConfigEntry<KeyboardShortcut> MoonveinMeteorHotkey;
        internal static ConfigEntry<KeyboardShortcut> MoonveinTornadoHotkey;
        internal static ConfigEntry<float> MoonveinProjectileVelocity;
        internal static ConfigEntry<float> MoonveinAcidBoltBaseDamage;
        internal static ConfigEntry<float> MoonveinAcidBoltDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> MoonveinLightningBoltBaseDamage;
        internal static ConfigEntry<float> MoonveinLightningBoltDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> MoonveinFireballBaseDamage;
        internal static ConfigEntry<float> MoonveinFireballDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> MoonveinMeteorCooldown;
        internal static ConfigEntry<float> MoonveinMeteorEitrUse;
        internal static ConfigEntry<float> MoonveinMeteorRange;
        internal static ConfigEntry<float> MoonveinMeteorBaseDamage;
        internal static ConfigEntry<float> MoonveinMeteorDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> MoonveinMeteorImpactForce;
        internal static ConfigEntry<float> MoonveinTornadoCooldown;
        internal static ConfigEntry<float> MoonveinTornadoEitrUse;
        internal static ConfigEntry<float> MoonveinTornadoDuration;
        internal static ConfigEntry<float> MoonveinTornadoRadius;
        internal static ConfigEntry<float> MoonveinTornadoBaseDamage;
        internal static ConfigEntry<float> MoonveinTornadoDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> MoonveinTornadoTickInterval;
        internal static ConfigEntry<float> MoonveinTornadoImpactForce;
        internal static ConfigEntry<float> MoonveinTornadoSlow;
        internal static ConfigEntry<float> MoonveinTornadoSlowDuration;
        internal static ConfigEntry<KeyboardShortcut> HelveigHolyHealHotkey;
        internal static ConfigEntry<KeyboardShortcut> HelveigBloodRiteHotkey;
        internal static ConfigEntry<KeyboardShortcut> HelveigHolyStrikeHotkey;
        internal static ConfigEntry<KeyboardShortcut> HelveigSummonUndeadHotkey;
        internal static ConfigEntry<float> HelveigHolyHealCooldown;
        internal static ConfigEntry<float> HelveigHolyHealEitrUse;
        internal static ConfigEntry<float> HelveigHolyHealBaseHealing;
        internal static ConfigEntry<float> HelveigHolyHealHealingPerBloodMagicLevel;
        internal static ConfigEntry<float> HelveigHolyStrikeCooldown;
        internal static ConfigEntry<float> HelveigHolyStrikeEitrUse;
        internal static ConfigEntry<float> HelveigHolyStrikeRange;
        internal static ConfigEntry<float> HelveigHolyStrikeBaseFireDamage;
        internal static ConfigEntry<float> HelveigHolyStrikeFireDamagePerBloodMagicLevel;
        internal static ConfigEntry<float> HelveigHolyStrikeBaseSpiritDamage;
        internal static ConfigEntry<float> HelveigHolyStrikeSpiritDamagePerBloodMagicLevel;
        internal static ConfigEntry<float> HelveigBloodRiteCooldown;
        internal static ConfigEntry<float> HelveigBloodRiteEitrUse;
        internal static ConfigEntry<float> HelveigBloodRiteRadius;
        internal static ConfigEntry<float> HelveigBloodRiteDuration;
        internal static ConfigEntry<float> HelveigBloodRiteTickInterval;
        internal static ConfigEntry<float> HelveigBloodRiteBaseHealing;
        internal static ConfigEntry<float> HelveigBloodRiteHealingPerBloodMagicLevel;
        internal static ConfigEntry<float> HelveigBloodRiteCancelMoveDistance;
        internal static ConfigEntry<float> HelveigSummonUndeadCooldown;
        internal static ConfigEntry<float> HelveigSummonUndeadEitrUse;
        internal static ConfigEntry<float> HelveigSummonUndeadDuration;
        internal static ConfigEntry<KeyboardShortcut> HeimdallLightningStormHotkey;
        internal static ConfigEntry<KeyboardShortcut> HeimdallStoneShieldHotkey;
        internal static ConfigEntry<float> HeimdallBlockArmorBonusPerStack;
        internal static ConfigEntry<float> HeimdallBlockArmorDuration;
        internal static ConfigEntry<int> HeimdallBlockArmorMaxStacks;
        internal static ConfigEntry<float> HeimdallLightningStormCooldown;
        internal static ConfigEntry<float> HeimdallLightningStormDuration;
        internal static ConfigEntry<float> HeimdallLightningStormRadius;
        internal static ConfigEntry<float> HeimdallLightningStormBaseDamage;
        internal static ConfigEntry<float> HeimdallLightningStormDamagePerBlockingLevel;
        internal static ConfigEntry<float> HeimdallLightningStormTickInterval;
        internal static ConfigEntry<float> HeimdallStoneShieldCooldown;
        internal static ConfigEntry<float> HeimdallStoneShieldDuration;
        internal static ConfigEntry<float> HeimdallStoneShieldStaminaUse;
        internal static ConfigEntry<float> HeimdallStoneShieldBaseReduction;
        internal static ConfigEntry<float> HeimdallStoneShieldReductionPerBlockingLevel;
        internal static ConfigEntry<float> HeimdallStoneShieldReflectBase;
        internal static ConfigEntry<float> HeimdallStoneShieldReflectPerBlockingLevel;
        internal static ConfigEntry<float> HeimdallStoneShieldReflectMax;
        internal static ConfigEntry<KeyboardShortcut> SeidrNanoCubeHotkey;
        internal static ConfigEntry<KeyboardShortcut> SeidrElementalShieldHotkey;
        internal static ConfigEntry<KeyboardShortcut> SeidrStoneGolemHotkey;
        internal static ConfigEntry<KeyboardShortcut> SeidrFrostNovaHotkey;
        internal static ConfigEntry<float> SeidrNanoCubeCooldown;
        internal static ConfigEntry<float> SeidrNanoCubeDuration;
        internal static ConfigEntry<float> SeidrNanoCubeEitrUse;
        internal static ConfigEntry<float> SeidrNanoCubeRadius;
        internal static ConfigEntry<float> SeidrNanoCubeMagicDamageBonus;
        internal static ConfigEntry<float> SeidrElementalShieldEitrPercentPerSecond;
        internal static ConfigEntry<float> SeidrElementalShieldCooldown;
        internal static ConfigEntry<float> SeidrElementalShieldEitrUse;
        internal static ConfigEntry<float> SeidrStoneGolemCooldown;
        internal static ConfigEntry<float> SeidrStoneGolemEitrUse;
        internal static ConfigEntry<float> SeidrStoneGolemDuration;
        internal static ConfigEntry<float> SeidrFrostNovaCooldown;
        internal static ConfigEntry<float> SeidrFrostNovaEitrUse;
        internal static ConfigEntry<float> SeidrFrostNovaRadius;
        internal static ConfigEntry<float> SeidrFrostNovaBaseDamage;
        internal static ConfigEntry<float> SeidrFrostNovaDamagePerElementalMagicLevel;
        internal static ConfigEntry<float> SeidrFrostNovaSlow;
        internal static ConfigEntry<float> SeidrFrostNovaSlowDuration;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;
            PluginConfig = Config;

            EnableNaturalDrops = Config.Bind("General", "Enable Natural Drops", true, "Allow configured non-legendary rarity sets to appear from normal EpicLoot rolls.");
            EnableSetActivationBuffs = Config.Bind("General", "Enable Set Activation Buffs", true, "Show a short status effect named after the active set base name, without rarity prefix. Example: MagicRagnar activates Ragnar.");
            GenerateManagedConfigFiles = Config.Bind("Files", "Generate Managed Config Files", true, "Write the EpicLoot and NorseDemigods config files managed by this DLL into BepInEx/config on startup.");
            ReadLegendariesJson = Config.Bind("Files", "Read Extra Sections From Legendaries Json", true, "Read MagicItems/RareItems/EpicItems/AncientItems and matching *Sets arrays from EpicLoot/baseconfig/legendaries.json.");
            ReadRaritySetsJson = Config.Bind("Files", "Read Rarity Sets Json", true, "Read extra rarity set definitions from EpicLoot/raritysets.json.");
            ReadBossSetDropRules = Config.Bind("Files", "Read Boss Set Drop Rules", true, "Read per-boss set conversion rules from EpicLoot/bosssetdrops.json.");
            ConfigureWolfPackCompatibility = Config.Bind("WolfPack Compatibility", "Configure WolfPack", true, "Update WolfPack config on startup so Hraesvelgr summoned beasts can be controlled by WolfPack.");
            WolfPackTrainableCreatures = Config.Bind("WolfPack Compatibility", "Trainable Creatures", "*wolf*,*bjorn*,*bear*,*lobo*,*oso*", "Creature name patterns written into WolfPack TrainableCreatures. Wildcards match summoned wolf and bjorn/bear names across localizations.");
            WolfPackDetectionRange = Config.Bind("WolfPack Compatibility", "Detection Range", 100, new ConfigDescription("DetectionRange written into WolfPack when compatibility sync is enabled.", new AcceptableValueRange<int>(1, 150)));
            WolfPackAttackRange = Config.Bind("WolfPack Compatibility", "Tames Attack Range", 150, new ConfigDescription("TamesAttackRange written into WolfPack when compatibility sync is enabled.", new AcceptableValueRange<int>(0, 150)));
            WolfPackMaxTames = Config.Bind("WolfPack Compatibility", "Max Tames", 20, new ConfigDescription("MaxTames written into WolfPack when compatibility sync is enabled.", new AcceptableValueRange<int>(1, 50)));

            EnableFrostbrandAbilities = Config.Bind("Frostbrand Abilities", "Enable Frostbrand Abilities", true, "Enable the complete-set abilities for the Frostbrand spellblade set.");
            FrostbrandDashHotkey = Config.Bind("Frostbrand Abilities", "Dash Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Frostbrand dash.");
            FrostbrandHolyStrikeHotkey = Config.Bind("Frostbrand Abilities", "Holy Strike Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Frostbrand holy strike. If Holy Sun requires blocking, hold block with this hotkey to cast Holy Sun instead.");
            FrostbrandWaterSphereHotkey = Config.Bind("Frostbrand Abilities", "Water Sphere Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Frostbrand Water Sphere.");
            FrostbrandHolySunRequiresBlock = Config.Bind("Frostbrand Abilities", "Holy Sun Requires Block", true, "When true, Holy Sun uses Block + Holy Strike Hotkey so two mouse side buttons can cover all three abilities.");
            FrostbrandDashCooldown = Config.Bind("Frostbrand Abilities", "Dash Cooldown", 10f, new ConfigDescription("Cooldown in seconds for Frostbrand dash.", new AcceptableValueRange<float>(0f, 120f)));
            FrostbrandDashEitrUse = Config.Bind("Frostbrand Abilities", "Dash Eitr Use", 20f, new ConfigDescription("Eitr consumed by Frostbrand dash.", new AcceptableValueRange<float>(0f, 250f)));
            FrostbrandDashRange = Config.Bind("Frostbrand Abilities", "Dash Range", 14f, new ConfigDescription("Legacy custom dash distance. NorseDemigods dash bridge uses NorseDemigods.cfg [Ability Dash] range settings.", new AcceptableValueRange<float>(1f, 50f)));
            FrostbrandDashDamage = Config.Bind("Frostbrand Abilities", "Dash Poison Damage", 25f, new ConfigDescription("Legacy custom dash damage. NorseDemigods dash bridge uses NorseDemigods.cfg [Ability Dash] damage settings.", new AcceptableValueRange<float>(0f, 500f)));
            FrostbrandHolyStrikeCooldown = Config.Bind("Frostbrand Abilities", "Holy Strike Cooldown", 6f, new ConfigDescription("Cooldown in seconds for Frostbrand holy strike.", new AcceptableValueRange<float>(0f, 120f)));
            FrostbrandHolyStrikeEitrUse = Config.Bind("Frostbrand Abilities", "Holy Strike Eitr Use", 20f, new ConfigDescription("Eitr consumed by Frostbrand holy strike.", new AcceptableValueRange<float>(0f, 250f)));
            FrostbrandHolyStrikeRange = Config.Bind("Frostbrand Abilities", "Holy Strike Range", 24f, new ConfigDescription("Legacy fallback range. NorseDemigods Eir bridge uses NorseDemigods.cfg [Ability Holy Strike] range when available.", new AcceptableValueRange<float>(1f, 80f)));
            FrostbrandHolyStrikeFireDamage = Config.Bind("Frostbrand Abilities", "Holy Strike Fire Damage", 25f, new ConfigDescription("Legacy custom damage. NorseDemigods Eir bridge uses NorseDemigods.cfg [Ability Holy Strike] damage settings.", new AcceptableValueRange<float>(0f, 500f)));
            FrostbrandHolyStrikeSpiritDamage = Config.Bind("Frostbrand Abilities", "Holy Strike Spirit Damage", 25f, new ConfigDescription("Legacy custom damage. NorseDemigods Eir bridge uses NorseDemigods.cfg [Ability Holy Strike] damage settings.", new AcceptableValueRange<float>(0f, 500f)));
            FrostbrandHolySunCooldown = Config.Bind("Frostbrand Abilities", "Holy Sun Cooldown", 10f, new ConfigDescription("Cooldown in seconds for Frostbrand holy sun.", new AcceptableValueRange<float>(0f, 120f)));
            FrostbrandHolySunEitrUse = Config.Bind("Frostbrand Abilities", "Holy Sun Eitr Use", 35f, new ConfigDescription("Eitr consumed by Frostbrand holy sun.", new AcceptableValueRange<float>(0f, 250f)));
            FrostbrandHolySunRange = Config.Bind("Frostbrand Abilities", "Holy Sun Range", 25f, new ConfigDescription("Legacy custom range. NorseDemigods Eir bridge uses NorseDemigods.cfg [Ability Holy Sun] range settings.", new AcceptableValueRange<float>(1f, 80f)));
            FrostbrandHolySunRadius = Config.Bind("Frostbrand Abilities", "Holy Sun Radius", 4f, new ConfigDescription("Legacy custom radius. NorseDemigods Eir bridge uses NorseDemigods.cfg [Ability Holy Sun] radius settings.", new AcceptableValueRange<float>(1f, 20f)));
            FrostbrandHolySunDuration = Config.Bind("Frostbrand Abilities", "Holy Sun Duration", 4f, new ConfigDescription("Legacy custom duration. NorseDemigods Eir bridge uses NorseDemigods Holy Sun timing.", new AcceptableValueRange<float>(0.5f, 30f)));
            FrostbrandHolySunFireDamagePerTick = Config.Bind("Frostbrand Abilities", "Holy Sun Fire Damage Per Tick", 10f, new ConfigDescription("Legacy custom damage. NorseDemigods Eir bridge uses NorseDemigods.cfg [Ability Holy Sun] damage settings.", new AcceptableValueRange<float>(0f, 500f)));
            FrostbrandHolySunSpiritDamagePerTick = Config.Bind("Frostbrand Abilities", "Holy Sun Spirit Damage Per Tick", 10f, new ConfigDescription("Legacy custom damage. NorseDemigods Eir bridge uses NorseDemigods.cfg [Ability Holy Sun] damage settings.", new AcceptableValueRange<float>(0f, 500f)));
            FrostbrandWaterSphereCooldown = Config.Bind("Frostbrand Abilities", "Water Sphere Cooldown", 20f, new ConfigDescription("Cooldown in seconds for Frostbrand Water Sphere.", new AcceptableValueRange<float>(0f, 120f)));
            FrostbrandWaterSphereEitrUse = Config.Bind("Frostbrand Abilities", "Water Sphere Eitr Use", 35f, new ConfigDescription("Eitr consumed by Frostbrand Water Sphere.", new AcceptableValueRange<float>(0f, 250f)));
            FrostbrandWaterSpherePullRadius = Config.Bind("Frostbrand Abilities", "Water Sphere Pull Radius", 20f, new ConfigDescription("Radius in meters around each Water Sphere that pulls enemies toward it.", new AcceptableValueRange<float>(1f, 60f)));
            FrostbrandWaterSpherePullForce = Config.Bind("Frostbrand Abilities", "Water Sphere Pull Force", 9f, new ConfigDescription("Gravity-like pull force applied to enemies around Water Sphere.", new AcceptableValueRange<float>(0f, 60f)));
            FrostbrandWaterSphereMaxDuration = Config.Bind("Frostbrand Abilities", "Water Sphere Max Duration", 8f, new ConfigDescription("Maximum seconds this mod keeps updating the bridged Norse Water Sphere and gravity pull.", new AcceptableValueRange<float>(1f, 60f)));
            FrostbrandLightningStrikeAttackCount = Config.Bind("Frostbrand Abilities", "Lightning Strike Attack Count", 3, new ConfigDescription("Legacy key: number of Frostbrand weapon attacks required to trigger Fire Ball.", new AcceptableValueRange<int>(1, 20)));
            FrostbrandLightningStrikeRadius = Config.Bind("Frostbrand Abilities", "Lightning Strike Radius", 3.5f, new ConfigDescription("Legacy key: fallback radius in meters around the aimed point damaged by Frostbrand Fire Ball when no projectile prefab is available.", new AcceptableValueRange<float>(0.1f, 20f)));
            FrostbrandLightningStrikeBaseDamage = Config.Bind("Frostbrand Abilities", "Lightning Strike Base Damage", 45f, new ConfigDescription("Legacy key: base fire damage for Frostbrand Fire Ball before Elemental Magic scaling.", new AcceptableValueRange<float>(0f, 2000f)));
            FrostbrandLightningStrikeDamagePerElementalMagicLevel = Config.Bind("Frostbrand Abilities", "Lightning Strike Damage Per Elemental Magic Level", 0.85f, new ConfigDescription("Legacy key: extra fire damage per Elemental Magic level for Frostbrand Fire Ball.", new AcceptableValueRange<float>(0f, 30f)));
            FrostbrandLightningStrikeImpactForce = Config.Bind("Frostbrand Abilities", "Lightning Strike Impact Force", 35f, new ConfigDescription("Legacy key: projectile speed/impact force used by Frostbrand Fire Ball.", new AcceptableValueRange<float>(0f, 500f)));
            FrostbrandSlashHotkey = Config.Bind("Frostbrand Abilities", "Slash Hotkey", new KeyboardShortcut(KeyCode.None), "Legacy fallback hotkey for Frostbrand Surt Slash. The primary input is the game's secondary attack.");
            FrostbrandCrushHotkey = Config.Bind("Frostbrand Abilities", "Crush Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Frostbrand Surt Crush.");
            FrostbrandElementalShieldHotkey = Config.Bind("Frostbrand Abilities", "Elemental Shield Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Frostbrand Surt Elemental Shield. Hold block with this hotkey.");
            FrostbrandSlashCooldown = Config.Bind("Frostbrand Abilities", "Slash Cooldown", 8f, new ConfigDescription("Cooldown in seconds for Frostbrand Surt Slash.", new AcceptableValueRange<float>(0f, 120f)));
            FrostbrandSlashEitrUse = Config.Bind("Frostbrand Abilities", "Slash Eitr Use", 30f, new ConfigDescription("Eitr consumed by Frostbrand Surt Slash.", new AcceptableValueRange<float>(0f, 250f)));
            FrostbrandSlashRange = Config.Bind("Frostbrand Abilities", "Slash Range", 10f, new ConfigDescription("Range in meters for Frostbrand Surt Slash.", new AcceptableValueRange<float>(1f, 50f)));
            FrostbrandSlashRadius = Config.Bind("Frostbrand Abilities", "Slash Radius", 4f, new ConfigDescription("Half-width radius for Frostbrand Surt Slash cone.", new AcceptableValueRange<float>(0.5f, 20f)));
            FrostbrandSlashBaseDamage = Config.Bind("Frostbrand Abilities", "Slash Base Damage", 50f, new ConfigDescription("Base fire/slash damage for Frostbrand Surt Slash before Elemental Magic scaling.", new AcceptableValueRange<float>(0f, 2000f)));
            FrostbrandSlashDamagePerElementalMagicLevel = Config.Bind("Frostbrand Abilities", "Slash Damage Per Elemental Magic Level", 0.9f, new ConfigDescription("Extra fire/slash damage per Elemental Magic level for Frostbrand Surt Slash.", new AcceptableValueRange<float>(0f, 30f)));
            FrostbrandCrushCooldown = Config.Bind("Frostbrand Abilities", "Crush Cooldown", 14f, new ConfigDescription("Cooldown in seconds for Frostbrand Surt Crush.", new AcceptableValueRange<float>(0f, 180f)));
            FrostbrandCrushEitrUse = Config.Bind("Frostbrand Abilities", "Crush Eitr Use", 45f, new ConfigDescription("Eitr consumed by Frostbrand Surt Crush.", new AcceptableValueRange<float>(0f, 250f)));
            FrostbrandCrushRange = Config.Bind("Frostbrand Abilities", "Crush Range", 16f, new ConfigDescription("Leap range in meters for Frostbrand Surt Crush.", new AcceptableValueRange<float>(1f, 60f)));
            FrostbrandCrushRadius = Config.Bind("Frostbrand Abilities", "Crush Radius", 5f, new ConfigDescription("Impact radius in meters for Frostbrand Surt Crush.", new AcceptableValueRange<float>(0.5f, 30f)));
            FrostbrandCrushBaseDamage = Config.Bind("Frostbrand Abilities", "Crush Base Damage", 65f, new ConfigDescription("Base fire/blunt damage for Frostbrand Surt Crush before Elemental Magic scaling.", new AcceptableValueRange<float>(0f, 3000f)));
            FrostbrandCrushDamagePerElementalMagicLevel = Config.Bind("Frostbrand Abilities", "Crush Damage Per Elemental Magic Level", 1.1f, new ConfigDescription("Extra fire/blunt damage per Elemental Magic level for Frostbrand Surt Crush.", new AcceptableValueRange<float>(0f, 40f)));
            FrostbrandBurningGroundDuration = Config.Bind("Frostbrand Abilities", "Burning Ground Duration", 10f, new ConfigDescription("Duration in seconds for Burning Ground left by Frostbrand Crush.", new AcceptableValueRange<float>(0.5f, 60f)));
            FrostbrandBurningGroundTickInterval = Config.Bind("Frostbrand Abilities", "Burning Ground Tick Interval", 1f, new ConfigDescription("Seconds between Burning Ground damage ticks.", new AcceptableValueRange<float>(0.1f, 10f)));
            FrostbrandBurningGroundBaseDamage = Config.Bind("Frostbrand Abilities", "Burning Ground Base Damage", 16f, new ConfigDescription("Base fire damage per Burning Ground tick before Elemental Magic scaling.", new AcceptableValueRange<float>(0f, 2000f)));
            FrostbrandBurningGroundDamagePerElementalMagicLevel = Config.Bind("Frostbrand Abilities", "Burning Ground Damage Per Elemental Magic Level", 0.32f, new ConfigDescription("Extra fire damage per Elemental Magic level for Burning Ground ticks.", new AcceptableValueRange<float>(0f, 20f)));
            FrostbrandElementalShieldCooldown = Config.Bind("Frostbrand Abilities", "Elemental Shield Cooldown", 24f, new ConfigDescription("Cooldown in seconds for Frostbrand Elemental Shield.", new AcceptableValueRange<float>(0f, 300f)));
            FrostbrandElementalShieldEitrUse = Config.Bind("Frostbrand Abilities", "Elemental Shield Eitr Use", 50f, new ConfigDescription("Eitr consumed by Frostbrand Elemental Shield.", new AcceptableValueRange<float>(0f, 250f)));
            FrostbrandElementalShieldDuration = Config.Bind("Frostbrand Abilities", "Elemental Shield Duration", 8f, new ConfigDescription("Duration in seconds for Frostbrand Elemental Shield.", new AcceptableValueRange<float>(0.5f, 60f)));
            FrostbrandElementalShieldBaseMitigation = Config.Bind("Frostbrand Abilities", "Elemental Shield Base Mitigation", 0.25f, new ConfigDescription("All-damage mitigation for Frostbrand Elemental Shield. 0.25 means 25%. Fire damage is always fully negated.", new AcceptableValueRange<float>(0f, 1f)));
            FrostbrandElementalShieldMitigationPerElementalMagicLevel = Config.Bind("Frostbrand Abilities", "Elemental Shield Mitigation Per Elemental Magic Level", 0.003f, new ConfigDescription("Extra all-damage mitigation per Elemental Magic level for Frostbrand Elemental Shield.", new AcceptableValueRange<float>(0f, 0.05f)));
            FrostbrandElementalShieldMaxMitigation = Config.Bind("Frostbrand Abilities", "Elemental Shield Max Mitigation", 0.70f, new ConfigDescription("Maximum all-damage mitigation for Frostbrand Elemental Shield before fire immunity.", new AcceptableValueRange<float>(0f, 1f)));
            UpgradeFloatConfig(FrostbrandWaterSphereCooldown, 10f, 20f);
            UpgradeFloatConfig(FrostbrandWaterSphereMaxDuration, 12f, 8f);
            UpgradeShortcutConfig(FrostbrandWaterSphereHotkey, KeyCode.Mouse4, KeyCode.Mouse3);
            UpgradeShortcutConfig(FrostbrandSlashHotkey, KeyCode.G, KeyCode.None);
            UpgradeShortcutConfig(FrostbrandCrushHotkey, KeyCode.R, KeyCode.Mouse4);
            UpgradeShortcutConfig(FrostbrandElementalShieldHotkey, KeyCode.T, KeyCode.Mouse4);
            UpgradeFloatConfig(FrostbrandBurningGroundDuration, 6f, 10f);

            EnableHraesvelgrAbilities = Config.Bind("Hraesvelgr Abilities", "Enable Hraesvelgr Abilities", true, "Enable the complete-set abilities for the Hraesvelgr archer set.");
            HraesvelgrVolleyHotkey = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Hraesvelgr rapid volley.");
            HraesvelgrSummonHotkey = Config.Bind("Hraesvelgr Abilities", "Summon Beasts Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Hraesvelgr summon beasts.");
            HraesvelgrSneakyNoiseModifier = Config.Bind("Hraesvelgr Abilities", "Sneaky Noise Modifier", 0.5f, new ConfigDescription("Noise multiplier while Hraesvelgr Sneaky is active. 0.5 means 50% noise.", new AcceptableValueRange<float>(0.05f, 1f)));
            HraesvelgrSneakyStealthModifier = Config.Bind("Hraesvelgr Abilities", "Sneaky Stealth Modifier", 0.3f, new ConfigDescription("Stealth multiplier while Hraesvelgr Sneaky is active. Lower values make enemies less likely to see the player.", new AcceptableValueRange<float>(0.05f, 1f)));
            HraesvelgrSneakySpeedModifier = Config.Bind("Hraesvelgr Abilities", "Sneaky Speed Modifier", 0.15f, new ConfigDescription("Movement speed modifier while Hraesvelgr Sneaky is active. 0.15 means +15%.", new AcceptableValueRange<float>(0f, 2f)));
            HraesvelgrVolleyDuration = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Duration", 3f, new ConfigDescription("Duration in seconds for Hraesvelgr rapid volley channel.", new AcceptableValueRange<float>(1f, 60f)));
            HraesvelgrVolleyCooldown = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Cooldown", 18f, new ConfigDescription("Cooldown in seconds for Hraesvelgr rapid volley.", new AcceptableValueRange<float>(0f, 300f)));
            HraesvelgrVolleyAttackSpeedMultiplier = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Attack Speed Multiplier", 100f, new ConfigDescription("Multiplier applied to bow attack update speed while Rapid Volley is active. Very high values make the bow behave like a machine-gun volley beyond visible EpicLoot caps.", new AcceptableValueRange<float>(1f, 300f)));
            HraesvelgrVolleyProjectileSpeedMultiplier = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Projectile Speed Multiplier", 6f, new ConfigDescription("Multiplier applied to arrow velocity while Rapid Volley is active. This simulates +100% projectile speed with an extra burst multiplier so it is clearly noticeable.", new AcceptableValueRange<float>(1f, 100f)));
            HraesvelgrVolleyStaminaUseMultiplier = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Stamina Use Multiplier", 0.05f, new ConfigDescription("Multiplier applied to bow stamina cost while Rapid Volley is active. 0.05 means 5% cost.", new AcceptableValueRange<float>(0f, 1f)));
            HraesvelgrVolleyShotsPerSecond = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Shots Per Second", 8f, new ConfigDescription("Automatic arrows fired per second while Rapid Volley is active and the attack button is held.", new AcceptableValueRange<float>(1f, 30f)));
            HraesvelgrVolleyDamageMultiplier = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Damage Multiplier", 0.5f, new ConfigDescription("Damage multiplier for arrows fired while Rapid Volley is active. 0.5 means 50% less damage.", new AcceptableValueRange<float>(0.05f, 5f)));
            HraesvelgrVolleyProjectileVelocity = Config.Bind("Hraesvelgr Abilities", "Rapid Volley Projectile Velocity", 90f, new ConfigDescription("Projectile velocity for automatic Rapid Volley arrows.", new AcceptableValueRange<float>(5f, 250f)));
            HraesvelgrDashCooldown = Config.Bind("Hraesvelgr Abilities", "Dash Cooldown", 10f, new ConfigDescription("Cooldown in seconds for Hraesvelgr Freyja dash on secondary attack.", new AcceptableValueRange<float>(0f, 120f)));
            HraesvelgrDashEitrUse = Config.Bind("Hraesvelgr Abilities", "Dash Eitr Use", 20f, new ConfigDescription("Eitr consumed by Hraesvelgr Freyja dash.", new AcceptableValueRange<float>(0f, 250f)));
            HraesvelgrSummonCooldown = Config.Bind("Hraesvelgr Abilities", "Summon Beasts Cooldown", 0f, new ConfigDescription("Legacy cooldown for Hraesvelgr summon beasts. The current ability is a no-cooldown toggle.", new AcceptableValueRange<float>(0f, 600f)));
            HraesvelgrSummonDuration = Config.Bind("Hraesvelgr Abilities", "Summon Beasts Duration", 0f, new ConfigDescription("Legacy lifetime for summoned beasts. The current summons stay until killed, set loss, logout cleanup, or manual store.", new AcceptableValueRange<float>(0f, 600f)));
            HraesvelgrSummonStaminaUse = Config.Bind("Hraesvelgr Abilities", "Summon Beasts Stamina Use", 35f, new ConfigDescription("Stamina consumed by Hraesvelgr summon beasts.", new AcceptableValueRange<float>(0f, 250f)));
            HraesvelgrSummonGuardRadius = Config.Bind("Hraesvelgr Abilities", "Summon Beasts Guard Radius", 30f, new ConfigDescription("Radius in meters around the owner where summoned beasts search hostile targets to defend the owner.", new AcceptableValueRange<float>(5f, 100f)));
            HraesvelgrTrapHotkey = Config.Bind("Hraesvelgr Abilities", "Trap Hotkey", new KeyboardShortcut(KeyCode.Mouse3, KeyCode.LeftControl), "Hotkey for Hraesvelgr armed trap. Defaults to Ctrl + Mouse3 so Summon Beasts remains on Mouse3.");
            HraesvelgrTrapStaminaUse = Config.Bind("Hraesvelgr Abilities", "Trap Stamina Use", 20f, new ConfigDescription("Stamina/vigor consumed when placing an armed Hraesvelgr trap.", new AcceptableValueRange<float>(0f, 250f)));
            HraesvelgrTrapMaxCharges = Config.Bind("Hraesvelgr Abilities", "Trap Max Charges", 5, new ConfigDescription("Maximum armed trap charges Hraesvelgr can store.", new AcceptableValueRange<int>(1, 20)));
            HraesvelgrTrapRechargeSeconds = Config.Bind("Hraesvelgr Abilities", "Trap Recharge Seconds", 60f, new ConfigDescription("Seconds required to recover one Hraesvelgr trap charge.", new AcceptableValueRange<float>(1f, 600f)));
            HraesvelgrTrapBuffDuration = Config.Bind("Hraesvelgr Abilities", "Trap Damage Buff Duration", 30f, new ConfigDescription("Duration in seconds for the next-shot damage buff granted when an enemy triggers your trap.", new AcceptableValueRange<float>(1f, 300f)));
            HraesvelgrTrapNextAttackDamageBonus = Config.Bind("Hraesvelgr Abilities", "Trap Next Attack Damage Bonus", 0.50f, new ConfigDescription("Damage bonus for the next Hraesvelgr bow attack after a trap catches an enemy. 0.50 means +50%.", new AcceptableValueRange<float>(0f, 10f)));
            HraesvelgrHeadshotAttackCount = Config.Bind("Hraesvelgr Abilities", "Headshot Passive Attack Count", 3, new ConfigDescription("Every this many Hraesvelgr bow shots, the fired shot is treated as a headshot/weak spot hit.", new AcceptableValueRange<int>(1, 20)));
            HraesvelgrHeadshotDamageMultiplier = Config.Bind("Hraesvelgr Abilities", "Headshot Passive Damage Multiplier", 1.5f, new ConfigDescription("Fallback damage multiplier for the Hraesvelgr headshot passive when Valheim's weak spot flag is unavailable.", new AcceptableValueRange<float>(1f, 10f)));
            UpgradeFloatConfig(HraesvelgrVolleyDuration, 8f, 16f);
            UpgradeFloatConfig(HraesvelgrVolleyDuration, 16f, 10f);
            UpgradeFloatConfig(HraesvelgrVolleyDuration, 10f, 3f);
            UpgradeFloatConfig(HraesvelgrVolleyCooldown, 35f, 18f);
            UpgradeFloatConfig(HraesvelgrVolleyAttackSpeedMultiplier, 2.4f, 7f);
            UpgradeFloatConfig(HraesvelgrVolleyAttackSpeedMultiplier, 7f, 20f);
            UpgradeFloatConfig(HraesvelgrVolleyAttackSpeedMultiplier, 20f, 100f);
            UpgradeFloatConfig(HraesvelgrVolleyProjectileSpeedMultiplier, 2.5f, 4f);
            UpgradeFloatConfig(HraesvelgrVolleyProjectileSpeedMultiplier, 4f, 6f);
            UpgradeFloatConfig(HraesvelgrVolleyStaminaUseMultiplier, 0.25f, 0.05f);
            UpgradeFloatConfig(HraesvelgrVolleyDamageMultiplier, 0.65f, 0.5f);
            UpgradeFloatConfig(HraesvelgrSummonCooldown, 60f, 0f);
            UpgradeFloatConfig(HraesvelgrSummonDuration, 60f, 0f);

            EnableSolomonKaneAbilities = Config.Bind("Solomon Kane Abilities", "Enable Solomon Kane Abilities", true, "Enable the complete-set abilities for the Solomon Kane crossbow set.");
            SolomonKaneInfusedBoltHotkey = Config.Bind("Solomon Kane Abilities", "Infused Bolt Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Solomon Kane Infused Bolt. It arms the next crossbow bolt.");
            SolomonKaneBombHotkey = Config.Bind("Solomon Kane Abilities", "Blackpowder Bomb Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Solomon Kane Blackpowder Bomb.");
            SolomonKaneBatFormHotkey = Config.Bind("Solomon Kane Abilities", "Bat Form Hotkey", new KeyboardShortcut(KeyCode.Mouse4, KeyCode.LeftControl), "Hotkey for Solomon Kane Bat Form. Press again while transformed to cancel it.");
            SolomonKaneInfusedBoltStaminaUse = Config.Bind("Solomon Kane Abilities", "Infused Bolt Stamina Use", 25f, new ConfigDescription("Stamina/vigor consumed when arming Infused Bolt.", new AcceptableValueRange<float>(0f, 250f)));
            SolomonKaneInfusedBoltCooldown = Config.Bind("Solomon Kane Abilities", "Infused Bolt Cooldown", 12f, new ConfigDescription("Cooldown in seconds for Infused Bolt.", new AcceptableValueRange<float>(0f, 300f)));
            SolomonKaneInfusedBoltDuration = Config.Bind("Solomon Kane Abilities", "Infused Bolt Armed Duration", 8f, new ConfigDescription("Seconds the next bolt can stay armed after pressing the Infused Bolt hotkey.", new AcceptableValueRange<float>(0.5f, 60f)));
            SolomonKaneInfusedBoltRadius = Config.Bind("Solomon Kane Abilities", "Infused Bolt Radius", 3.5f, new ConfigDescription("Explosion radius in meters for Infused Bolt.", new AcceptableValueRange<float>(0.1f, 25f)));
            SolomonKaneInfusedBoltBaseDamage = Config.Bind("Solomon Kane Abilities", "Infused Bolt Base Damage", 24f, new ConfigDescription("Base frost/spirit damage for Infused Bolt before Crossbows scaling and EpicLoot modifiers.", new AcceptableValueRange<float>(0f, 2000f)));
            SolomonKaneInfusedBoltDamagePerCrossbowsLevel = Config.Bind("Solomon Kane Abilities", "Infused Bolt Damage Per Crossbows Level", 0.55f, new ConfigDescription("Extra Infused Bolt damage per Crossbows skill level before EpicLoot modifiers.", new AcceptableValueRange<float>(0f, 30f)));
            SolomonKaneInfusedBoltSlow = Config.Bind("Solomon Kane Abilities", "Infused Bolt Slow", 0.35f, new ConfigDescription("Movement slow applied by Infused Bolt. 0.35 means 35% slow.", new AcceptableValueRange<float>(0f, 0.95f)));
            SolomonKaneInfusedBoltSlowDuration = Config.Bind("Solomon Kane Abilities", "Infused Bolt Slow Duration", 4f, new ConfigDescription("Duration in seconds for Infused Bolt slow.", new AcceptableValueRange<float>(0.1f, 60f)));
            SolomonKaneBombStaminaUse = Config.Bind("Solomon Kane Abilities", "Blackpowder Bomb Stamina Use", 35f, new ConfigDescription("Stamina/vigor consumed by Blackpowder Bomb.", new AcceptableValueRange<float>(0f, 250f)));
            SolomonKaneBombCooldown = Config.Bind("Solomon Kane Abilities", "Blackpowder Bomb Cooldown", 18f, new ConfigDescription("Cooldown in seconds for Blackpowder Bomb.", new AcceptableValueRange<float>(0f, 300f)));
            SolomonKaneBombRange = Config.Bind("Solomon Kane Abilities", "Blackpowder Bomb Range", 35f, new ConfigDescription("Maximum aiming range for Blackpowder Bomb.", new AcceptableValueRange<float>(1f, 100f)));
            SolomonKaneBombRadius = Config.Bind("Solomon Kane Abilities", "Blackpowder Bomb Radius", 5f, new ConfigDescription("Explosion radius in meters for Blackpowder Bomb.", new AcceptableValueRange<float>(0.1f, 30f)));
            SolomonKaneBombBaseDamage = Config.Bind("Solomon Kane Abilities", "Blackpowder Bomb Base Damage", 38f, new ConfigDescription("Base blunt/fire damage for Blackpowder Bomb before Crossbows scaling and EpicLoot modifiers.", new AcceptableValueRange<float>(0f, 3000f)));
            SolomonKaneBombDamagePerCrossbowsLevel = Config.Bind("Solomon Kane Abilities", "Blackpowder Bomb Damage Per Crossbows Level", 0.65f, new ConfigDescription("Extra Blackpowder Bomb damage per Crossbows skill level before EpicLoot modifiers.", new AcceptableValueRange<float>(0f, 30f)));
            SolomonKaneBombImpactForce = Config.Bind("Solomon Kane Abilities", "Blackpowder Bomb Impact Force", 120f, new ConfigDescription("Push force applied by Blackpowder Bomb hits.", new AcceptableValueRange<float>(0f, 1000f)));
            SolomonKaneBatFormStaminaUse = Config.Bind("Solomon Kane Abilities", "Bat Form Stamina Use", 40f, new ConfigDescription("Stamina/vigor consumed when activating Bat Form.", new AcceptableValueRange<float>(0f, 250f)));
            SolomonKaneBatFormCooldown = Config.Bind("Solomon Kane Abilities", "Bat Form Cooldown", 45f, new ConfigDescription("Cooldown in seconds for Bat Form.", new AcceptableValueRange<float>(0f, 600f)));
            SolomonKaneBatFormDuration = Config.Bind("Solomon Kane Abilities", "Bat Form Duration", 12f, new ConfigDescription("Duration in seconds for Bat Form flight.", new AcceptableValueRange<float>(0.5f, 120f)));
            SolomonKaneBatFormFlightSpeed = Config.Bind("Solomon Kane Abilities", "Bat Form Flight Speed", 9f, new ConfigDescription("Base debug-flight speed used while Bat Form is active. Holding run flies faster.", new AcceptableValueRange<float>(1f, 40f)));
            SolomonKaneBatFormVisualScale = Config.Bind("Solomon Kane Abilities", "Bat Form Visual Scale", 1.35f, new ConfigDescription("Scale multiplier applied to the attached bat visual.", new AcceptableValueRange<float>(0.1f, 5f)));
            SolomonKaneWitchmarkDuration = Config.Bind("Solomon Kane Abilities", "Witchmark Duration", 8f, new ConfigDescription("Seconds a direct Solomon Kane crossbow hit keeps an enemy marked.", new AcceptableValueRange<float>(0.1f, 120f)));
            SolomonKaneSilverVerdictBaseDamage = Config.Bind("Solomon Kane Abilities", "Silver Verdict Base Damage", 30f, new ConfigDescription("Base spirit damage added by Silver Verdict before Crossbows scaling and EpicLoot modifiers.", new AcceptableValueRange<float>(0f, 3000f)));
            SolomonKaneSilverVerdictDamagePerCrossbowsLevel = Config.Bind("Solomon Kane Abilities", "Silver Verdict Damage Per Crossbows Level", 0.60f, new ConfigDescription("Extra Silver Verdict damage per Crossbows skill level before EpicLoot modifiers.", new AcceptableValueRange<float>(0f, 30f)));
            SolomonKaneSilverVerdictTargetCount = Config.Bind("Solomon Kane Abilities", "Silver Verdict Target Count", 2, new ConfigDescription("Maximum extra enemies hit by the Silver Verdict chain.", new AcceptableValueRange<int>(0, 10)));
            SolomonKaneSilverVerdictSeekRadius = Config.Bind("Solomon Kane Abilities", "Silver Verdict Seek Radius", 15f, new ConfigDescription("Radius in meters for Silver Verdict to find chain targets.", new AcceptableValueRange<float>(1f, 60f)));
            SolomonKaneSilverVerdictDamageLossPerJump = Config.Bind("Solomon Kane Abilities", "Silver Verdict Damage Loss Per Jump", 0.30f, new ConfigDescription("Damage lost per Silver Verdict chain jump. 0.30 means 30% less per jump.", new AcceptableValueRange<float>(0f, 0.95f)));
            SolomonKaneSilverVerdictStaminaRefund = Config.Bind("Solomon Kane Abilities", "Silver Verdict Stamina Refund", 15f, new ConfigDescription("Stamina/vigor restored when Silver Verdict is consumed by a bolt.", new AcceptableValueRange<float>(0f, 250f)));

            EnableNottAbilities = Config.Bind("Nott Abilities", "Enable Nott Abilities", true, "Enable the complete-set abilities for the Nott assassin set.");
            NottWarpHotkey = Config.Bind("Nott Abilities", "Warp Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Nott Warp.");
            NottSneakyNoiseModifier = Config.Bind("Nott Abilities", "Sneaky Noise Modifier", 0.4f, new ConfigDescription("Noise multiplier while Nott Sneaky is active. 0.4 means 40% noise.", new AcceptableValueRange<float>(0.05f, 1f)));
            NottSneakyStealthModifier = Config.Bind("Nott Abilities", "Sneaky Stealth Modifier", 0.2f, new ConfigDescription("Stealth multiplier while Nott Sneaky is active. Lower values make enemies less likely to see the player.", new AcceptableValueRange<float>(0.05f, 1f)));
            NottSneakySpeedModifier = Config.Bind("Nott Abilities", "Sneaky Speed Modifier", 0.2f, new ConfigDescription("Movement speed modifier while Nott Sneaky is active. 0.2 means +20%.", new AcceptableValueRange<float>(0f, 2f)));
            NottWarpRange = Config.Bind("Nott Abilities", "Warp Range", 18f, new ConfigDescription("Maximum safe teleport distance for Nott Warp.", new AcceptableValueRange<float>(2f, 50f)));
            NottWarpCooldown = Config.Bind("Nott Abilities", "Warp Cooldown", 18f, new ConfigDescription("Cooldown in seconds for Nott Warp. The cooldown resets if a nearby enemy dies while it is cooling down.", new AcceptableValueRange<float>(0f, 300f)));
            NottWarpEitrUse = Config.Bind("Nott Abilities", "Warp Eitr Use", 0f, new ConfigDescription("Legacy setting. Nott Warp now consumes stamina/vigor instead of eitr.", new AcceptableValueRange<float>(0f, 250f)));
            NottWarpStaminaUse = Config.Bind("Nott Abilities", "Warp Stamina Use", 25f, new ConfigDescription("Stamina/vigor consumed by Nott Warp.", new AcceptableValueRange<float>(0f, 250f)));
            NottWarpDamageMultiplier = Config.Bind("Nott Abilities", "Warp Next Attack Damage Multiplier", 2.5f, new ConfigDescription("Damage multiplier for the first attack after Nott Warp.", new AcceptableValueRange<float>(1f, 20f)));
            NottWarpResetRadius = Config.Bind("Nott Abilities", "Warp Cooldown Reset Radius", 18f, new ConfigDescription("Enemy death radius in meters that refreshes Warp while it is cooling down.", new AcceptableValueRange<float>(1f, 100f)));
            NottHitSpeedBonus = Config.Bind("Nott Abilities", "Hit Speed Bonus Per Stack", 0.30f, new ConfigDescription("Movement speed bonus gained when Nott hits an enemy. 0.30 means +30%. This no longer stacks.", new AcceptableValueRange<float>(0f, 2f)));
            NottHitSpeedDuration = Config.Bind("Nott Abilities", "Hit Speed Bonus Duration", 5f, new ConfigDescription("Duration in seconds for Nott Shadow Momentum.", new AcceptableValueRange<float>(0.1f, 60f)));
            NottHitSpeedMaxStacks = Config.Bind("Nott Abilities", "Hit Speed Bonus Max Stacks", 1, new ConfigDescription("Legacy stack cap for Nott Shadow Momentum. Current behavior is a single refreshed +30% buff.", new AcceptableValueRange<int>(1, 20)));
            NottPoisonBaseDamage = Config.Bind("Nott Abilities", "Poison Base Damage", 8f, new ConfigDescription("Poison damage added by Nott attacks before Knives scaling.", new AcceptableValueRange<float>(0f, 1000f)));
            NottPoisonDamagePerKnivesLevel = Config.Bind("Nott Abilities", "Poison Damage Per Knives Level", 0.35f, new ConfigDescription("Extra poison damage added per Knives skill level by Nott attacks.", new AcceptableValueRange<float>(0f, 30f)));
            UpgradeFloatConfig(NottWarpRange, 12f, 18f);
            UpgradeFloatConfig(NottHitSpeedBonus, 0.10f, 0.30f);
            UpgradeIntConfig(NottHitSpeedMaxStacks, 3, 1);

            EnableRagnarAbilities = Config.Bind("Ragnar Abilities", "Enable Ragnar Abilities", true, "Enable the complete-set abilities for the Ragnar berserker set.");
            RagnarDecayAuraHotkey = Config.Bind("Ragnar Abilities", "Decay Aura Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Ragnar Decay Aura toggle.");
            RagnarDecayAuraRadius = Config.Bind("Ragnar Abilities", "Decay Aura Radius", 6f, new ConfigDescription("Radius in meters for Ragnar Decay Aura.", new AcceptableValueRange<float>(1f, 40f)));
            RagnarDecayAuraStaminaPerSecond = Config.Bind("Ragnar Abilities", "Decay Aura Stamina Per Second", 8f, new ConfigDescription("Stamina consumed per second while Ragnar Decay Aura is active.", new AcceptableValueRange<float>(0f, 100f)));
            RagnarDecayAuraBaseDamage = Config.Bind("Ragnar Abilities", "Decay Aura Base Damage", 8f, new ConfigDescription("Base poison/spirit damage per Decay Aura tick before Axes scaling.", new AcceptableValueRange<float>(0f, 1000f)));
            RagnarDecayAuraDamagePerAxesLevel = Config.Bind("Ragnar Abilities", "Decay Aura Damage Per Axes Level", 0.35f, new ConfigDescription("Extra poison/spirit damage per Axes level for Ragnar Decay Aura.", new AcceptableValueRange<float>(0f, 20f)));
            RagnarDecayAuraTickInterval = Config.Bind("Ragnar Abilities", "Decay Aura Tick Interval", 1f, new ConfigDescription("Seconds between Ragnar Decay Aura damage ticks.", new AcceptableValueRange<float>(0.1f, 10f)));
            RagnarFuryAttackSpeedPerStack = Config.Bind("Ragnar Abilities", "Fury Attack Speed Per Stack", 0.03f, new ConfigDescription("Attack speed bonus gained per Ragnar Fury stack. 0.03 means +3%.", new AcceptableValueRange<float>(0f, 2f)));
            RagnarFuryLifeStealPerStack = Config.Bind("Ragnar Abilities", "Fury Life Steal Per Stack", 0.005f, new ConfigDescription("Life steal gained per Ragnar Fury stack. 0.005 means 0.5% of outgoing hit damage.", new AcceptableValueRange<float>(0f, 1f)));
            RagnarFuryDuration = Config.Bind("Ragnar Abilities", "Fury Stack Duration", 6f, new ConfigDescription("Duration in seconds for Ragnar Fury stacks after each melee hit.", new AcceptableValueRange<float>(0.1f, 60f)));
            RagnarFuryMaxStacks = Config.Bind("Ragnar Abilities", "Fury Max Stacks", 10, new ConfigDescription("Maximum Ragnar Fury stacks.", new AcceptableValueRange<int>(1, 50)));
            RagnarFuryHealEveryAttacks = Config.Bind("Ragnar Abilities", "Fury Heal Every Attacks", 3, new ConfigDescription("Every this many melee hits Ragnar heals for a fraction of max health.", new AcceptableValueRange<int>(1, 50)));
            RagnarFuryHealMaxHealthFraction = Config.Bind("Ragnar Abilities", "Fury Heal Max Health Fraction", 0.05f, new ConfigDescription("Fraction of max health restored by Ragnar Fury periodic heal. 0.05 means 5%.", new AcceptableValueRange<float>(0f, 1f)));

            MagicSetDropChance = Config.Bind("Drop Chances", "Magic Set Drop Chance", 0.12f, new ConfigDescription("Chance for a magic item roll to become a configured Magic set item.", new AcceptableValueRange<float>(0f, 1f)));
            RareSetDropChance = Config.Bind("Drop Chances", "Rare Set Drop Chance", 0.10f, new ConfigDescription("Chance for a rare item roll to become a configured Rare set item.", new AcceptableValueRange<float>(0f, 1f)));
            EpicSetDropChance = Config.Bind("Drop Chances", "Epic Set Drop Chance", 0.08f, new ConfigDescription("Chance for an epic item roll to become a configured Epic set item.", new AcceptableValueRange<float>(0f, 1f)));
            AncientSetDropChance = Config.Bind("Drop Chances", "Ancient Set Drop Chance", 0.05f, new ConfigDescription("Chance for an ancient item roll to become a configured Ancient set item.", new AcceptableValueRange<float>(0f, 1f)));
            MoonveinMagicBowEitrUse = Config.Bind("Moonvein", "Magic Bow Eitr Use", 6f, new ConfigDescription("Base eitr cost for each Moonvein bow shot before ModifyAttackEitrUse reductions.", new AcceptableValueRange<float>(0f, 100f)));
            MoonveinRareBowEitrUse = Config.Bind("Moonvein", "Rare Bow Eitr Use", 8f, new ConfigDescription("Base eitr cost for each Moonvein bow shot before ModifyAttackEitrUse reductions.", new AcceptableValueRange<float>(0f, 100f)));
            MoonveinEpicBowEitrUse = Config.Bind("Moonvein", "Epic Bow Eitr Use", 10f, new ConfigDescription("Base eitr cost for each Moonvein bow shot before ModifyAttackEitrUse reductions.", new AcceptableValueRange<float>(0f, 100f)));
            MoonveinLegendaryBowEitrUse = Config.Bind("Moonvein", "Legendary Bow Eitr Use", 12f, new ConfigDescription("Base eitr cost for each Moonvein bow shot before ModifyAttackEitrUse reductions.", new AcceptableValueRange<float>(0f, 100f)));
            MoonveinMythicBowEitrUse = Config.Bind("Moonvein", "Mythic Bow Eitr Use", 14f, new ConfigDescription("Base eitr cost for each Moonvein bow shot before ModifyAttackEitrUse reductions.", new AcceptableValueRange<float>(0f, 100f)));
            MoonveinAncientBowEitrUse = Config.Bind("Moonvein", "Ancient Bow Eitr Use", 16f, new ConfigDescription("Base eitr cost for each Moonvein bow shot before ModifyAttackEitrUse reductions.", new AcceptableValueRange<float>(0f, 100f)));

            EnableMoonveinAbilities = Config.Bind("Moonvein Abilities", "Enable Moonvein Abilities", true, "Enable the complete-set abilities for the Moonvein magic archer set.");
            MoonveinMeteorHotkey = Config.Bind("Moonvein Abilities", "Meteor Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Moonvein meteor.");
            MoonveinTornadoHotkey = Config.Bind("Moonvein Abilities", "Tornado Shot Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Moonvein Tornado Shot. It arms your next arrow; the Njord tornado appears where that arrow lands.");
            MoonveinProjectileVelocity = Config.Bind("Moonvein Abilities", "Stack Projectile Velocity", 45f, new ConfigDescription("Velocity used by Acid Bolt, Lightning Bolt and Fireball fired from Moonvein stacks.", new AcceptableValueRange<float>(1f, 200f)));
            MoonveinAcidBoltBaseDamage = Config.Bind("Moonvein Abilities", "Acid Bolt Base Damage", 20f, new ConfigDescription("Base poison damage for Moonvein Acid Bolt before Elemental Magic scaling and EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 1000f)));
            MoonveinAcidBoltDamagePerElementalMagicLevel = Config.Bind("Moonvein Abilities", "Acid Bolt Damage Per Elemental Magic Level", 0.5f, new ConfigDescription("Extra poison damage per Elemental Magic skill level for Moonvein Acid Bolt before EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 20f)));
            MoonveinLightningBoltBaseDamage = Config.Bind("Moonvein Abilities", "Lightning Bolt Base Damage", 28f, new ConfigDescription("Base lightning damage for Moonvein Lightning Bolt before Elemental Magic scaling and EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 1000f)));
            MoonveinLightningBoltDamagePerElementalMagicLevel = Config.Bind("Moonvein Abilities", "Lightning Bolt Damage Per Elemental Magic Level", 0.65f, new ConfigDescription("Extra lightning damage per Elemental Magic skill level for Moonvein Lightning Bolt before EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 20f)));
            MoonveinFireballBaseDamage = Config.Bind("Moonvein Abilities", "Fireball Base Damage", 38f, new ConfigDescription("Base fire damage for Moonvein Fireball before Elemental Magic scaling and EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 1000f)));
            MoonveinFireballDamagePerElementalMagicLevel = Config.Bind("Moonvein Abilities", "Fireball Damage Per Elemental Magic Level", 0.8f, new ConfigDescription("Extra fire damage per Elemental Magic skill level for Moonvein Fireball before EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 20f)));
            MoonveinMeteorCooldown = Config.Bind("Moonvein Abilities", "Meteor Cooldown", 10f, new ConfigDescription("Cooldown in seconds for Moonvein meteor.", new AcceptableValueRange<float>(0f, 300f)));
            MoonveinMeteorEitrUse = Config.Bind("Moonvein Abilities", "Meteor Eitr Use", 45f, new ConfigDescription("Eitr consumed by Moonvein meteor.", new AcceptableValueRange<float>(0f, 250f)));
            MoonveinMeteorRange = Config.Bind("Moonvein Abilities", "Meteor Range", 60f, new ConfigDescription("Maximum aim range for Moonvein meteor.", new AcceptableValueRange<float>(1f, 150f)));
            MoonveinMeteorBaseDamage = Config.Bind("Moonvein Abilities", "Meteor Base Damage", 70f, new ConfigDescription("Base fire and blunt damage for Moonvein meteor before Elemental Magic scaling and EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 2000f)));
            MoonveinMeteorDamagePerElementalMagicLevel = Config.Bind("Moonvein Abilities", "Meteor Damage Per Elemental Magic Level", 1.2f, new ConfigDescription("Extra fire and blunt damage per Elemental Magic skill level for Moonvein meteor before EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 30f)));
            MoonveinMeteorImpactForce = Config.Bind("Moonvein Abilities", "Meteor Impact Force", 80f, new ConfigDescription("Impact force used by Moonvein meteor.", new AcceptableValueRange<float>(0f, 500f)));
            MoonveinTornadoCooldown = Config.Bind("Moonvein Abilities", "Tornado Shot Cooldown", 12f, new ConfigDescription("Cooldown in seconds for Moonvein Tornado Shot.", new AcceptableValueRange<float>(0f, 300f)));
            MoonveinTornadoEitrUse = Config.Bind("Moonvein Abilities", "Tornado Shot Eitr Use", 35f, new ConfigDescription("Eitr consumed when arming Moonvein Tornado Shot.", new AcceptableValueRange<float>(0f, 250f)));
            MoonveinTornadoDuration = Config.Bind("Moonvein Abilities", "Tornado Duration", 6f, new ConfigDescription("Duration in seconds for the Njord tornado created by Moonvein Tornado Shot.", new AcceptableValueRange<float>(0.5f, 60f)));
            MoonveinTornadoRadius = Config.Bind("Moonvein Abilities", "Tornado Radius", 4f, new ConfigDescription("Radius used by Moonvein fallback tornado damage ticks.", new AcceptableValueRange<float>(1f, 20f)));
            MoonveinTornadoBaseDamage = Config.Bind("Moonvein Abilities", "Tornado Base Damage Per Tick", 1f, new ConfigDescription("Base lightning damage per fallback tornado tick before Elemental Magic scaling and EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 1000f)));
            MoonveinTornadoDamagePerElementalMagicLevel = Config.Bind("Moonvein Abilities", "Tornado Damage Per Elemental Magic Level", 0.39f, new ConfigDescription("Extra lightning damage per fallback tornado tick per Elemental Magic skill level before EpicLoot damage modifiers.", new AcceptableValueRange<float>(0f, 20f)));
            MoonveinTornadoTickInterval = Config.Bind("Moonvein Abilities", "Tornado Tick Interval", 0.5f, new ConfigDescription("Seconds between Moonvein fallback tornado damage ticks.", new AcceptableValueRange<float>(0.1f, 5f)));
            MoonveinTornadoImpactForce = Config.Bind("Moonvein Abilities", "Tornado Impact Force", 30f, new ConfigDescription("Impact force used by Moonvein fallback tornado damage ticks.", new AcceptableValueRange<float>(0f, 500f)));
            MoonveinTornadoSlow = Config.Bind("Moonvein Abilities", "Tornado Slow", 0.40f, new ConfigDescription("Movement slow applied by Moonvein tornado hits. 0.40 means 40% slow.", new AcceptableValueRange<float>(0f, 0.95f)));
            MoonveinTornadoSlowDuration = Config.Bind("Moonvein Abilities", "Tornado Slow Duration", 6f, new ConfigDescription("Duration in seconds for Moonvein tornado slow.", new AcceptableValueRange<float>(0.1f, 30f)));
            UpgradeFloatConfig(MoonveinTornadoCooldown, 25f, 8f);
            UpgradeFloatConfig(MoonveinTornadoCooldown, 8f, 12f);
            UpgradeFloatConfig(MoonveinTornadoSlow, 0.30f, 0.40f);
            UpgradeFloatConfig(MoonveinTornadoSlowDuration, 2f, 6f);

            EnableHelveigAbilities = Config.Bind("Helveig Abilities", "Enable Helveig Abilities", true, "Enable the complete-set abilities for the Helveig blood mage set.");
            HelveigHolyHealHotkey = Config.Bind("Helveig Abilities", "Holy Heal Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Helveig Holy Heal.");
            HelveigBloodRiteHotkey = Config.Bind("Helveig Abilities", "Blood Rite Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Helveig Blood Rite channel.");
            HelveigHolyStrikeHotkey = Config.Bind("Helveig Abilities", "Holy Strike Hotkey", new KeyboardShortcut(KeyCode.None), "Legacy fallback hotkey for Helveig Holy Strike. The primary input is the game's secondary attack.");
            HelveigSummonUndeadHotkey = Config.Bind("Helveig Abilities", "Summon Undead Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Helveig Summon Undead. Hold block with this hotkey to cast it so Holy Heal stays on Mouse3.");
            HelveigHolyHealCooldown = Config.Bind("Helveig Abilities", "Holy Heal Cooldown", 10f, new ConfigDescription("Cooldown in seconds for Helveig Holy Heal.", new AcceptableValueRange<float>(0f, 120f)));
            HelveigHolyHealEitrUse = Config.Bind("Helveig Abilities", "Holy Heal Eitr Use", 25f, new ConfigDescription("Eitr consumed by Helveig Holy Heal.", new AcceptableValueRange<float>(0f, 250f)));
            HelveigHolyHealBaseHealing = Config.Bind("Helveig Abilities", "Holy Heal Base Healing", 35f, new ConfigDescription("Base healing for Helveig Holy Heal before Blood Magic scaling.", new AcceptableValueRange<float>(0f, 1000f)));
            HelveigHolyHealHealingPerBloodMagicLevel = Config.Bind("Helveig Abilities", "Holy Heal Healing Per Blood Magic Level", 0.75f, new ConfigDescription("Extra healing per Blood Magic level for Helveig Holy Heal.", new AcceptableValueRange<float>(0f, 20f)));
            HelveigHolyStrikeCooldown = Config.Bind("Helveig Abilities", "Holy Strike Cooldown", 6f, new ConfigDescription("Cooldown in seconds for Helveig Holy Strike.", new AcceptableValueRange<float>(0f, 120f)));
            HelveigHolyStrikeEitrUse = Config.Bind("Helveig Abilities", "Holy Strike Eitr Use", 25f, new ConfigDescription("Eitr consumed by Helveig Holy Strike.", new AcceptableValueRange<float>(0f, 250f)));
            HelveigHolyStrikeRange = Config.Bind("Helveig Abilities", "Holy Strike Range", 30f, new ConfigDescription("Maximum target range for Helveig Holy Strike.", new AcceptableValueRange<float>(1f, 100f)));
            HelveigHolyStrikeBaseFireDamage = Config.Bind("Helveig Abilities", "Holy Strike Base Fire Damage", 25f, new ConfigDescription("Base fire damage for Helveig Holy Strike before Blood Magic scaling.", new AcceptableValueRange<float>(0f, 2000f)));
            HelveigHolyStrikeFireDamagePerBloodMagicLevel = Config.Bind("Helveig Abilities", "Holy Strike Fire Damage Per Blood Magic Level", 0.45f, new ConfigDescription("Extra fire damage per Blood Magic level for Helveig Holy Strike.", new AcceptableValueRange<float>(0f, 30f)));
            HelveigHolyStrikeBaseSpiritDamage = Config.Bind("Helveig Abilities", "Holy Strike Base Spirit Damage", 35f, new ConfigDescription("Base spirit damage for Helveig Holy Strike before Blood Magic scaling.", new AcceptableValueRange<float>(0f, 2000f)));
            HelveigHolyStrikeSpiritDamagePerBloodMagicLevel = Config.Bind("Helveig Abilities", "Holy Strike Spirit Damage Per Blood Magic Level", 0.70f, new ConfigDescription("Extra spirit damage per Blood Magic level for Helveig Holy Strike.", new AcceptableValueRange<float>(0f, 30f)));
            HelveigBloodRiteCooldown = Config.Bind("Helveig Abilities", "Blood Rite Cooldown", 20f, new ConfigDescription("Cooldown in seconds for Helveig Blood Rite.", new AcceptableValueRange<float>(0f, 300f)));
            HelveigBloodRiteEitrUse = Config.Bind("Helveig Abilities", "Blood Rite Eitr Use", 45f, new ConfigDescription("Eitr consumed when starting Helveig Blood Rite.", new AcceptableValueRange<float>(0f, 250f)));
            HelveigBloodRiteRadius = Config.Bind("Helveig Abilities", "Blood Rite Radius", 30f, new ConfigDescription("Healing radius in meters for Helveig Blood Rite.", new AcceptableValueRange<float>(1f, 100f)));
            HelveigBloodRiteDuration = Config.Bind("Helveig Abilities", "Blood Rite Duration", 10f, new ConfigDescription("Maximum channel duration in seconds for Helveig Blood Rite.", new AcceptableValueRange<float>(0.5f, 60f)));
            HelveigBloodRiteTickInterval = Config.Bind("Helveig Abilities", "Blood Rite Tick Interval", 1f, new ConfigDescription("Seconds between Helveig Blood Rite healing ticks.", new AcceptableValueRange<float>(0.1f, 10f)));
            HelveigBloodRiteBaseHealing = Config.Bind("Helveig Abilities", "Blood Rite Base Healing Per Tick", 12f, new ConfigDescription("Base healing per tick for Helveig Blood Rite before Blood Magic scaling.", new AcceptableValueRange<float>(0f, 1000f)));
            HelveigBloodRiteHealingPerBloodMagicLevel = Config.Bind("Helveig Abilities", "Blood Rite Healing Per Blood Magic Level", 0.30f, new ConfigDescription("Extra healing per tick per Blood Magic level for Helveig Blood Rite.", new AcceptableValueRange<float>(0f, 20f)));
            HelveigBloodRiteCancelMoveDistance = Config.Bind("Helveig Abilities", "Blood Rite Cancel Move Distance", 0.65f, new ConfigDescription("Movement distance in meters that cancels Blood Rite channel.", new AcceptableValueRange<float>(0f, 5f)));
            HelveigSummonUndeadCooldown = Config.Bind("Helveig Abilities", "Summon Undead Cooldown", 60f, new ConfigDescription("Cooldown in seconds for Helveig Summon Undead.", new AcceptableValueRange<float>(0f, 300f)));
            HelveigSummonUndeadEitrUse = Config.Bind("Helveig Abilities", "Summon Undead Eitr Use", 55f, new ConfigDescription("Eitr consumed by Helveig Summon Undead.", new AcceptableValueRange<float>(0f, 250f)));
            HelveigSummonUndeadDuration = Config.Bind("Helveig Abilities", "Summon Undead Duration", 60f, new ConfigDescription("Duration in seconds for Helveig's summoned undead ally.", new AcceptableValueRange<float>(1f, 600f)));
            UpgradeFloatConfig(HelveigHolyHealCooldown, 6f, 10f);
            UpgradeShortcutConfig(HelveigHolyStrikeHotkey, KeyCode.G, KeyCode.None);

            EnableHeimdallAbilities = Config.Bind("Heimdall Abilities", "Enable Heimdall Abilities", true, "Enable the complete-set abilities for the Heimdall shield set.");
            HeimdallLightningStormHotkey = Config.Bind("Heimdall Abilities", "Lightning Storm Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Heimdall Thor Lightning Storm.");
            HeimdallStoneShieldHotkey = Config.Bind("Heimdall Abilities", "Stone Shield Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Heimdall Stone Shield.");
            HeimdallBlockArmorBonusPerStack = Config.Bind("Heimdall Abilities", "Block Armor Bonus Per Stack", 0.10f, new ConfigDescription("Damage reduction proxy granted per block stack. 0.10 means 10%.", new AcceptableValueRange<float>(0f, 1f)));
            HeimdallBlockArmorDuration = Config.Bind("Heimdall Abilities", "Block Armor Duration", 6f, new ConfigDescription("Duration in seconds for Heimdall block armor stacks.", new AcceptableValueRange<float>(0.1f, 120f)));
            HeimdallBlockArmorMaxStacks = Config.Bind("Heimdall Abilities", "Block Armor Max Stacks", 10, new ConfigDescription("Maximum Heimdall block armor stacks.", new AcceptableValueRange<int>(1, 20)));
            HeimdallLightningStormCooldown = Config.Bind("Heimdall Abilities", "Lightning Storm Cooldown", 30f, new ConfigDescription("Cooldown in seconds for Heimdall Thor Lightning Storm.", new AcceptableValueRange<float>(0f, 300f)));
            HeimdallLightningStormDuration = Config.Bind("Heimdall Abilities", "Lightning Storm Duration", 10f, new ConfigDescription("Duration in seconds for Heimdall Thor Lightning Storm and its visual effect.", new AcceptableValueRange<float>(0.5f, 60f)));
            HeimdallLightningStormRadius = Config.Bind("Heimdall Abilities", "Lightning Storm Radius", 8f, new ConfigDescription("Radius in meters around Heimdall damaged by Thor Lightning Storm.", new AcceptableValueRange<float>(1f, 40f)));
            HeimdallLightningStormBaseDamage = Config.Bind("Heimdall Abilities", "Lightning Storm Base Damage Per Tick", 24f, new ConfigDescription("Base lightning damage per Lightning Storm tick before Blocking scaling.", new AcceptableValueRange<float>(0f, 2000f)));
            HeimdallLightningStormDamagePerBlockingLevel = Config.Bind("Heimdall Abilities", "Lightning Storm Damage Per Blocking Level", 0.60f, new ConfigDescription("Extra lightning damage per Blocking level for Lightning Storm.", new AcceptableValueRange<float>(0f, 30f)));
            HeimdallLightningStormTickInterval = Config.Bind("Heimdall Abilities", "Lightning Storm Tick Interval", 1f, new ConfigDescription("Seconds between Lightning Storm damage/threat ticks.", new AcceptableValueRange<float>(0.1f, 10f)));
            HeimdallStoneShieldCooldown = Config.Bind("Heimdall Abilities", "Stone Shield Cooldown", 24f, new ConfigDescription("Cooldown in seconds for Heimdall Stone Shield.", new AcceptableValueRange<float>(0f, 300f)));
            HeimdallStoneShieldDuration = Config.Bind("Heimdall Abilities", "Stone Shield Duration", 8f, new ConfigDescription("Duration in seconds for Heimdall Stone Shield.", new AcceptableValueRange<float>(0.5f, 60f)));
            HeimdallStoneShieldStaminaUse = Config.Bind("Heimdall Abilities", "Stone Shield Stamina Use", 35f, new ConfigDescription("Stamina consumed by Heimdall Stone Shield.", new AcceptableValueRange<float>(0f, 250f)));
            HeimdallStoneShieldBaseReduction = Config.Bind("Heimdall Abilities", "Stone Shield Base Reduction", 20f, new ConfigDescription("Flat damage reduced by Heimdall Stone Shield before Blocking scaling.", new AcceptableValueRange<float>(0f, 1000f)));
            HeimdallStoneShieldReductionPerBlockingLevel = Config.Bind("Heimdall Abilities", "Stone Shield Reduction Per Blocking Level", 0.55f, new ConfigDescription("Extra flat damage reduction per Blocking level for Stone Shield.", new AcceptableValueRange<float>(0f, 30f)));
            HeimdallStoneShieldReflectBase = Config.Bind("Heimdall Abilities", "Stone Shield Reflect Base", 0.25f, new ConfigDescription("Fraction of mitigated damage reflected by Stone Shield. 0.25 means 25%.", new AcceptableValueRange<float>(0f, 5f)));
            HeimdallStoneShieldReflectPerBlockingLevel = Config.Bind("Heimdall Abilities", "Stone Shield Reflect Per Blocking Level", 0.003f, new ConfigDescription("Extra reflected fraction per Blocking level for Stone Shield.", new AcceptableValueRange<float>(0f, 0.1f)));
            HeimdallStoneShieldReflectMax = Config.Bind("Heimdall Abilities", "Stone Shield Reflect Max", 0.75f, new ConfigDescription("Maximum reflected fraction for Stone Shield.", new AcceptableValueRange<float>(0f, 5f)));
            UpgradeFloatConfig(HeimdallBlockArmorDuration, 15f, 6f);
            UpgradeIntConfig(HeimdallBlockArmorMaxStacks, 3, 10);

            EnableSeidrAbilities = Config.Bind("Seidr Abilities", "Enable Seidr Abilities", true, "Enable the complete-set abilities for the Seidr elemental mage set.");
            SeidrNanoCubeHotkey = Config.Bind("Seidr Abilities", "Nanocube Hotkey", new KeyboardShortcut(KeyCode.Mouse3), "Hotkey for Seidr Nanocube.");
            SeidrElementalShieldHotkey = Config.Bind("Seidr Abilities", "Elemental Shield Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Seidr Elemental Shield toggle.");
            SeidrStoneGolemHotkey = Config.Bind("Seidr Abilities", "Stone Golem Hotkey", new KeyboardShortcut(KeyCode.None), "Legacy fallback hotkey for Seidr Stone Golem. The primary input is the game's secondary attack.");
            SeidrFrostNovaHotkey = Config.Bind("Seidr Abilities", "Frost Nova Hotkey", new KeyboardShortcut(KeyCode.Mouse4), "Hotkey for Seidr Frost Nova. Hold block with this hotkey.");
            SeidrNanoCubeCooldown = Config.Bind("Seidr Abilities", "Nanocube Cooldown", 30f, new ConfigDescription("Cooldown in seconds for Seidr Nanocube.", new AcceptableValueRange<float>(0f, 300f)));
            SeidrNanoCubeDuration = Config.Bind("Seidr Abilities", "Nanocube Duration", 10f, new ConfigDescription("Duration in seconds for Seidr Nanocube.", new AcceptableValueRange<float>(0.5f, 60f)));
            SeidrNanoCubeEitrUse = Config.Bind("Seidr Abilities", "Nanocube Eitr Use", 50f, new ConfigDescription("Eitr consumed by Seidr Nanocube.", new AcceptableValueRange<float>(0f, 250f)));
            SeidrNanoCubeRadius = Config.Bind("Seidr Abilities", "Nanocube Radius", 5f, new ConfigDescription("Radius in meters for Seidr Nanocube area.", new AcceptableValueRange<float>(1f, 25f)));
            SeidrNanoCubeMagicDamageBonus = Config.Bind("Seidr Abilities", "Nanocube Magic Damage Bonus", 0.30f, new ConfigDescription("Magic damage bonus while the caster is inside Nanocube. 0.30 means +30%.", new AcceptableValueRange<float>(0f, 5f)));
            SeidrElementalShieldEitrPercentPerSecond = Config.Bind("Seidr Abilities", "Elemental Shield Eitr Percent Per Second", 0.02f, new ConfigDescription("Fraction of max eitr consumed per second by Seidr Elemental Shield. 0.02 means 2%.", new AcceptableValueRange<float>(0f, 1f)));
            SeidrElementalShieldCooldown = Config.Bind("Seidr Abilities", "Elemental Shield Cooldown", 0f, new ConfigDescription("Legacy cooldown for Seidr Elemental Shield. Current behavior is free toggle with no cooldown.", new AcceptableValueRange<float>(0f, 300f)));
            SeidrElementalShieldEitrUse = Config.Bind("Seidr Abilities", "Elemental Shield Eitr Use", 25f, new ConfigDescription("Eitr consumed to activate Seidr Elemental Shield.", new AcceptableValueRange<float>(0f, 250f)));
            SeidrStoneGolemCooldown = Config.Bind("Seidr Abilities", "Stone Golem Cooldown", 60f, new ConfigDescription("Cooldown in seconds for Seidr Stone Golem.", new AcceptableValueRange<float>(0f, 600f)));
            SeidrStoneGolemEitrUse = Config.Bind("Seidr Abilities", "Stone Golem Eitr Use", 80f, new ConfigDescription("Eitr consumed by Seidr Stone Golem.", new AcceptableValueRange<float>(0f, 300f)));
            SeidrStoneGolemDuration = Config.Bind("Seidr Abilities", "Stone Golem Duration", 60f, new ConfigDescription("Lifetime in seconds for Seidr Stone Golem.", new AcceptableValueRange<float>(1f, 600f)));
            SeidrFrostNovaCooldown = Config.Bind("Seidr Abilities", "Frost Nova Cooldown", 12f, new ConfigDescription("Cooldown in seconds for Seidr Frost Nova.", new AcceptableValueRange<float>(0f, 300f)));
            SeidrFrostNovaEitrUse = Config.Bind("Seidr Abilities", "Frost Nova Eitr Use", 45f, new ConfigDescription("Eitr consumed by Seidr Frost Nova.", new AcceptableValueRange<float>(0f, 250f)));
            SeidrFrostNovaRadius = Config.Bind("Seidr Abilities", "Frost Nova Radius", 8f, new ConfigDescription("Radius in meters for Seidr Frost Nova.", new AcceptableValueRange<float>(1f, 40f)));
            SeidrFrostNovaBaseDamage = Config.Bind("Seidr Abilities", "Frost Nova Base Damage", 45f, new ConfigDescription("Base frost damage for Seidr Frost Nova before Elemental Magic scaling.", new AcceptableValueRange<float>(0f, 2000f)));
            SeidrFrostNovaDamagePerElementalMagicLevel = Config.Bind("Seidr Abilities", "Frost Nova Damage Per Elemental Magic Level", 0.85f, new ConfigDescription("Extra frost damage per Elemental Magic level for Frost Nova.", new AcceptableValueRange<float>(0f, 30f)));
            SeidrFrostNovaSlow = Config.Bind("Seidr Abilities", "Frost Nova Slow", 0.40f, new ConfigDescription("Movement slow applied by Frost Nova. 0.40 means 40%.", new AcceptableValueRange<float>(0f, 0.95f)));
            SeidrFrostNovaSlowDuration = Config.Bind("Seidr Abilities", "Frost Nova Slow Duration", 5f, new ConfigDescription("Duration in seconds for Frost Nova slow.", new AcceptableValueRange<float>(0.1f, 60f)));
            UpgradeShortcutConfig(SeidrStoneGolemHotkey, KeyCode.G, KeyCode.None);
            UpgradeShortcutConfig(SeidrFrostNovaHotkey, KeyCode.R, KeyCode.Mouse4);
            UpgradeFloatConfig(SeidrElementalShieldCooldown, 12f, 0f);

            if (GenerateManagedConfigFiles.Value)
            {
                GeneratedConfigSynchronizer.Sync();
            }

            WolfPackCompatibilitySynchronizer.Sync();

            ReloadExternalConfigs();
            UniqueLegendaryHelper.OnSetupLegendaryItemConfig += ReloadExternalConfigs;

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();

            Log.LogInfo("Epic Loot Rarity Sets loaded.");
        }

        private void OnDestroy()
        {
            UniqueLegendaryHelper.OnSetupLegendaryItemConfig -= ReloadExternalConfigs;
            SetActivationBuffController.Clear(Player.m_localPlayer);
            AbilityCooldownBuffController.Clear(Player.m_localPlayer);
            FrostbrandAbilityController.Clear(Player.m_localPlayer);
            HraesvelgrAbilityController.Clear(Player.m_localPlayer);
            SolomonKaneAbilityController.Clear(Player.m_localPlayer);
            MoonveinAbilityController.Clear(Player.m_localPlayer);
            NottAbilityController.Clear(Player.m_localPlayer);
            RagnarAbilityController.Clear(Player.m_localPlayer);
            HelveigAbilityController.Clear(Player.m_localPlayer);
            HeimdallAbilityController.Clear(Player.m_localPlayer);
            SeidrAbilityController.Clear(Player.m_localPlayer);
            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
            }
        }

        private void ReloadExternalConfigs()
        {
            RaritySetRegistry.Load();
            BossSetDropController.Load();
        }

        private static void UpgradeFloatConfig(ConfigEntry<float> entry, float oldValue, float newValue)
        {
            if (entry == null)
            {
                return;
            }

            if (Mathf.Abs(entry.Value - oldValue) <= 0.001f)
            {
                entry.Value = newValue;
            }
        }

        private static void UpgradeIntConfig(ConfigEntry<int> entry, int oldValue, int newValue)
        {
            if (entry == null)
            {
                return;
            }

            if (entry.Value == oldValue)
            {
                entry.Value = newValue;
            }
        }

        private static void UpgradeShortcutConfig(ConfigEntry<KeyboardShortcut> entry, KeyCode oldKey, KeyCode newKey)
        {
            if (entry == null)
            {
                return;
            }

            KeyboardShortcut shortcut = entry.Value;
            if (shortcut.MainKey == oldKey && shortcut.Modifiers.Count() == 0)
            {
                entry.Value = new KeyboardShortcut(newKey);
            }
        }
    }

    internal static class GeneratedConfigSynchronizer
    {
        private const string ResourcePrefix = "Fran.EpicLootRaritySets.GeneratedConfig.";

        private static readonly ManagedConfigFile[] ManagedFiles =
        {
            new ManagedConfigFile(ResourcePrefix + "raritysets.json", "EpicLoot", "raritysets.json"),
            new ManagedConfigFile(ResourcePrefix + "legendaries.json", "EpicLoot", "baseconfig", "legendaries.json"),
            new ManagedConfigFile(ResourcePrefix + "loottables.json", "EpicLoot", "baseconfig", "loottables.json"),
            new ManagedConfigFile(ResourcePrefix + "iteminfo.json", "EpicLoot", "baseconfig", "iteminfo.json"),
            new ManagedConfigFile(ResourcePrefix + "magiceffects.json", "EpicLoot", "baseconfig", "magiceffects.json"),
            new ManagedConfigFile(ResourcePrefix + "adventuredata.json", "EpicLoot", "baseconfig", "adventuredata.json"),
            new ManagedConfigFile(ResourcePrefix + "bosssetdrops.json", "EpicLoot", "bosssetdrops.json"),
            new ManagedConfigFile(ResourcePrefix + "NorseDemigods.cfg", "NorseDemigods.cfg")
        };

        internal static void Sync()
        {
            int written = 0;
            foreach (ManagedConfigFile file in ManagedFiles)
            {
                try
                {
                    if (SyncFile(file))
                    {
                        written++;
                    }
                }
                catch (Exception ex)
                {
                    EpicLootRaritySetsPlugin.Log.LogError("Failed to sync managed config " + file.DisplayPath + ": " + ex);
                }
            }

            EpicLootRaritySetsPlugin.Log.LogInfo(string.Format("Managed config sync finished. Updated files: {0}.", written));
        }

        private static bool SyncFile(ManagedConfigFile file)
        {
            byte[] desired = ReadResourceBytes(file.ResourceName);
            if (desired == null || desired.Length == 0)
            {
                EpicLootRaritySetsPlugin.Log.LogWarning("Managed config resource missing or empty: " + file.ResourceName);
                return false;
            }

            string targetPath = file.GetTargetPath();
            if (File.Exists(targetPath))
            {
                byte[] current = File.ReadAllBytes(targetPath);
                if (BytesEqual(current, desired))
                {
                    return false;
                }

                BackupExisting(targetPath);
            }

            string directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(targetPath, desired);
            EpicLootRaritySetsPlugin.Log.LogInfo("Wrote managed config: " + file.DisplayPath);
            return true;
        }

        private static byte[] ReadResourceBytes(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }

                using (MemoryStream memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
        }

        private static void BackupExisting(string targetPath)
        {
            string backupBase = targetPath + ".bak-fran-managed-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string backupPath = backupBase;
            int suffix = 1;
            while (File.Exists(backupPath))
            {
                backupPath = backupBase + "-" + suffix;
                suffix++;
            }

            File.Copy(targetPath, backupPath, false);
        }

        private static bool BytesEqual(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
            {
                return false;
            }

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }

            return true;
        }

        private sealed class ManagedConfigFile
        {
            private readonly string[] _relativePathParts;

            internal ManagedConfigFile(string resourceName, params string[] relativePathParts)
            {
                ResourceName = resourceName;
                _relativePathParts = relativePathParts;
                DisplayPath = string.Join("/", relativePathParts);
            }

            internal string ResourceName { get; private set; }
            internal string DisplayPath { get; private set; }

            internal string GetTargetPath()
            {
                string path = Paths.ConfigPath;
                foreach (string part in _relativePathParts)
                {
                    path = Path.Combine(path, part);
                }

                return path;
            }
        }
    }

    internal static class WolfPackCompatibilitySynchronizer
    {
        private const string WolfPackConfigFileName = "neobotics.valheim_mod.wolfpack.cfg";

        internal static void Sync()
        {
            if (EpicLootRaritySetsPlugin.ConfigureWolfPackCompatibility == null ||
                !EpicLootRaritySetsPlugin.ConfigureWolfPackCompatibility.Value)
            {
                return;
            }

            string configPath = Path.Combine(Paths.ConfigPath, WolfPackConfigFileName);
            if (!File.Exists(configPath) && !IsWolfPackInstalled())
            {
                return;
            }

            try
            {
                ConfigFile config = new ConfigFile(configPath, true);
                bool changed = false;

                ConfigEntry<string> trainableCreatures = config.Bind(
                    "General",
                    "TrainableCreatures",
                    "wolf",
                    "List of creatures that will be called to Follow or Stay.");
                changed |= SetValue(trainableCreatures, MergePatterns(trainableCreatures.Value, EpicLootRaritySetsPlugin.WolfPackTrainableCreatures.Value));

                ConfigEntry<int> detectionRange = config.Bind(
                    "General",
                    "DetectionRange",
                    50,
                    new ConfigDescription("Tame creatures beyond this distance will not be affected.", new AcceptableValueRange<int>(1, 150)));
                changed |= SetValue(detectionRange, Mathf.Clamp(Mathf.Max(detectionRange.Value, EpicLootRaritySetsPlugin.WolfPackDetectionRange.Value), 1, 150));

                ConfigEntry<int> attackRange = config.Bind(
                    "General",
                    "TamesAttackRange",
                    100,
                    new ConfigDescription("The maximum distance at which enemies can be targeted by hovering and tames will attack.", new AcceptableValueRange<int>(0, 150)));
                changed |= SetValue(attackRange, Mathf.Clamp(Mathf.Max(attackRange.Value, EpicLootRaritySetsPlugin.WolfPackAttackRange.Value), 0, 150));

                ConfigEntry<int> maxTames = config.Bind(
                    "General",
                    "MaxTames",
                    10,
                    new ConfigDescription("Maximum number of tame creatures that can be summoned at one time.", new AcceptableValueRange<int>(1, 50)));
                changed |= SetValue(maxTames, Mathf.Clamp(Mathf.Max(maxTames.Value, EpicLootRaritySetsPlugin.WolfPackMaxTames.Value), 1, 50));

                if (changed)
                {
                    config.Save();
                    EpicLootRaritySetsPlugin.Log.LogInfo("WolfPack compatibility config synchronized for Hraesvelgr summons.");
                }
            }
            catch (Exception ex)
            {
                EpicLootRaritySetsPlugin.Log.LogWarning("Could not synchronize WolfPack compatibility config: " + ex.GetBaseException().Message);
            }
        }

        private static string MergePatterns(string current, string desired)
        {
            List<string> values = new List<string>();
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            AddPatterns(values, seen, current);
            AddPatterns(values, seen, desired);
            return values.Count > 0 ? string.Join(",", values.ToArray()) : desired;
        }

        private static void AddPatterns(List<string> values, HashSet<string> seen, string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return;
            }

            string[] parts = raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string value = part.Trim();
                if (value.Length == 0 || !seen.Add(value))
                {
                    continue;
                }

                values.Add(value);
            }
        }

        private static bool SetValue<T>(ConfigEntry<T> entry, T value)
        {
            if (entry == null || EqualityComparer<T>.Default.Equals(entry.Value, value))
            {
                return false;
            }

            entry.Value = value;
            return true;
        }

        private static bool IsWolfPackInstalled()
        {
            try
            {
                return !string.IsNullOrEmpty(Paths.PluginPath) &&
                       Directory.Exists(Paths.PluginPath) &&
                       Directory.GetFiles(Paths.PluginPath, "WolfPack.dll", SearchOption.AllDirectories).Length > 0;
            }
            catch
            {
                return false;
            }
        }
    }

    internal static class RaritySetRegistry
    {
        private static readonly ItemRarity[] SupportedRarities =
        {
            ItemRarity.Magic,
            ItemRarity.Rare,
            ItemRarity.Epic,
            ItemRarity.Legendary,
            ItemRarity.Mythic,
            ItemRarity.Ancient
        };

        private static readonly Dictionary<ItemRarity, List<LegendaryInfo>> ItemsByRarity = new Dictionary<ItemRarity, List<LegendaryInfo>>();
        private static readonly Dictionary<string, LegendaryInfo> ItemById = new Dictionary<string, LegendaryInfo>();
        private static readonly Dictionary<string, ItemRarity> ItemRarityById = new Dictionary<string, ItemRarity>();
        private static readonly Dictionary<string, LegendarySetInfo> SetById = new Dictionary<string, LegendarySetInfo>();
        private static readonly Dictionary<string, ItemRarity> SetRarityById = new Dictionary<string, ItemRarity>();
        private static readonly Dictionary<string, string> SetIdByItemId = new Dictionary<string, string>();
        private static readonly Dictionary<string, string[]> AdventureBaseItemsByCustomId = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "EpicSolomonKaneWitchfinderArbalest", new[] { "CrossbowArbalest" } },
            { "EpicSolomonKaneWidebrim", new[] { "HelmetCarapace", "HelmetPadded", "HelmetDrake" } },
            { "EpicSolomonKaneLongcoat", new[] { "ArmorCarapaceChest", "ArmorPaddedCuirass", "ArmorIronChest" } },
            { "EpicSolomonKaneBoots", new[] { "ArmorCarapaceLegs", "ArmorPaddedGreaves", "ArmorIronLegs" } },

            { "EpicThorReturningAxe", new[] { "AxeJotunBane", "AxeBlackMetal", "AxeIron" } },
            { "EpicThorStormHelm", new[] { "HelmetCarapace", "HelmetPadded", "HelmetDrake" } },
            { "EpicThorThunderHarness", new[] { "ArmorCarapaceChest", "ArmorPaddedCuirass", "ArmorIronChest" } },
            { "EpicThorStormstride", new[] { "ArmorCarapaceLegs", "ArmorPaddedGreaves", "ArmorIronLegs" } },

            { "EpicFlokiShipwrightHammer", new[] { "Hammer" } },
            { "EpicFlokiTarredHarness", new[] { "ArmorPaddedCuirass", "ArmorCarapaceChest", "ArmorIronChest" } },
            { "EpicFlokiDockstride", new[] { "ArmorPaddedGreaves", "ArmorCarapaceLegs", "ArmorIronLegs" } },
            { "EpicFlokiSailclothMantle", new[] { "CapeLox", "CapeFeather", "CapeWolf" } },

            { "EpicHraesvelgrLeatherQuiver", new[] { "LeatherQuiver" } },
            { "EpicMoonveinLeatherQuiver", new[] { "LeatherQuiver" } },
            { "HraesvelgrLeatherQuiver", new[] { "LeatherQuiver" } },
            { "MoonveinLeatherQuiver", new[] { "LeatherQuiver" } },
            { "MythicHraesvelgrLeatherQuiver", new[] { "LeatherQuiver" } },
            { "MythicMoonveinLeatherQuiver", new[] { "LeatherQuiver" } },
            { "AncientHraesvelgrLeatherQuiver", new[] { "LeatherQuiver" } },
            { "AncientMoonveinLeatherQuiver", new[] { "LeatherQuiver" } }
        };

        internal static void Load()
        {
            ItemsByRarity.Clear();
            ItemById.Clear();
            ItemRarityById.Clear();
            SetById.Clear();
            SetRarityById.Clear();
            SetIdByItemId.Clear();

            foreach (ItemRarity rarity in SupportedRarities)
            {
                ItemsByRarity[rarity] = new List<LegendaryInfo>();
            }

            if (EpicLootRaritySetsPlugin.ReadLegendariesJson == null || EpicLootRaritySetsPlugin.ReadLegendariesJson.Value)
            {
                LoadFile(Path.Combine(Paths.ConfigPath, "EpicLoot", "baseconfig", "legendaries.json"));
            }

            if (EpicLootRaritySetsPlugin.ReadRaritySetsJson == null || EpicLootRaritySetsPlugin.ReadRaritySetsJson.Value)
            {
                LoadFile(Path.Combine(Paths.ConfigPath, "EpicLoot", "raritysets.json"));
            }

            EpicLootRaritySetsPlugin.Log.LogInfo(string.Format(
                "Loaded rarity sets: Magic={0}, Rare={1}, Epic={2}, Legendary={3}, Mythic={4}, Ancient={5}",
                ItemsByRarity[ItemRarity.Magic].Count,
                ItemsByRarity[ItemRarity.Rare].Count,
                ItemsByRarity[ItemRarity.Epic].Count,
                ItemsByRarity[ItemRarity.Legendary].Count,
                ItemsByRarity[ItemRarity.Mythic].Count,
                ItemsByRarity[ItemRarity.Ancient].Count));
        }

        internal static bool TryGetInfo(string itemId, out LegendaryInfo info)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                info = null;
                return false;
            }

            return ItemById.TryGetValue(itemId, out info);
        }

        internal static bool TryGetSetInfo(string setId, out LegendarySetInfo setInfo, out ItemRarity rarity)
        {
            if (string.IsNullOrEmpty(setId))
            {
                setInfo = null;
                rarity = ItemRarity.Magic;
                return false;
            }

            if (SetById.TryGetValue(setId, out setInfo))
            {
                rarity = SetRarityById[setId];
                return true;
            }

            rarity = ItemRarity.Magic;
            return false;
        }

        internal static string GetSetForItem(LegendaryInfo info)
        {
            if (info == null || string.IsNullOrEmpty(info.ID))
            {
                return null;
            }

            string setId;
            return SetIdByItemId.TryGetValue(info.ID, out setId) ? setId : null;
        }

        internal static bool TryGetEffectValues(string itemId, string effectType, out MagicItemEffectDefinition.ValueDef values)
        {
            values = null;

            LegendaryInfo info;
            if (!TryGetInfo(itemId, out info) || info.GuaranteedMagicEffects == null)
            {
                return false;
            }

            foreach (GuaranteedMagicEffect effect in info.GuaranteedMagicEffects)
            {
                if (effect != null && effect.Type == effectType && effect.Values != null)
                {
                    values = effect.Values;
                    return true;
                }
            }

            return false;
        }

        internal static bool TryRollSetItem(ItemRarity rarity, ItemDrop.ItemData baseItem, MagicItem originalMagicItem, float powerLevelMod, out MagicItem rolledMagicItem)
        {
            rolledMagicItem = null;

            if (!ItemsByRarity.ContainsKey(rarity) || ItemsByRarity[rarity].Count == 0)
            {
                return false;
            }

            List<LegendaryInfo> available = GetAvailableItems(rarity, baseItem, originalMagicItem, true);
            if (available.Count == 0)
            {
                return false;
            }

            LegendaryInfo selected = RollWeighted(available);
            if (selected == null)
            {
                return false;
            }

            rolledMagicItem = BuildMagicItem(rarity, baseItem, originalMagicItem, selected, powerLevelMod);
            return rolledMagicItem != null;
        }

        internal static bool TryBuildForcedMagicItem(string itemId, ItemDrop.ItemData baseItem, float powerLevelMod, out MagicItem magicItem)
        {
            magicItem = null;

            LegendaryInfo info;
            ItemRarity rarity;
            if (!TryGetInfo(itemId, out info) || !ItemRarityById.TryGetValue(itemId, out rarity))
            {
                return false;
            }

            MagicItem probeMagicItem = new MagicItem();
            probeMagicItem.Rarity = rarity;

            if (info.Requirements != null &&
                !info.Requirements.CheckRequirements(baseItem, probeMagicItem, null, true, false, false, true) &&
                !IsLeatherQuiverCustomItem(itemId))
            {
                return false;
            }

            magicItem = BuildMagicItem(rarity, baseItem, null, info, powerLevelMod);
            return magicItem != null;
        }

        internal static bool TryCreateAdventureItemDrop(string itemId, out ItemDrop itemDrop)
        {
            itemDrop = null;

            LegendaryInfo info;
            ItemRarity rarity;
            if (!TryGetInfo(itemId, out info) || !ItemRarityById.TryGetValue(itemId, out rarity))
            {
                return false;
            }

            ItemDrop template = FindAdventureBaseItem(itemId, info, rarity);
            if (template == null)
            {
                EpicLootRaritySetsPlugin.Log.LogWarning("Could not find a base item for adventure set item " + itemId);
                return false;
            }

            bool oldForceDisableInit = ZNetView.m_forceDisableInit;
            try
            {
                ZNetView.m_forceDisableInit = true;
                itemDrop = UnityEngine.Object.Instantiate(template);
            }
            finally
            {
                ZNetView.m_forceDisableInit = oldForceDisableInit;
            }

            if (itemDrop == null || itemDrop.m_itemData == null)
            {
                itemDrop = null;
                return false;
            }

            itemDrop.gameObject.name = itemId;
            itemDrop.m_itemData.m_dropPrefab = template.gameObject;

            MagicItem magicItem = BuildMagicItem(rarity, itemDrop.m_itemData, null, info, 1f);
            if (magicItem == null)
            {
                if (ZNetScene.instance != null)
                {
                    ZNetScene.instance.Destroy(itemDrop.gameObject);
                }
                else
                {
                    UnityEngine.Object.Destroy(itemDrop.gameObject);
                }

                itemDrop = null;
                return false;
            }

            ItemDataExtensions.SaveMagicItem(itemDrop.m_itemData, magicItem);
            itemDrop.gameObject.SetActive(false);
            return true;
        }

        internal static float GetDropChance(ItemRarity rarity)
        {
            if (rarity == ItemRarity.Magic)
            {
                return EpicLootRaritySetsPlugin.MagicSetDropChance.Value;
            }

            if (rarity == ItemRarity.Rare)
            {
                return EpicLootRaritySetsPlugin.RareSetDropChance.Value;
            }

            if (rarity == ItemRarity.Epic)
            {
                return EpicLootRaritySetsPlugin.EpicSetDropChance.Value;
            }

            if (rarity == ItemRarity.Ancient)
            {
                return EpicLootRaritySetsPlugin.AncientSetDropChance.Value;
            }

            return 0f;
        }

        internal static string GetBaseSetName(string setId)
        {
            if (string.IsNullOrEmpty(setId))
            {
                return null;
            }

            foreach (string rarityName in new[] { "Legendary", "Ancient", "Mythic", "Magic", "Rare", "Epic" })
            {
                if (setId.StartsWith(rarityName, StringComparison.OrdinalIgnoreCase) && setId.Length > rarityName.Length)
                {
                    return setId.Substring(rarityName.Length);
                }
            }

            return setId;
        }

        internal static int GetActivationPieceCount(string setId)
        {
            LegendarySetInfo setInfo;
            ItemRarity rarity;
            if (!TryGetSetInfo(setId, out setInfo, out rarity) || setInfo == null)
            {
                return int.MaxValue;
            }

            int activationCount = 0;
            if (setInfo.SetBonuses != null)
            {
                foreach (SetBonusInfo bonus in setInfo.SetBonuses)
                {
                    if (bonus != null && bonus.Count > activationCount)
                    {
                        activationCount = bonus.Count;
                    }
                }
            }

            if (activationCount > 0)
            {
                return activationCount;
            }

            return setInfo.LegendaryIDs != null && setInfo.LegendaryIDs.Count > 0 ? setInfo.LegendaryIDs.Count : int.MaxValue;
        }

        private static void LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                RaritySetFile config = JsonConvert.DeserializeObject<RaritySetFile>(json);
                if (config == null)
                {
                    return;
                }

                AddRarity(ItemRarity.Magic, config.MagicItems, config.MagicSets, path);
                AddRarity(ItemRarity.Rare, config.RareItems, config.RareSets, path);
                AddRarity(ItemRarity.Epic, config.EpicItems, config.EpicSets, path);
                AddRarity(ItemRarity.Legendary, config.LegendaryItems, config.LegendarySets, path);
                AddRarity(ItemRarity.Mythic, config.MythicItems, config.MythicSets, path);
                AddRarity(ItemRarity.Ancient, config.AncientItems, config.AncientSets, path);
            }
            catch (Exception ex)
            {
                EpicLootRaritySetsPlugin.Log.LogError("Failed to load rarity set file " + path + ": " + ex);
            }
        }

        private static void AddRarity(ItemRarity rarity, List<LegendaryInfo> items, List<LegendarySetInfo> sets, string source)
        {
            if (items != null)
            {
                foreach (LegendaryInfo item in items)
                {
                    if (item == null || string.IsNullOrEmpty(item.ID))
                    {
                        continue;
                    }

                    if (ItemById.ContainsKey(item.ID))
                    {
                        EpicLootRaritySetsPlugin.Log.LogWarning(string.Format("{0}: duplicate rarity item ID {1}; keeping the first definition.", Path.GetFileName(source), item.ID));
                        continue;
                    }

                    ItemsByRarity[rarity].Add(item);
                    ItemById[item.ID] = item;
                    ItemRarityById[item.ID] = rarity;
                }
            }

            if (sets != null)
            {
                foreach (LegendarySetInfo set in sets)
                {
                    if (set == null || string.IsNullOrEmpty(set.ID))
                    {
                        continue;
                    }

                    if (SetById.ContainsKey(set.ID))
                    {
                        EpicLootRaritySetsPlugin.Log.LogWarning(string.Format("{0}: duplicate rarity set ID {1}; keeping the first definition.", Path.GetFileName(source), set.ID));
                        continue;
                    }

                    SetById[set.ID] = set;
                    SetRarityById[set.ID] = rarity;

                    if (set.LegendaryIDs == null)
                    {
                        continue;
                    }

                    foreach (string itemId in set.LegendaryIDs)
                    {
                        if (string.IsNullOrEmpty(itemId))
                        {
                            continue;
                        }

                        if (!ItemById.ContainsKey(itemId))
                        {
                            EpicLootRaritySetsPlugin.Log.LogWarning(string.Format("{0}: set {1} references unknown item {2}", Path.GetFileName(source), set.ID, itemId));
                        }

                        SetIdByItemId[itemId] = set.ID;
                    }
                }
            }
        }

        private static List<LegendaryInfo> GetAvailableItems(ItemRarity rarity, ItemDrop.ItemData baseItem, MagicItem magicItem, bool setItemsOnly)
        {
            List<LegendaryInfo> result = new List<LegendaryInfo>();
            List<LegendaryInfo> items;
            if (!ItemsByRarity.TryGetValue(rarity, out items))
            {
                return result;
            }

            foreach (LegendaryInfo info in items)
            {
                if (info == null)
                {
                    continue;
                }

                if (setItemsOnly && !info.IsSetItem)
                {
                    continue;
                }

                if (info.Requirements != null && !info.Requirements.CheckRequirements(baseItem, magicItem, null, true, false, false, true))
                {
                    continue;
                }

                result.Add(info);
            }

            return result;
        }

        private static ItemDrop FindAdventureBaseItem(string itemId, LegendaryInfo info, ItemRarity rarity)
        {
            string[] preferredItems;
            if (AdventureBaseItemsByCustomId.TryGetValue(itemId, out preferredItems))
            {
                foreach (string preferredItem in preferredItems)
                {
                    ItemDrop preferredDrop = GetItemDropFromObjectDB(preferredItem);
                    if (MatchesRequirements(preferredDrop, info, rarity) ||
                        (IsLeatherQuiverCustomItem(itemId) && preferredDrop != null))
                    {
                        return preferredDrop;
                    }
                }
            }

            if (ObjectDB.instance != null && ObjectDB.instance.m_items != null)
            {
                foreach (GameObject itemPrefab in ObjectDB.instance.m_items.OrderBy(x => x != null ? x.name : string.Empty))
                {
                    if (itemPrefab == null)
                    {
                        continue;
                    }

                    ItemDrop candidate = itemPrefab.GetComponent<ItemDrop>();
                    if (MatchesRequirements(candidate, info, rarity))
                    {
                        return candidate;
                    }
                }
            }

            if (ZNetScene.instance != null && ZNetScene.instance.m_prefabs != null)
            {
                foreach (GameObject prefab in ZNetScene.instance.m_prefabs.OrderBy(x => x != null ? x.name : string.Empty))
                {
                    if (prefab == null)
                    {
                        continue;
                    }

                    ItemDrop candidate = prefab.GetComponent<ItemDrop>();
                    if (MatchesRequirements(candidate, info, rarity))
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        private static ItemDrop GetItemDropFromObjectDB(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                return null;
            }

            GameObject itemPrefab = ObjectDB.instance != null ? ObjectDB.instance.GetItemPrefab(itemName) : null;
            if (itemPrefab == null && ZNetScene.instance != null)
            {
                itemPrefab = ZNetScene.instance.GetPrefab(itemName);
            }

            if (itemPrefab == null && ZNetScene.instance != null && ZNetScene.instance.m_prefabs != null)
            {
                itemPrefab = ZNetScene.instance.m_prefabs.FirstOrDefault(prefab =>
                    prefab != null && string.Equals(prefab.name, itemName, StringComparison.OrdinalIgnoreCase));
            }

            if (itemPrefab == null)
            {
                return null;
            }

            return itemPrefab.GetComponent<ItemDrop>();
        }

        private static bool IsLeatherQuiverCustomItem(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) &&
                   itemId.IndexOf("LeatherQuiver", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   AdventureBaseItemsByCustomId.ContainsKey(itemId);
        }

        private static bool MatchesRequirements(ItemDrop candidate, LegendaryInfo info, ItemRarity rarity)
        {
            if (candidate == null || candidate.m_itemData == null || candidate.m_itemData.m_shared == null)
            {
                return false;
            }

            candidate.m_itemData.m_dropPrefab = candidate.gameObject;

            if (info.Requirements == null)
            {
                return true;
            }

            MagicItem probeMagicItem = new MagicItem();
            probeMagicItem.Rarity = rarity;
            return info.Requirements.CheckRequirements(candidate.m_itemData, probeMagicItem, null, true, false, false, true);
        }

        private static LegendaryInfo RollWeighted(List<LegendaryInfo> available)
        {
            float total = 0f;
            foreach (LegendaryInfo info in available)
            {
                total += GetWeight(info);
            }

            if (total <= 0f)
            {
                return available[UnityEngine.Random.Range(0, available.Count)];
            }

            float roll = UnityEngine.Random.Range(0f, total);
            foreach (LegendaryInfo info in available)
            {
                roll -= GetWeight(info);
                if (roll <= 0f)
                {
                    return info;
                }
            }

            return available[available.Count - 1];
        }

        private static float GetWeight(LegendaryInfo info)
        {
            return info.SelectionWeight > 0f ? info.SelectionWeight : 1f;
        }

        private static MagicItem BuildMagicItem(ItemRarity rarity, ItemDrop.ItemData baseItem, MagicItem originalMagicItem, LegendaryInfo selected, float powerLevelMod)
        {
            MagicItem magicItem = new MagicItem();
            magicItem.Rarity = rarity;
            magicItem.SocketCount = originalMagicItem != null ? originalMagicItem.SocketCount : LootRoller.RollSocketCountPerRarity(rarity);
            magicItem.IsUnidentified = originalMagicItem != null && originalMagicItem.IsUnidentified;
            magicItem.TypeNameOverride = originalMagicItem != null ? originalMagicItem.TypeNameOverride : null;
            magicItem.LegendaryID = selected.ID;
            magicItem.DisplayName = selected.Name;
            magicItem.SetID = selected.IsSetItem ? GetSetForItem(selected) : null;

            int targetEffectCount = selected.GuaranteedEffectCount > 0 ? selected.GuaranteedEffectCount : (originalMagicItem != null ? originalMagicItem.Effects.Count : LootRoller.RollEffectCountPerRarity(rarity));
            AddGuaranteedEffects(magicItem, rarity, baseItem, selected, powerLevelMod);
            FillRandomEffects(magicItem, rarity, baseItem, targetEffectCount, powerLevelMod);

            return magicItem;
        }

        private static void AddGuaranteedEffects(MagicItem magicItem, ItemRarity rarity, ItemDrop.ItemData baseItem, LegendaryInfo selected, float powerLevelMod)
        {
            if (selected.GuaranteedMagicEffects == null)
            {
                return;
            }

            foreach (GuaranteedMagicEffect guaranteedEffect in selected.GuaranteedMagicEffects)
            {
                if (guaranteedEffect == null || string.IsNullOrEmpty(guaranteedEffect.Type))
                {
                    continue;
                }

                MagicItemEffectDefinition effectDef = MagicItemEffectDefinitions.Get(guaranteedEffect.Type);
                if (effectDef == null)
                {
                    EpicLootRaritySetsPlugin.Log.LogWarning(string.Format("Unknown guaranteed effect {0} on {1}", guaranteedEffect.Type, selected.ID));
                    continue;
                }

                if (!effectDef.CheckRequirements(baseItem, magicItem, true, false, false))
                {
                    EpicLootRaritySetsPlugin.Log.LogWarning(string.Format("Guaranteed effect {0} does not fit item {1}", guaranteedEffect.Type, selected.ID));
                    continue;
                }

                MagicItemEffect rolledEffect = LootRoller.RollEffect(effectDef, rarity, guaranteedEffect.Values, powerLevelMod);
                if (rolledEffect != null)
                {
                    magicItem.Effects.Add(rolledEffect);
                }
            }
        }

        private static void FillRandomEffects(MagicItem magicItem, ItemRarity rarity, ItemDrop.ItemData baseItem, int targetEffectCount, float powerLevelMod)
        {
            int remaining = targetEffectCount - magicItem.Effects.Count;
            if (remaining <= 0)
            {
                return;
            }

            List<MagicItemEffectDefinition> availableEffects = MagicItemEffectDefinitions.GetAvailableEffects(baseItem, magicItem, -1, true, false, false);
            if (availableEffects == null || availableEffects.Count == 0)
            {
                return;
            }

            List<MagicItemEffect> randomEffects = LootRoller.RollEffects(availableEffects, rarity, remaining, true);
            if (randomEffects != null)
            {
                magicItem.Effects.AddRange(randomEffects);
            }
        }

        #pragma warning disable 0649
        private class RaritySetFile
        {
            public List<LegendaryInfo> MagicItems;
            public List<LegendarySetInfo> MagicSets;
            public List<LegendaryInfo> RareItems;
            public List<LegendarySetInfo> RareSets;
            public List<LegendaryInfo> EpicItems;
            public List<LegendarySetInfo> EpicSets;
            public List<LegendaryInfo> LegendaryItems;
            public List<LegendarySetInfo> LegendarySets;
            public List<LegendaryInfo> MythicItems;
            public List<LegendarySetInfo> MythicSets;
            public List<LegendaryInfo> AncientItems;
            public List<LegendarySetInfo> AncientSets;
        }
        #pragma warning restore 0649
    }

    internal static class SetActivationBuffController
    {
        private const float UpdateInterval = 0.5f;
        private const float BuffTtl = 1.5f;
        private const string BuffNamePrefix = "FranSetActive_";
        private const string BuffCategory = "FranSetActivation";

        private static readonly Dictionary<string, StatusEffect> BuffsByBaseSet = new Dictionary<string, StatusEffect>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> ActiveBaseSets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static float _updateTimer;

        internal static bool HasActiveSet(string baseSetName)
        {
            return !string.IsNullOrEmpty(baseSetName) && ActiveBaseSets.Contains(baseSetName);
        }

        internal static bool HasActiveSet(Player player, string baseSetName)
        {
            if (player == null || string.IsNullOrEmpty(baseSetName))
            {
                return false;
            }

            if (player == Player.m_localPlayer)
            {
                return HasActiveSet(baseSetName);
            }

            StatusEffect template = GetOrCreateBuff(baseSetName, null);
            return template != null && player.GetSEMan().HaveStatusEffect(template.NameHash());
        }

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            _updateTimer -= dt;
            if (_updateTimer > 0f)
            {
                return;
            }

            _updateTimer = UpdateInterval;

            if (EpicLootRaritySetsPlugin.EnableSetActivationBuffs != null && !EpicLootRaritySetsPlugin.EnableSetActivationBuffs.Value)
            {
                Clear(player);
                return;
            }

            HashSet<string> activeNow = GetActiveBaseSets(player);
            SEMan seMan = player.GetSEMan();

            foreach (string oldBaseSet in ActiveBaseSets.ToArray())
            {
                if (activeNow.Contains(oldBaseSet))
                {
                    continue;
                }

                StatusEffect oldBuff = GetOrCreateBuff(oldBaseSet, null);
                if (oldBuff != null)
                {
                    seMan.RemoveStatusEffect(oldBuff.NameHash(), true);
                }
            }

            ActiveBaseSets.Clear();
            foreach (string baseSetName in activeNow)
            {
                ActiveBaseSets.Add(baseSetName);
                StatusEffect buff = GetOrCreateBuff(baseSetName, FindActiveSetIcon(player, baseSetName));
                if (buff != null)
                {
                    seMan.AddStatusEffect(buff, true, 0, 0f, 0);
                }
            }
        }

        internal static void Clear(Player player)
        {
            if (player != null)
            {
                SEMan seMan = player.GetSEMan();
                foreach (string baseSetName in ActiveBaseSets.ToArray())
                {
                    StatusEffect buff = GetOrCreateBuff(baseSetName, null);
                    if (buff != null)
                    {
                        seMan.RemoveStatusEffect(buff.NameHash(), true);
                    }
                }
            }

            ActiveBaseSets.Clear();
            _updateTimer = 0f;
        }

        private static HashSet<string> GetActiveBaseSets(Player player)
        {
            Dictionary<string, int> piecesBySetId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Inventory inventory = player.GetInventory();
            if (inventory == null)
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            foreach (ItemDrop.ItemData item in inventory.GetEquippedItems())
            {
                if (item == null)
                {
                    continue;
                }

                MagicItem magicItem = ItemDataExtensions.GetMagicItem(item);
                if (magicItem == null || string.IsNullOrEmpty(magicItem.SetID))
                {
                    continue;
                }

                if (!piecesBySetId.ContainsKey(magicItem.SetID))
                {
                    piecesBySetId[magicItem.SetID] = 0;
                }

                piecesBySetId[magicItem.SetID]++;
            }

            HashSet<string> activeBaseSets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, int> entry in piecesBySetId)
            {
                int activationPieceCount = RaritySetRegistry.GetActivationPieceCount(entry.Key);
                if (entry.Value < activationPieceCount)
                {
                    continue;
                }

                string baseSetName = RaritySetRegistry.GetBaseSetName(entry.Key);
                if (!string.IsNullOrEmpty(baseSetName))
                {
                    activeBaseSets.Add(baseSetName);
                }
            }

            return activeBaseSets;
        }

        private static StatusEffect GetOrCreateBuff(string baseSetName, Sprite icon)
        {
            if (string.IsNullOrEmpty(baseSetName))
            {
                return null;
            }

            StatusEffect buff;
            if (!BuffsByBaseSet.TryGetValue(baseSetName, out buff) || buff == null)
            {
                buff = ScriptableObject.CreateInstance<SE_Stats>();
                buff.name = BuffNamePrefix + baseSetName;
                buff.m_name = baseSetName;
                buff.m_category = BuffCategory;
                buff.m_ttl = 0f;
                buff.m_flashIcon = false;
                buff.m_cooldownIcon = false;
                buff.m_hidden = false;
                BuffsByBaseSet[baseSetName] = buff;
            }

            buff.m_tooltip = GetBuffTooltip(baseSetName);
            if (icon != null)
            {
                buff.m_icon = icon;
            }

            return buff;
        }

        internal static string GetBuffTooltip(string baseSetName)
        {
            if (string.Equals(baseSetName, "Frostbrand", StringComparison.OrdinalIgnoreCase))
            {
                float elemental = GetLocalSkillLevel(Skills.SkillType.ElementalMagic);
                float fireball = ScaleSkillValue(EpicLootRaritySetsPlugin.FrostbrandLightningStrikeBaseDamage.Value, EpicLootRaritySetsPlugin.FrostbrandLightningStrikeDamagePerElementalMagicLevel.Value, elemental);
                float slash = ScaleSkillValue(EpicLootRaritySetsPlugin.FrostbrandSlashBaseDamage.Value, EpicLootRaritySetsPlugin.FrostbrandSlashDamagePerElementalMagicLevel.Value, elemental);
                float crush = ScaleSkillValue(EpicLootRaritySetsPlugin.FrostbrandCrushBaseDamage.Value, EpicLootRaritySetsPlugin.FrostbrandCrushDamagePerElementalMagicLevel.Value, elemental);
                float burningGround = ScaleSkillValue(EpicLootRaritySetsPlugin.FrostbrandBurningGroundBaseDamage.Value, EpicLootRaritySetsPlugin.FrostbrandBurningGroundDamagePerElementalMagicLevel.Value, elemental);
                float shieldMitigation = ClampScaledFraction(EpicLootRaritySetsPlugin.FrostbrandElementalShieldBaseMitigation.Value, EpicLootRaritySetsPlugin.FrostbrandElementalShieldMitigationPerElementalMagicLevel.Value, elemental, EpicLootRaritySetsPlugin.FrostbrandElementalShieldMaxMitigation.Value) * 100f;
                return string.Format(
                    "Frostbrand set completo activo.\n\nEscalado actual: Magia elemental {23:0.#}.\n\nHabilidades:\n{0}: Water Sphere de Njord. Coste {1:0} eitr. CD {2:0}s. Dura {5:0.#}s. Atrae enemigos en {3:0.#}m. No anade dano propio; usa el efecto de Njord.\nAtaque secundario: Slash de Surt. Coste {10:0} eitr. CD {11:0}s. Escala con Magia elemental: {12:0.#}+{13:0.##}/nivel. Resultado actual: {24:0.#} fuego + {24:0.#} slash.\n{14}: Crush de Surt. Coste {15:0} eitr. CD {16:0}s. Escala con Magia elemental: {25:0.#} fuego + {25:0.#} contundente al caer. Burning Ground {17:0.#}s, tick fuego {26:0.#} cada {27:0.#}s.\n{18} + bloquear: Elemental Shield de Surt. Coste {19:0} eitr. CD {20:0}s. Dura {21:0.#}s, anula fuego y mitiga dano actual {22:0.#}%.\n\nPasiva:\nCada {6} ataques con arma Frostbrand muestra stacks y lanza Fire Ball a donde apuntas. Escala con Magia elemental: {8:0.#}+{9:0.##}/nivel. Resultado actual: {28:0.#} fuego antes de modificadores EpicLoot.",
                    FormatShortcut(EpicLootRaritySetsPlugin.FrostbrandWaterSphereHotkey),
                    EpicLootRaritySetsPlugin.FrostbrandWaterSphereEitrUse.Value,
                    EpicLootRaritySetsPlugin.FrostbrandWaterSphereCooldown.Value,
                    EpicLootRaritySetsPlugin.FrostbrandWaterSpherePullRadius.Value,
                    EpicLootRaritySetsPlugin.FrostbrandWaterSpherePullForce.Value,
                    EpicLootRaritySetsPlugin.FrostbrandWaterSphereMaxDuration.Value,
                    EpicLootRaritySetsPlugin.FrostbrandLightningStrikeAttackCount.Value,
                    EpicLootRaritySetsPlugin.FrostbrandLightningStrikeRadius.Value,
                    EpicLootRaritySetsPlugin.FrostbrandLightningStrikeBaseDamage.Value,
                    EpicLootRaritySetsPlugin.FrostbrandLightningStrikeDamagePerElementalMagicLevel.Value,
                    EpicLootRaritySetsPlugin.FrostbrandSlashEitrUse.Value,
                    EpicLootRaritySetsPlugin.FrostbrandSlashCooldown.Value,
                    EpicLootRaritySetsPlugin.FrostbrandSlashBaseDamage.Value,
                    EpicLootRaritySetsPlugin.FrostbrandSlashDamagePerElementalMagicLevel.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.FrostbrandCrushHotkey),
                    EpicLootRaritySetsPlugin.FrostbrandCrushEitrUse.Value,
                    EpicLootRaritySetsPlugin.FrostbrandCrushCooldown.Value,
                    EpicLootRaritySetsPlugin.FrostbrandBurningGroundDuration.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.FrostbrandElementalShieldHotkey),
                    EpicLootRaritySetsPlugin.FrostbrandElementalShieldEitrUse.Value,
                    EpicLootRaritySetsPlugin.FrostbrandElementalShieldCooldown.Value,
                    EpicLootRaritySetsPlugin.FrostbrandElementalShieldDuration.Value,
                    shieldMitigation,
                    elemental,
                    slash,
                    crush,
                    burningGround,
                    EpicLootRaritySetsPlugin.FrostbrandBurningGroundTickInterval.Value,
                    fireball);
            }

            if (string.Equals(baseSetName, "Moonvein", StringComparison.OrdinalIgnoreCase))
            {
                float elemental = GetLocalSkillLevel(Skills.SkillType.ElementalMagic);
                float acid = ScaleSkillValue(EpicLootRaritySetsPlugin.MoonveinAcidBoltBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinAcidBoltDamagePerElementalMagicLevel.Value, elemental);
                float lightning = ScaleSkillValue(EpicLootRaritySetsPlugin.MoonveinLightningBoltBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinLightningBoltDamagePerElementalMagicLevel.Value, elemental);
                float fireball = ScaleSkillValue(EpicLootRaritySetsPlugin.MoonveinFireballBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinFireballDamagePerElementalMagicLevel.Value, elemental);
                float meteor = ScaleSkillValue(EpicLootRaritySetsPlugin.MoonveinMeteorBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinMeteorDamagePerElementalMagicLevel.Value, elemental);
                float tornado = ScaleSkillValue(EpicLootRaritySetsPlugin.MoonveinTornadoBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinTornadoDamagePerElementalMagicLevel.Value, elemental);
                return string.Format(
                    "Moonvein set completo activo.\n\nEscalado actual: Magia elemental {21:0.#}.\n\nDisparos cargados:\nCada tercer disparo consecutivo lanza un hechizo aleatorio desde el arco y reinicia el contador.\nProbabilidades: Acid Bolt 40%, Lightning Bolt 40%, Fireball 20%.\nAcid Bolt: {6:0.#}+{7:0.##}/nivel = {22:0.#} veneno.\nLightning Bolt: {8:0.#}+{9:0.##}/nivel = {23:0.#} rayo.\nFireball: {10:0.#}+{11:0.##}/nivel = {24:0.#} fuego.\n\nHabilidades:\n{0}: Meteor. Coste: {1:0} eitr. CD: {2:0}s. Escala con Magia elemental: {12:0.#}+{13:0.##}/nivel = {25:0.#} fuego + {25:0.#} contundente.\n{3}: Tornado Shot. Coste: {4:0} eitr. CD: {5:0}s. Arma la siguiente flecha; al impactar invoca el tornado de Njord donde caiga durante {17:0.#}s. Slow {19:0.#}% durante {20:0.#}s por golpe. Fallback: rayo {14:0.#}+{15:0.##}/nivel = {26:0.#} cada {16:0.##}s en {18:0.#}m.\nLos hechizos de dano propios pasan por modificadores de EpicLoot como ModifyDamage y ModifyElementalDamage.",
                    FormatShortcut(EpicLootRaritySetsPlugin.MoonveinMeteorHotkey),
                    EpicLootRaritySetsPlugin.MoonveinMeteorEitrUse.Value,
                    EpicLootRaritySetsPlugin.MoonveinMeteorCooldown.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.MoonveinTornadoHotkey),
                    EpicLootRaritySetsPlugin.MoonveinTornadoEitrUse.Value,
                    EpicLootRaritySetsPlugin.MoonveinTornadoCooldown.Value,
                    EpicLootRaritySetsPlugin.MoonveinAcidBoltBaseDamage.Value,
                    EpicLootRaritySetsPlugin.MoonveinAcidBoltDamagePerElementalMagicLevel.Value,
                    EpicLootRaritySetsPlugin.MoonveinLightningBoltBaseDamage.Value,
                    EpicLootRaritySetsPlugin.MoonveinLightningBoltDamagePerElementalMagicLevel.Value,
                    EpicLootRaritySetsPlugin.MoonveinFireballBaseDamage.Value,
                    EpicLootRaritySetsPlugin.MoonveinFireballDamagePerElementalMagicLevel.Value,
                    EpicLootRaritySetsPlugin.MoonveinMeteorBaseDamage.Value,
                    EpicLootRaritySetsPlugin.MoonveinMeteorDamagePerElementalMagicLevel.Value,
                    EpicLootRaritySetsPlugin.MoonveinTornadoBaseDamage.Value,
                    EpicLootRaritySetsPlugin.MoonveinTornadoDamagePerElementalMagicLevel.Value,
                    EpicLootRaritySetsPlugin.MoonveinTornadoTickInterval.Value,
                    EpicLootRaritySetsPlugin.MoonveinTornadoDuration.Value,
                    EpicLootRaritySetsPlugin.MoonveinTornadoRadius.Value,
                    EpicLootRaritySetsPlugin.MoonveinTornadoSlow.Value * 100f,
                    EpicLootRaritySetsPlugin.MoonveinTornadoSlowDuration.Value,
                    elemental,
                    acid,
                    lightning,
                    fireball,
                    meteor,
                    tornado);
            }

            if (string.Equals(baseSetName, "Hraesvelgr", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(
                    "Hraesvelgr set completo activo.\n\nEscalado actual: estas habilidades no escalan con una skill magica; usan dano del arco/flecha y multiplicadores del set.\n\nPasivas:\nSneaky se activa al agacharte/en sigilo. Invisible para monstruos, visual Sneaky, ruido x{0:0.##}, deteccion x{1:0.##}, velocidad +{2:0.#}%.\nCada {16} disparos con arco Hraesvelgr, ese disparo cuenta como headshot/punto debil y aplica al menos x{17:0.##} dano del disparo.\n\nHabilidades:\n{3}: Summon Beasts. Toggle sin CD. Coste al invocar: {4:0} vigor. Las mascotas usan sus stats de criatura y no escalan por skill.\n{18}: Trampa armada. Coste {19:0} vigor. Si atrapa un enemigo lo inmoviliza y obtienes {20:0.#}s para que tu siguiente flecha haga +{21:0.#}% dano del disparo.\n{5}: Rapid Volley canalizado {6:0.#}s. CD {7:0}s. Mientras mantienes ataque y estas quieto, dispara {11:0.#} flechas/s. Dano resultante por flecha: x{12:0.##} del disparo normal, velocidad {13:0.#}.\nAtaque secundario: Dash de Freyja. Coste {14:0} eitr. CD {15:0}s. No anade dano propio.\nQuickDraw/tasa/proyectil se muestran al 100%, coste vigor x{10:0.##}. Al terminar recuperas todo el vigor.",
                    EpicLootRaritySetsPlugin.HraesvelgrSneakyNoiseModifier.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrSneakyStealthModifier.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrSneakySpeedModifier.Value * 100f,
                    FormatShortcut(EpicLootRaritySetsPlugin.HraesvelgrSummonHotkey),
                    EpicLootRaritySetsPlugin.HraesvelgrSummonStaminaUse.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.HraesvelgrVolleyHotkey),
                    EpicLootRaritySetsPlugin.HraesvelgrVolleyDuration.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrVolleyCooldown.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrVolleyAttackSpeedMultiplier.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrVolleyProjectileSpeedMultiplier.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrVolleyStaminaUseMultiplier.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrVolleyShotsPerSecond.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrVolleyDamageMultiplier.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrVolleyProjectileVelocity.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrDashEitrUse.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrDashCooldown.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrHeadshotAttackCount.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrHeadshotDamageMultiplier.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.HraesvelgrTrapHotkey),
                    EpicLootRaritySetsPlugin.HraesvelgrTrapStaminaUse.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrTrapBuffDuration.Value,
                    EpicLootRaritySetsPlugin.HraesvelgrTrapNextAttackDamageBonus.Value * 100f);
            }

            if (string.Equals(baseSetName, "SolomonKane", StringComparison.OrdinalIgnoreCase))
            {
                float crossbows = GetLocalSkillLevel(Skills.SkillType.Crossbows);
                float infused = ScaleSkillValue(EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltBaseDamage.Value, EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltDamagePerCrossbowsLevel.Value, crossbows);
                float bomb = ScaleSkillValue(EpicLootRaritySetsPlugin.SolomonKaneBombBaseDamage.Value, EpicLootRaritySetsPlugin.SolomonKaneBombDamagePerCrossbowsLevel.Value, crossbows);
                float verdict = ScaleSkillValue(EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictBaseDamage.Value, EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictDamagePerCrossbowsLevel.Value, crossbows);
                return string.Format(
                    "Solomon Kane set completo activo.\n\nEscalado actual: Ballestas {17:0.#}.\n\nHabilidades:\n{0}: Infused Bolt. Coste {1:0} vigor. CD {2:0}s. Arma el siguiente virote durante {3:0.#}s; al impactar explota en {4:0.#}m con {18:0.#} dano total ({5:0.#}+{6:0.##}/nivel), repartido entre frost y espiritu. Slow {7:0.#}% durante {8:0.#}s.\n{9}: Blackpowder Bomb. Coste {10:0} vigor. CD {11:0}s. Rango {12:0.#}m, radio {13:0.#}m, empuje {14:0.#}. Dano actual {19:0.#}: {20:0.#} fuego + {21:0.#} contundente.\n{28}: Bat Form. Coste {29:0} vigor. CD {30:0}s. Dura {31:0.#}s. Oculta el cuerpo, usa visual de murcielago y activa vuelo temporal a velocidad {32:0.#}.\n\nPasiva:\nWitchmark marca enemigos durante {15:0.#}s con impactos directos de ballesta Solomon Kane. Matar o golpear punto debil a un marcado arma Silver Verdict: el siguiente virote suma {22:0.#} espiritu ({23:0.#}+{24:0.##}/nivel), encadena hasta {16} enemigos en {25:0.#}m, pierde {26:0.#}% por salto y devuelve {27:0} vigor.\nLos danos propios pasan por modificadores de EpicLoot.",
                    FormatShortcut(EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltHotkey),
                    EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltStaminaUse.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltCooldown.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltDuration.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltRadius.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltBaseDamage.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltDamagePerCrossbowsLevel.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltSlow.Value * 100f,
                    EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltSlowDuration.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.SolomonKaneBombHotkey),
                    EpicLootRaritySetsPlugin.SolomonKaneBombStaminaUse.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneBombCooldown.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneBombRange.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneBombRadius.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneBombImpactForce.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneWitchmarkDuration.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictTargetCount.Value,
                    crossbows,
                    infused,
                    bomb,
                    bomb * 0.40f,
                    bomb * 0.60f,
                    verdict,
                    EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictBaseDamage.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictDamagePerCrossbowsLevel.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictSeekRadius.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictDamageLossPerJump.Value * 100f,
                    EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictStaminaRefund.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.SolomonKaneBatFormHotkey),
                    EpicLootRaritySetsPlugin.SolomonKaneBatFormStaminaUse.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneBatFormCooldown.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneBatFormDuration.Value,
                    EpicLootRaritySetsPlugin.SolomonKaneBatFormFlightSpeed.Value);
            }

            if (string.Equals(baseSetName, "Nott", StringComparison.OrdinalIgnoreCase))
            {
                float knives = GetLocalSkillLevel(Skills.SkillType.Knives);
                float poison = ScaleSkillValue(EpicLootRaritySetsPlugin.NottPoisonBaseDamage.Value, EpicLootRaritySetsPlugin.NottPoisonDamagePerKnivesLevel.Value, knives);
                return string.Format(
                    "Nott set completo activo.\n\nEscalado actual: Cuchillos {13:0.#}.\n\nPasivas:\nSneaky se activa al agacharte/en sigilo. Usa el visual Sneaky de Ullr, ruido x{0:0.##}, deteccion enemiga x{1:0.##}, velocidad +{2:0.#}%.\nShadow Momentum: golpear enemigos otorga +{9:0.#}% velocidad durante {10:0.#}s. No stackea; se refresca.\nPoison Edge escala con Cuchillos: {11:0.#}+{12:0.##}/nivel = {14:0.#} veneno anadido a cada golpe.\n\nHabilidad:\n{3}: Warp detras del enemigo apuntado o del enemigo mas cercano. Coste: {4:0} vigor. CD: {5:0}s. Rango: {6:0.#}m.\nSi no hay enemigos en rango, salta hacia delante. Si muere un enemigo a {7:0.#}m mientras Warp esta en CD, el CD se reinicia y recuperas vigor.\nDespues de usar Warp, tu siguiente ataque contra enemigo pega x{8:0.##} dano. Resultado: dano del golpe x{8:0.##}. El buff no expira por tiempo.",
                    EpicLootRaritySetsPlugin.NottSneakyNoiseModifier.Value,
                    EpicLootRaritySetsPlugin.NottSneakyStealthModifier.Value,
                    EpicLootRaritySetsPlugin.NottSneakySpeedModifier.Value * 100f,
                    FormatShortcut(EpicLootRaritySetsPlugin.NottWarpHotkey),
                    EpicLootRaritySetsPlugin.NottWarpStaminaUse.Value,
                    EpicLootRaritySetsPlugin.NottWarpCooldown.Value,
                    EpicLootRaritySetsPlugin.NottWarpRange.Value,
                    EpicLootRaritySetsPlugin.NottWarpResetRadius.Value,
                    EpicLootRaritySetsPlugin.NottWarpDamageMultiplier.Value,
                    EpicLootRaritySetsPlugin.NottHitSpeedBonus.Value * 100f,
                    EpicLootRaritySetsPlugin.NottHitSpeedDuration.Value,
                    EpicLootRaritySetsPlugin.NottPoisonBaseDamage.Value,
                    EpicLootRaritySetsPlugin.NottPoisonDamagePerKnivesLevel.Value,
                    knives,
                    poison);
            }

            if (string.Equals(baseSetName, "Ragnar", StringComparison.OrdinalIgnoreCase))
            {
                float axes = GetLocalSkillLevel(Skills.SkillType.Axes);
                float decay = ScaleSkillValue(EpicLootRaritySetsPlugin.RagnarDecayAuraBaseDamage.Value, EpicLootRaritySetsPlugin.RagnarDecayAuraDamagePerAxesLevel.Value, axes);
                return string.Format(
                    "Ragnar set completo activo.\n\nEscalado actual: Hachas {6:0.#}.\n\nPasivas:\nFury: cada golpe melee contra enemigo otorga +{8:0.#}% velocidad de ataque y {9:0.##}% robo de vida durante {10:0.#}s. Stackea hasta {11}. Maximo actual: +{12:0.#}% velocidad de ataque y {13:0.##}% robo de vida.\nBlood Surge: cada {14} golpes melee contra enemigos cura {15:0.#}% de tu salud maxima.\n\nHabilidad:\n{0}: Decay Aura toggle. Consume {1:0.#} vigor/s. Radio {2:0.#}m. Escala con Hachas: {3:0.#}+{4:0.##}/nivel. Resultado actual por tick cada {5:0.#}s: {7:0.#} veneno + {7:0.#} espiritu.",
                    FormatShortcut(EpicLootRaritySetsPlugin.RagnarDecayAuraHotkey),
                    EpicLootRaritySetsPlugin.RagnarDecayAuraStaminaPerSecond.Value,
                    EpicLootRaritySetsPlugin.RagnarDecayAuraRadius.Value,
                    EpicLootRaritySetsPlugin.RagnarDecayAuraBaseDamage.Value,
                    EpicLootRaritySetsPlugin.RagnarDecayAuraDamagePerAxesLevel.Value,
                    EpicLootRaritySetsPlugin.RagnarDecayAuraTickInterval.Value,
                    axes,
                    decay,
                    EpicLootRaritySetsPlugin.RagnarFuryAttackSpeedPerStack.Value * 100f,
                    EpicLootRaritySetsPlugin.RagnarFuryLifeStealPerStack.Value * 100f,
                    EpicLootRaritySetsPlugin.RagnarFuryDuration.Value,
                    EpicLootRaritySetsPlugin.RagnarFuryMaxStacks.Value,
                    EpicLootRaritySetsPlugin.RagnarFuryAttackSpeedPerStack.Value * EpicLootRaritySetsPlugin.RagnarFuryMaxStacks.Value * 100f,
                    EpicLootRaritySetsPlugin.RagnarFuryLifeStealPerStack.Value * EpicLootRaritySetsPlugin.RagnarFuryMaxStacks.Value * 100f,
                    EpicLootRaritySetsPlugin.RagnarFuryHealEveryAttacks.Value,
                    EpicLootRaritySetsPlugin.RagnarFuryHealMaxHealthFraction.Value * 100f);
            }

            if (string.Equals(baseSetName, "Heimdall", StringComparison.OrdinalIgnoreCase))
            {
                float blocking = GetLocalSkillLevel(Skills.SkillType.Blocking);
                float lightning = ScaleSkillValue(EpicLootRaritySetsPlugin.HeimdallLightningStormBaseDamage.Value, EpicLootRaritySetsPlugin.HeimdallLightningStormDamagePerBlockingLevel.Value, blocking);
                float stoneReduction = ScaleSkillValue(EpicLootRaritySetsPlugin.HeimdallStoneShieldBaseReduction.Value, EpicLootRaritySetsPlugin.HeimdallStoneShieldReductionPerBlockingLevel.Value, blocking);
                float stoneReflect = Mathf.Min(
                    Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallStoneShieldReflectBase.Value + blocking * EpicLootRaritySetsPlugin.HeimdallStoneShieldReflectPerBlockingLevel.Value),
                    Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallStoneShieldReflectMax.Value)) * 100f;
                return string.Format(
                    "Heimdall set completo activo.\n\nEscalado actual: Bloqueo {17:0.#}.\n\nPasiva:\nBloquear ataques otorga +{0:0.#}% reduccion de dano y +{0:0.#}% dano por stack durante {1:0.#}s. Stackea {2} veces. Resultado maximo: +{18:0.#}% reduccion y +{18:0.#}% dano.\n\nHabilidades:\n{3}: Lightning Storm de Thor. CD {4:0}s. Invoca una tormenta fija en el punto apuntado durante {5:0.#}s, radio {6:0.#}m, tick cada {7:0.#}s. Escala con Bloqueo: {8:0.#}+{9:0.##}/nivel = {19:0.#} rayo por tick; cada golpe fuerza la amenaza hacia Heimdall.\n{10}: Stone Shield. Coste {11:0} vigor. CD {12:0}s. Dura {13:0.#}s. Escala con Bloqueo: reduccion plana {14:0.#}+{15:0.##}/nivel = {20:0.#}; reflejo actual {21:0.#}% del dano mitigado.",
                    EpicLootRaritySetsPlugin.HeimdallBlockArmorBonusPerStack.Value * 100f,
                    EpicLootRaritySetsPlugin.HeimdallBlockArmorDuration.Value,
                    EpicLootRaritySetsPlugin.HeimdallBlockArmorMaxStacks.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.HeimdallLightningStormHotkey),
                    EpicLootRaritySetsPlugin.HeimdallLightningStormCooldown.Value,
                    EpicLootRaritySetsPlugin.HeimdallLightningStormDuration.Value,
                    EpicLootRaritySetsPlugin.HeimdallLightningStormRadius.Value,
                    EpicLootRaritySetsPlugin.HeimdallLightningStormTickInterval.Value,
                    EpicLootRaritySetsPlugin.HeimdallLightningStormBaseDamage.Value,
                    EpicLootRaritySetsPlugin.HeimdallLightningStormDamagePerBlockingLevel.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.HeimdallStoneShieldHotkey),
                    EpicLootRaritySetsPlugin.HeimdallStoneShieldStaminaUse.Value,
                    EpicLootRaritySetsPlugin.HeimdallStoneShieldCooldown.Value,
                    EpicLootRaritySetsPlugin.HeimdallStoneShieldDuration.Value,
                    EpicLootRaritySetsPlugin.HeimdallStoneShieldBaseReduction.Value,
                    EpicLootRaritySetsPlugin.HeimdallStoneShieldReductionPerBlockingLevel.Value,
                    EpicLootRaritySetsPlugin.HeimdallStoneShieldReflectMax.Value * 100f,
                    blocking,
                    EpicLootRaritySetsPlugin.HeimdallBlockArmorBonusPerStack.Value * EpicLootRaritySetsPlugin.HeimdallBlockArmorMaxStacks.Value * 100f,
                    lightning,
                    stoneReduction,
                    stoneReflect);
            }

            if (string.Equals(baseSetName, "Seidr", StringComparison.OrdinalIgnoreCase))
            {
                float elemental = GetLocalSkillLevel(Skills.SkillType.ElementalMagic);
                float frostNova = ScaleSkillValue(EpicLootRaritySetsPlugin.SeidrFrostNovaBaseDamage.Value, EpicLootRaritySetsPlugin.SeidrFrostNovaDamagePerElementalMagicLevel.Value, elemental);
                return string.Format(
                    "Seidr set completo activo.\n\nEscalado actual: Magia elemental {21:0.#}.\n\nHabilidades:\n{0}: Nanocube. Coste {1:0} eitr. CD {2:0}s. Dura {3:0.#}s, empuja enemigos fuera de {4:0.#}m. Dentro del cubo tu dano magico resultante se multiplica x{22:0.##} (+{5:0.#}%). No escala por skill.\n{6}: Elemental Shield toggle. Coste {7:0} eitr. Inmune al dano; consume {8:0.#}% eitr max/s, minimo 1 eitr/s. Sin CD, se puede activar/desactivar a placer. No escala por skill.\nAtaque secundario: Stone Golem friendly de Brokkr. Coste {10:0} eitr. CD {11:0}s. Invoca golem {12:0.#}s. La invocacion usa stats de criatura y no dano escalado directo.\n{13} + bloquear: Frost Nova. Coste {14:0} eitr. CD {15:0}s. Radio {16:0.#}m. Escala con Magia elemental: {17:0.#}+{18:0.##}/nivel = {23:0.#} frost y slow {19:0.#}% durante {20:0.#}s.",
                    FormatShortcut(EpicLootRaritySetsPlugin.SeidrNanoCubeHotkey),
                    EpicLootRaritySetsPlugin.SeidrNanoCubeEitrUse.Value,
                    EpicLootRaritySetsPlugin.SeidrNanoCubeCooldown.Value,
                    EpicLootRaritySetsPlugin.SeidrNanoCubeDuration.Value,
                    EpicLootRaritySetsPlugin.SeidrNanoCubeRadius.Value,
                    EpicLootRaritySetsPlugin.SeidrNanoCubeMagicDamageBonus.Value * 100f,
                    FormatShortcut(EpicLootRaritySetsPlugin.SeidrElementalShieldHotkey),
                    EpicLootRaritySetsPlugin.SeidrElementalShieldEitrUse.Value,
                    EpicLootRaritySetsPlugin.SeidrElementalShieldEitrPercentPerSecond.Value * 100f,
                    EpicLootRaritySetsPlugin.SeidrElementalShieldCooldown.Value,
                    EpicLootRaritySetsPlugin.SeidrStoneGolemEitrUse.Value,
                    EpicLootRaritySetsPlugin.SeidrStoneGolemCooldown.Value,
                    EpicLootRaritySetsPlugin.SeidrStoneGolemDuration.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.SeidrFrostNovaHotkey),
                    EpicLootRaritySetsPlugin.SeidrFrostNovaEitrUse.Value,
                    EpicLootRaritySetsPlugin.SeidrFrostNovaCooldown.Value,
                    EpicLootRaritySetsPlugin.SeidrFrostNovaRadius.Value,
                    EpicLootRaritySetsPlugin.SeidrFrostNovaBaseDamage.Value,
                    EpicLootRaritySetsPlugin.SeidrFrostNovaDamagePerElementalMagicLevel.Value,
                    EpicLootRaritySetsPlugin.SeidrFrostNovaSlow.Value * 100f,
                    EpicLootRaritySetsPlugin.SeidrFrostNovaSlowDuration.Value,
                    elemental,
                    1f + Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrNanoCubeMagicDamageBonus.Value),
                    frostNova);
            }

            if (string.Equals(baseSetName, "Helveig", StringComparison.OrdinalIgnoreCase))
            {
                float blood = GetLocalSkillLevel(Skills.SkillType.BloodMagic);
                float holyHeal = ScaleSkillValue(EpicLootRaritySetsPlugin.HelveigHolyHealBaseHealing.Value, EpicLootRaritySetsPlugin.HelveigHolyHealHealingPerBloodMagicLevel.Value, blood);
                float bloodRite = ScaleSkillValue(EpicLootRaritySetsPlugin.HelveigBloodRiteBaseHealing.Value, EpicLootRaritySetsPlugin.HelveigBloodRiteHealingPerBloodMagicLevel.Value, blood);
                float holyStrikeFire = ScaleSkillValue(EpicLootRaritySetsPlugin.HelveigHolyStrikeBaseFireDamage.Value, EpicLootRaritySetsPlugin.HelveigHolyStrikeFireDamagePerBloodMagicLevel.Value, blood);
                float holyStrikeSpirit = ScaleSkillValue(EpicLootRaritySetsPlugin.HelveigHolyStrikeBaseSpiritDamage.Value, EpicLootRaritySetsPlugin.HelveigHolyStrikeSpiritDamagePerBloodMagicLevel.Value, blood);
                return string.Format(
                    "Helveig set completo activo.\n\nEscalado actual: Magia de sangre {24:0.#}.\n\nHabilidades:\n{0}: Holy Heal. Coste: {1:0} eitr. CD: {2:0}s. Escala con Magia de sangre: {3:0.#}+{4:0.##}/nivel = {25:0.#} cura.\n{5}: Blood Rite. Coste: {6:0} eitr. CD: {7:0}s. Canaliza {8:0.#}s sin moverte, radio {9:0.#}m, tick cada {10:0.#}s. Cura aliados y a ti mismo: {11:0.#}+{12:0.##}/nivel = {26:0.#} por tick. El CD empieza al terminar o romperse.\nAtaque secundario: Holy Strike. Coste: {13:0} eitr. CD: {14:0}s. Rango {15:0.#}m. Escala con Magia de sangre: fuego {16:0.#}+{17:0.##}/nivel = {27:0.#}; espiritu {18:0.#}+{19:0.##}/nivel = {28:0.#}.\n{20} + bloquear: Summon Undead. Coste: {21:0} eitr. CD: {22:0}s. Dura {23:0.#}s. Escala por Magia de sangre: 0-29 Skeleton Hildir, 30-59 Fallen Warrior, 60-89 Unbjorn, 90-100 Charred Dyrnwyn. Invocacion actual: {29}.",
                    FormatShortcut(EpicLootRaritySetsPlugin.HelveigHolyHealHotkey),
                    EpicLootRaritySetsPlugin.HelveigHolyHealEitrUse.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyHealCooldown.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyHealBaseHealing.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyHealHealingPerBloodMagicLevel.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.HelveigBloodRiteHotkey),
                    EpicLootRaritySetsPlugin.HelveigBloodRiteEitrUse.Value,
                    EpicLootRaritySetsPlugin.HelveigBloodRiteCooldown.Value,
                    EpicLootRaritySetsPlugin.HelveigBloodRiteDuration.Value,
                    EpicLootRaritySetsPlugin.HelveigBloodRiteRadius.Value,
                    EpicLootRaritySetsPlugin.HelveigBloodRiteTickInterval.Value,
                    EpicLootRaritySetsPlugin.HelveigBloodRiteBaseHealing.Value,
                    EpicLootRaritySetsPlugin.HelveigBloodRiteHealingPerBloodMagicLevel.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyStrikeEitrUse.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyStrikeCooldown.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyStrikeRange.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyStrikeBaseFireDamage.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyStrikeFireDamagePerBloodMagicLevel.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyStrikeBaseSpiritDamage.Value,
                    EpicLootRaritySetsPlugin.HelveigHolyStrikeSpiritDamagePerBloodMagicLevel.Value,
                    FormatShortcut(EpicLootRaritySetsPlugin.HelveigSummonUndeadHotkey),
                    EpicLootRaritySetsPlugin.HelveigSummonUndeadEitrUse.Value,
                    EpicLootRaritySetsPlugin.HelveigSummonUndeadCooldown.Value,
                    EpicLootRaritySetsPlugin.HelveigSummonUndeadDuration.Value,
                    blood,
                    holyHeal,
                    bloodRite,
                    holyStrikeFire,
                    holyStrikeSpirit,
                    GetHelveigUndeadNameForSkill(blood));
            }

            return baseSetName + " set completo activo.";
        }

        private static float GetLocalSkillLevel(Skills.SkillType skillType)
        {
            Player player = Player.m_localPlayer;
            return player != null ? Mathf.Max(0f, player.GetSkillLevel(skillType)) : 0f;
        }

        private static float ScaleSkillValue(float baseValue, float perLevel, float skillLevel)
        {
            return Mathf.Max(0f, baseValue + skillLevel * perLevel);
        }

        private static float ClampScaledFraction(float baseValue, float perLevel, float skillLevel, float maxValue)
        {
            return Mathf.Min(Mathf.Clamp01(maxValue), Mathf.Clamp01(baseValue + skillLevel * perLevel));
        }

        private static string GetHelveigUndeadNameForSkill(float bloodMagic)
        {
            if (bloodMagic >= 90f)
            {
                return "Charred Dyrnwyn";
            }

            if (bloodMagic >= 60f)
            {
                return "Unbjorn";
            }

            if (bloodMagic >= 30f)
            {
                return "Fallen Warrior";
            }

            return "Skeleton Hildir";
        }

        private static string FormatShortcut(ConfigEntry<KeyboardShortcut> shortcut)
        {
            if (shortcut == null)
            {
                return "Sin tecla";
            }

            string value = shortcut.Value.ToString();
            return string.IsNullOrEmpty(value) ? "Sin tecla" : value;
        }

        private static Sprite FindActiveSetIcon(Player player, string baseSetName)
        {
            Inventory inventory = player.GetInventory();
            if (inventory == null)
            {
                return null;
            }

            foreach (ItemDrop.ItemData item in inventory.GetEquippedItems())
            {
                if (item == null || item.m_shared == null || item.m_shared.m_icons == null || item.m_shared.m_icons.Length == 0)
                {
                    continue;
                }

                MagicItem magicItem = ItemDataExtensions.GetMagicItem(item);
                string itemBaseSet = magicItem != null ? RaritySetRegistry.GetBaseSetName(magicItem.SetID) : null;
                if (string.Equals(itemBaseSet, baseSetName, StringComparison.OrdinalIgnoreCase))
                {
                    return item.m_shared.m_icons[0];
                }
            }

            return null;
        }
    }

    [HarmonyPatch(typeof(TextsDialog), "FillTextList")]
    internal static class RaritySetsCompendiumTextPatch
    {
        private static void Prefix(TextsDialog __instance)
        {
            RaritySetsCompendiumEntry.AddTo(__instance);
        }
    }

    internal static class RaritySetsCompendiumEntry
    {
        private const string Topic = "Epic Loot Rarity Sets";
        private static readonly FieldInfo TextsField = AccessTools.Field(typeof(TextsDialog), "m_texts");

        internal static void AddTo(TextsDialog dialog)
        {
            if (dialog == null || TextsField == null)
            {
                return;
            }

            List<TextsDialog.TextInfo> texts = TextsField.GetValue(dialog) as List<TextsDialog.TextInfo>;
            if (texts == null)
            {
                return;
            }

            string text = BuildText();
            for (int i = 0; i < texts.Count; i++)
            {
                TextsDialog.TextInfo info = texts[i];
                if (info == null || !string.Equals(info.m_topic, Topic, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                info.m_text = text;
                return;
            }

            texts.Add(new TextsDialog.TextInfo(Topic, text));
        }

        private static string BuildText()
        {
            return string.Join("\n\n", new[]
            {
                "Epic Loot Rarity Sets convierte EpicLoot en una progresion de clases por equipo. Completar las piezas necesarias de un set activa un buff con el nombre base del set y desbloquea las habilidades de esa clase.",
                "Rarezas disponibles: Magic, Rare, Epic, Legendary, Mythic y Ancient. Los sets suben de piezas y bonus con la rareza. Magic/Rare/Epic/Ancient pueden aparecer por drops naturales; Legendary y Mythic entran por las secciones generadas de EpicLoot y sus pools propios. Los bosses fuerzan una pieza de set garantizada cuando la regla de etapa encuentra una tirada valida.",
                "Entradas de clase y habilidades completas:",
                BuildSetBlock("Heimdall", "Tanque de escudo. Bloquea para ganar reduccion y dano, atrae amenaza con rayos y convierte el dano recibido en reflejo."),
                BuildSetBlock("Ragnar", "Berserker de hachas. Gana furia por golpes melee, robo de vida, velocidad de ataque y un aura de decadencia a costa de vigor."),
                BuildSetBlock("Hraesvelgr", "Arquero fisico. Usa sigilo, invocaciones, trampas, dash y rafagas de arco para jugar a distancia."),
                BuildSetBlock("SolomonKane", "Ballestero cazador de brujas. Prepara virotes imbuidos, bombas y marcas que convierten el siguiente disparo en sentencia encadenada."),
                BuildSetBlock("Nott", "Duelista de cuchillos y sigilo. Entra y sale de combate con Warp, veneno, velocidad por golpe e invisibilidad en sigilo."),
                BuildSetBlock("Seidr", "Mago elemental. Controla zona con Nanocube, escudo de eitr, golem y Frost Nova."),
                BuildSetBlock("Helveig", "Mago de sangre. Cura, canaliza Blood Rite, golpea a distancia con Holy Strike e invoca no muertos segun Magia de sangre."),
                BuildSetBlock("Moonvein", "Arquero magico. El Moonbow consume eitr, carga hechizos cada tercer disparo y puede invocar Meteor o Tornado Shot."),
                BuildSetBlock("Frostbrand", "Spellblade de espada a dos manos. Combina eitr, Surt Slash/Crush, escudo elemental y Fire Ball por ataques cargados."),
                BuildPassiveSetBlock("Thor", "Set Epic especial de tormenta. No tiene controlador de hotkeys propio de clase; su identidad viene de hacha arrojadiza, recall, dano de rayo y ChainLightning en sus piezas/bonus."),
                BuildPassiveSetBlock("Floki", "Set Epic especial de constructor. No tiene controlador de hotkeys propio de clase; potencia martillo de construccion, FreeBuild, distancia de construccion, carga, stamina y herramientas.")
            });
        }

        private static string BuildSetBlock(string baseSetName, string description)
        {
            return "== " + GetDisplayName(baseSetName) + " ==\n" + description + "\n\n" + SetActivationBuffController.GetBuffTooltip(baseSetName);
        }

        private static string BuildPassiveSetBlock(string baseSetName, string description)
        {
            return "== " + baseSetName + " ==\n" + description;
        }

        private static string GetDisplayName(string baseSetName)
        {
            return string.Equals(baseSetName, "SolomonKane", StringComparison.OrdinalIgnoreCase) ? "Solomon Kane" : baseSetName;
        }
    }

    internal static class AbilityCooldownBuffController
    {
        private const string BuffNamePrefix = "FranAbilityCooldown_";
        private const string BuffCategory = "FranAbilityCooldown";

        private static readonly Dictionary<string, ActiveCooldown> Cooldowns = new Dictionary<string, ActiveCooldown>(StringComparer.OrdinalIgnoreCase);
        private static Sprite _fallbackIcon;

        internal static void Start(Player player, string key, string displayName, float duration, Sprite icon)
        {
            if (player == null || string.IsNullOrEmpty(key) || duration <= 0f)
            {
                return;
            }

            ActiveCooldown active;
            if (!Cooldowns.TryGetValue(key, out active) || active == null)
            {
                active = new ActiveCooldown(key, displayName);
                Cooldowns[key] = active;
            }

            active.DisplayName = string.IsNullOrEmpty(displayName) ? key : displayName;
            active.Duration = Mathf.Max(0.1f, duration);
            active.Remaining = active.Duration;
            active.Icon = icon != null ? icon : GetFallbackIcon(player);
            Refresh(player, active);
        }

        internal static void Clear(Player player)
        {
            if (player != null)
            {
                SEMan seMan = player.GetSEMan();
                foreach (ActiveCooldown active in Cooldowns.Values.ToArray())
                {
                    if (active != null && active.Effect != null)
                    {
                        seMan.RemoveStatusEffect(active.Effect.NameHash(), true);
                    }
                }
            }

            Cooldowns.Clear();
        }

        internal static void Clear(Player player, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            ActiveCooldown active;
            if (!Cooldowns.TryGetValue(key, out active))
            {
                return;
            }

            if (player != null && active != null && active.Effect != null)
            {
                player.GetSEMan().RemoveStatusEffect(active.Effect.NameHash(), true);
            }

            Cooldowns.Remove(key);
        }

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer || Cooldowns.Count == 0)
            {
                return;
            }

            foreach (KeyValuePair<string, ActiveCooldown> pair in Cooldowns.ToArray())
            {
                ActiveCooldown active = pair.Value;
                if (active == null)
                {
                    Cooldowns.Remove(pair.Key);
                    continue;
                }

                active.Remaining -= dt;
                if (active.Remaining <= 0f)
                {
                    if (active.Effect != null)
                    {
                        player.GetSEMan().RemoveStatusEffect(active.Effect.NameHash(), true);
                    }

                    Cooldowns.Remove(pair.Key);
                    continue;
                }

                Refresh(player, active);
            }
        }

        private static void Refresh(Player player, ActiveCooldown active)
        {
            if (player == null || active == null)
            {
                return;
            }

            StatusEffect effect = active.GetOrCreateEffect();
            effect.m_ttl = Mathf.Max(0.1f, active.Remaining + 0.1f);
            effect.m_tooltip = string.Format("{0} en cooldown.\n\nTiempo restante: {1:0.#}s.", active.DisplayName, Mathf.Max(0f, active.Remaining));
            if (active.Icon != null)
            {
                effect.m_icon = active.Icon;
            }

            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(effect.NameHash(), true);
            seMan.AddStatusEffect(effect, true, 0, 0f, 0);
        }

        private static Sprite GetFallbackIcon(Player player)
        {
            if (player != null)
            {
                ItemDrop.ItemData weapon = player.GetCurrentWeapon();
                if (weapon != null && weapon.m_shared != null && weapon.m_shared.m_icons != null && weapon.m_shared.m_icons.Length > 0 && weapon.m_shared.m_icons[0] != null)
                {
                    return weapon.m_shared.m_icons[0];
                }

                Inventory inventory = player.GetInventory();
                if (inventory != null)
                {
                    foreach (ItemDrop.ItemData item in inventory.GetEquippedItems())
                    {
                        if (item != null && item.m_shared != null && item.m_shared.m_icons != null && item.m_shared.m_icons.Length > 0 && item.m_shared.m_icons[0] != null)
                        {
                            return item.m_shared.m_icons[0];
                        }
                    }
                }
            }

            if (_fallbackIcon == null)
            {
                Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                Color color = new Color(0.8f, 0.88f, 1f, 1f);
                Color[] pixels = Enumerable.Repeat(color, 16 * 16).ToArray();
                texture.SetPixels(pixels);
                texture.Apply();
                _fallbackIcon = Sprite.Create(texture, new Rect(0f, 0f, 16f, 16f), new Vector2(0.5f, 0.5f));
            }

            return _fallbackIcon;
        }

        private sealed class ActiveCooldown
        {
            internal readonly string Key;
            internal string DisplayName;
            internal float Duration;
            internal float Remaining;
            internal Sprite Icon;
            internal StatusEffect Effect;

            internal ActiveCooldown(string key, string displayName)
            {
                Key = key;
                DisplayName = string.IsNullOrEmpty(displayName) ? key : displayName;
            }

            internal StatusEffect GetOrCreateEffect()
            {
                if (Effect == null)
                {
                    Effect = ScriptableObject.CreateInstance<SE_Stats>();
                    Effect.name = BuffNamePrefix + Key;
                    Effect.m_name = DisplayName;
                    Effect.m_category = BuffCategory;
                    Effect.m_flashIcon = false;
                    Effect.m_cooldownIcon = true;
                    Effect.m_hidden = false;
                }

                Effect.m_name = DisplayName;
                return Effect;
            }
        }
    }

    internal static class SetAbilityInput
    {
        internal static bool IsSecondaryAttackDown()
        {
            return GetButtonDown("AltAttack") ||
                   GetButtonDown("SecondaryAttack") ||
                   GetButtonDown("SpecialAttack") ||
                   GetButtonDown("AttackSecondary") ||
                   Input.GetMouseButtonDown(2);
        }

        internal static bool IsBlockHeld()
        {
            return GetButton("Block") ||
                   GetButton("JoyBlock") ||
                   Input.GetMouseButton(1);
        }

        internal static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> shortcutEntry)
        {
            if (shortcutEntry == null)
            {
                return false;
            }

            KeyboardShortcut shortcut = shortcutEntry.Value;
            if (shortcut.IsDown())
            {
                return true;
            }

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool GetButtonDown(string name)
        {
            try
            {
                return ZInput.GetButtonDown(name);
            }
            catch
            {
                return false;
            }
        }

        private static bool GetButton(string name)
        {
            try
            {
                return ZInput.GetButton(name);
            }
            catch
            {
                return false;
            }
        }
    }

    internal static class NorseDashBridge
    {
        private const string NorseAssemblyName = "NorseDemigods";

        private static bool _initialized;
        private static bool _available;
        private static bool _loggedFailure;
        private static Type _abilityDashType;
        private static Type _abilityPropertyType;
        private static Type _abilityThemeType;
        private static Type _abilityParametersType;
        private static Type _abilityTypeType;
        private static Type _demigodType;
        private static ConstructorInfo _abilityConstructor;
        private static FieldInfo _propertyThemeField;
        private static FieldInfo _parametersAbilityTypeField;
        private static FieldInfo _parametersSkillTypesField;
        private static FieldInfo _parametersPropertiesField;
        private static FieldInfo _teleportPositionField;
        private static FieldInfo _teleportingToLookDirectionField;
        private static FieldInfo _layerMaskEnvNoTerrainField;
        private static MethodInfo _getTeleportRangeMethod;
        private static MethodInfo _damageEnemiesOnTheWayMethod;
        private static MethodInfo _dashMethod;
        private static MethodInfo _getSpawnPointMethod;
        private static object _natureTheme;
        private static object _slotOneAbilityType;

        internal static bool TryDash(Player player, out string reason)
        {
            reason = null;
            if (player == null)
            {
                reason = "Norse dash unavailable.";
                return false;
            }

            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                object ability = CreateAbility(player);
                Vector3 target;
                bool teleportingToLookDirection;
                if (!TryFindDashTarget(player, ability, out target, out teleportingToLookDirection))
                {
                    reason = "No Norse dash target.";
                    return false;
                }

                _teleportPositionField.SetValue(ability, target);
                _teleportingToLookDirectionField.SetValue(ability, teleportingToLookDirection);
                _damageEnemiesOnTheWayMethod.Invoke(ability, null);
                _dashMethod.Invoke(ability, null);
                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse dash failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static bool EnsureInitialized(out string reason)
        {
            reason = null;
            if (_initialized)
            {
                if (!_available)
                {
                    reason = "NorseDemigods dash unavailable.";
                }

                return _available;
            }

            _initialized = true;
            try
            {
                _abilityDashType = FindType("NorseDemigods.Abilities.AbilityDash");
                _abilityPropertyType = FindType("NorseDemigods.Abilities.AbilityDash+AbilityProperty");
                _abilityThemeType = FindType("NorseDemigods.Abilities.AbilityDash+AbilityTheme");
                _abilityParametersType = FindType("NorseDemigods.DemigodAbility+AbilityParametersStruct");
                _abilityTypeType = FindType("NorseDemigods.DemigodAbility+AbilityType");
                _demigodType = FindType("NorseDemigods.Demigod");
                Type helperType = FindType("NorseDemigods.Helper");
                Type cacheType = FindType("NorseDemigods.Cache");

                if (_abilityDashType == null || _abilityPropertyType == null || _abilityThemeType == null ||
                    _abilityParametersType == null || _abilityTypeType == null || _demigodType == null ||
                    helperType == null || cacheType == null)
                {
                    reason = "NorseDemigods dash unavailable.";
                    return false;
                }

                _abilityConstructor = _abilityDashType.GetConstructor(new[] { typeof(Player), _demigodType, _abilityParametersType });
                _propertyThemeField = _abilityPropertyType.GetField("AbilityTheme", BindingFlags.Public | BindingFlags.Instance);
                _parametersAbilityTypeField = _abilityParametersType.GetField("AbilityType", BindingFlags.Public | BindingFlags.Instance);
                _parametersSkillTypesField = _abilityParametersType.GetField("SkillTypes", BindingFlags.Public | BindingFlags.Instance);
                _parametersPropertiesField = _abilityParametersType.GetField("Properties", BindingFlags.Public | BindingFlags.Instance);
                _teleportPositionField = _abilityDashType.GetField("teleportPosition", BindingFlags.NonPublic | BindingFlags.Instance);
                _teleportingToLookDirectionField = _abilityDashType.GetField("teleportingToLookDirection", BindingFlags.NonPublic | BindingFlags.Instance);
                _getTeleportRangeMethod = _abilityDashType.GetMethod("GetTeleportRange", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _damageEnemiesOnTheWayMethod = _abilityDashType.GetMethod("DamageEnemiesOnTheWay", BindingFlags.NonPublic | BindingFlags.Instance);
                _dashMethod = _abilityDashType.GetMethod("Dash", BindingFlags.NonPublic | BindingFlags.Instance);
                _getSpawnPointMethod = helperType.GetMethod(
                    "GetSpawnPoint",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(float), typeof(float), typeof(Vector3).MakeByRefType(), typeof(bool) },
                    null);
                _layerMaskEnvNoTerrainField = cacheType.GetField("LayerMaskEnvNoTerrain", BindingFlags.Public | BindingFlags.Static);
                _natureTheme = Enum.Parse(_abilityThemeType, "NATURE");
                _slotOneAbilityType = Enum.Parse(_abilityTypeType, "SLOT_1");

                _available = _abilityConstructor != null && _propertyThemeField != null &&
                             _parametersAbilityTypeField != null && _parametersSkillTypesField != null &&
                             _parametersPropertiesField != null && _teleportPositionField != null &&
                             _teleportingToLookDirectionField != null && _getTeleportRangeMethod != null &&
                             _damageEnemiesOnTheWayMethod != null && _dashMethod != null &&
                             _getSpawnPointMethod != null && _layerMaskEnvNoTerrainField != null;

                if (!_available)
                {
                    reason = "NorseDemigods dash unavailable.";
                }

                return _available;
            }
            catch (Exception ex)
            {
                reason = "NorseDemigods dash unavailable.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static Type FindType(string fullName)
        {
            Type type = Type.GetType(fullName + ", " + NorseAssemblyName);
            if (type != null)
            {
                return type;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!string.Equals(assembly.GetName().Name, NorseAssemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private static object CreateAbility(Player player)
        {
            object properties = Activator.CreateInstance(_abilityPropertyType);
            _propertyThemeField.SetValue(properties, _natureTheme);

            object parameters = Activator.CreateInstance(_abilityParametersType);
            _parametersAbilityTypeField.SetValue(parameters, _slotOneAbilityType);
            _parametersSkillTypesField.SetValue(parameters, Array.CreateInstance(typeof(Skills.SkillType), 0));
            _parametersPropertiesField.SetValue(parameters, properties);

            return _abilityConstructor.Invoke(new[] { player, null, parameters });
        }

        private static bool TryFindDashTarget(Player player, object ability, out Vector3 target, out bool teleportingToLookDirection)
        {
            target = Vector3.zero;
            teleportingToLookDirection = false;

            float range = Convert.ToSingle(_getTeleportRangeMethod.Invoke(ability, null));
            if (range <= 0f || float.IsNaN(range) || float.IsInfinity(range))
            {
                return false;
            }

            Vector3 direction = player.GetLookYaw() * Vector3.forward;
            Vector3 heightOffset = new Vector3(0f, player.GetHeight(), 0f);
            Vector3 playerPosition = player.transform.position;
            List<Vector3> candidates = new List<Vector3>();
            Vector3 spawnPoint;

            if (TryGetNorseSpawnPoint(playerPosition + new Vector3(0f, 3.5f, 0f), direction, range, player.GetRadius(), 10f, out spawnPoint) &&
                !Physics.Linecast(playerPosition + heightOffset, spawnPoint + heightOffset, GetEnvNoTerrainLayerMask()))
            {
                candidates.Add(spawnPoint);
            }

            if (TryGetNorseSpawnPoint(playerPosition + heightOffset, direction, range, player.GetRadius(), 10f, out spawnPoint) &&
                !Physics.Linecast(playerPosition + heightOffset, spawnPoint + heightOffset, GetEnvNoTerrainLayerMask()))
            {
                candidates.Add(spawnPoint);
            }

            if (candidates.Count == 0)
            {
                return false;
            }

            target = candidates
                .OrderByDescending(candidate => Vector3.Distance(playerPosition, candidate))
                .First();
            return true;
        }

        private static bool TryGetNorseSpawnPoint(Vector3 referencePosition, Vector3 direction, float range, float radius, float maxHeightDifference, out Vector3 spawnPoint)
        {
            object[] args =
            {
                referencePosition,
                direction,
                range,
                radius,
                maxHeightDifference,
                Vector3.zero,
                false
            };

            bool result = (bool)_getSpawnPointMethod.Invoke(null, args);
            spawnPoint = (Vector3)args[5];
            return result;
        }

        private static int GetEnvNoTerrainLayerMask()
        {
            object layerMask = _layerMaskEnvNoTerrainField.GetValue(null);
            if (layerMask is LayerMask)
            {
                LayerMask unityLayerMask = (LayerMask)layerMask;
                return unityLayerMask.value;
            }

            if (layerMask is int)
            {
                return (int)layerMask;
            }

            return Physics.DefaultRaycastLayers;
        }

        private static void LogFailure(string reason, Exception ex)
        {
            if (_loggedFailure)
            {
                return;
            }

            _loggedFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(reason + " " + ex.GetBaseException().Message);
        }
    }

    internal static class NorseDemigodsSuppression
    {
        private const string NorseAssemblyName = "NorseDemigods";
        private static readonly string[] ManagedSets =
        {
            "Frostbrand",
            "Hraesvelgr",
            "SolomonKane",
            "Moonvein",
            "Nott",
            "Helveig",
            "Heimdall",
            "Seidr"
        };

        private static bool _removeMetersFailed;
        private static MethodInfo _removeMetersMethod;

        internal static bool ShouldSuppressLocal()
        {
            return Player.m_localPlayer != null &&
                   ManagedSets.Any(SetActivationBuffController.HasActiveSet);
        }

        internal static void RemoveMeters()
        {
            if (_removeMetersFailed)
            {
                return;
            }

            try
            {
                if (_removeMetersMethod == null)
                {
                    Type customUiType = FindNorseType("NorseDemigods.CustomUI");
                    _removeMetersMethod = customUiType != null
                        ? customUiType.GetMethod("RemoveMeters", BindingFlags.Public | BindingFlags.Static)
                        : null;
                }

                if (_removeMetersMethod != null)
                {
                    _removeMetersMethod.Invoke(null, null);
                }
            }
            catch (Exception ex)
            {
                _removeMetersFailed = true;
                EpicLootRaritySetsPlugin.Log.LogWarning("Could not hide NorseDemigods meters while Frostbrand is active. " + ex.GetBaseException().Message);
            }
        }

        internal static Type FindNorseType(string fullName)
        {
            Type type = Type.GetType(fullName + ", " + NorseAssemblyName);
            if (type != null)
            {
                return type;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!string.Equals(assembly.GetName().Name, NorseAssemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }

    internal static class NorseEirAbilityBridge
    {
        private static bool _initialized;
        private static bool _available;
        private static bool _loggedFailure;
        private static int _bridgeExecutionDepth;
        private static int _bridgeUpdateDepth;
        private static Type _abilityHolyStrikeType;
        private static Type _abilityHolySunType;
        private static Type _abilityParametersType;
        private static Type _abilityTypeType;
        private static Type _demigodType;
        private static Type _hitInfoType;
        private static ConstructorInfo _holyStrikeConstructor;
        private static ConstructorInfo _holySunConstructor;
        private static FieldInfo _parametersAbilityTypeField;
        private static FieldInfo _parametersSkillTypesField;
        private static FieldInfo _parametersPropertiesField;
        private static FieldInfo _holyStrikeTargetCharacterField;
        private static FieldInfo _holyStrikeRangeField;
        private static FieldInfo _holySunLifeTimerField;
        private static FieldInfo _energyRequiredField;
        private static FieldInfo _energyCostField;
        private static FieldInfo _eitrCostField;
        private static FieldInfo _cooldownField;
        private static MethodInfo _holyStrikeExecuteMethod;
        private static MethodInfo _holySunExecuteMethod;
        private static MethodInfo _holySunUpdateMethod;
        private static MethodInfo _findCrosshairTargetMethod;
        private static MethodInfo _areCharactersEnemiesMethod;
        private static PropertyInfo _hitInfoColliderProperty;
        private static object _slotTwoAbilityType;
        private static object _slotThreeAbilityType;
        private static readonly List<object> ActiveHolySuns = new List<object>();

        internal static bool IsBridgeExecuting
        {
            get { return _bridgeExecutionDepth > 0; }
        }

        internal static bool IsBridgeUpdating
        {
            get { return _bridgeUpdateDepth > 0; }
        }

        internal static void Update(float dt)
        {
            if (ActiveHolySuns.Count == 0)
            {
                return;
            }

            string failureReason;
            if (!EnsureInitialized(out failureReason))
            {
                ActiveHolySuns.Clear();
                return;
            }

            for (int i = ActiveHolySuns.Count - 1; i >= 0; i--)
            {
                object ability = ActiveHolySuns[i];
                try
                {
                    _bridgeUpdateDepth++;
                    try
                    {
                        _holySunUpdateMethod.Invoke(ability, new object[] { dt });
                    }
                    finally
                    {
                        _bridgeUpdateDepth--;
                    }

                    if (GetHolySunLifeTimer(ability) < 0f)
                    {
                        ActiveHolySuns.RemoveAt(i);
                    }
                }
                catch (Exception ex)
                {
                    ActiveHolySuns.RemoveAt(i);
                    LogFailure("Norse Holy Sun update failed.", ex);
                }
            }
        }

        internal static bool TryHolyStrike(Player player, out string reason)
        {
            reason = null;
            if (player == null)
            {
                reason = "Norse Holy Strike unavailable.";
                return false;
            }

            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            Character target;
            if (!TryFindHolyStrikeTarget(player, out target))
            {
                reason = "No holy strike target.";
                return false;
            }

            try
            {
                object ability = CreateAbility(_holyStrikeConstructor, player, _slotTwoAbilityType);
                NeutralizeNorseCosts(ability);
                _holyStrikeTargetCharacterField.SetValue(ability, target);
                InvokeBridgeExecute(_holyStrikeExecuteMethod, ability);
                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse Holy Strike failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        internal static bool TryHolySun(Player player, out string reason)
        {
            reason = null;
            if (player == null)
            {
                reason = "Norse Holy Sun unavailable.";
                return false;
            }

            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                object ability = CreateAbility(_holySunConstructor, player, _slotThreeAbilityType);
                NeutralizeNorseCosts(ability);
                InvokeBridgeExecute(_holySunExecuteMethod, ability);
                if (GetHolySunLifeTimer(ability) < 0f)
                {
                    reason = "No holy sun target.";
                    return false;
                }

                ActiveHolySuns.Add(ability);
                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse Holy Sun failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static bool EnsureInitialized(out string reason)
        {
            reason = null;
            if (_initialized)
            {
                if (!_available)
                {
                    reason = "Norse Eir abilities unavailable.";
                }

                return _available;
            }

            _initialized = true;
            try
            {
                _abilityHolyStrikeType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Abilities.AbilityHolyStrike");
                _abilityHolySunType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Abilities.AbilityHolySun");
                _abilityParametersType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityParametersStruct");
                _abilityTypeType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityType");
                _demigodType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Demigod");
                _hitInfoType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Helper+HitInfo");
                Type helperType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Helper");
                Type demigodAbilityType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility");

                if (_abilityHolyStrikeType == null || _abilityHolySunType == null ||
                    _abilityParametersType == null || _abilityTypeType == null ||
                    _demigodType == null || _hitInfoType == null ||
                    helperType == null || demigodAbilityType == null)
                {
                    reason = "Norse Eir abilities unavailable.";
                    return false;
                }

                Type[] constructorTypes = { typeof(Player), _demigodType, _abilityParametersType };
                _holyStrikeConstructor = _abilityHolyStrikeType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, constructorTypes, null);
                _holySunConstructor = _abilityHolySunType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, constructorTypes, null);
                _parametersAbilityTypeField = _abilityParametersType.GetField("AbilityType", BindingFlags.Public | BindingFlags.Instance);
                _parametersSkillTypesField = _abilityParametersType.GetField("SkillTypes", BindingFlags.Public | BindingFlags.Instance);
                _parametersPropertiesField = _abilityParametersType.GetField("Properties", BindingFlags.Public | BindingFlags.Instance);
                _holyStrikeTargetCharacterField = _abilityHolyStrikeType.GetField("targetCharacter", BindingFlags.NonPublic | BindingFlags.Instance);
                _holyStrikeRangeField = _abilityHolyStrikeType.GetField("AbilityRange", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                _holySunLifeTimerField = _abilityHolySunType.GetField("lifeTimer", BindingFlags.NonPublic | BindingFlags.Instance);
                _energyRequiredField = demigodAbilityType.GetField("<EnergyRequired>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _energyCostField = demigodAbilityType.GetField("<EnergyCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _eitrCostField = demigodAbilityType.GetField("<EitrCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _cooldownField = demigodAbilityType.GetField("<Cooldown>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _holyStrikeExecuteMethod = _abilityHolyStrikeType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _holySunExecuteMethod = _abilityHolySunType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _holySunUpdateMethod = _abilityHolySunType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
                _findCrosshairTargetMethod = helperType.GetMethod(
                    "FindCrosshairTarget",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(Player), typeof(float), _hitInfoType.MakeByRefType(), typeof(float), typeof(float) },
                    null);
                _areCharactersEnemiesMethod = helperType.GetMethod(
                    "AreCharactersEnemies",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(Character), typeof(Character) },
                    null);
                _hitInfoColliderProperty = _hitInfoType.GetProperty("Collider", BindingFlags.Public | BindingFlags.Instance);
                _slotTwoAbilityType = Enum.Parse(_abilityTypeType, "SLOT_2");
                _slotThreeAbilityType = Enum.Parse(_abilityTypeType, "SLOT_3");

                _available = _holyStrikeConstructor != null && _holySunConstructor != null &&
                             _parametersAbilityTypeField != null && _parametersSkillTypesField != null &&
                             _parametersPropertiesField != null && _holyStrikeTargetCharacterField != null &&
                             _holyStrikeRangeField != null && _holySunLifeTimerField != null &&
                             _energyRequiredField != null && _energyCostField != null &&
                             _eitrCostField != null && _cooldownField != null &&
                             _holyStrikeExecuteMethod != null && _holySunExecuteMethod != null &&
                             _holySunUpdateMethod != null && _findCrosshairTargetMethod != null &&
                             _areCharactersEnemiesMethod != null && _hitInfoColliderProperty != null;

                if (!_available)
                {
                    reason = "Norse Eir abilities unavailable.";
                }

                return _available;
            }
            catch (Exception ex)
            {
                reason = "Norse Eir abilities unavailable.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static object CreateAbility(ConstructorInfo constructor, Player player, object abilityType)
        {
            object parameters = Activator.CreateInstance(_abilityParametersType);
            _parametersAbilityTypeField.SetValue(parameters, abilityType);
            _parametersSkillTypesField.SetValue(parameters, Array.CreateInstance(typeof(Skills.SkillType), 0));
            _parametersPropertiesField.SetValue(parameters, null);

            return constructor.Invoke(new[] { player, null, parameters });
        }

        private static void NeutralizeNorseCosts(object ability)
        {
            _energyRequiredField.SetValue(ability, 0);
            _energyCostField.SetValue(ability, 0);
            _eitrCostField.SetValue(ability, 0);
            _cooldownField.SetValue(ability, 0f);
        }

        private static void InvokeBridgeExecute(MethodInfo executeMethod, object ability)
        {
            _bridgeExecutionDepth++;
            try
            {
                executeMethod.Invoke(ability, null);
            }
            finally
            {
                _bridgeExecutionDepth--;
            }
        }

        private static bool TryFindHolyStrikeTarget(Player player, out Character target)
        {
            target = null;
            object hitInfo = Activator.CreateInstance(_hitInfoType);
            object[] args =
            {
                player,
                GetHolyStrikeRange(),
                hitInfo,
                0.2f,
                0.5f
            };

            if (!(bool)_findCrosshairTargetMethod.Invoke(null, args))
            {
                return false;
            }

            Collider collider = _hitInfoColliderProperty.GetValue(args[2], null) as Collider;
            if (collider == null)
            {
                return false;
            }

            GameObject hitObject = Projectile.FindHitObject(collider);
            if (hitObject == null)
            {
                return false;
            }

            target = hitObject.GetComponent<Character>();
            return target != null && (bool)_areCharactersEnemiesMethod.Invoke(null, new object[] { player, target });
        }

        private static float GetHolyStrikeRange()
        {
            object configEntry = _holyStrikeRangeField.GetValue(null);
            PropertyInfo valueProperty = configEntry != null ? configEntry.GetType().GetProperty("Value") : null;
            if (valueProperty == null)
            {
                return EpicLootRaritySetsPlugin.FrostbrandHolyStrikeRange.Value;
            }

            return Convert.ToSingle(valueProperty.GetValue(configEntry, null));
        }

        private static float GetHolySunLifeTimer(object ability)
        {
            return Convert.ToSingle(_holySunLifeTimerField.GetValue(ability));
        }

        private static void LogFailure(string reason, Exception ex)
        {
            if (_loggedFailure)
            {
                return;
            }

            _loggedFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(reason + " " + ex.GetBaseException().Message);
        }
    }

    internal static class NorseWaterSphereBridge
    {
        private static readonly List<ActiveWaterSphere> ActiveWaterSpheres = new List<ActiveWaterSphere>();
        private static bool _initialized;
        private static bool _available;
        private static bool _loggedFailure;
        private static int _bridgeExecutionDepth;
        private static int _bridgeUpdateDepth;
        private static Type _abilityWaterSphereType;
        private static Type _abilityParametersType;
        private static Type _abilityTypeType;
        private static Type _demigodType;
        private static ConstructorInfo _constructorWithPlayer;
        private static ConstructorInfo _constructorSimple;
        private static FieldInfo _parametersAbilityTypeField;
        private static FieldInfo _parametersSkillTypesField;
        private static FieldInfo _parametersPropertiesField;
        private static FieldInfo _energyRequiredField;
        private static FieldInfo _energyCostField;
        private static FieldInfo _eitrCostField;
        private static FieldInfo _cooldownField;
        private static FieldInfo _waterSphereListField;
        private static FieldInfo _sphereFxField;
        private static MethodInfo _setupConfigsMethod;
        private static MethodInfo _executeMethod;
        private static MethodInfo _updateMethod;
        private static MethodInfo _stopMethod;
        private static MethodInfo _baseAIIsEnemyMethod;
        private static object _slotTwoAbilityType;
        private static ConfigFile _norseConfig;

        internal static bool IsBridgeExecuting
        {
            get { return _bridgeExecutionDepth > 0; }
        }

        internal static bool IsBridgeUpdating
        {
            get { return _bridgeUpdateDepth > 0; }
        }

        internal static void Update(float dt)
        {
            if (ActiveWaterSpheres.Count == 0)
            {
                return;
            }

            string reason;
            if (!EnsureInitialized(out reason))
            {
                ActiveWaterSpheres.Clear();
                return;
            }

            for (int i = ActiveWaterSpheres.Count - 1; i >= 0; i--)
            {
                ActiveWaterSphere active = ActiveWaterSpheres[i];
                if (active == null || active.Owner == null || active.Ability == null)
                {
                    ActiveWaterSpheres.RemoveAt(i);
                    continue;
                }

                active.Elapsed += dt;
                try
                {
                    _bridgeUpdateDepth++;
                    try
                    {
                        _updateMethod.Invoke(active.Ability, new object[] { dt });
                    }
                    finally
                    {
                        _bridgeUpdateDepth--;
                    }

                    ApplyGravityPull(active.Owner, active.Ability, dt);
                    if (active.Elapsed >= Mathf.Max(1f, EpicLootRaritySetsPlugin.FrostbrandWaterSphereMaxDuration.Value) ||
                        !HasLiveSpheres(active.Ability))
                    {
                        Stop(active.Ability);
                        ActiveWaterSpheres.RemoveAt(i);
                    }
                }
                catch (Exception ex)
                {
                    Stop(active.Ability);
                    ActiveWaterSpheres.RemoveAt(i);
                    LogFailure("Norse Water Sphere update failed.", ex);
                }
            }
        }

        internal static bool TryWaterSphere(Player player, out string reason)
        {
            reason = null;
            if (player == null)
            {
                reason = "Norse Water Sphere unavailable.";
                return false;
            }

            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                object ability = CreateAbility(player);
                SetupConfigs(ability);
                NeutralizeNorseCosts(ability);
                InvokeBridgeExecute(ability);
                ActiveWaterSpheres.Add(new ActiveWaterSphere(player, ability));
                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse Water Sphere failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static bool EnsureInitialized(out string reason)
        {
            reason = null;
            if (_initialized)
            {
                if (!_available)
                {
                    reason = "Norse Water Sphere unavailable.";
                }

                return _available;
            }

            _initialized = true;
            try
            {
                _abilityWaterSphereType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Abilities.AbilityWaterSphere");
                _abilityParametersType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityParametersStruct");
                _abilityTypeType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityType");
                _demigodType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Demigod");
                Type demigodAbilityType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility");

                if (_abilityWaterSphereType == null || _abilityParametersType == null ||
                    _abilityTypeType == null || demigodAbilityType == null)
                {
                    reason = "Norse Water Sphere unavailable.";
                    return false;
                }

                Type[] constructorTypes = { typeof(Player), _demigodType, _abilityParametersType };
                _constructorWithPlayer = _demigodType != null
                    ? _abilityWaterSphereType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, constructorTypes, null)
                    : null;
                _constructorSimple = _abilityWaterSphereType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { _abilityParametersType }, null);
                _parametersAbilityTypeField = _abilityParametersType.GetField("AbilityType", BindingFlags.Public | BindingFlags.Instance);
                _parametersSkillTypesField = _abilityParametersType.GetField("SkillTypes", BindingFlags.Public | BindingFlags.Instance);
                _parametersPropertiesField = _abilityParametersType.GetField("Properties", BindingFlags.Public | BindingFlags.Instance);
                _energyRequiredField = demigodAbilityType.GetField("<EnergyRequired>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _energyCostField = demigodAbilityType.GetField("<EnergyCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _eitrCostField = demigodAbilityType.GetField("<EitrCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _cooldownField = demigodAbilityType.GetField("<Cooldown>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _waterSphereListField = _abilityWaterSphereType.GetField("WaterSphereList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                Type waterSphereType = _abilityWaterSphereType.GetNestedType("WaterSphere", BindingFlags.Public | BindingFlags.NonPublic);
                _sphereFxField = waterSphereType != null ? waterSphereType.GetField("fxWaterSphere", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) : null;
                _setupConfigsMethod = _abilityWaterSphereType.GetMethod("SetupConfigs", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(ConfigFile), typeof(string) }, null);
                _executeMethod = _abilityWaterSphereType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _updateMethod = _abilityWaterSphereType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
                _stopMethod = demigodAbilityType.GetMethod("Stop", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(bool) }, null);
                _baseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
                _slotTwoAbilityType = Enum.Parse(_abilityTypeType, "SLOT_2");

                _available = (_constructorWithPlayer != null || _constructorSimple != null) &&
                             _parametersAbilityTypeField != null && _parametersSkillTypesField != null &&
                             _parametersPropertiesField != null && _waterSphereListField != null &&
                             _sphereFxField != null && _executeMethod != null && _updateMethod != null;

                if (!_available)
                {
                    reason = "Norse Water Sphere unavailable.";
                }

                return _available;
            }
            catch (Exception ex)
            {
                reason = "Norse Water Sphere unavailable.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static object CreateAbility(Player player)
        {
            object parameters = Activator.CreateInstance(_abilityParametersType);
            Array skillTypes = Array.CreateInstance(typeof(Skills.SkillType), 1);
            skillTypes.SetValue(Skills.SkillType.ElementalMagic, 0);
            _parametersAbilityTypeField.SetValue(parameters, _slotTwoAbilityType);
            _parametersSkillTypesField.SetValue(parameters, skillTypes);
            _parametersPropertiesField.SetValue(parameters, null);

            if (_constructorWithPlayer != null)
            {
                object demigod = _demigodType != null ? player.GetComponent(_demigodType) : null;
                return _constructorWithPlayer.Invoke(new[] { player, demigod, parameters });
            }

            return _constructorSimple.Invoke(new[] { parameters });
        }

        private static void SetupConfigs(object ability)
        {
            if (_setupConfigsMethod == null || ability == null)
            {
                return;
            }

            ConfigFile config = GetNorseConfig();
            if (config != null)
            {
                _setupConfigsMethod.Invoke(ability, new object[] { config, "Ability Water Sphere" });
            }
        }

        private static ConfigFile GetNorseConfig()
        {
            if (_norseConfig != null)
            {
                return _norseConfig;
            }

            try
            {
                string configPath = Path.Combine(Paths.ConfigPath, "NorseDemigods.cfg");
                if (File.Exists(configPath))
                {
                    _norseConfig = new ConfigFile(configPath, true);
                    return _norseConfig;
                }
            }
            catch
            {
            }

            return EpicLootRaritySetsPlugin.PluginConfig;
        }

        private static void NeutralizeNorseCosts(object ability)
        {
            if (_energyRequiredField != null)
            {
                _energyRequiredField.SetValue(ability, 0);
            }

            if (_energyCostField != null)
            {
                _energyCostField.SetValue(ability, 0);
            }

            if (_eitrCostField != null)
            {
                _eitrCostField.SetValue(ability, 0);
            }

            if (_cooldownField != null)
            {
                _cooldownField.SetValue(ability, 0f);
            }
        }

        private static void InvokeBridgeExecute(object ability)
        {
            _bridgeExecutionDepth++;
            try
            {
                _executeMethod.Invoke(ability, null);
            }
            finally
            {
                _bridgeExecutionDepth--;
            }
        }

        private static bool HasLiveSpheres(object ability)
        {
            foreach (Vector3 ignored in GetSpherePositions(ability))
            {
                return true;
            }

            return false;
        }

        private static IEnumerable<Vector3> GetSpherePositions(object ability)
        {
            IEnumerable spheres = _waterSphereListField != null ? _waterSphereListField.GetValue(ability) as IEnumerable : null;
            if (spheres == null)
            {
                yield break;
            }

            foreach (object sphere in spheres)
            {
                if (sphere == null)
                {
                    continue;
                }

                GameObject fx = _sphereFxField.GetValue(sphere) as GameObject;
                if (fx != null)
                {
                    yield return fx.transform.position;
                }
            }
        }

        private static void ApplyGravityPull(Player owner, object ability, float dt)
        {
            if (owner == null || ability == null)
            {
                return;
            }

            float radius = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.FrostbrandWaterSpherePullRadius.Value);
            float force = Mathf.Max(0f, EpicLootRaritySetsPlugin.FrostbrandWaterSpherePullForce.Value);
            if (force <= 0f)
            {
                return;
            }

            float radiusSqr = radius * radius;
            foreach (Vector3 spherePosition in GetSpherePositions(ability))
            {
                foreach (Character character in Character.GetAllCharacters())
                {
                    if (character == null || character == owner || character is Player || character.IsDead() || !IsEnemyTarget(owner, character))
                    {
                        continue;
                    }

                    Vector3 delta = spherePosition - character.transform.position;
                    float sqrDistance = delta.sqrMagnitude;
                    if (sqrDistance <= 0.01f || sqrDistance > radiusSqr)
                    {
                        continue;
                    }

                    Vector3 pull = delta.normalized * (force * dt * Mathf.Lerp(1.4f, 0.35f, Mathf.Sqrt(sqrDistance) / radius));
                    Rigidbody body = character.GetComponent<Rigidbody>();
                    if (body != null)
                    {
                        body.velocity += pull;
                    }
                    else
                    {
                        character.transform.position += pull * 0.25f;
                    }
                }
            }
        }

        private static bool IsEnemyTarget(Player owner, Character target)
        {
            if (owner == null || target == null || target == owner || target is Player)
            {
                return false;
            }

            if (_baseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = _baseAIIsEnemyMethod.Invoke(null, new object[] { owner, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        private static void Stop(object ability)
        {
            if (ability == null || _stopMethod == null)
            {
                return;
            }

            try
            {
                _stopMethod.Invoke(ability, new object[] { false });
            }
            catch
            {
            }
        }

        private static void LogFailure(string reason, Exception ex)
        {
            if (_loggedFailure)
            {
                return;
            }

            _loggedFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(reason + " " + ex.GetBaseException().Message);
        }

        private sealed class ActiveWaterSphere
        {
            internal readonly Player Owner;
            internal readonly object Ability;
            internal float Elapsed;

            internal ActiveWaterSphere(Player owner, object ability)
            {
                Owner = owner;
                Ability = ability;
                Elapsed = 0f;
            }
        }
    }

    internal static class NorseNjordTornadoBridge
    {
        private static bool _initialized;
        private static bool _available;
        private static bool _loggedFailure;
        private static int _bridgeExecutionDepth;
        private static int _bridgeUpdateDepth;
        private static Type _abilityTornadoType;
        private static Type _abilityParametersType;
        private static Type _abilityTypeType;
        private static Type _demigodType;
        private static ConstructorInfo _constructorWithPlayer;
        private static ConstructorInfo _constructorSimple;
        private static FieldInfo _parametersAbilityTypeField;
        private static FieldInfo _parametersSkillTypesField;
        private static FieldInfo _parametersPropertiesField;
        private static FieldInfo _energyRequiredField;
        private static FieldInfo _energyCostField;
        private static FieldInfo _eitrCostField;
        private static FieldInfo _cooldownField;
        private static FieldInfo _tornadoFxField;
        private static FieldInfo _directionField;
        private static FieldInfo _speedField;
        private static FieldInfo _lifeTimerField;
        private static FieldInfo _tornadoHeightField;
        private static MethodInfo _setupConfigsMethod;
        private static MethodInfo _executeMethod;
        private static MethodInfo _updateMethod;
        private static MethodInfo _stopMethod;
        private static object _slotFourAbilityType;
        private static ConfigFile _norseConfig;

        internal static bool IsBridgeExecuting
        {
            get { return _bridgeExecutionDepth > 0; }
        }

        internal static bool IsBridgeUpdating
        {
            get { return _bridgeUpdateDepth > 0; }
        }

        internal static bool TrySpawn(Player player, Vector3 position, out object ability, out GameObject effectObject, out string reason)
        {
            ability = null;
            effectObject = null;
            reason = null;
            if (player == null)
            {
                reason = "Norse tornado unavailable.";
                return false;
            }

            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                ability = CreateAbility(player);
                SetupConfigs(ability);
                NeutralizeNorseCosts(ability);
                InvokeBridgeExecute(ability);
                ForceTornadoLifetime(ability);
                PinTornadoAt(ability, position, out effectObject);
                if (effectObject == null)
                {
                    reason = "Norse tornado visual unavailable.";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse tornado failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        internal static bool Update(object ability, Vector3 position, float dt)
        {
            if (ability == null)
            {
                return false;
            }

            string reason;
            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                _bridgeUpdateDepth++;
                try
                {
                    _updateMethod.Invoke(ability, new object[] { dt });
                }
                finally
                {
                    _bridgeUpdateDepth--;
                }

                ForceTornadoLifetime(ability);
                GameObject effectObject;
                PinTornadoAt(ability, position, out effectObject);
                return true;
            }
            catch (Exception ex)
            {
                LogFailure("Norse tornado update failed.", ex);
                return false;
            }
        }

        internal static void Stop(object ability)
        {
            if (ability == null)
            {
                return;
            }

            string reason;
            if (!EnsureInitialized(out reason))
            {
                return;
            }

            try
            {
                if (_stopMethod != null)
                {
                    _stopMethod.Invoke(ability, new object[] { false });
                }
            }
            catch
            {
            }
        }

        private static bool EnsureInitialized(out string reason)
        {
            reason = null;
            if (_initialized)
            {
                if (!_available)
                {
                    reason = "NorseDemigods tornado unavailable.";
                }

                return _available;
            }

            _initialized = true;
            try
            {
                _abilityTornadoType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Abilities.AbilityTornado");
                _abilityParametersType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityParametersStruct");
                _abilityTypeType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityType");
                _demigodType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Demigod");
                Type demigodAbilityType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility");

                if (_abilityTornadoType == null || _abilityParametersType == null ||
                    _abilityTypeType == null || demigodAbilityType == null)
                {
                    reason = "NorseDemigods tornado unavailable.";
                    return false;
                }

                Type[] constructorTypes = { typeof(Player), _demigodType, _abilityParametersType };
                _constructorWithPlayer = _demigodType != null
                    ? _abilityTornadoType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, constructorTypes, null)
                    : null;
                _constructorSimple = _abilityTornadoType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { _abilityParametersType }, null);
                _parametersAbilityTypeField = _abilityParametersType.GetField("AbilityType", BindingFlags.Public | BindingFlags.Instance);
                _parametersSkillTypesField = _abilityParametersType.GetField("SkillTypes", BindingFlags.Public | BindingFlags.Instance);
                _parametersPropertiesField = _abilityParametersType.GetField("Properties", BindingFlags.Public | BindingFlags.Instance);
                _energyRequiredField = demigodAbilityType.GetField("<EnergyRequired>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _energyCostField = demigodAbilityType.GetField("<EnergyCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _eitrCostField = demigodAbilityType.GetField("<EitrCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _cooldownField = demigodAbilityType.GetField("<Cooldown>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _tornadoFxField = _abilityTornadoType.GetField("tornadoFx", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _directionField = _abilityTornadoType.GetField("direction", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _speedField = _abilityTornadoType.GetField("speed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _lifeTimerField = _abilityTornadoType.GetField("lifeTimer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _tornadoHeightField = _abilityTornadoType.GetField("TornadoHeight", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _setupConfigsMethod = _abilityTornadoType.GetMethod("SetupConfigs", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(ConfigFile), typeof(string) }, null);
                _executeMethod = _abilityTornadoType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _updateMethod = _abilityTornadoType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
                _stopMethod = demigodAbilityType.GetMethod("Stop", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(bool) }, null);
                _slotFourAbilityType = Enum.Parse(_abilityTypeType, "SLOT_4");

                _available = (_constructorWithPlayer != null || _constructorSimple != null) &&
                             _parametersAbilityTypeField != null && _parametersSkillTypesField != null &&
                             _parametersPropertiesField != null && _tornadoFxField != null &&
                             _directionField != null && _speedField != null &&
                             _executeMethod != null && _updateMethod != null;

                if (!_available)
                {
                    reason = "NorseDemigods tornado unavailable.";
                }

                return _available;
            }
            catch (Exception ex)
            {
                reason = "NorseDemigods tornado unavailable.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static object CreateAbility(Player player)
        {
            object parameters = Activator.CreateInstance(_abilityParametersType);
            Array skillTypes = Array.CreateInstance(typeof(Skills.SkillType), 1);
            skillTypes.SetValue(Skills.SkillType.ElementalMagic, 0);
            _parametersAbilityTypeField.SetValue(parameters, _slotFourAbilityType);
            _parametersSkillTypesField.SetValue(parameters, skillTypes);
            _parametersPropertiesField.SetValue(parameters, null);

            if (_constructorWithPlayer != null)
            {
                object demigod = _demigodType != null ? player.GetComponent(_demigodType) : null;
                return _constructorWithPlayer.Invoke(new[] { player, demigod, parameters });
            }

            return _constructorSimple.Invoke(new[] { parameters });
        }

        private static void SetupConfigs(object ability)
        {
            if (_setupConfigsMethod == null || ability == null)
            {
                return;
            }

            ConfigFile config = GetNorseConfig();
            if (config == null)
            {
                return;
            }

            _setupConfigsMethod.Invoke(ability, new object[] { config, "Ability Tornado" });
        }

        private static ConfigFile GetNorseConfig()
        {
            if (_norseConfig != null)
            {
                return _norseConfig;
            }

            try
            {
                string configPath = Path.Combine(Paths.ConfigPath, "NorseDemigods.cfg");
                if (File.Exists(configPath))
                {
                    _norseConfig = new ConfigFile(configPath, true);
                    return _norseConfig;
                }
            }
            catch
            {
            }

            return EpicLootRaritySetsPlugin.PluginConfig;
        }

        private static void NeutralizeNorseCosts(object ability)
        {
            if (_energyRequiredField != null)
            {
                _energyRequiredField.SetValue(ability, 0);
            }

            if (_energyCostField != null)
            {
                _energyCostField.SetValue(ability, 0);
            }

            if (_eitrCostField != null)
            {
                _eitrCostField.SetValue(ability, 0);
            }

            if (_cooldownField != null)
            {
                _cooldownField.SetValue(ability, 0f);
            }
        }

        private static void InvokeBridgeExecute(object ability)
        {
            _bridgeExecutionDepth++;
            try
            {
                _executeMethod.Invoke(ability, null);
            }
            finally
            {
                _bridgeExecutionDepth--;
            }
        }

        private static void PinTornadoAt(object ability, Vector3 position, out GameObject effectObject)
        {
            effectObject = null;
            if (ability == null)
            {
                return;
            }

            _directionField.SetValue(ability, Vector3.zero);
            _speedField.SetValue(ability, 0f);
            effectObject = _tornadoFxField.GetValue(ability) as GameObject;
            if (effectObject != null)
            {
                effectObject.transform.position = position + Vector3.up * GetTornadoVisualYOffset(ability);
                effectObject.transform.rotation = Quaternion.identity;
            }
        }

        private static void ForceTornadoLifetime(object ability)
        {
            if (ability == null || _lifeTimerField == null)
            {
                return;
            }

            try
            {
                float current = Convert.ToSingle(_lifeTimerField.GetValue(ability));
                float wanted = Mathf.Max(5f, EpicLootRaritySetsPlugin.MoonveinTornadoDuration.Value);
                if (current < wanted)
                {
                    _lifeTimerField.SetValue(ability, wanted);
                }
            }
            catch
            {
            }
        }

        private static float GetTornadoVisualYOffset(object ability)
        {
            if (ability != null && _tornadoHeightField != null)
            {
                try
                {
                    float height = Convert.ToSingle(_tornadoHeightField.GetValue(ability));
                    if (height > 0f)
                    {
                        return Mathf.Max(9f, height);
                    }
                }
                catch
                {
                }
            }

            return 9f;
        }

        private static void LogFailure(string reason, Exception ex)
        {
            if (_loggedFailure)
            {
                return;
            }

            _loggedFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(reason + " " + ex.GetBaseException().Message);
        }
    }

    internal static class NorseSneakyVisualBridge
    {
        private static bool _initialized;
        private static bool _available;
        private static bool _loggedFailure;
        private static Type _attributeSneakyType;
        private static Type _demigodType;
        private static ConstructorInfo _constructorWithPlayer;
        private static ConstructorInfo _constructorSimple;
        private static MethodInfo _setupConfigsMethod;
        private static MethodInfo _modifyBodyVisualMethod;
        private static MethodInfo _resetBodyVisualMethod;
        private static PropertyInfo _ownerProperty;
        private static FieldInfo _ownerField;
        private static ConfigFile _norseConfig;
        private static object _attribute;
        private static Player _owner;
        private static bool _applied;

        internal static bool Apply(Player player)
        {
            if (player == null)
            {
                return false;
            }

            string reason;
            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                if (_attribute == null || _owner != player)
                {
                    Reset();
                    _attribute = CreateAttribute(player);
                    _owner = player;
                    SetupConfigs(_attribute);
                }

                if (!_applied)
                {
                    _modifyBodyVisualMethod.Invoke(_attribute, null);
                    _applied = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogFailure("Norse Sneaky visual failed.", ex);
                return false;
            }
        }

        internal static void Reset()
        {
            if (_attribute == null || !_applied)
            {
                return;
            }

            try
            {
                _resetBodyVisualMethod.Invoke(_attribute, null);
            }
            catch
            {
            }
            finally
            {
                _applied = false;
            }
        }

        private static bool EnsureInitialized(out string reason)
        {
            reason = null;
            if (_initialized)
            {
                if (!_available)
                {
                    reason = "Norse Sneaky visual unavailable.";
                }

                return _available;
            }

            _initialized = true;
            try
            {
                _attributeSneakyType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Attributes.AttributeSneaky");
                _demigodType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Demigod");
                Type demigodAttributeType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAttribute");
                if (_attributeSneakyType == null || demigodAttributeType == null)
                {
                    reason = "Norse Sneaky visual unavailable.";
                    return false;
                }

                Type[] constructorTypes = { typeof(Player), _demigodType, typeof(object) };
                _constructorWithPlayer = _demigodType != null
                    ? _attributeSneakyType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, constructorTypes, null)
                    : null;
                _constructorSimple = _attributeSneakyType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _setupConfigsMethod = _attributeSneakyType.GetMethod("SetupConfigs", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(ConfigFile), typeof(string) }, null);
                _modifyBodyVisualMethod = _attributeSneakyType.GetMethod("ModifyBodyVisual", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _resetBodyVisualMethod = _attributeSneakyType.GetMethod("ResetBodyVisual", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _ownerProperty = demigodAttributeType.GetProperty("Owner", BindingFlags.Public | BindingFlags.Instance);
                _ownerField = demigodAttributeType.GetField("owner", BindingFlags.NonPublic | BindingFlags.Instance);

                _available = (_constructorWithPlayer != null || _constructorSimple != null) &&
                             _modifyBodyVisualMethod != null &&
                             _resetBodyVisualMethod != null;
                if (!_available)
                {
                    reason = "Norse Sneaky visual unavailable.";
                }

                return _available;
            }
            catch (Exception ex)
            {
                reason = "Norse Sneaky visual unavailable.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static object CreateAttribute(Player player)
        {
            object attribute;
            if (_constructorWithPlayer != null)
            {
                object demigod = _demigodType != null ? player.GetComponent(_demigodType) : null;
                attribute = _constructorWithPlayer.Invoke(new object[] { player, demigod, null });
            }
            else
            {
                attribute = _constructorSimple.Invoke(null);
                if (_ownerProperty != null && _ownerProperty.CanWrite)
                {
                    _ownerProperty.SetValue(attribute, player, null);
                }
                else if (_ownerField != null)
                {
                    _ownerField.SetValue(attribute, player);
                }
            }

            return attribute;
        }

        private static void SetupConfigs(object attribute)
        {
            if (_setupConfigsMethod == null || attribute == null)
            {
                return;
            }

            ConfigFile config = GetNorseConfig();
            if (config != null)
            {
                _setupConfigsMethod.Invoke(attribute, new object[] { config, "Attribute Sneaky" });
            }
        }

        private static ConfigFile GetNorseConfig()
        {
            if (_norseConfig != null)
            {
                return _norseConfig;
            }

            try
            {
                string configPath = Path.Combine(Paths.ConfigPath, "NorseDemigods.cfg");
                if (File.Exists(configPath))
                {
                    _norseConfig = new ConfigFile(configPath, true);
                    return _norseConfig;
                }
            }
            catch
            {
            }

            return EpicLootRaritySetsPlugin.PluginConfig;
        }

        private static void LogFailure(string reason, Exception ex)
        {
            if (_loggedFailure)
            {
                return;
            }

            _loggedFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(reason + " " + ex.GetBaseException().Message);
        }
    }

    internal static class NorseWarpBridge
    {
        private static bool _initialized;
        private static bool _available;
        private static bool _loggedFailure;
        private static int _bridgeExecutionDepth;
        private static Type _abilityWarpType;
        private static Type _abilityParametersType;
        private static Type _abilityTypeType;
        private static Type _demigodType;
        private static ConstructorInfo _constructorWithPlayer;
        private static ConstructorInfo _constructorSimple;
        private static FieldInfo _parametersAbilityTypeField;
        private static FieldInfo _parametersSkillTypesField;
        private static FieldInfo _parametersPropertiesField;
        private static FieldInfo _energyRequiredField;
        private static FieldInfo _energyCostField;
        private static FieldInfo _eitrCostField;
        private static FieldInfo _cooldownField;
        private static MethodInfo _setupConfigsMethod;
        private static MethodInfo _executeMethod;
        private static object _slotOneAbilityType;
        private static ConfigFile _norseConfig;

        internal static bool IsBridgeExecuting
        {
            get { return _bridgeExecutionDepth > 0; }
        }

        internal static bool TryWarp(Player player, out string reason)
        {
            reason = null;
            if (player == null)
            {
                reason = "Warp unavailable.";
                return false;
            }

            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                object ability = CreateAbility(player);
                SetupConfigs(ability);
                NeutralizeNorseCosts(ability);
                InvokeBridgeExecute(ability);
                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse Warp failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        internal static bool TrySafeWarp(Player player, float range, out string reason)
        {
            reason = null;
            if (player == null)
            {
                reason = "Warp unavailable.";
                return false;
            }

            Vector3 targetPoint;
            Vector3 lookDirection;
            if (!TryFindSafeWarpPoint(player, Mathf.Max(2f, range), out targetPoint, out lookDirection))
            {
                reason = "Warp: no se pudo calcular destino.";
                return false;
            }

            try
            {
                Vector3 from = player.transform.position;
                SpawnWarpEffect("FxTeleport1", from);
                player.StopMovement();
                Rigidbody body = player.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                    body.position = targetPoint;
                }

                player.transform.position = targetPoint;
                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    player.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(lookDirection, Vector3.up).normalized, Vector3.up);
                }

                SpawnWarpEffect("FxTeleport2", targetPoint);
                return true;
            }
            catch (Exception ex)
            {
                reason = "Warp failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static bool TryFindSafeWarpPoint(Player player, float range, out Vector3 targetPoint, out Vector3 lookDirection)
        {
            targetPoint = Vector3.zero;
            lookDirection = Vector3.zero;

            Character target;
            if (TryFindWarpTarget(player, range, out target))
            {
                if (TryFindPointBehindTarget(player, target, out targetPoint))
                {
                    lookDirection = target.GetCenterPoint() - targetPoint;
                    return true;
                }

                Vector3 behind = Vector3.ProjectOnPlane(-target.transform.forward, Vector3.up);
                if (behind.sqrMagnitude <= 0.001f)
                {
                    behind = Vector3.ProjectOnPlane(player.transform.position - target.transform.position, Vector3.up);
                }

                if (behind.sqrMagnitude <= 0.001f)
                {
                    behind = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up);
                }

                behind = behind.sqrMagnitude > 0.001f ? behind.normalized : Vector3.forward;
                TryProjectLenientGround(target.transform.position + behind * 2.25f, out targetPoint);
                lookDirection = target.GetCenterPoint() - targetPoint;
                return true;
            }

            Vector3 playerPosition = player.transform.position;
            Vector3 origin = player.GetCenterPoint();
            Vector3 direction = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up);
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = player.transform.forward;
            }

            direction.Normalize();
            float maxDistance = range;

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Character hitCharacter = hit.collider != null ? hit.collider.GetComponentInParent<Character>() : null;
                if (hitCharacter == null || hitCharacter != player)
                {
                    maxDistance = Mathf.Max(1.5f, hit.distance - 0.75f);
                }
            }

            for (float distance = maxDistance; distance >= 1.5f; distance -= 0.75f)
            {
                Vector3 probe = playerPosition + direction * distance;
                Vector3 safePoint;
                if (TryProjectSafeGround(probe, out safePoint))
                {
                    targetPoint = safePoint;
                    lookDirection = direction;
                    return true;
                }
            }

            TryProjectLenientGround(playerPosition + direction * Mathf.Max(1.5f, maxDistance), out targetPoint);
            lookDirection = direction;
            return true;
        }

        private static bool TryFindWarpTarget(Player player, float range, out Character target)
        {
            target = null;
            if (player == null)
            {
                return false;
            }

            Character aimedTarget;
            if (TryFindAimedEnemy(player, range, out aimedTarget))
            {
                target = aimedTarget;
                return true;
            }

            float bestDistance = range * range;
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character == player || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                float sqrDistance = (character.transform.position - player.transform.position).sqrMagnitude;
                if (sqrDistance > bestDistance)
                {
                    continue;
                }

                bestDistance = sqrDistance;
                target = character;
            }

            return target != null;
        }

        private static bool TryFindAimedEnemy(Player player, float range, out Character target)
        {
            target = null;
            Transform originTransform = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            RaycastHit[] hits = Physics.SphereCastAll(originTransform.position, 0.85f, originTransform.forward, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            if (hits == null || hits.Length == 0)
            {
                return false;
            }

            foreach (RaycastHit hit in hits.OrderBy(hit => hit.distance))
            {
                Character character = hit.collider != null ? hit.collider.GetComponentInParent<Character>() : null;
                if (character == null || character == player || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                target = character;
                return true;
            }

            return false;
        }

        private static bool TryFindPointBehindTarget(Player player, Character target, out Vector3 safePoint)
        {
            safePoint = Vector3.zero;
            if (player == null || target == null)
            {
                return false;
            }

            Vector3 behind = Vector3.ProjectOnPlane(-target.transform.forward, Vector3.up);
            if (behind.sqrMagnitude <= 0.001f)
            {
                behind = Vector3.ProjectOnPlane(player.transform.position - target.transform.position, Vector3.up);
            }

            if (behind.sqrMagnitude <= 0.001f)
            {
                behind = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up);
            }

            behind.Normalize();
            float[] distances = { 2.0f, 2.75f, 3.5f, 1.35f, 4.25f };
            float[] angles = { 0f, 20f, -20f, 40f, -40f, 65f, -65f, 90f, -90f };
            foreach (float distance in distances)
            {
                foreach (float angle in angles)
                {
                    Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * behind;
                    Vector3 probe = target.transform.position + direction * distance;
                    if (TryProjectSafeGround(probe, out safePoint))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryProjectSafeGround(Vector3 probe, out Vector3 safePoint)
        {
            safePoint = Vector3.zero;
            RaycastHit hit;
            Vector3 rayStart = probe + Vector3.up * 12f;
            if (!Physics.Raycast(rayStart, Vector3.down, out hit, 30f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            if (hit.collider == null ||
                hit.collider.GetComponentInParent<WaterVolume>() != null ||
                Vector3.Angle(hit.normal, Vector3.up) > 55f)
            {
                return false;
            }

            Vector3 candidate = hit.point + Vector3.up * 0.18f;
            if (IsWaterAt(candidate))
            {
                return false;
            }

            safePoint = candidate;
            return true;
        }

        private static bool TryProjectLenientGround(Vector3 probe, out Vector3 safePoint)
        {
            if (TryProjectSafeGround(probe, out safePoint))
            {
                return true;
            }

            RaycastHit hit;
            Vector3 rayStart = probe + Vector3.up * 18f;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 45f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) &&
                hit.collider != null &&
                hit.collider.GetComponentInParent<WaterVolume>() == null)
            {
                safePoint = hit.point + Vector3.up * 0.18f;
                return true;
            }

            safePoint = probe;
            return true;
        }

        private static void SpawnWarpEffect(string resourceName, Vector3 position)
        {
            GameObject prefab = GetNorseStaticGameObjectByName("NorseDemigods.Resources", resourceName) ??
                                GetNorseStaticGameObjectByName("NorseDemigods.Cache", resourceName);
            if (prefab == null)
            {
                return;
            }

            GameObject effect = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
            if (effect != null)
            {
                UnityEngine.Object.Destroy(effect, 5f);
            }
        }

        private static GameObject GetNorseStaticGameObjectByName(string typeName, string namePart)
        {
            Type type = NorseDemigodsSuppression.FindNorseType(typeName);
            if (type == null)
            {
                return null;
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field == null || field.FieldType != typeof(GameObject) ||
                    field.Name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                try
                {
                    GameObject value = field.GetValue(null) as GameObject;
                    if (value != null)
                    {
                        return value;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        private static bool HasWarpHeadroom(Vector3 point)
        {
            try
            {
                Vector3 bottom = point + Vector3.up * 0.65f;
                Vector3 top = point + Vector3.up * 1.9f;
                Collider[] colliders = Physics.OverlapCapsule(bottom, top, 0.28f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
                foreach (Collider collider in colliders)
                {
                    if (collider == null || collider.isTrigger)
                    {
                        continue;
                    }

                    if (collider.GetComponentInParent<Character>() != null ||
                        collider.GetComponentInParent<WaterVolume>() != null)
                    {
                        continue;
                    }

                    Vector3 closest = collider.ClosestPoint(point);
                    if (closest.y <= point.y + 0.25f)
                    {
                        continue;
                    }

                    return false;
                }
            }
            catch
            {
            }

            return true;
        }

        private static bool IsWaterAt(Vector3 point)
        {
            Collider[] colliders = Physics.OverlapSphere(point + Vector3.up * 0.25f, 0.6f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
            foreach (Collider collider in colliders)
            {
                if (collider != null && collider.GetComponentInParent<WaterVolume>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player)
            {
                return false;
            }

            MethodInfo isEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
            if (isEnemyMethod != null)
            {
                try
                {
                    object value = isEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        private static bool EnsureInitialized(out string reason)
        {
            reason = null;
            if (_initialized)
            {
                if (!_available)
                {
                    reason = "Norse Warp unavailable.";
                }

                return _available;
            }

            _initialized = true;
            try
            {
                _abilityWarpType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Abilities.AbilityWarp");
                _abilityParametersType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityParametersStruct");
                _abilityTypeType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityType");
                _demigodType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Demigod");
                Type demigodAbilityType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility");
                if (_abilityWarpType == null || _abilityParametersType == null || _abilityTypeType == null || demigodAbilityType == null)
                {
                    reason = "Norse Warp unavailable.";
                    return false;
                }

                Type[] constructorTypes = { typeof(Player), _demigodType, _abilityParametersType };
                _constructorWithPlayer = _demigodType != null
                    ? _abilityWarpType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, constructorTypes, null)
                    : null;
                _constructorSimple = _abilityWarpType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { _abilityParametersType }, null);
                _parametersAbilityTypeField = _abilityParametersType.GetField("AbilityType", BindingFlags.Public | BindingFlags.Instance);
                _parametersSkillTypesField = _abilityParametersType.GetField("SkillTypes", BindingFlags.Public | BindingFlags.Instance);
                _parametersPropertiesField = _abilityParametersType.GetField("Properties", BindingFlags.Public | BindingFlags.Instance);
                _energyRequiredField = demigodAbilityType.GetField("<EnergyRequired>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _energyCostField = demigodAbilityType.GetField("<EnergyCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _eitrCostField = demigodAbilityType.GetField("<EitrCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _cooldownField = demigodAbilityType.GetField("<Cooldown>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _setupConfigsMethod = _abilityWarpType.GetMethod("SetupConfigs", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(ConfigFile), typeof(string) }, null);
                _executeMethod = _abilityWarpType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _slotOneAbilityType = Enum.Parse(_abilityTypeType, "SLOT_1");

                _available = (_constructorWithPlayer != null || _constructorSimple != null) &&
                             _parametersAbilityTypeField != null &&
                             _parametersSkillTypesField != null &&
                             _parametersPropertiesField != null &&
                             _executeMethod != null;
                if (!_available)
                {
                    reason = "Norse Warp unavailable.";
                }

                return _available;
            }
            catch (Exception ex)
            {
                reason = "Norse Warp unavailable.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static object CreateAbility(Player player)
        {
            object parameters = Activator.CreateInstance(_abilityParametersType);
            Array skillTypes = Array.CreateInstance(typeof(Skills.SkillType), 1);
            skillTypes.SetValue(Skills.SkillType.Sneak, 0);
            _parametersAbilityTypeField.SetValue(parameters, _slotOneAbilityType);
            _parametersSkillTypesField.SetValue(parameters, skillTypes);
            _parametersPropertiesField.SetValue(parameters, null);

            if (_constructorWithPlayer != null)
            {
                object demigod = _demigodType != null ? player.GetComponent(_demigodType) : null;
                return _constructorWithPlayer.Invoke(new[] { player, demigod, parameters });
            }

            return _constructorSimple.Invoke(new[] { parameters });
        }

        private static void SetupConfigs(object ability)
        {
            if (_setupConfigsMethod == null || ability == null)
            {
                return;
            }

            ConfigFile config = GetNorseConfig();
            if (config != null)
            {
                _setupConfigsMethod.Invoke(ability, new object[] { config, "Ability Warp" });
            }
        }

        private static ConfigFile GetNorseConfig()
        {
            if (_norseConfig != null)
            {
                return _norseConfig;
            }

            try
            {
                string configPath = Path.Combine(Paths.ConfigPath, "NorseDemigods.cfg");
                if (File.Exists(configPath))
                {
                    _norseConfig = new ConfigFile(configPath, true);
                    return _norseConfig;
                }
            }
            catch
            {
            }

            return EpicLootRaritySetsPlugin.PluginConfig;
        }

        private static void NeutralizeNorseCosts(object ability)
        {
            if (_energyRequiredField != null)
            {
                _energyRequiredField.SetValue(ability, 0);
            }

            if (_energyCostField != null)
            {
                _energyCostField.SetValue(ability, 0);
            }

            if (_eitrCostField != null)
            {
                _eitrCostField.SetValue(ability, 0);
            }

            if (_cooldownField != null)
            {
                _cooldownField.SetValue(ability, 0f);
            }
        }

        private static void InvokeBridgeExecute(object ability)
        {
            _bridgeExecutionDepth++;
            try
            {
                _executeMethod.Invoke(ability, null);
            }
            finally
            {
                _bridgeExecutionDepth--;
            }
        }

        private static void LogFailure(string reason, Exception ex)
        {
            if (_loggedFailure)
            {
                return;
            }

            _loggedFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(reason + " " + ex.GetBaseException().Message);
        }
    }

    internal static class NorseSurtAbilityBridge
    {
        private static readonly List<object> ActiveCrushes = new List<object>();
        private static bool _initialized;
        private static bool _available;
        private static bool _loggedFailure;
        private static int _bridgeExecutionDepth;
        private static int _bridgeUpdateDepth;
        private static Type _abilitySlashType;
        private static Type _abilityCrushType;
        private static Type _abilityBurningGroundType;
        private static Type _abilityParametersType;
        private static Type _abilityTypeType;
        private static Type _demigodType;
        private static Type _demigodAbilityType;
        private static FieldInfo _parametersAbilityTypeField;
        private static FieldInfo _parametersSkillTypesField;
        private static FieldInfo _parametersPropertiesField;
        private static FieldInfo _energyRequiredField;
        private static FieldInfo _energyCostField;
        private static FieldInfo _eitrCostField;
        private static FieldInfo _cooldownField;
        private static FieldInfo _statusEffectTimeField = AccessTools.Field(typeof(StatusEffect), "m_time");
        private static PropertyInfo _crushActivatedProperty;
        private static MethodInfo _slashExecuteMethod;
        private static MethodInfo _crushExecuteMethod;
        private static MethodInfo _crushUpdateMethod;
        private static MethodInfo _burningGroundExecuteMethod;
        private static object _specialAttackAbilityType;
        private static object _specialAttackAirAbilityType;
        private static object _slotThreeAbilityType;
        private static ConfigFile _norseConfig;

        internal static bool IsBridgeExecuting
        {
            get { return _bridgeExecutionDepth > 0; }
        }

        internal static bool IsBridgeUpdating
        {
            get { return _bridgeUpdateDepth > 0; }
        }

        internal static void Update(float dt)
        {
            if (ActiveCrushes.Count == 0)
            {
                return;
            }

            string reason;
            if (!EnsureInitialized(out reason))
            {
                ActiveCrushes.Clear();
                return;
            }

            for (int i = ActiveCrushes.Count - 1; i >= 0; i--)
            {
                object ability = ActiveCrushes[i];
                if (ability == null)
                {
                    ActiveCrushes.RemoveAt(i);
                    continue;
                }

                try
                {
                    _bridgeUpdateDepth++;
                    try
                    {
                        _crushUpdateMethod.Invoke(ability, new object[] { dt });
                    }
                    finally
                    {
                        _bridgeUpdateDepth--;
                    }

                    if (!IsCrushActivated(ability))
                    {
                        ActiveCrushes.RemoveAt(i);
                    }
                }
                catch (Exception ex)
                {
                    ActiveCrushes.RemoveAt(i);
                    LogFailure("Norse Surt Crush update failed.", ex);
                }
            }
        }

        internal static bool TrySlash(Player player, out string reason)
        {
            reason = null;
            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                object ability = CreateAbility(_abilitySlashType, "FIRE", _specialAttackAbilityType);
                SetupConfigs(ability, _abilitySlashType, "Ability Slash");
                NeutralizeNorseCosts(ability);
                InvokeExecute(_slashExecuteMethod, ability);
                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse Surt Slash failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        internal static bool TryCrush(Player player, out string reason)
        {
            reason = null;
            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                object ability = CreateAbility(_abilityCrushType, "FIRE", _specialAttackAirAbilityType);
                SetupConfigs(ability, _abilityCrushType, "Ability Crush");
                NeutralizeNorseCosts(ability);
                InvokeExecute(_crushExecuteMethod, ability);
                ActiveCrushes.Add(ability);
                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse Surt Crush failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        internal static bool TryBurningGround(Player player, out string reason)
        {
            reason = null;
            if (!EnsureInitialized(out reason))
            {
                return false;
            }

            try
            {
                object ability = CreateAbility(_abilityBurningGroundType, null, _slotThreeAbilityType);
                SetupConfigs(ability, _abilityBurningGroundType, "Ability Burning Ground");
                SetConfigEntryValue(_abilityBurningGroundType, "AbilityDuration", Mathf.RoundToInt(Mathf.Max(1f, EpicLootRaritySetsPlugin.FrostbrandBurningGroundDuration.Value)));
                NeutralizeNorseCosts(ability);
                InvokeExecute(_burningGroundExecuteMethod, ability);
                return true;
            }
            catch (Exception ex)
            {
                reason = "Norse Surt Burning Ground failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static bool EnsureInitialized(out string reason)
        {
            reason = null;
            if (_initialized)
            {
                if (!_available)
                {
                    reason = "Norse Surt abilities unavailable.";
                }

                return _available;
            }

            _initialized = true;
            try
            {
                _abilitySlashType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Abilities.AbilitySlash");
                _abilityCrushType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Abilities.AbilityCrush");
                _abilityBurningGroundType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Abilities.AbilityBurningGround");
                _abilityParametersType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityParametersStruct");
                _abilityTypeType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility+AbilityType");
                _demigodType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Demigod");
                _demigodAbilityType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility");

                if (_abilitySlashType == null || _abilityCrushType == null || _abilityBurningGroundType == null ||
                    _abilityParametersType == null || _abilityTypeType == null || _demigodAbilityType == null)
                {
                    reason = "Norse Surt abilities unavailable.";
                    return false;
                }

                _parametersAbilityTypeField = _abilityParametersType.GetField("AbilityType", BindingFlags.Public | BindingFlags.Instance);
                _parametersSkillTypesField = _abilityParametersType.GetField("SkillTypes", BindingFlags.Public | BindingFlags.Instance);
                _parametersPropertiesField = _abilityParametersType.GetField("Properties", BindingFlags.Public | BindingFlags.Instance);
                _energyRequiredField = _demigodAbilityType.GetField("<EnergyRequired>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _energyCostField = _demigodAbilityType.GetField("<EnergyCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _eitrCostField = _demigodAbilityType.GetField("<EitrCost>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _cooldownField = _demigodAbilityType.GetField("<Cooldown>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                _crushActivatedProperty = _abilityCrushType.GetProperty("Activated", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _slashExecuteMethod = _abilitySlashType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _crushExecuteMethod = _abilityCrushType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _crushUpdateMethod = _abilityCrushType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
                _burningGroundExecuteMethod = _abilityBurningGroundType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                _specialAttackAbilityType = Enum.Parse(_abilityTypeType, "SPECIAL_ATTACK");
                _specialAttackAirAbilityType = Enum.Parse(_abilityTypeType, "SPECIAL_ATTACK_AIR");
                _slotThreeAbilityType = Enum.Parse(_abilityTypeType, "SLOT_3");

                _available = _parametersAbilityTypeField != null && _parametersSkillTypesField != null &&
                             _parametersPropertiesField != null && _energyRequiredField != null &&
                             _energyCostField != null && _eitrCostField != null && _cooldownField != null &&
                             _slashExecuteMethod != null && _crushExecuteMethod != null &&
                             _crushUpdateMethod != null && _burningGroundExecuteMethod != null;

                if (!_available)
                {
                    reason = "Norse Surt abilities unavailable.";
                }

                return _available;
            }
            catch (Exception ex)
            {
                reason = "Norse Surt abilities unavailable.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static object CreateAbility(Type abilityType, string themeName, object abilitySlot)
        {
            object parameters = Activator.CreateInstance(_abilityParametersType);
            Array skillTypes = Array.CreateInstance(typeof(Skills.SkillType), 1);
            skillTypes.SetValue(Skills.SkillType.ElementalMagic, 0);
            _parametersAbilityTypeField.SetValue(parameters, abilitySlot);
            _parametersSkillTypesField.SetValue(parameters, skillTypes);
            _parametersPropertiesField.SetValue(parameters, CreateAbilityProperties(abilityType, themeName));

            ConstructorInfo constructor = _demigodType != null
                ? abilityType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Player), _demigodType, _abilityParametersType }, null)
                : null;
            if (constructor != null)
            {
                object demigod = _demigodType != null ? Player.m_localPlayer.GetComponent(_demigodType) : null;
                return constructor.Invoke(new[] { Player.m_localPlayer, demigod, parameters });
            }

            constructor = abilityType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { _abilityParametersType }, null);
            return constructor.Invoke(new[] { parameters });
        }

        private static object CreateAbilityProperties(Type abilityType, string themeName)
        {
            if (abilityType == null || string.IsNullOrEmpty(themeName))
            {
                return null;
            }

            Type propertyType = abilityType.GetNestedType("AbilityProperty", BindingFlags.Public | BindingFlags.NonPublic);
            Type themeType = abilityType.GetNestedType("AbilityTheme", BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyType == null || themeType == null)
            {
                return null;
            }

            object properties = Activator.CreateInstance(propertyType);
            FieldInfo themeField = propertyType.GetField("AbilityTheme", BindingFlags.Public | BindingFlags.Instance);
            if (themeField != null)
            {
                themeField.SetValue(properties, Enum.Parse(themeType, themeName));
            }

            return properties;
        }

        private static void SetupConfigs(Type abilityType, string category)
        {
            MethodInfo setup = abilityType != null
                ? abilityType.GetMethod("SetupConfigs", BindingFlags.NonPublic | BindingFlags.Static)
                : null;
            if (setup != null)
            {
                setup.Invoke(null, new object[] { GetNorseConfig(), category });
            }
        }

        private static void SetupConfigs(object ability, Type abilityType, string category)
        {
            MethodInfo setup = ability != null
                ? ability.GetType().GetMethod("SetupConfigs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ConfigFile), typeof(string) }, null)
                : null;
            if (setup != null)
            {
                setup.Invoke(ability, new object[] { GetNorseConfig(), category });
                return;
            }

            SetupConfigs(abilityType, category);
        }

        private static void SetConfigEntryValue(Type abilityType, string fieldName, object value)
        {
            try
            {
                FieldInfo field = abilityType != null ? abilityType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static) : null;
                object entry = field != null ? field.GetValue(null) : null;
                PropertyInfo valueProperty = entry != null ? entry.GetType().GetProperty("Value") : null;
                if (valueProperty != null && valueProperty.CanWrite)
                {
                    object converted = Convert.ChangeType(value, valueProperty.PropertyType);
                    valueProperty.SetValue(entry, converted, null);
                }
            }
            catch
            {
            }
        }

        private static ConfigFile GetNorseConfig()
        {
            if (_norseConfig != null)
            {
                return _norseConfig;
            }

            try
            {
                string configPath = Path.Combine(Paths.ConfigPath, "NorseDemigods.cfg");
                if (File.Exists(configPath))
                {
                    _norseConfig = new ConfigFile(configPath, true);
                    return _norseConfig;
                }
            }
            catch
            {
            }

            return EpicLootRaritySetsPlugin.PluginConfig;
        }

        private static void NeutralizeNorseCosts(object ability)
        {
            _energyRequiredField.SetValue(ability, 0);
            _energyCostField.SetValue(ability, 0);
            _eitrCostField.SetValue(ability, 0);
            _cooldownField.SetValue(ability, 0f);
        }

        private static void InvokeExecute(MethodInfo method, object ability)
        {
            _bridgeExecutionDepth++;
            try
            {
                method.Invoke(ability, null);
            }
            finally
            {
                _bridgeExecutionDepth--;
            }
        }

        private static bool IsCrushActivated(object ability)
        {
            if (_crushActivatedProperty == null || ability == null)
            {
                return false;
            }

            object value = _crushActivatedProperty.GetValue(ability, null);
            return value is bool && (bool)value;
        }

        private static void LogFailure(string reason, Exception ex)
        {
            if (_loggedFailure)
            {
                return;
            }

            _loggedFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(reason + " " + ex.GetBaseException().Message);
        }
    }

    internal static class NorseWhirlwindBridge
    {
        private static bool _loggedFailure;

        internal static bool TrySpawn(Vector3 position, out GameObject effectObject, out string reason)
        {
            effectObject = null;
            reason = null;

            GameObject prefab = GetNorseStaticGameObjectByName("NorseDemigods.Resources", "WindWhirlwind") ??
                                GetNorseStaticGameObjectByName("NorseDemigods.Cache", "WindWhirlwind") ??
                                GetNorseStaticGameObjectByName("NorseDemigods.Resources", "Whirlwind") ??
                                GetNorseStaticGameObjectByName("NorseDemigods.Cache", "Whirlwind");
            if (prefab == null)
            {
                reason = "Norse Ability Whirlwind visual unavailable.";
                return false;
            }

            try
            {
                effectObject = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
                return effectObject != null;
            }
            catch (Exception ex)
            {
                reason = "Norse Ability Whirlwind visual failed.";
                LogFailure(reason, ex);
                return false;
            }
        }

        private static GameObject GetNorseStaticGameObjectByName(string typeName, string namePart)
        {
            Type type = NorseDemigodsSuppression.FindNorseType(typeName);
            if (type == null)
            {
                return null;
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field == null || field.FieldType != typeof(GameObject) ||
                    field.Name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                try
                {
                    GameObject value = field.GetValue(null) as GameObject;
                    if (value != null)
                    {
                        return value;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        private static void LogFailure(string reason, Exception ex)
        {
            if (_loggedFailure)
            {
                return;
            }

            _loggedFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(reason + " " + ex.GetBaseException().Message);
        }
    }

    internal static class NorseVisualEffectBridge
    {
        private static bool _loggedFailure;

        internal static void SpawnAtPlayer(Player player, params string[] nameParts)
        {
            if (player == null)
            {
                return;
            }

            Spawn(player.GetCenterPoint(), player.transform.rotation, nameParts);
        }

        internal static void Spawn(Vector3 position, Quaternion rotation, params string[] nameParts)
        {
            Spawn(position, rotation, 8f, nameParts);
        }

        internal static GameObject Spawn(Vector3 position, Quaternion rotation, float lifetime, params string[] nameParts)
        {
            GameObject prefab = FindPrefab(nameParts);
            if (prefab == null)
            {
                return null;
            }

            try
            {
                GameObject effect = UnityEngine.Object.Instantiate(prefab, position, rotation);
                if (effect != null)
                {
                    UnityEngine.Object.Destroy(effect, Mathf.Max(0.1f, lifetime));
                }

                return effect;
            }
            catch (Exception ex)
            {
                if (!_loggedFailure)
                {
                    _loggedFailure = true;
                    EpicLootRaritySetsPlugin.Log.LogWarning("Norse visual effect spawn failed. " + ex.GetBaseException().Message);
                }

                return null;
            }
        }

        internal static GameObject SpawnAttached(Player player, Vector3 localPosition, Quaternion localRotation, float lifetime, params string[] nameParts)
        {
            if (player == null)
            {
                return null;
            }

            return SpawnAttached(player.gameObject, localPosition, localRotation, lifetime, nameParts);
        }

        internal static GameObject SpawnAttached(GameObject parent, Vector3 localPosition, Quaternion localRotation, float lifetime, params string[] nameParts)
        {
            if (parent == null)
            {
                return null;
            }

            GameObject prefab = FindPrefab(nameParts);
            if (prefab == null)
            {
                return null;
            }

            try
            {
                GameObject effect = UnityEngine.Object.Instantiate(prefab, parent.transform);
                if (effect != null)
                {
                    effect.transform.localPosition = localPosition;
                    effect.transform.localRotation = localRotation;
                    UnityEngine.Object.Destroy(effect, Mathf.Max(0.1f, lifetime));
                }

                return effect;
            }
            catch (Exception ex)
            {
                if (!_loggedFailure)
                {
                    _loggedFailure = true;
                    EpicLootRaritySetsPlugin.Log.LogWarning("Norse attached visual effect spawn failed. " + ex.GetBaseException().Message);
                }

                return null;
            }
        }

        private static GameObject FindPrefab(params string[] nameParts)
        {
            if (nameParts == null || nameParts.Length == 0)
            {
                return null;
            }

            foreach (string namePart in nameParts)
            {
                GameObject prefab = FindNorseStaticGameObject(namePart);
                if (prefab != null)
                {
                    return prefab;
                }
            }

            if (ZNetScene.instance != null)
            {
                foreach (string namePart in nameParts)
                {
                    GameObject prefab = ZNetScene.instance.GetPrefab(namePart);
                    if (prefab != null)
                    {
                        return prefab;
                    }
                }
            }

            return null;
        }

        private static GameObject FindNorseStaticGameObject(string namePart)
        {
            if (string.IsNullOrEmpty(namePart))
            {
                return null;
            }

            foreach (string typeName in new[] { "NorseDemigods.Resources", "NorseDemigods.Cache" })
            {
                Type type = NorseDemigodsSuppression.FindNorseType(typeName);
                if (type == null)
                {
                    continue;
                }

                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (field == null || field.FieldType != typeof(GameObject) ||
                        field.Name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    try
                    {
                        GameObject value = field.GetValue(null) as GameObject;
                        if (value != null)
                        {
                            return value;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return null;
        }
    }

    internal static class FrostbrandAbilityController
    {
        private const string RequiredSet = "Frostbrand";
        private const string ElementalShieldBuffName = "FranFrostbrandElementalShield";
        private const string ElementalShieldBuffCategory = "FranFrostbrandElementalShield";
        private const string LightningStackBuffName = "FranFrostbrandLightningCharge";
        private const string LightningStackBuffCategory = "FranFrostbrandLightningCharge";

        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly MethodInfo ApplyMagicDamageModifiersMethod = AccessTools.Method(typeof(ModifyDamage), "ApplyMagicDamageModifiers");
        private static readonly MethodInfo PlayerStartAttackMethod = AccessTools
            .GetDeclaredMethods(typeof(Player))
            .Concat(AccessTools.GetDeclaredMethods(typeof(Humanoid)))
            .FirstOrDefault(method => method.Name == "StartAttack" && method.GetParameters().Any(parameter => parameter.ParameterType == typeof(bool)));
        private static readonly MethodInfo PlayerInAttackMethod =
            AccessTools.Method(typeof(Player), "InAttack", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Humanoid), "InAttack", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "InAttack", Type.EmptyTypes);
        private static readonly MethodInfo PlayerIsOnGroundMethod =
            AccessTools.Method(typeof(Character), "IsOnGround", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Player), "IsOnGround", Type.EmptyTypes);
        private static readonly FieldInfo StatusEffectTimeField = AccessTools.Field(typeof(StatusEffect), "m_time");

        private static readonly List<ActiveBurningGround> BurningGrounds = new List<ActiveBurningGround>();
        private static StatusEffect _elementalShieldBuff;
        private static StatusEffect _lightningStackBuff;
        private static GameObject _elementalShieldVisual;
        private static float _dashCooldown;
        private static float _waterSphereCooldown;
        private static float _slashCooldown;
        private static float _crushCooldown;
        private static float _elementalShieldCooldown;
        private static float _elementalShieldRemaining;
        private static float _burningGroundTickTimer;
        private static float _lightningStackRefreshTimer;
        private static int _lightningStrikeHits;
        private static int _abilityDamageDepth;
        private static bool _lightningStrikeExecuting;
        private static bool _pendingCrushActive;
        private static int _secondarySlashConsumedFrame = -1;
        private static int _slashAnimationDepth;
        private static Vector3 _pendingCrushPoint;
        private static Vector3 _pendingCrushStartPoint;
        private static ItemDrop.ItemData _pendingCrushWeapon;
        private static float _pendingCrushTimer;
        private static float _pendingCrushAirTimer;
        private static bool _pendingCrushLeftGround;
        private static bool _pendingCrushWasDescending;
        private static Vector3 _pendingCrushLastPosition;

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            NorseWaterSphereBridge.Update(dt);
            NorseSurtAbilityBridge.Update(dt);
            UpdateBurningGrounds(player, dt);
            UpdatePendingCrush(player, dt);

            _dashCooldown = Mathf.Max(0f, _dashCooldown - dt);
            _waterSphereCooldown = Mathf.Max(0f, _waterSphereCooldown - dt);
            _slashCooldown = Mathf.Max(0f, _slashCooldown - dt);
            _crushCooldown = Mathf.Max(0f, _crushCooldown - dt);
            _elementalShieldCooldown = Mathf.Max(0f, _elementalShieldCooldown - dt);
            UpdateElementalShield(player, dt);

            if (EpicLootRaritySetsPlugin.EnableFrostbrandAbilities != null && !EpicLootRaritySetsPlugin.EnableFrostbrandAbilities.Value)
            {
                RemoveElementalShield(player);
                return;
            }

            if (!SetActivationBuffController.HasActiveSet(RequiredSet))
            {
                RemoveElementalShield(player);
                RemoveLightningStackBuff(player);
                return;
            }

            UpdateLightningStackBuff(player, dt);
            if (!CanReadAbilityInput(player))
            {
                return;
            }

            if (SetAbilityInput.IsShortcutDown(EpicLootRaritySetsPlugin.FrostbrandWaterSphereHotkey))
            {
                TryWaterSphere(player);
                return;
            }

            if ((SetAbilityInput.IsSecondaryAttackDown() && _secondarySlashConsumedFrame != Time.frameCount) ||
                SetAbilityInput.IsShortcutDown(EpicLootRaritySetsPlugin.FrostbrandSlashHotkey))
            {
                TrySlash(player);
                return;
            }

            if (SetAbilityInput.IsShortcutDown(EpicLootRaritySetsPlugin.FrostbrandElementalShieldHotkey) &&
                SetAbilityInput.IsBlockHeld())
            {
                TryElementalShield(player);
                return;
            }

            if (SetAbilityInput.IsShortcutDown(EpicLootRaritySetsPlugin.FrostbrandCrushHotkey))
            {
                TryCrush(player);
            }
        }

        internal static void Clear(Player player)
        {
            RemoveElementalShield(player);
            RemoveLightningStackBuff(player);
            DestroyVisual(ref _elementalShieldVisual);
            BurningGrounds.Clear();
            _dashCooldown = 0f;
            _waterSphereCooldown = 0f;
            _slashCooldown = 0f;
            _crushCooldown = 0f;
            _elementalShieldCooldown = 0f;
            _elementalShieldRemaining = 0f;
            _lightningStackRefreshTimer = 0f;
            _secondarySlashConsumedFrame = -1;
            _slashAnimationDepth = 0;
            _pendingCrushActive = false;
            _pendingCrushLeftGround = false;
            _pendingCrushWasDescending = false;
            _pendingCrushAirTimer = 0f;
        }

        internal static void OnPlayerHit(Character target, HitData hit)
        {
        }

        internal static void OnWeaponAttackStarted(Player player, ItemDrop.ItemData weapon)
        {
            if (player == null || player != Player.m_localPlayer || !IsFrostbrandEnabledAndActive() || !IsFrostbrandWeapon(weapon))
            {
                return;
            }

            if (_lightningStrikeExecuting || _abilityDamageDepth > 0 ||
                _slashAnimationDepth > 0 ||
                NorseSurtAbilityBridge.IsBridgeExecuting || NorseSurtAbilityBridge.IsBridgeUpdating)
            {
                return;
            }

            int requiredHits = Mathf.Max(1, EpicLootRaritySetsPlugin.FrostbrandLightningStrikeAttackCount.Value);
            _lightningStrikeHits = Mathf.Clamp(_lightningStrikeHits + 1, 0, requiredHits);
            if (_lightningStrikeHits < requiredHits)
            {
                RefreshLightningStackBuff(player);
                return;
            }

            _lightningStrikeHits = 0;
            Vector3 strikePoint;
            Vector3 strikeDirection;
            if (!TryFindProjectileAim(player, Mathf.Max(12f, EpicLootRaritySetsPlugin.FrostbrandLightningStrikeRadius.Value + 20f), out strikePoint, out strikeDirection))
            {
                strikePoint = player.GetCenterPoint() + player.transform.forward * 5f;
                strikeDirection = player.GetAimDir(player.GetCenterPoint());
            }

            CastFrostbrandFireBall(player, weapon, strikePoint, strikeDirection);
            RefreshLightningStackBuff(player);
        }

        internal static bool TryConsumeSecondarySlash(Player player)
        {
            if (player == null || player != Player.m_localPlayer ||
                _slashAnimationDepth > 0 ||
                NorseSurtAbilityBridge.IsBridgeExecuting || NorseSurtAbilityBridge.IsBridgeUpdating ||
                !IsFrostbrandEnabledAndActive() || !IsFrostbrandWeapon(player.GetCurrentWeapon()))
            {
                return false;
            }

            _secondarySlashConsumedFrame = Time.frameCount;
            TrySlash(player);
            return true;
        }

        private static bool CanReadAbilityInput(Player player)
        {
            return !player.IsDead() &&
                   !player.IsTeleporting() &&
                   !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> shortcutEntry)
        {
            if (shortcutEntry == null)
            {
                return false;
            }

            KeyboardShortcut shortcut = shortcutEntry.Value;
            if (shortcut.IsDown())
            {
                return true;
            }

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAnyMenuOpen()
        {
            if (InventoryGui.instance != null && InventoryGui.IsVisible())
            {
                return true;
            }

            if (Menu.instance != null && Menu.IsVisible())
            {
                return true;
            }

            if (TextInput.instance != null && TextInput.IsVisible())
            {
                return true;
            }

            if (Chat.instance != null && Chat.instance.HasFocus())
            {
                return true;
            }

            return Minimap.instance != null && Minimap.IsOpen();
        }

        private static void TryDash(Player player)
        {
            if (!TrySpendEitrAndCooldown(player, "Dash", EpicLootRaritySetsPlugin.FrostbrandDashEitrUse.Value, _dashCooldown))
            {
                return;
            }

            string failureReason;
            if (!NorseDashBridge.TryDash(player, out failureReason))
            {
                ShowMessage(player, failureReason);
                return;
            }

            player.UseEitr(EpicLootRaritySetsPlugin.FrostbrandDashEitrUse.Value);
            _dashCooldown = EpicLootRaritySetsPlugin.FrostbrandDashCooldown.Value;
            AbilityCooldownBuffController.Start(player, "FrostbrandDash", "Dash", _dashCooldown, null);
        }

        private static void TryWaterSphere(Player player)
        {
            if (!TrySpendEitrAndCooldown(player, "Water Sphere", EpicLootRaritySetsPlugin.FrostbrandWaterSphereEitrUse.Value, _waterSphereCooldown))
            {
                return;
            }

            string failureReason;
            if (!NorseWaterSphereBridge.TryWaterSphere(player, out failureReason))
            {
                ShowMessage(player, failureReason);
                return;
            }

            player.UseEitr(EpicLootRaritySetsPlugin.FrostbrandWaterSphereEitrUse.Value);
            _waterSphereCooldown = EpicLootRaritySetsPlugin.FrostbrandWaterSphereCooldown.Value;
            AbilityCooldownBuffController.Start(player, "FrostbrandWaterSphere", "Water Sphere", _waterSphereCooldown, null);
        }

        private static void TrySlash(Player player)
        {
            if (!TrySpendEitrAndCooldown(player, "Slash", EpicLootRaritySetsPlugin.FrostbrandSlashEitrUse.Value, _slashCooldown))
            {
                return;
            }

            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            player.UseEitr(EpicLootRaritySetsPlugin.FrostbrandSlashEitrUse.Value);
            TriggerSlashAnimation(player);
            string failureReason;
            if (!NorseSurtAbilityBridge.TrySlash(player, out failureReason))
            {
                NorseVisualEffectBridge.SpawnAtPlayer(player, "FxFireSlash", "FireSlash", "Slash", "Surt", "FxFire", "Fire");
                WithAbilityDamage(() => DamageCone(player, weapon, EpicLootRaritySetsPlugin.FrostbrandSlashRange.Value, EpicLootRaritySetsPlugin.FrostbrandSlashRadius.Value, CreateSlashDamage(player, weapon), EpicLootRaritySetsPlugin.FrostbrandSlashBaseDamage.Value));
            }

            _slashCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.FrostbrandSlashCooldown.Value);
            AbilityCooldownBuffController.Start(player, "FrostbrandSlash", "Slash", _slashCooldown, null);
            ShowMessage(player, "Slash.");
        }

        private static void TriggerSlashAnimation(Player player)
        {
            _slashAnimationDepth++;
            try
            {
                TriggerSecondaryAttackIfNeeded(player, "knife_stab0", 0.5f, 0f);
            }
            finally
            {
                _slashAnimationDepth--;
            }
        }

        private static void TriggerCrushAnimation(Player player)
        {
            _slashAnimationDepth++;
            try
            {
                TriggerSecondaryAttackIfNeeded(player, "knife_secondary", null, 0f);
            }
            finally
            {
                _slashAnimationDepth--;
            }
        }

        private static void TryCrush(Player player)
        {
            if (!TrySpendEitrAndCooldown(player, "Crush", EpicLootRaritySetsPlugin.FrostbrandCrushEitrUse.Value, _crushCooldown))
            {
                return;
            }

            Vector3 targetPoint;
            if (!TryFindAimPoint(player, EpicLootRaritySetsPlugin.FrostbrandCrushRange.Value, out targetPoint))
            {
                ShowMessage(player, "Crush: sin punto.");
                return;
            }

            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            player.UseEitr(EpicLootRaritySetsPlugin.FrostbrandCrushEitrUse.Value);
            TriggerCrushAnimation(player);
            LaunchPlayerToward(player, targetPoint);

            _pendingCrushActive = true;
            _pendingCrushPoint = targetPoint;
            _pendingCrushStartPoint = player.transform.position;
            _pendingCrushWeapon = weapon;
            _pendingCrushTimer = 1.15f;
            _pendingCrushAirTimer = 0f;
            _pendingCrushLeftGround = false;
            _pendingCrushWasDescending = false;
            _pendingCrushLastPosition = player.transform.position;
            _crushCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.FrostbrandCrushCooldown.Value);
            AbilityCooldownBuffController.Start(player, "FrostbrandCrush", "Crush", _crushCooldown, null);
            ShowMessage(player, "Crush.");
        }

        private static void TryElementalShield(Player player)
        {
            if (_elementalShieldRemaining > 0f)
            {
                ShowMessage(player, "Elemental Shield: activo.");
                return;
            }

            if (!TrySpendEitrAndCooldown(player, "Elemental Shield", EpicLootRaritySetsPlugin.FrostbrandElementalShieldEitrUse.Value, _elementalShieldCooldown))
            {
                return;
            }

            player.UseEitr(EpicLootRaritySetsPlugin.FrostbrandElementalShieldEitrUse.Value);
            _elementalShieldRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.FrostbrandElementalShieldDuration.Value);
            _elementalShieldCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.FrostbrandElementalShieldCooldown.Value);
            EnsureElementalShieldVisual(player);
            RefreshElementalShieldBuff(player);
            AbilityCooldownBuffController.Start(player, "FrostbrandElementalShield", "Elemental Shield", _elementalShieldCooldown, null);
            ShowMessage(player, "Elemental Shield.");
        }

        internal static void ModifyIncomingDamage(Player player, HitData hit)
        {
            if (player == null || hit == null || player != Player.m_localPlayer || _elementalShieldRemaining <= 0f || !IsFrostbrandEnabledAndActive())
            {
                return;
            }

            float mitigation = GetElementalShieldMitigation(player);
            hit.m_damage.m_fire = 0f;
            ScaleNonFireDamage(ref hit.m_damage, Mathf.Clamp01(1f - mitigation));
        }

        private static void CastFrostbrandFireBall(Player player, ItemDrop.ItemData weapon, Vector3 center, Vector3 aimDirection)
        {
            float radius = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.FrostbrandLightningStrikeRadius.Value);
            HitData.DamageTypes damages = CreateFrostbrandFireBallDamage(player, weapon);

            _lightningStrikeExecuting = true;
            try
            {
                if (TryLaunchFrostbrandFireBallProjectile(player, weapon, center, aimDirection, damages))
                {
                    return;
                }

                foreach (Character character in Character.GetAllCharacters())
                {
                    if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                    {
                        continue;
                    }

                    if ((character.GetCenterPoint() - center).sqrMagnitude > radius * radius)
                    {
                        continue;
                    }

                    HitData strikeHit = new HitData
                    {
                        m_damage = damages,
                        m_skill = Skills.SkillType.ElementalMagic,
                        m_skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.ElementalMagic) : 0f,
                        m_ranged = true,
                        m_dodgeable = true,
                        m_blockable = true,
                        m_backstabBonus = 1f,
                        m_staggerMultiplier = 1f,
                        m_pushForce = EpicLootRaritySetsPlugin.FrostbrandLightningStrikeImpactForce.Value,
                        m_point = character.GetCenterPoint(),
                        m_dir = character.GetCenterPoint() - center
                    };

                    if (player != null)
                    {
                        strikeHit.SetAttacker(player);
                    }

                    character.Damage(strikeHit);
                }
            }
            finally
            {
                _lightningStrikeExecuting = false;
            }

            if (player != null)
            {
                NorseVisualEffectBridge.Spawn(center, Quaternion.identity, "FxFireExplosion2", "FxFireExplosion", "FxFireExpansion", "FxFire");
                ShowMessage(player, "Fire Ball.");
            }
        }

        private static bool TryLaunchFrostbrandFireBallProjectile(Player player, ItemDrop.ItemData weapon, Vector3 targetPoint, Vector3 aimDirection, HitData.DamageTypes damages)
        {
            if (player == null)
            {
                return false;
            }

            GameObject projectilePrefab = GetFrostbrandFireBallProjectilePrefab();
            if (projectilePrefab == null)
            {
                return false;
            }

            Vector3 spawnPoint = player.GetCenterPoint() + player.transform.forward * 0.8f + Vector3.up * 0.15f;
            Vector3 direction = aimDirection;
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = targetPoint - spawnPoint;
            }

            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = player.GetAimDir(spawnPoint);
            }

            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = player.transform.forward;
            }

            direction.Normalize();
            GameObject projectileObject = UnityEngine.Object.Instantiate(projectilePrefab, spawnPoint, Quaternion.LookRotation(direction));
            IProjectile projectile = projectileObject.GetComponent<IProjectile>();
            if (projectile == null)
            {
                projectile = projectileObject.GetComponentInChildren<IProjectile>();
            }

            if (projectile == null)
            {
                UnityEngine.Object.Destroy(projectileObject);
                return false;
            }

            HitData hit = CreateProjectileHit(player, weapon, damages, targetPoint, direction);
            float velocity = Mathf.Max(10f, EpicLootRaritySetsPlugin.FrostbrandLightningStrikeImpactForce.Value);
            projectile.Setup(player, direction * velocity, 0f, hit, weapon, null);
            ShowMessage(player, "Fire Ball.");
            return true;
        }

        private static GameObject GetFrostbrandFireBallProjectilePrefab()
        {
            GameObject fireball = GetAttackProjectileFromItem("StaffFireball");
            return fireball != null ? fireball : GetPrefab("fireball_projectile", "StaffFireball_projectile", "projectile_fireball");
        }

        private static GameObject GetAttackProjectileFromItem(string itemPrefabName)
        {
            GameObject itemPrefab = null;
            if (ObjectDB.instance != null)
            {
                itemPrefab = ObjectDB.instance.GetItemPrefab(itemPrefabName);
            }

            if (itemPrefab == null)
            {
                itemPrefab = GetPrefab(itemPrefabName);
            }

            ItemDrop itemDrop = itemPrefab != null ? itemPrefab.GetComponent<ItemDrop>() : null;
            if (itemDrop == null || itemDrop.m_itemData == null || itemDrop.m_itemData.m_shared == null || itemDrop.m_itemData.m_shared.m_attack == null)
            {
                return null;
            }

            return itemDrop.m_itemData.m_shared.m_attack.m_attackProjectile;
        }

        private static GameObject GetPrefab(params string[] names)
        {
            if (names == null || ZNetScene.instance == null)
            {
                return null;
            }

            foreach (string name in names)
            {
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                GameObject prefab = ZNetScene.instance.GetPrefab(name);
                if (prefab != null)
                {
                    return prefab;
                }
            }

            return null;
        }

        private static HitData CreateProjectileHit(Player player, ItemDrop.ItemData weapon, HitData.DamageTypes damages, Vector3 point, Vector3 direction)
        {
            HitData hit = new HitData
            {
                m_damage = damages,
                m_skill = Skills.SkillType.ElementalMagic,
                m_skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.ElementalMagic) : 0f,
                m_ranged = true,
                m_dodgeable = true,
                m_blockable = true,
                m_backstabBonus = 1f,
                m_staggerMultiplier = 1f,
                m_pushForce = EpicLootRaritySetsPlugin.FrostbrandLightningStrikeImpactForce.Value,
                m_point = point,
                m_dir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward
            };

            if (player != null)
            {
                hit.SetAttacker(player);
            }

            return hit;
        }

        private static HitData.DamageTypes CreateFrostbrandFireBallDamage(Player player, ItemDrop.ItemData weapon)
        {
            float skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.ElementalMagic) : 0f;
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_fire = Mathf.Max(0f, EpicLootRaritySetsPlugin.FrostbrandLightningStrikeBaseDamage.Value + skillLevel * EpicLootRaritySetsPlugin.FrostbrandLightningStrikeDamagePerElementalMagicLevel.Value)
            };

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static HitData.DamageTypes CreateSlashDamage(Player player, ItemDrop.ItemData weapon)
        {
            float amount = GetElementalScaled(player, EpicLootRaritySetsPlugin.FrostbrandSlashBaseDamage.Value, EpicLootRaritySetsPlugin.FrostbrandSlashDamagePerElementalMagicLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_slash = amount,
                m_fire = amount
            };

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static HitData.DamageTypes CreateCrushDamage(Player player, ItemDrop.ItemData weapon)
        {
            float amount = GetElementalScaled(player, EpicLootRaritySetsPlugin.FrostbrandCrushBaseDamage.Value, EpicLootRaritySetsPlugin.FrostbrandCrushDamagePerElementalMagicLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_blunt = amount,
                m_fire = amount
            };

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static HitData.DamageTypes CreateBurningGroundDamage(Player player, ItemDrop.ItemData weapon)
        {
            float amount = GetElementalScaled(player, EpicLootRaritySetsPlugin.FrostbrandBurningGroundBaseDamage.Value, EpicLootRaritySetsPlugin.FrostbrandBurningGroundDamagePerElementalMagicLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_fire = amount
            };

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static float GetElementalScaled(Player player, float baseDamage, float perLevel)
        {
            float skill = player != null ? player.GetSkillLevel(Skills.SkillType.ElementalMagic) : 0f;
            return Mathf.Max(0f, baseDamage + skill * perLevel);
        }

        private static void DamageCone(Player player, ItemDrop.ItemData weapon, float range, float radius, HitData.DamageTypes damages, float pushForce)
        {
            Vector3 origin = player.GetCenterPoint();
            Vector3 forward = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up);
            if (forward.sqrMagnitude <= 0.001f)
            {
                forward = player.transform.forward;
            }

            forward.Normalize();
            range = Mathf.Max(0.1f, range);
            radius = Mathf.Max(0.1f, radius);
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                Vector3 toTarget = character.GetCenterPoint() - origin;
                float forwardDistance = Vector3.Dot(Vector3.ProjectOnPlane(toTarget, Vector3.up), forward);
                if (forwardDistance < 0f || forwardDistance > range)
                {
                    continue;
                }

                Vector3 closest = origin + forward * forwardDistance;
                if ((character.GetCenterPoint() - closest).sqrMagnitude > radius * radius)
                {
                    continue;
                }

                character.Damage(CreateHit(player, damages, character.GetCenterPoint(), forward, pushForce));
            }
        }

        private static void DamageArea(Player player, ItemDrop.ItemData weapon, Vector3 center, float radius, HitData.DamageTypes damages)
        {
            radius = Mathf.Max(0.1f, radius);
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                if ((character.GetCenterPoint() - center).sqrMagnitude > radius * radius)
                {
                    continue;
                }

                Vector3 direction = character.GetCenterPoint() - center;
                character.Damage(CreateHit(player, damages, character.GetCenterPoint(), direction, EpicLootRaritySetsPlugin.FrostbrandCrushBaseDamage.Value));
            }
        }

        private static HitData CreateHit(Player player, HitData.DamageTypes damages, Vector3 point, Vector3 direction, float pushForce)
        {
            HitData hit = new HitData
            {
                m_damage = damages,
                m_skill = Skills.SkillType.ElementalMagic,
                m_skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.ElementalMagic) : 0f,
                m_ranged = false,
                m_dodgeable = true,
                m_blockable = true,
                m_backstabBonus = 1f,
                m_staggerMultiplier = 1f,
                m_pushForce = pushForce,
                m_point = point,
                m_dir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward
            };

            if (player != null)
            {
                hit.SetAttacker(player);
            }

            return hit;
        }

        private static void UpdateBurningGrounds(Player player, float dt)
        {
            if (BurningGrounds.Count == 0)
            {
                return;
            }

            ItemDrop.ItemData weapon = player != null ? player.GetCurrentWeapon() : null;
            _burningGroundTickTimer -= dt;
            bool shouldTick = _burningGroundTickTimer <= 0f;
            if (shouldTick)
            {
                _burningGroundTickTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.FrostbrandBurningGroundTickInterval.Value);
            }

            for (int i = BurningGrounds.Count - 1; i >= 0; i--)
            {
                ActiveBurningGround ground = BurningGrounds[i];
                ground.Remaining -= dt;
                if (ground.Remaining <= 0f)
                {
                    BurningGrounds.RemoveAt(i);
                    continue;
                }

                if (shouldTick && player != null)
                {
                    WithAbilityDamage(() => DamageArea(player, weapon, ground.Position, EpicLootRaritySetsPlugin.FrostbrandCrushRadius.Value, CreateBurningGroundDamage(player, weapon)));
                }
            }
        }

        private static void UpdatePendingCrush(Player player, float dt)
        {
            if (!_pendingCrushActive || player == null)
            {
                return;
            }

            _pendingCrushTimer -= dt;
            _pendingCrushAirTimer += dt;
            float verticalVelocity = GetVerticalVelocity(player);
            float groundDistance = GetGroundDistance(player);
            Vector3 currentPosition = player.transform.position;
            Vector3 horizontalTravelVector = Vector3.ProjectOnPlane(currentPosition - _pendingCrushStartPoint, Vector3.up);
            Vector3 expectedTravelVector = Vector3.ProjectOnPlane(_pendingCrushPoint - _pendingCrushStartPoint, Vector3.up);
            bool reachedTargetArea = expectedTravelVector.sqrMagnitude <= 0.01f ||
                                     horizontalTravelVector.magnitude >= expectedTravelVector.magnitude * 0.72f ||
                                     Vector3.ProjectOnPlane(currentPosition - _pendingCrushPoint, Vector3.up).sqrMagnitude <= 2.25f;
            bool pastMinimumTravel = _pendingCrushAirTimer >= 0.35f;
            bool forcedFinish = _pendingCrushTimer <= 0f || _pendingCrushAirTimer >= 0.95f;

            if (_pendingCrushLeftGround && (verticalVelocity < -0.08f || currentPosition.y < _pendingCrushLastPosition.y - 0.01f))
            {
                _pendingCrushWasDescending = true;
            }

            if (!_pendingCrushLeftGround)
            {
                if (_pendingCrushAirTimer > 0.12f ||
                    groundDistance > 0.35f ||
                    verticalVelocity > 0.35f ||
                    currentPosition.y > _pendingCrushStartPoint.y + 0.25f ||
                    horizontalTravelVector.magnitude > 0.75f)
                {
                    _pendingCrushLeftGround = true;
                    _pendingCrushAirTimer = 0f;
                }

                _pendingCrushLastPosition = currentPosition;
                return;
            }

            Vector3 impact;
            bool canLand = pastMinimumTravel &&
                           (groundDistance <= 0.65f || IsOnGround(player) || (reachedTargetArea && _pendingCrushAirTimer >= 0.55f) || forcedFinish) &&
                           (_pendingCrushWasDescending || verticalVelocity <= 0.2f || _pendingCrushAirTimer >= 0.65f || forcedFinish);
            if (canLand && ResolveCrushImpact(player, forcedFinish, out impact))
            {
                FinishPendingCrush(player, impact);
                return;
            }

            if (forcedFinish)
            {
                ResolveCrushImpact(player, true, out impact);
                FinishPendingCrush(player, impact);
                return;
            }

            _pendingCrushLastPosition = currentPosition;
        }

        private static bool ResolveCrushImpact(Player player, bool force, out Vector3 impact)
        {
            if (TryGetCrushLandingImpact(player, force, out impact))
            {
                return true;
            }

            Vector3 fallback = GetPendingCrushFallbackImpact(player);
            if (TryProjectCrushGround(fallback, out impact))
            {
                return true;
            }

            if (player != null && TryProjectCrushGround(player.transform.position, out impact))
            {
                return true;
            }

            if (TryProjectCrushGround(_pendingCrushPoint, out impact))
            {
                return true;
            }

            impact = fallback;
            return false;
        }

        private static void FinishPendingCrush(Player player, Vector3 impact)
        {
            TryProjectCrushGround(impact, out impact);
            _pendingCrushActive = false;
            _pendingCrushTimer = 0f;
            NorseVisualEffectBridge.Spawn(impact, Quaternion.identity, "FxFireExplosion2", "FxFireExpansion", "Crush", "Surt", "FxFire", "Fire");
            NorseVisualEffectBridge.Spawn(
                impact,
                Quaternion.identity,
                Mathf.Max(0.2f, EpicLootRaritySetsPlugin.FrostbrandBurningGroundDuration.Value),
                "FxBurningGround",
                "BurningGround",
                "Burning Ground",
                "FxFire",
                "Fire");
            WithAbilityDamage(() => DamageArea(player, _pendingCrushWeapon, impact, EpicLootRaritySetsPlugin.FrostbrandCrushRadius.Value, CreateCrushDamage(player, _pendingCrushWeapon)));
            BurningGrounds.Add(new ActiveBurningGround(impact, Mathf.Max(0.1f, EpicLootRaritySetsPlugin.FrostbrandBurningGroundDuration.Value)));
            _pendingCrushWeapon = null;
            _pendingCrushLeftGround = false;
            _pendingCrushWasDescending = false;
            _pendingCrushAirTimer = 0f;
        }

        private static bool TryGetCrushLandingImpact(Player player, bool force, out Vector3 impact)
        {
            impact = player != null ? player.transform.position : _pendingCrushPoint;
            if (player == null)
            {
                return false;
            }

            float verticalVelocity = GetVerticalVelocity(player);
            if (!force && !_pendingCrushWasDescending && verticalVelocity > 0.15f && _pendingCrushAirTimer < 0.75f)
            {
                return false;
            }

            Vector3 forward = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up);
            if (forward.sqrMagnitude > 0.001f)
            {
                forward.Normalize();
            }

            Vector3 current = player.transform.position;
            Vector3[] probes =
            {
                current + forward * 1.2f,
                current + forward * 1.8f,
                current,
                _pendingCrushPoint
            };

            for (int i = 0; i < probes.Length; i++)
            {
                float distance;
                if (!TryFindCrushGroundBelow(probes[i], force ? 12f : 2.25f, out impact, out distance))
                {
                    continue;
                }

                if (force || distance <= 0.75f || IsOnGround(player))
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector3 GetPendingCrushFallbackImpact(Player player)
        {
            if (player == null)
            {
                return _pendingCrushPoint;
            }

            Vector3 horizontalTravelVector = Vector3.ProjectOnPlane(player.transform.position - _pendingCrushStartPoint, Vector3.up);
            return horizontalTravelVector.sqrMagnitude >= 1f ? player.transform.position : _pendingCrushPoint;
        }

        private static void WithAbilityDamage(Action action)
        {
            if (action == null)
            {
                return;
            }

            _abilityDamageDepth++;
            try
            {
                action();
            }
            finally
            {
                _abilityDamageDepth--;
            }
        }

        private static bool TryFindAimPoint(Player player, float range, out Vector3 point)
        {
            Transform originTransform = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            RaycastHit hit;
            if (Physics.Raycast(originTransform.position, originTransform.forward, out hit, Mathf.Max(1f, range), Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                point = hit.point;
                return TryProjectGround(point, out point);
            }

            point = player.transform.position + player.transform.forward * Mathf.Max(1f, range);
            return TryProjectGround(point, out point);
        }

        private static bool TryFindProjectileAim(Player player, float range, out Vector3 point, out Vector3 direction)
        {
            Transform originTransform = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            float clampedRange = Mathf.Max(1f, range);
            RaycastHit hit;
            if (Physics.Raycast(originTransform.position, originTransform.forward, out hit, clampedRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                point = hit.point;
                Vector3 spawnPoint = player.GetCenterPoint() + player.transform.forward * 0.8f + Vector3.up * 0.15f;
                direction = point - spawnPoint;
                if (direction.sqrMagnitude <= 0.001f)
                {
                    direction = originTransform.forward;
                }

                direction.Normalize();
                return true;
            }

            Vector3 spawn = player.GetCenterPoint() + player.transform.forward * 0.8f + Vector3.up * 0.15f;
            direction = player.GetAimDir(spawn);
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = originTransform.forward;
            }

            direction.Normalize();
            point = spawn + direction * clampedRange;
            return true;
        }

        private static bool TryProjectGround(Vector3 probe, out Vector3 point)
        {
            RaycastHit[] hits = Physics.RaycastAll(probe + Vector3.up * 12f, Vector3.down, 30f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null || hit.collider.GetComponentInParent<Character>() != null)
                {
                    continue;
                }

                point = hit.point + Vector3.up * 0.15f;
                return true;
            }

            point = probe;
            return true;
        }

        private static bool TryProjectCrushGround(Vector3 probe, out Vector3 point)
        {
            float distance;
            return TryFindCrushGroundBelow(probe, 30f, out point, out distance);
        }

        private static bool TryFindCrushGroundBelow(Vector3 probe, float maxBelowDistance, out Vector3 point, out float distanceFromProbe)
        {
            RaycastHit[] hits = Physics.RaycastAll(probe + Vector3.up * 6f, Vector3.down, Mathf.Max(0.1f, maxBelowDistance) + 6f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null || hit.collider.GetComponentInParent<Character>() != null)
                {
                    continue;
                }

                point = hit.point + Vector3.up * 0.03f;
                distanceFromProbe = Mathf.Abs(probe.y - hit.point.y);
                return true;
            }

            point = probe;
            distanceFromProbe = float.MaxValue;
            return false;
        }

        private static void MovePlayerTo(Player player, Vector3 point)
        {
            player.StopMovement();
            Rigidbody body = player.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.position = point;
            }

            player.transform.position = point;
        }

        private static void LaunchPlayerToward(Player player, Vector3 point)
        {
            if (player == null)
            {
                return;
            }

            player.StopMovement();
            Vector3 origin = player.transform.position;
            Vector3 toTarget = point - origin;
            Vector3 horizontal = Vector3.ProjectOnPlane(toTarget, Vector3.up);
            if (horizontal.sqrMagnitude <= 0.001f)
            {
                horizontal = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up);
            }

            if (horizontal.sqrMagnitude <= 0.001f)
            {
                horizontal = Vector3.forward;
            }

            float distance = Mathf.Clamp(horizontal.magnitude, 3f, Mathf.Max(3f, EpicLootRaritySetsPlugin.FrostbrandCrushRange.Value));
            Vector3 velocity = horizontal.normalized * Mathf.Clamp(distance * 2.0f, 9f, 24f) + Vector3.up * 7.5f;
            Rigidbody body = player.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = velocity;
                body.angularVelocity = Vector3.zero;
                body.WakeUp();
            }
        }

        private static bool IsOnGround(Player player)
        {
            if (player == null)
            {
                return false;
            }

            if (PlayerIsOnGroundMethod != null)
            {
                try
                {
                    object value = PlayerIsOnGroundMethod.Invoke(player, null);
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return Physics.Raycast(player.transform.position + Vector3.up * 0.25f, Vector3.down, 0.65f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }

        private static float GetGroundDistance(Player player)
        {
            if (player == null)
            {
                return float.MaxValue;
            }

            RaycastHit hit;
            Vector3 origin = player.transform.position + Vector3.up * 0.5f;
            if (Physics.Raycast(origin, Vector3.down, out hit, 8f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                return Mathf.Max(0f, hit.distance - 0.5f);
            }

            return float.MaxValue;
        }

        private static float GetVerticalVelocity(Player player)
        {
            if (player == null)
            {
                return 0f;
            }

            try
            {
                return player.GetVelocity().y;
            }
            catch
            {
            }

            Rigidbody body = player.GetComponent<Rigidbody>();
            return body != null ? body.velocity.y : 0f;
        }

        private static void TriggerSecondaryAttackIfNeeded(Player player, string animationOverride = null, float? damageMultiplierOverride = null, float? staminaOverride = null)
        {
            if (player == null || PlayerStartAttackMethod == null)
            {
                return;
            }

            if (PlayerInAttackMethod != null)
            {
                try
                {
                    object inAttack = PlayerInAttackMethod.Invoke(player, null);
                    if (inAttack is bool && (bool)inAttack)
                    {
                        return;
                    }
                }
                catch
                {
                }
            }

            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            ItemDrop.ItemData.SharedData shared = weapon != null ? weapon.m_shared : null;
            string originalAnimation = null;
            float originalDamageMultiplier = 0f;
            float originalStamina = 0f;
            bool overrideShared = shared != null && shared.m_secondaryAttack != null && !string.IsNullOrEmpty(animationOverride);
            try
            {
                if (overrideShared)
                {
                    originalAnimation = shared.m_secondaryAttack.m_attackAnimation;
                    originalDamageMultiplier = shared.m_secondaryAttack.m_damageMultiplier;
                    originalStamina = shared.m_secondaryAttack.m_attackStamina;
                    shared.m_secondaryAttack.m_attackAnimation = animationOverride;
                    if (damageMultiplierOverride.HasValue)
                    {
                        shared.m_secondaryAttack.m_damageMultiplier = damageMultiplierOverride.Value;
                    }

                    if (staminaOverride.HasValue)
                    {
                        shared.m_secondaryAttack.m_attackStamina = staminaOverride.Value;
                    }
                }

                ParameterInfo[] parameters = PlayerStartAttackMethod.GetParameters();
                object[] args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type type = parameters[i].ParameterType;
                    if (type == typeof(bool))
                    {
                        args[i] = true;
                    }
                    else if (typeof(Character).IsAssignableFrom(type))
                    {
                        args[i] = null;
                    }
                    else
                    {
                        args[i] = type.IsValueType ? Activator.CreateInstance(type) : null;
                    }
                }

                PlayerStartAttackMethod.Invoke(player, args);
            }
            catch
            {
            }
            finally
            {
                if (overrideShared)
                {
                    shared.m_secondaryAttack.m_attackAnimation = originalAnimation;
                    shared.m_secondaryAttack.m_damageMultiplier = originalDamageMultiplier;
                    shared.m_secondaryAttack.m_attackStamina = originalStamina;
                }
            }
        }

        private static void UpdateElementalShield(Player player, float dt)
        {
            if (_elementalShieldRemaining <= 0f)
            {
                return;
            }

            _elementalShieldRemaining -= dt;
            if (_elementalShieldRemaining <= 0f)
            {
                RemoveElementalShield(player);
                return;
            }

            EnsureElementalShieldVisual(player);
            RefreshElementalShieldBuff(player);
        }

        private static void UpdateLightningStackBuff(Player player, float dt)
        {
            _lightningStackRefreshTimer -= dt;
            if (_lightningStackRefreshTimer > 0f)
            {
                return;
            }

            _lightningStackRefreshTimer = 0.5f;
            RefreshLightningStackBuff(player);
        }

        private static void RefreshElementalShieldBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateElementalShieldBuff();
            buff.m_ttl = Mathf.Max(0.1f, _elementalShieldRemaining + 0.1f);
            buff.m_tooltip = string.Format(
                "Escudo de fuego Frostbrand.\n\nMitigacion de todo dano: {0:0.#}%.\nDano de fuego: anulado.\nTiempo restante: {1:0.#}s.",
                GetElementalShieldMitigation(player) * 100f,
                Mathf.Max(0f, _elementalShieldRemaining));
            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 0, 0f, 0);
        }

        private static StatusEffect GetOrCreateElementalShieldBuff()
        {
            if (_elementalShieldBuff == null)
            {
                _elementalShieldBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _elementalShieldBuff.name = ElementalShieldBuffName;
                _elementalShieldBuff.m_name = "Elemental Shield";
                _elementalShieldBuff.m_category = ElementalShieldBuffCategory;
                _elementalShieldBuff.m_flashIcon = false;
                _elementalShieldBuff.m_cooldownIcon = true;
                _elementalShieldBuff.m_hidden = false;
            }

            return _elementalShieldBuff;
        }

        private static void RemoveElementalShield(Player player)
        {
            _elementalShieldRemaining = 0f;
            if (player != null && _elementalShieldBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_elementalShieldBuff.NameHash(), true);
            }

            DestroyVisual(ref _elementalShieldVisual);
        }

        private static void RefreshLightningStackBuff(Player player)
        {
            if (player == null || !IsFrostbrandEnabledAndActive())
            {
                return;
            }

            StatusEffect buff = GetOrCreateLightningStackBuff(GetWeaponIcon(player.GetCurrentWeapon()));
            int required = Mathf.Max(1, EpicLootRaritySetsPlugin.FrostbrandLightningStrikeAttackCount.Value);
            _lightningStrikeHits = Mathf.Clamp(_lightningStrikeHits, 0, required);
            buff.m_ttl = 0f;
            buff.m_name = string.Format("Frostbrand {0}/{1}", _lightningStrikeHits, required);
            buff.m_tooltip = string.Format(
                "Frostbrand esta cargando Fire Ball.\n\nStacks: {0}/{1}.\nCuenta cada ataque iniciado con arma Frostbrand, aunque no golpee. No cuentan habilidades.\nAl completar lanza una bola de fuego = {2:0.#}+{3:0.##}/nivel de Magia elemental. Si el proyectil no esta disponible, aplica dano en {4:0.#}m.",
                _lightningStrikeHits,
                required,
                EpicLootRaritySetsPlugin.FrostbrandLightningStrikeBaseDamage.Value,
                EpicLootRaritySetsPlugin.FrostbrandLightningStrikeDamagePerElementalMagicLevel.Value,
                EpicLootRaritySetsPlugin.FrostbrandLightningStrikeRadius.Value);
            SEMan seMan = player.GetSEMan();
            StatusEffect active = seMan.GetStatusEffect(buff.NameHash());
            if (active == null)
            {
                seMan.AddStatusEffect(buff, true, 1, 0f, 0);
                return;
            }

            active.m_name = buff.m_name;
            active.m_tooltip = buff.m_tooltip;
            active.m_ttl = buff.m_ttl;
            active.m_icon = buff.m_icon;
            active.m_flashIcon = buff.m_flashIcon;
            active.m_cooldownIcon = buff.m_cooldownIcon;
            active.m_hidden = buff.m_hidden;
            active.m_category = buff.m_category;
            if (StatusEffectTimeField != null)
            {
                try
                {
                    StatusEffectTimeField.SetValue(active, 0f);
                }
                catch
                {
                }
            }
        }

        private static StatusEffect GetOrCreateLightningStackBuff(Sprite icon)
        {
            if (_lightningStackBuff == null)
            {
                _lightningStackBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _lightningStackBuff.name = LightningStackBuffName;
                _lightningStackBuff.m_name = "Frostbrand Charge";
                _lightningStackBuff.m_category = LightningStackBuffCategory;
                _lightningStackBuff.m_flashIcon = false;
                _lightningStackBuff.m_cooldownIcon = false;
                _lightningStackBuff.m_hidden = false;
            }

            if (icon != null)
            {
                _lightningStackBuff.m_icon = icon;
            }

            return _lightningStackBuff;
        }

        private static void RemoveLightningStackBuff(Player player)
        {
            _lightningStrikeHits = 0;
            if (player != null && _lightningStackBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_lightningStackBuff.NameHash(), true);
            }
        }

        private static void EnsureElementalShieldVisual(Player player)
        {
            if (player == null || _elementalShieldRemaining <= 0f || _elementalShieldVisual != null)
            {
                return;
            }

            _elementalShieldVisual = NorseVisualEffectBridge.SpawnAttached(
                player,
                Vector3.up * 1.0f,
                Quaternion.identity,
                Mathf.Max(0.2f, _elementalShieldRemaining + 0.25f),
                "FxFireShield",
                "FireShield",
                "ElementalShield",
                "Shield");
        }

        private static Sprite GetWeaponIcon(ItemDrop.ItemData weapon)
        {
            if (weapon == null || weapon.m_shared == null || weapon.m_shared.m_icons == null || weapon.m_shared.m_icons.Length == 0)
            {
                return null;
            }

            return weapon.m_shared.m_icons[0];
        }

        private static void DestroyVisual(ref GameObject visual)
        {
            if (visual != null)
            {
                UnityEngine.Object.Destroy(visual);
                visual = null;
            }
        }

        private static float GetElementalShieldMitigation(Player player)
        {
            float skill = player != null ? player.GetSkillLevel(Skills.SkillType.ElementalMagic) : 0f;
            return Mathf.Clamp(
                EpicLootRaritySetsPlugin.FrostbrandElementalShieldBaseMitigation.Value + skill * EpicLootRaritySetsPlugin.FrostbrandElementalShieldMitigationPerElementalMagicLevel.Value,
                0f,
                Mathf.Clamp01(EpicLootRaritySetsPlugin.FrostbrandElementalShieldMaxMitigation.Value));
        }

        private static void ScaleNonFireDamage(ref HitData.DamageTypes damages, float multiplier)
        {
            damages.m_blunt *= multiplier;
            damages.m_slash *= multiplier;
            damages.m_pierce *= multiplier;
            damages.m_chop *= multiplier;
            damages.m_pickaxe *= multiplier;
            damages.m_frost *= multiplier;
            damages.m_lightning *= multiplier;
            damages.m_poison *= multiplier;
            damages.m_spirit *= multiplier;
        }

        private static void ApplyEpicLootDamageModifiers(Player player, ItemDrop.ItemData weapon, ref HitData.DamageTypes damages)
        {
            if (player == null || weapon == null || ApplyMagicDamageModifiersMethod == null)
            {
                return;
            }

            try
            {
                object[] args = { player, weapon, damages };
                ApplyMagicDamageModifiersMethod.Invoke(null, args);
                damages = (HitData.DamageTypes)args[2];
            }
            catch (Exception ex)
            {
                EpicLootRaritySetsPlugin.Log.LogWarning("Could not apply EpicLoot damage modifiers to Frostbrand Fire Ball. " + ex.GetBaseException().Message);
            }
        }

        private static bool IsFrostbrandEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableFrostbrandAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableFrostbrandAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool IsFrostbrandWeapon(ItemDrop.ItemData weapon)
        {
            if (weapon == null)
            {
                return false;
            }

            MagicItem magicItem = ItemDataExtensions.GetMagicItem(weapon);
            if (magicItem == null)
            {
                return false;
            }

            return ContainsFrostbrand(magicItem.SetID) || ContainsFrostbrand(magicItem.LegendaryID);
        }

        private static bool ContainsFrostbrand(string value)
        {
            return !string.IsNullOrEmpty(value) && value.IndexOf("Frostbrand", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player)
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        private static bool TrySpendEitrAndCooldown(Player player, string abilityName, float eitrUse, float cooldown)
        {
            if (cooldown > 0f)
            {
                ShowMessage(player, string.Format("{0}: {1:0}s cooldown.", abilityName, cooldown));
                return false;
            }

            if (eitrUse > 0f && !player.HaveEitr(eitrUse))
            {
                if (Hud.instance != null)
                {
                    Hud.instance.EitrBarEmptyFlash();
                }

                ShowMessage(player, string.Format("{0}: not enough eitr.", abilityName));
                return false;
            }

            return true;
        }

        private static void ShowMessage(Player player, string message)
        {
            player.Message(MessageHud.MessageType.Center, message, 0, null, false);
        }

        private sealed class ActiveBurningGround
        {
            internal readonly Vector3 Position;
            internal float Remaining;

            internal ActiveBurningGround(Vector3 position, float remaining)
            {
                Position = position;
                Remaining = remaining;
            }
        }
    }

    internal static class HraesvelgrAbilityController
    {
        private const string RequiredSet = "Hraesvelgr";
        private const string SneakyBuffName = "FranHraesvelgrSneaky";
        private const string SneakyBuffCategory = "FranHraesvelgrSneaky";
        private const string VolleyBuffName = "FranHraesvelgrRapidVolley";
        private const string VolleyBuffCategory = "FranHraesvelgrRapidVolley";
        private const string SummonBuffName = "FranHraesvelgrBeasts";
        private const string SummonBuffCategory = "FranHraesvelgrBeasts";
        private const string TrapDamageBuffName = "FranHraesvelgrTrapFocus";
        private const string TrapDamageBuffCategory = "FranHraesvelgrTrapFocus";
        private const string TrapRootBuffName = "FranHraesvelgrTrapRoot";
        private const string TrapRootBuffCategory = "FranHraesvelgrTrapRoot";
        private const string TrapChargeBuffName = "FranHraesvelgrTrapCharges";
        private const string TrapChargeBuffCategory = "FranHraesvelgrTrapCharges";
        private const string HeadshotBuffName = "FranHraesvelgrHeadshot";
        private const string HeadshotBuffCategory = "FranHraesvelgrHeadshot";
        private const float SneakyRefreshInterval = 0.35f;
        private const float SneakyBuffTtl = 1f;
        private const float AggroSuppressInterval = 0.25f;
        private const float SummonDeathCooldownSeconds = 60f;

        private static readonly FieldInfo NoiseModifierField = AccessTools.Field(typeof(SE_Stats), "m_noiseModifier");
        private static readonly FieldInfo StealthModifierField = AccessTools.Field(typeof(SE_Stats), "m_stealthModifier");
        private static readonly FieldInfo SpeedModifierField = AccessTools.Field(typeof(SE_Stats), "m_speedModifier");
        private static readonly FieldInfo AttackCharacterField = AccessTools.Field(typeof(Attack), "m_character");
        private static readonly FieldInfo AttackWeaponField = AccessTools.Field(typeof(Attack), "m_weapon");
        private static readonly MethodInfo IsCrouchingMethod =
            AccessTools.Method(typeof(Player), "IsCrouching", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "IsCrouching", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Player), "IsSneaking", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "IsSneaking", Type.EmptyTypes);
        private static readonly FieldInfo CrouchingField =
            AccessTools.Field(typeof(Player), "m_crouching") ??
            AccessTools.Field(typeof(Character), "m_crouching") ??
            AccessTools.Field(typeof(Player), "m_crouch") ??
            AccessTools.Field(typeof(Character), "m_crouch");
        private static readonly MethodInfo GetStaminaMethod =
            AccessTools.Method(typeof(Player), "GetStamina", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "GetStamina", Type.EmptyTypes);
        private static readonly MethodInfo GetMaxStaminaMethod =
            AccessTools.Method(typeof(Player), "GetMaxStamina", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "GetMaxStamina", Type.EmptyTypes);
        private static readonly MethodInfo AddStaminaMethod =
            AccessTools.Method(typeof(Player), "AddStamina", new[] { typeof(float) }) ??
            AccessTools.Method(typeof(Character), "AddStamina", new[] { typeof(float) });
        private static readonly MethodInfo UseStaminaMethod =
            AccessTools.Method(typeof(Player), "UseStamina", new[] { typeof(float) }) ??
            AccessTools.Method(typeof(Character), "UseStamina", new[] { typeof(float) });
        private static readonly FieldInfo StaminaField =
            AccessTools.Field(typeof(Player), "m_stamina") ??
            AccessTools.Field(typeof(Character), "m_stamina");
        private static readonly MethodInfo GetAllCharactersMethod = AccessTools.Method(typeof(Character), "GetAllCharacters", Type.EmptyTypes);
        private static readonly FieldInfo BaseAITargetCreatureField = AccessTools.Field(typeof(BaseAI), "m_targetCreature");
        private static readonly FieldInfo BaseAITargetStaticField = AccessTools.Field(typeof(BaseAI), "m_targetStatic");
        private static readonly FieldInfo BaseAIAlertedField = AccessTools.Field(typeof(BaseAI), "m_alerted");
        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly MethodInfo TameMethod = AccessTools.Method(typeof(Tameable), "Tame");
        private static readonly MethodInfo GetAmmoItemMethod =
            AccessTools.Method(typeof(Humanoid), "GetAmmoItem", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Player), "GetAmmoItem", Type.EmptyTypes);
        private static readonly MethodInfo ApplyMagicDamageModifiersMethod = AccessTools.Method(typeof(ModifyDamage), "ApplyMagicDamageModifiers");
        private static readonly FieldInfo ProjectileOwnerField = AccessTools.Field(typeof(Projectile), "m_owner");
        private static readonly FieldInfo ProjectileVelocityField = AccessTools.Field(typeof(Projectile), "m_vel");
        private static readonly MethodInfo BaseAISetTargetMethod = AccessTools.Method(typeof(BaseAI), "SetTarget", new[] { typeof(Character) });
        private static readonly MethodInfo MonsterAISetTargetMethod = AccessTools.Method(typeof(MonsterAI), "SetTarget", new[] { typeof(Character) });
        private static readonly MethodInfo TrapRequestStateChangeMethod = AccessTools.Method(typeof(Trap), "RequestStateChange");
        private static readonly MethodInfo TrapUpdateStateMethod = AccessTools.Method(typeof(Trap), "UpdateState", Type.EmptyTypes);
        private static readonly Type TrapStateType = AccessTools.Inner(typeof(Trap), "TrapState");
        private static readonly FieldInfo TrapTriggeredByPlayersField = AccessTools.Field(typeof(Trap), "m_triggeredByPlayers");
        private static readonly FieldInfo TrapTriggeredByTamedField = AccessTools.Field(typeof(Trap), "m_triggeredByTamed");
        private static readonly FieldInfo TrapTriggeredByTamesField = AccessTools.Field(typeof(Trap), "m_triggeredByTames");
        private static readonly FieldInfo ZdoVarsStateField = AccessTools.Field(typeof(ZDOVars), "s_state");

        private static StatusEffect _sneakyBuff;
        private static StatusEffect _volleyBuff;
        private static StatusEffect _summonBuff;
        private static StatusEffect _trapDamageBuff;
        private static StatusEffect _trapRootBuff;
        private static StatusEffect _trapChargeBuff;
        private static StatusEffect _headshotBuff;
        private static readonly List<GameObject> SummonedBeasts = new List<GameObject>();
        private static readonly Dictionary<int, string> SummonedBeastKinds = new Dictionary<int, string>();
        private static readonly Dictionary<string, string> StoredSummonNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly List<MaterialVisualState> SneakyVisualStates = new List<MaterialVisualState>();
        private static float _sneakyRefreshTimer;
        private static float _aggroSuppressTimer;
        private static float _volleyCooldown;
        private static float _volleyRemaining;
        private static float _volleyShotTimer;
        private static float _dashCooldown;
        private static float _summonDeathCooldown;
        private static float _summonGuardTimer;
        private static float _trapDamageRemaining;
        private static float _trapRechargeTimer;
        private static float _trapChargeRefreshTimer;
        private static float _headshotBuffRefreshTimer;
        private static int _trapCharges;
        private static int _headshotStacks;
        private static int _headshotProjectileDepth;
        private static int _lastBowShotFrame = -1;
        private static Player _headshotProjectileOwner;
        private static ItemDrop.ItemData _headshotProjectileWeapon;
        private static bool _trapDamageArmed;
        private static bool _headshotArmed;
        private static bool _trapChargesInitialized;
        private static bool _sneakyActive;

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            _volleyCooldown = Mathf.Max(0f, _volleyCooldown - dt);
            _dashCooldown = Mathf.Max(0f, _dashCooldown - dt);
            _summonDeathCooldown = Mathf.Max(0f, _summonDeathCooldown - dt);
            UpdateSummonedBeasts(dt);
            UpdateTrapDamageBuff(player, dt);

            if (!IsHraesvelgrEnabledAndActive())
            {
                RemoveStatusEffects(player);
                if (_sneakyActive || SneakyVisualStates.Count > 0)
                {
                    RestoreSneakyVisual();
                }

                DestroySummonedBeasts(false);
                _volleyRemaining = 0f;
                _volleyShotTimer = 0f;
                _dashCooldown = 0f;
                _summonDeathCooldown = 0f;
                _summonGuardTimer = 0f;
                _trapDamageRemaining = 0f;
                _trapDamageArmed = false;
                _trapChargesInitialized = false;
                _trapCharges = 0;
                _trapRechargeTimer = 0f;
                _trapChargeRefreshTimer = 0f;
                _headshotStacks = 0;
                _headshotArmed = false;
                _headshotProjectileDepth = 0;
                _headshotProjectileOwner = null;
                _headshotProjectileWeapon = null;
                _headshotBuffRefreshTimer = 0f;
                _sneakyRefreshTimer = 0f;
                _aggroSuppressTimer = 0f;
                _sneakyActive = false;
                return;
            }

            UpdateTrapCharges(player, dt);
            UpdateHeadshotBuff(player, dt);
            UpdatePassiveSneaky(player, dt);
            UpdateRapidVolley(player, dt);

            if (!CanReadAbilityInput(player))
            {
                return;
            }

            if (SetAbilityInput.IsSecondaryAttackDown())
            {
                TryDash(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.HraesvelgrVolleyHotkey))
            {
                TryRapidVolley(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.HraesvelgrTrapHotkey))
            {
                TryPlaceTrap(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.HraesvelgrSummonHotkey))
            {
                TrySummonBeasts(player);
            }
        }

        internal static void Clear(Player player)
        {
            RemoveStatusEffects(player);
            DestroySummonedBeasts(false);
            _sneakyRefreshTimer = 0f;
            _aggroSuppressTimer = 0f;
            _volleyCooldown = 0f;
            _volleyRemaining = 0f;
            _volleyShotTimer = 0f;
            _dashCooldown = 0f;
            _summonDeathCooldown = 0f;
            _summonGuardTimer = 0f;
            _trapDamageRemaining = 0f;
            _trapDamageArmed = false;
            _trapChargesInitialized = false;
            _trapCharges = 0;
            _trapRechargeTimer = 0f;
            _trapChargeRefreshTimer = 0f;
            _headshotStacks = 0;
            _headshotArmed = false;
            _headshotProjectileDepth = 0;
            _headshotProjectileOwner = null;
            _headshotProjectileWeapon = null;
            _headshotBuffRefreshTimer = 0f;
            _sneakyActive = false;
        }

        internal static bool IsRapidVolleyActive(Player player)
        {
            return player != null &&
                   player == Player.m_localPlayer &&
                   _volleyRemaining > 0f &&
                   IsHraesvelgrEnabledAndActive();
        }

        internal static bool IsHraesvelgrBow(ItemDrop.ItemData weapon)
        {
            if (weapon == null || weapon.m_shared == null || weapon.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Bow)
            {
                return false;
            }

            MagicItem magicItem = ItemDataExtensions.GetMagicItem(weapon);
            if (magicItem == null)
            {
                return false;
            }

            return IsHraesvelgrId(magicItem.SetID) || IsHraesvelgrId(magicItem.LegendaryID);
        }

        internal static bool ShouldAccelerateAttack(Attack attack)
        {
            Player player = GetAttackPlayer(attack);
            if (!IsRapidVolleyActive(player))
            {
                return false;
            }

            ItemDrop.ItemData weapon = GetAttackWeapon(attack);
            if (weapon == null && player != null)
            {
                weapon = player.GetCurrentWeapon();
            }

            return IsHraesvelgrBow(weapon);
        }

        internal static bool IsSneakyInvisibleTarget(Character character)
        {
            Player player = character as Player;
            return player != null &&
                   player == Player.m_localPlayer &&
                   _sneakyActive &&
                   IsHraesvelgrEnabledAndActive();
        }

        internal static bool ShouldKeepSneakyVisual()
        {
            return _sneakyActive && IsHraesvelgrEnabledAndActive();
        }

        internal static bool ShouldSummonTreatAsEnemy(Character first, Character second)
        {
            return IsHraesvelgrEnabledAndActive() &&
                   ((IsSummonedBeast(first) && IsWildSameKindAsSummon(second)) ||
                    (IsSummonedBeast(second) && IsWildSameKindAsSummon(first)));
        }

        internal static bool ShouldAccelerateProjectile(Player player, ItemDrop.ItemData weapon)
        {
            if (!IsRapidVolleyActive(player))
            {
                return false;
            }

            if (weapon == null && player != null)
            {
                weapon = player.GetCurrentWeapon();
            }

            return IsHraesvelgrBow(weapon);
        }

        internal static void ModifyProjectileDamageArgument(object[] args)
        {
            if (args == null || args.Length < 4)
            {
                return;
            }

            HitData hit = args[3] as HitData;
            if (hit == null)
            {
                return;
            }

            ScaleDamage(ref hit.m_damage, Mathf.Max(0.01f, EpicLootRaritySetsPlugin.HraesvelgrVolleyDamageMultiplier.Value));
        }

        internal static void ModifyOutgoingDamage(HitData hit)
        {
            if (hit == null)
            {
                return;
            }
        }

        internal static void ModifyOutgoingDamage(Character target, HitData hit)
        {
            if (target == null || hit == null || !IsHraesvelgrEnabledAndActive())
            {
                return;
            }

            Character attacker = null;
            try
            {
                attacker = hit.GetAttacker();
            }
            catch
            {
            }

            Player player = attacker as Player;
            if (player == null || player != Player.m_localPlayer || target == player || !IsEnemyTarget(player, target))
            {
                return;
            }

            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            if (!IsHraesvelgrBow(weapon))
            {
                return;
            }

            if (IsHeadshotProjectileDamage(player, weapon))
            {
                TrySetWeakSpot(hit);
                ScaleDamage(ref hit.m_damage, Mathf.Max(1f, EpicLootRaritySetsPlugin.HraesvelgrHeadshotDamageMultiplier.Value));
            }
            else if (_headshotArmed)
            {
                TrySetWeakSpot(hit);
                ScaleDamage(ref hit.m_damage, Mathf.Max(1f, EpicLootRaritySetsPlugin.HraesvelgrHeadshotDamageMultiplier.Value));
                _headshotArmed = false;
                _headshotStacks = 0;
                RefreshHeadshotBuff(player, weapon);
            }

            if (_trapDamageArmed)
            {
                ScaleDamage(ref hit.m_damage, 1f + Mathf.Max(0f, EpicLootRaritySetsPlugin.HraesvelgrTrapNextAttackDamageBonus.Value));
                _trapDamageArmed = false;
                _trapDamageRemaining = 0f;
                RemoveTrapDamageBuff(player);
            }
        }

        internal static void OnHraesvelgrProjectileSetup(Projectile projectile, Player player, ItemDrop.ItemData weapon)
        {
            if (projectile == null || player == null || player != Player.m_localPlayer || !IsHraesvelgrEnabledAndActive())
            {
                return;
            }

            if (!IsHraesvelgrBow(weapon))
            {
                weapon = player.GetCurrentWeapon();
                if (!IsHraesvelgrBow(weapon))
                {
                    return;
                }
            }

            if (_lastBowShotFrame == Time.frameCount)
            {
                return;
            }

            _lastBowShotFrame = Time.frameCount;
            int required = Mathf.Max(1, EpicLootRaritySetsPlugin.HraesvelgrHeadshotAttackCount.Value);
            _headshotStacks = Mathf.Clamp(_headshotStacks + 1, 0, required);
            if (_headshotStacks >= required)
            {
                HraesvelgrHeadshotProjectileMarker marker = projectile.GetComponent<HraesvelgrHeadshotProjectileMarker>();
                if (marker == null)
                {
                    marker = projectile.gameObject.AddComponent<HraesvelgrHeadshotProjectileMarker>();
                }

                marker.Owner = player;
                marker.Weapon = weapon;
                _headshotStacks = 0;
                _headshotArmed = false;
            }

            RefreshHeadshotBuff(player, weapon);
        }

        internal static void BeginHeadshotProjectile(Projectile projectile)
        {
            HraesvelgrHeadshotProjectileMarker marker = projectile != null ? projectile.GetComponent<HraesvelgrHeadshotProjectileMarker>() : null;
            if (marker == null || marker.Owner == null || marker.Owner != Player.m_localPlayer || !IsHraesvelgrEnabledAndActive())
            {
                return;
            }

            _headshotProjectileDepth++;
            _headshotProjectileOwner = marker.Owner;
            _headshotProjectileWeapon = marker.Weapon;
        }

        internal static void EndHeadshotProjectile()
        {
            if (_headshotProjectileDepth <= 0)
            {
                return;
            }

            _headshotProjectileDepth--;
            if (_headshotProjectileDepth <= 0)
            {
                _headshotProjectileOwner = null;
                _headshotProjectileWeapon = null;
            }
        }

        internal static void OnHraesvelgrBowShot(Player player, Attack attack, ItemDrop.ItemData weapon)
        {
            // Kept for older patches; the actual third-shot marker is now attached in Projectile.Setup.
        }

        private static bool IsHeadshotProjectileDamage(Player player, ItemDrop.ItemData weapon)
        {
            if (_headshotProjectileDepth <= 0 || player == null || _headshotProjectileOwner != player)
            {
                return false;
            }

            return _headshotProjectileWeapon == null || weapon == null || _headshotProjectileWeapon == weapon || IsHraesvelgrBow(weapon);
        }

        internal static void OnTrapTriggered(Player owner, Character victim)
        {
            if (owner == null || owner != Player.m_localPlayer || victim == null || !IsHraesvelgrEnabledAndActive())
            {
                return;
            }

            ApplyTrapRoot(victim);
            _trapDamageArmed = true;
            _trapDamageRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HraesvelgrTrapBuffDuration.Value);
            RefreshTrapDamageBuff(owner);
            ShowMessage(owner, "Trampa activada: siguiente flecha potenciada.");
        }

        internal static bool TryGetRapidVolleyEffectValue(Player player, string effectType, out float? value)
        {
            value = null;
            if (!IsRapidVolleyActive(player) || string.IsNullOrEmpty(effectType))
            {
                return false;
            }

            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            if (!IsHraesvelgrBow(weapon))
            {
                return false;
            }

            if (string.Equals(effectType, MagicEffectType.QuickDraw, StringComparison.OrdinalIgnoreCase))
            {
                value = 100f;
                return true;
            }

            if (string.Equals(effectType, MagicEffectType.ModifyFireRate, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(effectType, MagicEffectType.ModifyProjectileSpeed, StringComparison.OrdinalIgnoreCase))
            {
                value = 100f;
                return true;
            }

            return false;
        }

        internal static void ApplyRapidVolleyEffectValues(Player player, object values)
        {
            if (values == null || !IsRapidVolleyActive(player))
            {
                return;
            }

            ItemDrop.ItemData weapon = player != null ? player.GetCurrentWeapon() : null;
            if (!IsHraesvelgrBow(weapon))
            {
                return;
            }

            IDictionary dictionary = values as IDictionary;
            if (dictionary == null)
            {
                return;
            }

            SetEffectValue(dictionary, MagicEffectType.QuickDraw, 100f);
            SetEffectValue(dictionary, MagicEffectType.ModifyFireRate, 100f);
            SetEffectValue(dictionary, MagicEffectType.ModifyProjectileSpeed, 100f);
        }

        private static void SetEffectValue(IDictionary values, string effectType, float value)
        {
            try
            {
                values[effectType] = (float?)value;
            }
            catch
            {
                values[effectType] = value;
            }
        }

        private static void UpdatePassiveSneaky(Player player, float dt)
        {
            if (!IsSneaking(player))
            {
                RemoveSneakyBuff(player);
                RestoreSneakyVisual();
                _sneakyActive = false;
                _sneakyRefreshTimer = 0f;
                _aggroSuppressTimer = 0f;
                return;
            }

            _sneakyActive = true;
            ApplySneakyVisual(player);
            _aggroSuppressTimer -= dt;
            if (_aggroSuppressTimer <= 0f)
            {
                _aggroSuppressTimer = AggroSuppressInterval;
                SuppressNearbyAggro(player);
            }

            _sneakyRefreshTimer -= dt;
            if (_sneakyRefreshTimer > 0f)
            {
                return;
            }

            _sneakyRefreshTimer = SneakyRefreshInterval;
            StatusEffect buff = GetOrCreateSneakyBuff(FindHraesvelgrIcon(player));
            if (buff == null)
            {
                return;
            }

            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static void UpdateRapidVolley(Player player, float dt)
        {
            if (_volleyRemaining <= 0f)
            {
                return;
            }

            player.StopMovement();
            _volleyRemaining -= dt;
            UpdateRapidVolleyAutoFire(player, dt);
            if (_volleyRemaining > 0f)
            {
                return;
            }

            _volleyRemaining = 0f;
            _volleyShotTimer = 0f;
            RemoveVolleyBuff(player);
            RestoreFullStamina(player);
            if (_volleyCooldown > 0f)
            {
                AbilityCooldownBuffController.Start(player, "HraesvelgrRapidVolley", "Rapid Volley", _volleyCooldown, GetWeaponIcon(player.GetCurrentWeapon()));
            }
            ShowMessage(player, "Rapid Volley: vigor recuperado.");
        }

        private static void UpdateTrapDamageBuff(Player player, float dt)
        {
            if (!_trapDamageArmed)
            {
                return;
            }

            _trapDamageRemaining -= dt;
            if (_trapDamageRemaining > 0f && IsHraesvelgrEnabledAndActive())
            {
                RefreshTrapDamageBuff(player);
                return;
            }

            _trapDamageArmed = false;
            _trapDamageRemaining = 0f;
            RemoveTrapDamageBuff(player);
        }

        private static void UpdateTrapCharges(Player player, float dt)
        {
            int maxCharges = GetTrapMaxCharges();
            if (!_trapChargesInitialized)
            {
                _trapChargesInitialized = true;
                _trapCharges = maxCharges;
                _trapRechargeTimer = 0f;
                RefreshTrapChargeBuff(player);
                return;
            }

            if (_trapCharges > maxCharges)
            {
                _trapCharges = maxCharges;
            }

            bool changed = false;
            if (_trapCharges < maxCharges)
            {
                float rechargeSeconds = GetTrapRechargeSeconds();
                _trapRechargeTimer += Mathf.Max(0f, dt);
                while (_trapCharges < maxCharges && _trapRechargeTimer >= rechargeSeconds)
                {
                    _trapRechargeTimer -= rechargeSeconds;
                    _trapCharges++;
                    changed = true;
                }
            }
            else
            {
                _trapRechargeTimer = 0f;
            }

            _trapChargeRefreshTimer -= dt;
            if (changed || _trapChargeRefreshTimer <= 0f)
            {
                _trapChargeRefreshTimer = 1f;
                RefreshTrapChargeBuff(player);
            }
        }

        private static void UpdateHeadshotBuff(Player player, float dt)
        {
            _headshotBuffRefreshTimer -= dt;
            if (_headshotBuffRefreshTimer > 0f)
            {
                return;
            }

            _headshotBuffRefreshTimer = 0.5f;
            RefreshHeadshotBuff(player, player != null ? player.GetCurrentWeapon() : null);
        }

        private static void UpdateRapidVolleyAutoFire(Player player, float dt)
        {
            ItemDrop.ItemData weapon = player != null ? player.GetCurrentWeapon() : null;
            if (!IsHraesvelgrBow(weapon) || !IsAttackHeld())
            {
                _volleyShotTimer = 0f;
                return;
            }

            float shotsPerSecond = Mathf.Max(1f, EpicLootRaritySetsPlugin.HraesvelgrVolleyShotsPerSecond.Value);
            float interval = 1f / shotsPerSecond;
            _volleyShotTimer -= dt;
            while (_volleyShotTimer <= 0f && _volleyRemaining > 0f)
            {
                FireRapidVolleyArrow(player, weapon);
                _volleyShotTimer += interval;
            }
        }

        private static bool IsAttackHeld()
        {
            return Input.GetMouseButton(0) ||
                   Input.GetKey(KeyCode.JoystickButton5) ||
                   Input.GetKey(KeyCode.JoystickButton7);
        }

        private static void TryRapidVolley(Player player)
        {
            if (_volleyRemaining > 0f)
            {
                ShowMessage(player, string.Format("Rapid Volley: {0:0}s activo.", _volleyRemaining));
                return;
            }

            if (_volleyCooldown > 0f)
            {
                ShowMessage(player, string.Format("Rapid Volley: {0:0}s cooldown.", _volleyCooldown));
                return;
            }

            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            if (!IsHraesvelgrBow(weapon))
            {
                ShowMessage(player, "Rapid Volley: equipa el arco Hraesvelgr.");
                return;
            }

            _volleyRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HraesvelgrVolleyDuration.Value);
            _volleyCooldown = EpicLootRaritySetsPlugin.HraesvelgrVolleyCooldown.Value;
            _volleyShotTimer = 0f;

            StatusEffect buff = GetOrCreateVolleyBuff(GetWeaponIcon(weapon));
            if (buff != null)
            {
                SEMan seMan = player.GetSEMan();
                seMan.RemoveStatusEffect(buff.NameHash(), true);
                seMan.AddStatusEffect(buff, true, 1, 0f, 0);
            }
        }

        private static void TryDash(Player player)
        {
            if (_dashCooldown > 0f)
            {
                ShowMessage(player, string.Format("Dash: {0:0}s cooldown.", _dashCooldown));
                return;
            }

            float eitrUse = Mathf.Max(0f, EpicLootRaritySetsPlugin.HraesvelgrDashEitrUse.Value);
            if (eitrUse > 0f && !player.HaveEitr(eitrUse))
            {
                if (Hud.instance != null)
                {
                    Hud.instance.EitrBarEmptyFlash();
                }

                ShowMessage(player, "Dash: no tienes eitr suficiente.");
                return;
            }

            string failureReason;
            if (!NorseDashBridge.TryDash(player, out failureReason))
            {
                ShowMessage(player, failureReason);
                return;
            }

            if (eitrUse > 0f)
            {
                player.UseEitr(eitrUse);
            }

            _dashCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.HraesvelgrDashCooldown.Value);
            AbilityCooldownBuffController.Start(player, "HraesvelgrDash", "Dash", _dashCooldown, FindHraesvelgrIcon(player));
        }

        private static void TryPlaceTrap(Player player)
        {
            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            if (!IsHraesvelgrBow(weapon))
            {
                ShowMessage(player, "Trampa: equipa el arco Hraesvelgr.");
                return;
            }

            EnsureTrapChargesInitialized(player);
            if (_trapCharges <= 0)
            {
                ShowMessage(player, string.Format("Trampa: sin cargas. Siguiente carga en {0:0}s.", GetTrapSecondsUntilNextCharge()));
                RefreshTrapChargeBuff(player);
                return;
            }

            float staminaUse = Mathf.Max(0f, EpicLootRaritySetsPlugin.HraesvelgrTrapStaminaUse.Value);
            if (!TrySpendStamina(player, staminaUse))
            {
                ShowMessage(player, "Trampa: no tienes vigor suficiente.");
                return;
            }

            Vector3 spawn = player.transform.position + player.transform.forward * 1.8f;
            TryProjectGround(spawn, out spawn);
            GameObject prefab = GetAnyPrefab("piece_trap_troll", "piece_trap_wood", "piece_trap", "piece_trap_ashlands", "goblin_trap");
            GameObject trap = prefab != null
                ? UnityEngine.Object.Instantiate(prefab, spawn, Quaternion.LookRotation(player.transform.forward))
                : new GameObject("HraesvelgrArmedTrap");
            if (prefab == null)
            {
                trap.transform.position = spawn;
                trap.transform.rotation = Quaternion.LookRotation(player.transform.forward);
            }

            ForceTrapArmed(trap);

            ZNetView zNetView = trap.GetComponent<ZNetView>();
            if (zNetView != null && zNetView.GetZDO() != null)
            {
                zNetView.GetZDO().Persistent = false;
            }

            HraesvelgrTrapMarker marker = trap.GetComponent<HraesvelgrTrapMarker>();
            if (marker == null)
            {
                marker = trap.AddComponent<HraesvelgrTrapMarker>();
            }

            marker.Setup(player);
            ConsumeTrapCharge(player);
            NorseVisualEffectBridge.Spawn(spawn, Quaternion.identity, 2f, "FxRoots", "Trap", "Root", "Snare");
            ShowMessage(player, string.Format("Trampa armada. Cargas: {0}/{1}.", _trapCharges, GetTrapMaxCharges()));
        }

        internal static void ForceTrapArmed(GameObject trapObject)
        {
            if (trapObject == null)
            {
                return;
            }

            Trap trap = trapObject.GetComponent<Trap>();
            if (trap == null)
            {
                return;
            }

            try
            {
                trap.m_startsArmed = true;
                trap.m_triggeredByEnemies = true;
                SetBoolField(trap, TrapTriggeredByPlayersField, false);
                SetBoolField(trap, TrapTriggeredByTamedField, false);
                SetBoolField(trap, TrapTriggeredByTamesField, false);
                if (trap.m_trigger != null)
                {
                    trap.m_trigger.enabled = true;
                }

                if (trap.m_visualArmed != null)
                {
                    trap.m_visualArmed.SetActive(true);
                }

                if (trap.m_visualUnarmed != null)
                {
                    trap.m_visualUnarmed.SetActive(false);
                }
            }
            catch
            {
            }

            try
            {
                ZNetView zNetView = trap.GetComponent<ZNetView>();
                ZDO zdo = zNetView != null ? zNetView.GetZDO() : null;
                object stateKeyObject = ZdoVarsStateField != null ? ZdoVarsStateField.GetValue(null) : null;
                if (zdo != null && stateKeyObject is int)
                {
                    zdo.Set((int)stateKeyObject, 1, false);
                }
            }
            catch
            {
            }

            try
            {
                if (TrapStateType != null && TrapRequestStateChangeMethod != null)
                {
                    object armedState = Enum.ToObject(TrapStateType, 1);
                    TryInvoke(TrapRequestStateChangeMethod, trap, new[] { armedState });
                }

                TryInvoke(TrapUpdateStateMethod, trap, null);
            }
            catch
            {
            }
        }

        private static void TrySummonBeasts(Player player)
        {
            if (HasActiveSummons())
            {
                DestroySummonedBeasts(true);
                RemoveSummonBuff(player);
                ShowMessage(player, "Summon Beasts: mascotas guardadas.");
                return;
            }

            if (_summonDeathCooldown > 0f)
            {
                ShowMessage(player, string.Format("Summon Beasts: {0:0}s cooldown por mascota muerta.", _summonDeathCooldown));
                return;
            }

            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            if (!IsHraesvelgrBow(weapon))
            {
                ShowMessage(player, "Summon Beasts: equipa el arco Hraesvelgr.");
                return;
            }

            float staminaUse = Mathf.Max(0f, EpicLootRaritySetsPlugin.HraesvelgrSummonStaminaUse.Value);
            if (!TrySpendStamina(player, staminaUse))
            {
                ShowMessage(player, "Summon Beasts: no tienes vigor suficiente.");
                return;
            }

            GameObject wolfPrefab = GetPrefab("Wolf");
            GameObject bearPrefab = GetPrefab("Bjorn", "Bear", "BlackBear_TW", "GrizzlyBear_TW");
            if (wolfPrefab == null || bearPrefab == null)
            {
                ShowMessage(player, "Summon Beasts: prefab de lobo u oso no disponible.");
                return;
            }

            DestroySummonedBeasts(false);

            SpawnSummonedBeast(player, wolfPrefab, "Wolf", player.transform.forward * 2f + player.transform.right * 1.1f);
            SpawnSummonedBeast(player, bearPrefab, "Bear", player.transform.forward * 2.2f - player.transform.right * 1.1f);

            StatusEffect buff = GetOrCreateSummonBuff(GetWeaponIcon(weapon));
            if (buff != null)
            {
                SEMan seMan = player.GetSEMan();
                seMan.RemoveStatusEffect(buff.NameHash(), true);
                seMan.AddStatusEffect(buff, true, 1, 0f, 0);
            }

            ShowMessage(player, "Summon Beasts: aliados invocados.");
        }

        private static StatusEffect GetOrCreateSneakyBuff(Sprite icon)
        {
            if (_sneakyBuff == null)
            {
                _sneakyBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _sneakyBuff.name = SneakyBuffName;
                _sneakyBuff.m_name = "Hraesvelgr Sneaky";
                _sneakyBuff.m_category = SneakyBuffCategory;
                _sneakyBuff.m_flashIcon = false;
                _sneakyBuff.m_cooldownIcon = false;
                _sneakyBuff.m_hidden = false;
            }

            _sneakyBuff.m_ttl = SneakyBuffTtl;
            _sneakyBuff.m_tooltip = string.Format(
                "Pasiva del set Hraesvelgr mientras estas agachado/en sigilo.\n\nInvisible para monstruos.\nAspecto espiritual activo.\nRuido x{0:0.##}.\nDeteccion enemiga x{1:0.##}.\nVelocidad +{2:0.#}%.",
                EpicLootRaritySetsPlugin.HraesvelgrSneakyNoiseModifier.Value,
                EpicLootRaritySetsPlugin.HraesvelgrSneakyStealthModifier.Value,
                EpicLootRaritySetsPlugin.HraesvelgrSneakySpeedModifier.Value * 100f);
            ApplySneakyStats(_sneakyBuff);
            if (icon != null)
            {
                _sneakyBuff.m_icon = icon;
            }

            return _sneakyBuff;
        }

        private static StatusEffect GetOrCreateVolleyBuff(Sprite icon)
        {
            if (_volleyBuff == null)
            {
                _volleyBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _volleyBuff.name = VolleyBuffName;
                _volleyBuff.m_name = "Rapid Volley";
                _volleyBuff.m_category = VolleyBuffCategory;
                _volleyBuff.m_flashIcon = false;
                _volleyBuff.m_cooldownIcon = true;
                _volleyBuff.m_hidden = false;
            }

            _volleyBuff.m_ttl = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HraesvelgrVolleyDuration.Value);
            _volleyBuff.m_tooltip = string.Format(
                "Hraesvelgr dispara a maxima velocidad.\n\nMientras mantienes ataque, dispara {3:0.#} flechas/s durante el canal.\nQuickDraw: 100%.\nTasa de fuego: 100%.\nVelocidad de proyectil: 100%.\nMultiplicador de burst del arco: x{0:0.##}.\nMultiplicador extra de flecha: x{1:0.##}.\nDano de flechas durante el buff: x{4:0.##}.\nCoste de vigor: x{2:0.##}.\nAl terminar recuperas todo el vigor.",
                EpicLootRaritySetsPlugin.HraesvelgrVolleyAttackSpeedMultiplier.Value,
                EpicLootRaritySetsPlugin.HraesvelgrVolleyProjectileSpeedMultiplier.Value,
                EpicLootRaritySetsPlugin.HraesvelgrVolleyStaminaUseMultiplier.Value,
                EpicLootRaritySetsPlugin.HraesvelgrVolleyShotsPerSecond.Value,
                EpicLootRaritySetsPlugin.HraesvelgrVolleyDamageMultiplier.Value);
            if (icon != null)
            {
                _volleyBuff.m_icon = icon;
            }

            return _volleyBuff;
        }

        private static void FireRapidVolleyArrow(Player player, ItemDrop.ItemData weapon)
        {
            if (player == null || weapon == null)
            {
                return;
            }

            GameObject projectilePrefab = GetRapidVolleyProjectilePrefab(player, weapon);
            if (projectilePrefab == null)
            {
                ShowMessage(player, "Rapid Volley: projectile unavailable.");
                return;
            }

            Vector3 spawnPoint = player.GetCenterPoint() + player.transform.forward * 0.9f + Vector3.up * 0.15f;
            Vector3 direction = GetRapidVolleyDirection(player, spawnPoint);
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = player.transform.forward;
            }

            direction.Normalize();
            HitData hit = CreateRapidVolleyHit(player, weapon);
            GameObject projectileObject = UnityEngine.Object.Instantiate(projectilePrefab, spawnPoint, Quaternion.LookRotation(direction));
            IProjectile projectile = projectileObject.GetComponent<IProjectile>() ?? projectileObject.GetComponentInChildren<IProjectile>();
            if (projectile != null)
            {
                projectile.Setup(player, direction * Mathf.Max(5f, EpicLootRaritySetsPlugin.HraesvelgrVolleyProjectileVelocity.Value), 0f, hit, weapon, null);
            }

            Projectile projectileComponent = projectileObject.GetComponent<Projectile>() ?? projectileObject.GetComponentInChildren<Projectile>();
            if (projectileComponent != null)
            {
                if (ProjectileOwnerField != null)
                {
                    ProjectileOwnerField.SetValue(projectileComponent, player);
                }

                projectileComponent.m_damage = hit.m_damage;
                projectileComponent.m_backstabBonus = hit.m_backstabBonus;
                if (ProjectileVelocityField != null)
                {
                    ProjectileVelocityField.SetValue(projectileComponent, direction * Mathf.Max(5f, EpicLootRaritySetsPlugin.HraesvelgrVolleyProjectileVelocity.Value));
                }

                projectileComponent.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private static Vector3 GetRapidVolleyDirection(Player player, Vector3 spawnPoint)
        {
            Transform aimTransform = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            RaycastHit hit;
            Vector3 targetPoint = aimTransform.position + aimTransform.forward * 100f;
            if (Physics.Raycast(aimTransform.position, aimTransform.forward, out hit, 120f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Character hitCharacter = hit.collider != null ? hit.collider.GetComponentInParent<Character>() : null;
                if (hitCharacter == null || hitCharacter != player)
                {
                    targetPoint = hit.point;
                }
            }

            Vector3 direction = targetPoint - spawnPoint;
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = player.GetAimDir(spawnPoint);
            }

            return direction.sqrMagnitude > 0.001f ? direction.normalized : player.transform.forward;
        }

        private static GameObject GetRapidVolleyProjectilePrefab(Player player, ItemDrop.ItemData weapon)
        {
            ItemDrop.ItemData ammo = GetAmmoItem(player);
            if (ammo != null && ammo.m_shared != null && ammo.m_shared.m_attack.m_attackProjectile != null)
            {
                return ammo.m_shared.m_attack.m_attackProjectile;
            }

            if (weapon != null && weapon.m_shared != null && weapon.m_shared.m_attack.m_attackProjectile != null)
            {
                return weapon.m_shared.m_attack.m_attackProjectile;
            }

            return GetProjectilePrefab("projectile_woodarrow", "ArrowWood_projectile", "arrow_wood", "projectile_arrow");
        }

        private static ItemDrop.ItemData GetAmmoItem(Player player)
        {
            if (player == null || GetAmmoItemMethod == null)
            {
                return null;
            }

            try
            {
                return GetAmmoItemMethod.Invoke(player, null) as ItemDrop.ItemData;
            }
            catch
            {
                return null;
            }
        }

        private static HitData CreateRapidVolleyHit(Player player, ItemDrop.ItemData weapon)
        {
            ItemDrop.ItemData ammo = GetAmmoItem(player);
            HitData.DamageTypes damages = new HitData.DamageTypes();
            if (weapon != null && weapon.m_shared != null)
            {
                AddDamage(ref damages, weapon.m_shared.m_damages);
            }

            if (ammo != null && ammo.m_shared != null)
            {
                AddDamage(ref damages, ammo.m_shared.m_damages);
            }

            if (GetTotalDamage(damages) <= 0.01f)
            {
                damages.m_pierce = 20f;
            }

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);

            HitData hit = new HitData
            {
                m_damage = damages,
                m_skill = Skills.SkillType.Bows,
                m_skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.Bows) : 0f,
                m_ranged = true,
                m_dodgeable = true,
                m_blockable = true,
                m_backstabBonus = 1f,
                m_staggerMultiplier = 1f
            };

            if (player != null)
            {
                hit.SetAttacker(player);
            }

            return hit;
        }

        private static void ApplyEpicLootDamageModifiers(Player player, ItemDrop.ItemData weapon, ref HitData.DamageTypes damages)
        {
            if (player == null || weapon == null || ApplyMagicDamageModifiersMethod == null)
            {
                return;
            }

            try
            {
                object[] args = { player, weapon, damages };
                ApplyMagicDamageModifiersMethod.Invoke(null, args);
                damages = (HitData.DamageTypes)args[2];
            }
            catch
            {
            }
        }

        private static void AddDamage(ref HitData.DamageTypes target, HitData.DamageTypes add)
        {
            target.m_blunt += add.m_blunt;
            target.m_slash += add.m_slash;
            target.m_pierce += add.m_pierce;
            target.m_chop += add.m_chop;
            target.m_pickaxe += add.m_pickaxe;
            target.m_fire += add.m_fire;
            target.m_frost += add.m_frost;
            target.m_lightning += add.m_lightning;
            target.m_poison += add.m_poison;
            target.m_spirit += add.m_spirit;
        }

        private static void ScaleDamage(ref HitData.DamageTypes damages, float multiplier)
        {
            damages.m_blunt *= multiplier;
            damages.m_slash *= multiplier;
            damages.m_pierce *= multiplier;
            damages.m_chop *= multiplier;
            damages.m_pickaxe *= multiplier;
            damages.m_fire *= multiplier;
            damages.m_frost *= multiplier;
            damages.m_lightning *= multiplier;
            damages.m_poison *= multiplier;
            damages.m_spirit *= multiplier;
        }

        private static void TrySetWeakSpot(HitData hit)
        {
            if (hit == null)
            {
                return;
            }

            foreach (string fieldName in new[] { "m_weakSpot", "m_weakspot", "m_hitWeakSpot", "m_hitWeakspot" })
            {
                FieldInfo field = AccessTools.Field(typeof(HitData), fieldName);
                if (field == null)
                {
                    continue;
                }

                try
                {
                    if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(hit, true);
                    }
                    else if (field.FieldType == typeof(short))
                    {
                        field.SetValue(hit, (short)1);
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        field.SetValue(hit, 1);
                    }
                    else
                    {
                        continue;
                    }

                    return;
                }
                catch
                {
                }
            }
        }

        private static float GetTotalDamage(HitData.DamageTypes damages)
        {
            return damages.m_blunt + damages.m_slash + damages.m_pierce + damages.m_chop + damages.m_pickaxe +
                   damages.m_fire + damages.m_frost + damages.m_lightning + damages.m_poison + damages.m_spirit;
        }

        private static StatusEffect GetOrCreateSummonBuff(Sprite icon)
        {
            if (_summonBuff == null)
            {
                _summonBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _summonBuff.name = SummonBuffName;
                _summonBuff.m_name = "Summon Beasts";
                _summonBuff.m_category = SummonBuffCategory;
                _summonBuff.m_ttl = 0f;
                _summonBuff.m_flashIcon = false;
                _summonBuff.m_cooldownIcon = false;
                _summonBuff.m_hidden = false;
            }

            _summonBuff.m_tooltip = string.Format(
                "Mascotas Hraesvelgr activas.\n\n{0}: guarda el lobo y el oso actuales.\nSe comportan como mascotas domesticadas normales de Valheim y siguen/defienden al duenio con su IA tameada.\nSi les cambiaste el nombre, se recordara al guardarlos y volver a invocarlos, salvo que mueran.",
                FormatShortcut(EpicLootRaritySetsPlugin.HraesvelgrSummonHotkey));
            if (icon != null)
            {
                _summonBuff.m_icon = icon;
            }

            return _summonBuff;
        }

        private static StatusEffect GetOrCreateTrapDamageBuff(Sprite icon)
        {
            if (_trapDamageBuff == null)
            {
                _trapDamageBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _trapDamageBuff.name = TrapDamageBuffName;
                _trapDamageBuff.m_name = "Predator Focus";
                _trapDamageBuff.m_category = TrapDamageBuffCategory;
                _trapDamageBuff.m_flashIcon = false;
                _trapDamageBuff.m_cooldownIcon = true;
                _trapDamageBuff.m_hidden = false;
            }

            _trapDamageBuff.m_ttl = Mathf.Max(0.1f, _trapDamageRemaining + 0.1f);
            _trapDamageBuff.m_tooltip = string.Format(
                "Una trampa Hraesvelgr ha atrapado a un enemigo.\n\nTu siguiente ataque con arco Hraesvelgr inflige +{0:0.#}% dano.\nTiempo restante: {1:0.#}s.",
                EpicLootRaritySetsPlugin.HraesvelgrTrapNextAttackDamageBonus.Value * 100f,
                Mathf.Max(0f, _trapDamageRemaining));
            if (icon != null)
            {
                _trapDamageBuff.m_icon = icon;
            }

            return _trapDamageBuff;
        }

        private static void EnsureTrapChargesInitialized(Player player)
        {
            if (_trapChargesInitialized)
            {
                return;
            }

            _trapChargesInitialized = true;
            _trapCharges = GetTrapMaxCharges();
            _trapRechargeTimer = 0f;
            RefreshTrapChargeBuff(player);
        }

        private static int GetTrapMaxCharges()
        {
            return Mathf.Max(1, EpicLootRaritySetsPlugin.HraesvelgrTrapMaxCharges != null ? EpicLootRaritySetsPlugin.HraesvelgrTrapMaxCharges.Value : 5);
        }

        private static float GetTrapRechargeSeconds()
        {
            return Mathf.Max(1f, EpicLootRaritySetsPlugin.HraesvelgrTrapRechargeSeconds != null ? EpicLootRaritySetsPlugin.HraesvelgrTrapRechargeSeconds.Value : 60f);
        }

        private static float GetTrapSecondsUntilNextCharge()
        {
            if (_trapCharges >= GetTrapMaxCharges())
            {
                return 0f;
            }

            return Mathf.Max(0f, GetTrapRechargeSeconds() - _trapRechargeTimer);
        }

        private static void ConsumeTrapCharge(Player player)
        {
            EnsureTrapChargesInitialized(player);
            int maxCharges = GetTrapMaxCharges();
            _trapCharges = Mathf.Clamp(_trapCharges - 1, 0, maxCharges);
            if (_trapCharges >= maxCharges)
            {
                _trapRechargeTimer = 0f;
            }

            RefreshTrapChargeBuff(player);
        }

        private static void RefreshTrapChargeBuff(Player player)
        {
            if (player == null || !IsHraesvelgrEnabledAndActive())
            {
                return;
            }

            StatusEffect buff = GetOrCreateTrapChargeBuff(FindHraesvelgrIcon(player));
            if (buff == null)
            {
                return;
            }

            ApplyOrUpdateStatusEffect(player, buff);
        }

        private static StatusEffect GetOrCreateTrapChargeBuff(Sprite icon)
        {
            if (_trapChargeBuff == null)
            {
                _trapChargeBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _trapChargeBuff.name = TrapChargeBuffName;
                _trapChargeBuff.m_category = TrapChargeBuffCategory;
                _trapChargeBuff.m_ttl = 0f;
                _trapChargeBuff.m_flashIcon = false;
                _trapChargeBuff.m_cooldownIcon = false;
                _trapChargeBuff.m_hidden = false;
            }

            int maxCharges = GetTrapMaxCharges();
            _trapChargeBuff.m_name = string.Format("Trampas {0}/{1}", _trapCharges, maxCharges);
            _trapChargeBuff.m_tooltip = string.Format(
                "Cargas de trampas armadas Hraesvelgr.\n\nCargas: {0}/{1}.\nRecuperas 1 carga cada {2:0.#}s.\nSiguiente carga: {3:0.#}s.\n{4}: coloca una trampa ya abierta/armada.",
                _trapCharges,
                maxCharges,
                GetTrapRechargeSeconds(),
                GetTrapSecondsUntilNextCharge(),
                FormatShortcut(EpicLootRaritySetsPlugin.HraesvelgrTrapHotkey));
            if (icon != null)
            {
                _trapChargeBuff.m_icon = icon;
            }

            return _trapChargeBuff;
        }

        private static void RefreshTrapDamageBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateTrapDamageBuff(FindHraesvelgrIcon(player));
            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static void RemoveTrapDamageBuff(Player player)
        {
            if (player != null && _trapDamageBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_trapDamageBuff.NameHash(), true);
            }
        }

        private static void RemoveTrapChargeBuff(Player player)
        {
            if (player != null && _trapChargeBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_trapChargeBuff.NameHash(), true);
            }
        }

        private static void RefreshHeadshotBuff(Player player, ItemDrop.ItemData weapon)
        {
            if (player == null)
            {
                return;
            }

            int required = Mathf.Max(1, EpicLootRaritySetsPlugin.HraesvelgrHeadshotAttackCount.Value);
            _headshotStacks = Mathf.Clamp(_headshotStacks, 0, required);
            StatusEffect buff = GetOrCreateHeadshotBuff(GetWeaponIcon(weapon) ?? FindHraesvelgrIcon(player));
            buff.m_ttl = 0f;
            buff.m_name = string.Format("Keen Shot {0}/{1}", _headshotStacks, required);
            buff.m_tooltip = _headshotArmed
                ? string.Format("Hraesvelgr tiene Headshot preparado.\n\nEsta flecha contara como impacto en cabeza/punto debil y aplica al menos x{0:0.##} dano.", EpicLootRaritySetsPlugin.HraesvelgrHeadshotDamageMultiplier.Value)
                : string.Format("Hraesvelgr esta preparando un disparo preciso.\n\nStacks: {0}/{1}.\nCada {1} disparos con arco Hraesvelgr, ese disparo cuenta como headshot.", _headshotStacks, required);
            ApplyOrUpdateStatusEffect(player, buff);
        }

        private static void ApplyOrUpdateStatusEffect(Player player, StatusEffect buff)
        {
            if (player == null || buff == null)
            {
                return;
            }

            SEMan seMan = player.GetSEMan();
            StatusEffect active = seMan.GetStatusEffect(buff.NameHash());
            if (active == null)
            {
                seMan.AddStatusEffect(buff, true, 1, 0f, 0);
                return;
            }

            active.m_name = buff.m_name;
            active.m_tooltip = buff.m_tooltip;
            active.m_ttl = buff.m_ttl;
            active.m_icon = buff.m_icon;
            active.m_flashIcon = buff.m_flashIcon;
            active.m_cooldownIcon = buff.m_cooldownIcon;
            active.m_hidden = buff.m_hidden;
            active.m_category = buff.m_category;
        }

        private static StatusEffect GetOrCreateHeadshotBuff(Sprite icon)
        {
            if (_headshotBuff == null)
            {
                _headshotBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _headshotBuff.name = HeadshotBuffName;
                _headshotBuff.m_name = "Keen Shot";
                _headshotBuff.m_category = HeadshotBuffCategory;
                _headshotBuff.m_flashIcon = false;
                _headshotBuff.m_cooldownIcon = false;
                _headshotBuff.m_hidden = false;
            }

            if (icon != null)
            {
                _headshotBuff.m_icon = icon;
            }

            return _headshotBuff;
        }

        private static void RemoveHeadshotBuff(Player player)
        {
            if (player != null && _headshotBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_headshotBuff.NameHash(), true);
            }
        }

        private static void ApplyTrapRoot(Character target)
        {
            if (target == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateTrapRootBuff();
            target.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            target.GetSEMan().AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static StatusEffect GetOrCreateTrapRootBuff()
        {
            if (_trapRootBuff == null)
            {
                _trapRootBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _trapRootBuff.name = TrapRootBuffName;
                _trapRootBuff.m_name = "Hraesvelgr Trap";
                _trapRootBuff.m_category = TrapRootBuffCategory;
                _trapRootBuff.m_flashIcon = false;
                _trapRootBuff.m_cooldownIcon = true;
                _trapRootBuff.m_hidden = false;
            }

            _trapRootBuff.m_ttl = 5f;
            _trapRootBuff.m_tooltip = "Inmovilizado por una trampa Hraesvelgr.";
            SetFloatField(_trapRootBuff, SpeedModifierField, -1f);
            return _trapRootBuff;
        }

        private static GameObject SpawnSummonedBeast(Player player, GameObject prefab, string kind, Vector3 offset)
        {
            if (player == null || prefab == null)
            {
                return null;
            }

            Vector3 spawnPoint = player.transform.position + offset;
            Quaternion rotation = Quaternion.LookRotation(player.transform.forward);
            GameObject beast = UnityEngine.Object.Instantiate(prefab, spawnPoint, rotation);
            if (beast == null)
            {
                return null;
            }

            SetupSummonedBeast(player, beast);
            ApplyStoredSummonName(kind, beast);
            SummonedBeasts.Add(beast);
            SummonedBeastKinds[beast.GetInstanceID()] = kind;
            return beast;
        }

        private static void SetupSummonedBeast(Player player, GameObject beast)
        {
            Character character = beast != null ? beast.GetComponent<Character>() : null;
            if (character != null)
            {
                character.SetTamed(true);
            }

            Tameable tameable = beast.GetComponent<Tameable>();
            if (tameable == null)
            {
                tameable = beast.AddComponent<Tameable>();
            }

            if (tameable != null)
            {
                tameable.m_commandable = true;
                tameable.m_startsTamed = true;
                tameable.m_fedDuration = Mathf.Max(tameable.m_fedDuration, 3600f);
                tameable.m_unsummonDistance = Mathf.Max(tameable.m_unsummonDistance, 65f);
                tameable.m_unsummonOnOwnerLogoutSeconds = Mathf.Max(tameable.m_unsummonOnOwnerLogoutSeconds, 5f);
                if (TameMethod != null)
                {
                    TryInvoke(TameMethod, tameable, null);
                }
            }

            MonsterAI monsterAI = beast.GetComponent<MonsterAI>();
            if (monsterAI != null)
            {
                monsterAI.MakeTame();
                monsterAI.SetFollowTarget(player.gameObject);
            }

            ZNetView zNetView = beast.GetComponent<ZNetView>();
            ZDO zdo = zNetView != null ? zNetView.GetZDO() : null;
            if (zdo != null)
            {
                zdo.Persistent = false;
            }
        }

        private static void UpdateSummonedBeasts(float dt)
        {
            if (SummonedBeasts.Count == 0)
            {
                RemoveSummonBuff(Player.m_localPlayer);
                return;
            }

            Player owner = Player.m_localPlayer;
            for (int i = SummonedBeasts.Count - 1; i >= 0; i--)
            {
                GameObject beast = SummonedBeasts[i];
                if (beast == null)
                {
                    SummonedBeasts.RemoveAt(i);
                    continue;
                }

                Character character = beast.GetComponent<Character>();
                if (character != null && character.IsDead())
                {
                    ForgetSummonName(beast);
                    SummonedBeastKinds.Remove(beast.GetInstanceID());
                    SummonedBeasts.RemoveAt(i);
                    StartSummonDeathCooldown(owner);
                    continue;
                }

                if (owner != null)
                {
                    RefreshSummonedBeastCombat(owner, beast);
                }
            }

            if (SummonedBeasts.Count == 0)
            {
                RemoveSummonBuff(Player.m_localPlayer);
            }
        }

        private static void RefreshSummonedBeastFollow(Player owner, GameObject beast)
        {
            if (owner == null || beast == null)
            {
                return;
            }

            MonsterAI monsterAI = beast.GetComponent<MonsterAI>();
            if (monsterAI != null)
            {
                monsterAI.MakeTame();
                monsterAI.SetFollowTarget(owner.gameObject);
            }
        }

        private static void RefreshSummonedBeastCombat(Player owner, GameObject beast)
        {
            if (owner == null || beast == null)
            {
                return;
            }

            Character target = FindSummonDefenseTarget(owner);
            if (target == null)
            {
                return;
            }

            BaseAI baseAI = beast.GetComponent<BaseAI>();
            if (baseAI != null)
            {
                AssignAITarget(baseAI, target);
            }
        }

        private static Character FindSummonDefenseTarget(Player owner)
        {
            if (owner == null)
            {
                return null;
            }

            float radius = Mathf.Max(5f, EpicLootRaritySetsPlugin.HraesvelgrSummonGuardRadius.Value);
            float bestScore = radius * radius;
            Character best = null;
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character == owner || character is Player || character.IsDead() || !IsEnemyTarget(owner, character))
                {
                    continue;
                }

                float distanceToOwner = (character.transform.position - owner.transform.position).sqrMagnitude;
                if (distanceToOwner > radius * radius)
                {
                    continue;
                }

                float distanceToBeasts = GetClosestSummonDistanceSqr(character.transform.position);
                float score = Mathf.Min(distanceToOwner, distanceToBeasts);
                if (score >= bestScore)
                {
                    continue;
                }

                bestScore = score;
                best = character;
            }

            return best;
        }

        private static float GetClosestSummonDistanceSqr(Vector3 point)
        {
            float best = float.MaxValue;
            foreach (GameObject beast in SummonedBeasts)
            {
                if (beast == null)
                {
                    continue;
                }

                float distance = (beast.transform.position - point).sqrMagnitude;
                if (distance < best)
                {
                    best = distance;
                }
            }

            return best;
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player)
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return IsWildSameKindAsSummon(target);
        }

        internal static bool IsSummonedBeast(Character character)
        {
            if (character == null)
            {
                return false;
            }

            foreach (GameObject beast in SummonedBeasts)
            {
                if (beast == null)
                {
                    continue;
                }

                Character beastCharacter = beast.GetComponent<Character>();
                if (beastCharacter == character)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsWildSameKindAsSummon(Character target)
        {
            if (target == null || target is Player || target.IsTamed())
            {
                return false;
            }

            string prefabName = GetPrefabishName(target.gameObject);
            foreach (string kind in SummonedBeastKinds.Values)
            {
                if (IsSameBeastKind(prefabName, kind))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSameBeastKind(string prefabName, string kind)
        {
            if (string.IsNullOrEmpty(prefabName) || string.IsNullOrEmpty(kind))
            {
                return false;
            }

            if (string.Equals(kind, "Wolf", StringComparison.OrdinalIgnoreCase))
            {
                return prefabName.IndexOf("Wolf", StringComparison.OrdinalIgnoreCase) >= 0;
            }

            if (string.Equals(kind, "Bear", StringComparison.OrdinalIgnoreCase))
            {
                return prefabName.IndexOf("Bjorn", StringComparison.OrdinalIgnoreCase) >= 0 ||
                       prefabName.IndexOf("Bear", StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return prefabName.IndexOf(kind, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetPrefabishName(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return string.Empty;
            }

            string name = gameObject.name ?? string.Empty;
            return name.Replace("(Clone)", string.Empty).Trim();
        }

        private static void AssignAITarget(BaseAI baseAI, Character target)
        {
            if (baseAI == null || target == null)
            {
                return;
            }

            bool assigned = TryInvokeSetTarget(baseAI, target);
            if (!assigned)
            {
                SetTargetCreatureField(baseAI, target);
            }

            if (BaseAITargetStaticField != null)
            {
                try
                {
                    BaseAITargetStaticField.SetValue(baseAI, null);
                }
                catch
                {
                }
            }

            SetAIAlerted(baseAI, true);
        }

        private static bool TryInvokeSetTarget(BaseAI baseAI, Character target)
        {
            foreach (MethodInfo method in new[] { MonsterAISetTargetMethod, BaseAISetTargetMethod })
            {
                if (method == null || !method.DeclaringType.IsInstanceOfType(baseAI))
                {
                    continue;
                }

                try
                {
                    method.Invoke(baseAI, new object[] { target });
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        private static void SetTargetCreatureField(BaseAI baseAI, Character target)
        {
            if (BaseAITargetCreatureField == null)
            {
                return;
            }

            try
            {
                Type fieldType = BaseAITargetCreatureField.FieldType;
                if (typeof(Character).IsAssignableFrom(fieldType))
                {
                    BaseAITargetCreatureField.SetValue(baseAI, target);
                }
                else if (typeof(GameObject).IsAssignableFrom(fieldType))
                {
                    BaseAITargetCreatureField.SetValue(baseAI, target.gameObject);
                }
            }
            catch
            {
            }
        }

        private static void SetAIAlerted(BaseAI baseAI, bool alerted)
        {
            if (baseAI == null || BaseAIAlertedField == null)
            {
                return;
            }

            try
            {
                BaseAIAlertedField.SetValue(baseAI, alerted);
            }
            catch
            {
            }
        }

        private static bool HasActiveSummons()
        {
            for (int i = SummonedBeasts.Count - 1; i >= 0; i--)
            {
                GameObject beast = SummonedBeasts[i];
                if (beast == null)
                {
                    SummonedBeasts.RemoveAt(i);
                    continue;
                }

                Character character = beast.GetComponent<Character>();
                if (character != null && character.IsDead())
                {
                    ForgetSummonName(beast);
                    SummonedBeastKinds.Remove(beast.GetInstanceID());
                    SummonedBeasts.RemoveAt(i);
                    StartSummonDeathCooldown(Player.m_localPlayer);
                    continue;
                }

                return true;
            }

            return false;
        }

        private static void StartSummonDeathCooldown(Player owner)
        {
            _summonDeathCooldown = Mathf.Max(_summonDeathCooldown, SummonDeathCooldownSeconds);
            if (owner != null)
            {
                AbilityCooldownBuffController.Start(owner, "HraesvelgrSummonBeastsDeath", "Summon Beasts", _summonDeathCooldown, FindHraesvelgrIcon(owner));
            }
        }

        private static void DestroySummonedBeasts(bool storeNames)
        {
            for (int i = SummonedBeasts.Count - 1; i >= 0; i--)
            {
                GameObject beast = SummonedBeasts[i];
                if (beast == null)
                {
                    continue;
                }

                if (storeNames)
                {
                    StoreSummonName(beast);
                }

                if (ZNetScene.instance != null)
                {
                    ZNetScene.instance.Destroy(beast);
                }
                else
                {
                    UnityEngine.Object.Destroy(beast);
                }
            }

            SummonedBeasts.Clear();
            SummonedBeastKinds.Clear();
            RemoveSummonBuff(Player.m_localPlayer);
        }

        private static void StoreSummonName(GameObject beast)
        {
            if (beast == null)
            {
                return;
            }

            string kind;
            if (!SummonedBeastKinds.TryGetValue(beast.GetInstanceID(), out kind) || string.IsNullOrEmpty(kind))
            {
                return;
            }

            string name = GetTameableName(beast);
            if (!string.IsNullOrEmpty(name))
            {
                StoredSummonNames[kind] = name;
            }
        }

        private static void ForgetSummonName(GameObject beast)
        {
            if (beast == null)
            {
                return;
            }

            string kind;
            if (SummonedBeastKinds.TryGetValue(beast.GetInstanceID(), out kind) && !string.IsNullOrEmpty(kind))
            {
                StoredSummonNames.Remove(kind);
            }
        }

        private static void ApplyStoredSummonName(string kind, GameObject beast)
        {
            string name;
            if (string.IsNullOrEmpty(kind) || beast == null || !StoredSummonNames.TryGetValue(kind, out name) || string.IsNullOrEmpty(name))
            {
                return;
            }

            SetTameableName(beast, name);
        }

        private static string GetTameableName(GameObject beast)
        {
            Tameable tameable = beast != null ? beast.GetComponent<Tameable>() : null;
            if (tameable != null)
            {
                try
                {
                    return tameable.GetName();
                }
                catch
                {
                }
            }

            Character character = beast != null ? beast.GetComponent<Character>() : null;
            return character != null ? character.m_name : null;
        }

        private static void SetTameableName(GameObject beast, string name)
        {
            if (beast == null || string.IsNullOrEmpty(name))
            {
                return;
            }

            Tameable tameable = beast.GetComponent<Tameable>();
            if (tameable != null)
            {
                MethodInfo rpcSetName = AccessTools.Method(typeof(Tameable), "RPC_SetName", new[] { typeof(long), typeof(string), typeof(string) });
                if (rpcSetName != null)
                {
                    try
                    {
                        rpcSetName.Invoke(tameable, new object[] { 0L, GetTameableName(beast) ?? string.Empty, name });
                        return;
                    }
                    catch
                    {
                    }
                }
            }

            Character character = beast.GetComponent<Character>();
            if (character != null)
            {
                character.m_name = name;
            }
        }

        private static GameObject GetPrefab(params string[] prefabNames)
        {
            if (prefabNames == null || ZNetScene.instance == null)
            {
                return null;
            }

            foreach (string prefabName in prefabNames)
            {
                if (string.IsNullOrEmpty(prefabName))
                {
                    continue;
                }

                GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
                if (prefab != null && prefab.GetComponent<Character>() != null)
                {
                    return prefab;
                }
            }

            return null;
        }

        private static GameObject GetAnyPrefab(params string[] prefabNames)
        {
            if (prefabNames == null || ZNetScene.instance == null)
            {
                return null;
            }

            foreach (string prefabName in prefabNames)
            {
                if (string.IsNullOrEmpty(prefabName))
                {
                    continue;
                }

                GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
                if (prefab != null)
                {
                    return prefab;
                }
            }

            return null;
        }

        private static bool TryProjectGround(Vector3 probe, out Vector3 point)
        {
            RaycastHit hit;
            if (Physics.Raycast(probe + Vector3.up * 8f, Vector3.down, out hit, 20f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                point = hit.point + Vector3.up * 0.05f;
                return true;
            }

            point = probe;
            return false;
        }

        private static GameObject GetProjectilePrefab(params string[] prefabNames)
        {
            if (prefabNames == null || ZNetScene.instance == null)
            {
                return null;
            }

            foreach (string prefabName in prefabNames)
            {
                if (string.IsNullOrEmpty(prefabName))
                {
                    continue;
                }

                GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
                if (prefab != null && (prefab.GetComponent<IProjectile>() != null || prefab.GetComponentInChildren<IProjectile>() != null))
                {
                    return prefab;
                }
            }

            return null;
        }

        private static bool TrySpendStamina(Player player, float amount)
        {
            if (player == null || amount <= 0f)
            {
                return true;
            }

            float currentStamina = InvokeFloat(GetStaminaMethod, player, -1f);
            if (currentStamina >= 0f && currentStamina < amount)
            {
                return false;
            }

            if (UseStaminaMethod != null)
            {
                try
                {
                    UseStaminaMethod.Invoke(player, new object[] { amount });
                    return true;
                }
                catch
                {
                }
            }

            if (StaminaField != null && currentStamina >= 0f)
            {
                try
                {
                    StaminaField.SetValue(player, Mathf.Max(0f, currentStamina - amount));
                    return true;
                }
                catch
                {
                }
            }

            return currentStamina < 0f || currentStamina >= amount;
        }

        private static void ApplySneakyVisual(Player player)
        {
            if (player == null || SneakyVisualStates.Count > 0)
            {
                return;
            }

            if (NorseSneakyVisualBridge.Apply(player))
            {
                return;
            }

            Renderer[] renderers = player.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material material = materials[i];
                    if (material == null)
                    {
                        continue;
                    }

                    SneakyVisualStates.Add(new MaterialVisualState(material));
                    Color color = material.HasProperty("_Color") ? material.color : Color.white;
                    Color tintColor = material.HasProperty("_TintColor") ? material.GetColor("_TintColor") : color;
                    color.a = color.a <= 0.05f ? 0.68f : Mathf.Min(color.a, 0.68f);
                    tintColor.a = tintColor.a <= 0.05f ? 0.68f : Mathf.Min(tintColor.a, 0.68f);
                    if (material.HasProperty("_Color"))
                    {
                        material.color = color;
                    }

                    if (material.HasProperty("_TintColor"))
                    {
                        material.SetColor("_TintColor", tintColor);
                    }

                    material.SetOverrideTag("RenderType", "Transparent");
                    if (material.HasProperty("_Mode"))
                    {
                        material.SetFloat("_Mode", 3f);
                    }

                    if (material.HasProperty("_SrcBlend"))
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    }

                    if (material.HasProperty("_DstBlend"))
                    {
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    }

                    if (material.HasProperty("_ZWrite"))
                    {
                        material.SetInt("_ZWrite", 1);
                    }

                    material.renderQueue = 3000;
                }
            }
        }

        private static void RestoreSneakyVisual()
        {
            NorseSneakyVisualBridge.Reset();

            foreach (MaterialVisualState state in SneakyVisualStates)
            {
                state.Restore();
            }

            SneakyVisualStates.Clear();
        }

        private static void SuppressNearbyAggro(Player player)
        {
            if (player == null || GetAllCharactersMethod == null)
            {
                return;
            }

            IEnumerable<Character> characters = TryInvoke(GetAllCharactersMethod, null, null) as IEnumerable<Character>;
            if (characters == null)
            {
                return;
            }

            foreach (Character character in characters)
            {
                if (character == null || character == player)
                {
                    continue;
                }

                BaseAI baseAI = character.GetComponent<BaseAI>();
                if (baseAI == null)
                {
                    continue;
                }

                ClearTargetIfPlayer(baseAI, player);
            }
        }

        private static void ClearTargetIfPlayer(BaseAI baseAI, Player player)
        {
            if (baseAI == null || player == null)
            {
                return;
            }

            if (BaseAITargetCreatureField != null)
            {
                try
                {
                    if (object.ReferenceEquals(BaseAITargetCreatureField.GetValue(baseAI), player))
                    {
                        BaseAITargetCreatureField.SetValue(baseAI, null);
                    }
                }
                catch
                {
                }
            }

            if (BaseAITargetStaticField != null)
            {
                try
                {
                    BaseAITargetStaticField.SetValue(baseAI, null);
                }
                catch
                {
                }
            }

            if (BaseAIAlertedField != null)
            {
                try
                {
                    BaseAIAlertedField.SetValue(baseAI, false);
                }
                catch
                {
                }
            }
        }

        private static object TryInvoke(MethodInfo method, object target, object[] args)
        {
            if (method == null)
            {
                return null;
            }

            try
            {
                return method.Invoke(target, args);
            }
            catch
            {
                return null;
            }
        }

        private sealed class MaterialVisualState
        {
            private readonly Material _material;
            private readonly bool _hasColor;
            private readonly bool _hasTintColor;
            private readonly bool _hasEmissionColor;
            private readonly bool _hasMode;
            private readonly bool _hasSrcBlend;
            private readonly bool _hasDstBlend;
            private readonly bool _hasZWrite;
            private readonly Color _color;
            private readonly Color _tintColor;
            private readonly Color _emissionColor;
            private readonly float _mode;
            private readonly int _srcBlend;
            private readonly int _dstBlend;
            private readonly int _zWrite;
            private readonly int _renderQueue;

            internal MaterialVisualState(Material material)
            {
                _material = material;
                _hasColor = material != null && material.HasProperty("_Color");
                _hasTintColor = material != null && material.HasProperty("_TintColor");
                _hasEmissionColor = material != null && material.HasProperty("_EmissionColor");
                _hasMode = material != null && material.HasProperty("_Mode");
                _hasSrcBlend = material != null && material.HasProperty("_SrcBlend");
                _hasDstBlend = material != null && material.HasProperty("_DstBlend");
                _hasZWrite = material != null && material.HasProperty("_ZWrite");
                _color = _hasColor ? material.color : Color.white;
                _tintColor = _hasTintColor ? material.GetColor("_TintColor") : Color.white;
                _emissionColor = _hasEmissionColor ? material.GetColor("_EmissionColor") : Color.black;
                _mode = _hasMode ? material.GetFloat("_Mode") : 0f;
                _srcBlend = _hasSrcBlend ? material.GetInt("_SrcBlend") : 0;
                _dstBlend = _hasDstBlend ? material.GetInt("_DstBlend") : 0;
                _zWrite = _hasZWrite ? material.GetInt("_ZWrite") : 1;
                _renderQueue = material != null ? material.renderQueue : -1;
            }

            internal void Restore()
            {
                if (_material == null)
                {
                    return;
                }

                if (_hasColor)
                {
                    _material.color = _color;
                }

                if (_hasTintColor)
                {
                    _material.SetColor("_TintColor", _tintColor);
                }

                if (_hasEmissionColor)
                {
                    _material.SetColor("_EmissionColor", _emissionColor);
                }

                if (_hasMode)
                {
                    _material.SetFloat("_Mode", _mode);
                }

                if (_hasSrcBlend)
                {
                    _material.SetInt("_SrcBlend", _srcBlend);
                }

                if (_hasDstBlend)
                {
                    _material.SetInt("_DstBlend", _dstBlend);
                }

                if (_hasZWrite)
                {
                    _material.SetInt("_ZWrite", _zWrite);
                }

                _material.renderQueue = _renderQueue;
            }
        }

        private static void ApplySneakyStats(StatusEffect buff)
        {
            SetFloatField(buff, NoiseModifierField, EpicLootRaritySetsPlugin.HraesvelgrSneakyNoiseModifier.Value);
            SetFloatField(buff, StealthModifierField, EpicLootRaritySetsPlugin.HraesvelgrSneakyStealthModifier.Value);
            SetFloatField(buff, SpeedModifierField, EpicLootRaritySetsPlugin.HraesvelgrSneakySpeedModifier.Value);
        }

        private static void SetFloatField(object target, FieldInfo field, float value)
        {
            if (target == null || field == null)
            {
                return;
            }

            try
            {
                if (field.FieldType == typeof(float))
                {
                    field.SetValue(target, value);
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(target, Mathf.RoundToInt(value));
                }
            }
            catch
            {
            }
        }

        private static void SetBoolField(object target, FieldInfo field, bool value)
        {
            if (target == null || field == null)
            {
                return;
            }

            try
            {
                if (field.FieldType == typeof(bool))
                {
                    field.SetValue(target, value);
                }
            }
            catch
            {
            }
        }

        private static bool IsSneaking(Player player)
        {
            if (player == null)
            {
                return false;
            }

            bool result;
            if (TryInvokeBool(IsCrouchingMethod, player, out result))
            {
                return result;
            }

            if (CrouchingField != null)
            {
                try
                {
                    object value = CrouchingField.GetValue(player);
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            try
            {
                return ZInput.GetButton("Crouch");
            }
            catch
            {
                return false;
            }
        }

        private static bool TryInvokeBool(MethodInfo method, object target, out bool result)
        {
            result = false;
            if (method == null || target == null)
            {
                return false;
            }

            try
            {
                object value = method.Invoke(target, null);
                if (value is bool)
                {
                    result = (bool)value;
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        private static bool IsHraesvelgrEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableHraesvelgrAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableHraesvelgrAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool CanReadAbilityInput(Player player)
        {
            return !player.IsDead() &&
                   !player.IsTeleporting() &&
                   !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> shortcutEntry)
        {
            if (shortcutEntry == null)
            {
                return false;
            }

            KeyboardShortcut shortcut = shortcutEntry.Value;
            if (shortcut.IsDown())
            {
                return true;
            }

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static string FormatShortcut(ConfigEntry<KeyboardShortcut> shortcut)
        {
            if (shortcut == null)
            {
                return "Sin tecla";
            }

            string value = shortcut.Value.ToString();
            return string.IsNullOrEmpty(value) ? "Sin tecla" : value;
        }

        private static bool IsAnyMenuOpen()
        {
            if (InventoryGui.instance != null && InventoryGui.IsVisible())
            {
                return true;
            }

            if (Menu.instance != null && Menu.IsVisible())
            {
                return true;
            }

            if (TextInput.instance != null && TextInput.IsVisible())
            {
                return true;
            }

            if (Chat.instance != null && Chat.instance.HasFocus())
            {
                return true;
            }

            return Minimap.instance != null && Minimap.IsOpen();
        }

        private static void RestoreFullStamina(Player player)
        {
            if (player == null)
            {
                return;
            }

            float maxStamina = InvokeFloat(GetMaxStaminaMethod, player, -1f);
            float currentStamina = InvokeFloat(GetStaminaMethod, player, -1f);
            if (maxStamina > 0f && currentStamina >= 0f && AddStaminaMethod != null)
            {
                float amount = Mathf.Max(0f, maxStamina - currentStamina);
                if (amount > 0f)
                {
                    try
                    {
                        AddStaminaMethod.Invoke(player, new object[] { amount });
                        return;
                    }
                    catch
                    {
                    }
                }
            }

            if (maxStamina > 0f && StaminaField != null)
            {
                try
                {
                    StaminaField.SetValue(player, maxStamina);
                }
                catch
                {
                }
            }
        }

        private static float InvokeFloat(MethodInfo method, object target, float fallback)
        {
            if (method == null || target == null)
            {
                return fallback;
            }

            try
            {
                return Convert.ToSingle(method.Invoke(target, null));
            }
            catch
            {
                return fallback;
            }
        }

        private static Player GetAttackPlayer(Attack attack)
        {
            if (attack == null || AttackCharacterField == null)
            {
                return null;
            }

            try
            {
                return AttackCharacterField.GetValue(attack) as Player;
            }
            catch
            {
                return null;
            }
        }

        private static ItemDrop.ItemData GetAttackWeapon(Attack attack)
        {
            if (attack == null || AttackWeaponField == null)
            {
                return null;
            }

            try
            {
                return AttackWeaponField.GetValue(attack) as ItemDrop.ItemData;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsHraesvelgrId(string id)
        {
            return !string.IsNullOrEmpty(id) && id.IndexOf("Hraesvelgr", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static Sprite FindHraesvelgrIcon(Player player)
        {
            ItemDrop.ItemData weapon = player != null ? player.GetCurrentWeapon() : null;
            Sprite icon = GetWeaponIcon(weapon);
            if (icon != null)
            {
                return icon;
            }

            Inventory inventory = player != null ? player.GetInventory() : null;
            if (inventory == null)
            {
                return null;
            }

            foreach (ItemDrop.ItemData item in inventory.GetEquippedItems())
            {
                if (item == null || item.m_shared == null || item.m_shared.m_icons == null || item.m_shared.m_icons.Length == 0)
                {
                    continue;
                }

                MagicItem magicItem = ItemDataExtensions.GetMagicItem(item);
                if (magicItem != null && (IsHraesvelgrId(magicItem.SetID) || IsHraesvelgrId(magicItem.LegendaryID)))
                {
                    return item.m_shared.m_icons[0];
                }
            }

            return null;
        }

        private static Sprite GetWeaponIcon(ItemDrop.ItemData weapon)
        {
            if (weapon == null || weapon.m_shared == null || weapon.m_shared.m_icons == null || weapon.m_shared.m_icons.Length == 0)
            {
                return null;
            }

            return weapon.m_shared.m_icons[0];
        }

        private static void RemoveStatusEffects(Player player)
        {
            RemoveSneakyBuff(player);
            RemoveVolleyBuff(player);
            RemoveSummonBuff(player);
            RemoveTrapDamageBuff(player);
            RemoveTrapChargeBuff(player);
            RemoveHeadshotBuff(player);
        }

        private static void RemoveSneakyBuff(Player player)
        {
            if (player != null && _sneakyBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_sneakyBuff.NameHash(), true);
            }
        }

        private static void RemoveVolleyBuff(Player player)
        {
            if (player != null && _volleyBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_volleyBuff.NameHash(), true);
            }
        }

        private static void RemoveSummonBuff(Player player)
        {
            if (player != null && _summonBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_summonBuff.NameHash(), true);
            }
        }

        private static void ShowMessage(Player player, string message)
        {
            if (player != null)
            {
                player.Message(MessageHud.MessageType.Center, message, 0, null, false);
            }
        }
    }

    internal sealed class HraesvelgrHeadshotProjectileMarker : MonoBehaviour
    {
        internal Player Owner;
        internal ItemDrop.ItemData Weapon;
    }

    internal sealed class HraesvelgrTrapMarker : MonoBehaviour
    {
        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private Player _owner;
        private float _life = 120f;
        private float _armVisualTimer;
        private float _scanTimer;
        private bool _triggered;

        internal void Setup(Player owner)
        {
            _owner = owner;
            _armVisualTimer = 3f;
            HraesvelgrAbilityController.ForceTrapArmed(gameObject);
            SphereCollider collider = gameObject.GetComponent<SphereCollider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<SphereCollider>();
            }

            collider.isTrigger = true;
            collider.radius = Mathf.Max(collider.radius, 1.35f);
        }

        private void Update()
        {
            if (_triggered)
            {
                return;
            }

            _life -= Time.deltaTime;
            if (_life <= 0f || _owner == null)
            {
                Destroy(gameObject);
                return;
            }

            if (_armVisualTimer > 0f)
            {
                _armVisualTimer -= Time.deltaTime;
                HraesvelgrAbilityController.ForceTrapArmed(gameObject);
            }

            _scanTimer -= Time.deltaTime;
            if (_scanTimer > 0f)
            {
                return;
            }

            _scanTimer = 0.1f;
            TryTriggerNearby();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered || other == null)
            {
                return;
            }

            Character character = other.GetComponentInParent<Character>();
            TryTrigger(character);
        }

        private void TryTriggerNearby()
        {
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead())
                {
                    continue;
                }

                if ((character.transform.position - transform.position).sqrMagnitude <= 1.9f * 1.9f && TryTrigger(character))
                {
                    return;
                }
            }
        }

        private bool TryTrigger(Character target)
        {
            if (_owner == null || target == null || target == _owner || target is Player || target.IsDead() ||
                target.IsTamed() || HraesvelgrAbilityController.IsSummonedBeast(target) || !IsEnemyTarget(_owner, target))
            {
                return false;
            }

            _triggered = true;
            HraesvelgrAbilityController.OnTrapTriggered(_owner, target);
            NorseVisualEffectBridge.Spawn(transform.position, Quaternion.identity, 3f, "FxRoots", "Trap", "Root", "Snare");
            if (ZNetScene.instance != null)
            {
                ZNetScene.instance.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            return true;
        }

        private static bool IsEnemyTarget(Player owner, Character target)
        {
            if (owner == null || target == null || target == owner || target is Player ||
                target.IsTamed() || HraesvelgrAbilityController.IsSummonedBeast(target))
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { owner, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        internal static bool ShouldIgnoreNativeTrapTrigger(Trap trap, Collider other)
        {
            if (trap == null || other == null || trap.GetComponent<HraesvelgrTrapMarker>() == null)
            {
                return false;
            }

            Character character = other.GetComponentInParent<Character>();
            return HraesvelgrAbilityController.IsSummonedBeast(character);
        }
    }

    [HarmonyPatch]
    internal static class HraesvelgrTrapNativeTriggerPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (MethodInfo method in AccessTools.GetDeclaredMethods(typeof(Trap)))
            {
                if (method == null || (method.Name != "OnTriggerEnter" && method.Name != "OnTriggerStay"))
                {
                    continue;
                }

                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 1 && typeof(Collider).IsAssignableFrom(parameters[0].ParameterType))
                {
                    yield return method;
                }
            }
        }

        private static bool Prepare()
        {
            return TargetMethods().Any();
        }

        private static bool Prefix(Trap __instance, object[] __args)
        {
            Collider collider = __args != null && __args.Length > 0 ? __args[0] as Collider : null;
            return !HraesvelgrTrapMarker.ShouldIgnoreNativeTrapTrigger(__instance, collider);
        }
    }

    internal static class SolomonKaneAbilityController
    {
        private const string RequiredSet = "SolomonKane";
        private const string InfusedBoltBuffName = "FranSolomonKaneInfusedBolt";
        private const string InfusedBoltBuffCategory = "FranSolomonKaneInfusedBolt";
        private const string SilverVerdictBuffName = "FranSolomonKaneSilverVerdict";
        private const string SilverVerdictBuffCategory = "FranSolomonKaneSilverVerdict";
        private const string WitchmarkBuffName = "FranSolomonKaneWitchmark";
        private const string WitchmarkBuffCategory = "FranSolomonKaneWitchmark";
        private const string WitchmarkDebuffName = "FranSolomonKaneWitchmarked";
        private const string WitchmarkDebuffCategory = "FranSolomonKaneWitchmarked";
        private const string InfusedSlowBuffName = "FranSolomonKaneInfusedSlow";
        private const string InfusedSlowBuffCategory = "FranSolomonKaneInfusedSlow";
        private const string BatFormBuffName = "FranSolomonKaneBatForm";
        private const string BatFormBuffCategory = "FranSolomonKaneBatForm";
        private const float WitchmarkRefreshInterval = 0.45f;

        private static readonly MethodInfo ApplyMagicDamageModifiersMethod = AccessTools.Method(typeof(ModifyDamage), "ApplyMagicDamageModifiers");
        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly FieldInfo SpeedModifierField = AccessTools.Field(typeof(SE_Stats), "m_speedModifier");
        private static readonly FieldInfo DebugFlyField = AccessTools.Field(typeof(Player), "m_debugFly");
        private static readonly FieldInfo DebugFlySpeedField = AccessTools.Field(typeof(Character), "m_debugFlySpeed");
        private static readonly FieldInfo BodyField = AccessTools.Field(typeof(Character), "m_body");
        private static readonly FieldInfo LodVisibleField = AccessTools.Field(typeof(Character), "m_lodVisible");
        private static readonly FieldInfo NViewField = AccessTools.Field(typeof(Character), "m_nview");
        private static readonly FieldInfo ZdoVarsDebugFlyField = AccessTools.Field(typeof(ZDOVars), "s_debugFly");
        private static readonly MethodInfo SetVisibleMethod = AccessTools.Method(typeof(Character), "SetVisible", new[] { typeof(bool) });
        private static readonly MethodInfo GetStaminaMethod =
            AccessTools.Method(typeof(Player), "GetStamina", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "GetStamina", Type.EmptyTypes);
        private static readonly MethodInfo GetMaxStaminaMethod =
            AccessTools.Method(typeof(Player), "GetMaxStamina", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "GetMaxStamina", Type.EmptyTypes);
        private static readonly MethodInfo AddStaminaMethod =
            AccessTools.Method(typeof(Player), "AddStamina", new[] { typeof(float) }) ??
            AccessTools.Method(typeof(Character), "AddStamina", new[] { typeof(float) });
        private static readonly MethodInfo UseStaminaMethod =
            AccessTools.Method(typeof(Player), "UseStamina", new[] { typeof(float) }) ??
            AccessTools.Method(typeof(Character), "UseStamina", new[] { typeof(float) });
        private static readonly FieldInfo StaminaField =
            AccessTools.Field(typeof(Player), "m_stamina") ??
            AccessTools.Field(typeof(Character), "m_stamina");

        private static readonly Dictionary<Character, float> MarkedTargets = new Dictionary<Character, float>();
        private static readonly List<PendingSolomonKaneHit> PendingHits = new List<PendingSolomonKaneHit>();
        private static StatusEffect _infusedBoltBuff;
        private static StatusEffect _silverVerdictBuff;
        private static StatusEffect _witchmarkBuff;
        private static StatusEffect _witchmarkDebuff;
        private static StatusEffect _infusedSlowBuff;
        private static StatusEffect _batFormBuff;
        private static float _infusedBoltCooldown;
        private static float _bombCooldown;
        private static float _batFormCooldown;
        private static float _batFormRemaining;
        private static float _infusedBoltRemaining;
        private static float _witchmarkRefreshTimer;
        private static bool _infusedBoltArmed;
        private static bool _silverVerdictArmed;
        private static bool _batFormActive;
        private static bool _batSavedDebugFly;
        private static bool _batSavedDebugFlyValid;
        private static bool _batSavedLodVisible;
        private static bool _batSavedLodVisibleValid;
        private static int _batSavedDebugFlySpeed;
        private static bool _loggedEpicLootFailure;
        private static bool _loggedBatVisualFailure;
        private static int _projectileHitDepth;
        private static int _abilityDamageDepth;
        private static Player _projectileOwner;
        private static ItemDrop.ItemData _projectileWeapon;
        private static GameObject _batVisual;

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            _infusedBoltCooldown = Mathf.Max(0f, _infusedBoltCooldown - dt);
            _bombCooldown = Mathf.Max(0f, _bombCooldown - dt);
            _batFormCooldown = Mathf.Max(0f, _batFormCooldown - dt);

            if (!IsEnabledAndActive())
            {
                Clear(player);
                return;
            }

            UpdateArmedInfusedBolt(player, dt);
            UpdateMarkedTargets(player, dt);
            UpdateBatForm(player, dt);

            if (!CanReadAbilityInput(player))
            {
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltHotkey))
            {
                TryArmInfusedBolt(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.SolomonKaneBatFormHotkey))
            {
                TryBatForm(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.SolomonKaneBombHotkey))
            {
                TryBlackpowderBomb(player);
            }
        }

        internal static void Clear(Player player)
        {
            RemoveInfusedBoltBuff(player);
            RemoveSilverVerdictBuff(player);
            RemoveWitchmarkBuff(player);
            EndBatForm(player, true);
            RemoveMarkedTargetDebuffs();
            MarkedTargets.Clear();
            PendingHits.Clear();
            _infusedBoltCooldown = 0f;
            _bombCooldown = 0f;
            _batFormCooldown = 0f;
            _batFormRemaining = 0f;
            _infusedBoltRemaining = 0f;
            _witchmarkRefreshTimer = 0f;
            _infusedBoltArmed = false;
            _silverVerdictArmed = false;
            _batFormActive = false;
            _projectileHitDepth = 0;
            _abilityDamageDepth = 0;
            _projectileOwner = null;
            _projectileWeapon = null;
        }

        internal static void TryMarkProjectile(Projectile projectile, Player player, ItemDrop.ItemData weapon)
        {
            if (projectile == null || player == null || player != Player.m_localPlayer || !IsEnabledAndActive())
            {
                return;
            }

            if (!IsSolomonKaneCrossbow(weapon))
            {
                weapon = player.GetCurrentWeapon();
                if (!IsSolomonKaneCrossbow(weapon))
                {
                    return;
                }
            }

            if (IsKnownAbilityProjectile(projectile.gameObject))
            {
                return;
            }

            SolomonKaneProjectileMarker marker = projectile.GetComponent<SolomonKaneProjectileMarker>();
            if (marker == null)
            {
                marker = projectile.gameObject.AddComponent<SolomonKaneProjectileMarker>();
            }

            marker.Owner = player;
            marker.Weapon = weapon;
            marker.InfusedBolt = _infusedBoltArmed;

            if (_infusedBoltArmed)
            {
                _infusedBoltArmed = false;
                _infusedBoltRemaining = 0f;
                RemoveInfusedBoltBuff(player);
                ShowMessage(player, "Infused Bolt: virote cargado.");
            }
        }

        internal static void BeginProjectileHit(Projectile projectile, Vector3 hitPoint)
        {
            SolomonKaneProjectileMarker marker = projectile != null ? projectile.GetComponent<SolomonKaneProjectileMarker>() : null;
            if (marker == null || marker.Owner == null || marker.Owner != Player.m_localPlayer || !IsEnabledAndActive())
            {
                return;
            }

            _projectileHitDepth++;
            _projectileOwner = marker.Owner;
            _projectileWeapon = marker.Weapon;

            if (marker.InfusedBolt)
            {
                marker.InfusedBolt = false;
                DetonateInfusedBolt(marker.Owner, marker.Weapon, hitPoint);
            }
        }

        internal static void EndProjectileHit()
        {
            if (_projectileHitDepth <= 0)
            {
                return;
            }

            _projectileHitDepth--;
            if (_projectileHitDepth <= 0)
            {
                _projectileOwner = null;
                _projectileWeapon = null;
            }
        }

        internal static void ModifyOutgoingDamage(Character target, HitData hit)
        {
            if (_abilityDamageDepth > 0 || target == null || hit == null || !IsEnabledAndActive())
            {
                return;
            }

            Player player = GetAttacker(hit) as Player;
            if (player == null || player != Player.m_localPlayer || target == player || !IsEnemyTarget(player, target))
            {
                return;
            }

            ItemDrop.ItemData weapon = GetProjectileWeapon(player);
            if (!IsSolomonKaneCrossbow(weapon))
            {
                return;
            }

            bool wasMarked = IsMarked(target);
            bool weakSpot = IsWeakSpotHit(hit);
            bool consumeSilverVerdict = _silverVerdictArmed;
            HitData.DamageTypes silverDamage = new HitData.DamageTypes();

            if (consumeSilverVerdict)
            {
                silverDamage = CreateSilverVerdictDamage(player, weapon);
                AddDamage(ref hit.m_damage, silverDamage);
                _silverVerdictArmed = false;
                RemoveSilverVerdictBuff(player);
            }

            PendingHits.Add(new PendingSolomonKaneHit(player, weapon, target, hit, wasMarked, weakSpot, consumeSilverVerdict, silverDamage));
            TrimPendingHits();
        }

        internal static void OnPlayerHit(Character target, HitData hit)
        {
            PendingSolomonKaneHit pending = TakePendingHit(target, hit);
            if (pending == null || pending.Player == null || pending.Player != Player.m_localPlayer)
            {
                return;
            }

            if (pending.ConsumedSilverVerdict)
            {
                RefundStamina(pending.Player, EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictStaminaRefund.Value);
                ChainSilverVerdict(pending);
            }

            if (target != null && !target.IsDead())
            {
                MarkTarget(pending.Player, pending.Weapon, target);
            }

            if (!pending.ConsumedSilverVerdict && pending.WasMarked && (pending.WasWeakSpot || (target != null && target.IsDead())))
            {
                ArmSilverVerdict(pending.Player, pending.Weapon);
            }
        }

        private static void UpdateArmedInfusedBolt(Player player, float dt)
        {
            if (!_infusedBoltArmed)
            {
                return;
            }

            _infusedBoltRemaining -= dt;
            if (_infusedBoltRemaining <= 0f)
            {
                _infusedBoltArmed = false;
                _infusedBoltRemaining = 0f;
                RemoveInfusedBoltBuff(player);
            }
        }

        private static void UpdateMarkedTargets(Player player, float dt)
        {
            foreach (Character target in MarkedTargets.Keys.ToArray())
            {
                if (target == null || target.IsDead())
                {
                    MarkedTargets.Remove(target);
                    continue;
                }

                float remaining = MarkedTargets[target] - dt;
                if (remaining <= 0f)
                {
                    RemoveWitchmarkDebuff(target);
                    MarkedTargets.Remove(target);
                }
                else
                {
                    MarkedTargets[target] = remaining;
                }
            }

            _witchmarkRefreshTimer -= dt;
            if (_witchmarkRefreshTimer <= 0f)
            {
                _witchmarkRefreshTimer = WitchmarkRefreshInterval;
                RefreshWitchmarkBuff(player);
            }
        }

        private static void TryArmInfusedBolt(Player player)
        {
            ItemDrop.ItemData weapon = GetCurrentSolomonKaneCrossbow(player);
            if (weapon == null)
            {
                ShowMessage(player, "Infused Bolt: equipa la ballesta Solomon Kane.");
                return;
            }

            if (_infusedBoltArmed)
            {
                ShowMessage(player, "Infused Bolt: el siguiente virote ya esta cargado.");
                return;
            }

            if (!TrySpendStaminaAndCooldown(player, "Infused Bolt", EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltStaminaUse.Value, _infusedBoltCooldown))
            {
                return;
            }

            _infusedBoltCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltCooldown.Value);
            _infusedBoltRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltDuration.Value);
            _infusedBoltArmed = true;
            AbilityCooldownBuffController.Start(player, "SolomonKaneInfusedBolt", "Infused Bolt", _infusedBoltCooldown, GetItemIcon(weapon));
            RefreshInfusedBoltBuff(player, weapon);
            NorseVisualEffectBridge.SpawnAttached(player, Vector3.up * 1.1f, Quaternion.identity, 1.5f, "InfusedArrow", "Infused", "Holy", "Spirit", "Frost");
        }

        private static void TryBlackpowderBomb(Player player)
        {
            ItemDrop.ItemData weapon = GetCurrentSolomonKaneCrossbow(player);
            if (weapon == null)
            {
                ShowMessage(player, "Blackpowder Bomb: equipa la ballesta Solomon Kane.");
                return;
            }

            if (!TrySpendStaminaAndCooldown(player, "Blackpowder Bomb", EpicLootRaritySetsPlugin.SolomonKaneBombStaminaUse.Value, _bombCooldown))
            {
                return;
            }

            Vector3 targetPoint;
            if (!TryFindAimPoint(player, EpicLootRaritySetsPlugin.SolomonKaneBombRange.Value, out targetPoint))
            {
                ShowMessage(player, "Blackpowder Bomb: sin objetivo.");
                return;
            }

            DamageArea(player, weapon, targetPoint, EpicLootRaritySetsPlugin.SolomonKaneBombRadius.Value, CreateBombDamage(player, weapon), EpicLootRaritySetsPlugin.SolomonKaneBombImpactForce.Value);
            _bombCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.SolomonKaneBombCooldown.Value);
            AbilityCooldownBuffController.Start(player, "SolomonKaneBlackpowderBomb", "Blackpowder Bomb", _bombCooldown, GetItemIcon(weapon));
            NorseVisualEffectBridge.Spawn(targetPoint, Quaternion.identity, 4f, "FxBomb", "Bomb", "Explosion", "Fire", "Blast");
        }

        private static void TryBatForm(Player player)
        {
            if (player == null)
            {
                return;
            }

            if (_batFormActive)
            {
                EndBatForm(player, false);
                ShowMessage(player, "Bat Form: cancelada.");
                return;
            }

            GameObject batPrefab = GetBatPrefab();
            if (batPrefab == null)
            {
                ShowMessage(player, "Bat Form: prefab Bat no disponible.");
                LogBatVisualFailureOnce("Solomon Kane Bat Form could not resolve a Bat prefab.");
                return;
            }

            if (!TrySpendStaminaAndCooldown(player, "Bat Form", EpicLootRaritySetsPlugin.SolomonKaneBatFormStaminaUse.Value, _batFormCooldown))
            {
                return;
            }

            StartBatForm(player, batPrefab);
        }

        private static void StartBatForm(Player player, GameObject batPrefab)
        {
            if (player == null || batPrefab == null)
            {
                return;
            }

            _batSavedDebugFly = GetDebugFly(player);
            _batSavedDebugFlyValid = true;
            _batSavedDebugFlySpeed = GetDebugFlySpeed();
            _batSavedLodVisible = GetLodVisible(player, true, out _batSavedLodVisibleValid);
            _batFormRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SolomonKaneBatFormDuration.Value);
            _batFormCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.SolomonKaneBatFormCooldown.Value);
            _batFormActive = true;

            SetPlayerVisible(player, false);
            SetDebugFlySpeed(Mathf.Max(1, Mathf.RoundToInt(EpicLootRaritySetsPlugin.SolomonKaneBatFormFlightSpeed.Value)));
            SetDebugFly(player, true);
            SpawnBatVisual(player, batPrefab);
            RefreshBatFormBuff(player);
            AbilityCooldownBuffController.Start(player, "SolomonKaneBatForm", "Bat Form", _batFormCooldown, FindSolomonIcon(player));
            NorseVisualEffectBridge.SpawnAtPlayer(player, "Bat", "Bats", "Smoke", "Dark", "Spirit");
            ShowMessage(player, "Bat Form: vuelo activo.");
        }

        private static void UpdateBatForm(Player player, float dt)
        {
            if (!_batFormActive)
            {
                return;
            }

            _batFormRemaining -= dt;
            if (_batFormRemaining <= 0f || player == null || player.IsDead() || player.IsTeleporting())
            {
                EndBatForm(player, false);
                return;
            }

            SetDebugFly(player, true);
            SetDebugFlySpeed(Mathf.Max(1, Mathf.RoundToInt(EpicLootRaritySetsPlugin.SolomonKaneBatFormFlightSpeed.Value)));
            SetPlayerVisible(player, false);
            MaintainBatVisual(player);
            RefreshBatFormBuff(player);
        }

        private static void EndBatForm(Player player, bool clearing)
        {
            bool hadBatState = _batFormActive || _batVisual != null || _batSavedDebugFlyValid || _batSavedLodVisibleValid;
            if (!hadBatState)
            {
                RemoveBatFormBuff(player);
                return;
            }

            DestroyBatVisual();
            RemoveBatFormBuff(player);

            if (player != null)
            {
                if (_batSavedLodVisibleValid)
                {
                    SetPlayerVisible(player, _batSavedLodVisible);
                }
                else
                {
                    SetPlayerVisible(player, true);
                }

                if (_batSavedDebugFlyValid)
                {
                    SetDebugFly(player, _batSavedDebugFly);
                }
                else
                {
                    SetDebugFly(player, false);
                }

                if (!_batSavedDebugFly)
                {
                    RestoreGravity(player);
                }
            }

            SetDebugFlySpeed(_batSavedDebugFlySpeed > 0 ? _batSavedDebugFlySpeed : 20);
            _batFormActive = false;
            _batFormRemaining = 0f;
            _batSavedDebugFlyValid = false;
            _batSavedLodVisibleValid = false;

            if (!clearing && player != null)
            {
                NorseVisualEffectBridge.SpawnAtPlayer(player, "Bat", "Bats", "Smoke", "Dark", "Spirit");
            }
        }

        private static void DetonateInfusedBolt(Player player, ItemDrop.ItemData weapon, Vector3 hitPoint)
        {
            if (player == null || weapon == null)
            {
                return;
            }

            DamageArea(player, weapon, hitPoint, EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltRadius.Value, CreateInfusedBoltDamage(player, weapon), 35f);
            ApplyInfusedBoltSlow(player, hitPoint);
            NorseVisualEffectBridge.Spawn(hitPoint, Quaternion.identity, 4f, "InfusedArrow", "Frost", "Spirit", "Holy", "Explosion");
        }

        private static void DamageArea(Player player, ItemDrop.ItemData weapon, Vector3 center, float radius, HitData.DamageTypes damages, float pushForce)
        {
            radius = Mathf.Max(0.1f, radius);
            Collider[] colliders = Physics.OverlapSphere(center, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            HashSet<Character> damaged = new HashSet<Character>();

            _abilityDamageDepth++;
            try
            {
                foreach (Collider collider in colliders)
                {
                    Character target = collider != null ? collider.GetComponentInParent<Character>() : null;
                    if (target == null || damaged.Contains(target) || target.IsDead() || !IsEnemyTarget(player, target))
                    {
                        continue;
                    }

                    damaged.Add(target);
                    HitData hit = CreateHitData(player, weapon, damages);
                    hit.m_point = center;
                    hit.m_dir = (target.GetCenterPoint() - center).normalized;
                    hit.m_pushForce = pushForce;
                    target.Damage(hit);
                }
            }
            finally
            {
                _abilityDamageDepth--;
            }
        }

        private static void ApplyInfusedBoltSlow(Player player, Vector3 center)
        {
            float radius = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltRadius.Value);
            Collider[] colliders = Physics.OverlapSphere(center, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            HashSet<Character> slowed = new HashSet<Character>();
            foreach (Collider collider in colliders)
            {
                Character target = collider != null ? collider.GetComponentInParent<Character>() : null;
                if (target == null || slowed.Contains(target) || target.IsDead() || !IsEnemyTarget(player, target))
                {
                    continue;
                }

                slowed.Add(target);
                ApplyInfusedBoltSlow(target);
            }
        }

        private static void ApplyInfusedBoltSlow(Character target)
        {
            StatusEffect buff = GetOrCreateInfusedSlowBuff();
            if (target == null || buff == null)
            {
                return;
            }

            SEMan seMan = target.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static void MarkTarget(Player player, ItemDrop.ItemData weapon, Character target)
        {
            if (player == null || target == null || target.IsDead())
            {
                return;
            }

            MarkedTargets[target] = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SolomonKaneWitchmarkDuration.Value);
            ApplyWitchmarkDebuff(target, weapon);
            RefreshWitchmarkBuff(player);
            NorseVisualEffectBridge.SpawnAttached(target.gameObject, Vector3.up * 1.2f, Quaternion.identity, 1.2f, "SenseCreatures", "Sense", "Mark", "Spirit", "Holy");
        }

        private static void ArmSilverVerdict(Player player, ItemDrop.ItemData weapon)
        {
            if (player == null)
            {
                return;
            }

            bool wasArmed = _silverVerdictArmed;
            _silverVerdictArmed = true;
            RefreshSilverVerdictBuff(player, weapon);
            if (!wasArmed)
            {
                ShowMessage(player, "Silver Verdict: siguiente virote sentenciado.");
                NorseVisualEffectBridge.SpawnAttached(player, Vector3.up * 1.2f, Quaternion.identity, 1.8f, "Holy", "Spirit", "Lightning", "Sense");
            }
        }

        private static void ChainSilverVerdict(PendingSolomonKaneHit pending)
        {
            if (pending == null || pending.Player == null || pending.Target == null)
            {
                return;
            }

            int maxTargets = Mathf.Max(0, EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictTargetCount.Value);
            if (maxTargets <= 0)
            {
                return;
            }

            Vector3 origin = GetHitPoint(pending.Target, pending.Hit);
            List<Character> targets = FindChainTargets(pending.Player, origin, pending.Target, maxTargets, EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictSeekRadius.Value);
            if (targets.Count == 0)
            {
                return;
            }

            HitData.DamageTypes baseDamage = pending.SilverDamage;
            if (GetTotalDamage(baseDamage) <= 0.01f)
            {
                baseDamage = CreateSilverVerdictDamage(pending.Player, pending.Weapon);
            }

            float remainingFactor = 1f - Mathf.Clamp01(EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictDamageLossPerJump.Value);
            _abilityDamageDepth++;
            try
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    Character target = targets[i];
                    if (target == null || target.IsDead())
                    {
                        continue;
                    }

                    HitData.DamageTypes chainDamage = baseDamage;
                    ScaleDamage(ref chainDamage, Mathf.Pow(remainingFactor, i + 1));
                    HitData chainHit = CreateHitData(pending.Player, pending.Weapon, chainDamage);
                    chainHit.m_point = origin;
                    chainHit.m_dir = (target.GetCenterPoint() - origin).normalized;
                    chainHit.m_pushForce = 20f;
                    target.Damage(chainHit);
                    NorseVisualEffectBridge.Spawn(target.GetCenterPoint(), Quaternion.identity, 2f, "Holy", "Spirit", "Lightning", "Sense");
                }
            }
            finally
            {
                _abilityDamageDepth--;
            }
        }

        private static List<Character> FindChainTargets(Player player, Vector3 origin, Character firstTarget, int maxTargets, float radius)
        {
            List<Character> targets = new List<Character>();
            if (player == null || maxTargets <= 0)
            {
                return targets;
            }

            radius = Mathf.Max(0.1f, radius);
            HashSet<Character> seen = new HashSet<Character>();
            if (firstTarget != null)
            {
                seen.Add(firstTarget);
            }

            Collider[] colliders = Physics.OverlapSphere(origin, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            foreach (Collider collider in colliders)
            {
                Character target = collider != null ? collider.GetComponentInParent<Character>() : null;
                if (target == null || seen.Contains(target) || target.IsDead() || !IsEnemyTarget(player, target))
                {
                    continue;
                }

                seen.Add(target);
                targets.Add(target);
            }

            targets.Sort((left, right) =>
                (left.GetCenterPoint() - origin).sqrMagnitude.CompareTo((right.GetCenterPoint() - origin).sqrMagnitude));
            if (targets.Count > maxTargets)
            {
                targets.RemoveRange(maxTargets, targets.Count - maxTargets);
            }

            return targets;
        }

        private static HitData.DamageTypes CreateInfusedBoltDamage(Player player, ItemDrop.ItemData weapon)
        {
            float amount = GetScaledCrossbowDamage(player, EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltBaseDamage.Value, EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltDamagePerCrossbowsLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_frost = amount * 0.65f,
                m_spirit = amount * 0.35f
            };
            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static HitData.DamageTypes CreateBombDamage(Player player, ItemDrop.ItemData weapon)
        {
            float amount = GetScaledCrossbowDamage(player, EpicLootRaritySetsPlugin.SolomonKaneBombBaseDamage.Value, EpicLootRaritySetsPlugin.SolomonKaneBombDamagePerCrossbowsLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_blunt = amount * 0.60f,
                m_fire = amount * 0.40f
            };
            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static HitData.DamageTypes CreateSilverVerdictDamage(Player player, ItemDrop.ItemData weapon)
        {
            float amount = GetScaledCrossbowDamage(player, EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictBaseDamage.Value, EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictDamagePerCrossbowsLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_spirit = amount
            };
            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static HitData CreateHitData(Player player, ItemDrop.ItemData weapon, HitData.DamageTypes damages)
        {
            HitData hit = new HitData
            {
                m_damage = damages,
                m_skill = Skills.SkillType.Crossbows,
                m_skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.Crossbows) : 0f,
                m_ranged = true,
                m_dodgeable = true,
                m_blockable = true,
                m_backstabBonus = 1f,
                m_staggerMultiplier = 1f
            };

            if (player != null)
            {
                hit.SetAttacker(player);
            }

            return hit;
        }

        private static float GetScaledCrossbowDamage(Player player, float baseDamage, float damagePerCrossbowsLevel)
        {
            float skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.Crossbows) : 0f;
            return Mathf.Max(0f, baseDamage + skillLevel * damagePerCrossbowsLevel);
        }

        private static void ApplyEpicLootDamageModifiers(Player player, ItemDrop.ItemData weapon, ref HitData.DamageTypes damages)
        {
            if (player == null || weapon == null || ApplyMagicDamageModifiersMethod == null)
            {
                return;
            }

            try
            {
                object[] args = { player, weapon, damages };
                ApplyMagicDamageModifiersMethod.Invoke(null, args);
                damages = (HitData.DamageTypes)args[2];
            }
            catch (Exception ex)
            {
                if (!_loggedEpicLootFailure && EpicLootRaritySetsPlugin.Log != null)
                {
                    _loggedEpicLootFailure = true;
                    EpicLootRaritySetsPlugin.Log.LogWarning("Could not apply EpicLoot damage modifiers to Solomon Kane ability damage. " + ex.GetBaseException().Message);
                }
            }
        }

        private static void AddDamage(ref HitData.DamageTypes target, HitData.DamageTypes add)
        {
            target.m_blunt += add.m_blunt;
            target.m_slash += add.m_slash;
            target.m_pierce += add.m_pierce;
            target.m_chop += add.m_chop;
            target.m_pickaxe += add.m_pickaxe;
            target.m_fire += add.m_fire;
            target.m_frost += add.m_frost;
            target.m_lightning += add.m_lightning;
            target.m_poison += add.m_poison;
            target.m_spirit += add.m_spirit;
        }

        private static void ScaleDamage(ref HitData.DamageTypes damages, float multiplier)
        {
            damages.m_blunt *= multiplier;
            damages.m_slash *= multiplier;
            damages.m_pierce *= multiplier;
            damages.m_chop *= multiplier;
            damages.m_pickaxe *= multiplier;
            damages.m_fire *= multiplier;
            damages.m_frost *= multiplier;
            damages.m_lightning *= multiplier;
            damages.m_poison *= multiplier;
            damages.m_spirit *= multiplier;
        }

        private static float GetTotalDamage(HitData.DamageTypes damages)
        {
            return damages.m_blunt + damages.m_slash + damages.m_pierce + damages.m_chop + damages.m_pickaxe +
                   damages.m_fire + damages.m_frost + damages.m_lightning + damages.m_poison + damages.m_spirit;
        }

        private static bool IsSolomonKaneCrossbow(ItemDrop.ItemData weapon)
        {
            if (weapon == null || weapon.m_shared == null || weapon.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Bow)
            {
                return false;
            }

            MagicItem magicItem = ItemDataExtensions.GetMagicItem(weapon);
            if (magicItem == null)
            {
                return false;
            }

            return IsSolomonKaneId(magicItem.SetID) || IsSolomonKaneId(magicItem.LegendaryID);
        }

        private static bool IsSolomonKaneId(string id)
        {
            return !string.IsNullOrEmpty(id) && id.IndexOf("SolomonKane", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static ItemDrop.ItemData GetCurrentSolomonKaneCrossbow(Player player)
        {
            ItemDrop.ItemData weapon = player != null ? player.GetCurrentWeapon() : null;
            return IsSolomonKaneCrossbow(weapon) ? weapon : null;
        }

        private static ItemDrop.ItemData GetProjectileWeapon(Player player)
        {
            if (_projectileHitDepth > 0 && _projectileOwner == player && IsSolomonKaneCrossbow(_projectileWeapon))
            {
                return _projectileWeapon;
            }

            return GetCurrentSolomonKaneCrossbow(player);
        }

        private static Character GetAttacker(HitData hit)
        {
            if (hit == null)
            {
                return null;
            }

            try
            {
                return hit.GetAttacker();
            }
            catch
            {
                return null;
            }
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player || target.IsTamed())
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        private static bool IsMarked(Character target)
        {
            return target != null && MarkedTargets.ContainsKey(target);
        }

        private static bool IsWeakSpotHit(HitData hit)
        {
            if (hit == null)
            {
                return false;
            }

            foreach (string fieldName in new[] { "m_weakSpot", "m_weakspot", "m_hitWeakSpot", "m_hitWeakspot" })
            {
                FieldInfo field = AccessTools.Field(typeof(HitData), fieldName);
                if (field == null)
                {
                    continue;
                }

                try
                {
                    object value = field.GetValue(hit);
                    if (value is bool && (bool)value)
                    {
                        return true;
                    }

                    if (value != null && !(value is bool) && Convert.ToInt32(value) > 0)
                    {
                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        private static Vector3 GetHitPoint(Character target, HitData hit)
        {
            if (hit != null && hit.m_point != Vector3.zero)
            {
                return hit.m_point;
            }

            return target != null ? target.GetCenterPoint() : Vector3.zero;
        }

        private static bool TryFindAimPoint(Player player, float range, out Vector3 targetPoint)
        {
            Transform originTransform = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            Vector3 origin = originTransform.position;
            Vector3 direction = originTransform.forward;

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, Mathf.Max(1f, range), Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                targetPoint = hit.point;
                return true;
            }

            targetPoint = origin + direction.normalized * Mathf.Max(1f, range);
            return true;
        }

        private static bool TrySpendStaminaAndCooldown(Player player, string abilityName, float staminaUse, float cooldown)
        {
            if (cooldown > 0f)
            {
                ShowMessage(player, string.Format("{0}: {1:0}s cooldown.", abilityName, cooldown));
                return false;
            }

            if (!TrySpendStamina(player, Mathf.Max(0f, staminaUse)))
            {
                ShowMessage(player, string.Format("{0}: falta vigor.", abilityName));
                return false;
            }

            return true;
        }

        private static bool TrySpendStamina(Player player, float amount)
        {
            if (player == null || amount <= 0f)
            {
                return true;
            }

            float currentStamina = InvokeFloat(GetStaminaMethod, player, -1f);
            if (currentStamina >= 0f && currentStamina < amount)
            {
                return false;
            }

            if (UseStaminaMethod != null)
            {
                try
                {
                    UseStaminaMethod.Invoke(player, new object[] { amount });
                    return true;
                }
                catch
                {
                }
            }

            if (StaminaField != null && currentStamina >= 0f)
            {
                try
                {
                    StaminaField.SetValue(player, Mathf.Max(0f, currentStamina - amount));
                    return true;
                }
                catch
                {
                }
            }

            return currentStamina < 0f || currentStamina >= amount;
        }

        private static void RefundStamina(Player player, float amount)
        {
            if (player == null || amount <= 0f)
            {
                return;
            }

            if (AddStaminaMethod != null)
            {
                try
                {
                    AddStaminaMethod.Invoke(player, new object[] { amount });
                    return;
                }
                catch
                {
                }
            }

            float currentStamina = InvokeFloat(GetStaminaMethod, player, -1f);
            if (StaminaField != null && currentStamina >= 0f)
            {
                float maxStamina = InvokeFloat(GetMaxStaminaMethod, player, -1f);
                float newStamina = currentStamina + amount;
                if (maxStamina > 0f)
                {
                    newStamina = Mathf.Min(maxStamina, newStamina);
                }

                try
                {
                    StaminaField.SetValue(player, newStamina);
                }
                catch
                {
                }
            }
        }

        private static float InvokeFloat(MethodInfo method, object target, float fallback)
        {
            if (method == null || target == null)
            {
                return fallback;
            }

            try
            {
                return Convert.ToSingle(method.Invoke(target, null));
            }
            catch
            {
                return fallback;
            }
        }

        private static bool CanReadAbilityInput(Player player)
        {
            return !player.IsDead() &&
                   !player.IsTeleporting() &&
                   !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> shortcutEntry)
        {
            if (shortcutEntry == null)
            {
                return false;
            }

            KeyboardShortcut shortcut = shortcutEntry.Value;
            if (shortcut.IsDown())
            {
                return true;
            }

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAnyMenuOpen()
        {
            if (InventoryGui.instance != null && InventoryGui.IsVisible())
            {
                return true;
            }

            if (Menu.instance != null && Menu.IsVisible())
            {
                return true;
            }

            if (TextInput.instance != null && TextInput.IsVisible())
            {
                return true;
            }

            if (Chat.instance != null && Chat.instance.HasFocus())
            {
                return true;
            }

            return Minimap.instance != null && Minimap.IsOpen();
        }

        private static bool IsEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableSolomonKaneAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableSolomonKaneAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool IsKnownAbilityProjectile(GameObject projectileObject)
        {
            string name = projectileObject != null ? projectileObject.name : string.Empty;
            return name.IndexOf("Acid", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Lightning", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Fireball", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Meteor", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Tornado", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Whirlwind", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Bomb", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static GameObject GetBatPrefab()
        {
            if (ZNetScene.instance == null)
            {
                return null;
            }

            foreach (string prefabName in new[] { "Bat", "bat", "Bat_TW", "GiantBat", "Bats" })
            {
                GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
                if (prefab != null)
                {
                    return prefab;
                }
            }

            return null;
        }

        private static void SpawnBatVisual(Player player, GameObject batPrefab)
        {
            DestroyBatVisual();
            if (player == null || batPrefab == null)
            {
                return;
            }

            try
            {
                _batVisual = UnityEngine.Object.Instantiate(batPrefab, player.transform);
                _batVisual.name = "FranSolomonKaneBatFormVisual";
                PrepareBatVisual(_batVisual);
                MaintainBatVisual(player);
            }
            catch (Exception ex)
            {
                _batVisual = null;
                LogBatVisualFailureOnce("Solomon Kane Bat Form visual spawn failed. " + ex.GetBaseException().Message);
            }
        }

        private static void PrepareBatVisual(GameObject visual)
        {
            if (visual == null)
            {
                return;
            }

            foreach (MonoBehaviour behaviour in visual.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour != null)
                {
                    behaviour.enabled = false;
                }
            }

            foreach (Collider collider in visual.GetComponentsInChildren<Collider>(true))
            {
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }

            foreach (Rigidbody body in visual.GetComponentsInChildren<Rigidbody>(true))
            {
                if (body == null)
                {
                    continue;
                }

                body.isKinematic = true;
                body.useGravity = false;
                body.detectCollisions = false;
            }

            float scale = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SolomonKaneBatFormVisualScale.Value);
            visual.transform.localScale = Vector3.one * scale;
            visual.SetActive(true);
        }

        private static void MaintainBatVisual(Player player)
        {
            if (player == null)
            {
                return;
            }

            if (_batVisual == null)
            {
                GameObject prefab = GetBatPrefab();
                if (prefab != null)
                {
                    SpawnBatVisual(player, prefab);
                }

                return;
            }

            _batVisual.transform.SetParent(player.transform, false);
            _batVisual.transform.position = player.GetCenterPoint() + Vector3.up * 0.05f;
            Vector3 look = player.GetLookDir();
            if (look.sqrMagnitude <= 0.001f)
            {
                look = player.transform.forward;
            }

            look.Normalize();
            _batVisual.transform.rotation = Quaternion.LookRotation(look, Vector3.up);
            float scale = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SolomonKaneBatFormVisualScale.Value);
            _batVisual.transform.localScale = Vector3.one * scale;
        }

        private static void DestroyBatVisual()
        {
            if (_batVisual != null)
            {
                UnityEngine.Object.Destroy(_batVisual);
                _batVisual = null;
            }
        }

        private static bool GetDebugFly(Player player)
        {
            if (player == null || DebugFlyField == null)
            {
                return false;
            }

            try
            {
                object value = DebugFlyField.GetValue(player);
                return value is bool && (bool)value;
            }
            catch
            {
                return false;
            }
        }

        private static void SetDebugFly(Player player, bool enabled)
        {
            if (player == null)
            {
                return;
            }

            if (DebugFlyField != null)
            {
                try
                {
                    DebugFlyField.SetValue(player, enabled);
                }
                catch
                {
                }
            }

            if (NViewField == null || ZdoVarsDebugFlyField == null)
            {
                return;
            }

            try
            {
                ZNetView view = NViewField.GetValue(player) as ZNetView;
                ZDO zdo = view != null && view.IsValid() ? view.GetZDO() : null;
                object hashValue = ZdoVarsDebugFlyField.GetValue(null);
                if (zdo != null && hashValue is int)
                {
                    zdo.Set((int)hashValue, enabled);
                }
            }
            catch
            {
            }
        }

        private static int GetDebugFlySpeed()
        {
            if (DebugFlySpeedField == null)
            {
                return 20;
            }

            try
            {
                return Mathf.Max(1, Convert.ToInt32(DebugFlySpeedField.GetValue(null)));
            }
            catch
            {
                return 20;
            }
        }

        private static void SetDebugFlySpeed(int speed)
        {
            if (DebugFlySpeedField == null)
            {
                return;
            }

            try
            {
                DebugFlySpeedField.SetValue(null, Mathf.Max(1, speed));
            }
            catch
            {
            }
        }

        private static bool GetLodVisible(Player player, bool fallback, out bool valid)
        {
            valid = false;
            if (player == null || LodVisibleField == null)
            {
                return fallback;
            }

            try
            {
                object value = LodVisibleField.GetValue(player);
                if (value is bool)
                {
                    valid = true;
                    return (bool)value;
                }
            }
            catch
            {
            }

            return fallback;
        }

        private static void SetPlayerVisible(Player player, bool visible)
        {
            if (player == null || SetVisibleMethod == null)
            {
                return;
            }

            try
            {
                SetVisibleMethod.Invoke(player, new object[] { visible });
            }
            catch
            {
            }
        }

        private static void RestoreGravity(Player player)
        {
            Rigidbody body = null;
            if (player != null && BodyField != null)
            {
                try
                {
                    body = BodyField.GetValue(player) as Rigidbody;
                }
                catch
                {
                }
            }

            if (body == null)
            {
                return;
            }

            body.useGravity = true;
            Vector3 velocity = body.velocity;
            if (velocity.y > 0f)
            {
                velocity.y = 0f;
                body.velocity = velocity;
            }
        }

        private static void RefreshBatFormBuff(Player player)
        {
            StatusEffect buff = GetOrCreateBatFormBuff(FindSolomonIcon(player));
            if (player == null || buff == null)
            {
                return;
            }

            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static StatusEffect GetOrCreateBatFormBuff(Sprite icon)
        {
            if (_batFormBuff == null)
            {
                _batFormBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _batFormBuff.name = BatFormBuffName;
                _batFormBuff.m_name = "Bat Form";
                _batFormBuff.m_category = BatFormBuffCategory;
                _batFormBuff.m_flashIcon = false;
                _batFormBuff.m_cooldownIcon = true;
                _batFormBuff.m_hidden = false;
            }

            _batFormBuff.m_ttl = Mathf.Max(0.1f, _batFormRemaining + 0.1f);
            _batFormBuff.m_tooltip = string.Format(
                "Forma de murcielago Solomon Kane.\n\nVuelo temporal activo.\nDuracion restante: {0:0.#}s.\nVelocidad base: {1:0.#}; mantener correr aumenta la velocidad.\nSaltar sube; agacharse baja.",
                Mathf.Max(0f, _batFormRemaining),
                EpicLootRaritySetsPlugin.SolomonKaneBatFormFlightSpeed.Value);
            if (icon != null)
            {
                _batFormBuff.m_icon = icon;
            }

            return _batFormBuff;
        }

        private static void RemoveBatFormBuff(Player player)
        {
            if (player != null && _batFormBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_batFormBuff.NameHash(), true);
            }
        }

        private static void LogBatVisualFailureOnce(string message)
        {
            if (_loggedBatVisualFailure || EpicLootRaritySetsPlugin.Log == null)
            {
                return;
            }

            _loggedBatVisualFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(message);
        }

        private static PendingSolomonKaneHit TakePendingHit(Character target, HitData hit)
        {
            for (int i = PendingHits.Count - 1; i >= 0; i--)
            {
                PendingSolomonKaneHit pending = PendingHits[i];
                if (pending == null || pending.Target == null || pending.Hit == null)
                {
                    PendingHits.RemoveAt(i);
                    continue;
                }

                if (object.ReferenceEquals(pending.Target, target) && object.ReferenceEquals(pending.Hit, hit))
                {
                    PendingHits.RemoveAt(i);
                    return pending;
                }
            }

            return null;
        }

        private static void TrimPendingHits()
        {
            while (PendingHits.Count > 64)
            {
                PendingHits.RemoveAt(0);
            }
        }

        private static void RefreshInfusedBoltBuff(Player player, ItemDrop.ItemData weapon)
        {
            StatusEffect buff = GetOrCreateInfusedBoltBuff(GetItemIcon(weapon));
            if (player == null || buff == null)
            {
                return;
            }

            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static StatusEffect GetOrCreateInfusedBoltBuff(Sprite icon)
        {
            if (_infusedBoltBuff == null)
            {
                _infusedBoltBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _infusedBoltBuff.name = InfusedBoltBuffName;
                _infusedBoltBuff.m_name = "Infused Bolt";
                _infusedBoltBuff.m_category = InfusedBoltBuffCategory;
                _infusedBoltBuff.m_flashIcon = false;
                _infusedBoltBuff.m_cooldownIcon = true;
                _infusedBoltBuff.m_hidden = false;
            }

            _infusedBoltBuff.m_ttl = Mathf.Max(0.1f, _infusedBoltRemaining + 0.1f);
            _infusedBoltBuff.m_tooltip = string.Format(
                "El siguiente virote Solomon Kane explotara al impactar.\n\nRadio: {0:0.#}m.\nDano: {1:0.#} + {2:0.##} por nivel de Ballestas, repartido entre frost y espiritu.\nSlow: {3:0.#}% durante {4:0.#}s.",
                EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltRadius.Value,
                EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltBaseDamage.Value,
                EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltDamagePerCrossbowsLevel.Value,
                EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltSlow.Value * 100f,
                EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltSlowDuration.Value);
            if (icon != null)
            {
                _infusedBoltBuff.m_icon = icon;
            }

            return _infusedBoltBuff;
        }

        private static void RefreshSilverVerdictBuff(Player player, ItemDrop.ItemData weapon)
        {
            StatusEffect buff = GetOrCreateSilverVerdictBuff(GetItemIcon(weapon) ?? FindSolomonIcon(player));
            if (player == null || buff == null)
            {
                return;
            }

            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static StatusEffect GetOrCreateSilverVerdictBuff(Sprite icon)
        {
            if (_silverVerdictBuff == null)
            {
                _silverVerdictBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _silverVerdictBuff.name = SilverVerdictBuffName;
                _silverVerdictBuff.m_name = "Silver Verdict";
                _silverVerdictBuff.m_category = SilverVerdictBuffCategory;
                _silverVerdictBuff.m_ttl = 0f;
                _silverVerdictBuff.m_flashIcon = false;
                _silverVerdictBuff.m_cooldownIcon = false;
                _silverVerdictBuff.m_hidden = false;
            }

            _silverVerdictBuff.m_tooltip = string.Format(
                "Tu siguiente virote Solomon Kane suma dano de espiritu y encadena a enemigos cercanos.\n\nDano: {0:0.#} + {1:0.##} por nivel de Ballestas.\nObjetivos extra: {2} en {3:0.#}m.\nPerdida por salto: {4:0.#}%.\nDevuelve {5:0} vigor al consumirse.",
                EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictBaseDamage.Value,
                EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictDamagePerCrossbowsLevel.Value,
                EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictTargetCount.Value,
                EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictSeekRadius.Value,
                EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictDamageLossPerJump.Value * 100f,
                EpicLootRaritySetsPlugin.SolomonKaneSilverVerdictStaminaRefund.Value);
            if (icon != null)
            {
                _silverVerdictBuff.m_icon = icon;
            }

            return _silverVerdictBuff;
        }

        private static void RefreshWitchmarkBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            if (MarkedTargets.Count <= 0)
            {
                RemoveWitchmarkBuff(player);
                return;
            }

            StatusEffect buff = GetOrCreateWitchmarkBuff(FindSolomonIcon(player));
            if (buff == null)
            {
                return;
            }

            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static StatusEffect GetOrCreateWitchmarkBuff(Sprite icon)
        {
            if (_witchmarkBuff == null)
            {
                _witchmarkBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _witchmarkBuff.name = WitchmarkBuffName;
                _witchmarkBuff.m_category = WitchmarkBuffCategory;
                _witchmarkBuff.m_flashIcon = false;
                _witchmarkBuff.m_cooldownIcon = false;
                _witchmarkBuff.m_hidden = false;
            }

            _witchmarkBuff.m_name = string.Format("Witchmark {0}", MarkedTargets.Count);
            _witchmarkBuff.m_ttl = 1.1f;
            _witchmarkBuff.m_tooltip = string.Format(
                "Enemigos marcados por impactos directos de ballesta Solomon Kane.\n\nMarcados activos: {0}.\nDuracion base: {1:0.#}s.\nRemata o golpea punto debil a un marcado para armar Silver Verdict.",
                MarkedTargets.Count,
                EpicLootRaritySetsPlugin.SolomonKaneWitchmarkDuration.Value);
            if (icon != null)
            {
                _witchmarkBuff.m_icon = icon;
            }

            return _witchmarkBuff;
        }

        private static void ApplyWitchmarkDebuff(Character target, ItemDrop.ItemData weapon)
        {
            StatusEffect buff = GetOrCreateWitchmarkDebuff(GetItemIcon(weapon));
            if (target == null || buff == null)
            {
                return;
            }

            SEMan seMan = target.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static StatusEffect GetOrCreateWitchmarkDebuff(Sprite icon)
        {
            if (_witchmarkDebuff == null)
            {
                _witchmarkDebuff = ScriptableObject.CreateInstance<SE_Stats>();
                _witchmarkDebuff.name = WitchmarkDebuffName;
                _witchmarkDebuff.m_name = "Witchmark";
                _witchmarkDebuff.m_category = WitchmarkDebuffCategory;
                _witchmarkDebuff.m_flashIcon = false;
                _witchmarkDebuff.m_cooldownIcon = true;
                _witchmarkDebuff.m_hidden = false;
            }

            _witchmarkDebuff.m_ttl = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SolomonKaneWitchmarkDuration.Value);
            _witchmarkDebuff.m_tooltip = "Marcado por Solomon Kane. Un remate o punto debil prepara Silver Verdict.";
            if (icon != null)
            {
                _witchmarkDebuff.m_icon = icon;
            }

            return _witchmarkDebuff;
        }

        private static StatusEffect GetOrCreateInfusedSlowBuff()
        {
            if (_infusedSlowBuff == null)
            {
                _infusedSlowBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _infusedSlowBuff.name = InfusedSlowBuffName;
                _infusedSlowBuff.m_name = "Infused Slow";
                _infusedSlowBuff.m_category = InfusedSlowBuffCategory;
                _infusedSlowBuff.m_flashIcon = false;
                _infusedSlowBuff.m_cooldownIcon = true;
                _infusedSlowBuff.m_hidden = false;
            }

            float slow = Mathf.Clamp01(EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltSlow.Value);
            _infusedSlowBuff.m_ttl = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SolomonKaneInfusedBoltSlowDuration.Value);
            _infusedSlowBuff.m_tooltip = string.Format("Ralentizado por Infused Bolt.\n\nVelocidad: -{0:0.#}%.\nDuracion: {1:0.#}s.", slow * 100f, _infusedSlowBuff.m_ttl);
            if (SpeedModifierField != null)
            {
                try
                {
                    SpeedModifierField.SetValue(_infusedSlowBuff, -slow);
                }
                catch
                {
                }
            }

            return _infusedSlowBuff;
        }

        private static void RemoveInfusedBoltBuff(Player player)
        {
            if (player != null && _infusedBoltBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_infusedBoltBuff.NameHash(), true);
            }
        }

        private static void RemoveSilverVerdictBuff(Player player)
        {
            if (player != null && _silverVerdictBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_silverVerdictBuff.NameHash(), true);
            }
        }

        private static void RemoveWitchmarkBuff(Player player)
        {
            if (player != null && _witchmarkBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_witchmarkBuff.NameHash(), true);
            }
        }

        private static void RemoveMarkedTargetDebuffs()
        {
            foreach (Character target in MarkedTargets.Keys.ToArray())
            {
                RemoveWitchmarkDebuff(target);
            }
        }

        private static void RemoveWitchmarkDebuff(Character target)
        {
            if (target != null && _witchmarkDebuff != null)
            {
                target.GetSEMan().RemoveStatusEffect(_witchmarkDebuff.NameHash(), true);
            }
        }

        private static Sprite FindSolomonIcon(Player player)
        {
            ItemDrop.ItemData weapon = player != null ? player.GetCurrentWeapon() : null;
            Sprite weaponIcon = GetItemIcon(weapon);
            if (weaponIcon != null)
            {
                return weaponIcon;
            }

            Inventory inventory = player != null ? player.GetInventory() : null;
            if (inventory == null)
            {
                return null;
            }

            foreach (ItemDrop.ItemData item in inventory.GetEquippedItems())
            {
                if (item == null || item.m_shared == null || item.m_shared.m_icons == null || item.m_shared.m_icons.Length == 0)
                {
                    continue;
                }

                MagicItem magicItem = ItemDataExtensions.GetMagicItem(item);
                if (magicItem != null && (IsSolomonKaneId(magicItem.SetID) || IsSolomonKaneId(magicItem.LegendaryID)))
                {
                    return item.m_shared.m_icons[0];
                }
            }

            return null;
        }

        private static Sprite GetItemIcon(ItemDrop.ItemData item)
        {
            if (item == null || item.m_shared == null || item.m_shared.m_icons == null || item.m_shared.m_icons.Length == 0)
            {
                return null;
            }

            return item.m_shared.m_icons[0];
        }

        private static void ShowMessage(Player player, string message)
        {
            if (player != null)
            {
                player.Message(MessageHud.MessageType.Center, message, 0, null, false);
            }
        }

        private sealed class PendingSolomonKaneHit
        {
            internal readonly Player Player;
            internal readonly ItemDrop.ItemData Weapon;
            internal readonly Character Target;
            internal readonly HitData Hit;
            internal readonly bool WasMarked;
            internal readonly bool WasWeakSpot;
            internal readonly bool ConsumedSilverVerdict;
            internal readonly HitData.DamageTypes SilverDamage;

            internal PendingSolomonKaneHit(Player player, ItemDrop.ItemData weapon, Character target, HitData hit, bool wasMarked, bool wasWeakSpot, bool consumedSilverVerdict, HitData.DamageTypes silverDamage)
            {
                Player = player;
                Weapon = weapon;
                Target = target;
                Hit = hit;
                WasMarked = wasMarked;
                WasWeakSpot = wasWeakSpot;
                ConsumedSilverVerdict = consumedSilverVerdict;
                SilverDamage = silverDamage;
            }
        }
    }

    internal sealed class SolomonKaneProjectileMarker : MonoBehaviour
    {
        internal Player Owner;
        internal ItemDrop.ItemData Weapon;
        internal bool InfusedBolt;
    }

    internal static class NottAbilityController
    {
        private const string RequiredSet = "Nott";
        private const string SneakyBuffName = "FranNottSneaky";
        private const string SneakyBuffCategory = "FranNottSneaky";
        private const string WarpStrikeBuffName = "FranNottWarpStrike";
        private const string WarpStrikeBuffCategory = "FranNottWarpStrike";
        private const string HitSpeedBuffName = "FranNottShadowMomentum";
        private const string HitSpeedBuffCategory = "FranNottShadowMomentum";
        private const string PoisonPassiveBuffName = "FranNottPoisonEdge";
        private const string PoisonPassiveBuffCategory = "FranNottPoisonEdge";
        private const float SneakyRefreshInterval = 0.35f;
        private const float SneakyBuffTtl = 1f;
        private const float AggroSuppressInterval = 0.25f;

        private static readonly FieldInfo NoiseModifierField = AccessTools.Field(typeof(SE_Stats), "m_noiseModifier");
        private static readonly FieldInfo StealthModifierField = AccessTools.Field(typeof(SE_Stats), "m_stealthModifier");
        private static readonly FieldInfo SpeedModifierField = AccessTools.Field(typeof(SE_Stats), "m_speedModifier");
        private static readonly MethodInfo GetAllCharactersMethod = AccessTools.Method(typeof(Character), "GetAllCharacters", Type.EmptyTypes);
        private static readonly FieldInfo BaseAITargetCreatureField = AccessTools.Field(typeof(BaseAI), "m_targetCreature");
        private static readonly FieldInfo BaseAITargetStaticField = AccessTools.Field(typeof(BaseAI), "m_targetStatic");
        private static readonly FieldInfo BaseAIAlertedField = AccessTools.Field(typeof(BaseAI), "m_alerted");
        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly MethodInfo GetStaminaMethod =
            AccessTools.Method(typeof(Player), "GetStamina", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "GetStamina", Type.EmptyTypes);
        private static readonly MethodInfo UseStaminaMethod =
            AccessTools.Method(typeof(Player), "UseStamina", new[] { typeof(float) }) ??
            AccessTools.Method(typeof(Character), "UseStamina", new[] { typeof(float) });
        private static readonly MethodInfo AddStaminaMethod =
            AccessTools.Method(typeof(Player), "AddStamina", new[] { typeof(float) }) ??
            AccessTools.Method(typeof(Character), "AddStamina", new[] { typeof(float) });
        private static readonly FieldInfo StaminaField =
            AccessTools.Field(typeof(Player), "m_stamina") ??
            AccessTools.Field(typeof(Character), "m_stamina");
        private static readonly MethodInfo IsCrouchingMethod =
            AccessTools.Method(typeof(Player), "IsCrouching", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "IsCrouching", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Player), "IsSneaking", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Character), "IsSneaking", Type.EmptyTypes);
        private static readonly FieldInfo CrouchingField =
            AccessTools.Field(typeof(Player), "m_crouching") ??
            AccessTools.Field(typeof(Character), "m_crouching") ??
            AccessTools.Field(typeof(Player), "m_crouch") ??
            AccessTools.Field(typeof(Character), "m_crouch");

        private static StatusEffect _sneakyBuff;
        private static StatusEffect _warpStrikeBuff;
        private static StatusEffect _hitSpeedBuff;
        private static StatusEffect _poisonPassiveBuff;
        private static float _sneakyRefreshTimer;
        private static float _aggroSuppressTimer;
        private static float _warpCooldown;
        private static float _hitSpeedTimer;
        private static int _hitSpeedStacks;
        private static bool _wasActive;
        private static bool _sneakyActive;
        private static bool _warpStrikeArmed;

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            _warpCooldown = Mathf.Max(0f, _warpCooldown - dt);
            UpdateHitSpeedBuff(player, dt);

            if (!IsNottEnabledAndActive())
            {
                if (_wasActive || _sneakyActive || _warpStrikeArmed || _hitSpeedStacks > 0)
                {
                    Clear(player);
                }
                return;
            }

            _wasActive = true;
            RefreshPoisonPassiveBuff(player);
            UpdatePassiveSneaky(player, dt);

            if (!CanReadAbilityInput(player))
            {
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.NottWarpHotkey))
            {
                TryWarp(player);
            }
        }

        internal static void Clear(Player player)
        {
            RemoveStatusEffects(player);
            _sneakyRefreshTimer = 0f;
            _aggroSuppressTimer = 0f;
            if (_sneakyActive)
            {
                NorseSneakyVisualBridge.Reset();
            }

            _sneakyActive = false;
            _warpStrikeArmed = false;
            _hitSpeedStacks = 0;
            _hitSpeedTimer = 0f;
            _wasActive = false;
        }

        internal static bool IsSneakyInvisibleTarget(Character character)
        {
            Player player = character as Player;
            return player != null &&
                   player == Player.m_localPlayer &&
                   _sneakyActive &&
                   IsNottEnabledAndActive();
        }

        internal static bool ShouldKeepSneakyVisual()
        {
            return _sneakyActive && IsNottEnabledAndActive();
        }

        internal static void OnCharacterDeath(Character deadCharacter)
        {
            Player player = Player.m_localPlayer;
            if (player == null || deadCharacter == null || deadCharacter == player || _warpCooldown <= 0f || !IsNottEnabledAndActive())
            {
                return;
            }

            float radius = Mathf.Max(1f, EpicLootRaritySetsPlugin.NottWarpResetRadius.Value);
            if ((deadCharacter.transform.position - player.transform.position).sqrMagnitude > radius * radius)
            {
                return;
            }

            if (!IsEnemyTarget(player, deadCharacter))
            {
                return;
            }

            _warpCooldown = 0f;
            AbilityCooldownBuffController.Clear(player, "NottWarp");
            RestoreStamina(player, Mathf.Max(0f, EpicLootRaritySetsPlugin.NottWarpStaminaUse.Value));
            ShowMessage(player, "Warp: cooldown reiniciado.");
        }

        internal static void TryConsumeWarpStrike(Character target, HitData hit)
        {
            if (!_warpStrikeArmed || target == null || hit == null)
            {
                return;
            }

            Character attacker = null;
            try
            {
                attacker = hit.GetAttacker();
            }
            catch
            {
            }

            Player player = attacker as Player;
            if (player == null || player != Player.m_localPlayer || target == player || !IsEnemyTarget(player, target) || !IsNottEnabledAndActive())
            {
                return;
            }

            float multiplier = Mathf.Max(1f, EpicLootRaritySetsPlugin.NottWarpDamageMultiplier.Value);
            MultiplyDamage(ref hit.m_damage, multiplier);
            _warpStrikeArmed = false;
            RemoveWarpStrikeBuff(player);
        }

        internal static void ApplyPoison(Character target, HitData hit)
        {
            if (target == null || hit == null || !IsNottEnabledAndActive())
            {
                return;
            }

            Character attacker = null;
            try
            {
                attacker = hit.GetAttacker();
            }
            catch
            {
            }

            Player player = attacker as Player;
            if (player == null || player != Player.m_localPlayer || target == player || !IsEnemyTarget(player, target))
            {
                return;
            }

            float skill = player.GetSkillLevel(Skills.SkillType.Knives);
            hit.m_damage.m_poison += Mathf.Max(0f, EpicLootRaritySetsPlugin.NottPoisonBaseDamage.Value + skill * EpicLootRaritySetsPlugin.NottPoisonDamagePerKnivesLevel.Value);
        }

        internal static void OnPlayerHit(Character target, HitData hit)
        {
            if (target == null || hit == null || !IsNottEnabledAndActive())
            {
                return;
            }

            Character attacker = null;
            try
            {
                attacker = hit.GetAttacker();
            }
            catch
            {
            }

            Player player = attacker as Player;
            if (player == null || player != Player.m_localPlayer || target == player || !IsEnemyTarget(player, target))
            {
                return;
            }

            _hitSpeedStacks = 1;
            _hitSpeedTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.NottHitSpeedDuration.Value);
            RefreshHitSpeedBuff(player);
        }

        private static void UpdateHitSpeedBuff(Player player, float dt)
        {
            if (_hitSpeedStacks <= 0)
            {
                return;
            }

            _hitSpeedTimer -= dt;
            if (_hitSpeedTimer > 0f && IsNottEnabledAndActive())
            {
                return;
            }

            _hitSpeedStacks = 0;
            _hitSpeedTimer = 0f;
            RemoveHitSpeedBuff(player);
        }

        private static void UpdatePassiveSneaky(Player player, float dt)
        {
            if (!IsSneaking(player))
            {
                RemoveSneakyBuff(player);
                NorseSneakyVisualBridge.Reset();
                _sneakyActive = false;
                _sneakyRefreshTimer = 0f;
                _aggroSuppressTimer = 0f;
                return;
            }

            _sneakyActive = true;
            NorseSneakyVisualBridge.Apply(player);
            _aggroSuppressTimer -= dt;
            if (_aggroSuppressTimer <= 0f)
            {
                _aggroSuppressTimer = AggroSuppressInterval;
                SuppressNearbyAggro(player);
            }

            _sneakyRefreshTimer -= dt;
            if (_sneakyRefreshTimer > 0f)
            {
                return;
            }

            _sneakyRefreshTimer = SneakyRefreshInterval;
            StatusEffect buff = GetOrCreateSneakyBuff(FindNottIcon(player));
            if (buff == null)
            {
                return;
            }

            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static void TryWarp(Player player)
        {
            if (_warpCooldown > 0f)
            {
                ShowMessage(player, string.Format("Warp: {0:0}s cooldown.", _warpCooldown));
                return;
            }

            float staminaUse = Mathf.Max(0f, EpicLootRaritySetsPlugin.NottWarpStaminaUse.Value);
            if (!TrySpendStamina(player, staminaUse))
            {
                ShowMessage(player, "Warp: no tienes vigor suficiente.");
                return;
            }

            string failureReason;
            if (!NorseWarpBridge.TrySafeWarp(player, EpicLootRaritySetsPlugin.NottWarpRange.Value, out failureReason))
            {
                ShowMessage(player, failureReason);
                return;
            }

            _warpCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.NottWarpCooldown.Value);
            AbilityCooldownBuffController.Start(player, "NottWarp", "Warp", _warpCooldown, null);
            _warpStrikeArmed = true;

            StatusEffect buff = GetOrCreateWarpStrikeBuff(FindNottIcon(player));
            if (buff != null)
            {
                SEMan seMan = player.GetSEMan();
                seMan.RemoveStatusEffect(buff.NameHash(), true);
                seMan.AddStatusEffect(buff, true, 1, 0f, 0);
            }

            ShowMessage(player, "Warp: siguiente ataque potenciado.");
        }

        private static StatusEffect GetOrCreateSneakyBuff(Sprite icon)
        {
            if (_sneakyBuff == null)
            {
                _sneakyBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _sneakyBuff.name = SneakyBuffName;
                _sneakyBuff.m_name = "Nott Sneaky";
                _sneakyBuff.m_category = SneakyBuffCategory;
                _sneakyBuff.m_flashIcon = false;
                _sneakyBuff.m_cooldownIcon = false;
                _sneakyBuff.m_hidden = false;
            }

            _sneakyBuff.m_ttl = SneakyBuffTtl;
            _sneakyBuff.m_tooltip = string.Format(
                "Pasiva del set Nott mientras estas agachado/en sigilo.\n\nInvisible para monstruos.\nVisual Sneaky de Ullr activo.\nRuido x{0:0.##}.\nDeteccion enemiga x{1:0.##}.\nVelocidad +{2:0.#}%.",
                EpicLootRaritySetsPlugin.NottSneakyNoiseModifier.Value,
                EpicLootRaritySetsPlugin.NottSneakyStealthModifier.Value,
                EpicLootRaritySetsPlugin.NottSneakySpeedModifier.Value * 100f);
            ApplySneakyStats(_sneakyBuff);
            if (icon != null)
            {
                _sneakyBuff.m_icon = icon;
            }

            return _sneakyBuff;
        }

        private static StatusEffect GetOrCreateWarpStrikeBuff(Sprite icon)
        {
            if (_warpStrikeBuff == null)
            {
                _warpStrikeBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _warpStrikeBuff.name = WarpStrikeBuffName;
                _warpStrikeBuff.m_name = "Warp Strike";
                _warpStrikeBuff.m_category = WarpStrikeBuffCategory;
                _warpStrikeBuff.m_flashIcon = false;
                _warpStrikeBuff.m_cooldownIcon = false;
                _warpStrikeBuff.m_hidden = false;
            }

            _warpStrikeBuff.m_ttl = 0f;
            _warpStrikeBuff.m_tooltip = string.Format(
                "El siguiente ataque contra un enemigo tras Warp pega x{0:0.##} dano.\n\nNo expira por tiempo; desaparece al golpear. Si un enemigo cercano muere mientras Warp esta en cooldown, el cooldown se reinicia y recuperas el vigor usado.",
                EpicLootRaritySetsPlugin.NottWarpDamageMultiplier.Value);
            if (icon != null)
            {
                _warpStrikeBuff.m_icon = icon;
            }

            return _warpStrikeBuff;
        }

        private static StatusEffect GetOrCreateHitSpeedBuff(Sprite icon)
        {
            if (_hitSpeedBuff == null)
            {
                _hitSpeedBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _hitSpeedBuff.name = HitSpeedBuffName;
                _hitSpeedBuff.m_name = "Shadow Momentum";
                _hitSpeedBuff.m_category = HitSpeedBuffCategory;
                _hitSpeedBuff.m_flashIcon = false;
                _hitSpeedBuff.m_cooldownIcon = true;
                _hitSpeedBuff.m_hidden = false;
            }

            float bonus = Mathf.Max(0f, EpicLootRaritySetsPlugin.NottHitSpeedBonus.Value);
            _hitSpeedBuff.m_ttl = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.NottHitSpeedDuration.Value);
            _hitSpeedBuff.m_tooltip = string.Format(
                "Golpear enemigos acelera a Nott.\n\nVelocidad: +{0:0.#}%.\nDuracion restante: {1:0.#}s.\nNo stackea; cada golpe refresca el tiempo.",
                bonus * 100f,
                Mathf.Max(0f, _hitSpeedTimer));
            SetFloatField(_hitSpeedBuff, SpeedModifierField, bonus);
            if (icon != null)
            {
                _hitSpeedBuff.m_icon = icon;
            }

            return _hitSpeedBuff;
        }

        private static StatusEffect GetOrCreatePoisonPassiveBuff(Sprite icon)
        {
            if (_poisonPassiveBuff == null)
            {
                _poisonPassiveBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _poisonPassiveBuff.name = PoisonPassiveBuffName;
                _poisonPassiveBuff.m_name = "Poison Edge";
                _poisonPassiveBuff.m_category = PoisonPassiveBuffCategory;
                _poisonPassiveBuff.m_flashIcon = false;
                _poisonPassiveBuff.m_cooldownIcon = false;
                _poisonPassiveBuff.m_hidden = false;
            }

            _poisonPassiveBuff.m_ttl = 1.5f;
            _poisonPassiveBuff.m_tooltip = string.Format(
                "Pasiva de Nott activa.\n\nCada golpe contra enemigos anade veneno = {0:0.#}+{1:0.##}/nivel de Cuchillos.",
                EpicLootRaritySetsPlugin.NottPoisonBaseDamage.Value,
                EpicLootRaritySetsPlugin.NottPoisonDamagePerKnivesLevel.Value);
            if (icon != null)
            {
                _poisonPassiveBuff.m_icon = icon;
            }

            return _poisonPassiveBuff;
        }

        private static void RefreshPoisonPassiveBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreatePoisonPassiveBuff(FindNottIcon(player));
            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 0, 0f, 0);
        }

        private static void RefreshHitSpeedBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateHitSpeedBuff(FindNottIcon(player));
            if (buff == null)
            {
                return;
            }

            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static void SuppressNearbyAggro(Player player)
        {
            if (player == null || GetAllCharactersMethod == null)
            {
                return;
            }

            IEnumerable<Character> characters = TryInvoke(GetAllCharactersMethod, null, null) as IEnumerable<Character>;
            if (characters == null)
            {
                return;
            }

            foreach (Character character in characters)
            {
                if (character == null || character == player)
                {
                    continue;
                }

                BaseAI baseAI = character.GetComponent<BaseAI>();
                if (baseAI == null)
                {
                    continue;
                }

                ClearTargetIfPlayer(baseAI, player);
            }
        }

        private static void ClearTargetIfPlayer(BaseAI baseAI, Player player)
        {
            if (baseAI == null || player == null)
            {
                return;
            }

            if (BaseAITargetCreatureField != null)
            {
                try
                {
                    if (object.ReferenceEquals(BaseAITargetCreatureField.GetValue(baseAI), player))
                    {
                        BaseAITargetCreatureField.SetValue(baseAI, null);
                    }
                }
                catch
                {
                }
            }

            if (BaseAITargetStaticField != null)
            {
                try
                {
                    BaseAITargetStaticField.SetValue(baseAI, null);
                }
                catch
                {
                }
            }

            if (BaseAIAlertedField != null)
            {
                try
                {
                    BaseAIAlertedField.SetValue(baseAI, false);
                }
                catch
                {
                }
            }
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player)
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        private static void MultiplyDamage(ref HitData.DamageTypes damage, float multiplier)
        {
            damage.m_damage *= multiplier;
            damage.m_blunt *= multiplier;
            damage.m_slash *= multiplier;
            damage.m_pierce *= multiplier;
            damage.m_chop *= multiplier;
            damage.m_pickaxe *= multiplier;
            damage.m_fire *= multiplier;
            damage.m_frost *= multiplier;
            damage.m_lightning *= multiplier;
            damage.m_poison *= multiplier;
            damage.m_spirit *= multiplier;
            damage.m_nonPlayer *= multiplier;
        }

        private static void ApplySneakyStats(StatusEffect buff)
        {
            SetFloatField(buff, NoiseModifierField, EpicLootRaritySetsPlugin.NottSneakyNoiseModifier.Value);
            SetFloatField(buff, StealthModifierField, EpicLootRaritySetsPlugin.NottSneakyStealthModifier.Value);
            SetFloatField(buff, SpeedModifierField, EpicLootRaritySetsPlugin.NottSneakySpeedModifier.Value);
        }

        private static bool TrySpendStamina(Player player, float amount)
        {
            if (player == null || amount <= 0f)
            {
                return true;
            }

            float current = GetStamina(player);
            if (current >= 0f && current < amount)
            {
                return false;
            }

            if (UseStaminaMethod != null)
            {
                try
                {
                    UseStaminaMethod.Invoke(player, new object[] { amount });
                    return true;
                }
                catch
                {
                }
            }

            if (StaminaField != null && current >= 0f)
            {
                try
                {
                    StaminaField.SetValue(player, Mathf.Max(0f, current - amount));
                    return true;
                }
                catch
                {
                }
            }

            return current < 0f || current >= amount;
        }

        private static float GetStamina(Player player)
        {
            if (player == null)
            {
                return -1f;
            }

            if (GetStaminaMethod != null)
            {
                try
                {
                    return Convert.ToSingle(GetStaminaMethod.Invoke(player, null));
                }
                catch
                {
                }
            }

            if (StaminaField != null)
            {
                try
                {
                    return Convert.ToSingle(StaminaField.GetValue(player));
                }
                catch
                {
                }
            }

            return -1f;
        }

        private static void RestoreStamina(Player player, float amount)
        {
            if (player == null || amount <= 0f)
            {
                return;
            }

            if (AddStaminaMethod != null)
            {
                try
                {
                    AddStaminaMethod.Invoke(player, new object[] { amount });
                    return;
                }
                catch
                {
                }
            }

            if (StaminaField != null)
            {
                try
                {
                    float current = GetStamina(player);
                    if (current >= 0f)
                    {
                        StaminaField.SetValue(player, current + amount);
                    }
                }
                catch
                {
                }
            }
        }

        private static void SetFloatField(object target, FieldInfo field, float value)
        {
            if (target == null || field == null)
            {
                return;
            }

            try
            {
                if (field.FieldType == typeof(float))
                {
                    field.SetValue(target, value);
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(target, Mathf.RoundToInt(value));
                }
            }
            catch
            {
            }
        }

        private static bool IsSneaking(Player player)
        {
            if (player == null)
            {
                return false;
            }

            bool result;
            if (TryInvokeBool(IsCrouchingMethod, player, out result))
            {
                return result;
            }

            if (CrouchingField != null)
            {
                try
                {
                    object value = CrouchingField.GetValue(player);
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            try
            {
                return ZInput.GetButton("Crouch");
            }
            catch
            {
                return false;
            }
        }

        private static bool TryInvokeBool(MethodInfo method, object target, out bool result)
        {
            result = false;
            if (method == null || target == null)
            {
                return false;
            }

            try
            {
                object value = method.Invoke(target, null);
                if (value is bool)
                {
                    result = (bool)value;
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        private static object TryInvoke(MethodInfo method, object target, object[] args)
        {
            if (method == null)
            {
                return null;
            }

            try
            {
                return method.Invoke(target, args);
            }
            catch
            {
                return null;
            }
        }

        private static bool IsNottEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableNottAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableNottAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool CanReadAbilityInput(Player player)
        {
            return !player.IsDead() &&
                   !player.IsTeleporting() &&
                   !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> shortcutEntry)
        {
            if (shortcutEntry == null)
            {
                return false;
            }

            KeyboardShortcut shortcut = shortcutEntry.Value;
            if (shortcut.IsDown())
            {
                return true;
            }

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAnyMenuOpen()
        {
            if (InventoryGui.instance != null && InventoryGui.IsVisible())
            {
                return true;
            }

            if (Menu.instance != null && Menu.IsVisible())
            {
                return true;
            }

            if (TextInput.instance != null && TextInput.IsVisible())
            {
                return true;
            }

            if (Chat.instance != null && Chat.instance.HasFocus())
            {
                return true;
            }

            return Minimap.instance != null && Minimap.IsOpen();
        }

        private static Sprite FindNottIcon(Player player)
        {
            Inventory inventory = player != null ? player.GetInventory() : null;
            if (inventory == null)
            {
                return null;
            }

            foreach (ItemDrop.ItemData item in inventory.GetEquippedItems())
            {
                if (item == null || item.m_shared == null || item.m_shared.m_icons == null || item.m_shared.m_icons.Length == 0)
                {
                    continue;
                }

                MagicItem magicItem = ItemDataExtensions.GetMagicItem(item);
                if (magicItem != null && (IsNottId(magicItem.SetID) || IsNottId(magicItem.LegendaryID)))
                {
                    return item.m_shared.m_icons[0];
                }
            }

            return null;
        }

        private static bool IsNottId(string id)
        {
            return !string.IsNullOrEmpty(id) && id.IndexOf("Nott", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void RemoveStatusEffects(Player player)
        {
            RemoveSneakyBuff(player);
            RemoveWarpStrikeBuff(player);
            RemoveHitSpeedBuff(player);
            RemovePoisonPassiveBuff(player);
        }

        private static void RemoveSneakyBuff(Player player)
        {
            if (player != null && _sneakyBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_sneakyBuff.NameHash(), true);
            }
        }

        private static void RemoveWarpStrikeBuff(Player player)
        {
            if (player != null && _warpStrikeBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_warpStrikeBuff.NameHash(), true);
            }
        }

        private static void RemoveHitSpeedBuff(Player player)
        {
            if (player != null && _hitSpeedBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_hitSpeedBuff.NameHash(), true);
            }
        }

        private static void RemovePoisonPassiveBuff(Player player)
        {
            if (player != null && _poisonPassiveBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_poisonPassiveBuff.NameHash(), true);
            }
        }

        private static void ShowMessage(Player player, string message)
        {
            if (player != null)
            {
                player.Message(MessageHud.MessageType.Center, message, 0, null, false);
            }
        }
    }

    internal static class MoonveinAbilityController
    {
        private const string RequiredSet = "Moonvein";
        private const string StackBuffName = "FranMoonveinCharge";
        private const string StackBuffCategory = "FranMoonveinStacks";
        private const string TornadoSlowBuffName = "FranMoonveinTornadoSlow";
        private const string TornadoSlowBuffCategory = "FranMoonveinTornadoSlow";
        private const float WhirlwindMinDuration = 5f;
        private const float WhirlwindVisualYOffset = 9f;

        private static readonly MethodInfo GetProjectileSpawnPointMethod = AccessTools.Method(typeof(Attack), "GetProjectileSpawnPoint");
        private static readonly MethodInfo ApplyMagicDamageModifiersMethod = AccessTools.Method(typeof(ModifyDamage), "ApplyMagicDamageModifiers");
        private static readonly FieldInfo ProjectileOwnerField = AccessTools.Field(typeof(Projectile), "m_owner");
        private static readonly FieldInfo ProjectileVelocityField = AccessTools.Field(typeof(Projectile), "m_vel");
        private static readonly FieldInfo SpeedModifierField = AccessTools.Field(typeof(SE_Stats), "m_speedModifier");
        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly List<ActiveTornado> ActiveTornadoes = new List<ActiveTornado>();
        private static readonly Dictionary<int, TornadoShotContext> ArmedTornadoProjectiles = new Dictionary<int, TornadoShotContext>();
        private static StatusEffect _stackBuff;
        private static StatusEffect _tornadoArmedBuff;
        private static StatusEffect _tornadoSlowBuff;
        private static int _stacks;
        private static float _meteorCooldown;
        private static float _tornadoCooldown;
        private static int _lastBowShotFrame = -1;
        private static bool _tornadoArmed;
        private static bool _loggedProjectileFailure;

        private enum MoonveinSpell
        {
            AcidBolt,
            LightningBolt,
            Fireball
        }

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            _meteorCooldown = Mathf.Max(0f, _meteorCooldown - dt);
            _tornadoCooldown = Mathf.Max(0f, _tornadoCooldown - dt);
            UpdateTornadoes(dt);

            if (!IsMoonveinEnabledAndActive())
            {
                Clear(player);
                ClearTornadoes();
                return;
            }

            if (!CanReadAbilityInput(player))
            {
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.MoonveinTornadoHotkey))
            {
                TryArmTornadoShot(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.MoonveinMeteorHotkey))
            {
                TryMeteor(player);
            }
        }

        internal static void Clear(Player player)
        {
            if (player != null && _stackBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_stackBuff.NameHash(), true);
            }

            if (player != null && _tornadoArmedBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_tornadoArmedBuff.NameHash(), true);
            }

            _stacks = 0;
            _tornadoArmed = false;
            ArmedTornadoProjectiles.Clear();
        }

        internal static void OnMoonveinBowShot(Player player, Attack attack, ItemDrop.ItemData weapon)
        {
            MagicItem magicItem;
            if (player == null || player != Player.m_localPlayer ||
                !IsMoonveinEnabledAndActive())
            {
                return;
            }

            if (!MoonveinBowEitrUse.IsMoonveinBow(weapon, out magicItem))
            {
                weapon = player.GetCurrentWeapon();
                if (!MoonveinBowEitrUse.IsMoonveinBow(weapon, out magicItem))
                {
                    return;
                }
            }

            if (_lastBowShotFrame == Time.frameCount)
            {
                return;
            }

            _lastBowShotFrame = Time.frameCount;

            AddStack(player, attack, weapon);
        }

        private static void AddStack(Player player, Attack attack, ItemDrop.ItemData weapon)
        {
            _stacks = Mathf.Clamp(_stacks + 1, 1, 3);

            if (_stacks >= 3)
            {
                CastStackSpell(player, attack, weapon, RollStackSpell());
                _stacks = 0;
            }

            RefreshStackBuff(player, weapon);
        }

        private static MoonveinSpell RollStackSpell()
        {
            float roll = UnityEngine.Random.value;
            if (roll < 0.4f)
            {
                return MoonveinSpell.AcidBolt;
            }

            if (roll < 0.8f)
            {
                return MoonveinSpell.LightningBolt;
            }

            return MoonveinSpell.Fireball;
        }

        private static bool IsMoonveinEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableMoonveinAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableMoonveinAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool CanReadAbilityInput(Player player)
        {
            return !player.IsDead() &&
                   !player.IsTeleporting() &&
                   !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> shortcutEntry)
        {
            if (shortcutEntry == null)
            {
                return false;
            }

            KeyboardShortcut shortcut = shortcutEntry.Value;
            if (shortcut.IsDown())
            {
                return true;
            }

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAnyMenuOpen()
        {
            if (InventoryGui.instance != null && InventoryGui.IsVisible())
            {
                return true;
            }

            if (Menu.instance != null && Menu.IsVisible())
            {
                return true;
            }

            if (TextInput.instance != null && TextInput.IsVisible())
            {
                return true;
            }

            if (Chat.instance != null && Chat.instance.HasFocus())
            {
                return true;
            }

            return Minimap.instance != null && Minimap.IsOpen();
        }

        private static void RefreshStackBuff(Player player, ItemDrop.ItemData weapon)
        {
            if (player == null)
            {
                Clear(player);
                return;
            }

            _stacks = Mathf.Clamp(_stacks, 0, 3);
            StatusEffect buff = GetOrCreateStackBuff(GetWeaponIcon(weapon));
            if (buff == null)
            {
                return;
            }

            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static StatusEffect GetOrCreateStackBuff(Sprite icon)
        {
            if (_stackBuff == null)
            {
                _stackBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _stackBuff.name = StackBuffName;
                _stackBuff.m_category = StackBuffCategory;
                _stackBuff.m_ttl = 0f;
                _stackBuff.m_flashIcon = false;
                _stackBuff.m_cooldownIcon = false;
                _stackBuff.m_hidden = false;
            }

            _stackBuff.m_name = string.Format("Moonvein {0}/3", _stacks);
            _stackBuff.m_tooltip = GetStackBuffTooltip();
            if (icon != null)
            {
                _stackBuff.m_icon = icon;
            }

            return _stackBuff;
        }

        private static string GetStackBuffTooltip()
        {
            return string.Format(
                "Moonvein esta acumulando disparos magicos.\n\nStacks: {0}/3.\nAl tercer disparo consecutivo lanza un hechizo aleatorio desde el arco y reinicia el contador.\n\nProbabilidades:\nAcid Bolt: 40%.\nLightning Bolt: 40%.\nFireball: 20%.\n\nEscalado:\nAcid Bolt veneno = {1:0.#} + {2:0.##} por nivel de Magia elemental.\nLightning Bolt rayo = {3:0.#} + {4:0.##} por nivel de Magia elemental.\nFireball fuego = {5:0.#} + {6:0.##} por nivel de Magia elemental.\nEl dano recibe modificadores de EpicLoot.",
                _stacks,
                EpicLootRaritySetsPlugin.MoonveinAcidBoltBaseDamage.Value,
                EpicLootRaritySetsPlugin.MoonveinAcidBoltDamagePerElementalMagicLevel.Value,
                EpicLootRaritySetsPlugin.MoonveinLightningBoltBaseDamage.Value,
                EpicLootRaritySetsPlugin.MoonveinLightningBoltDamagePerElementalMagicLevel.Value,
                EpicLootRaritySetsPlugin.MoonveinFireballBaseDamage.Value,
                EpicLootRaritySetsPlugin.MoonveinFireballDamagePerElementalMagicLevel.Value);
        }

        private static Sprite GetWeaponIcon(ItemDrop.ItemData weapon)
        {
            if (weapon == null || weapon.m_shared == null || weapon.m_shared.m_icons == null || weapon.m_shared.m_icons.Length == 0)
            {
                return null;
            }

            return weapon.m_shared.m_icons[0];
        }

        private static void CastStackSpell(Player player, Attack attack, ItemDrop.ItemData weapon, MoonveinSpell spell)
        {
            GameObject projectilePrefab = GetStackProjectilePrefab(spell);
            if (projectilePrefab == null)
            {
                ShowMessage(player, SpellDisplayName(spell) + ": projectile unavailable.");
                LogProjectileFailureOnce(SpellDisplayName(spell) + " projectile could not be resolved.");
                return;
            }

            Vector3 spawnPoint;
            Vector3 direction;
            GetProjectileSpawn(attack, player, out spawnPoint, out direction);

            HitData hit = CreateHitData(player, weapon, CreateStackDamage(player, weapon, spell));
            SpawnProjectile(projectilePrefab, player, weapon, spawnPoint, direction, EpicLootRaritySetsPlugin.MoonveinProjectileVelocity.Value, hit);
        }

        private static GameObject GetStackProjectilePrefab(MoonveinSpell spell)
        {
            if (spell == MoonveinSpell.AcidBolt)
            {
                return GetNorseProjectileByName("Acid", new[] { "FxAcidBolt", "AcidBolt", "projectile_acidbolt" });
            }

            if (spell == MoonveinSpell.LightningBolt)
            {
                return GetNorseProjectileByName("Lightning", new[] { "FxLightningBolt", "LightningBolt", "projectile_lightningbolt" });
            }

            if (spell == MoonveinSpell.Fireball)
            {
                GameObject fireball = GetAttackProjectileFromItem("StaffFireball");
                return fireball != null ? fireball : GetPrefab("fireball_projectile", "StaffFireball_projectile", "projectile_fireball");
            }

            return null;
        }

        private static GameObject GetNorseProjectileByName(string namePart, string[] fallbackPrefabNames)
        {
            Type cacheType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Cache");
            if (cacheType != null)
            {
                GameObject looseMatch = null;
                foreach (FieldInfo field in cacheType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (field == null || field.Name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    GameObject resolved = TryResolveProjectileField(field);
                    if (resolved == null)
                    {
                        continue;
                    }

                    if (IsProjectileLikeFieldName(field.Name))
                    {
                        return resolved;
                    }

                    if (looseMatch == null)
                    {
                        looseMatch = resolved;
                    }
                }

                if (looseMatch != null)
                {
                    return looseMatch;
                }
            }

            return GetPrefab(fallbackPrefabNames);
        }

        private static GameObject TryResolveProjectileField(FieldInfo field)
        {
            try
            {
                object value = field.GetValue(null);
                Attack attack = value as Attack;
                if (attack != null && attack.m_attackProjectile != null)
                {
                    return attack.m_attackProjectile;
                }

                return value as GameObject;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsProjectileLikeFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return false;
            }

            return fieldName.IndexOf("Bolt", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   fieldName.IndexOf("Attack", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   fieldName.IndexOf("Projectile", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static GameObject GetAttackProjectileFromItem(string itemPrefabName)
        {
            GameObject itemPrefab = null;
            if (ObjectDB.instance != null)
            {
                itemPrefab = ObjectDB.instance.GetItemPrefab(itemPrefabName);
            }

            if (itemPrefab == null)
            {
                itemPrefab = GetPrefab(itemPrefabName);
            }

            ItemDrop itemDrop = itemPrefab != null ? itemPrefab.GetComponent<ItemDrop>() : null;
            if (itemDrop == null || itemDrop.m_itemData == null || itemDrop.m_itemData.m_shared == null || itemDrop.m_itemData.m_shared.m_attack == null)
            {
                return null;
            }

            return itemDrop.m_itemData.m_shared.m_attack.m_attackProjectile;
        }

        private static GameObject GetPrefab(params string[] prefabNames)
        {
            if (prefabNames == null || ZNetScene.instance == null)
            {
                return null;
            }

            foreach (string prefabName in prefabNames)
            {
                if (string.IsNullOrEmpty(prefabName))
                {
                    continue;
                }

                GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
                if (prefab != null)
                {
                    return prefab;
                }
            }

            return null;
        }

        private static HitData.DamageTypes CreateStackDamage(Player player, ItemDrop.ItemData weapon, MoonveinSpell spell)
        {
            float amount;
            HitData.DamageTypes damages = new HitData.DamageTypes();

            if (spell == MoonveinSpell.AcidBolt)
            {
                amount = GetScaledDamage(player, EpicLootRaritySetsPlugin.MoonveinAcidBoltBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinAcidBoltDamagePerElementalMagicLevel.Value);
                damages.m_poison = amount;
            }
            else if (spell == MoonveinSpell.LightningBolt)
            {
                amount = GetScaledDamage(player, EpicLootRaritySetsPlugin.MoonveinLightningBoltBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinLightningBoltDamagePerElementalMagicLevel.Value);
                damages.m_lightning = amount;
            }
            else
            {
                amount = GetScaledDamage(player, EpicLootRaritySetsPlugin.MoonveinFireballBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinFireballDamagePerElementalMagicLevel.Value);
                damages.m_fire = amount;
            }

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static HitData.DamageTypes CreateMeteorDamage(Player player, ItemDrop.ItemData weapon)
        {
            float amount = GetScaledDamage(player, EpicLootRaritySetsPlugin.MoonveinMeteorBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinMeteorDamagePerElementalMagicLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_fire = amount,
                m_blunt = amount
            };

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static float GetScaledDamage(Player player, float baseDamage, float damagePerElementalMagicLevel)
        {
            float skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.ElementalMagic) : 0f;
            return Mathf.Max(0f, baseDamage + skillLevel * damagePerElementalMagicLevel);
        }

        private static void ApplyEpicLootDamageModifiers(Player player, ItemDrop.ItemData weapon, ref HitData.DamageTypes damages)
        {
            if (player == null || weapon == null)
            {
                return;
            }

            try
            {
                if (ApplyMagicDamageModifiersMethod == null)
                {
                    return;
                }

                object[] args = { player, weapon, damages };
                ApplyMagicDamageModifiersMethod.Invoke(null, args);
                damages = (HitData.DamageTypes)args[2];
            }
            catch (Exception ex)
            {
                LogProjectileFailureOnce("Could not apply EpicLoot damage modifiers to Moonvein ability damage. " + ex.GetBaseException().Message);
            }
        }

        private static HitData CreateHitData(Player player, ItemDrop.ItemData weapon, HitData.DamageTypes damages)
        {
            HitData hit = new HitData
            {
                m_damage = damages,
                m_skill = Skills.SkillType.ElementalMagic,
                m_skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.ElementalMagic) : 0f,
                m_ranged = true,
                m_dodgeable = true,
                m_blockable = true,
                m_backstabBonus = 1f,
                m_staggerMultiplier = 1f
            };

            if (player != null)
            {
                hit.SetAttacker(player);
            }

            return hit;
        }

        private static void GetProjectileSpawn(Attack attack, Player player, out Vector3 spawnPoint, out Vector3 direction)
        {
            spawnPoint = player.GetCenterPoint() + player.transform.forward * 0.8f;
            direction = player.GetAimDir(spawnPoint);

            if (attack == null || GetProjectileSpawnPointMethod == null)
            {
                return;
            }

            try
            {
                object[] args = { spawnPoint, direction };
                GetProjectileSpawnPointMethod.Invoke(attack, args);
                spawnPoint = (Vector3)args[0];
                direction = ((Vector3)args[1]).normalized;
            }
            catch
            {
                direction = player.GetAimDir(spawnPoint);
            }
        }

        private static void SpawnProjectile(GameObject projectilePrefab, Player player, ItemDrop.ItemData weapon, Vector3 spawnPoint, Vector3 direction, float velocity, HitData hit)
        {
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = player.transform.forward;
            }

            direction.Normalize();
            GameObject projectileObject = UnityEngine.Object.Instantiate(projectilePrefab, spawnPoint, Quaternion.LookRotation(direction));
            IProjectile projectile = projectileObject.GetComponent<IProjectile>();
            if (projectile == null)
            {
                projectile = projectileObject.GetComponentInChildren<IProjectile>();
            }

            if (projectile == null)
            {
                UnityEngine.Object.Destroy(projectileObject);
                LogProjectileFailureOnce("Moonvein projectile prefab has no IProjectile component: " + projectilePrefab.name);
                return;
            }

            projectile.Setup(player, direction * velocity, 0f, hit, weapon, null);
        }

        private static void TryMeteor(Player player)
        {
            ItemDrop.ItemData weapon = GetCurrentMoonveinBow(player);
            if (weapon == null)
            {
                ShowMessage(player, "Meteor: equipa el arco Moonvein.");
                return;
            }

            if (!TrySpendEitrAndCooldown(player, "Meteor", EpicLootRaritySetsPlugin.MoonveinMeteorEitrUse.Value, _meteorCooldown))
            {
                return;
            }

            GameObject meteorPrefab = GetPrefab("projectile_meteor");
            if (meteorPrefab == null)
            {
                ShowMessage(player, "Meteor: projectile unavailable.");
                LogProjectileFailureOnce("Moonvein meteor prefab projectile_meteor could not be resolved.");
                return;
            }

            Vector3 targetPoint;
            if (!TryFindAimPoint(player, EpicLootRaritySetsPlugin.MoonveinMeteorRange.Value, out targetPoint))
            {
                ShowMessage(player, "Meteor: sin objetivo.");
                return;
            }

            HitData hit = CreateHitData(player, weapon, CreateMeteorDamage(player, weapon));
            Vector3 spawnPoint = targetPoint + Vector3.up * 30f - player.transform.forward * 6f;
            Vector3 velocity = (targetPoint - spawnPoint).normalized * 30f;
            GameObject meteorObject = UnityEngine.Object.Instantiate(meteorPrefab, spawnPoint, Quaternion.LookRotation(velocity.normalized));

            IProjectile projectile = meteorObject.GetComponent<IProjectile>();
            if (projectile != null)
            {
                projectile.Setup(player, velocity, 0f, hit, weapon, null);
            }

            Projectile meteor = meteorObject.GetComponent<Projectile>();
            if (meteor != null)
            {
                if (ProjectileOwnerField != null)
                {
                    ProjectileOwnerField.SetValue(meteor, player);
                }

                meteor.m_damage = hit.m_damage;
                meteor.m_backstabBonus = hit.m_backstabBonus;
                meteor.m_attackForce = EpicLootRaritySetsPlugin.MoonveinMeteorImpactForce.Value;
                if (ProjectileVelocityField != null)
                {
                    ProjectileVelocityField.SetValue(meteor, velocity);
                }
            }

            player.UseEitr(EpicLootRaritySetsPlugin.MoonveinMeteorEitrUse.Value);
            _meteorCooldown = EpicLootRaritySetsPlugin.MoonveinMeteorCooldown.Value;
            AbilityCooldownBuffController.Start(player, "MoonveinMeteor", "Meteor", _meteorCooldown, GetWeaponIcon(weapon));
        }

        private static bool TryFindAimPoint(Player player, float range, out Vector3 targetPoint)
        {
            Transform originTransform = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            Vector3 origin = originTransform.position;
            Vector3 direction = originTransform.forward;

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                targetPoint = hit.point;
                return true;
            }

            targetPoint = origin + direction.normalized * range;
            return true;
        }

        private static void TryArmTornadoShot(Player player)
        {
            ItemDrop.ItemData weapon = GetCurrentMoonveinBow(player);
            if (weapon == null)
            {
                ShowMessage(player, "Tornado Shot: equipa el arco Moonvein.");
                return;
            }

            if (_tornadoArmed)
            {
                ShowMessage(player, "Tornado Shot: siguiente flecha cargada.");
                return;
            }

            if (!TrySpendEitrAndCooldown(player, "Tornado Shot", EpicLootRaritySetsPlugin.MoonveinTornadoEitrUse.Value, _tornadoCooldown))
            {
                return;
            }

            player.UseEitr(EpicLootRaritySetsPlugin.MoonveinTornadoEitrUse.Value);
            _tornadoCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.MoonveinTornadoCooldown.Value);
            AbilityCooldownBuffController.Start(player, "MoonveinTornadoShot", "Tornado Shot", _tornadoCooldown, GetWeaponIcon(weapon));
            _tornadoArmed = true;

            StatusEffect buff = GetOrCreateTornadoArmedBuff(GetWeaponIcon(weapon));
            if (buff != null)
            {
                SEMan seMan = player.GetSEMan();
                seMan.RemoveStatusEffect(buff.NameHash(), true);
                seMan.AddStatusEffect(buff, true, 1, 0f, 0);
            }
        }

        internal static void TryMarkTornadoProjectile(Projectile projectile, Player player, ItemDrop.ItemData weapon)
        {
            if (!_tornadoArmed || projectile == null || player == null || player != Player.m_localPlayer || !IsMoonveinEnabledAndActive())
            {
                return;
            }

            MagicItem magicItem;
            if (!MoonveinBowEitrUse.IsMoonveinBow(weapon, out magicItem))
            {
                weapon = player.GetCurrentWeapon();
                if (!MoonveinBowEitrUse.IsMoonveinBow(weapon, out magicItem))
                {
                    return;
                }
            }

            if (IsMoonveinAbilityProjectile(projectile.gameObject))
            {
                return;
            }

            int projectileId = projectile.GetInstanceID();
            ArmedTornadoProjectiles[projectileId] = new TornadoShotContext(player, weapon);
            _tornadoArmed = false;
            if (_tornadoArmedBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_tornadoArmedBuff.NameHash(), true);
            }

            ShowMessage(player, "Tornado Shot: flecha cargada.");
        }

        internal static void OnTornadoProjectileHit(Projectile projectile, Vector3 hitPoint)
        {
            if (projectile == null)
            {
                return;
            }

            TornadoShotContext context;
            int projectileId = projectile.GetInstanceID();
            if (!ArmedTornadoProjectiles.TryGetValue(projectileId, out context))
            {
                return;
            }

            ArmedTornadoProjectiles.Remove(projectileId);
            SpawnTornado(context.Player, context.Weapon, hitPoint);
        }

        private static StatusEffect GetOrCreateTornadoArmedBuff(Sprite icon)
        {
            if (_tornadoArmedBuff == null)
            {
                _tornadoArmedBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _tornadoArmedBuff.name = "FranMoonveinTornadoShot";
                _tornadoArmedBuff.m_name = "Tornado Shot";
                _tornadoArmedBuff.m_category = "FranMoonveinTornadoShot";
                _tornadoArmedBuff.m_flashIcon = false;
                _tornadoArmedBuff.m_cooldownIcon = false;
                _tornadoArmedBuff.m_hidden = false;
                _tornadoArmedBuff.m_ttl = 0f;
            }

            _tornadoArmedBuff.m_tooltip = string.Format(
                "La siguiente flecha Moonvein invocara el tornado de Njord en el punto de impacto.\n\nCoste: {0:0} eitr.\nCD: {1:0}s.\nFallback propio si Norse no puede crearla: rayo cada {2:0.##}s durante {3:0.#}s, radio {4:0.#}.\nEscala: {5:0.#} + {6:0.##} por nivel de Magia elemental.",
                EpicLootRaritySetsPlugin.MoonveinTornadoEitrUse.Value,
                EpicLootRaritySetsPlugin.MoonveinTornadoCooldown.Value,
                EpicLootRaritySetsPlugin.MoonveinTornadoTickInterval.Value,
                EpicLootRaritySetsPlugin.MoonveinTornadoDuration.Value,
                EpicLootRaritySetsPlugin.MoonveinTornadoRadius.Value,
                EpicLootRaritySetsPlugin.MoonveinTornadoBaseDamage.Value,
                EpicLootRaritySetsPlugin.MoonveinTornadoDamagePerElementalMagicLevel.Value);
            if (icon != null)
            {
                _tornadoArmedBuff.m_icon = icon;
            }

            return _tornadoArmedBuff;
        }

        private static bool IsMoonveinAbilityProjectile(GameObject projectileObject)
        {
            string name = projectileObject != null ? projectileObject.name : string.Empty;
            return name.IndexOf("Acid", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Lightning", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Fireball", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Meteor", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Tornado", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Whirlwind", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void SpawnTornado(Player player, ItemDrop.ItemData weapon, Vector3 position)
        {
            object norseAbility;
            GameObject norseEffectObject;
            string failureReason;
            if (NorseNjordTornadoBridge.TrySpawn(player, position, out norseAbility, out norseEffectObject, out failureReason))
            {
                ActiveTornadoes.Add(new ActiveTornado(player, weapon, position, norseEffectObject, norseAbility));
                return;
            }

            if (!string.IsNullOrEmpty(failureReason))
            {
                LogProjectileFailureOnce(failureReason + " Falling back to Moonvein custom tornado damage area.");
            }

            GameObject effectPrefab = GetNorseEffectPrefabByName("Tornado");
            GameObject effectObject = null;
            if (effectPrefab != null)
            {
                effectObject = UnityEngine.Object.Instantiate(effectPrefab, position + Vector3.up * WhirlwindVisualYOffset, Quaternion.identity);
                KeepWhirlwindVisualAlive(effectObject);
            }
            else
            {
                LogProjectileFailureOnce("Moonvein tornado visual prefab could not be resolved; damage area will still be created.");
            }

            ActiveTornadoes.Add(new ActiveTornado(player, weapon, position, effectObject));
        }

        private static GameObject GetNorseEffectPrefabByName(string namePart)
        {
            GameObject resourceValue = GetNorseStaticGameObjectByName("NorseDemigods.Resources", namePart);
            if (resourceValue != null)
            {
                return resourceValue;
            }

            GameObject cacheValue = GetNorseStaticGameObjectByName("NorseDemigods.Cache", namePart);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            return GetPrefab("FxWindWhirlwind", "WindWhirlwind", "fx_wind_whirlwind", "vfx_wind_whirlwind", "tornadoFx", "Tornado", "fx_tornado", "vfx_tornado");
        }

        private static Vector3 GetWhirlwindVisualPosition(Vector3 position)
        {
            return position + Vector3.up * WhirlwindVisualYOffset;
        }

        private static void RefreshWhirlwindVisual(ActiveTornado tornado)
        {
            if (tornado == null)
            {
                return;
            }

            Vector3 visualPosition = GetWhirlwindVisualPosition(tornado.Position);
            if (tornado.EffectObject == null)
            {
                GameObject effectPrefab = GetNorseEffectPrefabByName("Tornado");
                if (effectPrefab != null)
                {
                    tornado.EffectObject = UnityEngine.Object.Instantiate(effectPrefab, visualPosition, Quaternion.identity);
                }
            }

            if (tornado.EffectObject != null)
            {
                tornado.EffectObject.transform.position = visualPosition;
                tornado.EffectObject.transform.rotation = Quaternion.identity;
                KeepWhirlwindVisualAlive(tornado.EffectObject);
            }
        }

        private static void KeepWhirlwindVisualAlive(GameObject effectObject)
        {
            if (effectObject == null)
            {
                return;
            }

            effectObject.SetActive(true);
            foreach (MonoBehaviour behaviour in effectObject.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour == null)
                {
                    continue;
                }

                string typeName = behaviour.GetType().Name;
                if (typeName.IndexOf("TimedDestruction", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    typeName.IndexOf("TimedDestroy", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    behaviour.enabled = false;
                }
            }
        }

        private static GameObject GetNorseStaticGameObjectByName(string typeName, string namePart)
        {
            Type type = NorseDemigodsSuppression.FindNorseType(typeName);
            if (type == null)
            {
                return null;
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field == null || field.FieldType != typeof(GameObject) ||
                    field.Name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                try
                {
                    GameObject value = field.GetValue(null) as GameObject;
                    if (value != null)
                    {
                        return value;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        private static void UpdateTornadoes(float dt)
        {
            for (int i = ActiveTornadoes.Count - 1; i >= 0; i--)
            {
                ActiveTornado tornado = ActiveTornadoes[i];
                if (tornado == null)
                {
                    ActiveTornadoes.RemoveAt(i);
                    continue;
                }

                tornado.Remaining -= dt;
                tornado.SlowTickTimer -= dt;
                if (tornado.SlowTickTimer <= 0f)
                {
                    tornado.SlowTickTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.MoonveinTornadoTickInterval.Value);
                    ApplyTornadoSlow(tornado);
                }

                if (tornado.NorseAbility != null)
                {
                    if (!NorseNjordTornadoBridge.Update(tornado.NorseAbility, tornado.Position, dt))
                    {
                        DestroyTornado(tornado);
                        ActiveTornadoes.RemoveAt(i);
                        continue;
                    }
                }
                else
                {
                    tornado.TickTimer -= dt;
                    if (tornado.TickTimer <= 0f)
                    {
                        tornado.TickTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.MoonveinTornadoTickInterval.Value);
                        TickTornado(tornado);
                    }

                    RefreshWhirlwindVisual(tornado);
                }

                if (tornado.Remaining <= 0f)
                {
                    DestroyTornado(tornado);
                    ActiveTornadoes.RemoveAt(i);
                }
            }
        }

        private static void TickTornado(ActiveTornado tornado)
        {
            Player player = tornado.Player != null ? tornado.Player : Player.m_localPlayer;
            if (player == null)
            {
                return;
            }

            float radius = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.MoonveinTornadoRadius.Value);
            HitData.DamageTypes damages = CreateTornadoDamage(player, tornado.Weapon);
            HashSet<Character> hitCharacters = new HashSet<Character>();
            Collider[] colliders = Physics.OverlapSphere(tornado.Position, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            foreach (Collider collider in colliders)
            {
                Character target = collider != null ? collider.GetComponentInParent<Character>() : null;
                if (target == null || hitCharacters.Contains(target) || target.IsDead() || !IsEnemyTarget(player, target))
                {
                    continue;
                }

                hitCharacters.Add(target);
                HitData hit = CreateHitData(player, tornado.Weapon, damages);
                hit.m_pushForce = EpicLootRaritySetsPlugin.MoonveinTornadoImpactForce.Value;
                hit.m_point = tornado.Position;
                target.Damage(hit);
            }
        }

        private static void ApplyTornadoSlow(ActiveTornado tornado)
        {
            Player player = tornado != null && tornado.Player != null ? tornado.Player : Player.m_localPlayer;
            if (player == null || tornado == null)
            {
                return;
            }

            float radius = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.MoonveinTornadoRadius.Value);
            Collider[] colliders = Physics.OverlapSphere(tornado.Position, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            HashSet<Character> slowedCharacters = new HashSet<Character>();
            foreach (Collider collider in colliders)
            {
                Character target = collider != null ? collider.GetComponentInParent<Character>() : null;
                if (target == null || slowedCharacters.Contains(target) || target.IsDead() || !IsEnemyTarget(player, target))
                {
                    continue;
                }

                slowedCharacters.Add(target);
                ApplyTornadoSlow(target);
            }
        }

        private static void ApplyTornadoSlow(Character target)
        {
            StatusEffect buff = GetOrCreateTornadoSlowBuff();
            if (target == null || buff == null)
            {
                return;
            }

            SEMan seMan = target.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static StatusEffect GetOrCreateTornadoSlowBuff()
        {
            if (_tornadoSlowBuff == null)
            {
                _tornadoSlowBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _tornadoSlowBuff.name = TornadoSlowBuffName;
                _tornadoSlowBuff.m_name = "Tornado Slow";
                _tornadoSlowBuff.m_category = TornadoSlowBuffCategory;
                _tornadoSlowBuff.m_flashIcon = false;
                _tornadoSlowBuff.m_cooldownIcon = true;
                _tornadoSlowBuff.m_hidden = false;
            }

            float slow = Mathf.Clamp01(EpicLootRaritySetsPlugin.MoonveinTornadoSlow.Value);
            _tornadoSlowBuff.m_ttl = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.MoonveinTornadoSlowDuration.Value);
            _tornadoSlowBuff.m_tooltip = string.Format("Ralentizado por el tornado Moonvein.\n\nVelocidad: -{0:0.#}%.\nDuracion: {1:0.#}s.", slow * 100f, _tornadoSlowBuff.m_ttl);
            if (SpeedModifierField != null)
            {
                SpeedModifierField.SetValue(_tornadoSlowBuff, -slow);
            }

            return _tornadoSlowBuff;
        }

        private static HitData.DamageTypes CreateTornadoDamage(Player player, ItemDrop.ItemData weapon)
        {
            float amount = GetScaledDamage(player, EpicLootRaritySetsPlugin.MoonveinTornadoBaseDamage.Value, EpicLootRaritySetsPlugin.MoonveinTornadoDamagePerElementalMagicLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_lightning = amount
            };

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);
            return damages;
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player)
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        private static void ClearTornadoes()
        {
            foreach (ActiveTornado tornado in ActiveTornadoes)
            {
                if (tornado != null)
                {
                    DestroyTornado(tornado);
                }
            }

            ActiveTornadoes.Clear();
            ArmedTornadoProjectiles.Clear();
            _tornadoArmed = false;
        }

        private static void DestroyTornado(ActiveTornado tornado)
        {
            if (tornado == null)
            {
                return;
            }

            if (tornado.NorseAbility != null)
            {
                NorseNjordTornadoBridge.Stop(tornado.NorseAbility);
            }

            DestroyTornadoEffect(tornado.EffectObject);
        }

        private static void DestroyTornadoEffect(GameObject effectObject)
        {
            if (effectObject == null)
            {
                return;
            }

            if (ZNetScene.instance != null)
            {
                ZNetScene.instance.Destroy(effectObject);
            }
            else
            {
                UnityEngine.Object.Destroy(effectObject);
            }
        }

        private sealed class TornadoShotContext
        {
            internal readonly Player Player;
            internal readonly ItemDrop.ItemData Weapon;

            internal TornadoShotContext(Player player, ItemDrop.ItemData weapon)
            {
                Player = player;
                Weapon = weapon;
            }
        }

        private sealed class ActiveTornado
        {
            internal readonly Player Player;
            internal readonly ItemDrop.ItemData Weapon;
            internal readonly Vector3 Position;
            internal GameObject EffectObject;
            internal readonly object NorseAbility;
            internal float Remaining;
            internal float TickTimer;
            internal float SlowTickTimer;

            internal ActiveTornado(Player player, ItemDrop.ItemData weapon, Vector3 position, GameObject effectObject)
                : this(player, weapon, position, effectObject, null)
            {
            }

            internal ActiveTornado(Player player, ItemDrop.ItemData weapon, Vector3 position, GameObject effectObject, object norseAbility)
            {
                Player = player;
                Weapon = weapon;
                Position = position;
                EffectObject = effectObject;
                NorseAbility = norseAbility;
                Remaining = Mathf.Max(WhirlwindMinDuration, EpicLootRaritySetsPlugin.MoonveinTornadoDuration.Value);
                TickTimer = 0f;
                SlowTickTimer = 0f;
            }
        }

        private static ItemDrop.ItemData GetCurrentMoonveinBow(Player player)
        {
            if (player == null)
            {
                return null;
            }

            ItemDrop.ItemData weapon = player.GetCurrentWeapon();
            MagicItem magicItem;
            return MoonveinBowEitrUse.IsMoonveinBow(weapon, out magicItem) ? weapon : null;
        }

        private static bool TrySpendEitrAndCooldown(Player player, string abilityName, float eitrUse, float cooldown)
        {
            if (cooldown > 0f)
            {
                ShowMessage(player, string.Format("{0}: {1:0}s cooldown.", abilityName, cooldown));
                return false;
            }

            if (eitrUse > 0f && !player.HaveEitr(eitrUse))
            {
                if (Hud.instance != null)
                {
                    Hud.instance.EitrBarEmptyFlash();
                }

                ShowMessage(player, string.Format("{0}: no tienes eitr suficiente.", abilityName));
                return false;
            }

            return true;
        }

        private static string SpellDisplayName(MoonveinSpell spell)
        {
            if (spell == MoonveinSpell.AcidBolt)
            {
                return "Acid Bolt";
            }

            if (spell == MoonveinSpell.LightningBolt)
            {
                return "Lightning Bolt";
            }

            if (spell == MoonveinSpell.Fireball)
            {
                return "Fireball";
            }

            return "Moonvein";
        }

        private static void ShowMessage(Player player, string message)
        {
            if (player != null)
            {
                player.Message(MessageHud.MessageType.Center, message, 0, null, false);
            }
        }

        private static void LogProjectileFailureOnce(string message)
        {
            if (_loggedProjectileFailure)
            {
                return;
            }

            _loggedProjectileFailure = true;
            EpicLootRaritySetsPlugin.Log.LogWarning(message);
        }
    }

    internal static class HelveigAbilityController
    {
        private const string RequiredSet = "Helveig";
        private const string BloodRiteBuffName = "FranHelveigBloodRite";
        private const string BloodRiteBuffCategory = "FranHelveigBloodRite";
        private const string UndeadSummonBuffName = "FranHelveigUndeadSummon";
        private const string UndeadSummonBuffCategory = "FranHelveigUndeadSummon";

        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly MethodInfo ApplyMagicDamageModifiersMethod = AccessTools.Method(typeof(ModifyDamage), "ApplyMagicDamageModifiers");
        private static readonly MethodInfo TameMethod = AccessTools.Method(typeof(Tameable), "Tame");

        private static StatusEffect _bloodRiteBuff;
        private static StatusEffect _undeadSummonBuff;
        private static GameObject _undeadSummon;
        private static float _holyHealCooldown;
        private static float _holyStrikeCooldown;
        private static float _bloodRiteCooldown;
        private static float _undeadSummonCooldown;
        private static float _undeadSummonRemaining;
        private static float _bloodRiteRemaining;
        private static float _bloodRiteTickTimer;
        private static Vector3 _bloodRiteOrigin;
        private static bool _bloodRiteActive;
        private static string _undeadSummonDisplayName = "No muerto";

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            _holyHealCooldown = Mathf.Max(0f, _holyHealCooldown - dt);
            _holyStrikeCooldown = Mathf.Max(0f, _holyStrikeCooldown - dt);
            _bloodRiteCooldown = Mathf.Max(0f, _bloodRiteCooldown - dt);
            _undeadSummonCooldown = Mathf.Max(0f, _undeadSummonCooldown - dt);

            if (!IsHelveigEnabledAndActive())
            {
                Clear(player);
                return;
            }

            UpdateBloodRite(player, dt);
            UpdateUndeadSummon(player, dt);

            if (!CanReadAbilityInput(player))
            {
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.HelveigBloodRiteHotkey))
            {
                TryBloodRite(player);
                return;
            }

            if (SetAbilityInput.IsSecondaryAttackDown() ||
                IsShortcutDown(EpicLootRaritySetsPlugin.HelveigHolyStrikeHotkey))
            {
                TryHolyStrike(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.HelveigSummonUndeadHotkey) &&
                SetAbilityInput.IsBlockHeld())
            {
                TrySummonUndead(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.HelveigHolyHealHotkey))
            {
                TryHolyHeal(player);
            }
        }

        internal static void Clear(Player player)
        {
            _bloodRiteActive = false;
            _bloodRiteRemaining = 0f;
            _bloodRiteTickTimer = 0f;
            RemoveBloodRiteBuff(player);
            DestroyUndeadSummon(player);
        }

        private static void TryHolyHeal(Player player)
        {
            if (!TrySpendEitrAndCooldown(player, "Holy Heal", EpicLootRaritySetsPlugin.HelveigHolyHealEitrUse.Value, _holyHealCooldown))
            {
                return;
            }

            float healing = GetScaledHealing(player, EpicLootRaritySetsPlugin.HelveigHolyHealBaseHealing.Value, EpicLootRaritySetsPlugin.HelveigHolyHealHealingPerBloodMagicLevel.Value);
            player.UseEitr(EpicLootRaritySetsPlugin.HelveigHolyHealEitrUse.Value);
            player.Heal(healing, true);
            _holyHealCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.HelveigHolyHealCooldown.Value);
            AbilityCooldownBuffController.Start(player, "HelveigHolyHeal", "Holy Heal", _holyHealCooldown, null);
            ShowMessage(player, string.Format("Holy Heal: +{0:0} vida.", healing));
        }

        private static void TryHolyStrike(Player player)
        {
            if (!TrySpendEitrAndCooldown(player, "Holy Strike", EpicLootRaritySetsPlugin.HelveigHolyStrikeEitrUse.Value, _holyStrikeCooldown))
            {
                return;
            }

            Character target;
            if (!TryFindHolyStrikeTarget(player, EpicLootRaritySetsPlugin.HelveigHolyStrikeRange.Value, out target))
            {
                ShowMessage(player, "Holy Strike: no hay objetivo.");
                return;
            }

            player.UseEitr(EpicLootRaritySetsPlugin.HelveigHolyStrikeEitrUse.Value);
            HitData hit = CreateHolyStrikeHit(player, player.GetCurrentWeapon(), target);
            target.Damage(hit);
            _holyStrikeCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.HelveigHolyStrikeCooldown.Value);
            AbilityCooldownBuffController.Start(player, "HelveigHolyStrike", "Holy Strike", _holyStrikeCooldown, null);
            ShowMessage(player, "Holy Strike.");
        }

        private static void TryBloodRite(Player player)
        {
            if (_bloodRiteActive)
            {
                ShowMessage(player, "Blood Rite: canalizando.");
                return;
            }

            if (!TrySpendEitrAndCooldown(player, "Blood Rite", EpicLootRaritySetsPlugin.HelveigBloodRiteEitrUse.Value, _bloodRiteCooldown))
            {
                return;
            }

            player.UseEitr(EpicLootRaritySetsPlugin.HelveigBloodRiteEitrUse.Value);
            _bloodRiteRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HelveigBloodRiteDuration.Value);
            _bloodRiteTickTimer = 0f;
            _bloodRiteOrigin = player.transform.position;
            _bloodRiteActive = true;
            RefreshBloodRiteBuff(player);
            ShowMessage(player, "Blood Rite: canalizando curacion.");
        }

        private static void UpdateBloodRite(Player player, float dt)
        {
            if (!_bloodRiteActive)
            {
                return;
            }

            if (player.IsDead())
            {
                CancelBloodRite(player, false);
                return;
            }

            float cancelDistance = Mathf.Max(0f, EpicLootRaritySetsPlugin.HelveigBloodRiteCancelMoveDistance.Value);
            if (cancelDistance > 0f && (player.transform.position - _bloodRiteOrigin).sqrMagnitude > cancelDistance * cancelDistance)
            {
                CancelBloodRite(player, true);
                return;
            }

            player.StopMovement();
            _bloodRiteRemaining -= dt;
            _bloodRiteTickTimer -= dt;
            if (_bloodRiteTickTimer <= 0f)
            {
                _bloodRiteTickTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HelveigBloodRiteTickInterval.Value);
                HealAlliesInBloodRite(player);
                RefreshBloodRiteBuff(player);
            }

            if (_bloodRiteRemaining <= 0f)
            {
                _bloodRiteActive = false;
                StartBloodRiteCooldown(player);
                RemoveBloodRiteBuff(player);
                ShowMessage(player, "Blood Rite: completado.");
            }
        }

        private static void CancelBloodRite(Player player, bool showMessage)
        {
            _bloodRiteActive = false;
            _bloodRiteRemaining = 0f;
            _bloodRiteTickTimer = 0f;
            StartBloodRiteCooldown(player);
            RemoveBloodRiteBuff(player);
            if (showMessage)
            {
                ShowMessage(player, "Blood Rite: cancelado al moverte.");
            }
        }

        private static void StartBloodRiteCooldown(Player player)
        {
            _bloodRiteCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.HelveigBloodRiteCooldown.Value);
            AbilityCooldownBuffController.Start(player, "HelveigBloodRite", "Blood Rite", _bloodRiteCooldown, null);
        }

        private static void TrySummonUndead(Player player)
        {
            if (HasLivingUndeadSummon())
            {
                ShowMessage(player, "Summon Undead: ya tienes un no muerto invocado.");
                return;
            }

            if (!TrySpendEitrAndCooldown(player, "Summon Undead", EpicLootRaritySetsPlugin.HelveigSummonUndeadEitrUse.Value, _undeadSummonCooldown))
            {
                return;
            }

            float skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.BloodMagic) : 0f;
            GameObject prefab = GetUndeadSummonPrefab(skillLevel, out _undeadSummonDisplayName);
            if (prefab == null)
            {
                ShowMessage(player, "Summon Undead: prefab no disponible.");
                EpicLootRaritySetsPlugin.Log.LogWarning("Helveig Summon Undead could not resolve a prefab for Blood Magic level " + skillLevel.ToString("0.#") + ".");
                return;
            }

            Vector3 forward = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up);
            if (forward.sqrMagnitude <= 0.001f)
            {
                forward = Vector3.forward;
            }

            forward.Normalize();
            Vector3 spawn = player.transform.position + forward * 2.2f + player.transform.right * 0.6f;
            TryProjectGround(spawn, out spawn);
            DestroyUndeadSummon(player);
            _undeadSummon = UnityEngine.Object.Instantiate(prefab, spawn, Quaternion.LookRotation(forward));
            if (_undeadSummon == null)
            {
                ShowMessage(player, "Summon Undead: fallo al invocar.");
                return;
            }

            SetupUndeadSummon(player, _undeadSummon);
            player.UseEitr(EpicLootRaritySetsPlugin.HelveigSummonUndeadEitrUse.Value);
            _undeadSummonRemaining = Mathf.Max(1f, EpicLootRaritySetsPlugin.HelveigSummonUndeadDuration.Value);
            _undeadSummonCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.HelveigSummonUndeadCooldown.Value);
            RefreshUndeadSummonBuff(player);
            AbilityCooldownBuffController.Start(player, "HelveigSummonUndead", "Summon Undead", _undeadSummonCooldown, null);
            NorseVisualEffectBridge.Spawn(spawn, Quaternion.identity, "fx_DvergerMage_Support", "sfx_seekerqueen_callout", "vfx_StaffSkeleton", "Spirit", "Ghost");
            ShowMessage(player, "Summon Undead: " + _undeadSummonDisplayName + ".");
        }

        private static void UpdateUndeadSummon(Player player, float dt)
        {
            if (_undeadSummon == null)
            {
                RemoveUndeadSummonBuff(player);
                _undeadSummonRemaining = 0f;
                return;
            }

            Character character = _undeadSummon.GetComponent<Character>() ?? _undeadSummon.GetComponentInChildren<Character>();
            if (character == null || character.IsDead())
            {
                _undeadSummon = null;
                _undeadSummonRemaining = 0f;
                RemoveUndeadSummonBuff(player);
                return;
            }

            _undeadSummonRemaining -= dt;
            if (_undeadSummonRemaining <= 0f)
            {
                DestroyUndeadSummon(player);
                return;
            }

            MonsterAI monsterAI = _undeadSummon.GetComponent<MonsterAI>();
            if (monsterAI != null)
            {
                monsterAI.SetFollowTarget(player.gameObject);
                monsterAI.MakeTame();
            }

            RefreshUndeadSummonBuff(player);
        }

        private static bool HasLivingUndeadSummon()
        {
            if (_undeadSummon == null)
            {
                return false;
            }

            Character character = _undeadSummon.GetComponent<Character>() ?? _undeadSummon.GetComponentInChildren<Character>();
            return character != null && !character.IsDead();
        }

        private static GameObject GetUndeadSummonPrefab(float skillLevel, out string displayName)
        {
            if (skillLevel >= 90f)
            {
                displayName = "Charred Dyrnwyn";
                return GetPrefab("Charred_Melee_Dyrnwyn", "CHARRED_MELEE_DYRNWYN", "Charred_Melee");
            }

            if (skillLevel >= 60f)
            {
                displayName = "Unbjorn";
                return GetPrefab("UNBJORN", "UnBjorn", "Ulv_Bjorn", "UlvBjorn", "Ulv");
            }

            if (skillLevel >= 30f)
            {
                displayName = "Fallen Warrior";
                return GetPrefab("FallenWarrior", "FALLENWARRIOR", "Fallen_Warrior");
            }

            displayName = "Skeleton Hildir";
            return GetPrefab("SKELETON_HILDIR_NOCHEST", "Skeleton_Hildir_nochest", "Skeleton_Hildir_NoChest", "Skeleton_Hildir", "Skeleton");
        }

        private static void SetupUndeadSummon(Player player, GameObject summon)
        {
            if (player == null || summon == null)
            {
                return;
            }

            Character character = summon.GetComponent<Character>() ?? summon.GetComponentInChildren<Character>();
            if (character != null)
            {
                character.SetTamed(true);
            }

            Tameable tameable = summon.GetComponent<Tameable>();
            if (tameable == null)
            {
                tameable = summon.AddComponent<Tameable>();
            }

            if (tameable != null)
            {
                tameable.m_commandable = true;
                tameable.m_startsTamed = true;
                tameable.m_fedDuration = Mathf.Max(tameable.m_fedDuration, 3600f);
                tameable.m_unsummonDistance = Mathf.Max(tameable.m_unsummonDistance, 65f);
                tameable.m_unsummonOnOwnerLogoutSeconds = Mathf.Max(tameable.m_unsummonOnOwnerLogoutSeconds, 5f);
                if (TameMethod != null)
                {
                    try
                    {
                        TameMethod.Invoke(tameable, null);
                    }
                    catch
                    {
                    }
                }
            }

            MonsterAI monsterAI = summon.GetComponent<MonsterAI>();
            if (monsterAI != null)
            {
                monsterAI.MakeTame();
                monsterAI.SetFollowTarget(player.gameObject);
            }

            ZNetView view = summon.GetComponent<ZNetView>();
            if (view != null && view.GetZDO() != null)
            {
                view.GetZDO().Persistent = false;
            }
        }

        private static void DestroyUndeadSummon(Player player)
        {
            if (_undeadSummon != null)
            {
                if (ZNetScene.instance != null)
                {
                    ZNetScene.instance.Destroy(_undeadSummon);
                }
                else
                {
                    UnityEngine.Object.Destroy(_undeadSummon);
                }
            }

            _undeadSummon = null;
            _undeadSummonRemaining = 0f;
            RemoveUndeadSummonBuff(player);
        }

        private static void HealAlliesInBloodRite(Player player)
        {
            float radius = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HelveigBloodRiteRadius.Value);
            float healing = GetScaledHealing(player, EpicLootRaritySetsPlugin.HelveigBloodRiteBaseHealing.Value, EpicLootRaritySetsPlugin.HelveigBloodRiteHealingPerBloodMagicLevel.Value);
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead())
                {
                    continue;
                }

                if ((character.transform.position - player.transform.position).sqrMagnitude > radius * radius || !IsAlly(player, character))
                {
                    continue;
                }

                character.Heal(healing, true);
            }
        }

        private static bool IsAlly(Player player, Character target)
        {
            if (player == null || target == null)
            {
                return false;
            }

            if (target == player || target is Player || target.IsTamed())
            {
                return true;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return !(bool)value;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        private static float GetScaledHealing(Player player, float baseHealing, float healingPerBloodMagicLevel)
        {
            float skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.BloodMagic) : 0f;
            return Mathf.Max(0f, baseHealing + skillLevel * healingPerBloodMagicLevel);
        }

        private static HitData CreateHolyStrikeHit(Player player, ItemDrop.ItemData weapon, Character target)
        {
            float skillLevel = player != null ? player.GetSkillLevel(Skills.SkillType.BloodMagic) : 0f;
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_fire = Mathf.Max(0f, EpicLootRaritySetsPlugin.HelveigHolyStrikeBaseFireDamage.Value + skillLevel * EpicLootRaritySetsPlugin.HelveigHolyStrikeFireDamagePerBloodMagicLevel.Value),
                m_spirit = Mathf.Max(0f, EpicLootRaritySetsPlugin.HelveigHolyStrikeBaseSpiritDamage.Value + skillLevel * EpicLootRaritySetsPlugin.HelveigHolyStrikeSpiritDamagePerBloodMagicLevel.Value)
            };

            ApplyEpicLootDamageModifiers(player, weapon, ref damages);

            HitData hit = new HitData
            {
                m_damage = damages,
                m_skill = Skills.SkillType.BloodMagic,
                m_skillLevel = skillLevel,
                m_ranged = true,
                m_dodgeable = true,
                m_blockable = true,
                m_backstabBonus = 1f,
                m_staggerMultiplier = 1f,
                m_point = target != null ? target.GetCenterPoint() : Vector3.zero,
                m_dir = target != null && player != null ? (target.GetCenterPoint() - player.GetCenterPoint()).normalized : Vector3.forward
            };

            if (player != null)
            {
                hit.SetAttacker(player);
            }

            return hit;
        }

        private static void ApplyEpicLootDamageModifiers(Player player, ItemDrop.ItemData weapon, ref HitData.DamageTypes damages)
        {
            if (player == null || weapon == null || ApplyMagicDamageModifiersMethod == null)
            {
                return;
            }

            try
            {
                object[] args = { player, weapon, damages };
                ApplyMagicDamageModifiersMethod.Invoke(null, args);
                damages = (HitData.DamageTypes)args[2];
            }
            catch (Exception ex)
            {
                EpicLootRaritySetsPlugin.Log.LogWarning("Could not apply EpicLoot damage modifiers to Helveig Holy Strike. " + ex.GetBaseException().Message);
            }
        }

        private static bool TryFindHolyStrikeTarget(Player player, float range, out Character target)
        {
            target = null;
            if (player == null)
            {
                return false;
            }

            range = Mathf.Max(1f, range);
            Transform originTransform = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            RaycastHit[] hits = Physics.SphereCastAll(originTransform.position, 0.75f, originTransform.forward, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            if (hits != null)
            {
                foreach (RaycastHit hit in hits.OrderBy(hit => hit.distance))
                {
                    Character character = hit.collider != null ? hit.collider.GetComponentInParent<Character>() : null;
                    if (character == null || character == player || character.IsDead() || !IsEnemy(player, character))
                    {
                        continue;
                    }

                    target = character;
                    return true;
                }
            }

            float bestDistance = range * range;
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character == player || character.IsDead() || !IsEnemy(player, character))
                {
                    continue;
                }

                float sqrDistance = (character.transform.position - player.transform.position).sqrMagnitude;
                if (sqrDistance > bestDistance)
                {
                    continue;
                }

                bestDistance = sqrDistance;
                target = character;
            }

            return target != null;
        }

        private static bool IsEnemy(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player)
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        private static bool IsHelveigEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableHelveigAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableHelveigAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool CanReadAbilityInput(Player player)
        {
            return !player.IsDead() &&
                   !player.IsTeleporting() &&
                   !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> shortcutEntry)
        {
            if (shortcutEntry == null)
            {
                return false;
            }

            KeyboardShortcut shortcut = shortcutEntry.Value;
            if (shortcut.IsDown())
            {
                return true;
            }

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAnyMenuOpen()
        {
            if (InventoryGui.instance != null && InventoryGui.IsVisible())
            {
                return true;
            }

            if (Menu.instance != null && Menu.IsVisible())
            {
                return true;
            }

            if (TextInput.instance != null && TextInput.IsVisible())
            {
                return true;
            }

            if (Chat.instance != null && Chat.instance.HasFocus())
            {
                return true;
            }

            return Minimap.instance != null && Minimap.IsOpen();
        }

        private static bool TrySpendEitrAndCooldown(Player player, string abilityName, float eitrUse, float cooldown)
        {
            if (cooldown > 0f)
            {
                ShowMessage(player, string.Format("{0}: {1:0}s cooldown.", abilityName, cooldown));
                return false;
            }

            if (eitrUse > 0f && !player.HaveEitr(eitrUse))
            {
                if (Hud.instance != null)
                {
                    Hud.instance.EitrBarEmptyFlash();
                }

                ShowMessage(player, string.Format("{0}: no tienes eitr suficiente.", abilityName));
                return false;
            }

            return true;
        }

        private static StatusEffect GetOrCreateBloodRiteBuff()
        {
            if (_bloodRiteBuff == null)
            {
                _bloodRiteBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _bloodRiteBuff.name = BloodRiteBuffName;
                _bloodRiteBuff.m_name = "Blood Rite";
                _bloodRiteBuff.m_category = BloodRiteBuffCategory;
                _bloodRiteBuff.m_flashIcon = false;
                _bloodRiteBuff.m_cooldownIcon = true;
                _bloodRiteBuff.m_hidden = false;
            }

            _bloodRiteBuff.m_ttl = Mathf.Max(0.2f, _bloodRiteRemaining + 0.1f);
            _bloodRiteBuff.m_tooltip = string.Format(
                "Canalizando curacion de sangre.\n\nRadio: {0:0.#}m.\nTick: cada {1:0.#}s.\nCuracion por tick: {2:0.#} + {3:0.##} por nivel de Magia de sangre.\nTiempo restante: {4:0.#}s.\nMoverte cancela la canalizacion.",
                EpicLootRaritySetsPlugin.HelveigBloodRiteRadius.Value,
                EpicLootRaritySetsPlugin.HelveigBloodRiteTickInterval.Value,
                EpicLootRaritySetsPlugin.HelveigBloodRiteBaseHealing.Value,
                EpicLootRaritySetsPlugin.HelveigBloodRiteHealingPerBloodMagicLevel.Value,
                Mathf.Max(0f, _bloodRiteRemaining));
            return _bloodRiteBuff;
        }

        private static void RefreshBloodRiteBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateBloodRiteBuff();
            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 1, 0f, 0);
        }

        private static void RemoveBloodRiteBuff(Player player)
        {
            if (player != null && _bloodRiteBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_bloodRiteBuff.NameHash(), true);
            }
        }

        private static StatusEffect GetOrCreateUndeadSummonBuff()
        {
            if (_undeadSummonBuff == null)
            {
                _undeadSummonBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _undeadSummonBuff.name = UndeadSummonBuffName;
                _undeadSummonBuff.m_name = "Summon Undead";
                _undeadSummonBuff.m_category = UndeadSummonBuffCategory;
                _undeadSummonBuff.m_flashIcon = false;
                _undeadSummonBuff.m_cooldownIcon = true;
                _undeadSummonBuff.m_hidden = false;
            }

            _undeadSummonBuff.m_ttl = Mathf.Max(0.2f, _undeadSummonRemaining + 0.1f);
            _undeadSummonBuff.m_tooltip = string.Format(
                "No muerto Helveig activo.\n\nInvocacion: {0}.\nTiempo restante: {1:0.#}s.\nEscalado por Magia de sangre: 0-29 Skeleton Hildir, 30-59 Fallen Warrior, 60-89 Unbjorn, 90-100 Charred Dyrnwyn.",
                _undeadSummonDisplayName,
                Mathf.Max(0f, _undeadSummonRemaining));
            return _undeadSummonBuff;
        }

        private static void RefreshUndeadSummonBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateUndeadSummonBuff();
            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, 0, 0f, 0);
        }

        private static void RemoveUndeadSummonBuff(Player player)
        {
            if (player != null && _undeadSummonBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_undeadSummonBuff.NameHash(), true);
            }
        }

        private static GameObject GetPrefab(params string[] names)
        {
            if (names == null || ZNetScene.instance == null)
            {
                return null;
            }

            foreach (string name in names)
            {
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                GameObject prefab = ZNetScene.instance.GetPrefab(name);
                if (prefab != null && (prefab.GetComponent<Character>() != null || prefab.GetComponentInChildren<Character>() != null))
                {
                    return prefab;
                }
            }

            return null;
        }

        private static bool TryProjectGround(Vector3 probe, out Vector3 point)
        {
            RaycastHit[] hits = Physics.RaycastAll(probe + Vector3.up * 8f, Vector3.down, 20f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null || hit.collider.GetComponentInParent<Character>() != null)
                {
                    continue;
                }

                point = hit.point + Vector3.up * 0.05f;
                return true;
            }

            point = probe;
            return false;
        }

        private static void ShowMessage(Player player, string message)
        {
            if (player != null)
            {
                player.Message(MessageHud.MessageType.Center, message, 0, null, false);
            }
        }
    }

    internal static class RagnarAbilityController
    {
        private const string RequiredSet = "Ragnar";
        private const string DecayBuffName = "FranRagnarDecayAura";
        private const string DecayBuffCategory = "FranRagnarDecayAura";
        private const string FuryBuffName = "FranRagnarFury";
        private const string FuryBuffCategory = "FranRagnarFury";

        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly MethodInfo GetStaminaMethod = AccessTools.Method(typeof(Player), "GetStamina", Type.EmptyTypes) ?? AccessTools.Method(typeof(Character), "GetStamina", Type.EmptyTypes);
        private static readonly MethodInfo UseStaminaMethod = AccessTools.Method(typeof(Player), "UseStamina", new[] { typeof(float) }) ?? AccessTools.Method(typeof(Character), "UseStamina", new[] { typeof(float) });
        private static readonly FieldInfo AttackCharacterField = AccessTools.Field(typeof(Attack), "m_character");

        private static StatusEffect _decayBuff;
        private static StatusEffect _furyBuff;
        private static GameObject _decayAuraVisual;
        private static bool _decayAuraActive;
        private static float _decayTickTimer;
        private static float _furyTimer;
        private static int _furyStacks;
        private static int _furyHitCounter;

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            if (!IsEnabledAndActive())
            {
                Clear(player);
                return;
            }

            UpdateDecayAura(player, dt);
            UpdateFuryBuff(player, dt);

            if (!CanReadInput(player))
            {
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.RagnarDecayAuraHotkey))
            {
                ToggleDecayAura(player);
            }
        }

        internal static void Clear(Player player)
        {
            _decayAuraActive = false;
            _decayTickTimer = 0f;
            _furyTimer = 0f;
            _furyStacks = 0;
            _furyHitCounter = 0;
            RemoveDecayBuff(player);
            RemoveFuryBuff(player);
            DestroyVisual(ref _decayAuraVisual);
        }

        internal static float GetAttackSpeedMultiplier(Attack attack)
        {
            if (_furyStacks <= 0 || !IsEnabledAndActive())
            {
                return 1f;
            }

            Player player = GetAttackPlayer(attack);
            if (player == null || player != Player.m_localPlayer)
            {
                return 1f;
            }

            return 1f + Mathf.Max(0f, EpicLootRaritySetsPlugin.RagnarFuryAttackSpeedPerStack.Value) * _furyStacks;
        }

        internal static void OnPlayerHit(Character target, HitData hit)
        {
            if (target == null || hit == null || !IsEnabledAndActive())
            {
                return;
            }

            Character attacker = null;
            try
            {
                attacker = hit.GetAttacker();
            }
            catch
            {
            }

            Player player = attacker as Player;
            if (player == null || player != Player.m_localPlayer || target == player || !IsEnemyTarget(player, target))
            {
                return;
            }

            if (hit.m_ranged)
            {
                return;
            }

            float damage = TotalDamage(hit.m_damage);
            if (damage <= 0.01f)
            {
                return;
            }

            AddFuryStack(player);
            ApplyLifeSteal(player, damage);
            ApplyPeriodicHeal(player);
        }

        private static void ToggleDecayAura(Player player)
        {
            _decayAuraActive = !_decayAuraActive;
            _decayTickTimer = 0f;
            if (_decayAuraActive)
            {
                EnsureDecayAuraVisual(player);
                RefreshDecayBuff(player);
                ShowMessage(player, "Decay Aura: activa.");
                return;
            }

            RemoveDecayBuff(player);
            DestroyVisual(ref _decayAuraVisual);
            ShowMessage(player, "Decay Aura: desactivada.");
        }

        private static void UpdateDecayAura(Player player, float dt)
        {
            if (!_decayAuraActive)
            {
                return;
            }

            float staminaCost = Mathf.Max(0f, EpicLootRaritySetsPlugin.RagnarDecayAuraStaminaPerSecond.Value) * dt;
            if (!SpendStamina(player, staminaCost))
            {
                _decayAuraActive = false;
                RemoveDecayBuff(player);
                DestroyVisual(ref _decayAuraVisual);
                ShowMessage(player, "Decay Aura: sin vigor.");
                return;
            }

            RefreshDecayBuff(player);
            EnsureDecayAuraVisual(player);
            _decayTickTimer -= dt;
            if (_decayTickTimer > 0f)
            {
                return;
            }

            _decayTickTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.RagnarDecayAuraTickInterval.Value);
            float amount = Mathf.Max(0f, EpicLootRaritySetsPlugin.RagnarDecayAuraBaseDamage.Value + player.GetSkillLevel(Skills.SkillType.Axes) * EpicLootRaritySetsPlugin.RagnarDecayAuraDamagePerAxesLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes { m_poison = amount, m_spirit = amount };
            DamageArea(player, player.GetCenterPoint(), EpicLootRaritySetsPlugin.RagnarDecayAuraRadius.Value, damages, Skills.SkillType.Axes);
        }

        private static void RefreshDecayBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateDecayBuff();
            buff.m_ttl = 1.2f;
            buff.m_tooltip = string.Format(
                "Aura de decadencia Ragnar activa.\n\nRadio: {0:0.#}m.\nConsume vigor/s: {1:0.#}.\nDano veneno+espiritu: {2:0.#}+{3:0.##}/nivel de Hachas.",
                EpicLootRaritySetsPlugin.RagnarDecayAuraRadius.Value,
                EpicLootRaritySetsPlugin.RagnarDecayAuraStaminaPerSecond.Value,
                EpicLootRaritySetsPlugin.RagnarDecayAuraBaseDamage.Value,
                EpicLootRaritySetsPlugin.RagnarDecayAuraDamagePerAxesLevel.Value);
            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 0, 0f, 0);
        }

        private static StatusEffect GetOrCreateDecayBuff()
        {
            if (_decayBuff == null)
            {
                _decayBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _decayBuff.name = DecayBuffName;
                _decayBuff.m_name = "Decay Aura";
                _decayBuff.m_category = DecayBuffCategory;
                _decayBuff.m_flashIcon = false;
                _decayBuff.m_cooldownIcon = false;
                _decayBuff.m_hidden = false;
            }

            return _decayBuff;
        }

        private static void RemoveDecayBuff(Player player)
        {
            if (player != null && _decayBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_decayBuff.NameHash(), true);
            }
        }

        private static void EnsureDecayAuraVisual(Player player)
        {
            if (player == null || !_decayAuraActive || _decayAuraVisual != null)
            {
                return;
            }

            _decayAuraVisual = NorseVisualEffectBridge.SpawnAttached(
                player,
                Vector3.up * 1.0f,
                Quaternion.identity,
                1.4f,
                "FxDecayAura",
                "FxDarkAura",
                "DecayAura",
                "DarkAura",
                "PoisonCloud",
                "Spirit");
        }

        private static void DestroyVisual(ref GameObject visual)
        {
            if (visual != null)
            {
                UnityEngine.Object.Destroy(visual);
                visual = null;
            }
        }

        private static void DamageArea(Player player, Vector3 center, float radius, HitData.DamageTypes damages, Skills.SkillType skill)
        {
            radius = Mathf.Max(0.1f, radius);
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                if ((character.GetCenterPoint() - center).sqrMagnitude <= radius * radius)
                {
            character.Damage(CreateHit(player, damages, character.GetCenterPoint(), character.GetCenterPoint() - center, skill));
                }
            }
        }

        private static void UpdateFuryBuff(Player player, float dt)
        {
            if (_furyStacks <= 0)
            {
                return;
            }

            _furyTimer -= dt;
            if (_furyTimer > 0f && IsEnabledAndActive())
            {
                RefreshFuryBuff(player);
                return;
            }

            _furyTimer = 0f;
            _furyStacks = 0;
            RemoveFuryBuff(player);
        }

        private static void AddFuryStack(Player player)
        {
            int maxStacks = Mathf.Max(1, EpicLootRaritySetsPlugin.RagnarFuryMaxStacks.Value);
            _furyStacks = Mathf.Clamp(_furyStacks + 1, 1, maxStacks);
            _furyTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.RagnarFuryDuration.Value);
            RefreshFuryBuff(player);
        }

        private static void ApplyLifeSteal(Player player, float damage)
        {
            float lifeSteal = Mathf.Max(0f, EpicLootRaritySetsPlugin.RagnarFuryLifeStealPerStack.Value) * _furyStacks;
            float healing = Mathf.Max(0f, damage * lifeSteal);
            if (healing > 0.01f)
            {
                player.Heal(healing, true);
            }
        }

        private static void ApplyPeriodicHeal(Player player)
        {
            int every = Mathf.Max(1, EpicLootRaritySetsPlugin.RagnarFuryHealEveryAttacks.Value);
            _furyHitCounter++;
            if (_furyHitCounter < every)
            {
                return;
            }

            _furyHitCounter = 0;
            float healing = Mathf.Max(0f, player.GetMaxHealth() * EpicLootRaritySetsPlugin.RagnarFuryHealMaxHealthFraction.Value);
            if (healing > 0.01f)
            {
                player.Heal(healing, true);
                ShowMessage(player, string.Format("Blood Surge: +{0:0.#} salud.", healing));
            }
        }

        private static void RefreshFuryBuff(Player player)
        {
            if (player == null || _furyStacks <= 0)
            {
                return;
            }

            StatusEffect buff = GetOrCreateFuryBuff();
            int maxStacks = Mathf.Max(1, EpicLootRaritySetsPlugin.RagnarFuryMaxStacks.Value);
            float attackSpeed = EpicLootRaritySetsPlugin.RagnarFuryAttackSpeedPerStack.Value * _furyStacks * 100f;
            float lifeSteal = EpicLootRaritySetsPlugin.RagnarFuryLifeStealPerStack.Value * _furyStacks * 100f;
            buff.m_name = string.Format("Ragnar Fury {0}/{1}", _furyStacks, maxStacks);
            buff.m_ttl = Mathf.Max(0.1f, _furyTimer + 0.1f);
            buff.m_tooltip = string.Format(
                "Pasiva de Ragnar activa.\n\nStacks: {0}/{1}.\nVelocidad de ataque: +{2:0.#}%.\nRobo de vida: {3:0.##}% del dano de golpe.\nBlood Surge: cada {4} golpes melee cura {5:0.#}% de salud maxima.\nTiempo restante: {6:0.#}s.",
                _furyStacks,
                maxStacks,
                attackSpeed,
                lifeSteal,
                Mathf.Max(1, EpicLootRaritySetsPlugin.RagnarFuryHealEveryAttacks.Value),
                EpicLootRaritySetsPlugin.RagnarFuryHealMaxHealthFraction.Value * 100f,
                Mathf.Max(0f, _furyTimer));
            SEMan seMan = player.GetSEMan();
            seMan.RemoveStatusEffect(buff.NameHash(), true);
            seMan.AddStatusEffect(buff, true, _furyStacks, 0f, 0);
        }

        private static StatusEffect GetOrCreateFuryBuff()
        {
            if (_furyBuff == null)
            {
                _furyBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _furyBuff.name = FuryBuffName;
                _furyBuff.m_category = FuryBuffCategory;
                _furyBuff.m_flashIcon = false;
                _furyBuff.m_cooldownIcon = true;
                _furyBuff.m_hidden = false;
            }

            return _furyBuff;
        }

        private static void RemoveFuryBuff(Player player)
        {
            if (player != null && _furyBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_furyBuff.NameHash(), true);
            }
        }

        private static float TotalDamage(HitData.DamageTypes damages)
        {
            return damages.m_blunt + damages.m_slash + damages.m_pierce + damages.m_chop + damages.m_pickaxe + damages.m_fire + damages.m_frost + damages.m_lightning + damages.m_poison + damages.m_spirit;
        }

        private static Player GetAttackPlayer(Attack attack)
        {
            if (attack == null || AttackCharacterField == null)
            {
                return null;
            }

            try
            {
                return AttackCharacterField.GetValue(attack) as Player;
            }
            catch
            {
                return null;
            }
        }

        private static HitData CreateHit(Player player, HitData.DamageTypes damages, Vector3 point, Vector3 direction, Skills.SkillType skill)
        {
            HitData hit = new HitData
            {
                m_damage = damages,
                m_skill = skill,
                m_skillLevel = player != null ? player.GetSkillLevel(skill) : 0f,
                m_ranged = true,
                m_dodgeable = true,
                m_blockable = true,
                m_backstabBonus = 1f,
                m_staggerMultiplier = 1f,
                m_point = point,
                m_dir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward
            };

            if (player != null)
            {
                hit.SetAttacker(player);
            }

            return hit;
        }

        private static bool SpendStamina(Player player, float amount)
        {
            if (player == null || amount <= 0f)
            {
                return true;
            }

            float current = GetStamina(player);
            if (current >= 0f && current < amount)
            {
                return false;
            }

            try
            {
                if (UseStaminaMethod != null)
                {
                    UseStaminaMethod.Invoke(player, new object[] { amount });
                    return true;
                }
            }
            catch
            {
            }

            return true;
        }

        private static float GetStamina(Player player)
        {
            try
            {
                if (GetStaminaMethod != null)
                {
                    return Convert.ToSingle(GetStaminaMethod.Invoke(player, null));
                }
            }
            catch
            {
            }

            return -1f;
        }

        private static bool IsEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableRagnarAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableRagnarAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool CanReadInput(Player player)
        {
            return !player.IsDead() && !player.IsTeleporting() && !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> entry)
        {
            if (entry == null)
            {
                return false;
            }

            KeyboardShortcut shortcut = entry.Value;
            if (shortcut.IsDown())
            {
                return true;
            }

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAnyMenuOpen()
        {
            return (InventoryGui.instance != null && InventoryGui.IsVisible()) ||
                   (Menu.instance != null && Menu.IsVisible()) ||
                   (TextInput.instance != null && TextInput.IsVisible()) ||
                   (Chat.instance != null && Chat.instance.HasFocus()) ||
                   (Minimap.instance != null && Minimap.IsOpen());
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player)
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        private static void ShowMessage(Player player, string message)
        {
            if (player != null)
            {
                player.Message(MessageHud.MessageType.Center, message, 0, null, false);
            }
        }
    }

    internal static class HeimdallAbilityController
    {
        private const string RequiredSet = "Heimdall";
        private const string ArmorBuffName = "FranHeimdallBlockArmor";
        private const string ArmorBuffCategory = "FranHeimdallBlockArmor";
        private const string LightningStormBuffName = "FranHeimdallLightningStorm";
        private const string LightningStormBuffCategory = "FranHeimdallLightningStorm";
        private const string StoneShieldBuffName = "FranHeimdallStoneShield";
        private const string StoneShieldBuffCategory = "FranHeimdallStoneShield";

        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly MethodInfo GetStaminaMethod = AccessTools.Method(typeof(Player), "GetStamina", Type.EmptyTypes) ?? AccessTools.Method(typeof(Character), "GetStamina", Type.EmptyTypes);
        private static readonly MethodInfo UseStaminaMethod = AccessTools.Method(typeof(Player), "UseStamina", new[] { typeof(float) }) ?? AccessTools.Method(typeof(Character), "UseStamina", new[] { typeof(float) });
        private static readonly MethodInfo IsBlockingMethod = AccessTools.Method(typeof(Character), "IsBlocking", Type.EmptyTypes) ?? AccessTools.Method(typeof(Player), "IsBlocking", Type.EmptyTypes);
        private static readonly MethodInfo MonsterAISetTargetMethod = AccessTools.Method(typeof(MonsterAI), "SetTarget", new[] { typeof(Character) });
        private static readonly MethodInfo BaseAISetAlertedMethod = AccessTools.Method(typeof(BaseAI), "SetAlerted", new[] { typeof(bool) });
        private static readonly MethodInfo BaseAIAlertMethod = AccessTools.Method(typeof(BaseAI), "Alert", Type.EmptyTypes);
        private static readonly MethodInfo GetCurrentBlockerMethod =
            AccessTools.Method(typeof(Humanoid), "GetCurrentBlocker", Type.EmptyTypes) ??
            AccessTools.Method(typeof(Player), "GetCurrentBlocker", Type.EmptyTypes);
        private static readonly FieldInfo HitBlockedField = AccessTools.Field(typeof(HitData), "m_blocked");
        private static readonly FieldInfo StatusEffectTimeField = AccessTools.Field(typeof(StatusEffect), "m_time");

        private static readonly Dictionary<HitData, float> PendingBlockDamage = new Dictionary<HitData, float>();
        private static StatusEffect _armorBuff;
        private static StatusEffect _lightningStormBuff;
        private static StatusEffect _stoneShieldBuff;
        private static GameObject _lightningStormVisual;
        private static GameObject _stoneShieldVisual;
        private static int _armorStacks;
        private static float _armorTimer;
        private static float _lightningStormCooldown;
        private static float _lightningStormRemaining;
        private static float _lightningStormTickTimer;
        private static Vector3 _lightningStormCenter;
        private static float _stoneShieldCooldown;
        private static float _stoneShieldRemaining;
        private static bool _reflecting;
        private static int _lastSuccessfulBlockFrame = -1;
        private static float _lastSuccessfulBlockTime = -999f;
        private static float _lastObservedStamina = -1f;
        private static float _blockStaminaDropLockout;
        private static float _recentBlockableDamageTimer;

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            _lightningStormCooldown = Mathf.Max(0f, _lightningStormCooldown - dt);
            _stoneShieldCooldown = Mathf.Max(0f, _stoneShieldCooldown - dt);
            _blockStaminaDropLockout = Mathf.Max(0f, _blockStaminaDropLockout - dt);
            _recentBlockableDamageTimer = Mathf.Max(0f, _recentBlockableDamageTimer - dt);
            float currentStamina = GetStamina(player);
            UpdateArmorBuff(player, dt);

            if (!IsEnabledAndActive())
            {
                Clear(player);
                _lastObservedStamina = currentStamina;
                return;
            }

            DetectBlockByStaminaDrop(player, currentStamina);
            UpdateLightningStorm(player, dt);
            UpdateStoneShield(player, dt);

            if (!CanReadInput(player))
            {
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.HeimdallLightningStormHotkey))
            {
                TryLightningStorm(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.HeimdallStoneShieldHotkey))
            {
                TryStoneShield(player);
            }
        }

        internal static void Clear(Player player)
        {
            _armorStacks = 0;
            _armorTimer = 0f;
            _lightningStormRemaining = 0f;
            _lightningStormTickTimer = 0f;
            _lightningStormCenter = Vector3.zero;
            _stoneShieldRemaining = 0f;
            _lastObservedStamina = -1f;
            _blockStaminaDropLockout = 0f;
            _recentBlockableDamageTimer = 0f;
            _lastSuccessfulBlockTime = -999f;
            if (player != null)
            {
                if (_armorBuff != null) player.GetSEMan().RemoveStatusEffect(_armorBuff.NameHash(), true);
                if (_lightningStormBuff != null) player.GetSEMan().RemoveStatusEffect(_lightningStormBuff.NameHash(), true);
                if (_stoneShieldBuff != null) player.GetSEMan().RemoveStatusEffect(_stoneShieldBuff.NameHash(), true);
            }
            DestroyVisual(ref _lightningStormVisual);
            DestroyVisual(ref _stoneShieldVisual);
        }

        internal static void ModifyIncomingDamage(Player player, HitData hit)
        {
            if (player == null || hit == null || player != Player.m_localPlayer || _reflecting || !IsEnabledAndActive())
            {
                return;
            }

            if (CanAcceptBlockSource(player, hit))
            {
                _recentBlockableDamageTimer = 0.6f;
            }

            TryDetectGuardingBlock(player, hit);

            if (_armorStacks > 0)
            {
                float reduction = Mathf.Clamp01(EpicLootRaritySetsPlugin.HeimdallBlockArmorBonusPerStack.Value * _armorStacks);
                ScaleAllDamage(ref hit.m_damage, 1f - reduction);
            }

            if (_stoneShieldRemaining > 0f)
            {
                float before = TotalDamage(hit.m_damage);
                float reduction = Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallStoneShieldBaseReduction.Value + player.GetSkillLevel(Skills.SkillType.Blocking) * EpicLootRaritySetsPlugin.HeimdallStoneShieldReductionPerBlockingLevel.Value);
                ReduceFlatDamage(ref hit.m_damage, reduction);
                float mitigated = Mathf.Max(0f, before - TotalDamage(hit.m_damage));
                ReflectMitigatedDamage(player, hit, mitigated);
            }
        }

        internal static void OnDamaged(Player player, HitData hit)
        {
            if (player == null || player != Player.m_localPlayer || hit == null || !IsEnabledAndActive())
            {
                return;
            }

            if (IsBlocked(player, hit))
            {
                OnSuccessfulBlock(player);
            }
        }

        internal static void TrackIncomingBlockCandidate(Player player, HitData hit)
        {
            if (!CanEvaluateBlock(player, hit))
            {
                return;
            }

            PendingBlockDamage[hit] = TotalDamage(hit.m_damage);
        }

        internal static void DetectApplyDamageBlock(Player player, HitData hit)
        {
            if (!CanEvaluateBlock(player, hit))
            {
                return;
            }

            float before;
            bool hadBefore = PendingBlockDamage.TryGetValue(hit, out before);
            if (hadBefore)
            {
                PendingBlockDamage.Remove(hit);
            }

            float after = TotalDamage(hit.m_damage);
            if (hadBefore && before > 0.01f && after <= before * 0.98f)
            {
                OnSuccessfulBlock(player);
                return;
            }

            if (!hadBefore && IsBlocked(player, hit))
            {
                OnSuccessfulBlock(player);
            }
        }

        internal static void OnBlockAttackResult(Player player, HitData hit, bool success)
        {
            if (success && CanAcceptBlockSource(player, hit))
            {
                OnSuccessfulBlock(player);
            }
        }

        internal static void OnSuccessfulBlock(Player player)
        {
            if (player == null || player != Player.m_localPlayer || !IsEnabledAndActive())
            {
                return;
            }

            if (_lastSuccessfulBlockFrame == Time.frameCount)
            {
                return;
            }

            if (Time.time - _lastSuccessfulBlockTime < 0.35f)
            {
                return;
            }

            _lastSuccessfulBlockFrame = Time.frameCount;
            _lastSuccessfulBlockTime = Time.time;
            _blockStaminaDropLockout = Mathf.Max(_blockStaminaDropLockout, 0.35f);
            _armorStacks = Mathf.Clamp(_armorStacks + 1, 1, Mathf.Max(1, EpicLootRaritySetsPlugin.HeimdallBlockArmorMaxStacks.Value));
            _armorTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HeimdallBlockArmorDuration.Value);
            NorseVisualEffectBridge.SpawnAtPlayer(player, "FxStoneHit", "Block");
            RefreshArmorBuff(player, true);
        }

        internal static void ModifyOutgoingDamage(HitData hit)
        {
            if (hit == null || _armorStacks <= 0 || !IsEnabledAndActive())
            {
                return;
            }

            Character attacker = null;
            try { attacker = hit.GetAttacker(); } catch { }
            Player player = attacker as Player;
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            float multiplier = 1f + Mathf.Clamp01(EpicLootRaritySetsPlugin.HeimdallBlockArmorBonusPerStack.Value * _armorStacks);
            ScaleAllDamage(ref hit.m_damage, multiplier);
        }

        private static void UpdateArmorBuff(Player player, float dt)
        {
            if (_armorStacks <= 0)
            {
                return;
            }

            _armorTimer -= dt;
            if (_armorTimer <= 0f || !IsEnabledAndActive())
            {
                _armorStacks = 0;
                _armorTimer = 0f;
                if (player != null && _armorBuff != null)
                {
                    player.GetSEMan().RemoveStatusEffect(_armorBuff.NameHash(), true);
                }
                return;
            }

            RefreshArmorBuff(player, false);
        }

        private static void UpdateStoneShield(Player player, float dt)
        {
            if (_stoneShieldRemaining <= 0f)
            {
                return;
            }

            _stoneShieldRemaining -= dt;
            if (_stoneShieldRemaining <= 0f)
            {
                RemoveStoneShieldBuff(player);
                return;
            }

            EnsureStoneShieldVisual(player);
            RefreshStoneShieldBuff(player);
        }

        private static void TryLightningStorm(Player player)
        {
            if (_lightningStormRemaining > 0f)
            {
                ShowMessage(player, "Lightning Storm: activa.");
                return;
            }

            if (_lightningStormCooldown > 0f)
            {
                ShowMessage(player, string.Format("Lightning Storm: {0:0}s cooldown.", _lightningStormCooldown));
                return;
            }

            Vector3 center;
            if (!TryFindLightningStormPoint(player, out center))
            {
                ShowMessage(player, "Lightning Storm: sin punto.");
                return;
            }

            _lightningStormCenter = center;
            _lightningStormRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HeimdallLightningStormDuration.Value);
            _lightningStormTickTimer = 0f;
            _lightningStormCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallLightningStormCooldown.Value);
            EnsureLightningStormVisual(player);
            SpawnLightningStormPulse(_lightningStormCenter, Mathf.Max(0.5f, _lightningStormRemaining));
            RefreshLightningStormBuff(player);
            AbilityCooldownBuffController.Start(player, "HeimdallLightningStorm", "Lightning Storm", _lightningStormCooldown, null);
            ShowMessage(player, "Lightning Storm.");
        }

        private static void UpdateLightningStorm(Player player, float dt)
        {
            if (_lightningStormRemaining <= 0f)
            {
                return;
            }

            _lightningStormRemaining -= dt;
            if (_lightningStormRemaining <= 0f)
            {
                RemoveLightningStormBuff(player);
                DestroyVisual(ref _lightningStormVisual);
                _lightningStormCenter = Vector3.zero;
                return;
            }

            EnsureLightningStormVisual(player);
            RefreshLightningStormBuff(player);
            _lightningStormTickTimer -= dt;
            if (_lightningStormTickTimer > 0f)
            {
                return;
            }

            _lightningStormTickTimer = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HeimdallLightningStormTickInterval.Value);
            TickLightningStorm(player);
        }

        private static void TickLightningStorm(Player player)
        {
            if (player == null)
            {
                return;
            }

            float radius = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HeimdallLightningStormRadius.Value);
            float amount = Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallLightningStormBaseDamage.Value + player.GetSkillLevel(Skills.SkillType.Blocking) * EpicLootRaritySetsPlugin.HeimdallLightningStormDamagePerBlockingLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes { m_lightning = amount };
            Vector3 center = _lightningStormCenter;
            if (center == Vector3.zero)
            {
                center = player.GetCenterPoint();
            }

            SpawnLightningStormPulse(center, 1.35f);
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                if ((character.GetCenterPoint() - center).sqrMagnitude > radius * radius)
                {
                    continue;
                }

                RedirectThreatToPlayer(character, player);
                character.Damage(CreateHit(player, damages, character.GetCenterPoint(), character.GetCenterPoint() - center, Skills.SkillType.Blocking));
                NorseVisualEffectBridge.Spawn(character.GetCenterPoint(), Quaternion.identity, 1.25f, "FxLightning", "Lightning", "Thunder", "Thor");
            }
        }

        private static bool TryFindLightningStormPoint(Player player, out Vector3 point)
        {
            point = player != null ? player.transform.position : Vector3.zero;
            if (player == null)
            {
                return false;
            }

            float range = Mathf.Max(18f, EpicLootRaritySetsPlugin.HeimdallLightningStormRadius.Value * 3f);
            Transform originTransform = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            RaycastHit hit;
            if (Physics.Raycast(originTransform.position, originTransform.forward, out hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                point = hit.point;
                return TryProjectGround(point, out point);
            }

            point = player.transform.position + Vector3.ProjectOnPlane(player.transform.forward, Vector3.up).normalized * Mathf.Min(range, 18f);
            return TryProjectGround(point, out point);
        }

        private static bool TryProjectGround(Vector3 probe, out Vector3 point)
        {
            RaycastHit[] hits = Physics.RaycastAll(probe + Vector3.up * 12f, Vector3.down, 30f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null || hit.collider.GetComponentInParent<Character>() != null)
                {
                    continue;
                }

                point = hit.point + Vector3.up * 0.08f;
                return true;
            }

            point = probe;
            return true;
        }

        private static void SpawnLightningStormPulse(Vector3 center, float lifetime)
        {
            NorseVisualEffectBridge.Spawn(
                center + Vector3.up * 0.15f,
                Quaternion.identity,
                Mathf.Max(0.2f, lifetime),
                "FxLightningExpansion",
                "FxLightningExplosion2",
                "FxLightningExplosion1",
                "FxLightningMist",
                "FxLightningWithSoundOnline",
                "FxLightningWithoutSoundOnline",
                "Storm",
                "Thunder",
                "Lightning");
        }

        private static void RedirectThreatToPlayer(Character target, Player player)
        {
            if (target == null || player == null)
            {
                return;
            }

            MonsterAI monsterAI = target.GetComponent<MonsterAI>();
            if (monsterAI != null)
            {
                try
                {
                    if (MonsterAISetTargetMethod != null)
                    {
                        MonsterAISetTargetMethod.Invoke(monsterAI, new object[] { player });
                    }
                }
                catch
                {
                }
            }

            BaseAI baseAI = target.GetComponent<BaseAI>();
            if (baseAI != null)
            {
                try
                {
                    if (BaseAISetAlertedMethod != null)
                    {
                        BaseAISetAlertedMethod.Invoke(baseAI, new object[] { true });
                    }

                    if (BaseAIAlertMethod != null)
                    {
                        BaseAIAlertMethod.Invoke(baseAI, null);
                    }
                }
                catch
                {
                }
            }
        }

        private static void TryStoneShield(Player player)
        {
            if (_stoneShieldRemaining > 0f)
            {
                ShowMessage(player, "Stone Shield: activo.");
                return;
            }

            if (_stoneShieldCooldown > 0f)
            {
                ShowMessage(player, string.Format("Stone Shield: {0:0}s cooldown.", _stoneShieldCooldown));
                return;
            }

            float stamina = Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallStoneShieldStaminaUse.Value);
            if (!SpendStamina(player, stamina))
            {
                ShowMessage(player, "Stone Shield: no tienes vigor suficiente.");
                return;
            }

            _stoneShieldRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.HeimdallStoneShieldDuration.Value);
            _stoneShieldCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallStoneShieldCooldown.Value);
            EnsureStoneShieldVisual(player);
            RefreshStoneShieldBuff(player);
            AbilityCooldownBuffController.Start(player, "HeimdallStoneShield", "Stone Shield", _stoneShieldCooldown, null);
            ShowMessage(player, "Stone Shield.");
        }

        private static void ReflectMitigatedDamage(Player player, HitData incoming, float mitigated)
        {
            if (mitigated <= 0f)
            {
                return;
            }

            Character attacker = null;
            try { attacker = incoming.GetAttacker(); } catch { }
            if (attacker == null || attacker == player || attacker.IsDead() || !IsEnemyTarget(player, attacker))
            {
                return;
            }

            float reflect = Mathf.Min(
                Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallStoneShieldReflectBase.Value + player.GetSkillLevel(Skills.SkillType.Blocking) * EpicLootRaritySetsPlugin.HeimdallStoneShieldReflectPerBlockingLevel.Value),
                Mathf.Max(0f, EpicLootRaritySetsPlugin.HeimdallStoneShieldReflectMax.Value));
            HitData hit = CreateHit(player, new HitData.DamageTypes { m_blunt = mitigated * reflect }, attacker.GetCenterPoint(), attacker.GetCenterPoint() - player.GetCenterPoint(), Skills.SkillType.Blocking);
            _reflecting = true;
            try { attacker.Damage(hit); }
            finally { _reflecting = false; }
        }

        private static bool IsBlocked(Player player, HitData hit)
        {
            if (HitBlockedField != null)
            {
                try
                {
                    object blocked = HitBlockedField.GetValue(hit);
                    if (blocked is bool && (bool)blocked)
                    {
                        return true;
                    }
                }
                catch { }
            }

            if (IsBlockingMethod != null)
            {
                try
                {
                    object value = IsBlockingMethod.Invoke(player, null);
                    return value is bool && (bool)value;
                }
                catch { }
            }

            return SetAbilityInput.IsBlockHeld() && HasCurrentBlocker(player);
        }

        private static bool HasCurrentBlocker(Player player)
        {
            if (player == null)
            {
                return false;
            }

            if (GetCurrentBlockerMethod != null)
            {
                try
                {
                    return GetCurrentBlockerMethod.Invoke(player, null) != null;
                }
                catch
                {
                }
            }

            return SetAbilityInput.IsBlockHeld();
        }

        private static ItemDrop.ItemData GetCurrentBlockerItem(Player player)
        {
            if (player == null || GetCurrentBlockerMethod == null)
            {
                return null;
            }

            try
            {
                return GetCurrentBlockerMethod.Invoke(player, null) as ItemDrop.ItemData;
            }
            catch
            {
                return null;
            }
        }

        private static Sprite GetBlockerIcon(Player player)
        {
            ItemDrop.ItemData item = GetCurrentBlockerItem(player);
            if (item == null && player != null)
            {
                item = player.GetCurrentWeapon();
            }

            if (item == null || item.m_shared == null || item.m_shared.m_icons == null || item.m_shared.m_icons.Length == 0)
            {
                return null;
            }

            return item.m_shared.m_icons[0];
        }

        private static void RefreshArmorBuff(Player player, bool forceLevelUpdate)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateArmorBuff();
            int maxStacks = Mathf.Max(1, EpicLootRaritySetsPlugin.HeimdallBlockArmorMaxStacks.Value);
            buff.m_ttl = Mathf.Max(0.1f, _armorTimer + 0.1f);
            buff.m_name = string.Format("Heimdall Guard {0}/{1}", _armorStacks, maxStacks);
            buff.m_tooltip = string.Format("Bloquear ataques fortalece Heimdall.\n\nStacks: {0}/{1}.\nReduccion de dano: {2:0.#}%.\nDano aumentado: {2:0.#}%.\nTiempo restante: {3:0.#}s.", _armorStacks, EpicLootRaritySetsPlugin.HeimdallBlockArmorMaxStacks.Value, EpicLootRaritySetsPlugin.HeimdallBlockArmorBonusPerStack.Value * _armorStacks * 100f, Mathf.Max(0f, _armorTimer));
            buff.m_icon = GetBlockerIcon(player);
            SEMan seMan = player.GetSEMan();
            StatusEffect active = seMan.GetStatusEffect(buff.NameHash());
            if (active == null || forceLevelUpdate)
            {
                seMan.RemoveStatusEffect(buff.NameHash(), true);
                seMan.AddStatusEffect(buff, true, Mathf.Max(1, _armorStacks), 0f, 0);
                return;
            }

            active.m_name = buff.m_name;
            active.m_tooltip = buff.m_tooltip;
            active.m_ttl = buff.m_ttl;
            active.m_icon = buff.m_icon;
            active.m_flashIcon = buff.m_flashIcon;
            active.m_cooldownIcon = buff.m_cooldownIcon;
            active.m_hidden = buff.m_hidden;
            active.m_category = buff.m_category;
            if (StatusEffectTimeField != null)
            {
                try
                {
                    StatusEffectTimeField.SetValue(active, 0f);
                }
                catch
                {
                }
            }
        }

        private static StatusEffect GetOrCreateArmorBuff()
        {
            if (_armorBuff == null)
            {
                _armorBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _armorBuff.name = ArmorBuffName;
                _armorBuff.m_name = "Heimdall Guard";
                _armorBuff.m_category = ArmorBuffCategory;
                _armorBuff.m_flashIcon = false;
                _armorBuff.m_cooldownIcon = true;
                _armorBuff.m_hidden = false;
            }

            return _armorBuff;
        }

        private static bool CanEvaluateBlock(Player player, HitData hit)
        {
            if (!CanAcceptBlockSource(player, hit))
            {
                return false;
            }

            return HasCurrentBlocker(player) || SetAbilityInput.IsBlockHeld() || IsBlocked(player, hit);
        }

        private static bool CanAcceptBlockSource(Player player, HitData hit)
        {
            if (player == null || player != Player.m_localPlayer || hit == null || !IsEnabledAndActive())
            {
                return false;
            }

            Character attacker = null;
            try
            {
                attacker = hit.GetAttacker();
            }
            catch
            {
            }

            if (attacker != null && (attacker == player || !IsEnemyTarget(player, attacker)))
            {
                return false;
            }

            return attacker != null || hit.m_blockable;
        }

        private static void TryDetectGuardingBlock(Player player, HitData hit)
        {
            if (!CanAcceptBlockSource(player, hit))
            {
                return;
            }

            if (!IsActivelyGuarding(player) && !IsBlocked(player, hit))
            {
                return;
            }

            if (!IsHitInBlockArc(player, hit))
            {
                return;
            }

            OnSuccessfulBlock(player);
        }

        private static void DetectBlockByStaminaDrop(Player player, float currentStamina)
        {
            try
            {
                if (player == null || currentStamina < 0f || _lastObservedStamina < 0f || _blockStaminaDropLockout > 0f)
                {
                    return;
                }

                float staminaDrop = _lastObservedStamina - currentStamina;
                if (staminaDrop < 0.2f)
                {
                    return;
                }

                if (!IsActivelyGuarding(player))
                {
                    return;
                }

                if (_recentBlockableDamageTimer <= 0f && !HasEnemyNear(player, 6f))
                {
                    return;
                }

                _blockStaminaDropLockout = 0.45f;
                OnSuccessfulBlock(player);
            }
            finally
            {
                _lastObservedStamina = currentStamina;
            }
        }

        private static bool IsActivelyGuarding(Player player)
        {
            return player != null && (SetAbilityInput.IsBlockHeld() || HasCurrentBlocker(player) || IsBlocking(player));
        }

        private static bool IsBlocking(Player player)
        {
            if (player == null || IsBlockingMethod == null)
            {
                return false;
            }

            try
            {
                object value = IsBlockingMethod.Invoke(player, null);
                return value is bool && (bool)value;
            }
            catch
            {
                return false;
            }
        }

        private static bool HasEnemyNear(Player player, float radius)
        {
            if (player == null)
            {
                return false;
            }

            float clampedRadius = Mathf.Max(0.1f, radius);
            float radiusSqr = clampedRadius * clampedRadius;
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                if ((character.GetCenterPoint() - player.GetCenterPoint()).sqrMagnitude <= radiusSqr)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsHitInBlockArc(Player player, HitData hit)
        {
            Character attacker = null;
            try { attacker = hit != null ? hit.GetAttacker() : null; } catch { }
            if (player == null || attacker == null)
            {
                return true;
            }

            Vector3 toAttacker = attacker.GetCenterPoint() - player.GetCenterPoint();
            toAttacker.y = 0f;
            Vector3 forward = player.transform.forward;
            forward.y = 0f;
            if (toAttacker.sqrMagnitude <= 0.001f || forward.sqrMagnitude <= 0.001f)
            {
                return true;
            }

            return Vector3.Dot(forward.normalized, toAttacker.normalized) >= -0.35f;
        }

        private static void RefreshLightningStormBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateLightningStormBuff();
            buff.m_ttl = Mathf.Max(0.1f, _lightningStormRemaining + 0.1f);
            buff.m_tooltip = string.Format(
                "Tormenta de relampagos de Thor activa.\n\nZona fija invocada en el punto apuntado.\nRadio: {0:0.#}m.\nTick: cada {1:0.#}s.\nDano rayo: {2:0.#}+{3:0.##}/nivel de Bloqueo.\nCada golpe redirige la amenaza de los enemigos afectados hacia Heimdall.\nTiempo restante: {4:0.#}s.",
                EpicLootRaritySetsPlugin.HeimdallLightningStormRadius.Value,
                EpicLootRaritySetsPlugin.HeimdallLightningStormTickInterval.Value,
                EpicLootRaritySetsPlugin.HeimdallLightningStormBaseDamage.Value,
                EpicLootRaritySetsPlugin.HeimdallLightningStormDamagePerBlockingLevel.Value,
                Mathf.Max(0f, _lightningStormRemaining));
            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 0, 0f, 0);
        }

        private static StatusEffect GetOrCreateLightningStormBuff()
        {
            if (_lightningStormBuff == null)
            {
                _lightningStormBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _lightningStormBuff.name = LightningStormBuffName;
                _lightningStormBuff.m_name = "Lightning Storm";
                _lightningStormBuff.m_category = LightningStormBuffCategory;
                _lightningStormBuff.m_flashIcon = false;
                _lightningStormBuff.m_cooldownIcon = true;
                _lightningStormBuff.m_hidden = false;
            }

            return _lightningStormBuff;
        }

        private static void RemoveLightningStormBuff(Player player)
        {
            _lightningStormRemaining = 0f;
            _lightningStormTickTimer = 0f;
            _lightningStormCenter = Vector3.zero;
            if (player != null && _lightningStormBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_lightningStormBuff.NameHash(), true);
            }
        }

        private static void RefreshStoneShieldBuff(Player player)
        {
            StatusEffect buff = GetOrCreateStoneShieldBuff();
            buff.m_ttl = Mathf.Max(0.1f, _stoneShieldRemaining + 0.1f);
            buff.m_tooltip = string.Format("Stone Shield activo.\n\nReduce dano plano y refleja parte del dano mitigado.\nTiempo restante: {0:0.#}s.", Mathf.Max(0f, _stoneShieldRemaining));
            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 0, 0f, 0);
        }

        private static StatusEffect GetOrCreateStoneShieldBuff()
        {
            if (_stoneShieldBuff == null)
            {
                _stoneShieldBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _stoneShieldBuff.name = StoneShieldBuffName;
                _stoneShieldBuff.m_name = "Stone Shield";
                _stoneShieldBuff.m_category = StoneShieldBuffCategory;
                _stoneShieldBuff.m_flashIcon = false;
                _stoneShieldBuff.m_cooldownIcon = true;
                _stoneShieldBuff.m_hidden = false;
            }

            return _stoneShieldBuff;
        }

        private static void RemoveStoneShieldBuff(Player player)
        {
            _stoneShieldRemaining = 0f;
            if (player != null && _stoneShieldBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_stoneShieldBuff.NameHash(), true);
            }
            DestroyVisual(ref _stoneShieldVisual);
        }

        private static void EnsureLightningStormVisual(Player player)
        {
            if (player == null || _lightningStormRemaining <= 0f || _lightningStormVisual != null)
            {
                return;
            }

            Vector3 center = _lightningStormCenter != Vector3.zero ? _lightningStormCenter : player.GetCenterPoint();
            _lightningStormVisual = NorseVisualEffectBridge.Spawn(
                center + Vector3.up * 0.2f,
                Quaternion.identity,
                Mathf.Max(0.2f, _lightningStormRemaining + 0.25f),
                "FxLightningExpansion",
                "FxLightningExplosion2",
                "FxLightningExplosion1",
                "FxLightningMist",
                "LightningStorm",
                "ThunderStorm",
                "ThorStorm",
                "Thunder",
                "Lightning",
                "Thor");
        }

        private static void EnsureStoneShieldVisual(Player player)
        {
            if (player == null || _stoneShieldRemaining <= 0f || _stoneShieldVisual != null)
            {
                return;
            }

            _stoneShieldVisual = NorseVisualEffectBridge.SpawnAttached(
                player,
                Vector3.up * 1.0f,
                Quaternion.identity,
                Mathf.Max(0.2f, _stoneShieldRemaining + 0.25f),
                "FxStoneShield",
                "StoneShield",
                "Stone",
                "Shield");
        }

        private static void DestroyVisual(ref GameObject visual)
        {
            if (visual != null)
            {
                UnityEngine.Object.Destroy(visual);
                visual = null;
            }
        }

        private static void DamageArea(Player player, Vector3 center, float radius, HitData.DamageTypes damages, Skills.SkillType skill)
        {
            radius = Mathf.Max(0.1f, radius);
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                if ((character.GetCenterPoint() - center).sqrMagnitude <= radius * radius)
                {
                    character.Damage(CreateHit(player, damages, character.GetCenterPoint(), character.GetCenterPoint() - center, skill));
                }
            }
        }

        private static HitData CreateHit(Player player, HitData.DamageTypes damages, Vector3 point, Vector3 direction, Skills.SkillType skill)
        {
            HitData hit = new HitData
            {
                m_damage = damages,
                m_skill = skill,
                m_skillLevel = player != null ? player.GetSkillLevel(skill) : 0f,
                m_ranged = true,
                m_dodgeable = true,
                m_blockable = true,
                m_backstabBonus = 1f,
                m_staggerMultiplier = 1f,
                m_point = point,
                m_dir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward
            };
            if (player != null) hit.SetAttacker(player);
            return hit;
        }

        private static bool SpendStamina(Player player, float amount)
        {
            if (player == null || amount <= 0f)
            {
                return true;
            }

            float current = GetStamina(player);
            if (current >= 0f && current < amount)
            {
                return false;
            }

            try
            {
                if (UseStaminaMethod != null)
                {
                    UseStaminaMethod.Invoke(player, new object[] { amount });
                    return true;
                }
            }
            catch { }

            return true;
        }

        private static float GetStamina(Player player)
        {
            try
            {
                if (GetStaminaMethod != null)
                {
                    return Convert.ToSingle(GetStaminaMethod.Invoke(player, null));
                }
            }
            catch { }
            return -1f;
        }

        private static void ReduceFlatDamage(ref HitData.DamageTypes damages, float reduction)
        {
            float total = TotalDamage(damages);
            if (total <= 0.001f)
            {
                return;
            }

            ScaleAllDamage(ref damages, Mathf.Clamp01((total - Mathf.Max(0f, reduction)) / total));
        }

        private static void ScaleAllDamage(ref HitData.DamageTypes damages, float multiplier)
        {
            damages.m_blunt *= multiplier;
            damages.m_slash *= multiplier;
            damages.m_pierce *= multiplier;
            damages.m_chop *= multiplier;
            damages.m_pickaxe *= multiplier;
            damages.m_fire *= multiplier;
            damages.m_frost *= multiplier;
            damages.m_lightning *= multiplier;
            damages.m_poison *= multiplier;
            damages.m_spirit *= multiplier;
        }

        private static float TotalDamage(HitData.DamageTypes damages)
        {
            return damages.m_blunt + damages.m_slash + damages.m_pierce + damages.m_chop + damages.m_pickaxe + damages.m_fire + damages.m_frost + damages.m_lightning + damages.m_poison + damages.m_spirit;
        }

        private static bool IsEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableHeimdallAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableHeimdallAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool CanReadInput(Player player)
        {
            return !player.IsDead() && !player.IsTeleporting() && !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> entry)
        {
            if (entry == null) return false;
            KeyboardShortcut shortcut = entry.Value;
            if (shortcut.IsDown()) return true;
            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey)) return false;
            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier)) return false;
            }
            return true;
        }

        private static bool IsAnyMenuOpen()
        {
            return (InventoryGui.instance != null && InventoryGui.IsVisible()) ||
                   (Menu.instance != null && Menu.IsVisible()) ||
                   (TextInput.instance != null && TextInput.IsVisible()) ||
                   (Chat.instance != null && Chat.instance.HasFocus()) ||
                   (Minimap.instance != null && Minimap.IsOpen());
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player)
            {
                return false;
            }

            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool) return (bool)value;
                }
                catch { }
            }

            return true;
        }

        private static void ShowMessage(Player player, string message)
        {
            if (player != null) player.Message(MessageHud.MessageType.Center, message, 0, null, false);
        }
    }

    internal static class SeidrAbilityController
    {
        private const string RequiredSet = "Seidr";
        private const string NanoBuffName = "FranSeidrNanocube";
        private const string NanoBuffCategory = "FranSeidrNanocube";
        private const string NanoDamageBuffName = "FranSeidrNanocubeDamage";
        private const string NanoDamageBuffCategory = "FranSeidrNanocubeDamage";
        private const string ShieldBuffName = "FranSeidrElementalShield";
        private const string ShieldBuffCategory = "FranSeidrElementalShield";
        private const string FrostNovaSlowBuffName = "FranSeidrFrostNovaSlow";
        private const string FrostNovaSlowBuffCategory = "FranSeidrFrostNovaSlow";

        private static readonly MethodInfo BaseAIIsEnemyMethod = AccessTools.Method(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) });
        private static readonly MethodInfo GetMaxEitrMethod = AccessTools.Method(typeof(Player), "GetMaxEitr", Type.EmptyTypes) ?? AccessTools.Method(typeof(Character), "GetMaxEitr", Type.EmptyTypes);
        private static readonly FieldInfo SpeedModifierField = AccessTools.Field(typeof(SE_Stats), "m_speedModifier");
        private static readonly FieldInfo StatusEffectTimeField = AccessTools.Field(typeof(StatusEffect), "m_time");
        private static readonly MethodInfo TameMethod = AccessTools.Method(typeof(Tameable), "Tame");

        private static StatusEffect _nanoBuff;
        private static StatusEffect _nanoDamageBuff;
        private static StatusEffect _shieldBuff;
        private static StatusEffect _frostNovaSlowBuff;
        private static GameObject _stoneGolem;
        private static GameObject _shieldVisual;
        private static Vector3 _nanoCenter;
        private static readonly List<GameObject> NanoVisuals = new List<GameObject>();
        private static float _nanoRemaining;
        private static float _nanoVisualRefreshTimer;
        private static bool _nanoAreaActive;
        private static bool _nanoPlayerInside;
        private static float _nanoCooldown;
        private static float _shieldCooldown;
        private static bool _shieldActive;
        private static float _stoneGolemCooldown;
        private static float _stoneGolemRemaining;
        private static float _frostNovaCooldown;

        internal static void Update(Player player, float dt)
        {
            if (player == null || player != Player.m_localPlayer)
            {
                return;
            }

            _nanoCooldown = Mathf.Max(0f, _nanoCooldown - dt);
            _shieldCooldown = Mathf.Max(0f, _shieldCooldown - dt);
            _stoneGolemCooldown = Mathf.Max(0f, _stoneGolemCooldown - dt);
            _frostNovaCooldown = Mathf.Max(0f, _frostNovaCooldown - dt);

            if (!IsEnabledAndActive())
            {
                Clear(player);
                return;
            }

            UpdateNanoCube(player, dt);
            UpdateElementalShield(player, dt);
            UpdateStoneGolem(dt);

            if (!CanReadInput(player))
            {
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.SeidrNanoCubeHotkey))
            {
                TryNanoCube(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.SeidrFrostNovaHotkey) &&
                SetAbilityInput.IsBlockHeld())
            {
                TryFrostNova(player);
                return;
            }

            if (IsShortcutDown(EpicLootRaritySetsPlugin.SeidrElementalShieldHotkey))
            {
                ToggleElementalShield(player);
                return;
            }

            if (SetAbilityInput.IsSecondaryAttackDown() ||
                IsShortcutDown(EpicLootRaritySetsPlugin.SeidrStoneGolemHotkey))
            {
                TryStoneGolem(player);
            }
        }

        internal static void Clear(Player player)
        {
            _nanoRemaining = 0f;
            _nanoVisualRefreshTimer = 0f;
            _nanoAreaActive = false;
            _nanoPlayerInside = false;
            _shieldActive = false;
            if (player != null)
            {
                if (_nanoBuff != null) player.GetSEMan().RemoveStatusEffect(_nanoBuff.NameHash(), true);
                if (_nanoDamageBuff != null) player.GetSEMan().RemoveStatusEffect(_nanoDamageBuff.NameHash(), true);
                if (_shieldBuff != null) player.GetSEMan().RemoveStatusEffect(_shieldBuff.NameHash(), true);
            }
            DestroyNanoVisuals();
            DestroyVisual(ref _shieldVisual);
            DestroyStoneGolem();
        }

        internal static void ModifyIncomingDamage(Player player, HitData hit)
        {
            if (player == null || hit == null || player != Player.m_localPlayer || !_shieldActive || !IsEnabledAndActive() || !HasShieldBuff(player))
            {
                return;
            }

            hit.m_damage = new HitData.DamageTypes();
        }

        internal static void ModifyOutgoingDamage(HitData hit)
        {
            if (hit == null || _nanoRemaining <= 0f)
            {
                return;
            }

            Character attacker = null;
            try { attacker = hit.GetAttacker(); } catch { }
            Player player = attacker as Player;
            if (player == null || player != Player.m_localPlayer || !IsEnabledAndActive())
            {
                return;
            }

            if (!IsNanoDamageActiveFor(player))
            {
                return;
            }

            float multiplier = 1f + Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrNanoCubeMagicDamageBonus.Value);
            hit.m_damage.m_fire *= multiplier;
            hit.m_damage.m_frost *= multiplier;
            hit.m_damage.m_lightning *= multiplier;
            hit.m_damage.m_poison *= multiplier;
            hit.m_damage.m_spirit *= multiplier;
        }

        private static void TryNanoCube(Player player)
        {
            if (_nanoCooldown > 0f)
            {
                ShowMessage(player, string.Format("Nanocube: {0:0}s cooldown.", _nanoCooldown));
                return;
            }

            float eitr = Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrNanoCubeEitrUse.Value);
            if (eitr > 0f && !player.HaveEitr(eitr))
            {
                if (Hud.instance != null)
                {
                    Hud.instance.EitrBarEmptyFlash();
                }
                ShowMessage(player, "Nanocube: no tienes eitr suficiente.");
                return;
            }

            player.UseEitr(eitr);
            _nanoCenter = player.transform.position;
            _nanoRemaining = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SeidrNanoCubeDuration.Value);
            _nanoVisualRefreshTimer = 0f;
            _nanoAreaActive = true;
            _nanoPlayerInside = IsPlayerInsideNano(player);
            _nanoCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrNanoCubeCooldown.Value);
            RefreshNanoVisual();
            RefreshNanoBuff(player);
            if (_nanoPlayerInside)
            {
                RefreshNanoDamageBuff(player);
            }
            else
            {
                RemoveNanoDamageBuff(player);
            }
            AbilityCooldownBuffController.Start(player, "SeidrNanocube", "Nanocube", _nanoCooldown, null);
            ShowMessage(player, "Nanocube.");
        }

        private static void ToggleElementalShield(Player player)
        {
            if (_shieldActive)
            {
                _shieldActive = false;
                _shieldCooldown = 0f;
                RemoveShieldBuff(player);
                DestroyVisual(ref _shieldVisual);
                AbilityCooldownBuffController.Clear(player, "SeidrElementalShield");
                ShowMessage(player, "Elemental Shield: desactivado.");
                return;
            }

            _shieldCooldown = 0f;

            float eitr = Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrElementalShieldEitrUse.Value);
            if (eitr > 0f && !player.HaveEitr(eitr))
            {
                if (Hud.instance != null)
                {
                    Hud.instance.EitrBarEmptyFlash();
                }
                ShowMessage(player, "Elemental Shield: no tienes eitr suficiente.");
                return;
            }

            player.UseEitr(eitr);
            _shieldActive = true;
            EnsureShieldVisual(player);
            RefreshShieldBuff(player);
            ShowMessage(player, "Elemental Shield: activo.");
        }

        private static void TryStoneGolem(Player player)
        {
            if (_stoneGolemCooldown > 0f)
            {
                ShowMessage(player, string.Format("Stone Golem: {0:0}s cooldown.", _stoneGolemCooldown));
                return;
            }

            float eitr = Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrStoneGolemEitrUse.Value);
            if (eitr > 0f && !player.HaveEitr(eitr))
            {
                if (Hud.instance != null)
                {
                    Hud.instance.EitrBarEmptyFlash();
                }
                ShowMessage(player, "Stone Golem: no tienes eitr suficiente.");
                return;
            }

            GameObject prefab = GetPrefab("stonegolem_friendly", "StoneGolem_friendly", "StoneGolem_Friendly", "StoneGolem", "GoblinBrute", "golem");
            if (prefab == null)
            {
                ShowMessage(player, "Stone Golem: prefab no disponible.");
                return;
            }

            DestroyStoneGolem();
            player.UseEitr(eitr);
            Vector3 spawn = player.transform.position + player.transform.forward * 3f;
            NorseVisualEffectBridge.Spawn(spawn, Quaternion.identity, "ElementalGolem", "StoneGolem", "Golem", "Stone");
            _stoneGolem = UnityEngine.Object.Instantiate(prefab, spawn, Quaternion.LookRotation(player.transform.forward));
            SetupSummon(player, _stoneGolem);
            _stoneGolemRemaining = Mathf.Max(1f, EpicLootRaritySetsPlugin.SeidrStoneGolemDuration.Value);
            _stoneGolemCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrStoneGolemCooldown.Value);
            AbilityCooldownBuffController.Start(player, "SeidrStoneGolem", "Stone Golem", _stoneGolemCooldown, null);
            ShowMessage(player, "Stone Golem.");
        }

        private static void TryFrostNova(Player player)
        {
            if (_frostNovaCooldown > 0f)
            {
                ShowMessage(player, string.Format("Frost Nova: {0:0}s cooldown.", _frostNovaCooldown));
                return;
            }

            float eitr = Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrFrostNovaEitrUse.Value);
            if (eitr > 0f && !player.HaveEitr(eitr))
            {
                if (Hud.instance != null)
                {
                    Hud.instance.EitrBarEmptyFlash();
                }
                ShowMessage(player, "Frost Nova: no tienes eitr suficiente.");
                return;
            }

            player.UseEitr(eitr);
            NorseVisualEffectBridge.SpawnAtPlayer(player, "FrostNova", "Frost", "Nova");
            float amount = Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrFrostNovaBaseDamage.Value + player.GetSkillLevel(Skills.SkillType.ElementalMagic) * EpicLootRaritySetsPlugin.SeidrFrostNovaDamagePerElementalMagicLevel.Value);
            HitData.DamageTypes damages = new HitData.DamageTypes { m_frost = amount };
            DamageArea(player, player.GetCenterPoint(), EpicLootRaritySetsPlugin.SeidrFrostNovaRadius.Value, damages);
            _frostNovaCooldown = Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrFrostNovaCooldown.Value);
            AbilityCooldownBuffController.Start(player, "SeidrFrostNova", "Frost Nova", _frostNovaCooldown, null);
            ShowMessage(player, "Frost Nova.");
        }

        private static void UpdateNanoCube(Player player, float dt)
        {
            if (_nanoRemaining <= 0f)
            {
                return;
            }

            _nanoRemaining -= dt;
            if (_nanoRemaining <= 0f)
            {
                if (_nanoBuff != null) player.GetSEMan().RemoveStatusEffect(_nanoBuff.NameHash(), true);
                RemoveNanoDamageBuff(player);
                DestroyNanoVisuals();
                _nanoAreaActive = false;
                _nanoPlayerInside = false;
                return;
            }

            _nanoVisualRefreshTimer -= dt;
            if (_nanoVisualRefreshTimer <= 0f)
            {
                RefreshNanoVisual();
                _nanoVisualRefreshTimer = 2.5f;
            }

            PushEnemiesOut(player);
            RefreshNanoBuff(player);
            bool inside = IsPlayerInsideNano(player);
            _nanoPlayerInside = inside;
            if (inside)
            {
                RefreshNanoDamageBuff(player);
            }
            else
            {
                RemoveNanoDamageBuff(player);
            }
        }

        private static void UpdateElementalShield(Player player, float dt)
        {
            if (!_shieldActive)
            {
                RemoveShieldBuff(player);
                DestroyVisual(ref _shieldVisual);
                return;
            }

            float maxEitr = GetMaxEitr(player);
            float costPerSecond = Mathf.Max(1f, maxEitr * Mathf.Max(0f, EpicLootRaritySetsPlugin.SeidrElementalShieldEitrPercentPerSecond.Value));
            float cost = costPerSecond * dt;
            if (cost > 0f && !player.HaveEitr(cost))
            {
                _shieldActive = false;
                _shieldCooldown = 0f;
                RemoveShieldBuff(player);
                DestroyVisual(ref _shieldVisual);
                AbilityCooldownBuffController.Clear(player, "SeidrElementalShield");
                ShowMessage(player, "Elemental Shield: sin eitr.");
                return;
            }

            player.UseEitr(cost);
            EnsureShieldVisual(player);
            RefreshShieldBuff(player);
        }

        private static void UpdateStoneGolem(float dt)
        {
            if (_stoneGolem == null)
            {
                return;
            }

            _stoneGolemRemaining -= dt;
            Character character = _stoneGolem.GetComponent<Character>();
            if (_stoneGolemRemaining <= 0f || (character != null && character.IsDead()))
            {
                DestroyStoneGolem();
            }
        }

        private static void PushEnemiesOut(Player player)
        {
            float radius = GetNanoLogicRadius();
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                Vector3 offset = character.transform.position - _nanoCenter;
                offset.y = 0f;
                if (offset.sqrMagnitude <= 0.001f || offset.sqrMagnitude > radius * radius)
                {
                    continue;
                }

                Vector3 target = _nanoCenter + offset.normalized * (radius + 0.35f);
                Rigidbody body = character.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.position = new Vector3(target.x, character.transform.position.y, target.z);
                    body.velocity = Vector3.zero;
                }
                character.transform.position = new Vector3(target.x, character.transform.position.y, target.z);
            }
        }

        private static void DamageArea(Player player, Vector3 center, float radius, HitData.DamageTypes damages)
        {
            radius = Mathf.Max(0.1f, radius);
            foreach (Character character in Character.GetAllCharacters())
            {
                if (character == null || character.IsDead() || !IsEnemyTarget(player, character))
                {
                    continue;
                }

                if ((character.GetCenterPoint() - center).sqrMagnitude <= radius * radius)
                {
                    HitData hit = new HitData
                    {
                        m_damage = damages,
                        m_skill = Skills.SkillType.ElementalMagic,
                        m_skillLevel = player.GetSkillLevel(Skills.SkillType.ElementalMagic),
                        m_ranged = true,
                        m_dodgeable = true,
                        m_blockable = true,
                        m_backstabBonus = 1f,
                        m_staggerMultiplier = 1f,
                        m_point = character.GetCenterPoint(),
                        m_dir = character.GetCenterPoint() - center
                    };
                    hit.SetAttacker(player);
                    character.Damage(hit);
                    ApplyFrostNovaSlow(character);
                }
            }
        }

        private static void ApplyFrostNovaSlow(Character character)
        {
            StatusEffect buff = GetOrCreateFrostNovaSlowBuff();
            character.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            character.GetSEMan().AddStatusEffect(buff, true, 0, 0f, 0);
        }

        private static StatusEffect GetOrCreateFrostNovaSlowBuff()
        {
            if (_frostNovaSlowBuff == null)
            {
                _frostNovaSlowBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _frostNovaSlowBuff.name = FrostNovaSlowBuffName;
                _frostNovaSlowBuff.m_name = "Frost Nova Slow";
                _frostNovaSlowBuff.m_category = FrostNovaSlowBuffCategory;
                _frostNovaSlowBuff.m_flashIcon = false;
                _frostNovaSlowBuff.m_cooldownIcon = true;
                _frostNovaSlowBuff.m_hidden = false;
            }

            _frostNovaSlowBuff.m_ttl = Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SeidrFrostNovaSlowDuration.Value);
            _frostNovaSlowBuff.m_tooltip = string.Format("Ralentizado por Frost Nova.\n\nVelocidad: -{0:0.#}%.\nDuracion: {1:0.#}s.", EpicLootRaritySetsPlugin.SeidrFrostNovaSlow.Value * 100f, _frostNovaSlowBuff.m_ttl);
            if (SpeedModifierField != null)
            {
                SpeedModifierField.SetValue(_frostNovaSlowBuff, -Mathf.Clamp01(EpicLootRaritySetsPlugin.SeidrFrostNovaSlow.Value));
            }
            return _frostNovaSlowBuff;
        }

        private static void RefreshNanoBuff(Player player)
        {
            StatusEffect buff = GetOrCreateNanoBuff();
            buff.m_ttl = Mathf.Max(0.1f, _nanoRemaining + 0.1f);
            buff.m_tooltip = string.Format("Nanocube activo.\n\nEnemigos empujados fuera del area.\nDano magico dentro: +{0:0.#}%.\nTiempo restante: {1:0.#}s.", EpicLootRaritySetsPlugin.SeidrNanoCubeMagicDamageBonus.Value * 100f, Mathf.Max(0f, _nanoRemaining));
            Sprite icon = FindSeidrIcon(player);
            if (icon != null)
            {
                buff.m_icon = icon;
            }
            ApplyOrRefreshStatusEffect(player, buff, 0);
        }

        private static void RefreshNanoDamageBuff(Player player)
        {
            if (player == null)
            {
                return;
            }

            StatusEffect buff = GetOrCreateNanoDamageBuff();
            buff.m_ttl = 0.75f;
            buff.m_tooltip = string.Format(
                "Estas dentro del Nanocube.\n\nDano magico aumentado: +{0:0.#}%.\nAfecta fuego, frost, rayo, veneno y espiritu.",
                EpicLootRaritySetsPlugin.SeidrNanoCubeMagicDamageBonus.Value * 100f);
            Sprite icon = FindSeidrIcon(player);
            if (icon != null)
            {
                buff.m_icon = icon;
            }
            ApplyOrRefreshStatusEffect(player, buff, 0);
        }

        private static void ApplyOrRefreshStatusEffect(Player player, StatusEffect buff, int itemLevel)
        {
            if (player == null || buff == null)
            {
                return;
            }

            SEMan seMan = player.GetSEMan();
            StatusEffect active = seMan.GetStatusEffect(buff.NameHash());
            if (active == null)
            {
                seMan.AddStatusEffect(buff, true, itemLevel, 0f, 0);
                return;
            }

            active.m_name = buff.m_name;
            active.m_tooltip = buff.m_tooltip;
            active.m_ttl = buff.m_ttl;
            active.m_icon = buff.m_icon;
            active.m_flashIcon = buff.m_flashIcon;
            active.m_cooldownIcon = buff.m_cooldownIcon;
            active.m_hidden = buff.m_hidden;
            active.m_category = buff.m_category;
            if (StatusEffectTimeField != null)
            {
                try
                {
                    StatusEffectTimeField.SetValue(active, 0f);
                }
                catch
                {
                }
            }
        }

        private static StatusEffect GetOrCreateNanoDamageBuff()
        {
            if (_nanoDamageBuff == null)
            {
                _nanoDamageBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _nanoDamageBuff.name = NanoDamageBuffName;
                _nanoDamageBuff.m_name = "Nanocube Power";
                _nanoDamageBuff.m_category = NanoDamageBuffCategory;
                _nanoDamageBuff.m_flashIcon = false;
                _nanoDamageBuff.m_cooldownIcon = false;
                _nanoDamageBuff.m_hidden = false;
            }

            return _nanoDamageBuff;
        }

        private static void RemoveNanoDamageBuff(Player player)
        {
            if (player != null && _nanoDamageBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_nanoDamageBuff.NameHash(), true);
            }
        }

        private static StatusEffect GetOrCreateNanoBuff()
        {
            if (_nanoBuff == null)
            {
                _nanoBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _nanoBuff.name = NanoBuffName;
                _nanoBuff.m_name = "Nanocube";
                _nanoBuff.m_category = NanoBuffCategory;
                _nanoBuff.m_flashIcon = false;
                _nanoBuff.m_cooldownIcon = true;
                _nanoBuff.m_hidden = false;
            }
            return _nanoBuff;
        }

        private static bool IsPlayerInsideNano(Player player)
        {
            if (player == null || !_nanoAreaActive || _nanoRemaining <= 0f)
            {
                return false;
            }

            float radius = GetNanoLogicRadius();
            return IsInsideNanoAround(player, _nanoCenter, radius);
        }

        private static bool IsInsideNanoAround(Player player, Vector3 center, float radius)
        {
            return IsInsideNanoPoint(player.transform.position, center, radius) ||
                   IsInsideNanoPoint(player.GetCenterPoint(), center, radius);
        }

        private static bool IsInsideNanoPoint(Vector3 point, Vector3 center, float radius)
        {
            Vector3 offset = point - center;
            float vertical = Mathf.Abs(offset.y);
            offset.y = 0f;
            return vertical <= GetNanoLogicHalfHeight() && offset.sqrMagnitude <= radius * radius;
        }

        private static float GetNanoLogicRadius()
        {
            return Mathf.Max(0.1f, EpicLootRaritySetsPlugin.SeidrNanoCubeRadius.Value) + 0.75f;
        }

        private static float GetNanoLogicHalfHeight()
        {
            return Mathf.Max(4f, EpicLootRaritySetsPlugin.SeidrNanoCubeRadius.Value + 2f);
        }

        private static bool IsNanoDamageActiveFor(Player player)
        {
            if (player == null || !_nanoAreaActive || _nanoRemaining <= 0f)
            {
                return false;
            }

            bool inside = IsPlayerInsideNano(player);
            if (inside != _nanoPlayerInside)
            {
                _nanoPlayerInside = inside;
                if (inside)
                {
                    RefreshNanoDamageBuff(player);
                }
                else
                {
                    RemoveNanoDamageBuff(player);
                }
            }

            return inside;
        }

        private static void RefreshNanoVisual()
        {
            if (_nanoRemaining <= 0f)
            {
                return;
            }

            GameObject visual = NorseVisualEffectBridge.Spawn(
                _nanoCenter,
                Quaternion.identity,
                Mathf.Max(0.2f, _nanoRemaining + 0.2f),
                "FxNanoCube",
                "NanoCube",
                "Nanocube",
                "Cube");
            if (visual != null)
            {
                NanoVisuals.Add(visual);
            }
        }

        private static void DestroyNanoVisuals()
        {
            for (int i = NanoVisuals.Count - 1; i >= 0; i--)
            {
                GameObject visual = NanoVisuals[i];
                if (visual != null)
                {
                    UnityEngine.Object.Destroy(visual);
                }
            }

            NanoVisuals.Clear();
        }

        private static void RefreshShieldBuff(Player player)
        {
            StatusEffect buff = GetOrCreateShieldBuff();
            buff.m_ttl = 1.2f;
            buff.m_tooltip = string.Format("Inmune al dano mientras haya eitr.\n\nToggle libre sin cooldown.\nConsume {0:0.#}% del eitr maximo por segundo, minimo 1 eitr/s.", EpicLootRaritySetsPlugin.SeidrElementalShieldEitrPercentPerSecond.Value * 100f);
            player.GetSEMan().RemoveStatusEffect(buff.NameHash(), true);
            player.GetSEMan().AddStatusEffect(buff, true, 0, 0f, 0);
        }

        private static StatusEffect GetOrCreateShieldBuff()
        {
            if (_shieldBuff == null)
            {
                _shieldBuff = ScriptableObject.CreateInstance<SE_Stats>();
                _shieldBuff.name = ShieldBuffName;
                _shieldBuff.m_name = "Elemental Shield";
                _shieldBuff.m_category = ShieldBuffCategory;
                _shieldBuff.m_flashIcon = false;
                _shieldBuff.m_cooldownIcon = false;
                _shieldBuff.m_hidden = false;
            }
            return _shieldBuff;
        }

        private static void RemoveShieldBuff(Player player)
        {
            if (player != null && _shieldBuff != null)
            {
                player.GetSEMan().RemoveStatusEffect(_shieldBuff.NameHash(), true);
            }
        }

        private static bool HasShieldBuff(Player player)
        {
            if (player == null || _shieldBuff == null)
            {
                return false;
            }

            try
            {
                return player.GetSEMan().HaveStatusEffect(_shieldBuff.NameHash());
            }
            catch
            {
                return false;
            }
        }

        private static void EnsureShieldVisual(Player player)
        {
            if (player == null || !_shieldActive || _shieldVisual != null)
            {
                return;
            }

            _shieldVisual = NorseVisualEffectBridge.SpawnAttached(
                player,
                Vector3.up * 1.0f,
                Quaternion.identity,
                1.4f,
                "FxLightningShield",
                "FxArcaneShield",
                "ElementalShield",
                "Shield",
                "FxLightning");
        }

        private static void DestroyVisual(ref GameObject visual)
        {
            if (visual != null)
            {
                UnityEngine.Object.Destroy(visual);
                visual = null;
            }
        }

        private static void SetupSummon(Player player, GameObject summon)
        {
            if (summon == null) return;
            Character character = summon.GetComponent<Character>();
            if (character != null) character.SetTamed(true);
            Tameable tameable = summon.GetComponent<Tameable>() ?? summon.AddComponent<Tameable>();
            if (tameable != null)
            {
                tameable.m_commandable = true;
                tameable.m_fedDuration = Mathf.Max(tameable.m_fedDuration, 3600f);
                if (TameMethod != null)
                {
                    try { TameMethod.Invoke(tameable, null); } catch { }
                }
            }
            MonsterAI monsterAI = summon.GetComponent<MonsterAI>();
            if (monsterAI != null)
            {
                monsterAI.MakeTame();
                monsterAI.SetFollowTarget(player.gameObject);
            }
            ZNetView view = summon.GetComponent<ZNetView>();
            if (view != null && view.GetZDO() != null) view.GetZDO().Persistent = false;
            ApplyGhostVisual(summon);
            NorseVisualEffectBridge.SpawnAttached(summon, Vector3.up * 1.2f, Quaternion.identity, Mathf.Max(1f, EpicLootRaritySetsPlugin.SeidrStoneGolemDuration.Value), "ElementalGolem", "StoneGolem", "Golem", "Spirit", "Ghost");
        }

        private static void ApplyGhostVisual(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                Material[] materials = renderer.materials;
                foreach (Material material in materials)
                {
                    if (material == null)
                    {
                        continue;
                    }

                    Color color = material.HasProperty("_Color") ? material.color : Color.white;
                    color.a = Mathf.Min(color.a <= 0.05f ? 0.55f : color.a, 0.55f);
                    if (material.HasProperty("_Color"))
                    {
                        material.color = color;
                    }

                    Color tintColor = material.HasProperty("_TintColor") ? material.GetColor("_TintColor") : color;
                    tintColor.a = Mathf.Min(tintColor.a <= 0.05f ? 0.55f : tintColor.a, 0.55f);
                    if (material.HasProperty("_TintColor"))
                    {
                        material.SetColor("_TintColor", tintColor);
                    }

                    if (material.HasProperty("_EmissionColor"))
                    {
                        material.SetColor("_EmissionColor", new Color(0.45f, 0.75f, 1f, 0.55f));
                    }

                    if (material.HasProperty("_EmissionColor"))
                    {
                        material.EnableKeyword("_EMISSION");
                    }

                    material.SetOverrideTag("RenderType", "Transparent");
                    if (material.HasProperty("_Mode"))
                    {
                        material.SetFloat("_Mode", 3f);
                    }

                    if (material.HasProperty("_SrcBlend"))
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    }

                    if (material.HasProperty("_DstBlend"))
                    {
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    }

                    if (material.HasProperty("_ZWrite"))
                    {
                        material.SetInt("_ZWrite", 1);
                    }

                    material.renderQueue = 3000;
                }
            }
        }

        private static void DestroyStoneGolem()
        {
            if (_stoneGolem != null)
            {
                if (ZNetScene.instance != null) ZNetScene.instance.Destroy(_stoneGolem);
                else UnityEngine.Object.Destroy(_stoneGolem);
            }
            _stoneGolem = null;
            _stoneGolemRemaining = 0f;
        }

        private static GameObject GetPrefab(params string[] names)
        {
            if (ZNetScene.instance == null || names == null) return null;
            foreach (string name in names)
            {
                GameObject prefab = ZNetScene.instance.GetPrefab(name);
                if (prefab != null && (prefab.GetComponent<Character>() != null || prefab.GetComponentInChildren<Character>() != null)) return prefab;
            }
            return null;
        }

        private static float GetMaxEitr(Player player)
        {
            try
            {
                if (GetMaxEitrMethod != null) return Mathf.Max(0f, Convert.ToSingle(GetMaxEitrMethod.Invoke(player, null)));
            }
            catch { }
            return 100f;
        }

        private static bool IsEnabledAndActive()
        {
            return EpicLootRaritySetsPlugin.EnableSeidrAbilities != null &&
                   EpicLootRaritySetsPlugin.EnableSeidrAbilities.Value &&
                   SetActivationBuffController.HasActiveSet(RequiredSet);
        }

        private static bool CanReadInput(Player player)
        {
            return !player.IsDead() && !player.IsTeleporting() && !IsAnyMenuOpen();
        }

        private static bool IsShortcutDown(ConfigEntry<KeyboardShortcut> entry)
        {
            if (entry == null) return false;
            KeyboardShortcut shortcut = entry.Value;
            if (shortcut.IsDown()) return true;
            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey)) return false;
            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier)) return false;
            }
            return true;
        }

        private static bool IsAnyMenuOpen()
        {
            return (InventoryGui.instance != null && InventoryGui.IsVisible()) ||
                   (Menu.instance != null && Menu.IsVisible()) ||
                   (TextInput.instance != null && TextInput.IsVisible()) ||
                   (Chat.instance != null && Chat.instance.HasFocus()) ||
                   (Minimap.instance != null && Minimap.IsOpen());
        }

        private static bool IsEnemyTarget(Player player, Character target)
        {
            if (player == null || target == null || target == player || target is Player) return false;
            if (BaseAIIsEnemyMethod != null)
            {
                try
                {
                    object value = BaseAIIsEnemyMethod.Invoke(null, new object[] { player, target });
                    if (value is bool) return (bool)value;
                }
                catch { }
            }
            return true;
        }

        private static Sprite FindSeidrIcon(Player player)
        {
            ItemDrop.ItemData weapon = player != null ? player.GetCurrentWeapon() : null;
            Sprite weaponIcon = GetItemIcon(weapon);
            if (weaponIcon != null)
            {
                return weaponIcon;
            }

            Inventory inventory = player != null ? player.GetInventory() : null;
            if (inventory == null)
            {
                return null;
            }

            foreach (ItemDrop.ItemData item in inventory.GetEquippedItems())
            {
                Sprite icon = GetItemIcon(item);
                if (icon == null)
                {
                    continue;
                }

                MagicItem magicItem = ItemDataExtensions.GetMagicItem(item);
                if (magicItem != null && (IsSeidrId(magicItem.SetID) || IsSeidrId(magicItem.LegendaryID)))
                {
                    return icon;
                }
            }

            return null;
        }

        private static Sprite GetItemIcon(ItemDrop.ItemData item)
        {
            if (item == null || item.m_shared == null || item.m_shared.m_icons == null || item.m_shared.m_icons.Length == 0)
            {
                return null;
            }

            return item.m_shared.m_icons[0];
        }

        private static bool IsSeidrId(string id)
        {
            return !string.IsNullOrEmpty(id) && id.IndexOf("Seidr", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void ShowMessage(Player player, string message)
        {
            if (player != null) player.Message(MessageHud.MessageType.Center, message, 0, null, false);
        }
    }

    internal static class BossSetDropController
    {
        private static readonly Dictionary<string, BossSetDropRule> RulesByObject = new Dictionary<string, BossSetDropRule>(StringComparer.OrdinalIgnoreCase);

        [ThreadStatic]
        private static BossLootContext _context;

        [ThreadStatic]
        private static Stack<string> _lootDropItems;

        internal static void Load()
        {
            RulesByObject.Clear();

            if (EpicLootRaritySetsPlugin.ReadBossSetDropRules != null && !EpicLootRaritySetsPlugin.ReadBossSetDropRules.Value)
            {
                return;
            }

            string path = Path.Combine(Paths.ConfigPath, "EpicLoot", "bosssetdrops.json");
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                BossSetDropConfig config = JsonConvert.DeserializeObject<BossSetDropConfig>(File.ReadAllText(path));
                if (config == null || !config.Enabled || config.Bosses == null)
                {
                    return;
                }

                foreach (BossSetDropRule rule in config.Bosses)
                {
                    if (rule == null || string.IsNullOrEmpty(rule.Object) || !rule.Enabled)
                    {
                        continue;
                    }

                    rule.GuaranteedSetDrops = Math.Max(0, rule.GuaranteedSetDrops);
                    rule.ExtraSetDropChance = Mathf.Clamp01(rule.ExtraSetDropChance);
                    if (rule.ForceSetDropItems == null)
                    {
                        rule.ForceSetDropItems = new List<string>();
                    }

                    RulesByObject[rule.Object] = rule;
                }

                EpicLootRaritySetsPlugin.Log.LogInfo(string.Format("Loaded boss set drop rules: {0}", RulesByObject.Count));
            }
            catch (Exception ex)
            {
                EpicLootRaritySetsPlugin.Log.LogError("Failed to load boss set drop rules from " + path + ": " + ex);
            }
        }

        internal static void EnterLootTable(string objectName, int level)
        {
            if (_context != null)
            {
                _context.Depth++;
                return;
            }

            BossSetDropRule rule;
            if (string.IsNullOrEmpty(objectName) || !RulesByObject.TryGetValue(objectName, out rule))
            {
                return;
            }

            _context = new BossLootContext(rule, objectName, level);
        }

        internal static void ExitLootTable()
        {
            if (_context == null)
            {
                return;
            }

            _context.Depth--;
            if (_context.Depth <= 0)
            {
                _context = null;
            }
        }

        internal static void PushLootDrop(LootDrop lootDrop)
        {
            if (_lootDropItems == null)
            {
                _lootDropItems = new Stack<string>();
            }

            _lootDropItems.Push(lootDrop != null ? lootDrop.Item : null);
        }

        internal static void PopLootDrop()
        {
            if (_lootDropItems == null || _lootDropItems.Count == 0)
            {
                return;
            }

            _lootDropItems.Pop();
        }

        internal static bool HasActiveBossRule()
        {
            return _context != null && _context.Rule != null;
        }

        internal static bool TryGetBossSetDropChance(out float chance)
        {
            chance = 0f;

            if (!HasActiveBossRule())
            {
                return false;
            }

            string lootDropItem = GetCurrentLootDropItem();
            if (!string.IsNullOrEmpty(lootDropItem) && _context.Rule.ForceSetDropItems.Contains(lootDropItem))
            {
                chance = 1f;
                return true;
            }

            chance = _context.SetDropsMade < _context.Rule.GuaranteedSetDrops ? 1f : _context.Rule.ExtraSetDropChance;
            return true;
        }

        internal static void RecordSetDrop()
        {
            if (HasActiveBossRule())
            {
                _context.SetDropsMade++;
            }
        }

        private static string GetCurrentLootDropItem()
        {
            if (_lootDropItems == null || _lootDropItems.Count == 0)
            {
                return null;
            }

            return _lootDropItems.Peek();
        }

        private class BossLootContext
        {
            public readonly BossSetDropRule Rule;
            public int Depth;
            public int SetDropsMade;

            public BossLootContext(BossSetDropRule rule, string objectName, int level)
            {
                Rule = rule;
                Depth = 1;
                SetDropsMade = 0;
            }
        }

        #pragma warning disable 0649
        private class BossSetDropConfig
        {
            public bool Enabled = true;
            public List<BossSetDropRule> Bosses;
        }

        private class BossSetDropRule
        {
            public string Object;
            public bool Enabled = true;
            public int GuaranteedSetDrops = 1;
            public float ExtraSetDropChance = 0.4f;
            public List<string> ForceSetDropItems;
        }
        #pragma warning restore 0649
    }

    internal static class MoonveinBowEitrUse
    {
        internal static float GetCost(Character character, ItemDrop.ItemData weapon)
        {
            MagicItem magicItem;
            if (!IsMoonveinBow(weapon, out magicItem))
            {
                return 0f;
            }

            float baseCost = GetBaseCost(magicItem.Rarity);
            if (baseCost <= 0f)
            {
                return 0f;
            }

            Player player = character as Player;
            if (player == null)
            {
                return baseCost;
            }

            float costMultiplier = ModifyAttackCosts.GetModifyAttackValue(player, weapon, MagicEffectType.ModifyAttackEitrUse);
            if (float.IsNaN(costMultiplier) || float.IsInfinity(costMultiplier))
            {
                return baseCost;
            }

            return Mathf.Max(0f, baseCost * costMultiplier);
        }

        internal static bool IsMoonveinBow(ItemDrop.ItemData weapon, out MagicItem magicItem)
        {
            magicItem = null;
            if (weapon == null || weapon.m_shared == null || weapon.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Bow)
            {
                return false;
            }

            magicItem = ItemDataExtensions.GetMagicItem(weapon);
            if (magicItem == null)
            {
                return false;
            }

            return IsMoonveinId(magicItem.SetID) || IsMoonveinId(magicItem.LegendaryID);
        }

        private static bool IsMoonveinId(string id)
        {
            return !string.IsNullOrEmpty(id) && id.IndexOf("Moonvein", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static float GetBaseCost(ItemRarity rarity)
        {
            if (rarity == ItemRarity.Magic)
            {
                return EpicLootRaritySetsPlugin.MoonveinMagicBowEitrUse.Value;
            }

            if (rarity == ItemRarity.Rare)
            {
                return EpicLootRaritySetsPlugin.MoonveinRareBowEitrUse.Value;
            }

            if (rarity == ItemRarity.Epic)
            {
                return EpicLootRaritySetsPlugin.MoonveinEpicBowEitrUse.Value;
            }

            if (rarity == ItemRarity.Legendary)
            {
                return EpicLootRaritySetsPlugin.MoonveinLegendaryBowEitrUse.Value;
            }

            if (rarity == ItemRarity.Mythic)
            {
                return EpicLootRaritySetsPlugin.MoonveinMythicBowEitrUse.Value;
            }

            if (rarity == ItemRarity.Ancient)
            {
                return EpicLootRaritySetsPlugin.MoonveinAncientBowEitrUse.Value;
            }

            return 0f;
        }
    }

    [HarmonyPatch(typeof(Attack), "GetAttackEitr", new Type[] { typeof(Character), typeof(ItemDrop.ItemData) })]
    [HarmonyAfter(new[] { "randyknapp.mods.epicloot" })]
    internal static class MoonveinBowAttackEitrPatch
    {
        private static void Postfix(Character character, ItemDrop.ItemData weapon, ref float __result)
        {
            if (__result > 0f)
            {
                return;
            }

            float moonveinCost = MoonveinBowEitrUse.GetCost(character, weapon);
            if (moonveinCost > 0f)
            {
                __result = moonveinCost;
            }
        }
    }

    [HarmonyPatch(typeof(Humanoid), "StartAttack", new Type[] { typeof(Character), typeof(bool) })]
    internal static class FrostbrandSecondaryAttackConsumePatch
    {
        private static bool Prefix(Humanoid __instance, bool secondaryAttack, ref bool __result)
        {
            if (!secondaryAttack)
            {
                return true;
            }

            Player player = __instance as Player;
            if (!FrostbrandAbilityController.TryConsumeSecondarySlash(player))
            {
                return true;
            }

            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(Attack), "Start")]
    internal static class FrostbrandWeaponAttackStartPatch
    {
        private static void Postfix(bool __result, [HarmonyArgument(0)] Humanoid character, [HarmonyArgument(5)] ItemDrop.ItemData weapon)
        {
            if (!__result)
            {
                return;
            }

            FrostbrandAbilityController.OnWeaponAttackStarted(character as Player, weapon);
        }
    }

    [HarmonyPatch(typeof(Attack), "FireProjectileBurst")]
    internal static class MoonveinBowFireProjectileBurstPatch
    {
        private static void Postfix(Attack __instance, Humanoid ___m_character, ItemDrop.ItemData ___m_weapon)
        {
            Player player = ___m_character as Player;
            if (player == null)
            {
                return;
            }

            MoonveinAbilityController.OnMoonveinBowShot(player, __instance, ___m_weapon);
        }
    }

    [HarmonyPatch]
    internal static class MoonveinTornadoProjectileSetupPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(Projectile))
                .FirstOrDefault(method =>
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    return method.Name == "Setup" &&
                           parameters.Length >= 2 &&
                           typeof(Character).IsAssignableFrom(parameters[0].ParameterType) &&
                           parameters[1].ParameterType == typeof(Vector3);
                });
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static void Postfix(Projectile __instance, object[] __args)
        {
            if (__args == null || __args.Length == 0)
            {
                return;
            }

            Player player = __args[0] as Player;
            ItemDrop.ItemData weapon = __args.Length >= 5 ? __args[4] as ItemDrop.ItemData : null;
            MoonveinAbilityController.TryMarkTornadoProjectile(__instance, player, weapon);
            SolomonKaneAbilityController.TryMarkProjectile(__instance, player, weapon);
        }
    }

    [HarmonyPatch]
    internal static class MoonveinTornadoProjectileHitPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(Projectile))
                .FirstOrDefault(method => method.Name == "OnHit");
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static void Prefix(Projectile __instance, object[] __args)
        {
            HraesvelgrAbilityController.BeginHeadshotProjectile(__instance);
            Vector3 hitPoint = __instance != null ? __instance.transform.position : Vector3.zero;
            if (__args != null)
            {
                foreach (object arg in __args)
                {
                    if (arg is Vector3)
                    {
                        hitPoint = (Vector3)arg;
                        break;
                    }
                }
            }

            MoonveinAbilityController.OnTornadoProjectileHit(__instance, hitPoint);
            SolomonKaneAbilityController.BeginProjectileHit(__instance, hitPoint);
        }

        private static void Finalizer()
        {
            SolomonKaneAbilityController.EndProjectileHit();
            HraesvelgrAbilityController.EndHeadshotProjectile();
        }
    }

    [HarmonyPatch(typeof(Player), "Update")]
    internal static class PlayerSetActivationBuffUpdatePatch
    {
        private static void Postfix(Player __instance)
        {
            SetActivationBuffController.Update(__instance, Time.deltaTime);
            AbilityCooldownBuffController.Update(__instance, Time.deltaTime);
            FrostbrandAbilityController.Update(__instance, Time.deltaTime);
            HraesvelgrAbilityController.Update(__instance, Time.deltaTime);
            SolomonKaneAbilityController.Update(__instance, Time.deltaTime);
            MoonveinAbilityController.Update(__instance, Time.deltaTime);
            NottAbilityController.Update(__instance, Time.deltaTime);
            RagnarAbilityController.Update(__instance, Time.deltaTime);
            HelveigAbilityController.Update(__instance, Time.deltaTime);
            HeimdallAbilityController.Update(__instance, Time.deltaTime);
            SeidrAbilityController.Update(__instance, Time.deltaTime);
            if (__instance == Player.m_localPlayer &&
                !HraesvelgrAbilityController.ShouldKeepSneakyVisual() &&
                !NottAbilityController.ShouldKeepSneakyVisual())
            {
                NorseSneakyVisualBridge.Reset();
            }

            if (__instance == Player.m_localPlayer && NorseDemigodsSuppression.ShouldSuppressLocal())
            {
                NorseDemigodsSuppression.RemoveMeters();
            }
        }
    }

    [HarmonyPatch]
    internal static class HraesvelgrRapidVolleyEquipmentEffectTryGetValuePatch
    {
        private static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("EpicLoot.EquipmentEffectCache");
            return type != null ? AccessTools.Method(type, "TryGetValue", new[] { typeof(Player), typeof(string), typeof(float?).MakeByRefType() }) : null;
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static void Postfix(Player __0, string __1, ref float? __2, ref bool __result)
        {
            float? value;
            if (!HraesvelgrAbilityController.TryGetRapidVolleyEffectValue(__0, __1, out value))
            {
                return;
            }

            __2 = value;
            __result = true;
        }
    }

    [HarmonyPatch]
    internal static class HraesvelgrRapidVolleyEquipmentEffectGetPatch
    {
        private static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("EpicLoot.EquipmentEffectCache");
            return type != null ? AccessTools.Method(type, "Get", new[] { typeof(Player), typeof(string), typeof(Func<float?>) }) : null;
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static void Postfix(Player __0, string __1, ref float? __result)
        {
            float? value;
            if (HraesvelgrAbilityController.TryGetRapidVolleyEffectValue(__0, __1, out value))
            {
                __result = value;
            }
        }
    }

    [HarmonyPatch]
    internal static class HraesvelgrRapidVolleyEquipmentEffectValuesForPatch
    {
        private static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("EpicLoot.EquipmentEffectCache");
            return type != null ? AccessTools.Method(type, "ValuesFor", new[] { typeof(Player) }) : null;
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static void Postfix(Player __0, Dictionary<string, float?> __result)
        {
            HraesvelgrAbilityController.ApplyRapidVolleyEffectValues(__0, __result);
        }
    }

    [HarmonyPatch]
    internal static class HraesvelgrRapidVolleyAttackUpdatePatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(Attack))
                .FirstOrDefault(method =>
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    return method.Name == "Update" &&
                           parameters.Length == 1 &&
                           parameters[0].ParameterType == typeof(float);
                });
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static void Prefix(Attack __instance, [HarmonyArgument(0)] ref float dt)
        {
            if (HraesvelgrAbilityController.ShouldAccelerateAttack(__instance))
            {
                dt *= EpicLootRaritySetsPlugin.HraesvelgrVolleyAttackSpeedMultiplier.Value;
            }

            float ragnarMultiplier = RagnarAbilityController.GetAttackSpeedMultiplier(__instance);
            if (ragnarMultiplier > 1f)
            {
                dt *= ragnarMultiplier;
            }
        }
    }

    [HarmonyPatch]
    [HarmonyAfter(new[] { "randyknapp.mods.epicloot" })]
    internal static class HraesvelgrRapidVolleyAttackStaminaPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(Attack), "GetAttackStamina", new[] { typeof(Character), typeof(ItemDrop.ItemData) });
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static void Postfix(Character character, ItemDrop.ItemData weapon, ref float __result)
        {
            Player player = character as Player;
            if (!HraesvelgrAbilityController.ShouldAccelerateProjectile(player, weapon))
            {
                return;
            }

            __result *= EpicLootRaritySetsPlugin.HraesvelgrVolleyStaminaUseMultiplier.Value;
        }
    }

    [HarmonyPatch]
    internal static class HraesvelgrRapidVolleyProjectileSetupPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(Projectile))
                .FirstOrDefault(method =>
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    return method.Name == "Setup" &&
                           parameters.Length >= 2 &&
                           typeof(Character).IsAssignableFrom(parameters[0].ParameterType) &&
                           parameters[1].ParameterType == typeof(Vector3);
                });
        }

        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static void Prefix(Projectile __instance, object[] __args, [HarmonyArgument(1)] ref Vector3 velocity)
        {
            if (__args == null || __args.Length < 2)
            {
                return;
            }

            Player player = __args[0] as Player;
            ItemDrop.ItemData weapon = __args.Length >= 5 ? __args[4] as ItemDrop.ItemData : null;
            HraesvelgrAbilityController.OnHraesvelgrProjectileSetup(__instance, player, weapon);
            if (!HraesvelgrAbilityController.ShouldAccelerateProjectile(player, weapon))
            {
                return;
            }

            HraesvelgrAbilityController.ModifyProjectileDamageArgument(__args);
            velocity *= EpicLootRaritySetsPlugin.HraesvelgrVolleyProjectileSpeedMultiplier.Value;
        }
    }

    [HarmonyPatch(typeof(BaseAI), "IsEnemy", new[] { typeof(Character), typeof(Character) })]
    internal static class HraesvelgrSummonSameKindEnemyPatch
    {
        private static void Postfix(Character a, Character b, ref bool __result)
        {
            if (__result)
            {
                return;
            }

            if (HraesvelgrAbilityController.ShouldSummonTreatAsEnemy(a, b))
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch]
    internal static class HraesvelgrSneakyAIBoolPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in new[] { typeof(BaseAI), typeof(MonsterAI) })
            {
                foreach (MethodInfo method in AccessTools.GetDeclaredMethods(type))
                {
                    if (method == null || method.ReturnType != typeof(bool))
                    {
                        continue;
                    }

                    string name = method.Name;
                    bool isSightOrTargetMethod =
                        name.IndexOf("See", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Sense", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Target", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!isSightOrTargetMethod)
                    {
                        continue;
                    }

                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Any(parameter =>
                            typeof(Character).IsAssignableFrom(parameter.ParameterType) ||
                            parameter.ParameterType == typeof(GameObject)))
                    {
                        yield return method;
                    }
                }
            }
        }

        private static void Postfix(object[] __args, ref bool __result)
        {
            if (!__result || __args == null)
            {
                return;
            }

            foreach (object arg in __args)
            {
                Character character = arg as Character;
                if (character == null)
                {
                    GameObject gameObject = arg as GameObject;
                    character = gameObject != null ? gameObject.GetComponent<Character>() : null;
                }

                if (HraesvelgrAbilityController.IsSneakyInvisibleTarget(character) ||
                    NottAbilityController.IsSneakyInvisibleTarget(character))
                {
                    __result = false;
                    return;
                }
            }
        }
    }

    [HarmonyPatch]
    internal static class HraesvelgrSneakyAICharacterPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in new[] { typeof(BaseAI), typeof(MonsterAI) })
            {
                foreach (MethodInfo method in AccessTools.GetDeclaredMethods(type))
                {
                    if (method == null || !typeof(Character).IsAssignableFrom(method.ReturnType))
                    {
                        continue;
                    }

                    string name = method.Name;
                    if (name.IndexOf("Target", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        yield return method;
                    }
                }
            }
        }

        private static void Postfix(ref Character __result)
        {
            if (HraesvelgrAbilityController.IsSneakyInvisibleTarget(__result) ||
                NottAbilityController.IsSneakyInvisibleTarget(__result))
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(Character), "OnDeath")]
    internal static class NottWarpCooldownDeathResetPatch
    {
        private static void Postfix(Character __instance)
        {
            NottAbilityController.OnCharacterDeath(__instance);
        }
    }

    [HarmonyPatch(typeof(Character), "Damage", new[] { typeof(HitData) })]
    internal static class NottWarpStrikeDamagePatch
    {
        private static void Prefix(Character __instance, HitData hit)
        {
            Player defender = __instance as Player;
            if (defender != null && defender == Player.m_localPlayer)
            {
                FrostbrandAbilityController.ModifyIncomingDamage(defender, hit);
                HeimdallAbilityController.ModifyIncomingDamage(defender, hit);
                SeidrAbilityController.ModifyIncomingDamage(defender, hit);
                HeimdallAbilityController.TrackIncomingBlockCandidate(defender, hit);
            }

            SeidrAbilityController.ModifyOutgoingDamage(hit);
            HraesvelgrAbilityController.ModifyOutgoingDamage(__instance, hit);
            SolomonKaneAbilityController.ModifyOutgoingDamage(__instance, hit);
            HeimdallAbilityController.ModifyOutgoingDamage(hit);
            NottAbilityController.ApplyPoison(__instance, hit);
            NottAbilityController.TryConsumeWarpStrike(__instance, hit);
        }

        private static void Postfix(Character __instance, HitData hit)
        {
            Player defender = __instance as Player;
            if (defender != null && defender == Player.m_localPlayer)
            {
                HeimdallAbilityController.OnDamaged(defender, hit);
            }

            FrostbrandAbilityController.OnPlayerHit(__instance, hit);
            SolomonKaneAbilityController.OnPlayerHit(__instance, hit);
            NottAbilityController.OnPlayerHit(__instance, hit);
            RagnarAbilityController.OnPlayerHit(__instance, hit);
        }
    }

    [HarmonyPatch(typeof(Character), "ApplyDamage", new Type[] { typeof(HitData), typeof(bool), typeof(bool), typeof(HitData.DamageModifier) })]
    internal static class HeimdallApplyDamageBlockDetectionPatch
    {
        private static void Prefix(Character __instance, HitData hit)
        {
            HeimdallAbilityController.DetectApplyDamageBlock(__instance as Player, hit);
        }
    }

    [HarmonyPatch]
    internal static class HeimdallSuccessfulBlockPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in new[] { typeof(Humanoid), typeof(Character), typeof(Player) })
            {
                foreach (MethodInfo method in AccessTools.GetDeclaredMethods(type))
                {
                    if (method == null || method.Name != "BlockAttack")
                    {
                        continue;
                    }

                    yield return method;
                }
            }
        }

        private static void Postfix(object __instance, HitData hit, bool __result)
        {
            Player player = __instance as Player;
            if (player == null)
            {
                Humanoid humanoid = __instance as Humanoid;
                player = humanoid as Player;
            }

            HeimdallAbilityController.OnBlockAttackResult(player, hit, __result);
        }
    }

    [HarmonyPatch]
    internal static class HeimdallSuccessfulBlockResultPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            MethodInfo characterBlock = AccessTools.Method(typeof(Character), "BlockAttack", new[] { typeof(HitData), typeof(Character) });
            if (characterBlock != null)
            {
                yield return characterBlock;
            }

            MethodInfo humanoidBlock = AccessTools.Method(typeof(Humanoid), "BlockAttack", new[] { typeof(HitData), typeof(Character) });
            if (humanoidBlock != null)
            {
                yield return humanoidBlock;
            }
        }

        private static void Postfix(object __instance, HitData hit, bool __result)
        {
            HeimdallAbilityController.OnBlockAttackResult(__instance as Player, hit, __result);
        }
    }

    [HarmonyPatch]
    internal static class NorseDemigodsClassUpdateSuppressionPatch
    {
        private static bool Prepare()
        {
            return NorseDemigodsSuppression.FindNorseType("NorseDemigods.Demigod") != null;
        }

        private static MethodBase TargetMethod()
        {
            Type type = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Demigod");
            return type != null ? AccessTools.Method(type, "Update") : null;
        }

        private static bool Prefix()
        {
            if (!NorseDemigodsSuppression.ShouldSuppressLocal())
            {
                return true;
            }

            NorseDemigodsSuppression.RemoveMeters();
            return false;
        }
    }

    [HarmonyPatch]
    internal static class NorseDemigodsAbilityInputSuppressionPatch
    {
        private static bool Prepare()
        {
            return NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility") != null;
        }

        private static MethodBase TargetMethod()
        {
            Type type = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility");
            return type != null ? AccessTools.Method(type, "ProcessAbilityInput") : null;
        }

        private static bool Prefix()
        {
            return !NorseEirAbilityBridge.IsBridgeUpdating &&
                   !NorseWaterSphereBridge.IsBridgeUpdating &&
                   !NorseNjordTornadoBridge.IsBridgeUpdating &&
                   !NorseSurtAbilityBridge.IsBridgeUpdating &&
                   !NorseWarpBridge.IsBridgeExecuting &&
                   !NorseDemigodsSuppression.ShouldSuppressLocal();
        }
    }

    [HarmonyPatch]
    internal static class NorseDemigodsMeterSuppressionPatch
    {
        private static bool Prepare()
        {
            return NorseDemigodsSuppression.FindNorseType("NorseDemigods.CustomUI") != null;
        }

        private static IEnumerable<MethodBase> TargetMethods()
        {
            Type type = NorseDemigodsSuppression.FindNorseType("NorseDemigods.CustomUI");
            if (type == null)
            {
                yield break;
            }

            MethodInfo buildMeters = AccessTools.Method(type, "BuildMeters");
            if (buildMeters != null)
            {
                yield return buildMeters;
            }

            MethodInfo updateMeters = AccessTools.Method(type, "UpdateMeters");
            if (updateMeters != null)
            {
                yield return updateMeters;
            }
        }

        private static bool Prefix()
        {
            if (!NorseDemigodsSuppression.ShouldSuppressLocal())
            {
                return true;
            }

            NorseDemigodsSuppression.RemoveMeters();
            return false;
        }
    }

    [HarmonyPatch]
    internal static class NorseDemigodsAbilityCanExecuteSuppressionPatch
    {
        private static bool Prepare()
        {
            return NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility") != null;
        }

        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in GetNorseAbilityTypes())
            {
                MethodInfo method = type.GetMethod("CanExecute", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(bool) }, null);
                if (method != null && method.DeclaringType == type)
                {
                    yield return method;
                }
            }
        }

        private static bool Prefix(ref bool __result)
        {
            if (!NorseDemigodsSuppression.ShouldSuppressLocal() ||
                NorseEirAbilityBridge.IsBridgeExecuting ||
                NorseWaterSphereBridge.IsBridgeExecuting ||
                NorseNjordTornadoBridge.IsBridgeExecuting ||
                NorseSurtAbilityBridge.IsBridgeExecuting ||
                NorseWarpBridge.IsBridgeExecuting)
            {
                return true;
            }

            __result = false;
            return false;
        }

        internal static IEnumerable<Type> GetNorseAbilityTypes()
        {
            Type baseType = NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility");
            if (baseType == null)
            {
                yield break;
            }

            Type[] types;
            try
            {
                types = baseType.Assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(type => type != null).ToArray();
            }

            foreach (Type type in types)
            {
                if (type != null && baseType.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }
    }

    [HarmonyPatch]
    internal static class NorseDemigodsAbilityExecuteSuppressionPatch
    {
        private static bool Prepare()
        {
            return NorseDemigodsSuppression.FindNorseType("NorseDemigods.DemigodAbility") != null;
        }

        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in NorseDemigodsAbilityCanExecuteSuppressionPatch.GetNorseAbilityTypes())
            {
                MethodInfo method = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (method != null && method.DeclaringType == type)
                {
                    yield return method;
                }
            }
        }

        private static bool Prefix()
        {
            return !NorseDemigodsSuppression.ShouldSuppressLocal() ||
                   NorseEirAbilityBridge.IsBridgeExecuting ||
                   NorseWaterSphereBridge.IsBridgeExecuting ||
                   NorseNjordTornadoBridge.IsBridgeExecuting ||
                   NorseSurtAbilityBridge.IsBridgeExecuting ||
                   NorseWarpBridge.IsBridgeExecuting;
        }
    }

    [HarmonyPatch]
    internal static class NorseDemigodsDrainingAbilityNullPatch
    {
        private static bool Prepare()
        {
            return NorseDemigodsSuppression.FindNorseType("NorseDemigods.Helper") != null;
        }

        private static MethodBase TargetMethod()
        {
            Type type = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Helper");
            return type != null ? AccessTools.Method(type, "IsAnyDrainingAbilityActive") : null;
        }

        private static bool Prefix(object __0, ref bool __result)
        {
            if (__0 != null)
            {
                return true;
            }

            __result = true;
            return false;
        }
    }

    [HarmonyPatch]
    internal static class NorseDemigodsPlayerBusyBridgePatch
    {
        private static bool Prepare()
        {
            return NorseDemigodsSuppression.FindNorseType("NorseDemigods.Helper") != null;
        }

        private static MethodBase TargetMethod()
        {
            Type type = NorseDemigodsSuppression.FindNorseType("NorseDemigods.Helper");
            return type != null
                ? AccessTools.Method(type, "IsPlayerBusy", new[] { typeof(Player), typeof(bool), typeof(bool), typeof(bool) })
                : null;
        }

        private static bool Prefix(ref bool __result)
        {
            if (!NorseEirAbilityBridge.IsBridgeExecuting &&
                !NorseEirAbilityBridge.IsBridgeUpdating &&
                !NorseWaterSphereBridge.IsBridgeExecuting &&
                !NorseWaterSphereBridge.IsBridgeUpdating &&
                !NorseNjordTornadoBridge.IsBridgeExecuting &&
                !NorseNjordTornadoBridge.IsBridgeUpdating &&
                !NorseWarpBridge.IsBridgeExecuting)
            {
                return true;
            }

            __result = false;
            return false;
        }
    }

    [HarmonyPatch]
    internal static class MaterialManPropertyContainerUpdateBlockPatch
    {
        private static bool _logged;

        private static MethodBase TargetMethod()
        {
            Type propertyContainerType = AccessTools.TypeByName("MaterialMan+PropertyContainer");
            return propertyContainerType == null ? null : AccessTools.Method(propertyContainerType, "UpdateBlock");
        }

        private static Exception Finalizer(Exception __exception)
        {
            if (__exception == null)
            {
                return null;
            }

            if (__exception is NullReferenceException)
            {
                if (!_logged)
                {
                    _logged = true;
                    EpicLootRaritySetsPlugin.Log.LogWarning("Suppressed a MaterialMan null material update. This usually means a saved character or preview model references a missing visual attachment.");
                }

                return null;
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(UniqueLegendaryHelper), "TryGetLegendaryInfo")]
    internal static class TryGetLegendaryInfoPatch
    {
        private static void Postfix(string legendaryID, ref LegendaryInfo legendaryInfo, ref bool __result)
        {
            LegendaryInfo info;
            if (RaritySetRegistry.TryGetInfo(legendaryID, out info))
            {
                legendaryInfo = info;
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(UniqueLegendaryHelper), "TryGetLegendarySetInfo")]
    internal static class TryGetLegendarySetInfoPatch
    {
        private static void Postfix(string setID, ref LegendarySetInfo legendarySetInfo, ref ItemRarity rarity, ref bool __result)
        {
            LegendarySetInfo setInfo;
            ItemRarity setRarity;
            if (RaritySetRegistry.TryGetSetInfo(setID, out setInfo, out setRarity))
            {
                legendarySetInfo = setInfo;
                rarity = setRarity;
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(UniqueLegendaryHelper), "GetSetForLegendaryItem")]
    internal static class GetSetForLegendaryItemPatch
    {
        private static void Postfix(LegendaryInfo legendary, ref string __result)
        {
            string setId = RaritySetRegistry.GetSetForItem(legendary);
            if (!string.IsNullOrEmpty(setId))
            {
                __result = setId;
            }
        }
    }

    [HarmonyPatch(typeof(UniqueLegendaryHelper), "GetLegendaryEffectValues")]
    internal static class GetLegendaryEffectValuesPatch
    {
        private static void Postfix(string legendaryID, string effectType, ref MagicItemEffectDefinition.ValueDef __result)
        {
            if (__result != null)
            {
                return;
            }

            MagicItemEffectDefinition.ValueDef values;
            if (RaritySetRegistry.TryGetEffectValues(legendaryID, effectType, out values))
            {
                __result = values;
            }
        }
    }

    [HarmonyPatch(typeof(ItemDataExtensions), "GetMundaneSetPieces")]
    internal static class GetMundaneSetPiecesPatch
    {
        private static bool Prefix(string setName, ref List<string> __result)
        {
            __result = new List<string>();

            HashSet<string> added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (ObjectDB.instance != null && ObjectDB.instance.m_items != null)
            {
                foreach (GameObject item in ObjectDB.instance.m_items)
                {
                    AddSetPiece(item, setName, __result, added);
                }
            }

            if (ZNetScene.instance != null && ZNetScene.instance.m_prefabs != null)
            {
                foreach (GameObject prefab in ZNetScene.instance.m_prefabs)
                {
                    AddSetPiece(prefab, setName, __result, added);
                }
            }

            return false;
        }

        private static void AddSetPiece(GameObject item, string setName, List<string> pieces, HashSet<string> added)
        {
            if (item == null)
            {
                return;
            }

            ItemDrop itemDrop = item.GetComponent<ItemDrop>();
            if (itemDrop == null || itemDrop.m_itemData == null || itemDrop.m_itemData.m_shared == null)
            {
                return;
            }

            ItemDrop.ItemData.SharedData shared = itemDrop.m_itemData.m_shared;
            if (shared.m_setName != setName || string.IsNullOrEmpty(shared.m_name) || added.Contains(shared.m_name))
            {
                return;
            }

            pieces.Add(shared.m_name);
            added.Add(shared.m_name);
        }
    }

    [HarmonyPatch]
    internal static class AdventureFeatureCreateItemDropPatch
    {
        private static MethodBase TargetMethod()
        {
            Type adventureFeatureType = AccessTools.TypeByName("EpicLoot.Adventure.Feature.AdventureFeature");
            return adventureFeatureType == null ? null : AccessTools.Method(adventureFeatureType, "CreateItemDrop", new[] { typeof(string) });
        }

        private static void Postfix(string prefabName, ref ItemDrop __result)
        {
            if (__result != null)
            {
                return;
            }

            ItemDrop itemDrop;
            if (RaritySetRegistry.TryCreateAdventureItemDrop(prefabName, out itemDrop))
            {
                __result = itemDrop;
                EpicLootRaritySetsPlugin.Log.LogInfo("Created adventure set item " + prefabName + ".");
            }
        }
    }

    [HarmonyPatch(typeof(LootRoller), "RollLootTableInternal", new Type[] { typeof(IEnumerable<LootTable>), typeof(int), typeof(string), typeof(Vector3), typeof(bool) })]
    internal static class RollLootTableInternalListPatch
    {
        private static void Prefix(int level, string objectName)
        {
            BossSetDropController.EnterLootTable(objectName, level);
        }

        private static void Finalizer()
        {
            BossSetDropController.ExitLootTable();
        }
    }

    [HarmonyPatch(typeof(LootRoller), "RollLootTableInternal", new Type[] { typeof(LootTable), typeof(int), typeof(string), typeof(Vector3), typeof(bool) })]
    internal static class RollLootTableInternalSinglePatch
    {
        private static void Prefix(int level, string objectName)
        {
            BossSetDropController.EnterLootTable(objectName, level);
        }

        private static void Finalizer()
        {
            BossSetDropController.ExitLootTable();
        }
    }

    [HarmonyPatch(typeof(LootRoller), "RollMagicItem", new Type[] { typeof(LootDrop), typeof(ItemDrop.ItemData), typeof(float), typeof(float) })]
    internal static class RollMagicItemFromLootDropPatch
    {
        private static void Prefix(LootDrop lootDrop)
        {
            BossSetDropController.PushLootDrop(lootDrop);
        }

        private static void Finalizer()
        {
            BossSetDropController.PopLootDrop();
        }
    }

    [HarmonyPatch(typeof(LootRoller), "RollMagicItem", new Type[] { typeof(ItemRarity), typeof(ItemDrop.ItemData), typeof(float), typeof(float) })]
    internal static class RollMagicItemPatch
    {
        private static bool Prefix(ItemRarity rarity, ItemDrop.ItemData baseItem, float powerlevelMod, ref MagicItem __result)
        {
            string forcedId = GetForcedCustomItemId();
            if (string.IsNullOrEmpty(forcedId))
            {
                return true;
            }

            MagicItem forcedMagicItem;
            if (!RaritySetRegistry.TryBuildForcedMagicItem(forcedId, baseItem, powerlevelMod, out forcedMagicItem))
            {
                return true;
            }

            __result = forcedMagicItem;
            EpicLootRaritySetsPlugin.Log.LogInfo(string.Format("Forced rarity set item {0} as {1}.", forcedId, forcedMagicItem.Rarity));
            return false;
        }

        private static void Postfix(ItemRarity rarity, ItemDrop.ItemData baseItem, float powerlevelMod, ref MagicItem __result)
        {
            if (!EpicLootRaritySetsPlugin.EnableNaturalDrops.Value || __result == null)
            {
                return;
            }

            bool bossRuleActive = BossSetDropController.HasActiveBossRule();
            if (bossRuleActive && !string.IsNullOrEmpty(__result.SetID))
            {
                BossSetDropController.RecordSetDrop();
                return;
            }

            if (!bossRuleActive && (rarity == ItemRarity.Legendary || rarity == ItemRarity.Mythic))
            {
                return;
            }

            if (!bossRuleActive && (!string.IsNullOrEmpty(__result.LegendaryID) || !string.IsNullOrEmpty(__result.SetID)))
            {
                return;
            }

            float chance;
            if (!BossSetDropController.TryGetBossSetDropChance(out chance))
            {
                chance = RaritySetRegistry.GetDropChance(rarity);
            }

            if (chance <= 0f || UnityEngine.Random.Range(0f, 1f) >= chance)
            {
                return;
            }

            MagicItem rolledMagicItem;
            if (RaritySetRegistry.TryRollSetItem(rarity, baseItem, __result, powerlevelMod, out rolledMagicItem))
            {
                __result = rolledMagicItem;
                if (!string.IsNullOrEmpty(rolledMagicItem.SetID))
                {
                    BossSetDropController.RecordSetDrop();
                }
            }
        }

        private static string GetForcedCustomItemId()
        {
            if (!string.IsNullOrEmpty(LootRoller.CheatForceMythic))
            {
                return LootRoller.CheatForceMythic;
            }

            if (!string.IsNullOrEmpty(LootRoller.CheatForceLegendary))
            {
                return LootRoller.CheatForceLegendary;
            }

            return null;
        }
    }
}

