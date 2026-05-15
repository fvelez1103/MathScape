using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PreguntaQuiz
{
    public int ID;
    public string Tema;
    public string PreguntaTexto;
    public string[] Opciones;
    public int IndiceCorrecto;
}

public class GestorQuizFinal : MonoBehaviour
{
    [Header("Conexiones UI - Panel Quiz")]
    public GameObject panelQuiz;
    public TextMeshProUGUI textoPregunta;
    public TextMeshProUGUI[] textosBotones; // Los 4 textos de las opciones
    public Button[] botonesOpciones;        // Los 4 botones

    [Header("Conexiones UI - Panel Resultados")]
    public GameObject panelResultados;
    public TextMeshProUGUI textoPuntuacionFinal;
    public Button botonFinalizar; 

    [Header("Colores de Feedback")]
    public Color colorNormal = Color.white;
    public Color colorCorrecto = Color.green;
    public Color colorIncorrecto = Color.red;

    // --- NUEVO: EFECTOS DE SONIDO ---
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;
    [Range(0f, 1f)] public float volumenSonido = 1f;
    // --------------------------------

    private List<PreguntaQuiz> todasLasPreguntas = new List<PreguntaQuiz>();
    private List<PreguntaQuiz> preguntasSeleccionadas = new List<PreguntaQuiz>();
    
    private int indicePreguntaActual = 0;
    private int respuestasCorrectas = 0;
    private bool esperandoSiguiente = false;
    private float tiempoInicioPregunta;
    
    private bool enviandoDatos = false; // <--- CANDADO ANTI-SPAM AÑADIDO

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        CargarCSV();
        SeleccionarPreguntasInteligentes();
        
        panelResultados.SetActive(false);
        panelQuiz.SetActive(true);

        if (botonFinalizar != null) botonFinalizar.interactable = true;
        
        MostrarPregunta(0);
    }

    void CargarCSV()
    {
        TextAsset archivoCSV = Resources.Load<TextAsset>("preguntasQuiz");
        if (archivoCSV == null)
        {
            Debug.LogError("No se encontró el archivo CSV. ¿Está en la carpeta Resources?");
            return;
        }

        string[] lineas = archivoCSV.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lineas.Length; i++)
        {
            string[] columnas = lineas[i].Split(';');
            if (columnas.Length >= 8) 
            {
                PreguntaQuiz p = new PreguntaQuiz();
                p.ID = int.Parse(columnas[0]);
                p.Tema = columnas[1];
                p.PreguntaTexto = columnas[2];
                p.Opciones = new string[] { columnas[3], columnas[4], columnas[5], columnas[6] };
                p.IndiceCorrecto = int.Parse(columnas[7]);
                
                todasLasPreguntas.Add(p);
            }
        }
    }

    void SeleccionarPreguntasInteligentes()
    {
        var gruposPorTema = todasLasPreguntas.GroupBy(p => p.Tema).ToList();

        foreach (var grupo in gruposPorTema)
        {
            var preguntasMezcladas = grupo.OrderBy(x => Guid.NewGuid()).ToList();
            preguntasSeleccionadas.Add(preguntasMezcladas[0]);
        }

        if (preguntasSeleccionadas.Count < 5)
        {
            var restantes = todasLasPreguntas.Except(preguntasSeleccionadas).OrderBy(x => Guid.NewGuid()).ToList();
            int faltantes = 5 - preguntasSeleccionadas.Count;
            preguntasSeleccionadas.AddRange(restantes.Take(faltantes));
        }

        preguntasSeleccionadas = preguntasSeleccionadas.OrderBy(x => Guid.NewGuid()).ToList();
    }

    void MostrarPregunta(int indice)
    {
        RestaurarBotones();
        esperandoSiguiente = false;
        tiempoInicioPregunta = Time.unscaledTime;

        PreguntaQuiz pActual = preguntasSeleccionadas[indice];
        textoPregunta.text = pActual.PreguntaTexto;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            textosBotones[i].text = pActual.Opciones[i];
            
            int indiceOpcion = i; 
            botonesOpciones[i].onClick.RemoveAllListeners();
            botonesOpciones[i].onClick.AddListener(() => EvaluarRespuesta(indiceOpcion));
        }
    }

    public void EvaluarRespuesta(int indiceElegido)
    {
        if (esperandoSiguiente) return; 
        esperandoSiguiente = true;

        PreguntaQuiz pActual = preguntasSeleccionadas[indicePreguntaActual];
        bool esCorrecta = (indiceElegido == pActual.IndiceCorrecto);
        float tiempoTardado = Time.unscaledTime - tiempoInicioPregunta;
        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.RegistrarPreguntaQuizFinal(
                pActual.ID, 
                pActual.PreguntaTexto, 
                pActual.Opciones[indiceElegido], 
                esCorrecta, 
                tiempoTardado
            );
        }

        if (esCorrecta)
        {
            respuestasCorrectas++;
            botonesOpciones[indiceElegido].GetComponent<Image>().color = colorCorrecto;
            
            // --- REPRODUCCIÓN SONIDO ÉXITO ---
            if (fuenteSonido != null && sonidoCorrecto != null)
            {
                fuenteSonido.PlayOneShot(sonidoCorrecto, volumenSonido);
            }
        }
        else
        {
            botonesOpciones[indiceElegido].GetComponent<Image>().color = colorIncorrecto;
            botonesOpciones[pActual.IndiceCorrecto].GetComponent<Image>().color = colorCorrecto;
            
            // --- REPRODUCCIÓN SONIDO ERROR ---
            if (fuenteSonido != null && sonidoIncorrecto != null)
            {
                fuenteSonido.PlayOneShot(sonidoIncorrecto, volumenSonido);
            }
        }

        StartCoroutine(PasarSiguientePregunta());
    }

    IEnumerator PasarSiguientePregunta()
    {
        yield return new WaitForSeconds(1.5f); 

        indicePreguntaActual++;

        if (indicePreguntaActual < preguntasSeleccionadas.Count)
        {
            MostrarPregunta(indicePreguntaActual);
        }
        else
        {
            MostrarResultadosFinales();
        }
    }

    void MostrarResultadosFinales()
    {
        panelQuiz.SetActive(false);
        panelResultados.SetActive(true);
        textoPuntuacionFinal.text = $"Obtuviste {respuestasCorrectas} de 5 respuestas correctas.";

        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.FijarPuntuacionQuizFinal(respuestasCorrectas);
        }
    }

    void RestaurarBotones()
    {
        foreach (var btn in botonesOpciones)
        {
            btn.GetComponent<Image>().color = colorNormal;
        }
    }

    public void FinalizarTodoElJuego()
    {
        if (enviandoDatos) return; // <--- REBOTA CLICS ADICIONALES
        enviandoDatos = true;      // <--- CIERRA EL CANDADO

        StartCoroutine(RutinaFinalizarJuego());
    }

    IEnumerator RutinaFinalizarJuego()
    {
        if (botonFinalizar != null) botonFinalizar.interactable = false;

        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.EnviarDatosFinales();
            Debug.Log("<color=green>Subiendo toda la tesis (DDA + Quiz) a la nube... Esperando confirmación.</color>");
        }
        
        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene("Escenario8-Creditos"); 
    }
}