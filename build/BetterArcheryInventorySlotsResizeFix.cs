using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Fran.BetterArcheryInventorySlotsResizeFix
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("ishid4.mods.betterarchery", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("sighsorry.InventorySlots", BepInDependency.DependencyFlags.HardDependency)]
    public sealed class BetterArcheryInventorySlotsResizeFixPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "fran.mods.betterarcheryinventoryslotsresizefix";
        public const string PluginName = "BetterArchery InventorySlots Resize Fix";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;
            _harmony = new Harmony(PluginGuid);

            Type patchType = AccessTools.TypeByName("BetterArchery.Player_SetInventorySize_Patch");
            MethodInfo target = patchType == null ? null : AccessTools.Method(patchType, "Prefix");
            MethodInfo prefix = AccessTools.Method(typeof(BetterArcherySetInventorySizePrefixPatch), "Prefix");

            if (target == null || prefix == null)
            {
                Logger.LogWarning("Could not patch BetterArchery Player.SetInventorySize prefix; target method was not found.");
                return;
            }

            _harmony.Patch(target, prefix: new HarmonyMethod(prefix));
            Logger.LogInfo("BetterArchery InventorySlots resize fix loaded.");
        }

        private void OnDestroy()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
            }
        }
    }

    internal static class BetterArcherySetInventorySizePrefixPatch
    {
        private static bool _logged;

        public static bool Prefix([HarmonyArgument(0)] Player player, int rows, ref bool __result)
        {
            if (player == null || !IsBetterArcheryQuiverEnabled())
            {
                return true;
            }

            try
            {
                SetBetterArcheryQuiverRowIndex(Mathf.Clamp(rows, 0, 9) + 1);
                __result = true;

                if (!_logged)
                {
                    _logged = true;
                    BetterArcheryInventorySlotsResizeFixPlugin.Log.LogInfo("Skipped BetterArchery inventory resize so InventorySlots can preserve equipment slot rows.");
                }

                return false;
            }
            catch (Exception ex)
            {
                BetterArcheryInventorySlotsResizeFixPlugin.Log.LogWarning("BetterArchery InventorySlots resize fix failed; falling back to BetterArchery resize. " + ex.GetBaseException().Message);
                return true;
            }
        }

        private static bool IsBetterArcheryQuiverEnabled()
        {
            Type betterArcheryType = AccessTools.TypeByName("BetterArchery.BetterArchery");
            FieldInfo field = betterArcheryType == null ? null : AccessTools.Field(betterArcheryType, "ConfigQuiverEnabled");
            ConfigEntry<bool> entry = field == null ? null : field.GetValue(null) as ConfigEntry<bool>;
            return entry != null && entry.Value;
        }

        private static void SetBetterArcheryQuiverRowIndex(int rowIndex)
        {
            Type betterArcheryType = AccessTools.TypeByName("BetterArchery.BetterArchery");
            FieldInfo field = betterArcheryType == null ? null : AccessTools.Field(betterArcheryType, "QuiverRowIndex");
            if (field == null)
            {
                throw new MissingFieldException("BetterArchery.BetterArchery", "QuiverRowIndex");
            }

            field.SetValue(null, rowIndex);
        }
    }
}
