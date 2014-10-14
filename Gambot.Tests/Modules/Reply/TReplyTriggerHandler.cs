using System;
using FluentAssertions;
using Gambot.Core;
using Gambot.Modules.Reply;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Modules.Reply
{
    [TestClass]
    internal class TReplyTriggerHandler :
        MessageHandlerTestBase<ReplyTriggerHandler>
    {
        protected Mock<IVariableHandler> VariableHandler { get; set; }

        public override void InitializeSubject()
        {
            VariableHandler = new Mock<IVariableHandler>();
            Subject = new ReplyTriggerHandler(VariableHandler.Object);
            Subject.Initialize(DataStoreManager.Object);
        }

        [TestClass]
        public class Digest : TReplyTriggerHandler
        {
            [TestMethod]
            public void ShouldParseMessageWithTrigger()
            {
                var replyDataStore = GetDataStore("Reply");
                InitializeSubject();

                // todo: use an auto mocker so i dont have to do this shit manually
                const string trigger = "hello";
                const string reply = "sup man";
                replyDataStore.Setup(dsm => dsm.GetRandomValue(trigger))
                              .Returns(reply);
                VariableHandler.Setup(
                    vh =>
                    vh.Substitute(It.IsAny<string>(), It.IsAny<IMessage>()))
                               .Returns<string, IMessage>((val, msg) => val);
                var messageStub = new StubMessage()
                {
                    Action = false,
                    Text = trigger,
                    Where = "some_place",
                    Who = "SomeDude69"
                };

                var returnValue = Subject.Process(String.Empty, messageStub,
                                                  true);

                returnValue.Should().Be(reply);
                replyDataStore.Verify(dsm => dsm.GetRandomValue(trigger),
                                      Times.Once);
                VariableHandler.Verify(
                    vh =>
                    vh.Substitute(It.IsAny<string>(), It.IsAny<IMessage>()),
                    Times.Once);
            }
        }
    }
}
