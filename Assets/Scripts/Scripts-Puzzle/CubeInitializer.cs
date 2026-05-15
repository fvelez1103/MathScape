using UnityEngine;

public class CubeInitializer : MonoBehaviour
{
    void Start()
    {
        Vector2Int cell = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );

        GridManager.Instance.RegisterCube(cell, GetComponent<MagneticCube>());
    }
}
