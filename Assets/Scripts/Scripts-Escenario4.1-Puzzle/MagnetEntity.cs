using UnityEngine;

public abstract class MagneticEntity : MonoBehaviour
{
    public Transform AttachedTo { get; private set; }

    public bool IsAttached => AttachedTo != null;

    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoAdhesion;
    [Range(0f, 1f)] public float volumenAdhesion = 1f;

    public virtual void AttachTo(Transform target, Vector3 direction)
    {
        AttachedTo = target;

        Vector3 offset = direction.normalized;
        offset = new Vector3(Mathf.Round(offset.x), Mathf.Round(offset.y), 0f);

        transform.position = target.position + offset;

        if (fuenteSonido != null && sonidoAdhesion != null)
        {
            fuenteSonido.pitch = Random.Range(0.9f, 1.1f);
            fuenteSonido.PlayOneShot(sonidoAdhesion, volumenAdhesion);
        }
    }

    public virtual void Detach()
    {
        AttachedTo = null;
    }
}