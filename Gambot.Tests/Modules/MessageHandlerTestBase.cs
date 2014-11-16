using System.Collections.Generic;
using Gambot.Core;
using Gambot.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Modules
{
    public abstract class MessageHandlerTestBase<THandlerType>
        where THandlerType : IMessageProducer
    {
        protected THandlerType Subject { get; set; }

        protected Mock<IDataStoreManager> DataStoreManager { get; set; }

        protected IDictionary<string, Mock<IDataStore>> DataStores { get; set; }

        [TestInitialize]
        public void ResetMocks()
        {
            DataStoreManager = new Mock<IDataStoreManager>();
            DataStores = new Dictionary<string, Mock<IDataStore>>();
        }

        public abstract void InitializeSubject();

        protected Mock<IDataStore> GetDataStore(string dataStoreName)
        {
            if (!DataStores.ContainsKey(dataStoreName))
                DataStores.Add(dataStoreName, new Mock<IDataStore>());

            var ds = DataStores[dataStoreName];
            DataStoreManager.Setup(idsm => idsm.Get(dataStoreName))
                            .Returns(ds.Object);
                // successive calls w/ same params will overwrite previous

            return ds;
        }
    }
}
