using System.Reflection;

namespace DroneLib.Helpers;

public static class Reflection
{
    /// <summary>
    /// Locates a static field, optionally calls an update function, and returns the current value
    /// </summary>
    /// <param name="type">Type of object to locate field in</param>
    /// <param name="fieldName">Name of the field to locate</param>
    /// <param name="flags">Flags to use for binding</param>
    /// <param name="update">Function that can modify and return a new value for the field</param>
    /// <typeparam name="T">Type of the field to modify</typeparam>
    /// <returns>The current value of the field</returns>
    /// <exception cref="Exception">Failed to locate field or field was not of type T</exception>
    public static T GetStaticField<T>(Type type, string fieldName, BindingFlags flags, Func<T, T> update = null)
    {
        // Locate the field
        FieldInfo fieldInfo = type.GetField(fieldName, flags);
        if (fieldInfo is null)
            throw new Exception($"Reflection failed. Expected to find field {type.Name}.{fieldName}.");

        // Get the actual value of the field
        object value = fieldInfo.GetValue(null);
        if (value is not T valueOfType)
            throw new Exception($"Reflection failed. Expected {type.Name}.{fieldName} to be of type {typeof(T).Name}.");
        // Return if an update isn't needed
        if (update is null) return valueOfType;
        
        // Run the update function
        T newValue = update.Invoke(valueOfType);
        // Update the field
        fieldInfo.SetValue(null, newValue);
        valueOfType = newValue;
        // Return the updated value
        return valueOfType;
    }
}