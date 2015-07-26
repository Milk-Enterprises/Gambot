using System;
using FluentAssertions;
using Gambot.Core;
using Gambot.Data;
using Gambot.Modules.Factoid;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Modules.Reply
{
    [TestClass]
    internal class TFactoidTriggerHandler :
        MessageHandlerTestBase<FactoidTriggerReactor>
    {
        protected Mock<IVariableHandler> VariableHandler { get; set; }

        public override void InitializeSubject()
        {
            VariableHandler = new Mock<IVariableHandler>();
            Subject = new FactoidTriggerReactor(VariableHandler.Object);
            Subject.Initialize(DataStoreManager.Object);
        }

        [TestClass]
        public class Digest : TFactoidTriggerHandler
        {
            [TestMethod]
            public void ShouldParseMessageWithTrigger()
            {
                var factoidDataStore = GetDataStore("Factoid");
                InitializeSubject();

                // todo: use an auto mocker so i dont have to do this shit manually
                const string trigger = "hello";
                const string reply = "sup man";
                factoidDataStore.Setup(dsm => dsm.GetRandomValue(trigger))
                              .Returns(new DataStoreValue(0, trigger, "<reply> " + reply));
                VariableHandler.Setup(
                    vh =>
                    vh.Substitute(It.IsAny<string>(), It.IsAny<IMessage>(), It.IsAny<VariableReplacement[]>()))
                               .Returns<string, IMessage, VariableReplacement[]>((val, msg, repls) => val);
                var messageStub = new StubMessage()
                {
                    Action = false,
                    Text = trigger,
                    Where = "some_place",
                    Who = "SomeDude69"
                };

                var returnValue = Subject.Process(messageStub, true);

                returnValue.Message.Should().Be(reply);
                factoidDataStore.Verify(dsm => dsm.GetRandomValue(trigger),
                                      Times.Once);
                VariableHandler.Verify(
                    vh =>
                    vh.Substitute(It.IsAny<string>(), It.IsAny<IMessage>()),
                    Times.Once);
            }
        }
    }
}
