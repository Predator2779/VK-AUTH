using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class VkRequestExample : MonoBehaviour
{
    [SerializeField] private string _id;

    private const string _token = "4dad4a624dad4a624dad4a62994e88b91744dad4dad4a622af8d9673970361d3b282282";
    private const string _vkUrl = "https://api.vk.com";

    public void GetUserInfo(Action<VkUserResponse> action)
    {
        StartCoroutine(SendRequest(action));
    }

    private IEnumerator SendRequest(Action<VkUserResponse> action)
    {
        var url =
            $"{_vkUrl}" +                           // осн адрес
            $"/method/users.get?user_ids={_id}" +   // id пользователя вк
            "&fields=photo_400_orig,bdate,online" + // параметры (дата рожд, статус онлайн)
            "&lang=ru" +                            // язык ответа
            "&name_case=nom" +                      // падеж ответ
            $"&access_token={_token}" +             // токен доступа
            "&v=5.199";                             // версия API

        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        var responses = ParseAndGetResponse(request.downloadHandler.text);
        print(responses.response[0]);
        action?.Invoke(responses);
    }

    private VkUserResponse ParseAndGetResponse(string requestResponse)
    {
        return JsonUtility.FromJson<VkUserResponse>(requestResponse);
    }
}