using UnityEngine;

public class GestorArranque : MonoBehaviour
{
    [Header("Conexión con la Memoria")]
    public MemoriaDelJuego memoria;

    void Awake()
    {
        if (memoria != null)
        {
            memoria.ReiniciarMemoria();
            Debug.Log("¡Memoria reseteada! Partida nueva y limpia.");
        }
    }
}