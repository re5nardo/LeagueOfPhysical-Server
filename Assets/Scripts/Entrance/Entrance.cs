using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using UnityEngine.UI;

public class Entrance : PunBehaviour
{
    [SerializeField] private LoginComponent loginComponent;
    [SerializeField] private Text textState;
    [SerializeField] private InputField inputFieldRoomName;

    public bool autoCreateRoom = true;

    #region MonoBehaviour
    private void Start()
    {
#if UNITY_STANDALONE && !UNITY_EDITOR
        AutoCreateRoom = true;
#endif
        loginComponent.successCallback = () =>
        {
            Debug.Log("Congratulations, you made your first successful API call!");

            if (autoCreateRoom)
            {
                OnCreateRoomBtnClicked();
            }
        };

        loginComponent.errorCallback = (error) =>
        {
            Debug.LogWarning("Something went wrong with your first API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        };

        loginComponent.StartLogin();
    }
    #endregion

    private void ConnectToMasterServer()
    {
        textState.text = "마스터 서버에 접속중입니다.";

		PhotonNetwork.ConnectUsingSettings("v1.0");
    }

	private void ConnectToLobby()
    {
        textState.text = "로비에 접속중입니다.";

		PhotonNetwork.JoinLobby();
    }

    #region Event Handler
    public void OnCreateRoomBtnClicked()
    {
        ConnectToMasterServer();
    }
    #endregion

    #region MonoBehaviourPunCallbacks
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        textState.text = string.Format("마스터 서버 접속에 실패했습니다, cause : {0}", cause.ToString());
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
		Debug.LogError(string.Format("[Photon OnConnectionFail] cause : {0}", cause.ToString()));
    }

	public override void OnConnectedToMaster ()
	{
        textState.text = "마스터 서버에 접속하였습니다.";

		foreach(TypedLobbyInfo lobbyInfo in PhotonNetwork.LobbyStatistics)
		{
			Debug.Log(string.Format("[Lobby Info] Type : {0}, IsDefault : {1}, Name : {2}, RoomCount : {3}, PlayerCount : {4}", lobbyInfo.Type, lobbyInfo.IsDefault, lobbyInfo.Name, lobbyInfo.RoomCount, lobbyInfo.PlayerCount));
		}

        ConnectToLobby();
	}

	public override void OnJoinedLobby ()
	{
        textState.text = "로비에 접속하였습니다.";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;

        string roomName = inputFieldRoomName.text;

#if UNITY_STANDALONE && !UNITY_EDITOR
        var arguments = System.Environment.GetCommandLineArgs();

        roomName = arguments[1];
#endif

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }

    public override void OnCreatedRoom()
    {
        textState.text = "룸을 생성하였습니다.";

        PhotonNetwork.isMessageQueueRunning = false;

        SceneManager.LoadScene("Room");
    }
#endregion
}