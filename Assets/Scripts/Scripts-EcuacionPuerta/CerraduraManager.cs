using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CerraduraManager : MonoBehaviour
{
    [Header("Configuración de Datos")]
    public EcuacionData datosEcuacion;

    [Header("Configuración de UI Principal")]
    public UnityEngine.UI.Image imagenEcuacionPrincipal; // Tu PNG en ContenedorEcuacion
    public TextMeshProUGUI textoExplicacion; 
    public TextMeshProUGUI textoPistaVisual; 

    [Header("Configuración de Sonidos")]
    public AudioSource fuenteAudio; 
    public AudioClip sonidoAcierto; 
    public AudioClip sonidoError; // Sonido para feedback negativo

    [Header("Configuración de Pasos UI")]
    public GameObject pasoPrefab;
    public Transform contenedorPasos;

    private List<PasoControlador> listaPasos = new List<PasoControlador>();
    private int indiceActivo = 0;
    private bool puertaAbierta = false;
    private int pasoMaximoAlcanzado = 0; 

    // Colores para el feedback
    private Color32 colorAzulOriginal = new Color32(62, 124, 177, 255);
    private Color32 colorErrorRojo = new Color32(200, 50, 50, 255);
    private Color32 colorAciertoVerde = new Color32(50, 200, 50, 255);

    private float tiempoEnPasoActual = 0f;
    private int fallosEnPasoActual = 0;

    public CerraduraTrigger activadorEscena;

    void Start()
    {
        if (textoExplicacion != null) textoExplicacion.text = "";
        if (textoPistaVisual != null) textoPistaVisual.text = "";

        ActualizarEcuacionVisual(0);
        GenerarPasos();
    }

   void Update()
    {
        if (puertaAbierta) return;

        // 🟢 NUEVO: Detección estricta de salida temporal
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activadorEscena != null)
            {
                activadorEscena.PausarMinijuego();
            }
            return; // Cortamos el Update aquí para que no registre nada más
        }

        tiempoEnPasoActual += Time.deltaTime; 
        ManejarNavegacion();
        ManejarEntradaTeclado();
    }

    void ActualizarEcuacionVisual(int indice)
    {
        if (datosEcuacion != null && imagenEcuacionPrincipal != null && datosEcuacion.imagenesProgreso != null)
        {
            if (indice >= 0 && indice < datosEcuacion.imagenesProgreso.Length)
            {
                imagenEcuacionPrincipal.sprite = datosEcuacion.imagenesProgreso[indice];
            }
        }
    }

    void GenerarPasos()
    {
        foreach (Transform child in contenedorPasos) Destroy(child.gameObject);
        listaPasos.Clear();

        for (int i = 0; i < datosEcuacion.acciones.Length; i++)
        {
            GameObject obj = Instantiate(pasoPrefab, contenedorPasos);
            obj.transform.localScale = Vector3.one;
            PasoControlador pc = obj.GetComponent<PasoControlador>();
            pc.ConfigurarPaso(i + 1, datosEcuacion.acciones[i], datosEcuacion.estadosIntermedios[i], datosEcuacion.explicacionesMatematicas[i]);
            listaPasos.Add(pc);
        }
        ResaltarPaso(0);
    }

    void ManejarNavegacion()
    {
        if (Input.GetKeyDown(KeyCode.W) && indiceActivo > 0)
        {
            CambiarDePaso(indiceActivo - 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) && indiceActivo < pasoMaximoAlcanzado)
        {
            CambiarDePaso(indiceActivo + 1);
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (PasoEstaResuelto(indiceActivo))
            {
                StartCoroutine(FeedbackVisual(true));
            }
            else 
            {
                StartCoroutine(FeedbackVisual(false));
                fallosEnPasoActual++;
                EvaluarAyuda();
            }
        }
    }

    System.Collections.IEnumerator FeedbackVisual(bool esAcierto)
    {
        PasoControlador paso = listaPasos[indiceActivo];
        
        if (esAcierto)
        {
            if (fuenteAudio != null && sonidoAcierto != null)
                fuenteAudio.PlayOneShot(sonidoAcierto);

            paso.fondoImagen.color = colorAciertoVerde;
            yield return new WaitForSeconds(0.3f);
            
            if (indiceActivo == pasoMaximoAlcanzado) pasoMaximoAlcanzado++; 
            
            paso.RevelarEstadoGris();
            MostrarExplicacionDelPaso(indiceActivo);

            if (indiceActivo < listaPasos.Count - 1)
            {
                CambiarDePaso(indiceActivo + 1);
            }
            else if (!puertaAbierta)
            {
                ActualizarEcuacionVisual(pasoMaximoAlcanzado);
                AbrirPuerta();
            }
        }
        else
        {
            if (fuenteAudio != null && sonidoError != null)
                fuenteAudio.PlayOneShot(sonidoError);

            paso.fondoImagen.color = colorErrorRojo;
            yield return new WaitForSeconds(0.5f); 
            paso.fondoImagen.color = colorAzulOriginal;
        }
    }

    void MostrarExplicacionDelPaso(int indice)
    {
        if (textoExplicacion != null && datosEcuacion.explicacionesMatematicas != null && datosEcuacion.explicacionesMatematicas.Length > indice)
        {
            // Limpiamos el string para asegurar que no haya caracteres ocultos
            string exp = datosEcuacion.explicacionesMatematicas[indice];
            textoExplicacion.text = "Explicación matemática:\n" + exp;
        }
    }

    void CambiarDePaso(int nuevoIndice)
    {
        indiceActivo = nuevoIndice;
        ResaltarPaso(indiceActivo);
        ActualizarEcuacionVisual(indiceActivo); 
        
        fallosEnPasoActual = 0;
        tiempoEnPasoActual = 0f;

        if(textoPistaVisual != null) textoPistaVisual.text = ""; 
        
        if (indiceActivo < pasoMaximoAlcanzado)
        {
            MostrarExplicacionDelPaso(indiceActivo);
        }
    }

    void ResaltarPaso(int indice)
    {
        for (int i = 0; i < listaPasos.Count; i++)
        {
            listaPasos[i].gameObject.SetActive(i == indice);
            if (i == indice)
            {
                listaPasos[i].fondoImagen.color = colorAzulOriginal;
                if (PasoEstaResuelto(i)) listaPasos[i].RevelarEstadoGris();
            }
        }
    }

    void ManejarEntradaTeclado()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b') BorrarCaracter();
            else if (c == '\n' || c == '\r') continue; 
            else if (EsCaracterValido(c)) EscribirCaracter(c);
        }
    }

    bool EsCaracterValido(char c)
    {
        return "0123456789+-*/= ()xX".Contains(c.ToString());
    }

    void EscribirCaracter(char c)
    {
        string valorActual = listaPasos[indiceActivo].valorActualTexto.text;
        if (valorActual == "0") listaPasos[indiceActivo].ActualizarValor(c.ToString());
        else listaPasos[indiceActivo].ActualizarValor(valorActual + c);
        ResaltarPaso(indiceActivo);
    }

    void BorrarCaracter()
    {
        string valorActual = listaPasos[indiceActivo].valorActualTexto.text;
        if (valorActual.Length > 1) 
            listaPasos[indiceActivo].ActualizarValor(valorActual.Substring(0, valorActual.Length - 1));
        else 
            listaPasos[indiceActivo].ActualizarValor("0");
        ResaltarPaso(indiceActivo);
    }

    void AbrirPuerta()
    {
        puertaAbierta = true;
        this.gameObject.SetActive(false); 
        if (activadorEscena != null) activadorEscena.FinalizarReto();
    }

    bool PasoEstaResuelto(int indice)
    {
        if (indice < 0 || indice >= listaPasos.Count) return false;
        string textoUsuario = listaPasos[indice].valorActualTexto.text;
        if (datosEcuacion.resultados == null || indice >= datosEcuacion.resultados.Length) return false;
        string respuestaCorrecta = datosEcuacion.resultados[indice];
        return LimpiarTexto(textoUsuario) == LimpiarTexto(respuestaCorrecta);
    }

    string LimpiarTexto(string entrada)
    {
        return entrada.Replace(" ", "").Trim().ToLower();
    }

    void EvaluarAyuda()
    {
        if ((fallosEnPasoActual >= 2 || tiempoEnPasoActual > 15f) && datosEcuacion.pistas != null && datosEcuacion.pistas.Length > indiceActivo)
        {
            if (textoPistaVisual != null) 
            {
                textoPistaVisual.text = "Pista: " + datosEcuacion.pistas[indiceActivo];
            }
        }
    }
}