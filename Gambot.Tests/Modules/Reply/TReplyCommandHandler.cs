using System;
using FluentAssertions;
using Gambot.Core;
using Gambot.Data;
using Gambot.Modules.Reply;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Modules.Reply
{
    [TestClass]
    public class TReplyCommandHandler : MessageHandlerTestBase<ReplyCommandHandler>
    {
        [TestClass]
        public class Digest : TReplyCommandHandler
        {
            [TestMethod]
            public void ShouldParseMessageWithNoVariables()
            {
                var replyDataStore = GetDataStore("Reply");
                InitializeSubject();

                // todo: use an auto mocker so i dont have to do this shit manually
                const string replyMsg = "hello man";
                const string name = "Dude";
                var expectedResponse = String.Format("Okay, {0}.", name);
                var messengerMock = new Mock<IMessenger>();
                var messageStub = new StubMessage()
                                  {
                                      Action = false,
                                      Text = "hello <reply> " + replyMsg,
                                      Where = "some_place",
                                      Who = name
                                  };

                var returnValue = Subject.Digest(messengerMock.Object, messageStub, true);

                returnValue.Should().BeFalse();
                replyDataStore.Verify(ids => ids.Put("hello", replyMsg), Times.Once);
                messengerMock.Verify(im => im.SendMessage(expectedResponse, messageStub.Where, false), Times.Once);
            }
        }
    }
}