using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Platforms;
using UnityEngine;

namespace DroneLib.Patches;

/// <summary>
/// Modifications to the ResourceManager class
/// </summary>
[HarmonyPatch(typeof(ResourceManager))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "Unity.UnknownResource")]
public static class ResourceManagerPatch
{
    /// <summary>
    /// Forces a single reload of the game's loaded objects
    /// </summary>
    public static bool ForceReload = false;

    /// <summary>
    /// Overrides the base game's reload settings to allow for easier patching
    /// </summary>
    /// <returns></returns>
    /// <remarks>This is a bit sketchy, but if I want to be able to hook into each of these separately,
    /// something must be done to split the base game's giant function</remarks>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ResourceManager.LoadAll))]
    public static bool LoadAllPatch()
    {
        if (!ForceReload)
        {
            // Copied from the base game
            if (ResourceManager.entitites != null || ResourceManager.grounds != null ||
                (UnityEngine.Object)ResourceManager.drone != (UnityEngine.Object)null || ResourceManager.hats != null ||
                ResourceManager.leaderboards != null || ResourceManager.unlocks != null || ResourceManager.items != null ||
                ResourceManager.groundFeaturesSorted != null)
                return false;
        }

        ForceReload = false;
        
        ResourceManager.dlcKeys = new Dictionary<string, EDlcType>();
        
        LoadItems();
        
        LoadEntities();

        LoadGrounds();
        
        ResourceManager.drone = Resources.Load<DroneSO>("FarmObjects/drone");

        LoadHats();

        LoadLeaderboards();

        LoadUnlocks();

        LoadGroundFeatures();

        // Don't run BaseGame's LoadAll
        return false;
    }

    /// <summary>
    /// Loads all items (modded and unmodded) into the game
    /// </summary>
    public static void LoadItems()
    {
        // Load base game items
        var items = Resources.LoadAll<ItemSO>("Items/");
        // Load modded items
        items = items.AddRangeToArray(Registration.Item.ModdedItems.Select(m => m.ItemSo).ToArray());
        // Order all items by priority
        items = items.OrderBy(x => x.priority).ToArray();
        // Assign itemIds
        for (int index = 0; index < items.Length; ++index)
        {
            items[index].itemId = index;
            TrackDlcKey(items[index].dlc, items[index].name);
        }

        ResourceManager.items = items;
        
        // Also load item names by item id
        StringIds.SetItemIds(ResourceManager.items.Select(x => x.itemName));
    }

    /// <summary>
    /// Loads all entities into the game
    /// </summary>
    /// <remarks>Entities were formerly known as FarmObjects</remarks>
    public static void LoadEntities()
    {
        ResourceManager.entitites = new Dictionary<string, EntitySO>();
        foreach (EntitySO entitySo in Resources.LoadAll<EntitySO>("FarmObjects/"))
        {
            entitySo.cost.Deserialize();
            entitySo.dropItemId = StringIds.GetItemId(entitySo.dropItem);
            TrackDlcKey(entitySo.dlc, entitySo.objectName);
            ResourceManager.entitites.Add(entitySo.objectName, entitySo);
        }
    }

    /// <summary>
    /// Loads all grounds into the game
    /// </summary>
    public static void LoadGrounds()
    {
        ResourceManager.grounds = [];
        foreach (GroundSO groundSo in Resources.LoadAll<GroundSO>("Grounds/"))
        {
            ResourceManager.grounds.Add(groundSo.objectName, groundSo);
            TrackDlcKey(groundSo.dlc, groundSo.objectName);
            groundSo.dropItemId = StringIds.GetItemId(groundSo.dropItem);
        }
    }

    /// <summary>
    /// Load all hats into the game
    /// </summary>
    public static void LoadHats()
    {
        ResourceManager.hats = new Dictionary<string, HatSO>();
        foreach (HatSO hatSo in Resources.LoadAll<HatSO>("Hats/"))
        {
            TrackDlcKey(hatSo.dlc, hatSo.hatName);
            ResourceManager.hats[hatSo.hatName] = hatSo;
        }
    }

    /// <summary>
    /// Load all leaderboards into the game
    /// </summary>
    public static void LoadLeaderboards()
    {
        ResourceManager.leaderboards = new Dictionary<string, LeaderboardSO>();
        foreach (LeaderboardSO leaderboardSo in Resources.LoadAll<LeaderboardSO>("Leaderboards/"))
        {
            leaderboardSo.startItems.Deserialize();
            leaderboardSo.goalItems.Deserialize();
            TrackDlcKey(leaderboardSo.dlc, leaderboardSo.leaderboardName);
            ResourceManager.leaderboards[leaderboardSo.leaderboardName] = leaderboardSo;
        }
    }

    /// <summary>
    /// Load all unlocks (modded and unmodded) into the game
    /// </summary>
    public static void LoadUnlocks()
    {
        ResourceManager.unlocks = new Dictionary<string, UnlockSO>();
        foreach (UnlockSO unlockSo in Resources.LoadAll<UnlockSO>("Unlocks/"))
        {
            foreach (ItemBlock itemBlock in unlockSo.multiUnlockCost)
                itemBlock.Deserialize();
            unlockSo.unlockCost.Deserialize();
            TrackDlcKey(unlockSo.dlc, unlockSo.unlockName);
            TrackDlcKey(unlockSo.dlc, unlockSo.docs);
            if (unlockSo.unlocks != null)
                foreach (string unlock in unlockSo.unlocks) TrackDlcKey(unlockSo.dlc, unlock);
            ResourceManager.unlocks.Add(unlockSo.unlockName, unlockSo);
        }

        foreach (UnlockSO unlock in Registration.Unlock.ModdedUnlocks)
        {
            ResourceManager.unlocks.Add(unlock.unlockName, unlock);
        }
    }

    /// <summary>
    /// Load all ground features into the game 
    /// </summary>
    public static void LoadGroundFeatures()
    {
        ResourceManager.groundFeaturesSorted = [];
        ResourceManager.groundFeaturesByName = [];
        foreach (GroundFeature groundFeature in Resources.LoadAll<GroundFeature>("GroundFeatures/"))
        {
            groundFeature.OnLoaded();
            ResourceManager.groundFeaturesSorted.Add(groundFeature);
            ResourceManager.groundFeaturesByName[groundFeature.name] = groundFeature;
        }
        ResourceManager.groundFeaturesSorted.Sort(GroundFeature.PriorityComparer);
    }
    
    /// <summary>
    /// Tracks strings for DLC comparison to determine if features are DLC-specific
    /// </summary>
    /// <param name="dlc"></param>
    /// <param name="key"></param>
    public static void TrackDlcKey(EDlcType dlc, string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (ResourceManager.dlcKeys.TryGetValue(key, out EDlcType eDlcType))
        {
            if (eDlcType == EDlcType.None || eDlcType == dlc) return;
            if (dlc == EDlcType.None)
            {
                ResourceManager.dlcKeys[key] = EDlcType.None;
                return;
            }
            Debug.LogWarning($"Duplicate DLC association for '{key}': {eDlcType} and {dlc}");
        }
        ResourceManager.dlcKeys[key] = dlc;
    }
}