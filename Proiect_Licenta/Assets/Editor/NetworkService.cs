using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NetworkDTO;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;

public class NetworkService
{
    private static HttpClient _client = new();
    private bool _isRunning;
    private static NetworkService _instance;
    private string uri = "http://127.0.0.1:";
    private ServerConfig _config;
    private Process _serverProcess;
    private static string configPath = Application.dataPath + "\\Editor\\serverConfig.json";

    private NetworkService()
    {
        _isRunning = false;

        string serializedState = File.ReadAllText(configPath);
        _config = JsonUtility.FromJson<ServerConfig>(serializedState);
        uri = String.Join("", uri, _config.port.ToString());
    }

    public async Task<ModelList> GetModels()
    {
        UnityWebRequest getRequest = UnityWebRequest.Get(String.Format("{0}/models", uri));

        getRequest.SetRequestHeader("Content-Type", "application/json");

        var operation = getRequest.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (getRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            return null;
        }
        else
        {
            string jsonResponse = getRequest.downloadHandler.text;
            ModelList modelList = JsonUtility.FromJson<ModelList>(jsonResponse);
            return modelList;
        }
    }

    public static NetworkService GetInstance()
    {
        if (_instance == null)
        {
            _instance = new NetworkService();
        }

        return _instance;
    }

    public async void StartServer()
    {
        await CheckServerStatus();
        if (!_isRunning)
        {
            try
            {
                _serverProcess = new Process();
                _serverProcess.EnableRaisingEvents = false;
                _serverProcess.StartInfo.FileName = _config.scriptPath;
                _serverProcess.StartInfo.Arguments = _config.port.ToString();
                _serverProcess.StartInfo.UseShellExecute = false;
                _serverProcess.StartInfo.CreateNoWindow = true;
                _serverProcess.StartInfo.RedirectStandardOutput = false;
                _serverProcess.StartInfo.RedirectStandardInput = false;
                _serverProcess.StartInfo.RedirectStandardError = false;
                _serverProcess.Start();
            }
            catch (Exception e)
            {
                Debug.LogError("Unable to launch server: " + e.Message);
            }
        }
    }

    public async Task<bool> CheckServerStatus()
    {
        UnityWebRequest getRequest = UnityWebRequest.Head(String.Format("{0}/status", uri));

        var operation = getRequest.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (getRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            _isRunning = false;
        }
        else
        {
            _isRunning = true;
        }

        return _isRunning;
    }

    public void SendKillRequest()
    {
        String url = String.Format(String.Format("{0}/shutdown", uri));
        UnityWebRequest getRequest = UnityWebRequest.Get(url);

        getRequest.SendWebRequest();

        if (getRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(getRequest.error);
        }
    }

    public void PostRequest(RequestDTO requestDTO)
    {
        Debug.Log(requestDTO);
    }
}