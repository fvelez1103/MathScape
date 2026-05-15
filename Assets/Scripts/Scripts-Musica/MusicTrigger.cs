using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [Tooltip("Arrastra aquí la canción que debe sonar en este conjunto de niveles")]
    public AudioClip musicaDeEsteNivel;

    private void Start()
    {
        // Apenas carga la escena, le avisa al AudioManager qué pista debe sonar
        if (AudioManager.Instancia != null)
        {
            AudioManager.Instancia.CambiarMusica(musicaDeEsteNivel);
        }
    }
}