using UnityEngine;

public class GridTile : MonoBehaviour
{
    void Start()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError("❌ GridManager no existe al registrar tile " + gameObject.name);
            return;
        }

        GridManager.Instance.RegisterTile(transform.position);
    }
}
