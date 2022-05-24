using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public static class AnimationSerializer
    {
        public static string ToJSON(Animation animation)
        {
            List<string> stepsJSON = new List<string>();

            foreach (AnimationStep step in animation.animationSteps)
            {
                string stepJSON = ToJSON(step);
                stepsJSON.Add(stepJSON);
            }

            string steps = String.Join(",\n", stepsJSON);

            return String.Format("{{\"animationName\":\"{0}\",\n\"steps\":[\n{1}]\n}}", animation.name, steps);
        }

        public static Animation AnimationFromJSON(string animationJSON)
        {
            return null;
        }

        public static string ToJSON(AnimationStep step)
        {
            List<string> partsJSON = new List<string>();

            partsJSON.Add(String.Format("\"framesPerStep\":{0}",
                step.framesPerStep));
            
            partsJSON.Add(String.Format("\"lowerBodyRotation\":{0}",
                EditorJsonUtility.ToJson(step.lowerBodyRotation)));
            
            partsJSON.Add(String.Format("\"upperLeftLegRotation\":{0}",
                EditorJsonUtility.ToJson(step.upperLeftLegRotation)));
            
            partsJSON.Add(String.Format("\"lowerLeftLegRotation\":{0}",
                EditorJsonUtility.ToJson(step.lowerLeftLegRotation)));
            
            partsJSON.Add(String.Format("\"upperRightLegRotation\":{0}",
                EditorJsonUtility.ToJson(step.upperRightLegRotation)));
            
            partsJSON.Add(String.Format("\"lowerRightLegRotation\":{0}",
                EditorJsonUtility.ToJson(step.lowerRightLegRotation)));
            
            partsJSON.Add(String.Format("\"upperBodyRotation\":{0}",
                EditorJsonUtility.ToJson(step.upperBodyRotation)));
            
            partsJSON.Add(String.Format("\"upperLeftArmRotation\":{0}",
                EditorJsonUtility.ToJson(step.upperLeftArmRotation)));
            
            partsJSON.Add(String.Format("\"lowerLeftArmRotation\":{0}",
                EditorJsonUtility.ToJson(step.lowerLeftArmRotation)));
            
            partsJSON.Add(String.Format("\"upperRightArmRotation\":{0}",
                EditorJsonUtility.ToJson(step.upperRightArmRotation)));
            
            partsJSON.Add(String.Format("\"lowerRightArmRotation\":{0}",
                EditorJsonUtility.ToJson(step.lowerRightArmRotation)));
            
            partsJSON.Add(String.Format("\"headRotation\":{0}",
                EditorJsonUtility.ToJson(step.headRotation)));

            string parts = String.Join(",\n", partsJSON);

            return String.Format("{{\n{0}\n}}", parts);
        }


        public static AnimationStep AnimationStepFromJSON(string stemJSON)
        {
            return null;
        }
    }
}