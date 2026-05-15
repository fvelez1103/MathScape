using UnityEngine;

public class PalancaControl : MonoBehaviour
{
    public GameObject puente;
    public VillanoIA villano;     
    public GameObject promptE; 

    // --- NUEVO: EFECTOS DE SONIDO ---
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoPalanca;
    [Range(0f, 1f)] public float volumenSonido = 1f;
    // --------------------------------
    
    private bool jugadorCerca = false;
    private bool palancaAccionada = false;

    void Update()
    {
        if (jugadorCerca && !palancaAccionada && Input.GetKeyDown(KeyCode.E))
        {
            Object.FindAnyObjectByType<ContadorCorrutina>().MostrarCaidaPuente();
            AccionarPalanca();
        }
    }

    void AccionarPalanca()
    {
        palancaAccionada = true;

        // --- REPRODUCIR SONIDO DE LA PALANCA ---
        if (fuenteSonido != null && sonidoPalanca != null)
        {
            fuenteSonido.PlayOneShot(sonidoPalanca, volumenSonido);
        }

        if (puente != null) puente.SetActive(false); 
        
        Object.FindAnyObjectByType<ContadorCorrutina>().RegistrarPalancaActivada();
        
        if(promptE) promptE.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !palancaAccionada)
        {
            jugadorCerca = true;
            if(promptE) promptE.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if(promptE) promptE.SetActive(false);
        }
    }
}