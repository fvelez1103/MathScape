using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PasoControlador : MonoBehaviour
{
    public TextMeshProUGUI numeroPasoTexto;
    public TextMeshProUGUI accionTexto;
    public TextMeshProUGUI valorActualTexto;
    public Image fondoImagen;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoEstadoIntermedio;
    
    [Header("Panel de Explicación Integrado")]
    public GameObject panelExplicacionObj; // Arrastra aquí el "PanelExplicacion"
    public TextMeshProUGUI textoExplicacionTMP; // Arrastra aquí el "TextoExplicacion"

    // 🟢 NOTA: Se añadió "string explicacion" al final de esta función
    public void ConfigurarPaso(int indice, string accion, string estadoFormula, string explicacion)
    {
        numeroPasoTexto.text = indice.ToString();
        accionTexto.text = accion;
        textoEstadoIntermedio.text = estadoFormula;
        
        // Al inicio, ocultamos el texto gris (o lo dejamos vacío)
        textoEstadoIntermedio.gameObject.SetActive(false); 
        valorActualTexto.text = "0";

        // 🟢 Asignamos el texto y ocultamos el panel de explicación al inicio
        if (textoExplicacionTMP != null)
        {
            textoExplicacionTMP.text = "💡 Explicación matemática:\n" + explicacion;
        }
        
        if (panelExplicacionObj != null)
        {
            panelExplicacionObj.SetActive(false); // Nace colapsado
        }
    }

    public void ActualizarValor(string nuevoValor)
    {
        valorActualTexto.text = nuevoValor;
    }

    public void MostrarResultadoSimplificado(string resultado)
    {
        if (textoEstado != null)
        {
            textoEstado.text = resultado;
            textoEstado.color = Color.green;
        }
    }

    public void RevelarEstadoGris()
    {
        textoEstadoIntermedio.gameObject.SetActive(true);

        // 🟢 Desplegamos la explicación al acertar el paso
        if (panelExplicacionObj != null)
        {
            panelExplicacionObj.SetActive(true);
        }
    }
}