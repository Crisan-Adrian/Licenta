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

    private GameObject _currentModel;
    private GameObject _pastModel;

    private Vector2 _scrollPos;
    private AnimationStep _defaultPose;
    private int _tab;
    private EditorCoroutine _coroutine;

    private StickmanEditorController _stickmanController;

    [MenuItem("Window/Animation Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationEditorWindow));
    }

    public void Awake()
    {
        _animationEditor = new AnimationEditor();
        minSize = new Vector2(375, 500);
    }

    private void OnEnable()
    {
        _stickmanController = new StickmanEditorController();
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

        if (_animation != null)
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
            _stickmanController.SetPastModelPose(_animation.animationSteps[previousStep]);
        }

        _stickmanController.SetCurrentModelPose(_animation.animationSteps[_currentStep]);
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

        _stickmanController.SetPastModelPose(animation.animationSteps[previousStep]);
        if (_currentStep < animation.animationSteps.Count)
        {
            _stickmanController.SetCurrentModelPose(animation.animationSteps[_currentStep]);
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
            _stickmanController.SetPastModelPose(animation.animationSteps[previousStep]);
        }

        _stickmanController.SetCurrentModelPose(animation.animationSteps[_currentStep]);
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
        _currentModel = GameObjectUtilities.FindChildWithName(_animationModel, "CurrentStep");
        _pastModel = GameObjectUtilities.FindChildWithName(_animationModel, "PreviousStep");

        _stickmanController.SetModels(_currentModel, _pastModel);
    }

    private void ShowAnimationSelector()
    {
        Animation a;
        a = (Animation) EditorGUILayout.ObjectField("Current Animation:", _animation, typeof(Animation), false);
        if (a == null && _animationModel != null && _defaultPose != null)
        {
            _stickmanController.SetPastModelPose(_defaultPose);
            _stickmanController.SetCurrentModelPose(_defaultPose);
        }

        if (_animation != a)
        {
            _animation = a;
            _animationEditor.SetAnimation(a);
            _currentStep = 0;
            _stepsCount = _animation.animationSteps.Count;
        }
    }
}