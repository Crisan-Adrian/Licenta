using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    [SerializeField] private string fileTrackerPath = "";

    [SerializeField] private string fileTrackerFile = "files.json";
    // Start is called before the first frame update
    void Start()
    {
        // How are files going to be tracked
        fileTrackerPath = Application.persistentDataPath;
        
        Debug.Log(fileTrackerPath+"/"+fileTrackerFile);
    }
    
}
