using UnityEngine;

public class TutorialContextual : MonoBehaviour
{
    [Header("Configuración UI")]
    public GameObject cartelTutorial; 
    public bool ocultarAlSalir = true; 

    [Header("Condición de Éxito")]
    public KeyCode teclaParaCompletar = KeyCode.None; 
    public bool esCompletado = false; 

    private bool jugadorEnZona = false;

    void Start()
    {
        if (cartelTutorial != null) cartelTutorial.SetActive(false);
    }

    void Update()
    {
        if (jugadorEnZona && !esCompletado && Input.GetKeyDown(teclaParaCompletar))
        {
            CompletarTutorial();
        }
    }

    void CompletarTutorial()
    {
        esCompletado = true;
        
        if (cartelTutorial != null) cartelTutorial.SetActive(false);
        
        Debug.Log("Tutorial aprendido: " + gameObject.name);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !esCompletado)
        {
            jugadorEnZona = true;
            if (cartelTutorial != null) cartelTutorial.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
            if (ocultarAlSalir && cartelTutorial != null)
            {
                cartelTutorial.SetActive(false);
            }
        }
    }
}