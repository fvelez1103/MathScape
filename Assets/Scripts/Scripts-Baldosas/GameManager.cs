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
            // --- CAMBIO CLAVE ---
            // Ya no usamos el Collider. Usamos la posición lógica y las dimensiones definidas.
            // Esto evita errores si el collider es un poco más pequeño (0.9) o si la física tiembla.
            
            Vector3 pos = tile.transform.position;
            Vector2 dim = tile.dimensions;

            // Calculamos los bordes teóricos (Izquierda, Derecha, Abajo, Arriba)
            // Asumimos que el pivote está en el centro (0.5 de distancia al borde si dimension es 1)
            float halfWidth = dim.x / 2f;
            float halfHeight = dim.y / 2f;

            float left = pos.x - halfWidth;
            float right = pos.x + halfWidth;
            float bottom = pos.y - halfHeight;
            float top = pos.y + halfHeight;

            // Actualizamos los límites del "Rectángulo Contenedor"
            if (left < minX) minX = left;
            if (bottom < minY) minY = bottom;
            if (right > maxX) maxX = right;
            if (top > maxY) maxY = top;

            // Sumamos el área lógica de esta pieza
            totalTileArea += (dim.x * dim.y);
        }

        // Calculamos el área del gran rectángulo que envuelve todo
        float boundingWidth = maxX - minX;
        float boundingHeight = maxY - minY;

        // Usamos Round para forzar números enteros (ej: 2.9999 -> 3)
        float currentArea = Mathf.Round(boundingWidth * boundingHeight);
        float expectedArea = Mathf.Round(totalTileArea);

        // DEBUG: Útil si vuelve a fallar
        // Debug.Log($"Área Rectángulo Envolvente: {currentArea} | Suma Área Piezas: {expectedArea}");

        // Si el área del rectángulo que envuelve todo es IGUAL a la suma de las piezas,
        // significa que no hay huecos vacíos dentro del rectángulo.
        return Mathf.Approximately(currentArea, expectedArea);
    }
}