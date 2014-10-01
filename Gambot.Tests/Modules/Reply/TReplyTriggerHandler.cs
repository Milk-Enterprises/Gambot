using FluentAssertions;
using Gambot.Core;
using Gambot.Modules.Reply;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Modules.Reply
{
    [TestClass]
    public class TReplyTriggerHandler
    {
        protected ReplyTriggerHandler Subject { get; set; }

        protected Mock<IDataStore> DataStore { get; set; }

        [TestInitialize]
        public void InitializeSubject()
        {
            DataStore = new Mock<IDataStore>();
            var dsm = new Mock<IDataStoreManager>();
            dsm.Setup(idsm => idsm.Get(It.IsAny<string>())).Returns(DataStore.Object);
            Subject = new ReplyTriggerHandler();
            Subject.Initialize(dsm.Object);
        }

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