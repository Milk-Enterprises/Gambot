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
    internal class TRememberCommandHandler :
        MessageHandlerTestBase<RememberCommandProducer>
    {
        protected Mock<IRecentMessageStore> RecentMessageStore { get; set; }

        public override void InitializeSubject()
        {
            Subject = new RememberCommandProducer(RecentMessageStore.Object);
            Subject.Initialize(DataStoreManager.Object);
        }

        [TestInitialize]
        public void ResetLocalMocks()
        {
            RecentMessageStore = new Mock<IRecentMessageStore>();
        }

        [TestClass]
        public class Digest : TRememberCommandHandler
        {
            private IMessage stubMessage;
            private const string SendingUsersName = "Dude";

            [TestInitialize]
            public void ClearStubMessage()
            {
                stubMessage = null;
            }

            [TestMethod]
            public void ShouldApologizeIfUserDoesNotExist()
            {
                SetupStubMessage("someDude", "This is the entire message.");
                SetupRecentlySaidMessagesWithStubMessage(true);
                TestRememberCommand("someDude", "entire message",
                                    String.Format(
                                        "Sorry, I don't know anyone named \"{0}.\"",
                                        stubMessage.Who));
            }

            [TestMethod]
            public void ShouldApologizeIfUserDidNotRecentlySayMessage()
            {
                SetupStubMessage("someDude", "This is the entire message.");
                SetupRecentlySaidMessagesWithStubMessage();
                TestRememberCommand("someDude", "some other shit",
                                    String.Format(
                                        "Sorry, I don't remember what {0} said about \"{1}.\"",
                                        stubMessage.Who, "some other shit"));
            }

            [TestMethod]
            public void ShouldApologizeIfUserAttemptsToQuoteHimself()
            {
                SetupStubMessage(SendingUsersName, "This is the entire message.");
                SetupRecentlySaidMessagesWithStubMessage();
                TestRememberCommand(SendingUsersName, "some other shit",
                                    String.Format(
                                        "Sorry {0}, but you can't quote yourself.",
                                        stubMessage.Who));
            }

            [TestMethod]
            public void ShouldRememberMessageThatTargetDidSay()
            {
                GetDataStore("Quotes");
                SetupStubMessage("someDude", "This is the entire message.");
                SetupRecentlySaidMessagesWithStubMessage();
                TestRememberCommand(stubMessage.Who, "entire message",
                                    String.Format(
                                        "Okay, {0}, remembering \"{1}.\"",
                                        SendingUsersName, stubMessage.Text));
                VerifyQuoteIsInDataStore();
            }

            private void SetupStubMessage(string who, string text)
            {
                stubMessage = new StubMessage(text: text, who: who);
            }

            private void SetupRecentlySaidMessagesWithStubMessage(
                bool returnNull = false)
            {
                RecentMessageStore.Setup(
                    rms => rms.GetRecentMessagesFromUser(stubMessage.Who))
                                  .Returns(returnNull
                                               ? null
                                               : new[] {stubMessage});
            }

            private void TestRememberCommand(string rememberTarget,
                                             string rememberMsg,
                                             string expectedResponse)
            {
                // Setup
                var messageStub =
                    new StubMessage(
                        String.Format("remember {0} {1}", rememberTarget,
                                      rememberMsg), where: "some_place",
                        who: SendingUsersName);

                InitializeSubject();

                // Act
                var returnValue = Subject.Process(String.Empty, messageStub,
                                                  true);

                // Verify
                returnValue.Should().Be(expectedResponse);
            }

            private void VerifyQuoteIsInDataStore()
            {
                GetDataStore("Quotes")
                    .Verify(ids => ids.Put(stubMessage.Who, stubMessage.Text),
                            Times.Once);
            }
        }
    }
}
