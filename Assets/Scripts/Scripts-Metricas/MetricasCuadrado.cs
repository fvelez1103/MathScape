using UnityEngine;

public class MetricasCuadrado : MonoBehaviour
{
    private float tiempoInicio;
    private float tiempoAcumulado = 0f;
    private bool cronometroActivo = false;
    private bool juegoTerminado = false;

    public void IniciarCronometro()
    {
        if (juegoTerminado) return; 

        if (!cronometroActivo)
        {
            tiempoInicio = Time.time;
            cronometroActivo = true;
            Debug.Log("<color=orange>M4: Cronómetro de cuadrado INICIADO/REANUDADO</color>");
        }
    }
    public void PausarCronometro()
    {
        if (cronometroActivo && !juegoTerminado)
        {
            tiempoAcumulado += Time.time - tiempoInicio;
            cronometroActivo = false;
            Debug.Log($"<color=orange>M4: Cronómetro PAUSADO. Tiempo acumulado: {tiempoAcumulado}s</color>");
        }
    }

    public void RegistrarVictoriaCuadrado()
    {
        if (juegoTerminado || !cronometroActivo) return;

        juegoTerminado = true;
        
        tiempoAcumulado += Time.time - tiempoInicio;
        cronometroActivo = false;

        float tiempoRedondeado = (float)System.Math.Round(tiempoAcumulado, 2);

        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.M4_RestarComprobacionFantasma();
            
            FirebaseManager.Instancia.M4_RegistrarTiempo(tiempoRedondeado);
            
            Debug.Log($"<color=orange>M4: Cuadrado completado en {tiempoRedondeado}s y enviado a Firebase</color>");
        }
    }
}