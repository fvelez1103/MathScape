using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaMinijuego : MonoBehaviour
{
    [Header("Conexión con la Memoria")]
    public MemoriaDelJuego memoria; 
    public string idPuerta = "Puerta_Nivel1_01"; 
    public string camaraDeEstaZona;

    [Header("Intervención DDA (Tesis)")]
    public bool esPuertaVariableDDA = false;
    public string escenaRutaCorta; 
    public string escenaRutaMedia; 
    public string escenaRutaLarga; 
    public string escenaNormal; 

    // --- NUEVO: SONIDO DE APERTURA ---
    [Header("Efectos de Sonido")]
    public AudioClip sonidoPuertaAbierta;
    public float volumenSonido = 1f;

    private Transform jugador;
    private bool cerca = false;

    // --- EL CERROJO ---
    private bool yaViajando = false; 

    void Start()
    {
        // Revisamos si esta puerta ya fue resuelta
        if (memoria != null && memoria.puertasResueltas.Contains(idPuerta))
        {
            // Verificamos si el jugador ACABA de regresar de resolver ESTA puerta
            if (memoria.idPuertaEnProgreso == idPuerta)
            {
                // Reproducimos el sonido de forma independiente para que no se corte
                if (sonidoPuertaAbierta != null)
                {
                    AudioSource.PlayClipAtPoint(sonidoPuertaAbierta, transform.position, volumenSonido);
                }
                
                // Limpiamos la memoria de progreso para que no vuelva a sonar si el jugador recarga la escena
                memoria.idPuertaEnProgreso = ""; 
            }

            // Desaparecemos la puerta
            gameObject.SetActive(false); 
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null) jugador = playerObj.transform;
    }

    void Update()
    {
        if (yaViajando || jugador == null) return;

        cerca = Vector3.Distance(transform.position, jugador.position) <= 3f;
        
        if (cerca && Input.GetKeyDown(KeyCode.E)) 
        {
            ViajarAlMinijuego();
        }
    }

    void ViajarAlMinijuego()
    {
        yaViajando = true; 

        string escenaACargar = escenaNormal;

        if (esPuertaVariableDDA && DDA_cuadriculaPiedra.Instancia != null)
        {
            var ruta = DDA_cuadriculaPiedra.Instancia.rutaActual;
            if (ruta == RutaAsignada.Corta) escenaACargar = escenaRutaCorta;
            else if (ruta == RutaAsignada.Media) escenaACargar = escenaRutaMedia;
            else if (ruta == RutaAsignada.Larga) escenaACargar = escenaRutaLarga;
        }
        
        CronometroM1 cronometro = Object.FindAnyObjectByType<CronometroM1>();
        if (cronometro != null)
        {
            cronometro.GuardarTiempoAntesDeSalir();
        }

        memoria.ultimaPosicionJugador = jugador.position;
        memoria.idPuertaEnProgreso = idPuerta; // <-- Aquí guardamos la ID justo antes de irnos
        memoria.nombreEscenaPrincipal = SceneManager.GetActiveScene().name;
        memoria.nombreCamaraRetorno = camaraDeEstaZona;

        Debug.Log($"<color=green><b>[Puerta]</b></color> Viajando a {escenaACargar}. Entrada bloqueada para evitar spam.");
        SceneManager.LoadScene(escenaACargar);
    }
}