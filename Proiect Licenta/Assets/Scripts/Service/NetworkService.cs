using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkService : MonoBehaviour
{
    //TODO make into singleton
    private static HttpClient _client = new HttpClient();

    public async void RunTask()
    {
        StartCoroutine(SendGetRequest());

        var msg = await _client.GetAsync("http://127.0.0.1:5000/models");
        Debug.Log(String.Format("Response {0}", msg));
    }

    IEnumerator SendGetRequest()
    {
        UnityWebRequest getRequest = UnityWebRequest.Get("http://127.0.0.1:5000/models");

        yield return getRequest.Send();
 
        if(getRequest.isNetworkError) {
            Debug.Log(getRequest.error);
        }
        else {
            // Show results as text
            Debug.Log(getRequest.downloadHandler.text);
        }
    }
}
