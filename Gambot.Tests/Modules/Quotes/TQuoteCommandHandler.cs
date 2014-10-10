using System;
using FluentAssertions;
using Gambot.Core;
using Gambot.Modules.Quotes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gambot.Tests.Modules.Quotes
{
    [TestClass]
    internal class TQuoteCommandHandler : MessageHandlerTestBase<QuoteCommandHandler>
    {
        protected Mock<IVariableHandler> VariableHandler;
        
        public override void InitializeSubject()
        {
            VariableHandler = new Mock<IVariableHandler>();
            Subject = new QuoteCommandHandler(VariableHandler.Object);
            Subject.Initialize(DataStoreManager.Object);
        }

        [TestClass]
        public class Digest : TQuoteCommandHandler
        {
            private IMessage stubMessage;
            private const string SendingUsersName = "Dude";
            
            [TestMethod]
            public void ShouldApologizeForNoQuotesForUser()
            {
                const string quoteTarget = "Robert";

                SetupQuotesLog(quoteTarget, null);
                TestQuoteCommand(quoteTarget, String.Format("Sorry, {0} has not said anything quote-worthy.", quoteTarget));
            }

            [TestMethod]
            public void ShouldListRandomQuoteFromUser()
            {
                const string quoteTarget = "Robert";
                const string quoteMsg = "i delight in dicks";

                SetupQuotesLog(quoteTarget, quoteMsg);
                TestQuoteCommand(quoteTarget, String.Format("<{0}> {1}", quoteTarget, quoteMsg));
            }



            private void SetupQuotesLog(string who, string randomMsg)
            {
                GetDataStore("Quotes").Setup(ids => ids.GetRandomValue(who)).Returns(randomMsg);
            }

            private void TestQuoteCommand(string quoteTarget, string expectedResponse, bool expectedResult = false)
            {
                // Setup
                var messageStub = new StubMessage(String.Format("quote {0}", quoteTarget), where: "some_place", who: SendingUsersName);

                InitializeSubject();

                // Act
                var messengerMock = new Mock<IMessenger>();
                var returnValue = Subject.Digest(messengerMock.Object, messageStub, true);

                // Verify
                returnValue.Should().Be(expectedResult);
                messengerMock.Verify(im => im.SendMessage(expectedResponse, messageStub.Where, false), Times.Once);
            }
        }
    }
}