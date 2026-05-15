using UnityEngine;
using UnityEngine.SceneManagement; 

public class ResetManager : MonoBehaviour
{
    public void EjecutarRetorno()
    {
        string nombreEscenaActual = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(nombreEscenaActual);
    }
}