using Gambot.Data;

namespace Gambot.Core
{
    public class ProducerResponse
    {
        public string Message { get; set; }
        // todo: possibly add Where
        public bool IsAction { get; set; }

        public ProducerResponse(string message, bool isAction)
        {
            Message = message;
            IsAction = isAction;
        }
    }

    public interface IMessageHandler { }

    public interface IMessageFilter : IMessageHandler
    {
        void Initialize(IDataStoreManager dataStoreManager);

        /// <summary>
        /// Returns whether or not a message should be filtered.
        /// </summary>
        /// <returns><b>true</b> if the message should be processed; <b>false</b> if it should be discarded</returns>
        bool ShouldMessagePassThrough(IMessage message, bool addressed);
    }
    
    public interface IMessageListener : IMessageHandler
    {
        void Initialize(IDataStoreManager dataStoreManager);

        /// <summary>
        /// Listens to a message and acts upon it.
        /// </summary>
        void Listen(IMessage message, bool addressed);
    }
    
    public interface IMessageProducer : IMessageHandler
    {
        void Initialize(IDataStoreManager dataStoreManager);

        /// <summary>
        /// Produces contents that users actively seek (like quotes, factoids; something that the user specifically wants triggered).
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>Returns the response message (if it responds to the input message); <b>null</b> if the handler does not care for the message.</returns>
        ProducerResponse Process(IMessage message);
    }

    public interface IMessageReactor : IMessageHandler
    {
        void Initialize(IDataStoreManager dataStoreManager);

        /// <summary>
        /// Reacts to user inputs that they did not necessarily intend (like TLAs).
        /// </summary>
        /// <param name="message">The message to react to.</param>
        /// <param name="addressed">A boolean indicating whether or not the bot was mentioned in the message. For example, if the bot's name is "Gambot", then <paramref name="addressed"/> would be true if the user typed "gambot, some thing here".</param>
        /// <returns>Returns the response message (if it responds to the input message); <b>null</b> if the reactor did not react to the message.</returns>
        ProducerResponse Process(IMessage message, bool addressed);
    }

    public interface IMessageTransformer : IMessageHandler
    {
        void Initialize(IDataStoreManager dataStoreManager);

        /// <summary>
        /// Transforms messages before they are finally sent.
        /// </summary>
        /// <returns>Returns the transformed message. It should return the input message text if it does not plan on transforming the message.</returns>
        string Transform(bool isAction, string messageText, bool addressed);
    }
}
