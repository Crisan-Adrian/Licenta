using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class StickmanObserver : MonoBehaviour
{
    //TODO clean-up code
    [SerializeField] private GameObject observationTarget;
    private StickmanController _controller;
    private List<string> _observationData = new();
    [SerializeField] private List<string> observationFilter = new();
    private string _path;

    void Start()
    {
        _controller = observationTarget.GetComponent<StickmanController>();
        _controller.AddDelegate(FinishDelegate);
    }

    private void Awake()
    {
        _path = EditorProxy.GetObservationsPath();
    }

    public void FinishDelegate()
    {
        List<string> dataToSave = _observationData;
        _observationData = new List<string>();
        
        string json = "{\"frames\": [";
        for (int i = 0; i < dataToSave.Count; i++)
        {
            json += dataToSave[i];
            if (i + 1 < dataToSave.Count)
            {
                json += ",\n";
            }
        }

        json += "]\n}";
        File.WriteAllText(_path, json);
        // Debug.Log("Saved json data");
    }

    void FixedUpdate()
    {
        Dictionary<string, Vector3> rotations = _controller.GetBodyRotation();

        Dictionary<string, Vector3> filteredRotations = new Dictionary<string, Vector3>();
        if (observationFilter.Count == 0)
        {
            filteredRotations = rotations;
        }
        else
        {
            foreach (string key in rotations.Keys)
            {
                if (observationFilter.Contains(key))
                {
                    filteredRotations.Add(key, rotations[key]);
                }
            }
        }

        List<string> encoded = new List<string>();
        foreach (var keyValuePair in filteredRotations)
        {
            // Debug.Log(keyValuePair.Key + ": " + keyValuePair.Value.x);
            encoded.Add("\""+ keyValuePair.Key + "\"" + ":" + keyValuePair.Value.x);
        }

        string encodedString = "{";
        for (int i = 0; i < encoded.Count; i++)
        {
            encodedString += encoded[i];
            if (i + 1 < encoded.Count)
            {
                encodedString += ",";
            }
        }

        encodedString += "}";
        _observationData.Add(encodedString);
    }
}