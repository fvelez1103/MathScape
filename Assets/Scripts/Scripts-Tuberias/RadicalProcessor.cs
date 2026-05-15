using UnityEngine;

public class RadicalProcessor : MonoBehaviour
{
    [Header("Inputs")]
    public float x;
    public float y;

    [Header("Equation Parameters")]
    public float c;

    public FlowRouter junction;

    public float Result()
    {
        // √(x + y) = c   →   x + y = c²
        return Mathf.Sqrt(x + y);
    }

    public bool IsValid()
    {
        if (x + y < 0) return false;
        return Mathf.Abs(Result() - c) < 0.1f;
    }

}
