using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    HashSet<Vector2Int> validTiles = new HashSet<Vector2Int>();

    Dictionary<Vector2Int, MagneticCube> cubes = new Dictionary<Vector2Int, MagneticCube>();
    Dictionary<Vector2Int, OperatorPickup> operators = new Dictionary<Vector2Int, OperatorPickup>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void RegisterTile(Vector3 worldPos)
    {
        Vector2Int cell = WorldToGrid(worldPos);
        validTiles.Add(cell);
    }

    public bool IsValidCell(Vector2Int cell)
    {
        return validTiles.Contains(cell);
    }


    public void RegisterCube(Vector2Int cell, MagneticCube cube)
    {
        if (!cubes.ContainsKey(cell))
            cubes.Add(cell, cube);
    }

    public void RegisterCube(MagneticCube cube, Vector3 worldPos)
    {
        Vector2Int cell = WorldToGrid(worldPos);

        if (!cubes.ContainsKey(cell))
            cubes.Add(cell, cube);
    }

    public void UnregisterCube(Vector3 worldPos)
    {
        Vector2Int cell = WorldToGrid(worldPos);

        if (cubes.ContainsKey(cell))
            cubes.Remove(cell);
    }

    public bool IsCubeAt(Vector2Int cell)
    {
        return cubes.ContainsKey(cell);
    }

    public MagneticCube GetCubeAt(Vector2Int cell)
    {
        cubes.TryGetValue(cell, out MagneticCube cube);
        return cube;
    }


    public void RegisterOperator(Vector2Int cell, OperatorPickup op)
    {
        if (!operators.ContainsKey(cell))
            operators.Add(cell, op);
    }

    public void RegisterOperator(OperatorPickup op, Vector3 worldPos)
    {
        Vector2Int cell = WorldToGrid(worldPos);

        if (!operators.ContainsKey(cell))
            operators.Add(cell, op);
    }

    public void UnregisterOperator(Vector3 worldPos)
    {
        Vector2Int cell = WorldToGrid(worldPos);

        if (operators.ContainsKey(cell))
            operators.Remove(cell);
    }

    public bool IsOperatorAt(Vector2Int cell)
    {
        return operators.ContainsKey(cell);
    }

    public OperatorPickup GetOperatorAt(Vector2Int cell)
    {
        operators.TryGetValue(cell, out OperatorPickup op);
        return op;
    }


    Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.z)
        );
    }
}
