namespace VIC.ObjectConfig.Abstraction
{
    public interface IConfig
    {
        IConfigSource GetConfigSource(string Key);
    }
}