using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuenteHaciaQuiz : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre exacto de tu escena del Quiz Final")]
    public string nombreEscenaQuiz = "EscenaQuizFinal";

    private bool cruzando = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cruzando)
        {
            cruzando = true;
            StartCoroutine(ViajarAlQuiz());
        }
    }

    IEnumerator ViajarAlQuiz()
    {
        
        Debug.Log("<color=yellow><b>DDA:</b> Nivel final superado. Preservando datos y cargando Quiz...</color>");
        
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(nombreEscenaQuiz);
    }
}