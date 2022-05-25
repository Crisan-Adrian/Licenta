using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewAnimationWindow : EditorWindow
{
    private string _animationName = "";
    
    public static string Open()
    {
        NewAnimationWindow window = CreateInstance<NewAnimationWindow>();
        window.ShowModal();
        return window._animationName;
    }

    public void OnGUI()
    {
        EditorGUILayout.Space();
        _animationName = EditorGUILayout.TextField("Animation Name:", _animationName);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Submit"))
        {
            Close();
        }
    }
}
