using System;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    public interface IDataStoreManager
    {
        IDataStore Get(string name);
    }
}
