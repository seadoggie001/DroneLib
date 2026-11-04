using DroneLib.Model;
using HarmonyLib;
using JetBrains.Annotations;

namespace DroneLib.Registration;

/// <summary>
/// Function registration
/// </summary>
[UsedImplicitly]
public class Function : IRegistration<BaseFunction>
{
    /// <summary>The functions already registered</summary>
    public static IEnumerable<BaseFunction> ModdedFunctions { get; private set; } = [];

    /// <summary>
    /// Register a new modded function
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="Exception"></exception>
    public void Add(BaseFunction item)
    {
        try
        {
            // If the functions are null, there are major issues
            if (BuiltinFunctions.Functions is null)
                throw new NullReferenceException("Failed to register function due to internal issues.");

            // Throw an exception if the modded function list already contains this function's name
            if (ModdedFunctions.Any(m => m.Name == item.Name))
                throw new Exception($"Cannot register another modded function with the name of {item.Name}.");

            // Throw an exception if the game's function list already contains this function's name
            if (BuiltinFunctions.Functions.ContainsKey(item.Name))
                throw new Exception($"Cannot register another function with the name of {item.Name}.");

            Main.Log.LogInfo($"Registering new custom function: {item.Name}.");

            // Set the internal PyFunction
            item.PyFunction = new PyFunction(
                item.Name,
                (list, simulation, execution, droneId) =>
                {
                    item.Binding(new BindingParams(list, simulation, execution, droneId));
                    item.RegisterSideEffect(execution.States[droneId]);
                    return 0.0;
                }
                , item.MethodObject
                , item.IsFree
            );

            // Track this function (it is needed in ExecutionPatch to actually run the function)
            ModdedFunctions = ModdedFunctions.AddItem(item);

            // Actually add it to the function list now
            BuiltinFunctions.functions.Add(item.Name, item.PyFunction);
        }
        catch (Exception ex)
        {
            Main.LogException(ex);
            throw;
        }
    }

    /// <summary>
    /// Deregister a modded function by name
    /// </summary>
    /// <param name="name"></param>
    public void Remove(string name)
    {
        if (BuiltinFunctions.functions.TryGetValue(name, out PyFunction _)) BuiltinFunctions.functions.Remove(name);

        ModdedFunctions = ModdedFunctions.Where(m => m.Name != name);
    }
}