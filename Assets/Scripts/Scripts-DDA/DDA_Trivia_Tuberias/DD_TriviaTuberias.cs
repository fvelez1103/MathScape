using UnityEngine;

public class DDA_TriviaTuberias : MonoBehaviour
{
    public static DDA_TriviaTuberias Instancia;

    [Header("Variables de Entrada (Crisp)")]
    public float tiempoUltimaPregunta;
    public int fallosEnPregunta;
    public bool usoAyuda5050;

    [Header("Resultado Mamdani")]
    [Tooltip("Nivel de dominio actual (0 a 100)")]
    public float nivelDominio = 50f;

    private void Awake() 
    {
        if (Instancia == null) {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private float TrapecioIzquierdo(float x, float a, float b) => x <= a ? 1 : (x >= b ? 0 : (b - x) / (b - a));
    private float Triangulo(float x, float a, float b, float c) => Mathf.Max(0, Mathf.Min((x - a) / (b - a), (c - x) / (c - b)));
    private float TrapecioDerecho(float x, float a, float b) => x <= a ? 0 : (x >= b ? 1 : (x - a) / (b - a));

    public string CalcularSiguienteDificultad(string dificultadActual)
    {
        float tiempoRapido = TrapecioIzquierdo(tiempoUltimaPregunta, 8, 15);
        float tiempoLento = TrapecioDerecho(tiempoUltimaPregunta, 20, 30);

        float perfecto = fallosEnPregunta == 0 ? 1 : 0;
        float conErrores = fallosEnPregunta > 0 ? 1 : 0;

        float penalizacionAyuda = usoAyuda5050 ? 0.5f : 1.0f;
        float dominioAlto = Mathf.Min(tiempoRapido, perfecto) * penalizacionAyuda;
        float dominioBajo = Mathf.Max(tiempoLento, conErrores);
        float dominioMedio = Triangulo(tiempoUltimaPregunta, 12, 20, 28);

        nivelDominio = (dominioAlto * 80 + dominioMedio * 50 + dominioBajo * 20) / 
                       Mathf.Max(0.01f, (dominioAlto + dominioMedio + dominioBajo));

        Debug.Log($"<color=cyan>DDA Trivia:</color> Dominio: {nivelDominio:F2} | Próxima Ayuda en: {ObtenerTiempoParaAyuda()}s");

        if (nivelDominio > 65) return SubirDificultad(dificultadActual);
        if (nivelDominio < 35) return BajarDificultad(dificultadActual);
        return dificultadActual;
    }

    public float ObtenerTiempoParaAyuda()
    {
        return Mathf.Lerp(10f, 40f, nivelDominio / 100f);
    }

    private string SubirDificultad(string actual) {
        if (actual == "Facil") return "Medio";
        return "Dificil";
    }

    private string BajarDificultad(string actual) {
        if (actual == "Dificil") return "Medio";
        return "Facil";
    }
}