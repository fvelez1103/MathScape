using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transiciones : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private int sceneToLoad; 

    [Header("Configuración")]
    public bool esNivelConMetricas = true;
    public enum TipoMinijuego { Ninguno, M1_Plataformas, M6_Laberinto }
    public TipoMinijuego minijuegoActual;
    public int nivelID;

    private float tiempoInicioNivel;
    private bool isTransitioning = false;

    private void Start()
    {
        tiempoInicioNivel = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTransitioning && (other.CompareTag("Player") || other.CompareTag("MagnetCube")))
        {
            isTransitioning = true;
            RegistrarDatos();
            StartCoroutine(SceneLoad(sceneToLoad));
        }
    }

    private void RegistrarDatos()
    {
        if (FirebaseManager.Instancia == null || !esNivelConMetricas) return;

        if (minijuegoActual == TipoMinijuego.M1_Plataformas)
        {
            FirebaseManager.Instancia.M1_CapturarDDA(nivelID);
        }
        else if (minijuegoActual == TipoMinijuego.M6_Laberinto)
        {
            float tiempoTotal = Time.time - tiempoInicioNivel;
            float tiempoRedondeado = (float)System.Math.Round(tiempoTotal, 2);
            
            FirebaseManager.Instancia.M6_FinalizarLaberinto(nivelID, tiempoRedondeado);
            FirebaseManager.Instancia.M6_CapturarDDA(nivelID); 
        }
    }

    IEnumerator SceneLoad(int sceneIndex)
    {
        if (transitionAnimator != null) transitionAnimator.SetTrigger("FadeOut");    
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
    }
}