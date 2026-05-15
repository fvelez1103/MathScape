using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector3 ReadMovement()
    {
        if (Input.GetKeyDown(KeyCode.W)) return Vector3.up;
        if (Input.GetKeyDown(KeyCode.S)) return Vector3.down;
        if (Input.GetKeyDown(KeyCode.A)) return Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) return Vector3.right;
        if (Input.GetKeyDown(KeyCode.UpArrow)) return Vector3.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) return Vector3.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return Vector3.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) return Vector3.right;

        return Vector3.zero;
    }

    public int ReadRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q)) return 1;
        if (Input.GetKeyDown(KeyCode.E)) return -1;
        return 0;
    }
}