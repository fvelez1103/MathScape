using UnityEngine;

public class Door : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(false);
        Debug.Log("¡La puerta se ha abierto!");
    }
}