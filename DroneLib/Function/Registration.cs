using System.Reflection;
using DroneLib.Helpers;
using HarmonyLib;

namespace DroneLib.Function;

public static class Registration
{
    private static Dictionary<string, PyFunction> BuiltinFunctionsFunctions =>
        Reflection.GetStaticField<Dictionary<string, PyFunction>>(typeof(BuiltinFunctions), 
            "functions",
            BindingFlags.NonPublic | BindingFlags.Static);

    /// <summary>
    /// The functions already registered
    /// </summary>
    public static IEnumerable<BaseFunction> ModdedFunctions { get; private set; } = [];

    /// <summary>
    /// Register your custom function
    /// </summary>
    /// <param name="function">The custom function to register</param>
    /// <exception cref="Exception">Thrown when the function name is already used</exception>
    public static void RegisterFunction(BaseFunction function)
    {
        try
        {
            // If the functions are null, try getting them to load them all
            if (BuiltinFunctionsFunctions is null) _ = BuiltinFunctions.Functions;

            // If they're still null, there are major issues
            if (BuiltinFunctionsFunctions is null)
                throw new NullReferenceException("Failed to register function due to internal issues.");

            // Throw an exception if the modded function list already contains this function's name
            if (ModdedFunctions.Any(m => m.Name == function.Name))
                throw new Exception($"Cannot register another modded function with the name of {function.Name}.");

            // Throw an exception if the game's function list already contains this function's name
            if (BuiltinFunctionsFunctions?.ContainsKey(function.Name) ?? false)
                throw new Exception($"Cannot register another function with the name of {function.Name}.");

            Main.Log.LogInfo($"Registering new custom function: {function.Name}.");

            // Set the internal PyFunction
            function.PyFunction = new PyFunction(
                function.Name,
                (list, simulation, execution, droneId) =>
                {
                    function.ValidateCall(new FunctionValidation(list, simulation, execution, droneId));
                    function.RegisterSideEffect(execution.States[droneId]);
                    return 0.0;
                }
                , function.MethodObject
                , function.IsFree
            );

            // Track this function (it is needed in ExecutionPatch to actually run the function)
            ModdedFunctions = ModdedFunctions.AddItem(function);

            // Actually add it to the function list now
            BuiltinFunctionsFunctions!.Add(function.Name, function.PyFunction);
        }
        catch (Exception ex)
        {
            Main.LogException(ex);
            throw;
        }
    }

    // ToDo: Unregister a function?
}