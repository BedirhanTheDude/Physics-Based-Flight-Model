using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class CopyInput : MonoBehaviour
{
    [Header("Components")]

    [SerializeField] private Controller part;
    private Controller thisController = null;

    private void Awake()
    {
        thisController = GetComponent<Controller>();
    }

    private void Update()
    {
        thisController.targetAngleInput = part.targetAngleInput;
    }

}
