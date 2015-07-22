using UnityEngine;
using System.Collections;

public class VectorUtil {

    public static Vector2 RotateVector(Vector2 vector, float radians)
    {
        return new Vector2(vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians),
            vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians));
    }

    public static Vector3 RotateVector(Vector3 vector, float radians)
    {
        return new Vector3(vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians),
            vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians), 0);
    }
}
