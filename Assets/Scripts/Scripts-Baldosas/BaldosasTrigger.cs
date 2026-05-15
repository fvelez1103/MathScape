using UnityEngine;
using UnityEngine.Events;

public class BaldosasTrigger : MonoBehaviour
{
    public UnityEvent OnLeverActivate; 
    private bool playerInRange = false;
    private bool puzzleFinalizado = false; 

    [Header("Métricas DDA")]
    public MetricasCuadrado scriptMetricas; 

    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoPalanca;
    [Range(0f, 1f)] public float volumenSonido = 1f;
    // --------------------------------

    void Update()
    {
        if (playerInRange && !puzzleFinalizado && Input.GetKeyDown(KeyCode.E))
        {
            if (fuenteSonido != null && sonidoPalanca != null)
            {
                fuenteSonido.PlayOneShot(sonidoPalanca, volumenSonido);
            }

            OnLeverActivate.Invoke();
            
            if (scriptMetricas != null)
            {
                scriptMetricas.IniciarCronometro();
            }
        }
    }

    public void DesactivarPalanca()
    {
        puzzleFinalizado = true;
        Debug.Log("<color=yellow>Palanca desactivada para siempre.</color>");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}