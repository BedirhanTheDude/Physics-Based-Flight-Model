using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the class that takes care of controlling the aircraft's brakes, control surfaces, the engine and the autopilot
public class AircraftController : MonoBehaviour
{ 
    [Header("Components")]

    [SerializeField] private Controller rightAirleon = null;
    [SerializeField] private Controller leftAirleon = null;
    [SerializeField] private Controller elevon = null;
    [SerializeField] private Controller rudder = null;
    [SerializeField] private Part brake1 = null;
    [SerializeField] private Part brake2 = null;
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
        // If the key is pressed the autopilot will be enabled/disabled
        if (Input.GetKeyDown(ActivateAutoPilot))
            autoPilotActivated = !autoPilotActivated;
    }

    private void FixedUpdate()
    {
        // Rotating the world x axis vector relative to our aircraft rotation so that it keeps lined up with our local x axis
        Vector3 aircraftRotationEuler = transform.rotation.eulerAngles;
        aircraftRotationEuler.x = aircraftRotationEuler.z = 0f;
        Quaternion aircraftRotation = Quaternion.Euler(aircraftRotationEuler);

        // Calculating the angle between the rotated world x axis and our local x axis
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
        if (_pilotEnabled)
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

        // Calculating how hard the autopilot should pull in order to level the aircraft with the local x axis
        if (Mathf.Sign(angle) >= 0)
            autoPilotInput = Mathf.InverseLerp(0, 180, angle);
        else
            autoPilotInput = -Mathf.InverseLerp(0, -180, angle);
        return Mathf.Clamp(autoPilotInput, -0.6f, 0.6f);

    }

    private void CheckAutoPilot()
    {
     /* Checking if any of the WASD keys are pressed, if the angle is not
        negligible enough for the autopilot to ignore and also if the user has enabled the autopilot
        If all conditions are met the autopilot is allowed to take control */
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && (angle >= 10f || angle <= -10f) && autoPilotActivated)
            _pilotEnabled = true;
        else
            _pilotEnabled = false;
    }

}
