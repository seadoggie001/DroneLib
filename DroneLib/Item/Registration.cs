using System.Reflection;
using DroneLib.Helpers;
using HarmonyLib;

namespace DroneLib.Item;

public static class Registration
{
    /// <summary>
    /// List of already registered modded items
    /// </summary>
    public static IEnumerable<BaseItem> ModdedItems { get; private set; } = [];
    
    private const string ExceptionPrefix = "Failed to register item: ";

    /// <summary>
    /// Register an item with the mod
    /// </summary>
    /// <param name="item">Custom Item to register</param>
    /// <exception cref="Exception"></exception>
    public static void RegisterItem(BaseItem item)
    {
        if (item.ItemSO is null) throw new Exception(ExceptionPrefix + $"{nameof(item.ItemSO)} is not initialized");
        if (item.IsRegistered()) return;

        Main.Log.LogInfo($"Registering new custom item: {item.ItemSO.name}");

        RegisterResourceManager(item);
        RegisterStringIds(item);

        // Keep a reference because it is needed later 
        ModdedItems = ModdedItems.AddItem(item);
    }

    private static void RegisterResourceManager(BaseItem item)
    {
        // Try to load items from the ResourceManager. Call LoadAll if necessary
        if (ResourceManager.GetAllItems() is null) ResourceManager.LoadAll();
        if (ResourceManager.GetAllItems() is null)
            throw new Exception(ExceptionPrefix + "Unable to obtain items from ResourceManager");
        try
        {
            // Find the next ID to use for the item
            item.ItemSO.itemId = ResourceManager.GetAllItems().Count();

            // Get ResourceManager.items and add the item's ItemSO to it
            Reflection.GetStaticField<ItemSO[]>(typeof(ResourceManager),
                "items",
                BindingFlags.NonPublic | BindingFlags.Static,
                (items) => items.AddItem(item.ItemSO).ToArray());
        }
        catch (Exception ex)
        {
            Main.LogException(ex, ExceptionPrefix + "Failed to register item due to exception");
            throw;
        }
    }

    private static void RegisterStringIds(BaseItem item)
    {
        try
        {
            // Get StringIds.itemNames and add the item's name to it
            Reflection.GetStaticField<string[]>(typeof(StringIds),
                "itemNames",
                BindingFlags.NonPublic | BindingFlags.Static,
                (itemNames) => itemNames.AddItem(item.ItemSO.name).ToArray());

            // Get StringIds.itemIds and add the item's name and id to it
            Dictionary<string, int> itemIds = Reflection.GetStaticField<Dictionary<string, int>>(typeof(StringIds), 
                "itemIds",
                BindingFlags.NonPublic | BindingFlags.Static);
            itemIds.Add(item.ItemSO.name, item.ItemSO.itemId);
        }
        catch (Exception ex)
        {
            Main.LogException(ex, "Failed to register string ids");
            throw;
        }
    }
}