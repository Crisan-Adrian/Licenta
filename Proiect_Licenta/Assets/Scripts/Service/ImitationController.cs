using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utilities;

public class ImitationController : MonoBehaviour
{
    private string _imitationFile;
    private Imitation _frames;
    private int _index;
    private Dictionary<string, GameObject> _bodyParts = new();
    private Dictionary<string, Vector3> _rotations = new();
    private Dictionary<string, Vector3> _initialRotations = new();

    // Start is called before the first frame update
    void Start()
    {
        _imitationFile = EditorProxy.GetImitationFile();
        _frames = Imitation.FromJson(_imitationFile);
        _index = 0;
        SetBodyParts();
        SetRotations();
    }

    private void SetRotations()
    {
        _rotations["left_leg_lower"] = _bodyParts["left_leg_lower"].transform.eulerAngles;
        _initialRotations["left_leg_lower"] = _bodyParts["left_leg_lower"].transform.eulerAngles;
        _rotations["left_leg_upper"] = _bodyParts["left_leg_upper"].transform.eulerAngles;
        _initialRotations["left_leg_upper"] = _bodyParts["left_leg_upper"].transform.eulerAngles;
        _rotations["right_leg_lower"] = _bodyParts["right_leg_lower"].transform.eulerAngles;
        _initialRotations["right_leg_lower"] = _bodyParts["right_leg_lower"].transform.eulerAngles;
        _rotations["right_leg_upper"] = _bodyParts["right_leg_upper"].transform.eulerAngles;
        _initialRotations["right_leg_upper"] = _bodyParts["right_leg_upper"].transform.eulerAngles;
        _rotations["left_arm_lower"] = _bodyParts["left_arm_lower"].transform.eulerAngles;
        _initialRotations["left_arm_lower"] = _bodyParts["left_arm_lower"].transform.eulerAngles;
        _rotations["left_arm_upper"] = _bodyParts["left_arm_upper"].transform.eulerAngles;
        _initialRotations["left_arm_upper"] = _bodyParts["left_arm_upper"].transform.eulerAngles;
        _rotations["right_arm_lower"] = _bodyParts["right_arm_lower"].transform.eulerAngles;
        _initialRotations["right_arm_lower"] = _bodyParts["right_arm_lower"].transform.eulerAngles;
        _rotations["right_arm_upper"] = _bodyParts["right_arm_upper"].transform.eulerAngles;
        _initialRotations["right_arm_upper"] = _bodyParts["right_arm_upper"].transform.eulerAngles;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StepImitation();
    }

    private void StepImitation()
    {
        if (_index >= _frames.predictions.Count)
        {
            _index = 0;
            ResetRotations();
        }
        ImitationFrame imitationFrame = _frames.predictions[_index];
        ApplyRotations(imitationFrame);
        _index++;
    }

    private void ResetRotations()
    {
        _bodyParts["left_leg_lower"].transform.eulerAngles = _initialRotations["left_leg_lower"];
        _rotations["left_leg_lower"] = _initialRotations["left_leg_lower"];
        
        _bodyParts["left_leg_upper"].transform.eulerAngles = _initialRotations["left_leg_upper"];
        _rotations["left_leg_upper"] = _initialRotations["left_leg_upper"];
        
        _bodyParts["right_leg_lower"].transform.eulerAngles = _initialRotations["right_leg_lower"];
        _rotations["right_leg_lower"] = _initialRotations["right_leg_lower"];
        
        _bodyParts["right_leg_upper"].transform.eulerAngles = _initialRotations["right_leg_upper"];
        _rotations["right_leg_upper"] = _initialRotations["right_leg_upper"];
        
        _bodyParts["left_arm_lower"].transform.eulerAngles = _initialRotations["left_arm_lower"];
        _rotations["left_arm_lower"] = _initialRotations["left_arm_lower"];
        
        _bodyParts["left_arm_upper"].transform.eulerAngles = _initialRotations["left_arm_upper"];
        _rotations["left_arm_upper"] = _initialRotations["left_arm_upper"];
        
        _bodyParts["right_arm_lower"].transform.eulerAngles = _initialRotations["right_arm_lower"];
        _rotations["right_arm_lower"] = _initialRotations["right_arm_lower"];
        
        _bodyParts["right_arm_upper"].transform.eulerAngles = _initialRotations["right_arm_upper"];
        _rotations["right_arm_upper"] = _initialRotations["right_arm_upper"];
    }

    private void ApplyRotations(ImitationFrame imitationFrame)
    {
        
        _rotations["left_leg_lower"] += new Vector3(imitationFrame.left_leg_lower, 0, 0);
        _bodyParts["left_leg_lower"].transform.eulerAngles = _rotations["left_leg_lower"];
        
        _rotations["left_leg_upper"] += new Vector3(imitationFrame.left_leg_upper, 0, 0);
        _bodyParts["left_leg_upper"].transform.eulerAngles = _rotations["left_leg_upper"];
        
        _rotations["right_leg_lower"] += new Vector3(imitationFrame.right_leg_lower, 0, 0);
        _bodyParts["right_leg_lower"].transform.eulerAngles = _rotations["right_leg_lower"];
        
        _rotations["right_leg_upper"] += new Vector3(imitationFrame.right_leg_upper, 0, 0);
        _bodyParts["right_leg_upper"].transform.eulerAngles = _rotations["right_leg_upper"];
        
        _rotations["left_arm_lower"] += new Vector3(imitationFrame.left_arm_lower, 0, 0);
        _bodyParts["left_arm_lower"].transform.eulerAngles = _rotations["left_arm_lower"];
        
        _rotations["left_arm_upper"] += new Vector3(imitationFrame.left_arm_upper, 0, 0);
        _bodyParts["left_arm_upper"].transform.eulerAngles = _rotations["left_arm_upper"];
        
        _rotations["right_arm_lower"] += new Vector3(imitationFrame.right_arm_lower, 0, 0);
        _bodyParts["right_arm_lower"].transform.eulerAngles = _rotations["right_arm_lower"];
        
        _rotations["right_arm_upper"] += new Vector3(imitationFrame.right_arm_upper, 0, 0);
        _bodyParts["right_arm_upper"].transform.eulerAngles = _rotations["right_arm_upper"];
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
    }
}

[System.Serializable]
internal class Imitation
{
    public List<ImitationFrame> predictions;

    public static Imitation FromJson(string path)
    {
        string imitationJson = File.ReadAllText(path);
        Imitation imitation = new Imitation();
        imitation = JsonUtility.FromJson<Imitation>(imitationJson);

        return imitation;
    }
}

[System.Serializable]
internal class ImitationFrame
{
    public float left_leg_upper;
    public float left_leg_lower;
    public float right_leg_upper;
    public float right_leg_lower;
    public float left_arm_upper;
    public float left_arm_lower;
    public float right_arm_upper;
    public float right_arm_lower;
}