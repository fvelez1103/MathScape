using UnityEngine;
using UnityEngine.SceneManagement;
public class PasarNivel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player" ))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
        else if(other.CompareTag("MagnetCube"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }

}

