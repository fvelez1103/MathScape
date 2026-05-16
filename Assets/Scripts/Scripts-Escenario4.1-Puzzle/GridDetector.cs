using UnityEngine;

public class GridDetector : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Asegúrate de asignar aquí la capa 'GridCell' en el Inspector")]
    [SerializeField] private LayerMask gridLayer;

    public bool HasGridAt(Vector3 targetPosition)
    {
        Vector2 checkPoint = new Vector2(targetPosition.x, targetPosition.y);
        
        Collider2D hit = Physics2D.OverlapPoint(checkPoint, gridLayer);
        
        return hit != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}