using UnityEngine;
using TMPro;

public class EquationUI : MonoBehaviour
{
    public RadicalProcessor module;
    public TextMeshPro equationText;

    void Update()
    {
        if (module == null) return;

        float x = module.x;
        float y = module.y;
        float c = module.c;

        equationText.text =
            $"x = {x}\n" +
            $"y = {y}\n\n" +
            $"√(x + y) = {Mathf.Sqrt(Mathf.Max(0, x + y)):0.00}";
    }
}
