using UnityEngine;

public class ControlMuroAyuda : MonoBehaviour
{
    [Header("Configuración de Ayuda")]
    [Tooltip("Si es true, solo se activará si el jugador se ha caído 3 o más veces.")]
    public bool esProteccionContraCaidas = true;

    [Tooltip("El objeto físico o visual que se activará (muro, texto, partículas)")]
    public GameObject visualMuro; 

    private void OnEnable()
    {
        DDA_DifusoPuro.OnCambioEstado += ActualizarEstadoMuro;
    }

    private void OnDisable()
    {
        DDA_DifusoPuro.OnCambioEstado -= ActualizarEstadoMuro;
    }

    private void Start()
    {
        if (DDA_DifusoPuro.Instancia != null)
        {
            ActualizarEstadoMuro(DDA_DifusoPuro.Instancia.estadoActual);
        }
    }

    private void ActualizarEstadoMuro(EstadoJugador nuevoEstado)
    {
        bool debeEstarActivo = false;

        // --- LÓGICA CONTEXTUAL PARA LA TESIS ---
        if (esProteccionContraCaidas)
        {
            // Las PAREDES solo se activan en estado Crítico (Bloqueado) 
            // Y SI el jugador realmente se está cayendo (3 o más caídas).
            if (nuevoEstado == EstadoJugador.Bloqueado && DDA_DifusoPuro.Instancia.caidasAlVacio >= 3)
            {
                debeEstarActivo = true;
            }
        }
        else
        {
            // Las PISTAS (Textos/Brillos) se activan normalmente 
            // cuando el estado deja de ser Óptimo (por tiempo, resets o fallos).
            if (nuevoEstado != EstadoJugador.Optimo)
            {
                debeEstarActivo = true;
            }
        }

        // Aplicar el estado al objeto
        if (visualMuro != null)
        {
            visualMuro.SetActive(debeEstarActivo);
        }
        else
        {
            gameObject.SetActive(debeEstarActivo);
        }
    }
}