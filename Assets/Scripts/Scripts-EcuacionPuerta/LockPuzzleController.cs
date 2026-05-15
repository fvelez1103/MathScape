using UnityEngine;

public class CerraduraTrigger : MonoBehaviour
{
    public GameObject canvasMinijuego; 
    public GameObject objetoPuerta;    
    public float distanciaInteraccion = 3f;
    
    public MonoBehaviour playerMovementScript; 

    private GameObject jugador;
    private bool estaCerca = false;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        if (canvasMinijuego != null) canvasMinijuego.SetActive(false);
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.transform.position);
        estaCerca = distancia <= distanciaInteraccion;

        if (estaCerca && Input.GetKeyDown(KeyCode.E) && !canvasMinijuego.activeSelf)
        {
            ActivarMinijuego();
        }
    }

    void ActivarMinijuego()
    {
        canvasMinijuego.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (playerMovementScript != null) playerMovementScript.enabled = false;
    }

    public void PausarMinijuego()
    {
        if (canvasMinijuego != null) canvasMinijuego.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }

    public void FinalizarReto()
    {
        if (canvasMinijuego != null) canvasMinijuego.SetActive(false);

        if (objetoPuerta != null) 
        {
            Destroy(objetoPuerta); 
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (playerMovementScript != null) playerMovementScript.enabled = true;

        this.enabled = false; 
    }
}