using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace DroneLib.Model;

/// <summary>
/// This is a helper class that describes how <see cref="UnlockSO"/> works. Use <see cref="ToUnlockSO"/> to convert it.
/// </summary>
// Silly messages, this is just a wrapper class, leave me alone!
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
// ReSharper disable once ClassNeverInstantiated.Global
public class UnlockDescription
{
    /// <summary>The text that is displayed to players at the top of the unlock tooltip.</summary>
    public string DisplayName;

    /// <summary>A unique name stored in the save file. May not contain spaces. Should (must?) be lowercase.</summary>
    public string UnlockName;

    /// <summary>Text displayed on the unlock tooltip body when <see cref="MaxUnlockLevel"/> is 1</summary>
    public string Description;

    /// <summary>A relative path to a Markdown file that will be displayed when the unlock item is unlocked and right-clicked</summary>
    /// <remarks>If empty, attempts to use <c>unlocks/UnlockName</c>. If the file is empty, displays any text.</remarks>
    public string Docs;

    /// <summary>Used to determine if a texture will be loaded instead of code</summary>
    /// <remarks>The mesh itself is never used(!) Instead a sprite is loaded from "UnlockTextures/UnlockName"</remarks>
    public Mesh Mesh;

    /// <summary>Used to display code in the unlock item</summary>
    /// <remarks>For example, see the Debug unlock item. It displays <c>print()</c> instead of an image</remarks>
    public string DisplayCode;

    /// <summary>Things to unlock when purchasing the unlock item</summary>
    /// <remarks>For example, unlocking carrots gives you at least <c>Entities.Carrots</c> and <c>till()</c></remarks>
    public List<string> Unlocks = [];

    /// <summary><see cref="UnlockName"/> of a parent item, if it has one.</summary>
    /// <remarks>Used for the research menu and allowing unlocking. Use lowercase.</remarks>
    public string ParentUnlock;

    /// <summary>A single hat to unlock when researching.</summary>
    public string UnlockedHat;

    /// <summary>Order of items on the research menu. Lower is more left.</summary>
    public int Order;

    /// <summary>Cost of an item with <see cref="MaxUnlockLevel"/> set to 1</summary>
    public ItemBlock UnlockCost;

    /// <summary>The base cost for MultiUnlocks</summary>
    public List<ItemBlock> MultiUnlockCost;

    /// <summary>Used to determine the cost of an unlock item. <seealso cref="Farm.GetUnlockCost" /></summary>
    public float MultiUnlockFactor = 2f;

    /// <summary>Text to display on the tooltip for the unlock item</summary>
    public string MultiUnlockDescr;

    /// <summary>Describes which method to use for calculation of costs</summary>
    public MultiUnlockDescrMode MultiUnlockDescrMode;

    /// <summary>Used to determine the cost of an unlock item. Only for a tooltip</summary>
    public float AdditivePercentStart;

    /// <summary>Used to determine the cost of an unlock item. Only for a tooltip</summary>
    public float AdditivePercentFactor = 2f;

    /// <summary>Number of times it can be unlocked/upgraded</summary>
    public int MaxUnlockLevel = 1;

    /// <summary>If false, completely disables and hides the unlock item</summary>
    public bool Enabled = true;

    /// <summary>
    /// Create an UnlockSO from this instance
    /// </summary>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    public virtual UnlockSO ToUnlockSO()
    {
        UnlockSO unlock = ScriptableObject.CreateInstance<UnlockSO>();
        unlock.name = $"unlock_{UnlockName}";
        unlock.unlockName = UnlockName;
        unlock.description = Description;
        unlock.docs = Docs;
        unlock.mesh = Mesh;
        unlock.displayCode = DisplayCode;
        unlock.unlocks = Unlocks;
        unlock.parentUnlock = ParentUnlock;
        unlock.unlockedHat = UnlockedHat;
        unlock.order = Order;
        unlock.unlockCost = UnlockCost;
        unlock.multiUnlockCost = MultiUnlockCost;
        unlock.multiUnlockFactor = MultiUnlockFactor;
        unlock.multiUnlockDescr = MultiUnlockDescr;
        unlock.multiUnlockDescrMode = MultiUnlockDescrMode;
        unlock.additivePercentStart = AdditivePercentStart;
        unlock.additivePercentFactor = AdditivePercentFactor;
        unlock.maxUnlockLevel = MaxUnlockLevel;
        unlock.enabled = Enabled;
        return unlock;
    }
}