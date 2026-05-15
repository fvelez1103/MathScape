using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoriaMinijuego : MonoBehaviour
{
    [Header("Conexión con la Memoria")]
    public MemoriaDelJuego memoria;

    [Header("Métricas de Tesis")]
    [Tooltip("Arrastra aquí el objeto de la escena que tiene el script MetricasPuzzleNivel")]
    public MetricasPuzzleNivel scriptMetricas;
    public int nivelID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<MagneticEntity>() != null) GanarYVolver();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<MagneticEntity>() != null) GanarYVolver();
    }

    public void GanarYVolver()
    {        
        if (scriptMetricas != null)
        {
            scriptMetricas.FinalizarNivel();
        }

        if (FirebaseManager.Instancia != null) FirebaseManager.Instancia.M4_CapturarNivel(nivelID);

        if (DDA_cuadriculaPiedra.Instancia != null)
        {
            DDA_cuadriculaPiedra.Instancia.RegistrarFinDeNivel();
        }

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
            Debug.LogError("Falta asignar el Archivo de Memoria en el Inspector.");
        }
    }
}