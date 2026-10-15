using System.Reflection;
using DroneLib.Helpers;
using DroneLib.Model;
using JetBrains.Annotations;

namespace DroneLib.Registration;

/// <summary>
/// Unlock Registration
/// </summary>
[UsedImplicitly]
public class Unlock : IRegistration<UnlockDescription>
{
    /// <summary>List of already registered modded unlocks</summary>
    public static IEnumerable<UnlockSO> ModdedUnlocks { get; private set; } = [];

    private const string UnlockExceptionPrefix = "Failed to register unlock: ";

    public void Add(UnlockDescription item)
    {
        // Localize the display name as the actual text displayed on the tooltip
        Localize.RegisterValue(item.UnlockName, item.DisplayName);

        // Localize the description
        string description = $"unlock_descr_{item.UnlockName}";
        Localize.RegisterValue(description, item.Description);
        item.Description = description;

        // Try to load items from the ResourceManager. Call LoadAll if necessary
        if (ResourceManager.GetAllUnlocks() is null) ResourceManager.LoadAll();
        if (ResourceManager.GetAllUnlocks() is null)
            throw new Exception(UnlockExceptionPrefix + "Unable to obtain items from ResourceManager");

        if (ModdedUnlocks.Any(m => m.unlockName == item.UnlockName))
            throw new Exception(UnlockExceptionPrefix + "Unable to register item, it was already registered");

        Reflection.GetStaticField<Dictionary<string, UnlockSO>>(
            type: typeof(ResourceManager),
            fieldName: "unlocks",
            flags: BindingFlags.NonPublic | BindingFlags.Static,
            update: (unlocks) =>
            {
                unlocks[item.UnlockName] = item.ToUnlockSO();
                return unlocks;
            });
    }

    public void Remove(string name)
    {
        Reflection.GetStaticField<Dictionary<string, UnlockSO>>(
            type: typeof(ResourceManager),
            fieldName: "unlocks",
            flags: BindingFlags.NonPublic | BindingFlags.Static,
            update: (unlocks) =>
            {
                unlocks.Remove(name);
                return unlocks;
            });
        ModdedUnlocks = ModdedUnlocks.Where(m => m.name != name);
    }
}