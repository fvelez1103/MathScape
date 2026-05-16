using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionesCambioNivel : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 1f;
    [Tooltip("El índice (Build Settings) de la siguiente escena a cargar")]
    [SerializeField] private int sceneToLoad; 

    [Header("Intervención DDA")]
    [Tooltip("Desmarca esto en el M1 para que el DDA ignore el tiempo de ese nivel.")]
    public bool registrarEnDDA = true;

    private bool isTransitioning = false;
    public int nivelID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;
        
        bool isPlayer = other.CompareTag("Player");
        bool isMagneticBlock = other.GetComponent<MagneticEntity>() != null;
        
        if (isPlayer || isMagneticBlock)
        {
            isTransitioning = true;
            
            MetricasPuzzleNivel metricas = Object.FindAnyObjectByType<MetricasPuzzleNivel>();
            if (metricas != null) 
            {
                metricas.FinalizarNivel();
            }
            
            if (registrarEnDDA)
            {
                if (FirebaseManager.Instancia != null) {
                    FirebaseManager.Instancia.M4_CapturarNivel(nivelID);
                }
                
                if (DDA_cuadriculaPiedra.Instancia != null) {
                    DDA_cuadriculaPiedra.Instancia.RegistrarFinDeNivel();
                }
            }
            
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