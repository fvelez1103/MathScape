using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillanoIA : MonoBehaviour
{
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip Spawn;
    [Range(0f, 1f)] public float volumenSonido = 1f;

    public Transform jugador;
    public float velocidad = 3.5f;
    private bool puedeSeguir = false;
    private SpriteRenderer sr;
    private Animator anim; 

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); 
        
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) jugador = playerObj.transform;
        }
    }

    void Update()
    {
        if (puedeSeguir && jugador != null)
        {
            Vector3 posicionObjetivo = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
            
            float distancia = Vector3.Distance(transform.position, posicionObjetivo);

            if (distancia > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, velocidad * Time.deltaTime);
                
                if (anim != null) anim.SetBool("estaCorriendo", true);
            }
            else
            {
                if (anim != null) anim.SetBool("estaCorriendo", false);
            }
            
            float anguloCamaraY = Camera.main.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, anguloCamaraY, 0);

            if (sr != null)
            {
                sr.flipX = (jugador.position.x < transform.position.x);
            }
        }
    }

    public void IniciarPersecucion()
    {
        puedeSeguir = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float tiempoEspera = 1.2f;

            if (fuenteSonido != null && Spawn != null)
            {
                fuenteSonido.PlayOneShot(Spawn, volumenSonido);
                tiempoEspera = Spawn.length;
            }

            StartCoroutine(RestartAfterDelay(tiempoEspera));
        }
    }

    private IEnumerator RestartAfterDelay(float tiempoEspera)
    {
        yield return new WaitForSecondsRealtime(tiempoEspera);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}