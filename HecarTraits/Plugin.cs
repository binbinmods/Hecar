using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using Obeliskial_Essentials;
using System.IO;
using UnityEngine;
using System;
using static Hecar.Traits;
using BepInEx.Configuration;

namespace Hecar
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials")]
    [BepInDependency("com.stiffmeds.obeliskialcontent")]
    [BepInProcess("AcrossTheObelisk.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal int ModDate = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;

        public static ConfigEntry<bool> EnableDebugging { get; set; }


        public static string characterName = "Hecar";
        public static string subclassName = "Moontouched"; // needs caps
        public static string debugBase = "Binbin - Testing " + characterName + " ";

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");

            EnableDebugging = Config.Bind(new ConfigDefinition(subclassName, "Enable Debugging"), true, new ConfigDescription("Enables debugging logs."));

            // register with Obeliskial Essentials
            RegisterMod(
                _name: PluginInfo.PLUGIN_NAME,
                _author: "binbin",
                _description: "Hecar, the Moontouched",
                _version: PluginInfo.PLUGIN_VERSION,
                _date: ModDate,
                _link: @"https://github.com/binbinmods/Hecar",
                _contentFolder: "Hecar",
                _type: ["content", "hero", "trait"]
            );
            // apply patches


            harmony.PatchAll();
        }

        internal static void LogDebug(string msg)
        {
            Log.LogDebug(debugBase + msg);
        }
        internal static void LogInfo(string msg)
        {
            Log.LogInfo(debugBase + msg);
        }
        internal static void LogError(string msg)
        {
            Log.LogError(debugBase + msg);
        }
    }
}
