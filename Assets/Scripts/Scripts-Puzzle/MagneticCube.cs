using UnityEngine;

public class MagneticCube : MonoBehaviour
{
    public Vector2Int gridPos;
    public bool isAttached = false;
    public int value = 2;


    public void SetGridPosition(Vector2Int pos)
    {
        gridPos = pos;
        transform.position = new Vector3(pos.x, transform.position.y, pos.y);
    }

    public Vector2Int GridPos => gridPos;
}
