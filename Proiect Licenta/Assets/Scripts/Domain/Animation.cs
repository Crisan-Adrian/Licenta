using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName="Create Animation")]
[Serializable]
public class Animation : ScriptableObject
{
    [FormerlySerializedAs("walkSteps")] public List<AnimationStep> animationSteps;
}
