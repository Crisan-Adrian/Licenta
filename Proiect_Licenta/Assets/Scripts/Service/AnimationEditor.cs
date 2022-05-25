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
    private string assetsPath = "Assets/Animations";
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
        string assetName = String.Format("/{0}_Step_{1}.asset", animationName, _animation.animationSteps.Count);
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
        animationName = animationDto.animationName;
        Debug.Log(animationName);
        string result = AssetDatabase.CreateFolder(assetsPath, animationName);
        Debug.Log(result);
        AssetDatabase.SaveAssets();
        AssetDatabase.CreateAsset(_animation, assetsPath + String.Format("/{0}/{0}.asset", animationName));
        AssetDatabase.SaveAssets();
        int i = 1;
        foreach (AnimationStepDTO stepDto in animationDto.steps)
        {
        AnimationStep animationStep = ScriptableObject.CreateInstance<AnimationStep>();
        TransferData(animationStep, stepDto);
        AssetDatabase.CreateAsset(animationStep,
            assetsPath + String.Format("/{0}/{0}_Step_{1}.asset", animationName, i));
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