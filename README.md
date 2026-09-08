# DroneLib

A modding library designed for the game The Farmer Was Replaced to assist modders in development.

> [!WARNING]
> This library is in beta, be aware of issues and please report them or submit a pull request to fix.

## Features
 - Simplified custom Function definition and registration
   - Registers function in `BuiltinFunctions.cs`, allowing the game to locate the function for scripts
   - Function execution is performed in the function object's own `PerformAction` function
 - Simplified custom Item definition and registration
   - Registers item in `StringIds.cs`, allowing the game to locate the item's name/id
   - Registers item in `ResourceManager.cs`, allowing the game to locate the item's details

## How to Use
For now, no nuget package is offered. The library is not well tested and should be considered beta.

### Installation
1. Download the nuget package from the releases. Alternately, download the source and pack the nuget package:
   - `dotnet pack --version <version-number> --output </path/to/local/nuget/feed/>` 
2. Move the nuget package to a custom nuget source
3. If you don't have a local nuget source yet, you can create one like this:
`dotnet nuget add source "C:\Your\Custom\Nuget\Path" --name "LocalNugetFeed"`
4. Reference the nuget package in your mod's .csproj file:
`<PackageReference Include="DroneLib" Version="0.0.1-20200101-1234" />`

### Patching

Before patching methods with your mod, call `DroneLib.Main.PatchAll()`. 
This is often located in your mod's `Plugin.cs` file

### Registration

   - Registering new items and functions likely needs to be performed after `Awake()` is called on your mod.
     - Using the `Start()` method is suggested.
     - This has not been tested well
   - Add a new function: `DroneLib.Functions.Registration.RegisterFunction(new HelloWorld());`
     - You __must__ extend `BaseFunction`
     - You __should__ override `BaseFunction.Name`
     - You __may__ call `CorrectParams()` or `NoParams()` to validate the parameters passed in `ValidateCall()`
     - You __may__ call `StoreArgument()` to pass data from `ValidateCall()` to `PerformAction()`
     - You __should not__ store data elsewhere in the custom function class. 
       - A single object instance is used for all function calls and data may persist between function calls
       - Function calls happen on multiple drones and may happen before another drone's function call is complete
   - Add a new item: `DroneLib.Item.Registration.RegisterItem(new FoolsGold());`
     - You __should__ create `ItemSO` with `ScriptableObject.CreateInstance<ItemSO>()`
     - You __should__ provide a value for all properties of the `ItemSO`