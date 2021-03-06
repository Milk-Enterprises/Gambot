﻿using System;
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
        MessageHandlerTestBase<FactoidCommandProducer>
    {
        public override void InitializeSubject()
        {
            Subject = new FactoidCommandProducer();
            Subject.Initialize(DataStoreManager.Object);
        }

        [TestClass]
        public class Digest : TFactoidCommandHandler
        {
            [TestMethod]
            public void ShouldParseMessageWithNoVariables()
            {
                var factoidDataStore = GetDataStore("Factoid");
                factoidDataStore.Setup(
                    rds => rds.Put(It.IsAny<string>(), It.IsAny<string>()))
                              .Returns(true);
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

                var returnValue = Subject.Process(messageStub, true);

                returnValue.Message.Should().Be(expectedResponse);
            }
        }
    }
}
