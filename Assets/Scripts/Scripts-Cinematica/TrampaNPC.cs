using UnityEngine;

public class TrampaNPC : MonoBehaviour
{
    public GameObject puertaCelda;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            if (puertaCelda != null)
            {
                puertaCelda.SetActive(true); // Al activarse, la puerta hará sonar su propio script
            }

            gameObject.SetActive(false); // La trampa se apaga sin interrumpir el sonido
        }
    }
}