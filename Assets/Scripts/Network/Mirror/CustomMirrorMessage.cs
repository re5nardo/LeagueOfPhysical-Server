using Mirror;

public struct CustomMirrorMessage : NetworkMessage
{
    public int sender;
    public byte id;
    public IMirrorMessage payload;
}
