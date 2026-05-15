using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections; 

public class RastreadorSombra : MonoBehaviour
{
    [Header("Configuración Tesis (M6)")]
    public int nivelID; 

    [Header("Configuración")]
    public Transform rey;
    [Tooltip("Sincronizado con DDA_Laberinto (Normal: 1.7)")]
    public float velocidadSombra = 1.7f; 
    public float distanciaParaGuardarPaso = 0.5f;

    // --- NUEVO: EFECTOS DE SONIDO ---
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoAtrapar;
    [Range(0f, 1f)] public float volumenAtrapar = 1f;

    private List<Vector2> pasosDelRey = new List<Vector2>();
    private int pasoActualSombra = 0;
    private Vector2 ultimaPosicionGuardada;
    
    private bool puedePerseguir = false; 
    private SpriteRenderer miSprite;     
    private bool activado = false; 

    private void OnEnable() {
        DDA_Laberinto.OnCambioVelocidadSombra += AjustarVelocidad;
    }

    private void OnDisable() {
        DDA_Laberinto.OnCambioVelocidadSombra -= AjustarVelocidad;
    }

    void Start()
    {
        miSprite = GetComponent<SpriteRenderer>();
        miSprite.enabled = false; 
        
        if(DDA_Laberinto.Instancia != null)
        {
            velocidadSombra = DDA_Laberinto.Instancia.velocidadActualSombra;
            Debug.Log("<color=green>Rastreador:</color> Iniciando con velocidad: " + velocidadSombra);
        }

        transform.position = rey.position;
        ultimaPosicionGuardada = rey.position;
        pasosDelRey.Add(ultimaPosicionGuardada);
    }

    private void AjustarVelocidad(float nuevaVel) {
        velocidadSombra = nuevaVel;
        Debug.Log("<color=yellow>Rastreador:</color> Mi velocidad ha sido ajustada a " + nuevaVel);
    }

    public void IniciarDesdeTrigger()
    {
        if (!activado)
        {
            activado = true;
            miSprite.enabled = true; 
            puedePerseguir = true;
        }
    }

    void Update()
    {
        if (rey == null || !activado) return;

        if (Vector2.Distance(rey.position, ultimaPosicionGuardada) > distanciaParaGuardarPaso)
        {
            ultimaPosicionGuardada = rey.position;
            pasosDelRey.Add(ultimaPosicionGuardada);
        }

        if (!puedePerseguir) return;

        Vector2 destino;
        if (pasoActualSombra < pasosDelRey.Count)
        {
            destino = pasosDelRey[pasoActualSombra];
            transform.position = Vector2.MoveTowards(transform.position, destino, velocidadSombra * Time.deltaTime);

            if (Vector2.Distance(transform.position, destino) < 0.1f)
            {
                pasoActualSombra++;
            }
        }
        else
        {
            destino = rey.position;
            transform.position = Vector2.MoveTowards(transform.position, destino, velocidadSombra * Time.deltaTime);
        }

        if (destino.x < transform.position.x - 0.01f) miSprite.flipX = true; 
        else if (destino.x > transform.position.x + 0.01f) miSprite.flipX = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && puedePerseguir)
        {
            puedePerseguir = false; 

            Player scriptJugador = collision.GetComponent<Player>();
            if (scriptJugador != null)
            {
                scriptJugador.Muerte();
            }

            StartCoroutine(RutinaAtraparJugador());
        }
    }

    private IEnumerator RutinaAtraparJugador()
    {
        float tiempoEspera = 1.5f;

        if (fuenteSonido != null && sonidoAtrapar != null)
        {
            fuenteSonido.PlayOneShot(sonidoAtrapar, volumenAtrapar);
            tiempoEspera = sonidoAtrapar.length + 0.2f; 
        }

        if (DDA_Laberinto.Instancia != null)
        {
            DDA_Laberinto.Instancia.RegistrarMuerteNPC();
        }

        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.M6_RegistrarMuerteVillano(nivelID);
        }

        yield return new WaitForSecondsRealtime(tiempoEspera);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- NUEVO MÉTODO PARA DETENERLO DESDE AFUERA ---
    public void DetenerSombra()
    {
        puedePerseguir = false;
    }
}