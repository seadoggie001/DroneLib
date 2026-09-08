using BepInEx.Logging;
using HarmonyLib;

namespace DroneLib;

public static class Main
{
    internal static ManualLogSource Log { get; } = BepInEx.Logging.Logger.CreateLogSource("DroneLib");

    private static readonly Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);
    
    internal static void LogException(Exception ex, string message = null)
    {
        if(!string.IsNullOrWhiteSpace(message)) Log.LogError(message);
        Log.LogError($"Exception Message: {ex.Message}\n{ex.StackTrace}");
    }

    /// <summary>
    /// Patches all harmony methods for the library
    /// </summary>
    public static void PatchAll() => Harmony.PatchAll();
}