using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using Unity.EditorCoroutines.Editor;

public class ServerManager
{
    //TODO clean-up code
    private Process _serverProcess;
    private int _port = 5001;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _serverProcess = new Process();
            _serverProcess.EnableRaisingEvents = false;
            _serverProcess.StartInfo.FileName = "D:/Facultate/Licenta/Proiect_Practic/Proiect_AI/startFlaskApp.bat";
            _serverProcess.StartInfo.Arguments = _port.ToString();
            _serverProcess.StartInfo.UseShellExecute = false;
            _serverProcess.StartInfo.CreateNoWindow = true;
            _serverProcess.StartInfo.RedirectStandardOutput = false;
            _serverProcess.StartInfo.RedirectStandardInput = false;
            _serverProcess.StartInfo.RedirectStandardError = false;
            _serverProcess.Start();
       
            UnityEngine.Debug.Log( "Successfully launched app" );
        }
        catch( Exception e )
        {
            Debug.LogError( "Unable to launch app: " + e.Message );
        }
    }

    public bool CheckServerStatus()
    {
        //TODO Implement
        return true; 
    }

    public void RestartSerer()
    {
        //TODO Implement
    }
    
    // private void KillProcess()
    // {
    //     Process killProcess = new Process();
    //     killProcess.EnableRaisingEvents = false;
    //     killProcess.StartInfo.FileName = "cmd.exe";
    //     killProcess.StartInfo.Arguments = "/c taskkill /F /IM flask.exe";
    //     killProcess.StartInfo.UseShellExecute = false;
    //     killProcess.StartInfo.CreateNoWindow = true;
    //     killProcess.StartInfo.RedirectStandardOutput = false;
    //     killProcess.StartInfo.RedirectStandardInput = false;
    //     killProcess.StartInfo.RedirectStandardError = false;
    //     killProcess.Start();
    //     killProcess.WaitForExit();
    //
    //     Debug.Log(string.Format("Killer process ended: {0}", killProcess.HasExited));
    // }

    private void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        if (_serverProcess != null && !_serverProcess.HasExited)
        {
            try
            {
                // StartCoroutine(SendKillRequest());
                _serverProcess.Kill();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        else
        {
            Debug.Log("Process is already closed");
        }
    }

    private IEnumerator SendKillRequest()
    {
        //TODO move request to NetworkService
        String url = String.Format("http://127.0.0.1:{0}/shutdown", _port); 
        UnityWebRequest getRequest = UnityWebRequest.Get(url);

        yield return getRequest.SendWebRequest();
 
        if(getRequest.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log(getRequest.error);
        }
        else {
            // Show results as text
            Debug.Log(getRequest.downloadHandler.text);
        }
    }
}