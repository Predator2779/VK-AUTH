using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class VkAuth : MonoBehaviour
{
    public Action<VkUserResponse> OnUserAuthorized;
    
    private const string AppId = "52818805";
    private const string ServiceKey = "4dad4a624dad4a624dad4a62994e88b91744dad4dad4a622af8d9673970361d3b282282";
    private const string SecretKey = "CzYsUMY3NfKEhcWdAjEx";
    private const string RedirectUri = "https://oauth.vk.com/blank.html";
    private const string ApiVersion = "5.199";
    
    private const string Scope = "email,friends"; 

    private string _accessToken;
    private string _userId;
    private string _email;
    
    public void StartAuthorization()
    {
        string url = "https://oauth.vk.com/authorize?" +
                     $"client_id={AppId}&" +
                     "display=popup&" +
                     $"redirect_uri={RedirectUri}&" +
                     $"scope={Scope}&" +
                     "response_type=code&" +
                     $"v={ApiVersion}";

        Application.OpenURL(url);
    }
    
    public void ProcessAuthorizationCode(TMP_Text code)
    {
        StartCoroutine(GetAccessToken(code.text));
    }
    
    private IEnumerator GetAccessToken(string code)
    {
        string url = "https://oauth.vk.com/access_token?" +
                     $"client_id={AppId}&" +
                     $"client_secret={SecretKey}&" +
                     $"redirect_uri={RedirectUri}&" +
                     $"code={code}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Token Response: {request.downloadHandler.text}");
                
                var tokenData = JsonUtility.FromJson<VkTokenResponse>(request.downloadHandler.text);
                _accessToken = tokenData.access_token;
                _userId = tokenData.user_id;
                _email = tokenData.email;

                Debug.Log($"Access Token: {_accessToken}");
                Debug.Log($"User ID: {_userId}");
                Debug.Log($"Email: {_email}");
                
                StartCoroutine(GetUserInfo());
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
            }
        }
    }
    
    private IEnumerator GetUserInfo()
    {
        string url = "https://api.vk.com/method/users.get?" +
                     $"user_ids={_userId}&" +
                     $"fields=photo_100&" +
                     $"access_token={_accessToken}&" +
                     $"v={ApiVersion}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"User Info: {request.downloadHandler.text}");
                var userData = JsonUtility.FromJson<VkUserResponse>(request.downloadHandler.text);
                Debug.Log($"Name: {userData.response[0].first_name} {userData.response[0].last_name}");
                OnUserAuthorized?.Invoke(userData);
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
            }
        }
    }
}