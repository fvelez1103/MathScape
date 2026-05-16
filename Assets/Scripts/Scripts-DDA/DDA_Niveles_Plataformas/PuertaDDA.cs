using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaDDA : MonoBehaviour
{
    [Header("Configuración de Rutas (Macro-DDA)")]
    public string escenaFacil = "salaBalanzas-1.1";
    public string escenaMedio = "salaBalanzas-1.2";
    public string escenaDificil = "salaBalanzas-1.3";

    [Header("Configuración de Físicas")]
    public string tagJugador = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            TeletransportarJugador();
        }
    }

    public void TeletransportarJugador()
    {
        float tiempoTranscurrido = Time.timeSinceLevelLoad;
        
        int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60);
        int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60);
        string tiempoFormateado = string.Format("{0:00}:{1:00}", minutos, segundos);

        if (DDA_DifusoPuro.Instancia != null)
        {
            EstadoJugador estadoFinal = DDA_DifusoPuro.Instancia.estadoActual;
            float frustracionFinal = DDA_DifusoPuro.Instancia.nivelFrustracionCrisp;

            Debug.Log($"<color=green><b>[REPORTE FINAL DE NIVEL]</b></color>\n" +
                      $"Tiempo: {tiempoFormateado} ({tiempoTranscurrido:F2}s)\n" +
                      $"Estado: {estadoFinal}\n" +
                      $"Valor Crisp: {frustracionFinal}");
            DDA_DifusoPuro.Instancia.LimpiarDDA();

            switch (estadoFinal)
            {
                case EstadoJugador.Bloqueado:
                case EstadoJugador.Frustrado:
                    SceneManager.LoadScene(escenaFacil);
                    break;
                case EstadoJugador.Enganchado:
                    SceneManager.LoadScene(escenaMedio);
                    break;
                case EstadoJugador.Optimo:
                    SceneManager.LoadScene(escenaDificil);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Puerta DDA: No se encontró el motor DDA.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}