using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instancia;
    
    [Header("Configuración Firebase")]
    public string urlDatabase = "https://db-mathscapedatos-default-rtdb.firebaseio.com/"; 

    private DatabaseReference baseDatos;
    private string idSesionActual;
    private float tiempoInicio;

    private DatosSesionDDA datosDDA;

    void Awake()
    {
        if (Instancia == null) {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            datosDDA = new DatosSesionDDA(); 
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AppOptions opciones = new AppOptions();
        opciones.DatabaseUrl = new Uri(urlDatabase); 
        opciones.ApiKey = "AIzaSyA0JLF0DvgX6vf5zRIsfxU8WFzWz0zG69I"; 
        opciones.AppId = "1:404514317364:android:548f3b8a552c1fdc1c9661"; 

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available) {
                FirebaseApp app = FirebaseApp.Create(opciones, "MathScapeApp");
                baseDatos = FirebaseDatabase.GetInstance(app, urlDatabase).RootReference;
                
                idSesionActual = Guid.NewGuid().ToString();
                tiempoInicio = Time.unscaledTime; 
                
                Debug.Log("<color=green>Firebase Conectado: Sesión " + idSesionActual + "</color>");
            } else {
                Debug.LogError("Error dependencias Firebase: " + task.Result);
            }
        });
    }

    public void RegistrarPerfilInicial(string nombre, int edad, int conocimiento) {
        if (baseDatos == null) return;

        string fechaHoraActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        PerfilEstudiante perfil = new PerfilEstudiante(nombre, edad, conocimiento, fechaHoraActual);
        
        string json = JsonUtility.ToJson(perfil);
        baseDatos.Child("Estudiantes").Child(idSesionActual).Child("Perfil").SetRawJsonValueAsync(json);
        
        Debug.Log($"<color=yellow>Perfil Registrado:</color> {nombre} a las {fechaHoraActual}");
    }

    // ==========================================
    // M1: PLATAFORMAS (Balanzas)
    // ==========================================
    public void M1_RegistrarFalla(int nivelID) {
        if (nivelID == 0) return; 
        ObtenerONivelM1(nivelID).fallasTotales++; 
    }

    public void M1_RegistrarErrorBloque(int nivelID) {
        if (nivelID == 0) return; 
        ObtenerONivelM1(nivelID).bloquesMalPuestos++;
    }

    public void M1_RegistrarAyudaTexto(int nivelID) {
        if (nivelID == 0) return; 
        ObtenerONivelM1(nivelID).usoAyudaTexto = true;
    }
    public void M1_RegistrarAyudaVisual(int nivelID) {
        if (nivelID == 0) return; 
        MetricasM1 nivel = ObtenerONivelM1(nivelID);
        nivel.usoAyudaVisual = true;
        nivel.usoAyudaTexto = true; 
    }

    public void M1_RegistrarPrimerMovimiento(int nivelID, float tiempo) {
        if (nivelID == 0) return; 
        MetricasM1 nivel = ObtenerONivelM1(nivelID);
        if (nivel.tiempoPrimerMovimiento == 0f) {
            nivel.tiempoPrimerMovimiento = (float)Math.Round(tiempo, 2);
        }
    }

    public void M1_FinalizarNivel(int nivelID, float tiempo) {
        if (nivelID == 0) return; 
        MetricasM1 nivel = ObtenerONivelM1(nivelID);
        nivel.tiempoCompletado = (float)Math.Round(tiempo, 2);
    }
    
    public void M1_SumarTiempoHistorico(int nivelID, float tiempoPerdido) {
        if (nivelID == 0) return;
        ObtenerONivelM1(nivelID).tiempoTotalHistorico += (float)Math.Round(tiempoPerdido, 2);
    }

    public void M1_CapturarDDA(int nivelID) {
        if (nivelID == 0 || DDA_DifusoPuro.Instancia == null) return;
        MetricasM1 nivel = ObtenerONivelM1(nivelID);
        
        nivel.frustracionDDA = (float)Math.Round(DDA_DifusoPuro.Instancia.nivelFrustracionCrisp, 2);
        nivel.estadoFinalDDA = DDA_DifusoPuro.Instancia.estadoActual.ToString();
    }

    private MetricasM1 ObtenerONivelM1(int id) {
        MetricasM1 nivel = datosDDA.m1_niveles.Find(n => n.nivelID == id);
        if (nivel == null) { nivel = new MetricasM1 { nivelID = id }; datosDDA.m1_niveles.Add(nivel); }
        return nivel;
    }

    // ==========================================
    // M2: PUZZLES
    // ==========================================
    private MetricasM2Nivel ObtenerONivelM2(int id) {
        MetricasM2Nivel nivel = datosDDA.m2_puzzles.Find(n => n.nivelID == id);
        if (nivel == null) { nivel = new MetricasM2Nivel { nivelID = id }; datosDDA.m2_puzzles.Add(nivel); }
        return nivel;
    }

    public void M2_RegistrarReinicio(int nivelID) {
        if (nivelID == 0) return; ObtenerONivelM2(nivelID).vecesTeclaR++;
    }

    public void M2_RegistrarTeclaZ(int nivelID) {
        if (nivelID == 0) return; ObtenerONivelM2(nivelID).vecesTeclaZ++;
    }

    public void M2_RegistrarMovimiento(int nivelID) {
        if (nivelID == 0) return; ObtenerONivelM2(nivelID).movimientosTotales++;
    }

    public void M2_RegistrarNivel(int id, float tiempo) {
        if (id == 0) return; 
        MetricasM2Nivel nivel = ObtenerONivelM2(id);
        nivel.tiempoEnNivel = (float)Math.Round(tiempo, 2); 
    }

    public void M2_CapturarNivel(int nivelID) 
    {
        if (nivelID == 0) return; 
        MetricasM2Nivel puzzle = ObtenerONivelM2(nivelID);  
        if (DDA_cuadriculaPiedra.Instancia != null)
        {
            puzzle.frustracionAlcanzada = (float)Math.Round(DDA_cuadriculaPiedra.Instancia.frustracionMaximaEnNivel, 2);
            puzzle.perfilHabilidadNivel = (float)Math.Round(DDA_cuadriculaPiedra.Instancia.perfilHabilidadGlobal, 2);            
            puzzle.ayudaImagenUsada = DDA_cuadriculaPiedra.Instancia.ayudaActivada;
            puzzle.rutaAsignada = DDA_cuadriculaPiedra.Instancia.rutaActual.ToString();
        }
    }

    // ==========================================
    // M4: CUADRADO PERFECTO
    // ==========================================
    public void M4_RegistrarComprobar() { datosDDA.m4_cuadrado.vecesComprobarTotales++; }
    
    public void M4_RestarComprobacionFantasma() { 
        if(datosDDA.m4_cuadrado.vecesComprobarTotales > 0) datosDDA.m4_cuadrado.vecesComprobarTotales--; 
    }

    public void M4_RegistrarMovimientoPieza() { datosDDA.m4_cuadrado.piezasMovidas++; }
    
    public void M4_RegistrarAyuda() { datosDDA.m4_cuadrado.ayudaActivada = true; }

    public void M4_RegistrarTiempo(float tiempo) {
        datosDDA.m4_cuadrado.tiempoArmado = (float)Math.Round(tiempo, 2);
    }
    public void M4_CapturarNivel(int nivelID) {
        M2_CapturarNivel(nivelID);
    }

    // ==========================================
    // M5: TUBERIAS (Trivia)
    // ==========================================
    public void M5_RegistrarPregunta(int id, float tiempo, bool correcta, string elegida) {
        float dominioActual = 0f;
        bool ayudaUsada = false;

        if (DDA_TriviaTuberias.Instancia != null) {
            dominioActual = (float)Math.Round(DDA_TriviaTuberias.Instancia.nivelDominio, 2);
            ayudaUsada = DDA_TriviaTuberias.Instancia.usoAyuda5050;
        }

        datosDDA.m5_trivia.Add(new MetricasM5Pregunta(id, (float)Math.Round(tiempo, 2), correcta, dominioActual, ayudaUsada, elegida));
    }

    // ==========================================
    // M6: LABERINTO 
    // ==========================================
    public void M6_RegistrarMuerteVillano(int nivelID) {
        if (nivelID == 0) return; ObtenerONivelM6(nivelID).muertesVillano++;
    }

    public void M6_RegistrarMuertePuerta(int nivelID) {
        if (nivelID == 0) return; ObtenerONivelM6(nivelID).muertesPuertaIncorrecta++;
    }

    public void M6_RegistrarVelocidadReducida(int nivelID) {
        if (nivelID == 0) return; ObtenerONivelM6(nivelID).velocidadReducida = true;
    }

    public void M6_FinalizarLaberinto(int nivelID, float tiempo) {
        if (nivelID == 0) return; ObtenerONivelM6(nivelID).tiempoCompletado = (float)Math.Round(tiempo, 2);
    }

    public void M6_CapturarDDA(int nivelID) {
        if (nivelID == 0) return; 
        MetricasM6Laberinto nivel = ObtenerONivelM6(nivelID);
        
        if (DDA_Laberinto.Instancia != null) {
            nivel.usoAyuda5050 = DDA_Laberinto.Instancia.ayuda5050Activa;
            nivel.rutaElegida = DDA_Laberinto.Instancia.DeterminarSiguienteLaberinto();
            nivel.frustracionMamdani = (float)Math.Round(DDA_Laberinto.Instancia.nivelHabilidadGlobal, 2);
        }
    }

    private MetricasM6Laberinto ObtenerONivelM6(int id) {
        MetricasM6Laberinto nivel = datosDDA.m6_laberintos.Find(n => n.nivelID == id);
        if (nivel == null) { nivel = new MetricasM6Laberinto { nivelID = id }; datosDDA.m6_laberintos.Add(nivel); }
        return nivel;
    }

    // ==========================================
    // QUIZ FINAL (POST-TEST)
    // ==========================================
    public void RegistrarPreguntaQuizFinal(int id, string pregunta, string elegida, bool correcta, float tiempo) {
        datosDDA.quiz_final.respuestas.Add(new DetallePreguntaQuiz(id, pregunta, elegida, correcta, tiempo));
    }

    public void FijarPuntuacionQuizFinal(int puntuacion) {
        datosDDA.quiz_final.puntuacionFinal = puntuacion;
    }

    // ==========================================
    // ENVÍO A LA BASE DE DATOS Y CÁLCULOS FINALES
    // ==========================================
    public void EnviarDatosFinales() {
        if (baseDatos == null) return;
        
        float tiempoCrudo = Time.unscaledTime - tiempoInicio;
        datosDDA.tiempoTotalJuego = (float)Math.Round(tiempoCrudo, 2);

        float frustracionMax = 0f;
        string nivelTrauma = "Ninguno";

        foreach (var m1 in datosDDA.m1_niveles) {
            if (m1.frustracionDDA > frustracionMax) { frustracionMax = m1.frustracionDDA; nivelTrauma = "M1-Plataformas"; }
        }
        foreach (var m2 in datosDDA.m2_puzzles) {
            if (m2.frustracionAlcanzada > frustracionMax) { frustracionMax = m2.frustracionAlcanzada; nivelTrauma = "M2-Puzzles"; }
        }
        foreach (var m6 in datosDDA.m6_laberintos) {
            if (m6.frustracionMamdani > frustracionMax) { frustracionMax = m6.frustracionMamdani; nivelTrauma = "M6-Laberintos"; }
        }

        datosDDA.frustracionGlobal = (float)Math.Round(frustracionMax, 2);
        datosDDA.minijuegoMasFrustrante = nivelTrauma;

        if (DDA_cuadriculaPiedra.Instancia != null) {
            datosDDA.m2_global.perfilHabilidadGlobal = (float)Math.Round(DDA_cuadriculaPiedra.Instancia.perfilHabilidadGlobal, 2);
            datosDDA.m2_global.rutaAsignada = DDA_cuadriculaPiedra.Instancia.rutaActual.ToString();
            
            float tiempoM2Global = 0f;
            foreach (var puzzle in datosDDA.m2_puzzles) { tiempoM2Global += puzzle.tiempoEnNivel; }
            datosDDA.m2_global.tiempoTotalGlobal = (float)Math.Round(tiempoM2Global, 2);

            datosDDA.habilidadGlobalFinal = datosDDA.m2_global.perfilHabilidadGlobal;
        }
        
        string json = JsonUtility.ToJson(datosDDA);
        baseDatos.Child("Estudiantes").Child(idSesionActual).Child("Resultados").SetRawJsonValueAsync(json);
        Debug.Log("<color=cyan>Reporte DDA enviado con éxito a la nube.</color>");

        datosDDA = new DatosSesionDDA(); 
        idSesionActual = Guid.NewGuid().ToString(); 
        tiempoInicio = Time.unscaledTime; 
    }
}

// ==========================================
// CLASES SERIALIZABLES 
// ==========================================
[Serializable]
public class DatosSesionDDA {
    public float tiempoTotalJuego;
    public float frustracionGlobal; 
    public float habilidadGlobalFinal; 
    public string minijuegoMasFrustrante; 

    public List<MetricasM1> m1_niveles = new List<MetricasM1>(); 
    public MetricasM2Global m2_global = new MetricasM2Global();
    public List<MetricasM2Nivel> m2_puzzles = new List<MetricasM2Nivel>();
    public MetricasM4Cuadrado m4_cuadrado = new MetricasM4Cuadrado();
    public List<MetricasM5Pregunta> m5_trivia = new List<MetricasM5Pregunta>();
    public List<MetricasM6Laberinto> m6_laberintos = new List<MetricasM6Laberinto>();
    
    public MetricasQuizFinal quiz_final = new MetricasQuizFinal(); // <-- Lista añadida aquí
}

[Serializable]
public class MetricasM1 {
    public int nivelID; 
    public int fallasTotales; 
    public int bloquesMalPuestos; 
    public float tiempoCompletado;
    public float tiempoTotalHistorico;
    public float frustracionDDA;
    public string estadoFinalDDA;
    public bool usoAyudaVisual;
    public bool usoAyudaTexto; 
    public float tiempoPrimerMovimiento; 
}

[Serializable]
public class MetricasM2Global { 
    public string rutaAsignada;
    public float perfilHabilidadGlobal;
    public float tiempoTotalGlobal; 
}

[Serializable]
public class MetricasM2Nivel {
    public int nivelID;
    public int vecesTeclaR; 
    public int vecesTeclaZ; 
    public float tiempoEnNivel; 
    public float perfilHabilidadNivel; 
    public float frustracionAlcanzada;
    public bool ayudaImagenUsada;
    public string rutaAsignada; 
    public int movimientosTotales; 
}

[Serializable]
public class MetricasM4Cuadrado {
    public float tiempoArmado;
    public int vecesComprobarTotales;
    public bool ayudaActivada; 
    public int piezasMovidas; 
}

[Serializable]
public class MetricasM5Pregunta {
    public int preguntaID;
    public float tiempoRespuesta;
    public bool fueCorrecta;
    public bool usoAyuda5050;
    public float nivelDominioMomento;
    public string respuestaElegida; 

    public MetricasM5Pregunta(int id, float t, bool c, float dominio, bool ayuda, string elegida) {
        this.preguntaID = id; 
        this.tiempoRespuesta = t; 
        this.fueCorrecta = c;
        this.nivelDominioMomento = dominio;
        this.usoAyuda5050 = ayuda;
        this.respuestaElegida = elegida;
    }
}

[Serializable]
public class MetricasM6Laberinto {
    public int nivelID;
    public int muertesPuertaIncorrecta; 
    public int muertesVillano; 
    public float tiempoCompletado; 
    public bool usoAyuda5050;
    public bool velocidadReducida; 
    public string rutaElegida; 
    public float frustracionMamdani; 
}

[Serializable]
public class PerfilEstudiante {
    public string nombre;
    public int edad;
    public int conocimientoPrevio;
    public string fechaHora; 

    public PerfilEstudiante(string n, int e, int c, string fh) {
        nombre = n; 
        edad = e; 
        conocimientoPrevio = c;
        fechaHora = fh; 
    }
}

// ==========================================
// CLASES NUEVAS (QUIZ FINAL)
// ==========================================
[Serializable]
public class MetricasQuizFinal {
    public int puntuacionFinal;
    public List<DetallePreguntaQuiz> respuestas = new List<DetallePreguntaQuiz>();
}

[Serializable]
public class DetallePreguntaQuiz {
    public int idPregunta;
    public string preguntaTexto;
    public string opcionElegida;
    public bool fueCorrecta;
    public float tiempoRespuesta;

    public DetallePreguntaQuiz(int id, string preg, string opc, bool cor, float t) {
        idPregunta = id; preguntaTexto = preg; opcionElegida = opc; fueCorrecta = cor; tiempoRespuesta = (float)Math.Round(t, 2);
    }
}