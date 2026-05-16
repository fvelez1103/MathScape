using UnityEngine;

public class TextoAyudaUi : MonoBehaviour
{
    [Header("UI Local de este Nivel")]
    [Tooltip("Arrastra aquí el panel de texto de ayuda de ESTA escena.")]
    public GameObject panelAyudaLocal;

    private void Start()
    {
        DDA_DifusoPuro.OnCambioEstado += EvaluarAyuda;

        if (DDA_DifusoPuro.Instancia != null)
        {
            EvaluarAyuda(DDA_DifusoPuro.Instancia.estadoActual);
        }
    }

    private void OnDestroy()
    {
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