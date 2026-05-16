using UnityEngine;

public class TriviaScreen : MonoBehaviour
{
    [Header("Configuración de Trivia")]
    [SerializeField] public string dificultadDeseada = "Medio"; 
    [SerializeField] private GameObject marcadorInteraccion;
    [SerializeField] private CanvasTriviaController canvasTrivia;

    [Header("Consecuencias")]
    public GameObject sueloADesaparecer;
    public Transform tuberiaARotar; 
    public Vector3 rotacionFinalTuberia;

    [Header("Sonidos")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoRespuestaCorrecta;
    [SerializeField] private AudioClip sonidoSueloCae;
    [SerializeField] private AudioClip sonidoTuberiaRota;
    [Range(0f, 1f)] public float volumenGlobal = 1f;
    // ---------------------------------------------------------

    private bool isPlayerInRange;
    public bool yaFueResueltaCorrectamente = false; 
    private int idPreguntaActual;
    private float tiempoInicioPregunta; 
    
    private bool ayuda5050Activada = false;
    private int fallosEnEstaPregunta = 0;
    private float tiempoAyudaDinamico = 45f;
    
    private bool estaTriviaEnUso = false;

    [System.Obsolete]
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !canvasTrivia.gameObject.activeSelf && !yaFueResueltaCorrectamente)
        {
            AbrirTrivia();
        }

        if (estaTriviaEnUso && !yaFueResueltaCorrectamente && !ayuda5050Activada)
        {
            float tiempoTranscurrido = Time.unscaledTime - tiempoInicioPregunta;
            if (tiempoTranscurrido >= tiempoAyudaDinamico)
            {
                ActivarAyudaAutomatica();
            }
        }
    }

    [System.Obsolete]
    private void AbrirTrivia()
    {
        PreguntaTrivia pregunta = TriviaManager.Instancia.ObtenerPregunta(dificultadDeseada);

        if (pregunta != null)
        {
            if (marcadorInteraccion != null) marcadorInteraccion.SetActive(false);
            
            if (!int.TryParse(pregunta.id, out idPreguntaActual)) {
                idPreguntaActual = 0;
            }

            fallosEnEstaPregunta = 0;
            ayuda5050Activada = false;
            estaTriviaEnUso = true; 

            if (DDA_TriviaTuberias.Instancia != null) {
                tiempoAyudaDinamico = DDA_TriviaTuberias.Instancia.ObtenerTiempoParaAyuda();
            }

            IniciarCronometroPregunta();

            canvasTrivia.ConfigurarTrivia(pregunta, this);
            canvasTrivia.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No se encontraron preguntas para la dificultad: " + dificultadDeseada);
        }
    }

    private void ActivarAyudaAutomatica()
    {
        ayuda5050Activada = true;
        Debug.Log($"<color=yellow>DDA Trivia:</color> Tiempo excedido ({tiempoAyudaDinamico:F1}s). Activando 50/50.");
        canvasTrivia.Aplicar5050(); 
    }

    public void RegistrarIntento(bool fueCorrecto, string textoElegido)
    {
        if (DDA_TriviaTuberias.Instancia != null)
        {
            DDA_TriviaTuberias.Instancia.usoAyuda5050 = ayuda5050Activada;
        }

        if (fueCorrecto)
        {
            if (audioSource != null)
            {
                if (sonidoRespuestaCorrecta != null) audioSource.PlayOneShot(sonidoRespuestaCorrecta, volumenGlobal);
                if (sonidoTuberiaRota != null) audioSource.PlayOneShot(sonidoTuberiaRota, volumenGlobal);
            }

            float tiempoFinalCrudo = Time.unscaledTime - tiempoInicioPregunta;
            float tiempoFinalRedondeado = (float)System.Math.Round(tiempoFinalCrudo, 2);

            if (DDA_TriviaTuberias.Instancia != null)
            {
                DDA_TriviaTuberias.Instancia.tiempoUltimaPregunta = tiempoFinalRedondeado;
                DDA_TriviaTuberias.Instancia.fallosEnPregunta = fallosEnEstaPregunta;
                
                string proximaDif = DDA_TriviaTuberias.Instancia.CalcularSiguienteDificultad(dificultadDeseada);
                ActualizarDificultadFutura(proximaDif);
            }

            FinalizarPregunta(idPreguntaActual, true, textoElegido);
            MarcarComoResuelta();
            
            estaTriviaEnUso = false; 
            canvasTrivia.gameObject.SetActive(false);
            Time.timeScale = 1f; 
        }
        else
        {
            if (audioSource != null && sonidoSueloCae != null)
            {
                audioSource.PlayOneShot(sonidoSueloCae, volumenGlobal);
            }

            fallosEnEstaPregunta++;
            if (DDA_TriviaTuberias.Instancia != null)
            {
                DDA_TriviaTuberias.Instancia.fallosEnPregunta = fallosEnEstaPregunta;
            }
            Debug.Log("<color=red>DDA Trivia:</color> Fallo registrado. Total fallos: " + fallosEnEstaPregunta);
            
            FinalizarPregunta(idPreguntaActual, false, textoElegido);
        }
    }

    private void ActualizarDificultadFutura(string nuevaDif)
    {
        TriviaScreen[] todas = Object.FindObjectsByType<TriviaScreen>(FindObjectsSortMode.None);
        foreach (var t in todas)
        {
            if (!t.yaFueResueltaCorrectamente)
            {
                t.dificultadDeseada = nuevaDif;
            }
        }
    }

    public void IniciarCronometroPregunta() {
        tiempoInicioPregunta = Time.unscaledTime; 
        Debug.Log($"<color=green>M5:</color> Cronómetro iniciado para pregunta {idPreguntaActual}");
    }

    public void FinalizarPregunta(int id, bool esCorrecta, string elegida) {
        float tiempoTranscurridoCrudo = Time.unscaledTime - tiempoInicioPregunta;
        float tiempoRedondeado = (float)System.Math.Round(tiempoTranscurridoCrudo, 2);
        if (tiempoRedondeado <= 0) tiempoRedondeado = 0.01f;

        if (FirebaseManager.Instancia != null) {
            FirebaseManager.Instancia.M5_RegistrarPregunta(id, tiempoRedondeado, esCorrecta, elegida);
        }
    }

    public void MarcarComoResuelta() 
    {
        if (!yaFueResueltaCorrectamente)
        {
            yaFueResueltaCorrectamente = true;
            if (LevelProgresoManager.Instancia != null) {
                LevelProgresoManager.Instancia.RegistrarTriviaCompletada();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaFueResueltaCorrectamente)
        {
            isPlayerInRange = true;
            if (marcadorInteraccion != null) marcadorInteraccion.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (marcadorInteraccion != null) marcadorInteraccion.SetActive(false);
        }
    }
}