using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{

    [Header("Components")]

    [SerializeField] private Transform cameraRigTransform;
    [SerializeField] private Transform planeTransform;

    [Header("Parameters")]
    [SerializeField] private float rotationSpeed = 10f;

    private void LateUpdate()
    {
        cameraRigTransform.position = planeTransform.position;
        RotateCamera();
    }
    private void RotateCamera()
    {
        Quaternion planeRotation = planeTransform.rotation;
        Quaternion cameraRotation = cameraRigTransform.rotation;

        cameraRigTransform.rotation = Maths.DampQ(cameraRotation, planeRotation, rotationSpeed, Time.deltaTime);
    }

}
