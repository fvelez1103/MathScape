using UnityEngine;

public class TextoAyudaUi : MonoBehaviour
{
    [Header("UI Local de este Nivel")]
    [Tooltip("Arrastra aquí el panel de texto de ayuda de ESTA escena.")]
    public GameObject panelAyudaLocal;

    private void Start()
    {
        // 1. Nos suscribimos para escuchar los gritos del Cerebro (DDA)
        DDA_DifusoPuro.OnCambioEstado += EvaluarAyuda;

        // 2. Revisamos cómo viene el jugador nada más cargar este nivel
        if (DDA_DifusoPuro.Instancia != null)
        {
            EvaluarAyuda(DDA_DifusoPuro.Instancia.estadoActual);
        }
    }

    private void OnDestroy()
    {
        // Vital: Si este nivel se destruye, dejamos de escuchar al DDA
        DDA_DifusoPuro.OnCambioEstado -= EvaluarAyuda;
    }

    private void EvaluarAyuda(EstadoJugador estadoActual)
    {
        if (panelAyudaLocal == null) return;

        if (estadoActual == EstadoJugador.Bloqueado || estadoActual == EstadoJugador.Frustrado)
        {
            panelAyudaLocal.SetActive(true);
        }
        else
        {
            panelAyudaLocal.SetActive(false);
        }
    }
}