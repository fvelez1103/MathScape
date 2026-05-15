using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    private PlatformValueDisplay platform;
    private BalanzaModular balanza; 

    [Header("Configuración Tesis (M1)")]
    public int nivelID; 

    [Header("Visual Feedback")]
    public GameObject objetoFantasma;

    private void Awake()
    {
        platform = GetComponentInParent<PlatformValueDisplay>();
        balanza = Object.FindFirstObjectByType<BalanzaModular>();
    }

    public void ColocarObjeto(GameObject objeto)
    {
        objeto.transform.SetParent(transform);
        objeto.transform.localPosition = Vector3.zero;

        if (platform != null)
        {
            pesoObjeto scriptPeso = objeto.GetComponent<pesoObjeto>();
            int valorSuma = (scriptPeso != null) ? (int)scriptPeso.peso : 1;
            platform.AddValue(valorSuma);

            ValidarMovimientoIncorrecto();
        }

        if (objetoFantasma != null) objetoFantasma.SetActive(false);
    }

    private void ValidarMovimientoIncorrecto()
    {
        if (platform == null) return;

        if (platform.targetValue > 0)
        {
            int pesoActual = platform.GetValue();

            if (pesoActual > platform.targetValue)
            {
                if (FirebaseManager.Instancia != null)
                {
                    FirebaseManager.Instancia.M1_RegistrarErrorBloque(nivelID);
                }
                Debug.Log($"<color=red>M1 Error:</color> El jugador puso {pesoActual} superando el target de {platform.targetValue}.");
                if (DDA_DifusoPuro.Instancia != null)
                {
                    DDA_DifusoPuro.Instancia.RegistrarErrorMatematico();
                }
            }
        }
    }public void QuitarObjetoLogico(GameObject objeto)
    {
        if (platform != null)
        {
            pesoObjeto scriptPeso = objeto.GetComponent<pesoObjeto>();
            int valorResta = (scriptPeso != null) ? (int)scriptPeso.peso : 1;
            platform.AddValue(-valorResta);
        }
    }
    public void QuitarObjeto(GameObject objeto)
    {
        QuitarObjetoLogico(objeto);
        objeto.transform.SetParent(null);
    }
    public void ActivarFantasma(bool estado)
    {
        pesoObjeto tieneCuboReal = GetComponentInChildren<pesoObjeto>();
        if (objetoFantasma != null && tieneCuboReal == null) objetoFantasma.SetActive(estado);
        else if (objetoFantasma != null) objetoFantasma.SetActive(false); 
    }
}