using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaMatematica : MonoBehaviour
{
    [Header("Configuración (M6)")]
    public int nivelID;
    public int puertaID; 

    [Header("¿Es la puerta ganadora?")]
    public bool esRespuestaCorrecta = false;

    [Header("Flujo de Escenas")]
    [Tooltip("Si está marcado, el DDA decide el siguiente nivel (Solo para el laberinto 5.1).")]
    public bool decidirDificultadConDDA = false;

    [Tooltip("El nombre de la escena de la historia a la que debe ir después del laberinto.")]
    public string nombreEscenaSiguiente; 

    [Header("Animaciones")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 1f;

    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoCorrecta;
    public AudioClip sonidoIncorrecta;
    [Range(0f, 1f)] public float volumenSonido = 1f;

    private bool isTransitioning = false;
    
    private float tiempoInicioNivel;

    private void OnEnable() {
        DDA_Laberinto.OnActivar5050 += Ejecutar5050;
    }

    private void OnDisable() {
        DDA_Laberinto.OnActivar5050 -= Ejecutar5050;
    }

    private void Start()
    {
        tiempoInicioNivel = Time.time;

        if (DDA_Laberinto.Instancia != null && DDA_Laberinto.Instancia.ayuda5050Activa)
        {
            Ejecutar5050();
        }
    }

    private void Ejecutar5050() 
    {
        if (!esRespuestaCorrecta && puertaID % 2 == 0) 
        {
            gameObject.SetActive(false);
            Debug.Log($"DDA: Ocultando puerta incorrecta {gameObject.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            // Primero, nos aseguramos de que el jugador no esté ya muerto (por si chocan al mismo milisegundo)
            Player scriptJugador = collision.GetComponent<Player>();
            if (scriptJugador != null && scriptJugador.estaMuerto) return;

            isTransitioning = true; 

            // --- LA SOLUCIÓN: CONGELAR AL VILLANO ---
            RastreadorSombra villanoSombra = Object.FindAnyObjectByType<RastreadorSombra>();
            if (villanoSombra != null)
            {
                villanoSombra.DetenerSombra();
            }
            // ----------------------------------------

            if (esRespuestaCorrecta)
            {
                if (fuenteSonido != null && sonidoCorrecta != null)
                {
                    fuenteSonido.PlayOneShot(sonidoCorrecta, volumenSonido);
                }
                
                StartCoroutine(SceneLoad());
            }
            else
            {
                StartCoroutine(RutinaPuertaIncorrecta());
            }
        }
    }

    private IEnumerator RutinaPuertaIncorrecta()
    {
        float tiempoEspera = 1.0f; 

        if (fuenteSonido != null && sonidoIncorrecta != null)
        {
            fuenteSonido.PlayOneShot(sonidoIncorrecta, volumenSonido);
            tiempoEspera = sonidoIncorrecta.length + 0.1f;
        }

        if (DDA_Laberinto.Instancia != null)
        {
            DDA_Laberinto.Instancia.RegistrarErrorPuerta();
        }
        
        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.M6_RegistrarMuertePuerta(nivelID);
        }

        yield return new WaitForSecondsRealtime(tiempoEspera);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator SceneLoad()
    {
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("FadeOut");

        float tiempoTotal = Time.time - tiempoInicioNivel;
        float tiempoRedondeado = (float)System.Math.Round(tiempoTotal, 2);

        float esperaFinal = transitionTime;
        if (sonidoCorrecta != null)
        {
            esperaFinal = Mathf.Max(transitionTime, sonidoCorrecta.length);
        }

        yield return new WaitForSeconds(esperaFinal);
        
        if (FirebaseManager.Instancia != null) {
            FirebaseManager.Instancia.M6_FinalizarLaberinto(nivelID, tiempoRedondeado);
            FirebaseManager.Instancia.M6_CapturarDDA(nivelID);
            Debug.Log($"<color=lime>Firebase M6:</color> Laberinto {nivelID} completado en {tiempoRedondeado}s.");
        }

        if (decidirDificultadConDDA && DDA_Laberinto.Instancia != null)
        {
            string siguienteLaberinto = DDA_Laberinto.Instancia.DeterminarSiguienteLaberinto();
            SceneManager.LoadScene(siguienteLaberinto);
        }
        else
        {
            if (!string.IsNullOrEmpty(nombreEscenaSiguiente))
            {
                SceneManager.LoadScene(nombreEscenaSiguiente);
            }
            else
            {
                Debug.LogError("¡Ojo! No pusiste el nombre de la escena de historia en el Inspector.");
            }
        }
    }
}