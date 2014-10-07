using System;
using System.Collections.Generic;
using FluentAssertions;
using Gambot.Core;
using Gambot.Modules.Quotes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Modules.Quotes
{
    [TestClass]
    internal class TRememberCommandHandler : MessageHandlerTestBase<RememberCommandHandler>
    {
        public override void InitializeSubject()
        {
            Subject = new RememberCommandHandler();
            Subject.Initialize(DataStoreManager.Object);
        }

        [TestClass]
        public class Digest : TRememberCommandHandler
        {
            public void ShouldRememberMessageThatTargetDidSay()
            {
                const string name = "Dude";
                const string rememberTarget = "RememberMe";
                const string rememberMsg = "hello man world";
                const string logMsg = "sup guys hello man world";
                var expectedResponse = String.Format("Okay, {0}, remembering \"{1}\".", name, rememberMsg);

                var logDataStore = GetDataStore("Log");
                logDataStore.Setup(ids => ids.GetAllValues(name)).Returns(new List<string> { logMsg });
                var remDataStore = GetDataStore("Remember");
                InitializeSubject();

                var messengerMock = new Mock<IMessenger>();
                var messageStub = new StubMessage()
                {
                    Action = false,
                    Text = String.Format("remember {0} {1}", rememberTarget, rememberMsg),
                    Where = "some_place",
                    Who = name
                };

                var returnValue = Subject.Digest(messengerMock.Object, messageStub, true);

                returnValue.Should().BeFalse();
                remDataStore.Verify(ids => ids.GetAllValues(name), Times.Once);
                remDataStore.Verify(ids => ids.Put(rememberTarget, logMsg), Times.Once);
                messengerMock.Verify(im => im.SendMessage(expectedResponse, messageStub.Where, false), Times.Once);
            }
        }
    }
}