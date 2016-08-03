namespace VIC.ObjectConfig.Abstraction
{
    public interface IConfigProvider
    {
        IConfigResolver GetResolver();
    }
}