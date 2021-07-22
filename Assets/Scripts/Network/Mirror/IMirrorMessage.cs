using GameFramework;

public interface IMirrorMessage : IMessage, IPoolable
{
    byte GetMessageId();
}
