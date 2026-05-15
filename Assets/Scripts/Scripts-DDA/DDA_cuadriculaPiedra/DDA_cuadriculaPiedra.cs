using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum RutaAsignada { Corta, Media, Larga }


public class DDA_cuadriculaPiedra : MonoBehaviour
{
    public static DDA_cuadriculaPiedra Instancia;

    [Header("Métricas Actuales (Nivel)")]
    public int totalResets = 0;
    public int totalUndos = 0;
    public float tiempoEnNivelActual = 0f;
    public float tiempoTotalHistoricoNivel = 0f;

    [Header("Historial Tutorial (Niveles 1-3)")]
    public int nivelesCompletados = 0;
    public int acumuladoResets = 0;
    public int acumuladoUndos = 0;
    public float acumuladoTiempo = 0f;

    [Header("Salida Mamdani (Perfil Final)")]
    [Range(0, 100)] public float perfilHabilidadGlobal = 0f;
    public float nivelFrustracionPuzzle = 0f;
    public float frustracionMaximaEnNivel = 0f; 
    public bool ayudaActivada = false;

    [Header("Ajustes de Nivel")]
    public int nivelID; 

    private static int ultimoIDRegistrado = -1; 
    private static bool ayudaPersistente = false; 
    private static float frustracionPersistente = 0f;

    public bool ayudaHistorialVista = false; 
    
    [Header("Plan de Ruta (Post-Tutorial)")]
    public RutaAsignada rutaActual = RutaAsignada.Media;

    [Header("Filtro de Escenas")]
    public List<string> escenasDePuzzles = new List<string> { "Puzzle1", "Puzzle2", "Puzzle3", "Puzzle4", "Puzzle5", "Puzzle6" };

    public static event Action OnMostrarAyuda;
    private string escenaActual;
    private bool estaEnEscenaValida = false;

    private void Awake()
    {
        if (Instancia == null) { Instancia = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        if (nivelID == ultimoIDRegistrado) 
        {
            ayudaActivada = ayudaPersistente; 
            frustracionMaximaEnNivel = frustracionPersistente; // Recuperamos el pico de frustración
            Debug.Log($"<color=yellow>DDA:</color> Reinicio nivel {nivelID}. Recuperando frustración máxima: {frustracionMaximaEnNivel:F1}");
        }
        else 
        {
            ayudaActivada = false;
            ayudaPersistente = false;
            frustracionPersistente = 0f; // Reset real solo si cambia el ID del nivel
            frustracionMaximaEnNivel = 0f;
            ultimoIDRegistrado = nivelID;
            Debug.Log($"<color=green>DDA:</color> Nivel nuevo {nivelID}. Reseteando memoria persistente.");
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != escenaActual)
        {
            escenaActual = SceneManager.GetActiveScene().name;
            estaEnEscenaValida = escenasDePuzzles.Contains(escenaActual);

            if (estaEnEscenaValida)
            {
                tiempoEnNivelActual = 0f;
                ayudaActivada = false;
                ayudaHistorialVista = false;
                nivelFrustracionPuzzle = 0f;             
            }
        }

        if (estaEnEscenaValida)
        {
            tiempoEnNivelActual += Time.deltaTime;
            tiempoTotalHistoricoNivel += Time.unscaledDeltaTime;
            
            if (Time.frameCount % 30 == 0) EjecutarMamdaniFrustracion();
        }
    }

    public void NotificarReinicioLocal()
    {
        totalResets++;
        tiempoEnNivelActual = 0f; 
        ayudaActivada = false;   
        nivelFrustracionPuzzle = 0f;
        // NOTA: NO reseteamos frustracionMaximaEnNivel aquí para que Firebase la capture al final
    }

    public void RegistrarFinDeNivel()
    {
        nivelesCompletados++;

        // Hacemos que la habilidad se calcule siempre, no solo en el nivel 2, para que sea dinámica
        if (nivelesCompletados <= 6) // Extendemos el rango de actualización
        {
            acumuladoResets += totalResets;
            acumuladoUndos += totalUndos;
            acumuladoTiempo += tiempoEnNivelActual;
            
            // Recalculamos perfil para que varíe según el desempeño actual
            CalcularPerfilHabilidadMamdani();
        }

        ResetMetricasNivel();
    }

    private void EjecutarMamdaniFrustracion()
    {
        // --- SENSIBILIDAD AJUSTADA PARA LA TESIS ---
        // Antes: 25s y 8 resets. Ahora: 15s y 3 resets para que detecte frustración real.
        float tiempoBase = ayudaHistorialVista ? 8f : 15f; 
        float tiempoLargo = Mathf.Clamp01((tiempoEnNivelActual - tiempoBase) / 20f); 
        
        float muchosResets = Mathf.Clamp01((totalResets - 3f) / 5f); 

        float r_Alta = Mathf.Max(tiempoLargo, muchosResets);
        nivelFrustracionPuzzle = r_Alta * 100f; 

        // --- MEMORIA DE PICO MÁXIMO ---
        if (nivelFrustracionPuzzle > frustracionMaximaEnNivel)
        {
            frustracionMaximaEnNivel = nivelFrustracionPuzzle;
            frustracionPersistente = frustracionMaximaEnNivel; // Guardamos en la mochila estática
        }

        if (tiempoEnNivelActual < 3f) nivelFrustracionPuzzle = 0f;

        if (nivelFrustracionPuzzle >= 75f && !ayudaActivada)
        {
            RegistrarUsoDeAyuda();
            ayudaHistorialVista = true; 
            Debug.Log($"<color=cyan>DDA Puzzle:</color> Frustración > 75% en {tiempoEnNivelActual:F1}s. Mostrando ayuda.");
            OnMostrarAyuda?.Invoke();
        }
    }

   private void CalcularPerfilHabilidadMamdani()
    {
        // --- 1. AJUSTE DE SENSIBILIDAD (Hacerlo más difícil de ser "Experto") ---
        // Para ser rápido, ahora debe bajar de 25s (antes 30s era muy fácil)
        float h_Rapido = 1 - Mathf.Clamp01((acumuladoTiempo - 15f) / 30f); 
        float h_Lento = Mathf.Clamp01((acumuladoTiempo - 45f) / 50f);
        float h_TiempoMedio = Mathf.Max(0, 1 - Mathf.Abs(acumuladoTiempo - 35f) / 20f); 

        // Para tener "Pocos Errores", ahora máximo 2 (antes 4 era muy permisivo)
        float totalErrores = acumuladoResets + acumuladoUndos; 
        float h_PocosErrores = 1 - Mathf.Clamp01(totalErrores / 2f);
        float h_MuchosErrores = Mathf.Clamp01((totalErrores - 3f) / 6f); 

        // --- 2. REGLAS MAMDANI ---
        float reglaExperto = Mathf.Min(h_Rapido, h_PocosErrores); // Rápido Y preciso
        float reglaNovato = Mathf.Max(h_Lento, h_MuchosErrores);  // Lento O con fallos
        float reglaMedio = h_TiempoMedio; 

        // --- 3. DEFUZZIFICACIÓN (CENTROIDE) ---
        float numerador = 0, denominador = 0;
        for (int i = 0; i <= 100; i += 5)
        {
            // Definimos las áreas de salida: 
            // Novato se concentra en el 0-30, Medio en 50, Experto en 70-100
            float mu_Novato = Mathf.Min(reglaNovato, (30f - i) / 30f); // Pico en 0
            float mu_Medio = Mathf.Min(reglaMedio, Mathf.Max(0, 1 - Mathf.Abs(i - 50f) / 25f)); // Pico en 50
            float mu_Experto = Mathf.Min(reglaExperto, (i - 70f) / 30f); // Pico en 100
            
            float mu_agregada = Mathf.Max(mu_Novato, Mathf.Max(mu_Medio, mu_Experto));
            numerador += i * mu_agregada;
            denominador += mu_agregada;
        }

        perfilHabilidadGlobal = (denominador == 0) ? 50f : numerador / denominador;

        if (perfilHabilidadGlobal <= 35f) 
        {
            rutaActual = RutaAsignada.Corta; 
        }
        else if (perfilHabilidadGlobal >= 65f) 
        {
            rutaActual = RutaAsignada.Larga; 
        }
        else 
        {
            rutaActual = RutaAsignada.Media; 
        }
            
        GenerarReporteTesis();
    }

    public void RegistrarUsoDeAyuda()
    {
        ayudaActivada = true;
        ayudaPersistente = true;
        Debug.Log("<color=green>DDA:</color> Ayuda registrada y guardada en memoria persistente.");
    }

    public void ResetMetricasNivel()
    {
        totalResets = 0;
        totalUndos = 0;
        tiempoEnNivelActual = 0f;
        tiempoTotalHistoricoNivel = 0f;
        ayudaActivada = false;
        ayudaHistorialVista = false;
        // NOTA: No reseteamos frustracionMaximaEnNivel aquí para que el FirebaseManager 
        // pueda leerlo al final del nivel antes de que cambie el ID en el Start
    }

    public void GenerarReporteTesis()
    {
        string reporte = "\n<color=yellow>========== REPORTE ACTUAL DDA (Habilidad) ==========</color>\n" +
                         $"<b>TIEMPO ACUMULADO:</b> {acumuladoTiempo:F2}s\n" +
                         $"<b>ERRORES ACUMULADOS:</b> {acumuladoResets + acumuladoUndos}\n" +
                         $"<color=lime><b>PERFIL MAMDANI:</b> {perfilHabilidadGlobal:F2}</color>\n" +
                         $"<color=orange><b>RUTA ASIGNADA:</b> {rutaActual}</color>\n" +
                         "<color=yellow>===================================================</color>";
        Debug.Log(reporte);
    }
}