using UnityEngine;
using TMPro;

public class ContextualPromptManager : MonoBehaviour
{
    public GameObject promptE; 
    public GameObject promptCarga;
    
    private bool estaCercaDeCubo = false;
    private bool estaCargandoCubo = false;

    void Start()
    {
        promptE.SetActive(false);
        promptCarga.SetActive(false);
    }

    public void SetCercaDeCubo(bool valor)
    {
        estaCercaDeCubo = valor;
        ActualizarUI();
    }

    public void SetCargandoCubo(bool valor)
    {
        estaCargandoCubo = valor;
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (estaCargandoCubo)
        {
            promptE.SetActive(false);
            promptCarga.SetActive(true);
        }
        else if (estaCercaDeCubo)
        {
            promptE.SetActive(true);
            promptCarga.SetActive(false);
        }
        else
        {
            promptE.SetActive(false);
            promptCarga.SetActive(false);
        }
    }
}