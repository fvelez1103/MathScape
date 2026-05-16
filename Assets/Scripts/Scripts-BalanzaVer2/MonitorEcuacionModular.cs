using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MonitorEcuacionModular : MonoBehaviour
{
    public enum TipoMision { IgualarAyB, ActivarEspecifica, Personalizado }

    [Header("Conexión con la Balanza")]
    public BalanzaModular balanza; 

    [Header("UI Elements")]
    public TextMeshProUGUI textoEcuacion;
    public TextMeshProUGUI textGuia;

    [Header("Tamaños de Fuente")]
    public float tamanoTextoEcuacion = 5f;
    public float tamanoGuiaNormal = 4f;
    public float tamanoGuiaPistaSutil = 3f;
    public float tamanoGuiaPistaObvia = 2.7f;

    [Header("Colores de Feedback")]
    public Color colorDesbalance = Color.red;
    public Color colorEquilibrio = Color.green;
    public Color colorPista = Color.yellow;

    [Header("Configuración de Misión")]
    public TipoMision misionActual = TipoMision.IgualarAyB;
    public string variableObjetivo = "B";

    private bool ayudaTextoReportada = false;
    private bool ayudaVisualReportada = false;

    void Update()
    {
        if (balanza == null || textoEcuacion == null || textGuia == null) return;

        // Aplicamos el tamaño de la ecuación constantemente
        textoEcuacion.fontSize = tamanoTextoEcuacion;

        string textoIzquierdo = ConstruirTextoLado(balanza.ladoIzquierdo);
        int pesoIzquierdo = ObtenerPeso(balanza.ladoIzquierdo);
        string textoDerecho = ConstruirTextoLado(balanza.ladoDerecho);
        int pesoDerecho = ObtenerPeso(balanza.ladoDerecho);

        bool iguales = (pesoIzquierdo == pesoDerecho && pesoIzquierdo > 0);

        if (iguales)
        {
            if (DDA_DifusoPuro.Instancia != null && !DDA_DifusoPuro.Instancia.objetivoCompletado)
            {
                DDA_DifusoPuro.Instancia.objetivoCompletado = true;
                Debug.Log("<color=lime>DDA:</color> Ecuación resuelta. Cronómetro de frustración detenido.");
            }

            textoEcuacion.text = $"{textoIzquierdo} = {textoDerecho}";
            textoEcuacion.color = colorEquilibrio;
            
            textGuia.text = "Plataformas Igualadas";
            textGuia.color = colorEquilibrio;
            textGuia.fontSize = tamanoGuiaNormal; // Tamaño normal
        }
        else
        {
            if (DDA_DifusoPuro.Instancia != null && DDA_DifusoPuro.Instancia.objetivoCompletado)
            {
                DDA_DifusoPuro.Instancia.objetivoCompletado = false;
                Debug.Log("<color=orange>DDA:</color> Ecuación desequilibrada de nuevo. Cronómetro reanudado.");
            }

            string simbolo = (pesoIzquierdo == pesoDerecho) ? "=" : "≠";
            textoEcuacion.text = $"{textoIzquierdo} {simbolo} {textoDerecho}"; 

            if (DDA_DifusoPuro.Instancia != null)
            {
                EstadoJugador estado = DDA_DifusoPuro.Instancia.estadoActual;

                if (estado == EstadoJugador.Frustrado || estado == EstadoJugador.Bloqueado)
                {
                    textoEcuacion.color = colorPista;
                    textGuia.color = colorPista;
                    textGuia.fontSize = tamanoGuiaPistaObvia; // Tamaño para texto largo
                    textGuia.text = "Usa los bloques parpadeantes para igualar la balanza";

                    if (!ayudaVisualReportada && FirebaseManager.Instancia != null) {
                        FirebaseManager.Instancia.M1_RegistrarAyudaVisual(CronometroM1.nivelActualID);
                        ayudaVisualReportada = true;
                        ayudaTextoReportada = true; 
                    }
                }
                else if (estado == EstadoJugador.Enganchado)
                {
                    textoEcuacion.color = colorPista;
                    textGuia.color = colorPista;
                    textGuia.fontSize = tamanoGuiaPistaSutil; // Tamaño intermedio
                    
                    if (pesoIzquierdo > pesoDerecho) textGuia.text = "Quita peso de la izquierda o añade a la derecha";
                    else if (pesoDerecho > pesoIzquierdo) textGuia.text = "La balanza necesita más peso en el lado izquierdo";
                    else textGuia.text = "Estás cerca, revisa los valores";

                    if (!ayudaTextoReportada && FirebaseManager.Instancia != null) {
                        FirebaseManager.Instancia.M1_RegistrarAyudaTexto(CronometroM1.nivelActualID);
                        ayudaTextoReportada = true;
                    }
                }
                else 
                {
                    textoEcuacion.color = colorDesbalance;
                    textGuia.color = colorDesbalance;
                    textGuia.fontSize = tamanoGuiaNormal; // Tamaño normal
                    ActualizarTextoGuiaSegunMision();
                }
            }
        }
    }

    void ActualizarTextoGuiaSegunMision()
    {
        switch (misionActual)
        {
            case TipoMision.IgualarAyB: textGuia.text = "Iguala las plataformas A y B"; break;
            case TipoMision.ActivarEspecifica: textGuia.text = $"Añade un bloque en la plataforma {variableObjetivo}"; break;
            case TipoMision.Personalizado: textGuia.text = variableObjetivo; break;
        }
    }

    string ConstruirTextoLado(List<PlatformValueDisplay> lado)
    {
        if (lado == null || lado.Count == 0) return "0";
        List<string> valores = new List<string>();
        foreach (var plat in lado) if (plat != null) valores.Add(plat.GetValue().ToString());
        return valores.Count == 0 ? "0" : string.Join(" + ", valores);
    }

    int ObtenerPeso(List<PlatformValueDisplay> lado)
    {
        int total = 0;
        foreach (var plat in lado) if (plat != null) total += plat.GetValue();
        return total;
    }
}