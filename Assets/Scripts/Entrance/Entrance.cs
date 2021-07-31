using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using UnityEngine.UI;
using System.Collections;

public class Entrance : PunBehaviour
{
    [SerializeField] private LoginComponent loginComponent;
    [SerializeField] private Text textState;
    [SerializeField] private InputField inputFieldRoomName;
    [SerializeField] private bool autoCreateRoom = true;

#region MonoBehaviour
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => LOP.Application.IsInitialized);

        loginComponent.successCallback = () =>
        {
            Debug.Log("Congratulations, you made your first successful API call!");

            textState.text = "login success";

            if (autoCreateRoom)
            {
                SceneManager.LoadScene("Room");
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

#region Event Handler
    public void OnCreateRoomBtnClicked()
    {
        SceneManager.LoadScene("Room");
    }
#endregion
}
