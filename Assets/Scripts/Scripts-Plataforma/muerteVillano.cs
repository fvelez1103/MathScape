using UnityEngine;
using UnityEngine.Events;

public class muerteVillano : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Asegúrate de que el villano tenga este Tag exacto.")]
    public string tagVillano = "NPC";

    [Header("Efectos de Sonido")]
    [Tooltip("El AudioSource debe estar pegado a este mismo objeto (la trampa/zona), no al villano.")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoCaidaVillano;
    [Range(0f, 1f)] public float volumenSonido = 1f;

    [Header("Eventos Adicionales (Opcional)")]
    [Tooltip("Aquí puedes arrastrar sonidos, partículas o scripts de victoria para que se activen cuando muera.")]
    public UnityEvent alMorirVillano;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagVillano))
        {
            Debug.Log("<color=red>¡Villano eliminado!</color> Ha tocado la zona de muerte.");

            if (fuenteSonido != null && sonidoCaidaVillano != null)
            {
                fuenteSonido.PlayOneShot(sonidoCaidaVillano, volumenSonido);
            }

            alMorirVillano?.Invoke();

            Destroy(other.gameObject);
        }
    }
}