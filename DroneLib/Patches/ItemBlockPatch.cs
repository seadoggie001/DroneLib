using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace DroneLib.Patches;

/// <summary>
/// ItemBlock expects that all ItemBlock.items will have the number of items. When modding, this isn't always true.
/// </summary>
[HarmonyPatch(typeof(ItemBlock))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ItemBlockPatch
{
    /// <summary>
    /// Increase the number of items before multiplying or dividing
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    [HarmonyPatch(typeof(ItemBlock), "op_Multiply")]
    [HarmonyPatch(typeof(ItemBlock), "op_Division")]
    [HarmonyPrefix]
    public static void MultiplyDivide(ref ItemBlock a, double b) => UpdateItemCount(ref a);

    /// <summary>
    /// Increase the number of items before adding or subtracting
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    [HarmonyPatch(typeof(ItemBlock), "op_Addition")]
    [HarmonyPatch(typeof(ItemBlock), "op_Subtraction")]
    [HarmonyPrefix]
    public static void AddSubtract(ref ItemBlock a, ref ItemBlock b)
    {
        UpdateItemCount(ref a);
        UpdateItemCount(ref b);
    }

    /// <summary>
    /// Increate the number of items before performing various operations on ItemBlock itself
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.AddItem))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.Clear))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.Contains), typeof(int), typeof(double))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.GetNumber))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.ItemIds))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.Remove), typeof(int), typeof(double))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.SetNumber))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.ToString))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.TruncateNumbers))]
    [HarmonyPrefix]
    public static void ItemBlockParameterLess(ref ItemBlock __instance) => UpdateItemCount(ref __instance);

    /// <summary>
    /// Increase the number of items before performing various operations on ItemBlock and an other
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="other"></param>
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.Add))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.Contains), typeof(ItemBlock))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.Remove), typeof(ItemBlock))]
    [HarmonyPatch(typeof(ItemBlock), nameof(ItemBlock.RemovePartially), typeof(ItemBlock))]
    [HarmonyPrefix]
    public static void ItemBlockAndOther(ref ItemBlock __instance, ref ItemBlock other)
    {
        UpdateItemCount(ref __instance);
        if (other == null) return;
        UpdateItemCount(ref other);
    }
    
    // Add 0 to the array until the item counts match StringIds.NumItems
    private static void UpdateItemCount(ref ItemBlock item)
    {
        while (item.items.Length < StringIds.NumItems) item.items = item.items.AddToArray(0);
    }
}