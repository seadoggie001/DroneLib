using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace DroneLib.Patches;

[HarmonyPatch(typeof(ResourceManager))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ResourceManagerPatch
{
    /// <summary>
    /// Adds modded items and unlocks after the base game loads its resources
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResourceManager.LoadAll))]
    public static void LoadAllPatch(ref ItemSO[] ___items, ref Dictionary<string, UnlockSO> ___unlocks)
    {
        if (___items is null || ___unlocks is null) return;

        LoadItems(ref ___items);

        LoadUnlocks(ref ___unlocks);
    }

    public static void LoadItems(ref ItemSO[] ___items)
    {
        ItemSO[] loaded = ___items;
        ItemSO[] moddedItems = Registration.Item.ModdedItems
            .Select(m => m.ItemSO)
            .Where(m => !loaded.Contains(m))
            .ToArray();
        if (moddedItems.Length == 0) return;

        for (int index = 0; index < moddedItems.Length; ++index) moddedItems[index].itemId = ___items.Length + index;
        ___items = ___items.AddRangeToArray(moddedItems);

        StringIds.SetItemIds(___items.Select(x => x.itemName));
    }

    public static void LoadUnlocks(ref Dictionary<string, UnlockSO> ___unlocks)
    {
        foreach (UnlockSO unlock in Registration.Unlock.ModdedUnlocks)
        {
            ___unlocks[unlock.unlockName] = unlock;
        }
    }
}
