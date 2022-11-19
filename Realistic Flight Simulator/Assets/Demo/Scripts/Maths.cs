using UnityEngine;

public class Maths : MonoBehaviour
{
    public static float CalculateG(Rigidbody rb)
    {
        Vector3 velocity = rb.velocity;
        Vector3 angularVelocity = rb.angularVelocity;

        //Since a = v2 / r , a = w2 * r and r = v / w
        //we can substitude r with r = v / w and we are left with a = w * v

        float gForce = Vector3.Cross(angularVelocity, velocity).magnitude / 9.81f;

        return gForce;
    }

    public static Quaternion DampQ(Quaternion a, Quaternion b, float speed, float dt)
    {
        return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-speed * dt));
    }

    public static Vector3 DampV(Vector3 a, Vector3 b, float speed, float dt)
    {
        return Vector3.Slerp(a, b, 1 - Mathf.Exp(-speed * dt));
    }
}
