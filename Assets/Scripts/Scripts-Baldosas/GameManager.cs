using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool CheckSolution(List<BaldosaData> tiles)
    {
        if (tiles.Count == 0) return false;

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;
        float totalTileArea = 0;

        foreach (var tile in tiles)
        {
            
            Vector3 pos = tile.transform.position;
            Vector2 dim = tile.dimensions;

            float halfWidth = dim.x / 2f;
            float halfHeight = dim.y / 2f;

            float left = pos.x - halfWidth;
            float right = pos.x + halfWidth;
            float bottom = pos.y - halfHeight;
            float top = pos.y + halfHeight;

            if (left < minX) minX = left;
            if (bottom < minY) minY = bottom;
            if (right > maxX) maxX = right;
            if (top > maxY) maxY = top;

            totalTileArea += (dim.x * dim.y);
        }

        float boundingWidth = maxX - minX;
        float boundingHeight = maxY - minY;

        float currentArea = Mathf.Round(boundingWidth * boundingHeight);
        float expectedArea = Mathf.Round(totalTileArea);

        return Mathf.Approximately(currentArea, expectedArea);
    }
}