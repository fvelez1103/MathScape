using UnityEngine;
using UnityEngine.SceneManagement;

public class MetricasPuzzleNivel : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    public int nivelID; 

    private float tiempoInicioNivel;
    private bool nivelCompletado = false;

    private static int acumuladorR = 0;
    private static int nivelActualID = -1;

    private int contadorZ = 0;

    void Start()
    {
        if (nivelActualID != nivelID)
        {
            acumuladorR = 0;
            nivelActualID = nivelID;
        }

        tiempoInicioNivel = Time.time;
        nivelCompletado = false;
        contadorZ = 0;
    }

    void Update()
    {
        if (nivelCompletado) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            contadorZ++;
            // Reportamos inmediatamente a Firebase
            if (FirebaseManager.Instancia != null) 
                FirebaseManager.Instancia.M2_RegistrarTeclaZ(nivelID);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            acumuladorR++;
            // Reportamos inmediatamente a Firebase
            if (FirebaseManager.Instancia != null) 
                FirebaseManager.Instancia.M2_RegistrarReinicio(nivelID);
        }
    }

   public void FinalizarNivel()
   {
        if (nivelCompletado) return;

        nivelCompletado = true;
        
        // --- CAMBIO CLAVE AQUÍ ---
        // Le pedimos al DDA el tiempo histórico total (que incluye todos los reinicios)
        float tiempoTotalReal = 0f;
        
        if (DDA_cuadriculaPiedra.Instancia != null)
        {
            tiempoTotalReal = DDA_cuadriculaPiedra.Instancia.tiempoTotalHistoricoNivel;
        }
        else
        {
            // Respaldo por si pruebas el nivel suelto sin el DDA activo
            tiempoTotalReal = Time.time - tiempoInicioNivel; 
        }

        // Formateamos para Firebase
        float tiempoOrdenado = (float)System.Math.Round(tiempoTotalReal, 2);
        if (tiempoOrdenado <= 0) tiempoOrdenado = 0.01f;
        // -------------------------

        if (FirebaseManager.Instancia != null)
        {
            // Ahora enviamos el tiempo real absoluto
            FirebaseManager.Instancia.M2_RegistrarNivel(nivelID, tiempoOrdenado);
            FirebaseManager.Instancia.M2_CapturarNivel(nivelID);
            
            Debug.Log($"<color=magenta>M2 UNIFICADO: Nivel {nivelID} enviado.</color>");
            Debug.Log($"<color=cyan>Tiempo Total Real (Con reinicios): {tiempoOrdenado}s</color>");
            
            acumuladorR = 0;
        }
   }
}