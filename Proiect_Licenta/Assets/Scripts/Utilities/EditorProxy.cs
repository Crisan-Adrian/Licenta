using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class EditorProxy
{
    private static string path = Application.dataPath + "\\Editor\\EditorState\\proxy.json";

    private static EditorProxyState _state = new();

    public static void SetObservationsPath(string observationsPath)
    {
        _state.observationsPath = observationsPath;
        WriteToFile();
    }

    public static void SetAnimation(Animation animation)
    {
        _state.animation = animation;
        WriteToFile();
    }

    public static string GetObservationsPath()
    {
        ReadFromFile();
        return _state.observationsPath;
    }

    public static Animation GetAnimation()
    {
        ReadFromFile();
        return _state.animation;
    }

    private static void WriteToFile()
    {
        string serializedState = JsonUtility.ToJson(_state);
        File.WriteAllText(path, serializedState);
    }
    
    private static void ReadFromFile()
    {
        string serializedState = File.ReadAllText(path);
        _state = JsonUtility.FromJson<EditorProxyState>(serializedState);
    }
}

class EditorProxyState
{
    public string observationsPath = "";
    public Animation animation = null;
}
