using FluentAssertions;
using Gambot.Core;
using Gambot.Data;
using Gambot.Modules.Reply;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Modules.Reply
{
    [TestClass]
    public class TReplyTriggerHandler
    {
        internal ReplyTriggerHandler Subject { get; set; }

        protected Mock<IDataStore> DataStore { get; set; }
        protected Mock<IVariableHandler> VariableHandler { get; set; }

        [TestInitialize]
        public void InitializeSubject()
        {
            DataStore = new Mock<IDataStore>();
            VariableHandler = new Mock<IVariableHandler>();

            var dsm = new Mock<IDataStoreManager>();
            dsm.Setup(idsm => idsm.Get(It.IsAny<string>())).Returns(DataStore.Object);
            Subject = new ReplyTriggerHandler(VariableHandler.Object);
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
                VariableHandler.Setup(vh => vh.Substitute(It.IsAny<string>(), It.IsAny<IMessage>())).Returns<string, IMessage>((val, msg) => val);
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
                VariableHandler.Verify(vh => vh.Substitute(It.IsAny<string>(), It.IsAny<IMessage>()), Times.Once);
            }
        }
    }
}