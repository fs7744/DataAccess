namespace VIC.DataAccess.Abstratiion
{
    public interface IDbManager
    {
        IDataCommand GetCommand(string commandName);
    }
}