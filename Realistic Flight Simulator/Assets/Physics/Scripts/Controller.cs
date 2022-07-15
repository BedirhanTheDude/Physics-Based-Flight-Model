using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotates the control surface with user input
public class Controller : MonoBehaviour
{

    [Header("Angles")]

    [SerializeField, Range(0,45)] private float maxAngle = 15f;
    [SerializeField, Range(0,45)] private float minAngle = 15f;

    public float targetAngleInput = 0f;

    [SerializeField] private float speed = 0f;

    [Header("Type")]

    private float targetAngle = 0f;
    private float angle = 0f;

    private Quaternion startingRot;

    private void Awake()
    {
        startingRot = transform.localRotation;
    }

    private void FixedUpdate()
    {
        targetAngle = (targetAngleInput > 0) ? targetAngleInput * maxAngle : targetAngleInput * minAngle;
        angle = Mathf.MoveTowards(angle, targetAngle, speed * Time.fixedDeltaTime);

        transform.localRotation = startingRot;
        transform.Rotate(Vector3.right, angle, Space.Self);
    }
}