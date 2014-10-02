using System;
using System.Linq;
using FluentAssertions;
using Gambot.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Core
{
    [TestClass]
    public class TReplyTriggerHandler : MessageHandlerTestBase<ReplyTriggerHandler>
    {
        [TestClass]
        public class Digest : TReplyTriggerHandler
        {
            [TestMethod]
            public void ShouldParseMessageWithTrigger()
            {
                // todo: use an auto mocker so i dont have to do this shit manually
                const string trigger = "hello";
                const string reply = "sup man";
                DataStore.Setup(dsm => dsm.GetRandomValue(trigger)).Returns(reply);
                var messengerMock = new Mock<IMessenger>();
                var messageStub = new StubMessage()
                {
                    Action = false,
                    Text = trigger,
                    Where = "some_place",
                    Who = "SomeDude69"
                };

                var returnValue = Subject.Digest(messengerMock.Object, messageStub, true);

                DataStore.Verify(dsm => dsm.GetRandomValue(trigger), Times.Once);
                returnValue.Should().BeFalse();
                messengerMock.Verify(im => im.SendMessage(reply, messageStub.Where, false), Times.Once);
            }
        }
    }
}