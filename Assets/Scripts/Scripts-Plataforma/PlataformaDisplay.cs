using UnityEngine;
using TMPro;

public class PlatformValueDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro valueText;

    [Header("Configuración Tesis")]
    public int targetValue;

    private int currentValue = 0;

    void Start()
    {
        InicializarValor();
    }

    public void InicializarValor()
    {
        currentValue = 0;

        foreach (Transform child in transform)
        {
            SnapPoint snap = child.GetComponent<SnapPoint>();
            if (snap != null && child.childCount > 0)
            {
                pesoObjeto scriptPeso = child.GetComponentInChildren<pesoObjeto>();
                if (scriptPeso != null) currentValue += (int)scriptPeso.peso; 
                else currentValue += 1; 
            }
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