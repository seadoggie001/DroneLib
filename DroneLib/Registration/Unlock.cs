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

    /// <summary>
    /// Register an Unlock with the base game
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="Exception"></exception>
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
        
        ResourceManager.unlocks.Add(item.UnlockName, item.ToUnlockSO());
    }

    /// <summary>
    /// Un-register an unlock with the game
    /// </summary>
    /// <param name="name"></param>
    public void Remove(string name)
    {
        ResourceManager.unlocks.Remove(name);
        ModdedUnlocks = ModdedUnlocks.Where(m => m.name != name);
    }
}