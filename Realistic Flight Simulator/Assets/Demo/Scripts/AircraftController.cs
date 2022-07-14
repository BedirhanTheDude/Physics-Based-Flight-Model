using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftController : MonoBehaviour
{

    [Header("Components")]

    [SerializeField] private Surface rightAirleon = null;
    [SerializeField] private Surface leftAirleon = null;
    [SerializeField] private Surface elevon = null;
    [SerializeField] private Surface rudder = null;
    [SerializeField] private Wing brake1 = null;
    [SerializeField] private Wing brake2 = null;
    [Space]
    [SerializeField] private Engine engine = default;

    [Header("Keys")]

    [SerializeField] private KeyCode ActivateAutoPilot = KeyCode.P;
    private float autoPilotInput = 0f;


    private bool _pilotEnabled = false;
    public bool autoPilotActivated = false;
    private float angle;

    private void Update()
    {
        if (Input.GetKeyDown(ActivateAutoPilot))
            autoPilotActivated = !autoPilotActivated;
    }

    private void FixedUpdate()
    {
        Vector3 aircraftRotationEuler = transform.rotation.eulerAngles;
        aircraftRotationEuler.x = 0f;
        aircraftRotationEuler.z = 0f;
        Quaternion aircraftRotation = Quaternion.Euler(aircraftRotationEuler);

        angle = Vector3.SignedAngle(aircraftRotation * Vector3.right, transform.right, transform.forward);

        try { Input.GetAxis("Yaw"); } catch { return; }

        if (elevon != null)
            elevon.targetAngleInput = -Input.GetAxis("Vertical");

        if (rudder != null)
            rudder.targetAngleInput = Input.GetAxis("Yaw");

        if (engine != null)
        {
            if (Input.GetKey(KeyCode.LeftShift)) engine.TargetThrottle = 1f;
            else if (Input.GetKey(KeyCode.LeftControl)) engine.TargetThrottle = -1f;
        }
        if (brake1 != null && brake2 != null)
        {
            brake1.BrakeInput = Input.GetKey(KeyCode.B) ? 1f : 0f;
            brake2.BrakeInput = brake1.BrakeInput;
        }

        if (rightAirleon != null)
            rightAirleon.targetAngleInput = Input.GetAxis("Horizontal");

        if (leftAirleon != null)
            leftAirleon.targetAngleInput = -Input.GetAxis("Horizontal");

        CheckAutoPilot();
        if (_pilotEnabled && autoPilotActivated)
        {
            autoPilotInput = RunAutoPilot();

            if (rightAirleon != null)
                rightAirleon.targetAngleInput = autoPilotInput;

            if (leftAirleon != null)
                leftAirleon.targetAngleInput = -autoPilotInput;
        }
    }

    private float RunAutoPilot()
    {
        // Returns input for auto pilot
        float autoPilotInput = 0f;

        if (Mathf.Sign(angle) >= 0)
            autoPilotInput = Mathf.InverseLerp(0, 180, angle);
        else
            autoPilotInput = -Mathf.InverseLerp(0, -180, angle);
        return Mathf.Clamp(autoPilotInput, -0.6f, 0.6f);

    }

    private void CheckAutoPilot()
    {
        // Autopilot logic

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && (angle >= 10f || angle <= -10f))
            _pilotEnabled = true;
        else
            _pilotEnabled = false;
    }

}
