using UnityEngine;

public class GridDetector : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Asegúrate de asignar aquí la capa 'GridCell' en el Inspector")]
    [SerializeField] private LayerMask gridLayer;

    public bool HasGridAt(Vector3 targetPosition)
    {
        // En 2D puro, ignoramos la Z. Convertimos el Vector3 a Vector2.
        Vector2 checkPoint = new Vector2(targetPosition.x, targetPosition.y);
        
        // OverlapPoint revisa si hay un colisionador exactamente en ese punto (el centro de la baldosa)
        Collider2D hit = Physics2D.OverlapPoint(checkPoint, gridLayer);
        
        return hit != null;
    }

    // 🟢 Extra: Dibujamos un pequeño punto verde en la escena para depurar
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}