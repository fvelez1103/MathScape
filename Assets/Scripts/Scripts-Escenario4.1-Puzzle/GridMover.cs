using UnityEngine;
using System.Collections.Generic;

public class GridMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private GridDetector gridDetector;

    private bool isMoving;

    public bool IsMoving => isMoving;

    public bool TryMove(Vector3 direction, List<Transform> magneticChain)
    {
        if (isMoving) return false;

        Vector3 target = transform.position + direction;

        // Validar suelo del jugador
        if (!gridDetector.HasGridAt(target))
            return false;

        // Validar suelo de TODA la cadena
        foreach (Transform t in magneticChain)
        {
            Vector3 chainTarget = t.position + direction;
            if (!gridDetector.HasGridAt(chainTarget))
                return false;
        }

        StartCoroutine(MoveRoutine(direction, magneticChain));
        return true;
    }

    private System.Collections.IEnumerator MoveRoutine(Vector3 dir, List<Transform> chain)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 end = start + dir;

        Dictionary<Transform, Vector3> chainStart = new();
        foreach (Transform t in chain)
            chainStart[t] = t.position;

        float tLerp = 0f;
        while (tLerp < 1f)
        {
            tLerp += Time.deltaTime * moveSpeed;

            transform.position = Vector3.Lerp(start, end, tLerp);

            foreach (var kvp in chainStart)
                kvp.Key.position = Vector3.Lerp(kvp.Value, kvp.Value + dir, tLerp);

            yield return null;
        }

        // 🟢 SOLUCIÓN: Asignamos la posición exacta final en lugar de redondear a un entero absoluto
        transform.position = end;

        foreach (var kvp in chainStart)
        {
            // Las cajas de la cadena también terminan en su destino relativo exacto
            kvp.Key.position = kvp.Value + dir;
        }

        isMoving = false;
    }

    public bool IsValidMove(Vector3 direction, IReadOnlyList<Transform> magneticChain)
    {
        if (isMoving) return false;

        Vector3 target = transform.position + direction;

        if (!gridDetector.HasGridAt(target)) return false;

        foreach (Transform t in magneticChain)
        {
            Vector3 chainTarget = t.position + direction;
            if (!gridDetector.HasGridAt(chainTarget)) return false;
        }

        return true;
    }
}