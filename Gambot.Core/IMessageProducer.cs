using Gambot.Data;

namespace Gambot.Core
{
    public interface IMessageProducer
    {
        void Initialize(IDataStoreManager dataStoreManager);

        /// <summary>
        /// Produces contents that users actively seek (like quotes, factoids; something that the user specifically wants triggered).
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <param name="addressed">A boolean indicating whether or not the bot was mentioned in the message. For example, if the bot's name is "Gambot", then <paramref name="addressed"/> would be true if the user typed "gambot, some thing here".</param>
        /// <returns>Returns the response message (if it responds to the input message); <b>null</b> if the handler does not care for the message.</returns>
        IMessage Process(IMessage message, bool addressed);
    }

    /// <summary>
    /// Reacts to user inputs that they did not necessarily intend (like TLAs).
    /// </summary>
    public interface IMessageReactor : IMessageProducer
    { }

    /// <summary>
    /// Transforms messages before they are finally sent.
    /// </summary>
    public interface IMessageTransformer : IMessageProducer
    { }
}
