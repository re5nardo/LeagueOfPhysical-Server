using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

//  https://api.playfab.com/docs/tutorials/landing-tournaments/photon-unity
public class LoginComponent : MonoBehaviour
{
    public string customId = "Server1";

    public Action successCallback;
    public Action<PlayFabError> errorCallback;

    private string _playFabPlayerIdCache;

    public void StartLogin()
    {
        Login();
    }

    /*
     * Step 1
     * We authenticate current PlayFab user normally. 
     * In this case we use LoginWithCustomID API call for simplicity.
     * You can absolutely use any Login method you want.
     * We use PlayFabSettings.DeviceUniqueIdentifier as our custom ID.
     * We pass RequestPhotonToken as a callback to be our next step, if 
     * authentication was successful.
     */
    private void Login()
    {
#if UNITY_EDITOR
        Login_Editor();
#elif UNITY_ANDROID
        Login_Android();
#elif UNITY_STANDALONE
        Login_StandAlone();
#endif
    }

    private void Login_Editor()
    {
#if UNITY_EDITOR
        var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, RequestPhotonToken, errorCallback);
#endif
    }

    private void Login_StandAlone()
    {
#if UNITY_STANDALONE && !UNITY_EDITOR
        var request = new LoginWithCustomIDRequest { CustomId = "Server1", CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, RequestPhotonToken, errorCallback);
#endif
    }

    private void Login_Android()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDevice = SystemInfo.deviceModel,
            AndroidDeviceId = PlayFabSettings.DeviceUniqueIdentifier,
            CreateAccount = true,
            OS = SystemInfo.operatingSystem,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, RequestPhotonToken, errorCallback);
#endif
    }

    /*
    * Step 2
    * We request Photon authentication token from PlayFab.
    * This is a crucial step, because Photon uses different authentication tokens
    * than PlayFab. Thus, you cannot directly use PlayFab SessionTicket and
    * you need to explicitely request a token. This API call requires you to 
    * pass Photon App ID. App ID may be hardcoded, but, in this example,
    * We are accessing it using convenient static field on PhotonNetwork class
    * We pass in AuthenticateWithPhoton as a callback to be our next step, if 
    * we have acquired token succesfully
    */
    private void RequestPhotonToken(LoginResult obj)
    {
        Debug.Log("PlayFab authenticated. Requesting photon token...");

        //We can player PlayFabId. This will come in handy during next step
        _playFabPlayerIdCache = obj.PlayFabId;
        Debug.Log("PlayFabId : " + obj.PlayFabId);

        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
            {
                PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID
            }, AuthenticateWithPhoton, errorCallback);
    }

    /*
     * Step 3
     * This is the final and the simplest step. We create new AuthenticationValues instance.
     * This class describes how to authenticate a players inside Photon environment.
     */
    private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj)
    {
        Debug.Log("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

        //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

        //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
        customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service

        //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
        customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

        //We finally tell Photon to use this authentication parameters throughout the entire application.
        PhotonNetwork.AuthValues = customAuth;

        successCallback();
    }
}
