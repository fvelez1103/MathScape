using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private int value;

    public int Value => value;

    private void Awake()
    {
        // 🟢 MIGRACIÓN 2D: BoxCollider2D en lugar de BoxCollider
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }
}