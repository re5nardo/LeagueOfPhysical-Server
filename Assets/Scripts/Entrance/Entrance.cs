using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Entrance : MonoBehaviour
{
    [SerializeField] private LoginComponent loginComponent;
    [SerializeField] private Text textState;
    [SerializeField] private InputField inputFieldRoomName;
    [SerializeField] private bool autoCreateRoom = true;

#region MonoBehaviour
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => LOP.Application.IsInitialized);

        if (autoCreateRoom)
        {
            SceneManager.LoadScene("Room");
        }
    }
#endregion

#region Event Handler
    public void OnCreateRoomBtnClicked()
    {
        SceneManager.LoadScene("Room");
    }
#endregion
}
