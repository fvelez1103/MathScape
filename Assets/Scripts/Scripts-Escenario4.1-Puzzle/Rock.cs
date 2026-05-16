using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private int value;

    public int Value => value;

    private void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }
}