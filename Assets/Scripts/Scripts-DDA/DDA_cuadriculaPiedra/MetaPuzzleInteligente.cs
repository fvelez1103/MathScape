using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaPuzzleInteligente : MonoBehaviour
{
    [Header("Configuración de Flujo")]
    public int nivelActual; 
    public int siguienteEscenaIndex;
    public MemoriaDelJuego memoria;

    private bool activado = false;
    public int nivelID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;

        if (other.CompareTag("Player") || other.GetComponent<MagneticEntity>() != null)
        {
            activado = true;
            AlLlegarALaMeta();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return;

        if (other.CompareTag("Player") || other.GetComponent<MagneticEntity>() != null)
        {
            activado = true;
            AlLlegarALaMeta();
        }
    }

    public void AlLlegarALaMeta()
    {
        MetricasPuzzleNivel metricas = Object.FindAnyObjectByType<MetricasPuzzleNivel>();
        if (metricas != null) metricas.FinalizarNivel();

        // 1. CAPTURAR EN FIREBASE PRIMERO
        if (FirebaseManager.Instancia != null) FirebaseManager.Instancia.M4_CapturarNivel(nivelID);

        // 2. LUEGO TERMINAR EN DDA
        if (DDA_cuadriculaPiedra.Instancia != null)
        {
            DDA_cuadriculaPiedra.Instancia.RegistrarFinDeNivel();
        }

        bool debeTerminar = EvaluarSiDebeFinalizar();
        if (debeTerminar)
        {
            Debug.Log($"<color=orange>DDA:</color> Finalizando juego en Nivel {nivelActual} por diseño de ruta.");
            FinalizarYVolverAPlataformas();
        }
        else
        {
            Debug.Log($"<color=green>DDA:</color> Avanzando a la siguiente escena del puzzle.");
            SceneManager.LoadScene(siguienteEscenaIndex);
        }
    }

  private bool EvaluarSiDebeFinalizar()
{
    if (DDA_cuadriculaPiedra.Instancia == null) return false;
    RutaAsignada ruta = DDA_cuadriculaPiedra.Instancia.rutaActual;

    if (nivelActual == 8 && ruta == RutaAsignada.Media) return true; 

    return false;
}

    private void FinalizarYVolverAPlataformas()
    {
        if (memoria != null)
        {
            if (!memoria.puertasResueltas.Contains(memoria.idPuertaEnProgreso))
            {
                memoria.puertasResueltas.Add(memoria.idPuertaEnProgreso);
            }
            
            if (string.IsNullOrEmpty(memoria.nombreEscenaPrincipal))
            {
                SceneManager.LoadScene("Escenario3"); 
            }
            else
            {
                SceneManager.LoadScene(memoria.nombreEscenaPrincipal);
            }
        }
        else
        {
            Debug.LogError("Falta asignar el Archivo de Memoria en el Inspector de la Meta Inteligente.");
        }
    }
}