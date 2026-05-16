using UnityEngine;

public class Temporizador_Rompecabeza : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camara1;
    
    [Header("Datos")]
    public float tiempoAcumulado = 0f;
    private bool nivelTerminado = false;

    void Update()
    {
        if (nivelTerminado) return;

        if (camara1 != null && camara1.gameObject.activeInHierarchy)
        {
            tiempoAcumulado += Time.unscaledDeltaTime; 
        }
    }

    public void TerminarNivel()
    {
        nivelTerminado = true;
        float tiempoFinalFormateado = (float)System.Math.Round(tiempoAcumulado, 2);
        
        if (FirebaseManager.Instancia != null) {
            FirebaseManager.Instancia.M4_RegistrarTiempo(tiempoFinalFormateado);
        }
    }
}