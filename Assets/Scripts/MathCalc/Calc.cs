using UnityEngine;

public class Calc : MonoBehaviour
{
    public static bool IsWithinXRange(Transform target, Transform current, float range)
    {
        return Mathf.Abs(target.position.x - current.position.x) <= range;
    }

    public static bool IsWithinYRange(Transform target, Transform current, float range)
    {
        return Mathf.Abs(target.position.y - current.position.y) <= range;
    }
}
