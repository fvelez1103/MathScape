using UnityEngine;

public class OperatorPickup : MonoBehaviour
{
    public enum OperationType
    {
        Add,
        Subtract,
        Multiply
    }

    public OperationType operation;
    [HideInInspector] public bool isAttached = false;
}
