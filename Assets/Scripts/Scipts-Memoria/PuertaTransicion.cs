using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaTransicion : MonoBehaviour
{
    [Header("Configuración del Viaje")]
    [Tooltip("El nombre EXACTO de la escena a la que quieres ir (Ej: Minijuego1)")]
    public string nombreEscenaDestino;
    
    [Tooltip("El ID de la puerta por la que vas a salir en la OTRA escena (Ej: PuertaRegreso1)")]
    public string idPuertaDestino;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MemoriaNiveles.puertaDestino = idPuertaDestino;
            
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}