using UnityEngine;

public class PuntoAparicion : MonoBehaviour
{
    [Tooltip("El ID de esta puerta. (Ej: PuertaRegreso1)")]
    public string idDeEstaPuerta;

    void Start()
    {
        if (MemoriaNiveles.puertaDestino == idDeEstaPuerta)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            
            if (jugador != null)
            {
                jugador.SetActive(false); 
                
                jugador.transform.position = transform.position;
                
                jugador.SetActive(true);
                
                MemoriaNiveles.puertaDestino = "";
            }
        }
    }
}