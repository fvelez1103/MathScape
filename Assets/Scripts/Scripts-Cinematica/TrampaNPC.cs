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
                puertaCelda.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}