using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic; 

public enum EstadoJugador
{ 
    Optimo, 
    Enganchado, 
    Frustrado, 
    Bloqueado 
}

public class DDA_DifusoPuro : MonoBehaviour
{
    public static DDA_DifusoPuro Instancia;

    [Header("Métricas Brutas (Entradas Crisp)")]
    public int erroresMatematicos = 0;
    public int caidasAlVacio = 0;
    public int reseteosManuales = 0;
    public float tiempoEnEscena = 0f; 
    
    [Header("Estado de la Misión")]
    public bool objetivoCompletado = false; 

    [Header("Memoria de Salida")]
    public EstadoJugador estadoNivelAnterior = EstadoJugador.Optimo;

    [Header("Salida Mamdani (Escala 0-100)")]
    public float nivelFrustracionCrisp = 0f;
    public EstadoJugador estadoActual = EstadoJugador.Optimo;

    [Header("Filtro de Escenas")]
    public List<string> escenasDePlataformas = new List<string>(); 

    private bool estaEnEscenaValida = false;
    private bool enZonaSegura = false;
    private string ultimaPlataformaCargada = "";

    public static event Action<EstadoJugador> OnCambioEstado;

    private void Awake()
    {
        if (Instancia == null) { Instancia = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += AlCargarEscena;
    private void OnDisable() => SceneManager.sceneLoaded -= AlCargarEscena;

    private void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        estaEnEscenaValida = escenasDePlataformas.Contains(scene.name);

        if (estaEnEscenaValida)
        {
            enZonaSegura = false; 
            if (scene.name != ultimaPlataformaCargada)
            {
                LimpiarDDA();
                ultimaPlataformaCargada = scene.name;
            }
        }
        else
        {
            enZonaSegura = true; 
        }
    }

    private void Update()
    {
        // El DDA solo avanza si NO estamos en zona segura y el objetivo NO está completado
        if (estaEnEscenaValida && !enZonaSegura && !objetivoCompletado)
        {
            tiempoEnEscena += Time.deltaTime;

            if (Time.frameCount % 60 == 0) 
            {
                EjecutarMotorDifuso();
            }
        }
    }

    private float Func_Triangulo(float x, float a, float b, float c) => Mathf.Max(0, Mathf.Min((x - a) / (b - a), (c - x) / (c - b)));
    private float Func_HombroDerecho(float x, float a, float b) => x <= a ? 0f : (x >= b ? 1f : (x - a) / (b - a));
    private float Func_HombroIzquierdo(float x, float a, float b) => x >= b ? 0f : (x <= a ? 1f : (b - x) / (b - a));

    public void RegistrarErrorMatematico() { erroresMatematicos++; EjecutarMotorDifuso(); }
    public void RegistrarCaida() { caidasAlVacio++; EjecutarMotorDifuso(); }
    public void RegistrarReseteoManual() { reseteosManuales++; EjecutarMotorDifuso(); }

    private void EjecutarMotorDifuso()
    {
        float matAlto = Func_HombroDerecho(erroresMatematicos, 5f, 7f);
        float motorAlto = Func_HombroDerecho(caidasAlVacio, 3f, 6f);
        float resetAlto = Func_HombroDerecho(reseteosManuales, 2f, 5f);

        float tiempoEstancadoMedio = Func_Triangulo(tiempoEnEscena, 40f, 60f, 85f);
        float tiempoEstancadoGrave = Func_HombroDerecho(tiempoEnEscena, 80f, 100f);

        float reglaMedia = Mathf.Max(
            Func_Triangulo(erroresMatematicos, 2f, 4f, 6f), 
            Func_Triangulo(reseteosManuales, 1f, 2f, 4f),
            tiempoEstancadoMedio 
        );

        float reglaAlta = Mathf.Max(
            matAlto, 
            motorAlto, 
            resetAlto, 
            tiempoEstancadoGrave 
        );

        float reglaBaja = Mathf.Min(
            Func_HombroIzquierdo(erroresMatematicos, 1f, 3f),
            Func_HombroIzquierdo(caidasAlVacio, 1f, 4f),
            Func_HombroIzquierdo(tiempoEnEscena, 30f, 50f)
        );

        float sumaNumerador = 0f;
        float sumaDenominador = 0f;
        for (int i = 0; i <= 20; i++)
        {
            float x = i * 5f; 
            float mu_Baja = Mathf.Min(reglaBaja, Func_HombroIzquierdo(x, 5f, 20f));
            float mu_Media = Mathf.Min(reglaMedia, Func_Triangulo(x, 15f, 40f, 65f));
            float mu_Alta = Mathf.Min(reglaAlta, Func_HombroDerecho(x, 60f, 85f));

            float mu_Agregada = Mathf.Max(mu_Baja, mu_Media, mu_Alta);
            sumaNumerador += x * mu_Agregada;
            sumaDenominador += mu_Agregada;
        }

        nivelFrustracionCrisp = (sumaDenominador != 0) ? sumaNumerador / sumaDenominador : 0f;

        EvaluarEstadoFinal();
    }

    private void EvaluarEstadoFinal()
    {
        EstadoJugador nuevoEstado = EstadoJugador.Optimo;

        if (nivelFrustracionCrisp >= 70f) nuevoEstado = EstadoJugador.Bloqueado;
        else if (nivelFrustracionCrisp >= 45f) nuevoEstado = EstadoJugador.Frustrado;
        else if (nivelFrustracionCrisp >= 15f) nuevoEstado = EstadoJugador.Enganchado; 

        if (!enZonaSegura)
        {
            if (nuevoEstado != estadoActual)
            {
                estadoActual = nuevoEstado;
                OnCambioEstado?.Invoke(estadoActual);
                Debug.Log($"<color=cyan>DDA:</color> Nuevo Estado detectado: {estadoActual}. Motivo: Tiempo/Fallas.");
            }
        }
    }

    public void LimpiarDDA()
    {
        estadoNivelAnterior = estadoActual; 
        erroresMatematicos = 0;
        caidasAlVacio = 0;
        reseteosManuales = 0;
        tiempoEnEscena = 0f;
        nivelFrustracionCrisp = 0f;
        estadoActual = EstadoJugador.Optimo;
        objetivoCompletado = false; // Reiniciamos el freno para el nuevo nivel
        
        OnCambioEstado?.Invoke(estadoActual);
        Debug.Log($"<color=white>DDA:</color> Memoria guardada ({estadoNivelAnterior}). Contadores reiniciados.");
    }
}