using UnityEngine;

public class DisparadorSombra : MonoBehaviour
{
    public RastreadorSombra scriptSombra;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            scriptSombra.IniciarDesdeTrigger();
            gameObject.SetActive(false); 
        }
    }
}