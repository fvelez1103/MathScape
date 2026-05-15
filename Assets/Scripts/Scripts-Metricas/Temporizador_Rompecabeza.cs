using UnityEngine;

public class Temporizador_Rompecabeza : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camara1; // Arrastra tu cámara aquí en el inspector
    
    [Header("Datos")]
    public float tiempoAcumulado = 0f;
    private bool nivelTerminado = false;

    void Update()
    {
        if (nivelTerminado) return;

        // Solo suma tiempo si la cámara existe y está activa (es decir, estamos jugando M4)
        if (camara1 != null && camara1.gameObject.activeInHierarchy)
        {
            // Usamos unscaledDeltaTime por si el juego llega a estar en pausa general
            tiempoAcumulado += Time.unscaledDeltaTime; 
        }
    }

    public void TerminarNivel()
    {
        nivelTerminado = true;
        // Redondeamos para Firebase
        float tiempoFinalFormateado = (float)System.Math.Round(tiempoAcumulado, 2);
        
        if (FirebaseManager.Instancia != null) {
            FirebaseManager.Instancia.M4_RegistrarTiempo(tiempoFinalFormateado);
        }
    }
}