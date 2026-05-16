using UnityEngine;

[System.Serializable]
public struct CancionAjustada
{
    public AudioClip cancion;
    [Range(0f, 1f)] public float volumenIdeal;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia;
    private AudioSource audioSource;

    [Header("Configura tus 3 canciones aquí")]
    public CancionAjustada[] listaDeCanciones;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (nuevaMusica == null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            return;
        }

        float volumenEncontrado = 0.5f;
        foreach (CancionAjustada item in listaDeCanciones)
        {
            if (item.cancion == nuevaMusica)
            {
                volumenEncontrado = item.volumenIdeal;
                break;
            }
        }

        if (audioSource.clip == nuevaMusica) 
        {
            audioSource.volume = volumenEncontrado;
            return; 
        }

        audioSource.clip = nuevaMusica;
        audioSource.volume = volumenEncontrado;
        audioSource.Play();
    }
}