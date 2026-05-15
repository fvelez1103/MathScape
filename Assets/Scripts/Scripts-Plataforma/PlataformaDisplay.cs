using UnityEngine;
using TMPro;

public class PlatformValueDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro valueText;

    [Header("Configuración Tesis")]
    public int targetValue; // Cuánto peso debería tener esta plataforma según el nivel

    private int currentValue = 0;

    void Start()
    {
        InicializarValor();
    }

    // Tu lógica original de hijos directos
    public void InicializarValor()
    {
        currentValue = 0;

        foreach (Transform child in transform)
        {
            SnapPoint snap = child.GetComponent<SnapPoint>();
            // Si el SnapPoint tiene un bloque (childCount > 0)
            if (snap != null && child.childCount > 0)
            {
                pesoObjeto scriptPeso = child.GetComponentInChildren<pesoObjeto>();
                if (scriptPeso != null) currentValue += (int)scriptPeso.peso; 
                else currentValue += 1; 
            }
            // Si es un objeto de peso puesto directamente (sin snap)
            else if (snap == null) 
            {
                pesoObjeto scriptPeso = child.GetComponent<pesoObjeto>();
                if (scriptPeso != null) currentValue += (int)scriptPeso.peso;
            }
        }
        UpdateText();
    }

    public void AddValue(int amount)
    {
        currentValue += amount;
        UpdateText();
    }

    public int GetValue() => currentValue;

    void UpdateText()
    {
        if (valueText != null)
            valueText.text = currentValue.ToString();
    }
}