using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Utilities;

class AnimationEditorWindow : EditorWindow
{
    //TODO Try upgrade to Unity Version 2021.3 

    private string newAnimationButton = "Create New Animation";
    private string prevStepButton = "Prev Step";
    private string nextStepButton = "Next Step";
    private string addStepButton = "Add Step";
    private string exportAnimation = "Export Animation";
    private string previewAnimation = "Preview Animation";
    private string stopPreview = "Stop Preview";

    private int currentStep;
    private int stepsCount;

    private AnimationEditor animationEditor;
    private Animation animation;
    private GameObject animationModel;

    private GameObject previewModel;
    private GameObject currentModel;
    private GameObject pastModel;

    private List<string> keys = new List<string>();
    private Dictionary<string, GameObject> _currentModelBodyParts = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _pastModelBodyParts = new Dictionary<string, GameObject>();
    private bool editModeEnabled;
    private Vector2 scrollPos;
    private AnimationStep defaultPose;
    private int tab;
    private bool wasInPlayMode;

    [MenuItem("Window/Animation Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationEditorWindow));
    }

    public void Awake()
    {
        animationEditor = new AnimationEditor();
        editModeEnabled = true;
        wasInPlayMode = false;
    }

    private void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            editModeEnabled = true;
            if (animationModel != null)
            {
                SetActiveObjects();
            }
        }
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
        if (wasInPlayMode)
        {
            wasInPlayMode = false;
            Debug.Log("On Enable");
        }
    }

    private void OnDisable()
    {
        Debug.Log("On Disable");
    }

    void OnGUI()
    {
        tab = GUILayout.Toolbar(tab, new string[] {"Edit Animations", "Imitation "});
        switch (tab)
        {
            case 0:
                ShowAnimationsTab();
                break;
            case 1:
                break;
            default:
                break;
        }
    }

    private void ShowAnimationsTab()
    {
        EditorGUILayout.Space();
        ShowModelSelector();
        ShowDefaultPoseSelector();

        EditorGUILayout.Space();
        ShowAnimationSelector();

        EditorGUI.BeginDisabledGroup(animationModel == null);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(!editModeEnabled);
        if (GUILayout.Button(previewAnimation))
        {
            PreviewAnimation();
        }

        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(editModeEnabled);
        if (GUILayout.Button(stopPreview))
        {
            StopPreview();
        }

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(newAnimationButton))
        {
            animationEditor.CreateNewAnimation();
        }

        if (GUILayout.Button(exportAnimation))
        {
            Debug.Log(exportAnimation);
            ExportAnimation();
        }

        EditorGUILayout.EndHorizontal();

        if (animation != null && editModeEnabled)
        {
            ShowAnimationDetails();
            EditorGUILayout.Space();
        }
    }

    private void PreviewAnimation()
    {
        EditorApplication.EnterPlaymode();
        editModeEnabled = false;
        SetActiveObjects();
    }

    private void StopPreview()
    {
        EditorApplication.ExitPlaymode();
    }

    private void ExportAnimation()
    {
        string savePath = EditorUtility.SaveFilePanel(
            "Export animation as JSON",
            "",
            "animation" + ".json",
            "json");

        if (savePath.Length != 0)
        {
            Debug.Log(savePath);
        }
    }

    private void ShowDefaultPoseSelector()
    {
        defaultPose =
            (AnimationStep) EditorGUILayout.ObjectField("Default Pose:", defaultPose, typeof(AnimationStep), false);
    }

    private void SetActiveObjects()
    {
        previewModel.SetActive(!editModeEnabled);
        currentModel.SetActive(editModeEnabled);
        pastModel.SetActive(editModeEnabled);
    }

    private void ShowAnimationDetails()
    {
        EditorGUILayout.Separator();

        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.LabelField("Animation Details");

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(currentStep == 0);
        if (GUILayout.Button(prevStepButton))
        {
            Debug.Log(prevStepButton);
            StepBackwards();
        }

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button(addStepButton))
        {
            Debug.Log(addStepButton);
        }

        EditorGUI.BeginDisabledGroup(currentStep == stepsCount - 1);
        if (GUILayout.Button(nextStepButton))
        {
            Debug.Log(nextStepButton);
            StepForwards();
        }

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(String.Format("Animation Steps:{0}", stepsCount));
        EditorGUILayout.EndHorizontal();
        currentStep = EditorGUILayout.IntField("Current Step:", currentStep, GUILayout.ExpandWidth(true));

        // Show current step inspector

        Editor e = Editor.CreateEditor(animation.animationSteps[currentStep]);

        e.DrawDefaultInspector();
        EditorGUILayout.EndScrollView();
    }

    private void StepForwards()
    {
        Animation animation = animationEditor.GetCurrentAnimation();
        currentStep++;

        int previousStep = currentStep - 1;

        SetPastModelPose(animation.animationSteps[previousStep]);
        if (currentStep < animation.animationSteps.Count)
        {
            SetCurrentModelPose(animation.animationSteps[currentStep]);
        }

        Repaint();
    }

    private void StepBackwards()
    {
        Animation animation = animationEditor.GetCurrentAnimation();

        currentStep--;

        int previousStep = currentStep - 1;

        if (previousStep >= 0)
        {
            SetPastModelPose(animation.animationSteps[previousStep]);
        }

        SetCurrentModelPose(animation.animationSteps[currentStep]);
        Repaint();
    }

    private void ShowModelSelector()
    {
        GameObject model = (GameObject) EditorGUILayout.ObjectField("Model:", animationModel, typeof(GameObject), true);
        if (animationModel != model)
        {
            animationModel = model;

            previewModel = GameObjectUtilities.FindChildWithName(animationModel, "Model");
            currentModel = GameObjectUtilities.FindChildWithName(animationModel, "CurrentStep");
            pastModel = GameObjectUtilities.FindChildWithName(animationModel, "PreviousStep");

            SetActiveObjects();

            SetBodyParts(_currentModelBodyParts, currentModel);
            SetBodyParts(_pastModelBodyParts, pastModel);
        }
    }

    private void ShowAnimationSelector()
    {
        Animation a;
        a = (Animation) EditorGUILayout.ObjectField("Current Animation:", animation, typeof(Animation), false);
        if (a == null && animationModel != null && defaultPose != null)
        {
            SetPastModelPose(defaultPose);
            SetCurrentModelPose(defaultPose);
        }

        if (animation != a)
        {
            animation = a;
            animationEditor.SetAnimation(a);
            currentStep = 0;
            stepsCount = animation.animationSteps.Count;
        }
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