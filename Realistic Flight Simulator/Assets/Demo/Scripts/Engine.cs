using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Engine : MonoBehaviour
{
    
    [Header("Engine Properties")]
    [SerializeField] private float thrustSpeed = 10f;
    [SerializeField, Tooltip("kN")] private float enginePower = 100f;

    /*[Header("Brakes")]

    [SerializeField] private float brakePower = 1f;*/

    private float brakeInput = 0f;

    private float _throttle = 0f;
    private float _targetThrottle = 0f;

    private Vector3 thrustVector = Vector3.zero;

    public float BrakeInput
    {
        set { brakeInput = value; }
        get { return brakeInput; }
    }

    public float TargetThrottle
    { 
        get { return _targetThrottle; }
        set { _targetThrottle = Mathf.Clamp01(value); }
    }

    public float Throttle
    {
        get { return _throttle; }
        set { _throttle = Mathf.Clamp01(value); }
    }

    private Rigidbody rb;

    public Rigidbody Rigidbody { get { return rb; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl))
        {
            Throttle = Mathf.MoveTowards(Throttle, TargetThrottle, thrustSpeed * Time.fixedDeltaTime);
        }
        thrustVector = Vector3.forward * Throttle * enginePower;

        //Vector3 brakeVector = -rb.velocity.normalized * brakePower * brakeInput;

        
        
        if (rb != null)
        {
            rb.AddRelativeForce(thrustVector, ForceMode.Force);
            //rb.AddRelativeForce(brakeVector, ForceMode.Force);
        }
    }

}
