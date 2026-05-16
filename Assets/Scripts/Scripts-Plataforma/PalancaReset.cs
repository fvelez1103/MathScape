using UnityEngine;

public class PalancaReset : MonoBehaviour
{
    public GameObject panelConfirmacion;
    public cogerObjetos scriptJugador;
    private bool jugadorCerca = false;

    [System.Obsolete]
    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E) && !panelConfirmacion.activeSelf)
        {
            AbrirMenu();
        }

        if (panelConfirmacion.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.S)) ConfirmarReset();
            if (Input.GetKeyDown(KeyCode.N)) CerrarMenu();
        }
    }

    void AbrirMenu()
    {
        panelConfirmacion.SetActive(true);
    }

    public void CerrarMenu()
    {
        panelConfirmacion.SetActive(false);
    }

    [System.Obsolete]
    public void ConfirmarReset()
    {
        FindObjectOfType<ResetManager>().EjecutarRetorno();
        CerrarMenu();
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) jugadorCerca = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) jugadorCerca = false; }
}