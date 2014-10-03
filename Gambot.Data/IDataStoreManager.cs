namespace Gambot.Data
{
    public interface IDataStoreManager
    {
        IDataStore Get(string name);
    }
}
