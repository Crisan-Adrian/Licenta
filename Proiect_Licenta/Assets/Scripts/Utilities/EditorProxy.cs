using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorProxy
{
    private static EditorProxy _instance;

    public Animation Animation { get; set; }

    private EditorProxy()
    {
    }

    public static EditorProxy GetInstance()
    {
        if (_instance == null)
        {
            _instance = new EditorProxy();
        }

        return _instance;
    }
}
