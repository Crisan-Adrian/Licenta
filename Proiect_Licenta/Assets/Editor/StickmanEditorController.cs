using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using UnityEditor;

public class StickmanEditorController
{
    private Dictionary<string, GameObject> _currentModelBodyParts;
    private Dictionary<string, GameObject> _pastModelBodyParts;

    public StickmanEditorController()
    {
        _currentModelBodyParts = new();
        _pastModelBodyParts = new();
    }

    public void SetModels(GameObject currentModel, GameObject pastModel)
    {
        SetBodyParts(_currentModelBodyParts, currentModel);
        SetBodyParts(_pastModelBodyParts, pastModel);
    }
    
    private void SetBodyParts(Dictionary<string, GameObject> modelBodyParts, GameObject target)
    {
        modelBodyParts["body_lower"] = GameObjectUtilities.FindChildWithName(target, "Body_Lower");
        modelBodyParts["body_upper"] = GameObjectUtilities.FindChildWithName(target, "Body_Upper");
        modelBodyParts["left_leg_lower"] = GameObjectUtilities.FindChildWithName(target, "Left_Leg_Lower");
        modelBodyParts["left_leg_upper"] = GameObjectUtilities.FindChildWithName(target, "Left_Leg_Upper");
        modelBodyParts["right_leg_lower"] = GameObjectUtilities.FindChildWithName(target, "Right_Leg_Lower");
        modelBodyParts["right_leg_upper"] = GameObjectUtilities.FindChildWithName(target, "Right_Leg_Upper");
        modelBodyParts["left_arm_lower"] = GameObjectUtilities.FindChildWithName(target, "Left_Arm_Lower");
        modelBodyParts["left_arm_upper"] = GameObjectUtilities.FindChildWithName(target, "Left_Arm_Upper");
        modelBodyParts["right_arm_lower"] = GameObjectUtilities.FindChildWithName(target, "Right_Arm_Lower");
        modelBodyParts["right_arm_upper"] = GameObjectUtilities.FindChildWithName(target, "Right_Arm_Upper");
        modelBodyParts["head"] = GameObjectUtilities.FindChildWithName(target, "Head");
    }

    public void SetPastModelPose(AnimationStep animationStep)
    {
        SetPose(animationStep, _pastModelBodyParts);
    }

    public void SetCurrentModelPose(AnimationStep animationStep)
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