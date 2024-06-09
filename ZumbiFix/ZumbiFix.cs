using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AetharNet.Mods.ZumbiBlocks2.ZumbiFix;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class ZumbiFix : BaseUnityPlugin
{
    public const string PluginGUID = "AetharNet.Mods.ZumbiBlocks2.ZumbiFix";
    public const string PluginAuthor = "wowi";
    public const string PluginName = "ZumbiFix";
    public const string PluginVersion = "0.1.0";

    internal new static ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);
    }
}
