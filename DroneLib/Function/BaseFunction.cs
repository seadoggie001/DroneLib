using System.Reflection;

namespace DroneLib.Function;

/// <summary>
/// A function framework that allows for a simpler, faster way to create functions
/// </summary>
public abstract class BaseFunction
{
    /// <summary>
    /// The custom side effect used to trigger the custom function call.
    /// When used: 
    /// <list type="bullet">
    ///     <item>the function name is placed in <c>SideEffect.currentSideEffect</c></item>
    ///     <item>the parameters are placed in an object in <c>SideEffect.currentSideEffectArgument2</c></item>
    /// </list>
    /// </summary>
    public const SideEffect ModdedSideEffect = (SideEffect)6969;

    /// <summary>
    /// The name of the Python function.
    /// </summary>
    /// <remarks>It's probably best to use lowercase and underscores to match Python functions.</remarks>
    public virtual string Name => "base_function";

    /// <summary>
    /// This appears to be unused in the game, though it is implemented.
    /// Used by get_time, get_tick_count, and quick_print. Indicates the Ops used should be 0?
    /// </summary>
    public virtual bool IsFree => false;

    /// <summary>
    /// This appears to be unused in the game, though it is implemented.
    /// My best reading of it is that it would provide an object as the first parameter of your function. 
    /// </summary>
    public virtual IPyObject MethodObject => null;

    /// <summary>
    /// Performs validation of the function arguments and the current state.
    /// </summary>
    /// <remarks>
    /// Store data needed for taking action with <c>StoreArgument</c>
    /// </remarks>
    /// <param name="validationState"></param>
    /// <returns></returns>
    public abstract void ValidateCall(FunctionValidation validationState);

    /// <summary>
    /// Sets the program state to the triggered by the 
    /// </summary>
    /// <param name="programState"></param>
    internal void RegisterSideEffect(ProgramState programState)
    {
        programState.currentSideEffect = ModdedSideEffect;
        programState.currentSideEffectArgument = new PyString(Name);
    }

    /// <summary>
    /// Stores an argument that will be passed to PerformAction
    /// </summary>
    /// <param name="programState">Current state of the program</param>
    /// <param name="arguments">Object to be stored</param>
    public void StoreArgument(ProgramState programState, object arguments) =>
        programState.currentSideEffectArgument2 = arguments;

    /// <summary>
    /// Calls internal function that validates the parameter list against the expected type.
    /// </summary>
    /// <param name="parameters">Function parameters to validate</param>
    /// <param name="expectedTypes">A list of expected types needed for the function to work</param>
    /// <exception cref="NullReferenceException">Failed to locate internal function</exception>
    /// <exception cref="ExecuteException">Parameter list doesn't match expected types. Do not handle.</exception>
    public void CorrectParams(List<IPyObject> parameters, List<Type> expectedTypes) =>
        CorrectParamsInfo.Invoke(null, [parameters, expectedTypes, Name]);

    /// <summary>
    /// Calls internal function that validates the parameter list is empty.
    /// </summary>
    /// <param name="parameters">Function parameters to validate</param>
    /// <exception cref="NullReferenceException">Failed to locate internal function</exception>
    /// <exception cref="ExecuteException">Parameter list is not empty. Do not handle.</exception>
    public void NoParams(List<IPyObject> parameters) => NoParamsInfo.Invoke(null, [parameters, Name]);

    /// <summary>
    /// Performs the action of the function. Replaces ApplySideEffect.
    /// </summary>
    /// <remarks>
    /// If you need multiple arguments, create a custom object and cast <c>customArgument</c>
    /// </remarks>
    /// <param name="execution">The current state of execution</param>
    /// <param name="droneId">The drone id reference executing the function</param>
    /// <param name="customArgument">Optional argument, registered with <see cref="StoreArgument"/></param>
    /// <returns>The return value for ApplySideEffect</returns>
    public abstract double PerformAction(Execution execution, int droneId, object customArgument = null);

    internal PyFunction PyFunction { get; set; }

    private static MethodInfo CorrectParamsInfo =>
        typeof(BuiltinFunctions).GetMethod("CorrectParams", BindingFlags.Static | BindingFlags.NonPublic)
        ?? throw new NullReferenceException("Unable to locate CorrectParams method");

    private static MethodInfo NoParamsInfo =>
        typeof(BuiltinFunctions).GetMethod("NoParams", BindingFlags.Static | BindingFlags.NonPublic)
        ?? throw new NullReferenceException("Unable to locate NoParams method");
}

// ToDo: Explain what FunctionValidation is and maybe rename it
public record FunctionValidation(List<IPyObject> Parameters, Simulation Simulation, Execution Execution, int DroneId)
{
    /// <summary>
    /// Parameters that the user passed to the function
    /// </summary>
    public List<IPyObject> Parameters { get; } = Parameters;

    /// <summary>
    /// The game object? It wraps Farm and the running code 
    /// </summary>
    public Simulation Simulation { get; } = Simulation;

    /// <summary>
    /// Current state of the running program
    /// </summary>
    public Execution Execution { get; } = Execution;

    /// <summary>
    /// ID of the drone that is performing the function
    /// </summary>
    public int DroneId { get; } = DroneId;
}