using Gambot.Data;

namespace Gambot.Core
{
    public interface IMessageHandler
    {
        void Initialize(IDataStoreManager dataStoreManager);

        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="response">The current response to send.</param>
        /// <param name="message">The message to process.</param>
        /// <param name="addressed">A boolean indicating whether or not the bot was mentioned in the message. For example, if the bot's name is "Gambot", then <paramref name="addressed"/> would be true if the user typed "gambot, some thing here".</param>
        /// <returns>Returns the new response to be fed to the next processor in the pipeline; <b>null</b> if the message should not have a response and not go any further in the pipeline.</returns>
        string Process(string currentResponse, IMessage message, bool addressed);
    }
}
