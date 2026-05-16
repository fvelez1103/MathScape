using UnityEngine;

public class cogerObjetos : MonoBehaviour
{
    [Header("Conexión con el Cerebro")]
    public BalanzaModular cerebroBalanza;

    [Header("Configuración del Jugador")]
    public GameObject handPoint;
    public Transform plataformaB;
    public bool puedeAgarrar = true;

    [Header("Interfaz de Usuario (HUD)")]
    public GameObject promptE;      
    public GameObject promptRQ;      

    [Header("Métricas M1 (Tesis)")]
    [Tooltip("El ID del nivel actual (ej. 11, 12, 13) para Firebase")]
    public int nivelID = 0; 
    private bool primerMovimientoRegistrado = false;
    private float tiempoInicioNivel;

    private GameObject pickedObject;
    private bool cercaDeObjeto = false;
    
    void Start()
    {
        if(promptE) promptE.SetActive(false);
        if(promptRQ) promptRQ.SetActive(false);

        tiempoInicioNivel = Time.unscaledTime;
    }

    [System.Obsolete]
    void Update()
    {
        if (cerebroBalanza != null && cerebroBalanza.nivelResuelto)
        {
            puedeAgarrar = false;
            ActualizarHUD(); 

            if (pickedObject != null) SoltarObjeto();
            return; 
        }

        if (pickedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.R)) EnviarAPlataformaB();
            if (Input.GetKeyDown(KeyCode.Q)) SoltarObjeto();
        }

        ActualizarHUD();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("objeto")) cercaDeObjeto = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("objeto")) cercaDeObjeto = false;
    }

    void ActualizarHUD()
    {
        if (cerebroBalanza != null && cerebroBalanza.nivelResuelto)
        {
            promptE.SetActive(false);
            promptRQ.SetActive(false);
            return;
        }

        if (pickedObject != null)
        {
            promptE.SetActive(false);
            promptRQ.SetActive(true);
        }
        else if (cercaDeObjeto && puedeAgarrar)
        {
            promptE.SetActive(true);
            promptRQ.SetActive(false);
        }
        else
        {
            promptE.SetActive(false);
            promptRQ.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!puedeAgarrar) return;
        if (!other.CompareTag("objeto")) return;
        if (pickedObject != null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TomarObjeto(other.gameObject);
        }
    }

    void TomarObjeto(GameObject obj)
    {
        if (!primerMovimientoRegistrado && nivelID != 0)
        {
            primerMovimientoRegistrado = true;
            float tiempoTardado = Time.unscaledTime - tiempoInicioNivel;
            if (FirebaseManager.Instancia != null) {
                FirebaseManager.Instancia.M1_RegistrarPrimerMovimiento(nivelID, tiempoTardado);
                Debug.Log($"<color=cyan>M1 DDA:</color> Primer movimiento registrado en {tiempoTardado:F2}s");
            }
        }

        Rigidbody rb = obj.GetComponentInParent<Rigidbody>();
        if (rb != null) 
        {
            obj = rb.gameObject; 
        }

        pesoObjeto po = obj.GetComponent<pesoObjeto>();
        SnapPoint snapActual = obj.GetComponentInParent<SnapPoint>();

        if (po != null && snapActual != null)
        {
            po.origenLogico = snapActual;
        }

        Collider col = obj.GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        if (col != null) col.enabled = false;

        obj.transform.SetParent(handPoint.transform);
        obj.transform.localPosition = Vector3.zero;

        pickedObject = obj;
        GestionarFantasmas(true);
    }

    [System.Obsolete]
    void SoltarObjeto()
    {
        Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
        Collider col = pickedObject.GetComponent<Collider>();

        pickedObject.transform.SetParent(null);

        if (col != null) col.enabled = true;
        
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero; 
            rb.WakeUp(); 
        }

        RaycastHit hit;
        if (Physics.Raycast(pickedObject.transform.position, Vector3.down, out hit, 5f))
        {
            if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("objeto"))
            {
                pickedObject.transform.SetParent(hit.collider.transform);
            }
        }

        pickedObject = null;
        GestionarFantasmas(false);
    }

    void EnviarAPlataformaB()
    {
        Transform snapLibre = ObtenerSnapPointLibre();
        if (snapLibre == null) return;

        SnapPoint snapDestino = snapLibre.GetComponent<SnapPoint>();
        if (snapDestino == null) return;

        pesoObjeto po = pickedObject.GetComponent<pesoObjeto>();

        if (po != null && po.origenLogico != null)
        {
            po.origenLogico.QuitarObjetoLogico(pickedObject);
            po.origenLogico = null; 
        }

        Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
        Collider col = pickedObject.GetComponent<Collider>();

        if (col != null) col.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        snapDestino.ColocarObjeto(pickedObject);

        pickedObject = null;
        GestionarFantasmas(false);
    }

    Transform ObtenerSnapPointLibre()
    {
        foreach (Transform snap in plataformaB)
        {
            if (snap.childCount == 0) return snap;
        }
        return null;
    }

    void GestionarFantasmas(bool activar)
    {
        foreach (Transform snapChild in plataformaB)
        {
            SnapPoint snap = snapChild.GetComponent<SnapPoint>();
            if (snap != null) snap.ActivarFantasma(activar);
        }
    }

    public void BloquearAgarrarNuevos()
    {
        puedeAgarrar = false;
    }

    public bool EstaSosteniendoObjeto()
    {
        return pickedObject != null;
    }
}