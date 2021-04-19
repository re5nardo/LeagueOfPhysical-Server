using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using UnityEngine.UI;
using System;

public class Entrance : PunBehaviour
{
    [SerializeField] private LoginComponent loginComponent;
    [SerializeField] private Text textState;
    [SerializeField] private InputField inputFieldRoomName;
    [SerializeField] private bool autoCreateRoom = true;

#region MonoBehaviour
    private void Start()
    {
#if UNITY_STANDALONE && !UNITY_EDITOR
        var arguments = Environment.GetCommandLineArgs();
        var preferredRegion = Util.TryEnumParse(arguments[1], CloudRegionCode.kr);

        PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
        PhotonNetwork.PhotonServerSettings.PreferredRegion = preferredRegion;
#endif
        loginComponent.successCallback = () =>
        {
            Debug.Log("Congratulations, you made your first successful API call!");

            ConnectToMasterServer();
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

    private void CreateRoom(string roomName, string[] expectedUsers)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        roomOptions.MaxPlayers = 3;     //  1 server + 2 players

        PhotonNetwork.CreateRoom(roomName, roomOptions, null, expectedUsers);
    }

#region Event Handler
    public void OnCreateRoomBtnClicked()
    {
        CreateRoom(inputFieldRoomName.text, null);
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

	public override void OnConnectedToMaster()
	{
        textState.text = "마스터 서버에 접속하였습니다.";

		foreach(TypedLobbyInfo lobbyInfo in PhotonNetwork.LobbyStatistics)
		{
			Debug.Log(string.Format("[Lobby Info] Type : {0}, IsDefault : {1}, Name : {2}, RoomCount : {3}, PlayerCount : {4}", lobbyInfo.Type, lobbyInfo.IsDefault, lobbyInfo.Name, lobbyInfo.RoomCount, lobbyInfo.PlayerCount));
		}

        ConnectToLobby();
	}

	public override void OnJoinedLobby()
	{
        textState.text = "로비에 접속하였습니다.";

#if UNITY_STANDALONE && !UNITY_EDITOR
        var arguments = Environment.GetCommandLineArgs();
        string roomName = arguments[2];
        //string[] expectedUsers = new string[arguments.Length - 3];
        //for (int i = 3; i < arguments.Length; ++i)
        //{
        //    expectedUsers[i - 3] = arguments[i];
        //}

        var matchSettingData = AppDataContainer.Get<MatchSettingData>();
        matchSettingData.matchSetting.matchType = Util.TryEnumParse(arguments[3], MatchType.Friendly);
        matchSettingData.matchSetting.subGameId = arguments[4];
        matchSettingData.matchSetting.mapId = arguments[5];
        
        CreateRoom(roomName, null);
#else
        if (autoCreateRoom)
        {
            OnCreateRoomBtnClicked();
        }
#endif
    }

    public override void OnCreatedRoom()
    {
        textState.text = "룸을 생성하였습니다.";

        PhotonNetwork.isMessageQueueRunning = false;

        SceneManager.LoadScene("Room");
    }
#endregion
}
