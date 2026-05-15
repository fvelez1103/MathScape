using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 
public class ContadorCorrutina : MonoBehaviour
{
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip Spawn;
    [Range(0f, 1f)] public float volumenSonido = 1f;

    [Header("Referencias del Villano")]
    public VillanoIA villano;
    public Transform puntoAparicionVillano;

    [Header("Control de Progresión")]
    public GameObject muroInvisible; 
    public bool palancaAccionada = false; 

    [Header("Cámaras")]
    public GameObject camaraJugador;       
    public GameObject camaraVillano;    
    public GameObject camaraPalanca;    

    private bool yaSeActivo = false;

    void Start()
    {
        if (muroInvisible != null) 
        {
            muroInvisible.SetActive(true);
            
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            Collider muroCol = muroInvisible.GetComponent<Collider>();
            
            foreach (GameObject npc in npcs)
            {
                Collider npcCol = npc.GetComponent<Collider>();
                if (npcCol != null && muroCol != null)
                {
                    Physics.IgnoreCollision(muroCol, npcCol);
                }
            }
            Debug.Log("Muro activo: Bloqueando al Jugador, permitiendo paso a NPCs.");
        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaSeActivo)
        {
            yaSeActivo = true;
            if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;
            StartCoroutine(SecuenciaFinal());
        }

        if (other.CompareTag("Player") && palancaAccionada && other.gameObject == muroInvisible)
        {
            SceneManager.LoadScene("MenuPrincipal"); 
        }
    }

    IEnumerator SecuenciaFinal()
    {
        
        yield return new WaitForSeconds(3f);
        if (villano != null && puntoAparicionVillano != null)
        {
            villano.transform.position = puntoAparicionVillano.position;
            villano.gameObject.SetActive(true);
            AlternarCamara(camaraVillano, true);
            yield return new WaitForSeconds(0.1f);
            if (fuenteSonido != null && Spawn != null)
        {
            fuenteSonido.PlayOneShot(Spawn, volumenSonido);
        }
            yield return new WaitForSeconds(2f);
            villano.IniciarPersecucion();
            AlternarCamara(camaraVillano, false);
        }
        
    }

    public void RegistrarPalancaActivada()
    {
        palancaAccionada = true;
        Debug.Log("Objetivo completado: El puente ha caído.");
        
        if (muroInvisible != null)
        {
            muroInvisible.GetComponent<Collider>().isTrigger = true;
        }

        MostrarCaidaPuente();
    }

    public void MostrarCaidaPuente()
    {
        StopAllCoroutines(); 
        StartCoroutine(RutinaCamaraTemporal(camaraPalanca, 3f));
    }

    private void AlternarCamara(GameObject camEspecial, bool activa)
    {
        if (camEspecial == null || camaraJugador == null) return;
        camaraJugador.SetActive(!activa);
        camEspecial.SetActive(activa);
    }

    IEnumerator RutinaCamaraTemporal(GameObject camEspecial, float tiempo)
    {
        AlternarCamara(camEspecial, true);
        yield return new WaitForSeconds(tiempo);
        AlternarCamara(camEspecial, false);
    }

    
}