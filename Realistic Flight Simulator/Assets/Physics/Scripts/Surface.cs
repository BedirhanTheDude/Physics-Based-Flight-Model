using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{

    [Header("Angles")]

    [SerializeField, Range(0,45)] private float maxAngle = 15f;
    [SerializeField, Range(0,45)] private float minAngle = 15f;

    public float targetAngleInput = 0f;

    [SerializeField] private float speed = 0f;

    [Header("Type")]

    private float targetAngle = 0f;
    private float angle = 0f;

    private Quaternion startRotation;

    private void Awake()
    {
        startRotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        targetAngle = (targetAngleInput > 0) ? targetAngleInput * maxAngle : targetAngleInput * minAngle;
        angle = Mathf.MoveTowards(angle, targetAngle, speed * Time.fixedDeltaTime);

        transform.localRotation = startRotation;
        transform.Rotate(Vector3.right, angle, Space.Self);
    }

}