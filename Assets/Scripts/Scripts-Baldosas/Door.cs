using UnityEngine;

public class Door : MonoBehaviour
{
    public void Open()
    {
        // Simplemente desactivamos la puerta para poder pasar
        // O podrías animarla
        gameObject.SetActive(false);
        Debug.Log("¡La puerta se ha abierto!");
    }
}