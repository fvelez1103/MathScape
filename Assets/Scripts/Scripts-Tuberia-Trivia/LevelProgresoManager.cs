using UnityEngine;

public class LevelProgresoManager : MonoBehaviour
{
    public static LevelProgresoManager Instancia;

    [Header("Configuración del Nivel")]
    [SerializeField] private int triviasNecesarias = 3;
    [SerializeField] private GameObject puertaADesaparecer;

    private int triviasResueltasActuales = 0;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
    }

    public void RegistrarTriviaCompletada()
    {
        triviasResueltasActuales++;
        Debug.Log($"Progreso: {triviasResueltasActuales} / {triviasNecesarias}");

        if (triviasResueltasActuales >= triviasNecesarias)
        {
            AbrirPuerta();
        }
    }

    private void AbrirPuerta()
    {
        if (puertaADesaparecer != null)
        {
            puertaADesaparecer.SetActive(false);
            Debug.Log("¡Puerta abierta! Todas las trivias completadas.");
        }
    }
}