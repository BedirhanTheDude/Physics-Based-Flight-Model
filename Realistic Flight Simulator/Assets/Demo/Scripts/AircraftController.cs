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

    private void FixedUpdate()
    {
        try { Input.GetAxis("Yaw"); } catch { return; }

        if (elevon != null)
            elevon.targetAngleInput = -Input.GetAxis("Vertical");

        if (rightAirleon != null)
            rightAirleon.targetAngleInput = Input.GetAxis("Horizontal");

        if (leftAirleon != null)
            leftAirleon.targetAngleInput = -Input.GetAxis("Horizontal");

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
    }
}
