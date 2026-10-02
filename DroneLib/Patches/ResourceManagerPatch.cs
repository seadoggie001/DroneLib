using System.Diagnostics.CodeAnalysis;
using DroneLib.Item;
using DroneLib.Model;
using HarmonyLib;
using UnityEngine;

namespace DroneLib.Patches;

[HarmonyPatch(typeof(ResourceManager))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "Unity.UnknownResource")]
public static class ResourceManagerPatch
{
    public static bool ForceReload = false;

    /// <summary>
    /// Overrides the base game's reload settings to allow for easier patching
    /// </summary>
    /// <returns></returns>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ResourceManager.LoadAll))]
    public static bool LoadAllPatch(
        ref Dictionary<string, FarmObjectSO> ___farmObjects,
        ref DroneSO ___drone,
        ref Dictionary<string, HatSO> ___hats,
        ref Dictionary<string, LeaderboardSO> ___leaderboards,
        ref Dictionary<string, UnlockSO> ___unlocks,
        ref ItemSO[] ___items
    )
    {
        if (___farmObjects != null || ___drone != null || ___hats != null ||
            ___leaderboards != null || ___unlocks != null || ___items != null || ForceReload) return false;

        ForceReload = false;

        LoadItems(ref ___items);

        StringIds.SetItemIds(___items.Select(x => x.itemName));

        LoadFarmObjects(ref ___farmObjects);

        ___drone = Resources.Load<DroneSO>("FarmObjects/drone");

        LoadHats(ref ___hats);

        LoadLeaderboards(ref ___leaderboards);

        LoadUnlocks(ref ___unlocks);

        // Don't run BaseGame's LoadAll
        return false;
    }

    public static void LoadItems(ref ItemSO[] ___items)
    {
        // Load base game items
        var items = Resources.LoadAll<ItemSO>("Items/");
        // Load modded items
        items = items.AddRangeToArray(Registration.ModdedItems.Select(m => m.ItemSO).ToArray());
        // Order all items by priority
        ___items = items.OrderBy(x => x.priority).ToArray();
        // Assign itemIds
        for (int index = 0; index < ___items.Length; ++index) ___items[index].itemId = index;
    }

    public static void LoadFarmObjects(ref Dictionary<string, FarmObjectSO> ___farmObjects)
    {
        ___farmObjects = new Dictionary<string, FarmObjectSO>();
        foreach (FarmObjectSO farmObjectSo in Resources.LoadAll<FarmObjectSO>("FarmObjects/"))
        {
            farmObjectSo.cost.Deserialize();
            farmObjectSo.dropItemId = StringIds.GetItemId(farmObjectSo.dropItem);
            ___farmObjects[farmObjectSo.objectName] = farmObjectSo;
        }
    }

    public static void LoadHats(ref Dictionary<string, HatSO> ___hats)
    {
        ___hats = new Dictionary<string, HatSO>();
        foreach (HatSO hatSo in Resources.LoadAll<HatSO>("Hats/"))
            ___hats[hatSo.hatName] = hatSo;
    }

    public static void LoadLeaderboards(ref Dictionary<string, LeaderboardSO> ___leaderboards)
    {
        ___leaderboards = new Dictionary<string, LeaderboardSO>();
        foreach (LeaderboardSO leaderboardSo in Resources.LoadAll<LeaderboardSO>("Leaderboards/"))
        {
            leaderboardSo.startItems.Deserialize();
            leaderboardSo.goalItems.Deserialize();
            ___leaderboards[leaderboardSo.leaderboardName] = leaderboardSo;
        }
    }

    public static void LoadUnlocks(ref Dictionary<string, UnlockSO> ___unlocks)
    {
        ___unlocks = new Dictionary<string, UnlockSO>();
        foreach (UnlockSO unlockSo in Resources.LoadAll<UnlockSO>("Unlocks/"))
        {
            foreach (ItemBlock itemBlock in unlockSo.multiUnlockCost)
                itemBlock.Deserialize();
            unlockSo.unlockCost.Deserialize();
            ___unlocks[unlockSo.unlockName] = unlockSo;
        }
    }
}