using UnityEngine;
using TMPro;

public class EquationMonitor : MonoBehaviour
{
    public PlatformValueDisplay displayA;
    public PlatformValueDisplay displayB;
    
    [Header("UI Elements")]
    public TextMeshProUGUI textoEcuacion;
    public TextMeshProUGUI textGuia;

    [Header("Colores de Feedback")]
    public Color colorDesbalance = Color.red;
    public Color colorEquilibrio = Color.green;

    void Update()
{
    int valA = displayA.GetValue();
    int valB = displayB.GetValue();

    if (valA == valB && valA != 0)
    {
        // CASO: IGUALDAD
        textoEcuacion.text = $"{valA} = {valB}";
        textGuia.text = "Igualdad alcanzada";
        textGuia.color = colorEquilibrio;
        textoEcuacion.color = colorEquilibrio;
    }
    else
    {
        textoEcuacion.text = $"{valA} ≠ {valB}"; 
        textGuia.text = "Encuentra la igualdad";
        textoEcuacion.color = colorDesbalance;
        textGuia.color = colorDesbalance;
    }
}
}