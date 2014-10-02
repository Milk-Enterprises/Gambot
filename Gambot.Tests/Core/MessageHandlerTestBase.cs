using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gambot.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Core
{
    public class MessageHandlerTestBase<THandlerType> where THandlerType : IMessageHandler, new()
    {
        protected THandlerType Subject { get; set; }

        protected Mock<IDataStore> DataStore { get; set; }

        [TestInitialize]
        public void InitializeSubject()
        {
            DataStore = new Mock<IDataStore>();

            var dsm = new Mock<IDataStoreManager>();
            dsm.Setup(idsm => idsm.Get(It.IsAny<string>())).Returns(DataStore.Object);

            Subject = new THandlerType();
            Subject.Initialize(dsm.Object);
        }
    }
}
