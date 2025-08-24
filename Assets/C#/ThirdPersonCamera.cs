using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    private GameObject _mainCamera;
    [Header("Cinemachine")]
    //[Tooltip("Follow Object")]
    public GameObject CameraTarget;

    [Tooltip("Up Angle")]
    public float TopClamp = 70.0f;

    [Tooltip("Down Angle")]
    public float BottomClamp = -30.0f;

    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
          
   
    void Start()
    {
        if(_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        _cinemachineTargetYaw = CameraTarget.transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        if(_look.sqrMagnitude >= _threshold)
        {
            _cinemachineTargetYaw -= _look.x;
            _cinemachineTargetPitch += _look.y;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);


        CameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private Vector2 _look;
    public void Onlook(Vector2 look)
    {
        _look = look;
    }

    public float GetYaw()
    {
        return _cinemachineTargetYaw;
    }


}
