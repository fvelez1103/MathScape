using UnityEngine;

public class BaldosaData : MonoBehaviour
{
    // Dimensiones: (1,1) para cuadritos, (1,3) para barras, (3,3) para el grande
    public Vector2 dimensions = new Vector2(1, 1); 
    
    private SpriteRenderer sr;
    private Color originalColor;
    private int originalOrder;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        originalOrder = sr.sortingOrder;
    }

    public void Select()
    {
        sr.color = Color.red; // Color de selección
        sr.sortingOrder = 100;   // Traer al frente
    }

    public void Deselect()
    {
        sr.color = originalColor;
        sr.sortingOrder = originalOrder;
    }

    public void Move(Vector2 direction)
    {
        // Mueve la pieza 1 unidad en la dirección deseada
        transform.position += (Vector3)direction;
    }
}