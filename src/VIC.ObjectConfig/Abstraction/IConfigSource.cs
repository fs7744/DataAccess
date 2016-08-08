namespace VIC.ObjectConfig.Abstraction
{
    public interface IConfigSource
    {
        string Key { get; }

        object GetValue();
    }
}