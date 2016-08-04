namespace VIC.ObjectConfig.Abstraction
{
    public interface IConfigProvider
    {
        void SetConfig(IConfigStore config);
    }
}