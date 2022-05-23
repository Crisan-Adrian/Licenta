using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class AnimationEditor
{
    private AnimationStep defaultPose;
    private Animation _animation;
    private string assetsPath = "Assets/Animations/"; 
    private string animationName;
    
    private int _index;
    private List<string> keys = new List<string>();
    private Dictionary<string, GameObject> _currentModelBodyParts = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _pastModelBodyParts = new Dictionary<string, GameObject>();

    public AnimationEditor()
    {
        
    }

    public void CreateNewAnimation()
    {
        Debug.Log("New Animation Created");
    }

    public void SaveAnimation()
    {
        
    }

    public string ExportAnimation()
    {
        return "";
    }

    public void SetAnimation(Animation animation)
    {
        _animation = animation;
        animationName = animation.name;
        Debug.Log(animationName);
    }

    public void AddNewAnimationStep()
    {
        AnimationStep newStep = ScriptableObject.CreateInstance<AnimationStep>();
        string assetName = String.Format("{0}_Step_{1}.asset", animationName, _animation.animationSteps.Count);
        _animation.animationSteps.Add(newStep);
        
        AssetDatabase.CreateAsset(newStep, assetsPath+assetName);
        AssetDatabase.SaveAssets();
    }

    public Animation GetCurrentAnimation()
    {
        return _animation;
    }
}
