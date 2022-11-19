using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The class where all physics calculations take place and where all the forces are applied onto the aircraft
public class Part : MonoBehaviour
{
    private const float BRAKE_DRAG_MULT = 200000f;

    [Header("Wing Properties")]

    [SerializeField] private Vector2 wingDimensions = Vector2.zero;
    [SerializeField] private AeroCurves wingCurve;
    [SerializeField] private bool isBrake = false;

    [Header("Physics")]

    [SerializeField] private bool centralizedForce = false;
    [SerializeField] private float liftMultiplier = 1f;
    [SerializeField] private float dragMultiplier = 1f;
    private float inducedDragMult = 2f;

    [SerializeField] private Rigidbody rb;

    private float Cd = 1f;
    private float Cl = 1f;
    private float Ci = 1f;
    private Vector3 liftForce;
    private Vector3 dragForce;
    private Vector3 inducedDrag;

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
        // Only works if run in Unity
        // Draws rays to visualize how much force is being applied and where
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

    // Physics
    private void FixedUpdate()
    {
        Vector3 forceApplyPos = (centralizedForce) ? rb.transform.TransformPoint(rb.centerOfMass) : transform.position;

        Vector3 velocity = transform.InverseTransformDirection(rb.GetPointVelocity(transform.position));
        velocity.x = 0f; // Discarding the velocity on the x axis as it doesn't affect the angle of attack
        float aoa = Vector3.Angle(Vector3.forward, velocity);

        if (!isBrake)
        {
            Vector3 liftVelocity = Vector3.ProjectOnPlane(velocity, Vector3.right);
            Cl = wingCurve.GetLiftCoefficient(aoa);
            Cd = wingCurve.GetDragCoefficient(aoa);
            Cl *= -Mathf.Sign(velocity.y);
            // Because the Vector3.Angle method always returns a positive float
            // we will need to multiply the lift coefficient with the sign of the velocity's y component and this results
            // in the lift coefficient flipping to the other side of the lift curve

            Ci = Cl * Cl * inducedDragMult;

            // The aerodynamic forces are calculated with the equations shown in these NASA articles
            // https://www.grc.nasa.gov/www/k-12/airplane/drageq.html
            // https://www.grc.nasa.gov/www/k-12/airplane/lifteq.html
            liftDirection = Vector3.Cross(rb.velocity, transform.right).normalized;
            liftForce = liftVelocity.sqrMagnitude * Cl * wingArea * liftMultiplier * liftDirection;
            inducedDrag = liftVelocity.sqrMagnitude * Cl * Cl * inducedDragMult * -rb.velocity.normalized;
            liftForce += inducedDrag;
            dragForce = velocity.sqrMagnitude * Cd * wingArea * dragMultiplier * -rb.velocity.normalized;

            

            // Forces are being applied at relevant wing positions
            rb.AddForceAtPosition(liftForce, forceApplyPos, ForceMode.Force);
            rb.AddForceAtPosition(dragForce, forceApplyPos, ForceMode.Force);
        }
        else
        {
            // Calculating brake forces that are calculated differently to the normal drag calculations
            // These brakes don't generate lift
            float velocityLerpFactor = Mathf.InverseLerp(0, 200, rb.velocity.magnitude);
            if (rb.velocity.magnitude >= 60 || Mathf.Sign(transform.position.y) >= 0)
                dragForce = -rb.velocity.normalized * BRAKE_DRAG_MULT * BrakeInput * velocityLerpFactor;
            else
                dragForce = -rb.velocity.normalized * BRAKE_DRAG_MULT * BrakeInput;
            rb.AddForceAtPosition(dragForce, forceApplyPos, ForceMode.Force);
        }        
    }

    // For editor purposes
    private void OnDrawGizmosSelected()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(wingDimensions.x, 0f, wingDimensions.y));

        Gizmos.matrix = oldMatrix;
    }
}
