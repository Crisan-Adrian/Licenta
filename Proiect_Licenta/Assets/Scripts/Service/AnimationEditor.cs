using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class AnimationEditor
{
    private AnimationStep _defaultPose;
    private Animation _animation;
    private string assetsPath = "Assets/Animations";
    private string _animationName;

    private int _index;
    private List<string> _keys = new();
    private Dictionary<string, GameObject> _currentModelBodyParts = new();
    private Dictionary<string, GameObject> _pastModelBodyParts = new();

    public AnimationEditor()
    {
    }

    public Animation CreateNewAnimation(string name)
    {
        AssetDatabase.SaveAssets();
        _animation = ScriptableObject.CreateInstance<Animation>();
        _animation.animationSteps = new List<AnimationStep>();
        _animationName = name;
        string result = AssetDatabase.CreateFolder(assetsPath, _animationName);
        Debug.Log(result);
        AssetDatabase.SaveAssets();
        AssetDatabase.CreateAsset(_animation, assetsPath + String.Format("/{0}/{0}.asset", _animationName));
        AssetDatabase.SaveAssets();
        AnimationStep animationStep = ScriptableObject.CreateInstance<AnimationStep>();
        AssetDatabase.CreateAsset(animationStep,
                assetsPath + String.Format("/{0}/{0}_Step_{1}.asset", _animationName, 1));
            _animation.animationSteps.Add(animationStep);
        AssetDatabase.SaveAssets();

        return _animation;
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
        _animationName = animation.name;
        Debug.Log(_animationName);
    }

    public void AddNewAnimationStep()
    {
        AnimationStep newStep = ScriptableObject.CreateInstance<AnimationStep>();
        string assetName = String.Format("/{0}/{0}_Step_{1}.asset", _animationName, _animation.animationSteps.Count);
        _animation.animationSteps.Add(newStep);

        AssetDatabase.CreateAsset(newStep, assetsPath + assetName);
        AssetDatabase.SaveAssets();
    }

    public Animation GetCurrentAnimation()
    {
        return _animation;
    }

    public Animation ImportAnimation(AnimationDTO animationDto)
    {
        AssetDatabase.SaveAssets();
        _animation = ScriptableObject.CreateInstance<Animation>();
        _animation.animationSteps = new List<AnimationStep>();
        _animationName = animationDto.animationName;
        Debug.Log(_animationName);
        string result = AssetDatabase.CreateFolder(assetsPath, _animationName);
        Debug.Log(result);
        AssetDatabase.SaveAssets();
        AssetDatabase.CreateAsset(_animation, assetsPath + String.Format("/{0}/{0}.asset", _animationName));
        AssetDatabase.SaveAssets();
        int i = 1;
        foreach (AnimationStepDTO stepDto in animationDto.steps)
        {
        AnimationStep animationStep = ScriptableObject.CreateInstance<AnimationStep>();
        TransferData(animationStep, stepDto);
        AssetDatabase.CreateAsset(animationStep,
            assetsPath + String.Format("/{0}/{0}_Step_{1}.asset", _animationName, i));
        _animation.animationSteps.Add(animationStep);
        AssetDatabase.SaveAssets();
        i++;
        }

        return _animation;
    }

    private void TransferData(AnimationStep animationStep, AnimationStepDTO stepDto)
    {
        animationStep.framesPerStep = stepDto.framesPerStep;
        animationStep.lowerBodyRotation = stepDto.lowerBodyRotation;
        animationStep.upperLeftLegRotation = stepDto.upperLeftLegRotation;
        animationStep.lowerLeftLegRotation = stepDto.lowerLeftLegRotation;
        animationStep.upperRightLegRotation = stepDto.upperRightLegRotation;
        animationStep.lowerRightLegRotation = stepDto.lowerRightLegRotation;
        animationStep.upperBodyRotation = stepDto.upperBodyRotation;
        animationStep.upperLeftArmRotation = stepDto.upperLeftArmRotation;
        animationStep.lowerLeftArmRotation = stepDto.lowerLeftArmRotation;
        animationStep.upperRightArmRotation = stepDto.upperRightArmRotation;
        animationStep.lowerRightArmRotation = stepDto.lowerRightArmRotation;
        animationStep.headRotation = stepDto.headRotation;
    }
}