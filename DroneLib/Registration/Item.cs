using System.Reflection;
using DroneLib.Helpers;
using DroneLib.Model;
using HarmonyLib;
using JetBrains.Annotations;

namespace DroneLib.Registration;

/// <summary>
/// Item Registration
/// </summary>
[UsedImplicitly]
public class Item : IRegistration<BaseItem>
{
    /// <summary>
    /// List of already registered modded items
    /// </summary>
    public static IEnumerable<BaseItem> ModdedItems { get; private set; } = [];

    private const string ItemExceptionPrefix = "Failed to register item: ";

    public void Add(BaseItem item)
    {
        if (item.ItemSO is null) throw new Exception(ItemExceptionPrefix + $"{nameof(item.ItemSO)} is not initialized");
        if (item.IsRegistered()) return;

        Main.Log.LogInfo($"Registering new custom item: {item.ItemSO.name}");

        AddItem_ResourceManager(item);
        AddItem_StringIds(item);

        // Keep a reference because it is needed later 
        ModdedItems = ModdedItems.AddItem(item);
    }

    public void Remove(string name)
    {
        throw new NotImplementedException("Items cannot be deregistered, at least yet");
    }

    private static void AddItem_ResourceManager(BaseItem item)
    {
        // Try to load items from the ResourceManager. Call LoadAll if necessary
        if (ResourceManager.GetAllItems() is null) ResourceManager.LoadAll();
        if (ResourceManager.GetAllItems() is null)
            throw new Exception(ItemExceptionPrefix + "Unable to obtain items from ResourceManager");
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
            Main.LogException(ex, ItemExceptionPrefix + "Failed to register item due to exception");
            throw;
        }
    }

    private static void AddItem_StringIds(BaseItem item)
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