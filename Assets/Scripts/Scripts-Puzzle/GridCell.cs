using UnityEngine;

public class GridCell : MonoBehaviour
{
    void Start()
    {
        Vector2Int cell = Vector2Int.RoundToInt(
            new Vector2(transform.position.x, transform.position.z)
        );

       // GridOccupancy.Instance.RegisterValidCell(cell);
    }
}
