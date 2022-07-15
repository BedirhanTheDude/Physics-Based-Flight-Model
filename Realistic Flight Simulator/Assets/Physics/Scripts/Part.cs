using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The class where all physics calculations take place and where all the forces are applied onto the aircraft
public class Part : MonoBehaviour
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
            Cl = wingCurve.GetLiftCoefficient(aoa);
            Cd = wingCurve.GetDragCoefficient(aoa);
            Cl *= -Mathf.Sign(velocity.y); // Because the Vector3.Angle method always returns a positive float
            // we will need to multiply the lift coefficient with the sign of the velocity's y component and this results
            // in the lift coefficient flipping to the other side of the lift curve

            // The aerodynamic forces are calculated with the equations shown in these NASA articles
            // https://www.grc.nasa.gov/www/k-12/airplane/drageq.html
            // https://www.grc.nasa.gov/www/k-12/airplane/lifteq.html
            liftDirection = Vector3.Cross(rb.velocity, transform.right).normalized;
            liftForce = velocity.sqrMagnitude * Cl * wingArea * liftMultiplier * liftDirection;
            dragForce = velocity.sqrMagnitude * Cd * wingArea * dragMultiplier * -rb.velocity.normalized;

            // Max lift force to prevent the aircraft from spinning out of control, unrealistic but gets the job done
            liftForce = Vector3.ClampMagnitude(liftForce, 200000); 

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
                dragForce = -rb.velocity.normalized * brakeDragMult * BrakeInput * velocityLerpFactor;
            else
                dragForce = -rb.velocity.normalized * brakeDragMult * BrakeInput;
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
