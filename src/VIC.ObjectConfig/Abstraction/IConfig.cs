namespace VIC.ObjectConfig.Abstraction
{
    public interface IConfig
    {
        T Get<T>(string key);
    }
}