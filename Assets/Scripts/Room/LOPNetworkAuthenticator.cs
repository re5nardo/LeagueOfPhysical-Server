using UnityEngine;
using Mirror;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/components/network-authenticators
    API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkAuthenticator.html
*/

public class LOPNetworkAuthenticator : NetworkAuthenticator
{
    #region Messages

    public struct AuthRequestMessage : NetworkMessage
    {
        public CustomProperties customProperties;
    }

    public struct AuthResponseMessage : NetworkMessage
    {
        public int code;
        public string message;
    }

    #endregion

    #region Server

    /// <summary>
    /// Called on server from StartServer to initialize the Authenticator
    /// <para>Server message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartServer()
    {
        // register a handler for the authentication request we expect from client
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }

    /// <summary>
    /// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    public override void OnServerAuthenticate(NetworkConnection conn) { }

    /// <summary>
    /// Called on server when the client's AuthRequestMessage arrives
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    /// <param name="msg">The message payload</param>
    public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
    {
        //  현재는 무조건 수락.. 추후에 게임에 예정된 플레이어인지 체크하는 로직 추가하기!
        //  ...
        bool authenticated = true;
        if (authenticated)
        {
            AuthResponseMessage authResponseMessage = new AuthResponseMessage();
            authResponseMessage.code = 200;
            authResponseMessage.message = "success";

            conn.Send(authResponseMessage);

            //  Save customProperties
            conn.authenticationData = msg.customProperties;

            // Accept the successful authentication
            ServerAccept(conn);
        }
        else
        {
            // create and send msg to client so it knows to disconnect
            AuthResponseMessage authResponseMessage = new AuthResponseMessage
            {
                code = 300,
                message = "Invalid Credentials",
            };

            conn.Send(authResponseMessage);

            // must set NetworkConnection isAuthenticated = false
            conn.isAuthenticated = false;

            ServerReject(conn);
        }
    }

    #endregion

    #region Client

    /// <summary>
    /// Called on client from StartClient to initialize the Authenticator
    /// <para>Client message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartClient()
    {
        // register a handler for the authentication response we expect from server
        NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
    }

    /// <summary>
    /// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
    /// </summary>
    public override void OnClientAuthenticate()
    {
        AuthRequestMessage authRequestMessage = new AuthRequestMessage();
        authRequestMessage.customProperties = new CustomProperties
        {
            userId = SystemInfo.deviceUniqueIdentifier,
            token = "token",
            //characterId = Random.Range(Define.MasterData.CharacterID.EVELYNN, Define.MasterData.CharacterID.MALPHITE + 1),
            characterId = Define.MasterData.CharacterID.GAREN,
        };

        NetworkClient.Send(authRequestMessage);
    }

    /// <summary>
    /// Called on client when the server's AuthResponseMessage arrives
    /// </summary>
    /// <param name="msg">The message payload</param>
    public void OnAuthResponseMessage(AuthResponseMessage msg)
    {
        if (msg.code == 200)
        {
            // Authentication has been accepted
            ClientAccept();
        }
        else
        {
            Debug.LogError($"Authentication Response: {msg.message}");

            // Authentication has been rejected
            ClientReject();
        }
    }

    #endregion
}
