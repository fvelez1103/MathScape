using UnityEngine;

public class LockTile : MonoBehaviour
{
    public int requiredValue = 4;
    public bool isUnlocked = false;

    void Start()
    {
        Debug.Log($"Candado creado con valor {requiredValue}");
    }

    public void Unlock()
    {
        if (isUnlocked)
        {
            Debug.Log("ya estaba desbloqueado");
            return;
        }

        isUnlocked = true;
        Debug.Log($"Candado desbloqueado (valor {requiredValue})");
        gameObject.SetActive(false);
    }

    public void Relock()
    {
        isUnlocked = false;
        Debug.Log($"candado restaurado (valor {requiredValue})");
        gameObject.SetActive(true);
    }
}
