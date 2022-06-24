using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AeroCurves")]
public class AeroCurves : ScriptableObject
{

    [Tooltip("Lift Curve"), SerializeField]
    private AnimationCurve liftCurve = new AnimationCurve();

    [Tooltip("Drag Curve"), SerializeField]
    private AnimationCurve dragCurve = new AnimationCurve();

    ///<summary>
    /// Gets lift coefficinet at specified angle of attack
    ///</summary>
    public float GetLiftCoefficient(float aoa) { return liftCurve.Evaluate(aoa); }

    ///<summary>
    /// Gets drag coefficint at specified angle of attack
    ///</summary>
    public float GetDragCoefficient(float aoa) { return dragCurve.Evaluate(aoa); }

}
