using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text throttleText = null;
    [SerializeField] private Text speedText = null;
    [SerializeField] private Text brakeText = null;
    [SerializeField] private Text pilotText = null;
    [SerializeField] private Engine planeEngine = null;
    [SerializeField] private Part brake = null;    
    [SerializeField] private Image crosshair = null;
    [SerializeField] private Image velocityIndicator = null;
    [SerializeField] private AircraftController controller = null;

    private void Update()
    {
        if (planeEngine != null)
        {
            string throttle = (planeEngine.Throttle * 100).ToString("0");
            string velocity = planeEngine.Rigidbody.velocity.magnitude.ToString("0.0");
            string pilot = controller.autoPilotActivated ? "ON" : "OFF";

            bool brakeOn = brake.BrakeInput == 1f;

            brakeText.text = brakeOn ? "BRK: ON" : "BRK: OFF";
            throttleText.text = $"THR: {throttle}%";
            speedText.text = $"SPD: {velocity}m/s";
            pilotText.text = $"PLT: {pilot}";

            if (crosshair != null)
            {
                Transform planeTransform = planeEngine.transform;
                Vector3 sightPosition = planeTransform.position + planeTransform.forward * 500;
                Vector3 spriteLocation = Camera.main.WorldToScreenPoint(sightPosition);

                crosshair.transform.position = spriteLocation;
            }
        }
    }

    private void FixedUpdate()
    {
        Rigidbody _rb = planeEngine.GetComponent<Rigidbody>();
        bool stopped = _rb.velocity.magnitude <= 1f;
        Vector3 velocityDirection = !stopped ? _rb.velocity.normalized : planeEngine.transform.forward;
        Vector3 velocityPosition = velocityDirection * 500 + planeEngine.transform.position;
        velocityIndicator.transform.position = Camera.main.WorldToScreenPoint(velocityPosition);
    }

}
