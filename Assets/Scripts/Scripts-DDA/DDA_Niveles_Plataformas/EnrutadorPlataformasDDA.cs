using UnityEngine;
using UnityEngine.SceneManagement;

public class EnrutadorPlataformasDDA : MonoBehaviour
{
    [Header("Conexión con Memoria")]
    public MemoriaDelJuego memoria;

    [Header("Ruta Óptima (Salto al Nivel 3)")]
    public string escenaNivel3;

    [Header("Ruta Media (Al Nivel 2)")]
    public string escenaNivel2;

    [Header("Ruta Mala (Versión Fácil)")]
    public string escenaNivel2Facil;

    [Header("Métricas")]
    public int nivelID = 11; 

    private bool enTransicion = false;

    private void OnTriggerEnter(Collider other)
    {
        if (enTransicion) return;

        if (other.CompareTag("Player"))
        {
            enTransicion = true;
            string escenaACargar = DeterminarRuta();
            
            if (memoria != null)
            {
                memoria.ultimaPosicionJugador = Vector3.zero;
            }
            if (FirebaseManager.Instancia != null) {
                FirebaseManager.Instancia.M1_CapturarDDA(nivelID);
            }

            Debug.Log($"<color=yellow>Enrutador:</color> Cargando {escenaACargar}.");
            SceneManager.LoadScene(escenaACargar);
        }
    }
    private string DeterminarRuta()
    {
        if (DDA_DifusoPuro.Instancia == null) return escenaNivel2; 
        EstadoJugador estado = DDA_DifusoPuro.Instancia.estadoActual;

        switch (estado)
        {
            case EstadoJugador.Optimo:
                return escenaNivel3;
            case EstadoJugador.Enganchado:
                return escenaNivel2;
            case EstadoJugador.Frustrado:
            case EstadoJugador.Bloqueado:
                return escenaNivel2Facil;
            default:
                return escenaNivel2;
        }
    }
}