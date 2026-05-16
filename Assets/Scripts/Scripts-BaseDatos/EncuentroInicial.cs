using System.Collections;
using UnityEngine;

public class EncuentroInicial : MonoBehaviour
{
    [Header("Conexiones")]
    public MonoBehaviour scriptMovimientoJugador;
    public GestorFormulario formularioUI; 

    private bool yaSePresento = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !yaSePresento)
        {
            StartCoroutine(SecuenciaPresentacion());
        }
    }

    IEnumerator SecuenciaPresentacion()
    {
        yaSePresento = true;

        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        
        Rigidbody2D rb = scriptMovimientoJugador.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f);

        if (formularioUI != null)
        {
            formularioUI.MostrarFormulario();
        }
    }
}