using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalizarSesionTesis : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre exacto de tu escena de menú principal")]
    public string nombreEscenaMenu = "MenuInicio";

    private bool enviandoDatos = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !enviandoDatos)
        {
            enviandoDatos = true;
            StartCoroutine(TerminarSesionSeguro());
        }
    }
    IEnumerator TerminarSesionSeguro()
    {
        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.EnviarDatosFinales();
            Debug.Log("<color=green><b>DDA:</b> Iniciando subida a la nube... Esperando confirmación de red.</color>");
        }
        else
        {
            Debug.LogError("No se encontró el FirebaseManager para enviar los datos.");
        }
        yield return new WaitForSeconds(2f);

        Debug.Log("<color=cyan><b>DDA:</b> Tiempo de gracia cumplido. Cargando menú.</color>");
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}