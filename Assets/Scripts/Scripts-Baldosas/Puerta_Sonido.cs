using UnityEngine;

public class SonidoPuerta : MonoBehaviour
{
    [Header("Configuración de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoAlAparecer;
    [Range(0f, 1f)] public float volumen = 1f;

    private void OnEnable()
    {
        if (fuenteSonido != null && sonidoAlAparecer != null)
        {
            fuenteSonido.PlayOneShot(sonidoAlAparecer, volumen);
            Debug.Log("<color=green>Audio:</color> La puerta se ha activado y el sonido está sonando.");
        }
    }
}