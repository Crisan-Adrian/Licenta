using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject[] focusTargets;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private float rotationSpeed;
    private Camera _camera;

    private int _focusIndex;

    // Start is called before the first frame update
    void Start()
    {
        _focusIndex = 0;
        _camera = Camera.main;
        
        if (focusTargets.Length > 0)
        {
            Vector3 targetPos = focusTargets[_focusIndex].transform.position;
            _camera.transform.position = targetPos + cameraOffset;
            _camera.transform.LookAt(targetPos + targetOffset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 rotationTarget = focusTargets[_focusIndex].transform.position;
            rotationTarget.y = _camera.transform.position.y;
            _camera.transform.RotateAround(rotationTarget, Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Vector3 rotationTarget = focusTargets[_focusIndex].transform.position;
            rotationTarget.y = _camera.transform.position.y;
            _camera.transform.RotateAround(rotationTarget, Vector3.up, -1 * rotationSpeed * Time.deltaTime);
        }
    }

    public void SwitchFocus()
    {
        _focusIndex++;
        if (_focusIndex >= focusTargets.Length)
        {
            _focusIndex = 0;
        }

        if (focusTargets.Length > 0)
        {
            Vector3 targetPos = focusTargets[_focusIndex].transform.position;
            _camera.transform.position = targetPos + cameraOffset;
            _camera.transform.LookAt(targetPos + targetOffset);
        }
    }
}