using UnityEngine;

public class CronometroM1 : MonoBehaviour
{
    [Header("Configuración")]
    public int nivelID;

    private static float alcanciaTiempoHistorico = 0f;
    private static float tiempoAcumuladoIntentoBueno = 0f;
    private static int ultimoNivelID = -1;
    
    public static int nivelActualID = -1; 

    private float tiempoInicioIntento;
    private bool yaRegistrado = false;
    private bool tiempoYaGuardadoEnEsteTramo = false;

    void Start()
    {
        nivelActualID = nivelID; 

        if (ultimoNivelID != nivelID)
        {
            alcanciaTiempoHistorico = 0f;
            tiempoAcumuladoIntentoBueno = 0f;
            ultimoNivelID = nivelID;
            Debug.Log($"<color=white><b>[Cronómetro M1]</b></color> Nivel Nuevo {nivelID}. Memorias reseteadas.");
        }

        tiempoInicioIntento = Time.unscaledTime;
        Debug.Log($"<color=white><b>[Cronómetro M1]</b></color> Intento iniciado. Alcancía: {alcanciaTiempoHistorico:F2}s | Intento Bueno lleva: {tiempoAcumuladoIntentoBueno:F2}s");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !yaRegistrado && !tiempoYaGuardadoEnEsteTramo)
        {
            float delta = Time.unscaledTime - tiempoInicioIntento;
            alcanciaTiempoHistorico += delta;
            
            if (FirebaseManager.Instancia != null) {
                FirebaseManager.Instancia.M1_SumarTiempoHistorico(nivelID, delta);
            }

            tiempoAcumuladoIntentoBueno = 0f;
            tiempoYaGuardadoEnEsteTramo = true;

            Debug.Log("<color=orange><b>[REINICIO]</b></color> Tiempo al historial. Intento bueno reseteado.");
        }
    }

    public void GuardarTiempoAntesDeSalir()
    {
        if (tiempoYaGuardadoEnEsteTramo) return;

        float delta = Time.unscaledTime - tiempoInicioIntento;
        alcanciaTiempoHistorico += delta;
        tiempoAcumuladoIntentoBueno += delta;
        
        if (FirebaseManager.Instancia != null) {
            FirebaseManager.Instancia.M1_SumarTiempoHistorico(nivelID, delta);
        }

        tiempoYaGuardadoEnEsteTramo = true;
        Debug.Log($"<color=yellow><b>[PUERTA]</b></color> Saliendo al puzzle. Intento bueno salvado: {tiempoAcumuladoIntentoBueno:F2}s");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaRegistrado) return;

        if (other.CompareTag("Player"))
        {
            if (Time.unscaledTime - tiempoInicioIntento < 1f) return;

            yaRegistrado = true;

            float tiempoEsteTramo = Time.unscaledTime - tiempoInicioIntento;
            
            float tiempoIntentoBueno = tiempoAcumuladoIntentoBueno + tiempoEsteTramo;
            float tiempoTotalHistorico = alcanciaTiempoHistorico + tiempoEsteTramo;

            float buenoRedondeado = (float)System.Math.Round(tiempoIntentoBueno, 2);
            float totalRedondeado = (float)System.Math.Round(tiempoTotalHistorico, 2);

            if (FirebaseManager.Instancia != null)
            {
                FirebaseManager.Instancia.M1_FinalizarNivel(nivelID, buenoRedondeado);
                FirebaseManager.Instancia.M1_SumarTiempoHistorico(nivelID, (float)System.Math.Round(tiempoEsteTramo, 2));
            }

            Debug.Log($"<color=cyan><b>=== ENVÍO FINAL M1 ===</b></color>\n" +
                      $"<b>INTENTO BUENO REAL:</b> {buenoRedondeado}s\n" +
                      $"<b>TIEMPO TOTAL EN ESCENA:</b> {totalRedondeado}s");

            alcanciaTiempoHistorico = 0f;
            tiempoAcumuladoIntentoBueno = 0f;
        }
    }
}