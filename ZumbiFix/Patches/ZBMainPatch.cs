using AetharNet.Mods.ZumbiBlocks2.ZumbiFix.MonoBehaviours;
using HarmonyLib;

namespace AetharNet.Mods.ZumbiBlocks2.ZumbiFix.Patches;

[HarmonyPatch(typeof(ZBMain))]
public static class ZBMainPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZBMain.Start))]
    public static void AddEscapeHandler(ZBMain __instance)
    {
        __instance.gameObject.AddComponent<EscapeHandler>();
    }
}
