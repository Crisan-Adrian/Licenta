using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class StickmanController : MonoBehaviour
{
    //TODO clean-up code
    [SerializeField] private bool loop = true;
    [SerializeField] private AnimationStep initialPose;
    [SerializeField] private Animation Animation { get; set; }
    [SerializeField] private float frameModifier = 1f;
    [SerializeField] private float epsilon = .5f;
    [SerializeField] private float overshootEpsilon = .0001f;
    private int _index;
    private int _frames;
    private bool _reachedEnd;

    public delegate void OnFinishDelegate();

    private OnFinishDelegate _onFinishDelegate;

    private Dictionary<string, GameObject> _bodyParts = new();
    private Dictionary<string, Vector3> _animationStepComponents = new();
    private Dictionary<string, Vector3> _rotationsPerFrame = new();
    private List<string> keys = new();
    private Dictionary<string, Vector3> _bodyEulerAngles = new();

    
    
    // Start is called before the first frame update
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        
        Setup();
        
        _frames = 0;
        _reachedEnd = false;
    }
    
    private void Setup()
    {
        _index = -1;
        
        SetKeys();

        SetBodyParts();
        
        SetAnimation();

        SetInitialRotations();
        
        UpdateWalkStepComponents();
    }

    private void SetKeys()
    {
        keys.Add("body_lower");
        keys.Add("body_upper");
        keys.Add("left_leg_upper");
        keys.Add("left_leg_lower");
        keys.Add("right_leg_upper");
        keys.Add("right_leg_lower");
        keys.Add("left_arm_upper");
        keys.Add("left_arm_lower");
        keys.Add("right_arm_upper");
        keys.Add("right_arm_lower");
        keys.Add("head");
        Debug.Log("SetKeys");
    }

    public void AddDelegate(OnFinishDelegate finishDelegate)
    {
        _onFinishDelegate += finishDelegate;
    }
    
    public void RemoveDelegate(OnFinishDelegate finishDelegate)
    {
        _onFinishDelegate -= finishDelegate;
    }

    private void SetBodyParts()
    {
        _bodyParts["body_lower"] = GameObjectUtilities.FindChildWithName(gameObject,"Body_Lower");
        _bodyParts["body_upper"] = GameObjectUtilities.FindChildWithName(gameObject,"Body_Upper");
        _bodyParts["left_leg_lower"] = GameObjectUtilities.FindChildWithName(gameObject,"Left_Leg_Lower");
        _bodyParts["left_leg_upper"] = GameObjectUtilities.FindChildWithName(gameObject,"Left_Leg_Upper");
        _bodyParts["right_leg_lower"] = GameObjectUtilities.FindChildWithName(gameObject,"Right_Leg_Lower");
        _bodyParts["right_leg_upper"] = GameObjectUtilities.FindChildWithName(gameObject,"Right_Leg_Upper");
        _bodyParts["left_arm_lower"] = GameObjectUtilities.FindChildWithName(gameObject,"Left_Arm_Lower");
        _bodyParts["left_arm_upper"] = GameObjectUtilities.FindChildWithName(gameObject,"Left_Arm_Upper");
        _bodyParts["right_arm_lower"] = GameObjectUtilities.FindChildWithName(gameObject,"Right_Arm_Lower");
        _bodyParts["right_arm_upper"] = GameObjectUtilities.FindChildWithName(gameObject,"Right_Arm_Upper");
        _bodyParts["head"] = GameObjectUtilities.FindChildWithName(gameObject,"Head");
        Debug.Log("SetBodyParts");
    }

    void FixedUpdate()
    {
        // {Debug.Log(String.Format("Current index {0}", _index));}
        if (AnimationStepCompleted())
        {
            UpdateWalkStepComponents();
        }

        _frames++;
        DoWalkStep();
    }

    public Dictionary<string, Vector3> GetBodyRotation()
    {
        Dictionary<string, Vector3> bodyRotation = new Dictionary<string, Vector3>();

        foreach (string key in keys)
        {
            Vector3 eulerAngles = _bodyEulerAngles[_bodyParts[key].name];
            bodyRotation.Add(key, eulerAngles);
        }

        return bodyRotation;
    }

    private void DoWalkStep()
    {
        foreach (string key in keys)
        {
            StepRotation(_bodyParts[key], _rotationsPerFrame[key], _animationStepComponents[key]);
        }
    }

    private void StepRotation(GameObject bodyPart, Vector3 rotationPerFrame, Vector3 targetRotation)
    {
        Quaternion rotation = bodyPart.transform.localRotation;
        Vector3 eulerAngles = _bodyEulerAngles[bodyPart.name];
        Vector3 adjustedTargetRotation = AdjustRotationVector(targetRotation);
        eulerAngles = AdjustRotationVector(eulerAngles);

        if (ApproximatelyEqual(eulerAngles, adjustedTargetRotation, overshootEpsilon))
        {
            eulerAngles = adjustedTargetRotation;
            rotation.eulerAngles = eulerAngles;
            _bodyEulerAngles[bodyPart.name] = eulerAngles;
            bodyPart.transform.localRotation = rotation;
            return;
        }

        eulerAngles = Rotate(eulerAngles, adjustedTargetRotation, rotationPerFrame);
        eulerAngles = AdjustRotationVector(eulerAngles);

        rotation = Quaternion.Euler(eulerAngles);
        _bodyEulerAngles[bodyPart.name] = eulerAngles;
        bodyPart.transform.localRotation = rotation;
    }

    private Vector3 Rotate(Vector3 eulerAngles, Vector3 targetRotation, Vector3 rotationPerFrame)
    {
        Vector3 rotatedEulerAngles = Vector3.zero;
        if (Mathf.Abs(eulerAngles.x - targetRotation.x) <= Mathf.Abs(rotationPerFrame.x))
        {
            rotatedEulerAngles.x = targetRotation.x;
        }
        else
        {
            rotatedEulerAngles.x = eulerAngles.x + rotationPerFrame.x;
        }

        if (Mathf.Abs(eulerAngles.y - targetRotation.y) <= Mathf.Abs(rotationPerFrame.y))
        {
            rotatedEulerAngles.y = targetRotation.y;
        }
        else
        {
            rotatedEulerAngles.y = eulerAngles.y + rotationPerFrame.y;
        }

        if (Mathf.Abs(eulerAngles.z - targetRotation.z) <= Mathf.Abs(rotationPerFrame.z))
        {
            rotatedEulerAngles.z = targetRotation.z;
        }
        else
        {
            rotatedEulerAngles.z = eulerAngles.z + rotationPerFrame.z;
        }

        return rotatedEulerAngles;
    }

    private Vector3 TransformToUnitVector(Vector3 rotationVector)
    {
        rotationVector.x = TransformToUnitValue(rotationVector.x);
        rotationVector.y = TransformToUnitValue(rotationVector.y);
        rotationVector.z = TransformToUnitValue(rotationVector.z);

        return rotationVector;
    }

    private float TransformToUnitValue(float value)
    {
        float sign = 0;
        if (value < 0)
        {
            sign = -1;
        }
        else
        {
            sign = 1;
        }

        return sign;
    }

    private static Vector3 AdjustRotationVector(Vector3 rotationValues)
    {
        //TODO: solve knee issue
        Vector3 adjustedRotationVector = Vector3.zero;
        if (rotationValues.x < -0.1)
        {
            adjustedRotationVector.x = 360 + rotationValues.x;
        }
        else if (rotationValues.x > 360)
        {
            adjustedRotationVector.x = rotationValues.x - 360;
        }
        else
        {
            adjustedRotationVector.x = rotationValues.x;
        }

        if (rotationValues.y < -0.1)
        {
            adjustedRotationVector.y = 360 + rotationValues.y;
        }
        else if (rotationValues.y > 360)
        {
            adjustedRotationVector.y = rotationValues.y - 360;
        }
        else
        {
            adjustedRotationVector.y = rotationValues.y;
        }

        if (rotationValues.z < -0.1)
        {
            adjustedRotationVector.z = 360 + rotationValues.z;
        }
        else if (rotationValues.z > 360)
        {
            adjustedRotationVector.z = rotationValues.z - 360;
        }
        else
        {
            adjustedRotationVector.z = rotationValues.z;
        }

        return adjustedRotationVector;
    }

    private Vector3 GetRotationStepTarget(Vector3 targetRotation, Vector3 currentRotation)
    {
        return new Vector3(
            GetRotationStepTargetAxis(targetRotation.x, currentRotation.x),
            GetRotationStepTargetAxis(targetRotation.y, currentRotation.y),
            GetRotationStepTargetAxis(targetRotation.z, currentRotation.z));
    }

    private float GetRotationStepTargetAxis(float targetRotation, float currentRotation)
    {
        float rotationTarget = 0;

        targetRotation = ResolveDoublePoint(targetRotation, currentRotation);

        if (currentRotation - targetRotation > 180)
        {
            rotationTarget = currentRotation - targetRotation + 360;
        }
        else if (currentRotation - targetRotation < -180)
        {
            rotationTarget = -currentRotation + targetRotation - 360;
        }
        else if (targetRotation > currentRotation)
        {
            rotationTarget = targetRotation - currentRotation;
        }
        else
        {
            rotationTarget = targetRotation - currentRotation;
        }

        return rotationTarget;
    }

    private float ResolveDoublePoint(float targetRotation, float currentRotation)
    {
        if (currentRotation > 180 && Mathf.Abs(targetRotation) < epsilon)
        {
            targetRotation += 360;
        }
        else if (currentRotation <= 180 && Mathf.Abs(targetRotation - 360) < epsilon)
        {
            targetRotation -= 360;
        }

        return targetRotation;
    }

    private bool ApproximatelyEqual(Vector3 eulerAngles, Vector3 walkStepComponent, float epsilon)
    {
        if (Math.Abs(eulerAngles.x - walkStepComponent.x) > epsilon)
        {
            return false;
        }

        if (Math.Abs(eulerAngles.y - walkStepComponent.y) > epsilon)
        {
            return false;
        }

        if (Math.Abs(eulerAngles.z - walkStepComponent.z) > epsilon)
        {
            return false;
        }

        return true;
    }

    private void SetInitialRotations()
    {
        SetInitialRotation(_bodyParts["body_lower"], initialPose.lowerBodyRotation);
        SetInitialRotation(_bodyParts["body_upper"], initialPose.upperBodyRotation);
        SetInitialRotation(_bodyParts["left_leg_lower"], initialPose.lowerLeftLegRotation);
        SetInitialRotation(_bodyParts["left_leg_upper"], initialPose.upperLeftLegRotation);
        SetInitialRotation(_bodyParts["right_leg_lower"], initialPose.lowerRightLegRotation);
        SetInitialRotation(_bodyParts["right_leg_upper"], initialPose.upperRightLegRotation);
        SetInitialRotation(_bodyParts["left_arm_lower"], initialPose.lowerLeftArmRotation);
        SetInitialRotation(_bodyParts["left_arm_upper"], initialPose.upperLeftArmRotation);
        SetInitialRotation(_bodyParts["right_arm_lower"], initialPose.lowerRightArmRotation);
        SetInitialRotation(_bodyParts["right_arm_upper"], initialPose.upperRightArmRotation);
        SetInitialRotation(_bodyParts["head"], initialPose.headRotation);
    }

    private void SetInitialRotation(GameObject bodyPart, Vector3 initialPoseBodyRotation)
    {
        Quaternion quaternion = Quaternion.Euler(initialPoseBodyRotation);
        bodyPart.transform.localRotation = quaternion;
        _bodyEulerAngles.Add(bodyPart.name, initialPoseBodyRotation);
    }


    private void UpdateWalkStepComponents()
    {
        _index++;
        if (_index == GetComponent<Animation>().animationSteps.Count)
        {
            if (loop == true)
            {
                if (!_reachedEnd && _onFinishDelegate != null)
                {
                    _reachedEnd = true;
                    _onFinishDelegate();
                }
                _index = 0;
            }
            else
            {
                _index--;
            }
            _frames = 0;
        }
        
        AnimationStep animationStep = GetComponent<Animation>().animationSteps[_index];

        _animationStepComponents["body_lower"] = animationStep.lowerBodyRotation;
        _animationStepComponents["body_upper"] = animationStep.upperBodyRotation;
        _animationStepComponents["left_leg_lower"] = animationStep.lowerLeftLegRotation;
        _animationStepComponents["left_leg_upper"] = animationStep.upperLeftLegRotation;
        _animationStepComponents["right_leg_lower"] = animationStep.lowerRightLegRotation;
        _animationStepComponents["right_leg_upper"] = animationStep.upperRightLegRotation;
        _animationStepComponents["left_arm_lower"] = animationStep.lowerLeftArmRotation;
        _animationStepComponents["left_arm_upper"] = animationStep.upperLeftArmRotation;
        _animationStepComponents["right_arm_lower"] = animationStep.lowerRightArmRotation;
        _animationStepComponents["right_arm_upper"] = animationStep.upperRightArmRotation;
        _animationStepComponents["head"] = animationStep.headRotation;


        foreach (string key in keys)
        {
            Vector3 adjustedRotationVector = AdjustRotationVector(_animationStepComponents[key]);
            Vector3 adjustedEulerAngles = AdjustRotationVector(_bodyEulerAngles[_bodyParts[key].name]);
            Vector3 rotationVector = GetRotationStepTarget(adjustedRotationVector, adjustedEulerAngles);
            rotationVector /= animationStep.framesPerStep * (1 / frameModifier);
            // Debug.Log("Rotation " + rotationVector);
            _rotationsPerFrame[key] = rotationVector;
        }
    }

    private bool AnimationStepCompleted()
    {
        bool walkStepComplete = true;

        foreach (string key in keys)
        {
            string name = _bodyParts[key].name;

            Vector3 bodyEulerAngle = _bodyEulerAngles[name];
            
            if (!ApproximatelyEqual(AdjustRotationVector(bodyEulerAngle),
                AdjustRotationVector(_animationStepComponents[key]),
                overshootEpsilon))
            {
                walkStepComplete = false;
                // Debug.Log(String.Format("Rotation vectors not equal {0}, {1})",
                //     _bodyParts[key].transform.localEulerAngles, AdjustRotationVector(_animationStepComponents[key])));
                break;
            }
        }

        // if (walkStepComplete == true)
        //     Debug.Log(String.Format("Walk step {0} was completed", _index + 1));

        return walkStepComplete;
    }

    public void SetAnimation()
    {
        if (GetComponent<Animation>() == null)
        {
            EditorProxy editorProxy = EditorProxy.GetInstance();
            Animation = editorProxy.Animation;
        }
        if (GetComponent<Animation>().animationSteps.Count > 0)
        {
            initialPose = GetComponent<Animation>().animationSteps[0];
        }
    }
}