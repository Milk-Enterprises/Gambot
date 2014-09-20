using System;

namespace Gambot.Core
{
    public interface IMessage
    {
        bool Action { get; }
        string Text { get; }
        string To { get; }
        string Where { get; }
        string Who { get; }
    }
}
