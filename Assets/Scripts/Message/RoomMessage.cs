using Mirror;

namespace RoomMessage
{
    public struct PlayerEnter
    {
        public NetworkConnection networkConnection;

        public PlayerEnter(NetworkConnection networkConnection)
        {
            this.networkConnection = networkConnection;
        }
    }

    public struct PlayerLeave
    {
        public NetworkConnection networkConnection;

        public PlayerLeave(NetworkConnection networkConnection)
        {
            this.networkConnection = networkConnection;
        }
    }
}
