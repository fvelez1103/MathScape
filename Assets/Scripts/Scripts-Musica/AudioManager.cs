using UnityEngine;

// 1. Creamos esta estructura para "unir" una canción con su volumen ideal en el Inspector
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
    public CancionAjustada[] listaDeCanciones; // Esto aparecerá en el Inspector

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

        // 2. Buscamos la canción en nuestra lista para ver qué volumen le toca
        float volumenEncontrado = 0.5f; // Volumen por defecto
        foreach (CancionAjustada item in listaDeCanciones)
        {
            if (item.cancion == nuevaMusica)
            {
                volumenEncontrado = item.volumenIdeal;
                break; // Encontramos la canción, dejamos de buscar
            }
        }

        // 3. Aplicamos la lógica de cambio de música con el volumen correcto
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