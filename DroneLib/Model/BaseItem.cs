using UnityEngine;

namespace DroneLib.Model;

/// <summary>
/// An object used to register custom items
/// </summary>
public abstract class BaseItem
{
    /// <summary>
    /// The data behind an item
    /// </summary>
    /// <remarks>
    /// ScriptableObjects should be created with <c>ScriptableObject.CreateInstance&lt;ItemSO&gt;()</c>
    /// </remarks>
    public ItemSO ItemSo { get; set; }

    /// <summary>
    /// Checks if an item was already registered
    /// </summary>
    /// <returns></returns>
    public bool IsRegistered() => ItemSo?.itemId != 0;

    /// <summary>
    /// Method used to load a sprite for the item
    /// </summary>
    /// <returns></returns>
    public abstract Sprite LoadSprite();
}