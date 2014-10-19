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
    internal class TFactoidCommandHandler :
        MessageHandlerTestBase<FactoidCommandHandler>
    {
        public override void InitializeSubject()
        {
            Subject = new FactoidCommandHandler();
            Subject.Initialize(DataStoreManager.Object);
        }

        [TestClass]
        public class Digest : TFactoidCommandHandler
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
                var messageStub = new StubMessage()
                {
                    Action = false,
                    Text = "hello <reply> " + replyMsg,
                    Where = "some_place",
                    Who = name
                };

                var returnValue = Subject.Process(String.Empty, messageStub,
                                                  true);

                returnValue.Should().Be(expectedResponse);
            }
        }
    }
}
