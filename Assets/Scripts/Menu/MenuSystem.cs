using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 3f;
    [SerializeField] private int sceneToLoad; 

    public AudioSource fuenteSonido;
    public AudioClip sonido;
    [Range(0f, 1f)] public float volumenSonido = 1f;

    private bool isTransitioning = false;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Jugar()
    {
        if (isTransitioning) return; 
        
        StartCoroutine(JugarCoroutine());
    }

    private IEnumerator JugarCoroutine()
    {
        isTransitioning = true;

        float tiempoEspera = 0f;
        
        if (fuenteSonido != null && sonido != null)
        {
            fuenteSonido.PlayOneShot(sonido, volumenSonido);
            tiempoEspera = sonido.length;
        }

        yield return new WaitForSecondsRealtime(tiempoEspera);
        StartCoroutine(SceneLoad(sceneToLoad));
    }

    public void SalirJuego()
    {
        Application.Quit();
    }

    IEnumerator SceneLoad(int sceneIndex)
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("FadeOut");
        }

        yield return new WaitForSecondsRealtime(transitionTime);

        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneIndex);
    }
}