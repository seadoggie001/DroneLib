using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace DroneLib.Patches;

[HarmonyPatch(typeof(Execution))]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony requires a specific naming convention")]
public class ExecutionPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("ApplySideEffect", typeof(int))]
    private static void ApplySideEffect(Execution __instance, int droneId, ref double __result)
    {
        try
        {
            // Get the current drone's program state
            ProgramState state = __instance.States[droneId];
            if (state is null) return;

            // Only handle modded side effect calls
            if (state.currentSideEffect != Function.BaseFunction.ModdedSideEffect) return;

            // Locate the requested function
            string requestedFunction = ((PyString)state.currentSideEffectArgument).str;

            // Locate the modded function
            Function.BaseFunction function = Function.Registration.ModdedFunctions.FirstOrDefault(m =>
                string.Equals(m.Name, requestedFunction, StringComparison.OrdinalIgnoreCase));
            if (function is null)
            {
                throw new Exception($"Failed to locate a valid function. " +
                                    $"Expected {requestedFunction} to be a function");
            }

            // Execute the modded function
            __result = function.PerformAction(__instance, droneId,
                __instance.States[droneId].currentSideEffectArgument2);
        }
        catch (Exception ex)
        {
            Main.LogException(ex, "ApplySideEffect Exception");
        }
    }
}