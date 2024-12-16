using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileDrawer : MonoBehaviour
{
    [SerializeField] private VkAuth _vkAuth;
    [SerializeField] private TMP_Text _firstNameText;
    [SerializeField] private Image _avatar;

    private void Start()
    {
        _vkAuth.OnUserAuthorized += DrawUserInfo;
    }

    private void DrawUserInfo(VkUserResponse userResponse)
    {
        SetName(userResponse);
        SetImage(userResponse);
    }

    private void SetName(VkUserResponse vkResponse)
    {
        _firstNameText.text = vkResponse.response[0].first_name + " " + vkResponse.response[0].last_name;
    }  
    
    private void SetImage(VkUserResponse vkResponse)
    {
        StartCoroutine(ImageDownloader(vkResponse.response[0].photo_100));
    }

    private IEnumerator ImageDownloader(string imageUrl) 
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success) {

            DownloadHandlerTexture textureDownloadHandler = (DownloadHandlerTexture) request.downloadHandler;
            
            var texture = textureDownloadHandler.texture;
            var rect = new Rect(0, 0, texture.width, texture.width);
            var sprite = Sprite.Create(texture, rect, Vector2.one / 4);

            _avatar.sprite = sprite;
            
            // обработка ошибок...
        }
    }
}