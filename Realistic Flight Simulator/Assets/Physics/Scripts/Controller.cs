using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotates the control surface with user input
public class Controller : MonoBehaviour
{

    [Header("Angles")]

    [SerializeField, Range(0,45)] private float maxAngle = 15f;
    [SerializeField, Range(0,45)] private float minAngle = 5f;

    public float targetAngleInput = 0f;

    [SerializeField] private float speed = 0f;

    [Header("Type")]

    [SerializeField] private bool isVisualRudder = false;
    [SerializeField] private bool isVisualLeftRudder = false;

    private float targetAngle = 0f;
    private float angle = 0f;

    private Vector3 rotationAxis = Vector3.right;

    private Quaternion startingRot;

    private void Awake()
    {
        startingRot = transform.localRotation;
        rotationAxis = (isVisualRudder) ? Vector3.up : Vector3.right;
    }

    private void FixedUpdate()
    {
        if (isVisualRudder || isVisualLeftRudder)
            targetAngleInput *= -1;
        targetAngle = (targetAngleInput > 0) ? targetAngleInput * maxAngle : targetAngleInput * minAngle;
        angle = Mathf.MoveTowards(angle, targetAngle, speed * Time.fixedDeltaTime);

        transform.localRotation = startingRot;
        transform.Rotate(rotationAxis, angle, Space.Self);
    }
}