namespace DroneLib;

/// <summary>
/// We need to register custom text for the mod, so we'll tell the internal localizer to stuff it... right in the dictionary
/// </summary>
public static class Localize
{
    /// <summary>
    /// Registers a custom value that will be stored in the current language's dictionary
    /// </summary>
    /// <remarks>This will persist until the game reloads or the player changes the language</remarks>
    /// <param name="key">A unique identifier for the text</param>
    /// <param name="value">The localized text</param>
    public static void RegisterValue(string key, string value)
    {
        Localizer.LoadLang(OptionHolder.GetString("language", "EN"));
        Localizer.language.Add(key, value);
    }

    //ToDo: Possibly help the user update the game's files? Seems like a bad idea though.
    //     Text would persist after removing mods
}