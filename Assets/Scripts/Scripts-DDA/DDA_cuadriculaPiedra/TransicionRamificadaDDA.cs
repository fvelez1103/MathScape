using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionRamificadaDDA : MonoBehaviour
{
    public int indexRutaCorta;         
    public int indexRutaMediaLarga; 
    public int nivelID;
   private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<MagneticEntity>() != null)
        {
            MetricasPuzzleNivel metricas = Object.FindAnyObjectByType<MetricasPuzzleNivel>();
            if (metricas != null) metricas.FinalizarNivel();

            // 1. CAPTURAR EN FIREBASE PRIMERO
            if (FirebaseManager.Instancia != null) FirebaseManager.Instancia.M4_CapturarNivel(nivelID);

            // 2. LUEGO TERMINAR EN DDA
            if (DDA_cuadriculaPiedra.Instancia != null)
                DDA_cuadriculaPiedra.Instancia.RegistrarFinDeNivel();

            int siguiente = indexRutaMediaLarga;
            // ... (resto de tu código)
            if (DDA_cuadriculaPiedra.Instancia != null && DDA_cuadriculaPiedra.Instancia.rutaActual == RutaAsignada.Corta)
            {
                siguiente = indexRutaCorta;
            }

            SceneManager.LoadScene(siguiente);
        }
    }
}