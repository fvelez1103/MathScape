using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionesCambioNivelPuzzle1 : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 1f;
    [Tooltip("El índice (Build Settings) de la siguiente escena a cargar")]
    [SerializeField] private int sceneToLoad; 

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        bool isPlayer = other.CompareTag("Player");
        bool isMagneticBlock = other.GetComponent<MagneticEntity>() != null;

        if (isPlayer || isMagneticBlock)
        {
            isTransitioning = true;
            
            // 1. Cierra las métricas locales de tu nivel
            MetricasPuzzleNivel metricas = Object.FindAnyObjectByType<MetricasPuzzleNivel>();
            if (metricas != null) 
            {
                metricas.FinalizarNivel();
            }

            // 2. Avisa al DDA (Crucial para que el tutorial cuente 1/3, 2/3 y 3/3)
            if (DDA_cuadriculaPiedra.Instancia != null)
            {
                DDA_cuadriculaPiedra.Instancia.RegistrarFinDeNivel();
            }

            // 3. Inicia la transición visual y cambia de escena
            StartCoroutine(SceneLoad(sceneToLoad));
        }
    }

    IEnumerator SceneLoad(int sceneIndex)
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(transitionTime);
        }
        
        SceneManager.LoadScene(sceneIndex);
    }
}