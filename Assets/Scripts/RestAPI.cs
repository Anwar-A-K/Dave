using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QuickType;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class RestAPI : MonoBehaviour
{

    [SerializeField] private string url = "https://test.iamdave.ai/conversation/exhibit_aldo/74710c52-42a5-3e65-b1f0-2dc39ebe42c2" ;
    [SerializeField] private string enterpriseId = "dave_expo";
    [SerializeField] private string userId = "74710c52-42a5-3e65-b1f0-2dc39ebe42c2";
    [SerializeField] private string apiKey = "NzQ3MTBjNTItNDJhNS0zZTY1LWIxZjAtMmRjMzllYmU0MmMyMTYwNzIyMDY2NiAzNw__";

    [SerializeField] private TextMeshProUGUI textResponse;

    [SerializeField] private AudioSource audioSource;
    
    

    public void CallApiButton(string customerState)
    {
        StartCoroutine(CallApi(customerState));

    }
    IEnumerator CallApi(string customerState)
    {
        ApiRequestBody apiRequestBody = new ApiRequestBody
        {
            system_response = "sr_init",
            engagement_id = "NzQ3MTBjNTItNDJhNS0zZTY1LWIxZjAtMmRjMzllYmU0MmMyZXhoaWJpdF9hbGRv",
            customer_state = customerState
        };

        string apiBodyString = JsonUtility.ToJson(apiRequestBody);
        
        Debug.Log(apiBodyString);
        
        
        var apiRequest = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(apiBodyString);
        apiRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        apiRequest.downloadHandler = new DownloadHandlerBuffer();

        apiRequest.SetRequestHeader("Content-Type", "application/json");
        apiRequest.SetRequestHeader("X-I2CE-ENTERPRISE-ID", enterpriseId);
        apiRequest.SetRequestHeader("X-I2CE-USER-ID", userId);
        apiRequest.SetRequestHeader("X-I2CE-API-KEY", apiKey);

        yield return apiRequest.SendWebRequest();

        if (apiRequest.isNetworkError || apiRequest.isHttpError)
        {
            Debug.LogError(apiRequest.error);
            if (apiRequest.error == "HTTP/1.1 401 Unauthorized")
            {
                Debug.Log(apiRequest.error);
                //RefreshToken.instance.GetRefreshToken(GetSimilarProductsInDetail());

            }

            yield break;
        }
        
        Debug.Log(apiRequest.downloadHandler.text);

        ApiResponse response = ApiResponse.FromJson(apiRequest.downloadHandler.text);

        textResponse.text = response.Placeholder;

        StartCoroutine(PlayAudio(response.ResponseChannels.Voice.AbsoluteUri));
    }


    IEnumerator PlayAudio(string audioUrl)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.WAV))
        {
            yield return www.Send();

            if (www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var myClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = myClip;
                audioSource.Play();
            }
        }
    }
}
