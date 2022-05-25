using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using Utilities;

public class AnimationEditorWindow : EditorWindow
{
    private string newAnimationButton = "Create New Animation";
    private string prevStepButton = "Prev Step";
    private string nextStepButton = "Next Step";
    private string resetPoseButton = "Reset Step";
    private string addStepButton = "Add Step";
    private string exportAnimationButton = "Export Animation";
    private string importAnimationButton = "Import Animation";
    private string previewAnimation = "Preview Animation";
    private string stopPreview = "Stop Preview";

    private int _currentStep;
    private int _stepsCount;

    private AnimationEditor _animationEditor;
    private Animation _animation;
    private GameObject _animationModel;

    private GameObject _previewModel;
    private GameObject _currentModel;
    private GameObject _pastModel;

    private List<string> _keys = new();
    private Dictionary<string, GameObject> _currentModelBodyParts = new();
    private Dictionary<string, GameObject> _pastModelBodyParts = new();
    private bool _editModeEnabled;
    private Vector2 _scrollPos;
    private AnimationStep _defaultPose;
    private int _tab;
    private EditorCoroutine _coroutine;

    [MenuItem("Window/Animation Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationEditorWindow));
    }

    public void Awake()
    {
        _animationEditor = new AnimationEditor();
        _editModeEnabled = true;
        minSize = new Vector2(375, 500);
    }

    private void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            _editModeEnabled = true;
            if (_animationModel != null)
            {
                SetActiveObjects();
            }
        }
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    void OnGUI()
    {
        _tab = GUILayout.Toolbar(_tab, new string[] {"Edit Animations", "Imitation "});
        switch (_tab)
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

        EditorGUI.BeginDisabledGroup(_animationModel == null);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(!_editModeEnabled);
        if (GUILayout.Button(previewAnimation))
        {
            PreviewAnimation();
        }

        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(_editModeEnabled);
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
            CreateNewAnimation();
        }

        if (GUILayout.Button(importAnimationButton))
        {
            Debug.Log(importAnimationButton);
            ImportAnimation();
        }

        if (GUILayout.Button(exportAnimationButton))
        {
            Debug.Log(exportAnimationButton);
            ExportAnimation();
        }

        EditorGUILayout.EndHorizontal();

        if (_animation != null && _editModeEnabled)
        {
            ShowAnimationDetails();
            EditorGUILayout.Space();
        }
    }

    private void CreateNewAnimation()
    {
        string animationName = NewAnimationWindow.Open();
        if (animationName != "")
        {
            _animation = _animationEditor.CreateNewAnimation(animationName);
            _stepsCount = _animation.animationSteps.Count;
        }
    }

    private void PreviewAnimation()
    {
        _editModeEnabled = false;
        StickmanEditorController stickManEditorController = new StickmanEditorController();
        stickManEditorController.FrameModifier = 2;
        stickManEditorController.FrameRate = 60;
        stickManEditorController.Setup(_animation, _previewModel);
        SetActiveObjects();
        _coroutine = this.StartCoroutine(
            stickManEditorController.StartPreview());
    }

    private void StopPreview()
    {
        _editModeEnabled = true;
        SetActiveObjects();
        this.StopCoroutine(_coroutine);
    }

    private void ExportAnimation()
    {
        string savePath = EditorUtility.SaveFilePanel(
            "Export animation as JSON",
            "Assets",
            "animation" + ".json",
            "json");

        if (savePath.Length != 0)
        {
            string json = AnimationSerializer.ToJSON(_animation);

            File.WriteAllText(savePath, json);
            
            // Debug.Log(json);
        }
    }

    private void ImportAnimation()
    {
        string openPath = EditorUtility.OpenFilePanel(
            "Import JSON animation",
            "Assets",
            "json");

        if (openPath.Length != 0)
        {
            Debug.Log(openPath);
            string json = File.ReadAllText(openPath);
            AnimationDTO animationDto = AnimationSerializer.FromJSON(json);
            _animation = _animationEditor.ImportAnimation(animationDto);
            _stepsCount = _animation.animationSteps.Count;
        }
    }

    private void ShowDefaultPoseSelector()
    {
        _defaultPose =
            (AnimationStep) EditorGUILayout.ObjectField("Default Pose:", _defaultPose, typeof(AnimationStep), false);
    }

    private void SetActiveObjects()
    {
        _previewModel.SetActive(!_editModeEnabled);
        _currentModel.SetActive(_editModeEnabled);
        _pastModel.SetActive(_editModeEnabled);
    }

    private void ShowAnimationDetails()
    {
        EditorGUILayout.Separator();

        _scrollPos =
            EditorGUILayout.BeginScrollView(_scrollPos);
        EditorGUILayout.LabelField("Animation Details");

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(_currentStep == 0);
        if (GUILayout.Button(prevStepButton))
        {
            StepBackwards();
        }
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button(resetPoseButton))
        {
            ResetPose();
        }

        EditorGUI.BeginDisabledGroup(_currentStep == _stepsCount - 1);
        if (GUILayout.Button(nextStepButton))
        {
            StepForwards();
        }

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button(addStepButton))
        {
            AddAnimationStep();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(String.Format("Animation Steps:{0}", _stepsCount));
        EditorGUILayout.EndHorizontal();
        _currentStep = EditorGUILayout.IntField("Current Step:", _currentStep, GUILayout.ExpandWidth(true));

        // Show current step inspector

        Editor e = Editor.CreateEditor(_animation.animationSteps[_currentStep]);

        e.DrawDefaultInspector();
        EditorGUILayout.EndScrollView();
    }

    private void AddAnimationStep()
    {
        _animationEditor.AddNewAnimationStep();
        _stepsCount = _animation.animationSteps.Count;
        
        SetCurrentStep(_animation.animationSteps.Count - 1);
    }

    private void SetCurrentStep(int step)
    {
        _currentStep = step;
        
        int previousStep = _currentStep - 1;

        if (previousStep >= 0)
        {
            SetPastModelPose(_animation.animationSteps[previousStep]);
        }

        SetCurrentModelPose(_animation.animationSteps[_currentStep]);
        Repaint();
    }

    private void ResetPose()
    {
        SetCurrentStep(0);
    }

    private void StepForwards()
    {
        Animation animation = _animationEditor.GetCurrentAnimation();
        _currentStep++;

        int previousStep = _currentStep - 1;

        SetPastModelPose(animation.animationSteps[previousStep]);
        if (_currentStep < animation.animationSteps.Count)
        {
            SetCurrentModelPose(animation.animationSteps[_currentStep]);
        }

        Repaint();
    }

    private void StepBackwards()
    {
        Animation animation = _animationEditor.GetCurrentAnimation();

        _currentStep--;

        int previousStep = _currentStep - 1;

        if (previousStep >= 0)
        {
            SetPastModelPose(animation.animationSteps[previousStep]);
        }

        SetCurrentModelPose(animation.animationSteps[_currentStep]);
        Repaint();
    }

    private void ShowModelSelector()
    {
        GameObject model = (GameObject) EditorGUILayout.ObjectField("Model:", _animationModel, typeof(GameObject), true);
        if (_animationModel != model)
        {
            _animationModel = model;

            NewModelSetup();
        }

        if (_animationModel == null)
        {
            if (GUILayout.Button("Create Model"))
            {
                Debug.Log("This will create and assign a model to the editor.");
                GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Editor/AnimationModel.prefab", typeof(GameObject));
                Debug.Log(prefab);
                _animationModel = Instantiate(prefab);
                NewModelSetup();
            }
        }
    }

    private void NewModelSetup()
    {
        _previewModel = GameObjectUtilities.FindChildWithName(_animationModel, "Model");
        _currentModel = GameObjectUtilities.FindChildWithName(_animationModel, "CurrentStep");
        _pastModel = GameObjectUtilities.FindChildWithName(_animationModel, "PreviousStep");

        SetActiveObjects();

        SetBodyParts(_currentModelBodyParts, _currentModel);
        SetBodyParts(_pastModelBodyParts, _pastModel);
    }

    private void ShowAnimationSelector()
    {
        Animation a;
        a = (Animation) EditorGUILayout.ObjectField("Current Animation:", _animation, typeof(Animation), false);
        if (a == null && _animationModel != null && _defaultPose != null)
        {
            SetPastModelPose(_defaultPose);
            SetCurrentModelPose(_defaultPose);
        }

        if (_animation != a)
        {
            _animation = a;
            _animationEditor.SetAnimation(a);
            _currentStep = 0;
            _stepsCount = _animation.animationSteps.Count;

            EditorProxy editorProxy = EditorProxy.GetInstance();
            editorProxy.Animation = _animation;
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