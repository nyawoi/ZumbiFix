using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace AetharNet.Mods.ZumbiBlocks2.ZumbiFix.Patches;

[HarmonyPatch]
public static class DisableEscapeKey
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        // As of 2.1.0.5, these are the only methods that use `Input.GetKeyDown(KeyCode.Escape)`
        yield return AccessTools.Method(typeof(BossfightCameraSequence), nameof(BossfightCameraSequence.MyUpdate));
        yield return AccessTools.Method(typeof(InGameMenuController), "Update");
        yield return AccessTools.Method(typeof(MenuController), nameof(MenuController.Update));
    }
    
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var onKeyDownMethod = AccessTools.Method(typeof(Input), nameof(Input.GetKeyDown), new[] { typeof(KeyCode) });

        using var enumerator = instructions.GetEnumerator();

        // This transpiler patch searches for the IL equivalent of `if (Input.GetKeyDown(KeyCode.Escape))`
        // and then patches it so that it does not execute.
        // You can think of it as changing `if (Input.GetKeyDown(KeyCode.Escape))` into `if (false)`.
        while (enumerator.MoveNext() && enumerator.Current != null)
        {
            var currentInstruction = enumerator.Current;
            
            // IL: `ldc.i4.s  27`
            // KeyCode.Escape is loaded (enum value is 27)
            if (currentInstruction.LoadsConstant(27) && enumerator.MoveNext())
            {
                var nextInstruction = enumerator.Current;

                // IL: `call  bool [UnityEngine.InputLegacyModule]UnityEngine.Input::GetKeyDown(valuetype [UnityEngine.CoreModule]UnityEngine.KeyCode)`
                // Input.GetKeyDown() is called using the loaded value
                if (nextInstruction.Calls(onKeyDownMethod) && enumerator.MoveNext())
                {
                    var lastInstruction = enumerator.Current;

                    // IL: `brfalse.s  $LABEL`
                    // If the preceding call returned false, skip ahead
                    if (lastInstruction.Branches(out _))
                    {
                        currentInstruction.opcode = OpCodes.Nop;
                        nextInstruction.opcode = OpCodes.Nop;
                        lastInstruction.opcode = OpCodes.Br_S;
                    }
                    
                    yield return currentInstruction;
                    yield return nextInstruction;
                    yield return lastInstruction;
                }
                else
                {
                    yield return currentInstruction;
                    yield return nextInstruction;
                }
            }
            else
            {
                yield return currentInstruction;
            }
        }
    }
}
