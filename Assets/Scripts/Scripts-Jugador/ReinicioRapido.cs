using UnityEngine;
using UnityEngine.SceneManagement;

public class ReinicioRapido : MonoBehaviour
{
    [Header("Configuración")]
    public KeyCode teclaReinicio = KeyCode.F;

    [Header("Configuración (M1)")]
    public int nivelID;

    void Update()
    {
        if (Input.GetKeyDown(teclaReinicio))
        {
            ReiniciarNivel();
        }
    }

    void ReiniciarNivel()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.M1_RegistrarFalla(nivelID);
            Debug.Log($"<color=yellow>Falla (Reinicio F) registrada en Nivel {nivelID}</color>");
        }
        if (DDA_DifusoPuro.Instancia != null)
        {
            DDA_DifusoPuro.Instancia.RegistrarReseteoManual();
        }
        
        SceneManager.LoadScene(escenaActual);
    }
}