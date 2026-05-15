using UnityEngine;
using UnityEngine.SceneManagement; 

public class ZonaMuerte : MonoBehaviour
{
    [Header("Configuración para activar")]
    public bool registrarEnMetricasM1 = true;
    public int nivelID;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player")) 
        {
            if (registrarEnMetricasM1 && FirebaseManager.Instancia != null)
            {
                FirebaseManager.Instancia.M1_RegistrarFalla(nivelID);
                Debug.Log($"<color=red>M1: Falla (Caída) registrada en Nivel {nivelID}</color>");
            }
            else
            {
                Debug.Log("<color=yellow>M1: Caída detectada pero NO registrada (Interruptor apagado).</color>");
            }
            if (DDA_DifusoPuro.Instancia != null)
            {
                DDA_DifusoPuro.Instancia.RegistrarCaida();
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}