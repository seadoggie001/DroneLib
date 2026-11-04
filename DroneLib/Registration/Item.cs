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

    /// <summary>
    /// Registers an item in the game
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="Exception"></exception>
    public void Add(BaseItem item)
    {
        if (item.ItemSo is null) throw new Exception(ItemExceptionPrefix + $"{nameof(item.ItemSo)} is not initialized");
        if (item.IsRegistered()) return;

        Main.Log.LogInfo($"Registering new custom item: {item.ItemSo.name}");

        AddItem_ResourceManager(item);
        AddItem_StringIds(item);

        // Keep a reference because it is needed later 
        ModdedItems = ModdedItems.AddItem(item);
    }

    /// <summary>
    /// Not implemented yet; do not use
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Remove(string name)
    {
        //ToDo: Implement this or remove it
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
            item.ItemSo.itemId = ResourceManager.GetAllItems().Count();

            // Get ResourceManager.items and add the item's ItemSO to it
            ResourceManager.items = [.. ResourceManager.items, item.ItemSo];
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
            StringIds.itemNames = [.. StringIds.itemNames, item.ItemSo.name];
            
            // Get StringIds.itemIds and add the item's name and id to it
            StringIds.itemIds.Add(item.ItemSo.name, item.ItemSo.itemId);
        }
        catch (Exception ex)
        {
            Main.LogException(ex, "Failed to register string ids");
            throw;
        }
    }
}