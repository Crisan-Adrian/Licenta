using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Create Walk Step")]
[Serializable]
public class AnimationStep : ScriptableObject
{
    public float framesPerStep = 30f;
    public Vector3 lowerBodyRotation;
    public Vector3 upperLeftLegRotation;
    public Vector3 lowerLeftLegRotation;
    public Vector3 upperRightLegRotation;
    public Vector3 lowerRightLegRotation;
    public Vector3 upperBodyRotation;
    public Vector3 upperLeftArmRotation;
    public Vector3 lowerLeftArmRotation;
    public Vector3 upperRightArmRotation;
    public Vector3 lowerRightArmRotation;
    public Vector3 headRotation;
}
