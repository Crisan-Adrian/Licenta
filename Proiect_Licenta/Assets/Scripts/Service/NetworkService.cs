using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using Random = UnityEngine.Random;

public class NetworkService
{
    //TODO make into singleton
    private static HttpClient _client = new HttpClient();

    public async void RunTask()
    {
        // EditorStartCoroutine(SendGetRequest());

        var msg = await _client.GetAsync("http://127.0.0.1:5000/models");
        Debug.Log(String.Format("Response {0}", msg));
    }

    IEnumerator SendGetRequest()
    {
        UnityWebRequest getRequest = UnityWebRequest.Get("http://127.0.0.1:5000/models");

        yield return getRequest.SendWebRequest();
 
        if(getRequest.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log(getRequest.error);
        }
        else {
            // Show results as text
            Debug.Log(getRequest.downloadHandler.text);
        }
    }

    public static bool CheckServerStatus()
    {
        int a = Random.Range(0, 2);
        if (a == 1)
        {
            return true;
        }

        return false;
    }
}
