using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZBoom.Common.SpatialMap;

public class TextureLoader
{
    public IEnumerator LoadTexture(Image image, string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
            image.sprite = sprite;
        }
    }
    
    public IEnumerator LoadTexture(string url, IResultListener<Sprite> resultListener)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            resultListener.OnError(RaribleApiController.MESSAGE_ERROR);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
            //image.sprite = sprite;
            resultListener.OnSuccess(sprite, RaribleApiController.MESSAGE_SUCCESS);
        }
    }
}
