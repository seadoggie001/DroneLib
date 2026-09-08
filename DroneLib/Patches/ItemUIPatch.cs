using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine.UI;

namespace DroneLib.Patches;

[HarmonyPatch(typeof(ItemUI))]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony requires a specific naming convention")]
public class ItemUIPatch
{
    /// <summary>
    /// Instruct the game to use your custom sprite based on the BaseItem's LoadSprite
    /// </summary>
    /// <param name="___image">Private field that the sprite gets loaded into</param>
    /// <param name="itemId">The ID of the item</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ItemUI.Setup), typeof(int), typeof(double))]
    public static void Setup(ref Image ___image, int itemId)
    {
        try
        {
            // Locate the modded item
            Item.BaseItem item = Item.Registration.ModdedItems.FirstOrDefault(m => m.ItemSO.itemId == itemId);
            if (item is null) return;

            // Load the sprite
            ___image.sprite = item.LoadSprite();
        }
        catch (Exception ex)
        {
            Plugin.LogException(ex, $"{nameof(Setup)}");
        }
    }
}