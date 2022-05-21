using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class AnimationEditor : MonoBehaviour
{
    [SerializeField] private AnimationStep defaultPose;
    [SerializeField] private GameObject currentPoseModel;
    [SerializeField] private GameObject pastPoseModel;
    [SerializeField] private Animation _animation;
    [SerializeField] private string assetsPath = "Assets/TestStep/"; 
    [SerializeField] private string animationName;
    
    private int _index;
    private List<string> keys = new List<string>();
    private Dictionary<string, GameObject> _currentModelBodyParts = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _pastModelBodyParts = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetBodyParts(_currentModelBodyParts, currentPoseModel);
        SetBodyParts(_pastModelBodyParts, pastPoseModel);
        
        _index = 0;
        if (_animation.animationSteps.Count == 0)
        {
            Debug.Log("Not enough steps");
            SetCurrentModelPose(defaultPose);
            SetPastModelPose(defaultPose);
        }
        if (_animation.animationSteps.Count == 1)
        {
            Debug.Log("Not enough steps");
            SetCurrentModelPose(_animation.animationSteps[_index]);
            SetPastModelPose(_animation.animationSteps[_index]);
        }
        else
        {
            SetCurrentModelPose(_animation.animationSteps[_index]);
            SetPastModelPose(_animation.animationSteps[_index + 1]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void SetBodyParts(Dictionary<string, GameObject> modelBodyParts, GameObject target)
    {
        modelBodyParts["body_lower"] = GameObjectUtilities.FindChildWithName(target,"Body_Lower");
        modelBodyParts["body_upper"] = GameObjectUtilities.FindChildWithName(target,"Body_Upper");
        modelBodyParts["left_leg_lower"] = GameObjectUtilities.FindChildWithName(target,"Left_Leg_Lower");
        modelBodyParts["left_leg_upper"] = GameObjectUtilities.FindChildWithName(target,"Left_Leg_Upper");
        modelBodyParts["right_leg_lower"] = GameObjectUtilities.FindChildWithName(target,"Right_Leg_Lower");
        modelBodyParts["right_leg_upper"] = GameObjectUtilities.FindChildWithName(target,"Right_Leg_Upper");
        modelBodyParts["left_arm_lower"] = GameObjectUtilities.FindChildWithName(target,"Left_Arm_Lower");
        modelBodyParts["left_arm_upper"] = GameObjectUtilities.FindChildWithName(target,"Left_Arm_Upper");
        modelBodyParts["right_arm_lower"] = GameObjectUtilities.FindChildWithName(target,"Right_Arm_Lower");
        modelBodyParts["right_arm_upper"] = GameObjectUtilities.FindChildWithName(target,"Right_Arm_Upper");
        modelBodyParts["head"] = GameObjectUtilities.FindChildWithName(target,"Head");
    }

    public void CreateNewAnimation()
    {
        
    }

    public void SaveAnimation()
    {
        
    }

    public void ExportAnimation()
    {
        
    }

    public void PreviewCurrentAnimation()
    {
        
    }

    public void AddNewAnimationStep()
    {
        AnimationStep previousStep = _animation.animationSteps[_animation.animationSteps.Count - 1];
        
        AnimationStep newStep = ScriptableObject.CreateInstance<AnimationStep>();
        string assetName = String.Format("{0}_{1}.asset", animationName, _animation.animationSteps.Count);
        AssetDatabase.CreateAsset(newStep, assetsPath+assetName);
        // TODO Save differently
        AssetDatabase.SaveAssets();
        
        _animation.animationSteps.Add(newStep);

        SetPastModelPose(previousStep);
        SetCurrentModelPose(newStep);
    }

    private void SetPastModelPose(AnimationStep animationStep)
    {
        SetPose(animationStep, _pastModelBodyParts);
    }

    private void SetCurrentModelPose(AnimationStep animationStep)
    {
        SetPose(animationStep, _currentModelBodyParts);
    }

    private void SetPose(AnimationStep animationStep, Dictionary<string, GameObject> model)
    {
        SetBodyPartRotation(model["body_lower"], animationStep.lowerBodyRotation);
        SetBodyPartRotation(model["body_upper"], animationStep.upperBodyRotation);
        SetBodyPartRotation(model["left_leg_lower"], animationStep.lowerLeftLegRotation);
        SetBodyPartRotation(model["left_leg_upper"], animationStep.upperLeftLegRotation);
        SetBodyPartRotation(model["right_leg_lower"], animationStep.lowerRightLegRotation);
        SetBodyPartRotation(model["right_leg_upper"], animationStep.upperRightLegRotation);
        SetBodyPartRotation(model["left_arm_lower"], animationStep.lowerLeftArmRotation);
        SetBodyPartRotation(model["left_arm_upper"], animationStep.upperLeftArmRotation);
        SetBodyPartRotation(model["right_arm_lower"], animationStep.lowerRightArmRotation);
        SetBodyPartRotation(model["right_arm_upper"], animationStep.upperRightArmRotation);
        SetBodyPartRotation(model["head"], animationStep.headRotation);
    }
    
    private void SetBodyPartRotation(GameObject bodyPart, Vector3 initialPoseBodyRotation)
    {
        Quaternion quaternion = Quaternion.Euler(initialPoseBodyRotation);
        bodyPart.transform.localRotation = quaternion;
    }
    
}
