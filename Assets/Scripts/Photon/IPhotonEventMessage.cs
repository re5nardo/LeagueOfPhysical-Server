using GameFramework;

public interface IPhotonEventMessage : IMessage
{
    int senderID { get; set; }
    byte GetEventID();
}
