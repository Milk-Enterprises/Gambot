using System;
namespace Gambot
{
    interface IMessage
    {
        bool Action { get; }
        string Text { get; }
        string To { get; }
        string Where { get; }
        string Who { get; }
    }
}
