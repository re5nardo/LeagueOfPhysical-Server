using GameFramework;

public interface IMirrorMessage : IMessage, IPoolObject
{
    byte GetMessageId();
}
