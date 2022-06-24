using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing : MonoBehaviour
{
    private const float brakeDragMult = 100000f;

    [Header("Wing Properties")]

    [SerializeField] private Vector2 wingDimensions = Vector2.zero;
    [SerializeField] private AeroCurves wingCurve;
    [SerializeField] private bool isBrake = false;

    [Header("Physics")]

    [SerializeField] private bool centralizedForce = false;
    [SerializeField] private float liftMultiplier = 1f;
    [SerializeField] private float dragMultiplier = 1f;

    [SerializeField] private Rigidbody rb;

    [SerializeField] public float Cd = 1f;
    [SerializeField] public float Cl = 1f;
    [SerializeField] private Vector3 liftForce;
    [SerializeField] public Vector3 dragForce;

    private float _brakeInput = 1f;

    private Vector3 liftDirection = Vector3.up;

    public float BrakeInput
    {
        get { return _brakeInput; }
        set { _brakeInput = value; }
    }

    private void Awake()
    {
        rb.maxAngularVelocity = 40f;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, .0001f * liftForce, Color.blue);
        if (isBrake && !(rb.velocity.magnitude <= 2f))
        {
            Debug.DrawRay(transform.position, .00005f * dragForce, Color.red);
        }
        else if (!isBrake)
        {
            Debug.DrawRay(transform.position, .00005f * dragForce, Color.red);
        }
        
    }

    public float wingArea
    {
        get { return wingDimensions.x * wingDimensions.y; }
    }


    private void FixedUpdate()
    {
        
        Vector3 forceApplyPos = (centralizedForce) ? rb.transform.TransformPoint(rb.centerOfMass) : transform.position;

        Vector3 velocity = transform.InverseTransformDirection(rb.GetPointVelocity(transform.position));
        velocity.x = 0f;
        float aoa = Vector3.Angle(Vector3.forward, velocity);

        if (!isBrake)
        {            
            Cl = wingCurve.GetLiftCoefficient(aoa);
            Cd = wingCurve.GetDragCoefficient(aoa);
            Cl *= -Mathf.Sign(velocity.y);

            liftDirection = Vector3.Cross(rb.velocity, transform.right).normalized;
            liftForce = velocity.sqrMagnitude * Cl * wingArea * liftMultiplier * liftDirection;
            dragForce = velocity.sqrMagnitude * Cd * wingArea * dragMultiplier * -rb.velocity.normalized;

            liftForce = Vector3.ClampMagnitude(liftForce, 200000);

            rb.AddForceAtPosition(liftForce, forceApplyPos, ForceMode.Force);
            rb.AddForceAtPosition(dragForce, forceApplyPos, ForceMode.Force);
        }
        else
        {
            float velocityLerpFactor = Mathf.InverseLerp(0, 200, rb.velocity.magnitude);
            if (rb.velocity.magnitude >= 60 || Mathf.Sign(transform.position.y) >= 0)
                dragForce = -rb.velocity.normalized * brakeDragMult * BrakeInput * velocityLerpFactor;
            else
                dragForce = -rb.velocity.normalized * brakeDragMult * BrakeInput;
            rb.AddForceAtPosition(dragForce, forceApplyPos, ForceMode.Force);
        }



        
    }

    private void OnDrawGizmosSelected()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(wingDimensions.x, 0f, wingDimensions.y));

        Gizmos.matrix = oldMatrix;
    }
}
