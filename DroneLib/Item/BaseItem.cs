using UnityEngine;

namespace DroneLib.Item;

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
    public ItemSO ItemSO { get; set; }
    
    public bool IsRegistered() => ItemSO?.itemId != 0;

    public abstract Sprite LoadSprite();
}