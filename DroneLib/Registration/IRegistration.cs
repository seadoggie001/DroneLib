namespace DroneLib.Registration;

internal interface IRegistration<in T>
{
    void Add(T item);
    void Remove(string name);
}