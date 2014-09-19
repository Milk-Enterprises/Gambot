using System;
namespace Gambot
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
