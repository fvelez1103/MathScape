using UnityEngine;
using UnityEngine.SceneManagement; 
using System;

public class DDA_Laberinto : MonoBehaviour
{
    public static DDA_Laberinto Instancia;
    private string escenaActual;

    [Header("Métricas de Entrada (Crisp)")]
    public int muertesNPC = 0;
    public int erroresPuerta = 0;

    [Header("Ajustes de Salida")]
    public float velocidadNormal = 1.7f;
    public float velocidadReducida = 1.1f;
    [HideInInspector] public float velocidadActualSombra;
    public bool ayuda5050Activa = false;

    [Header("Salida Mamdani (Asistencia)")]
    public float nivelAsistenciaPuertas = 0f;
    public float nivelAsistenciaSombra = 0f;

    [Header("Salida Mamdani (Enrutamiento Final)")]
    public float nivelHabilidadGlobal = 0f;

    public static event Action OnActivar5050;
    public static event Action<float> OnCambioVelocidadSombra;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            velocidadActualSombra = velocidadNormal;
        }
        else { Destroy(gameObject); }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != escenaActual)
        {
            escenaActual = SceneManager.GetActiveScene().name;
            ResetDDA();
        }
    }

    public void RegistrarMuerteNPC()
    {
        muertesNPC++;
        EjecutarMotorMamdani_Asistencia();
    }

    public void RegistrarErrorPuerta()
    {
        erroresPuerta++;
        EjecutarMotorMamdani_Asistencia();
    }

    private float Func_HombroIzquierdo(float x, float a, float b) => x <= a ? 1f : (x >= b ? 0f : (b - x) / (b - a));
    private float Func_Triangulo(float x, float a, float b, float c) => Mathf.Max(0, Mathf.Min((x - a) / (b - a), (c - x) / (c - b)));
    private float Func_HombroDerecho(float x, float a, float b) => x <= a ? 0f : (x >= b ? 1f : (x - a) / (b - a));

    private void EjecutarMotorMamdani_Asistencia()
    {
        // Se desplazaron los umbrales para que la velocidad cambie en la 4ta muerte en lugar de la 3ra
        float muertesBajas = Func_HombroIzquierdo(muertesNPC, 2f, 3f);
        float muertesMedias = Func_Triangulo(muertesNPC, 2f, 3f, 4f);
        float muertesAltas = Func_HombroDerecho(muertesNPC, 3f, 4f);

        float erroresBajos = Func_HombroIzquierdo(erroresPuerta, 1f, 2f);
        float erroresAltos = Func_HombroDerecho(erroresPuerta, 1f, 2f);

        nivelAsistenciaPuertas = CalcularCentroide(erroresBajos, 0f, erroresAltos);
        nivelAsistenciaSombra = CalcularCentroide(muertesBajas, muertesMedias, muertesAltas);

        AplicarDecisiones();
    }

    private void AplicarDecisiones()
    {
        if (nivelAsistenciaPuertas >= 50f && !ayuda5050Activa)
        {
            ayuda5050Activa = true;
            OnActivar5050?.Invoke();
        }

        if (nivelAsistenciaSombra >= 70f && velocidadActualSombra != velocidadReducida)
        {
            velocidadActualSombra = velocidadReducida;
            OnCambioVelocidadSombra?.Invoke(velocidadActualSombra);
        }
    }

    // --- MOTOR MAMDANI 2: ENRUTAMIENTO ACTUALIZADO ---
    public string DeterminarSiguienteLaberinto()
    {
        int totalErrores = muertesNPC + erroresPuerta;

        // 1. Fuzzificación Calibrada
        // Experto: 100% con 0 o 1 error. Cae a 0 al llegar a 2.5 errores.
        float errBajos = Func_HombroIzquierdo(totalErrores, 1.1f, 2.5f);
        
        // Medio: Pico en 2.5 errores (cubre el rango de 2 y 3 errores).
        float errMedios = Func_Triangulo(totalErrores, 1.5f, 2.5f, 4.5f);
        
        // Novato: Empieza a subir después de 3 errores. 100% en 4.5 errores.
        float errAltos = Func_HombroDerecho(totalErrores, 3.5f, 4.5f);

        // 2. Inferencia
        float reglaExperto = errBajos;
        float reglaMedio = errMedios;
        float reglaNovato = errAltos;

        // 3. Defuzzificación por Centroide
        float sumaNum = 0f;
        float sumaDen = 0f;
        for (int i = 0; i <= 20; i++)
        {
            float x = i * 5f; 
            float mu_Novato = Mathf.Min(reglaNovato, Func_HombroIzquierdo(x, 20f, 40f));
            float mu_Medio = Mathf.Min(reglaMedio, Func_Triangulo(x, 30f, 50f, 70f));
            float mu_Experto = Mathf.Min(reglaExperto, Func_HombroDerecho(x, 60f, 80f));
            float mu_Agregada = Mathf.Max(mu_Novato, Mathf.Max(mu_Medio, mu_Experto));

            sumaNum += x * mu_Agregada;
            sumaDen += mu_Agregada;
        }

        nivelHabilidadGlobal = (sumaDen != 0) ? (sumaNum / sumaDen) : 50f;
        Debug.Log($"<color=lime>DDA Laberinto:</color> Habilidad Final = {nivelHabilidadGlobal:F1}% con {totalErrores} errores totales.");

        // 4. Decisión Final basada en Habilidad Global
        if (nivelHabilidadGlobal >= 70f) return "Laberinto-5.4"; // Ruta Difícil (Experto)
        else if (nivelHabilidadGlobal >= 35f) return "Laberinto-5.3"; // Ruta Media (Medio)
        else return "Laberinto-5.2"; // Ruta Fácil (Novato)
    }

    private float CalcularCentroide(float rBaja, float rMedia, float rAlta)
    {
        float sumaNumerador = 0f;
        float sumaDenominador = 0f;
        for (int i = 0; i <= 20; i++)
        {
            float x = i * 5f; 
            float mu_Baja = Mathf.Min(rBaja, Func_HombroIzquierdo(x, 20f, 40f));
            float mu_Media = Mathf.Min(rMedia, Func_Triangulo(x, 30f, 50f, 70f));
            float mu_Alta = Mathf.Min(rAlta, Func_HombroDerecho(x, 60f, 80f));
            float mu_Agregada = Mathf.Max(mu_Baja, Mathf.Max(mu_Media, mu_Alta));
            sumaNumerador += x * mu_Agregada;
            sumaDenominador += mu_Agregada;
        }
        return (sumaDenominador != 0) ? (sumaNumerador / sumaDenominador) : 0f;
    }

    public void ResetDDA()
    {
        muertesNPC = 0;
        erroresPuerta = 0;
        ayuda5050Activa = false;
        velocidadActualSombra = velocidadNormal; 
        nivelAsistenciaSombra = 0f;
        nivelAsistenciaPuertas = 0f;
    }
}