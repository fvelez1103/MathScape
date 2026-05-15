using UnityEngine;

public enum MathOperator
{
    Add,
    Subtract,
    Multiply,
    Divide,
    OpenParenthesis,  
    CloseParenthesis   
}
public class OperatorBox : MagneticEntity
{
    [SerializeField] private MathOperator op;
    public MathOperator Operator => op;

    [Header("Sprites de Paréntesis")]
    [SerializeField] private Sprite spriteAbrir; 
    [SerializeField] private Sprite spriteCerrar;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity; 
    }

   public void ActualizarApariencia(bool esIzquierdaOArriba)
{
    if (op == MathOperator.OpenParenthesis || op == MathOperator.CloseParenthesis)
    {
        op = esIzquierdaOArriba ? MathOperator.OpenParenthesis : MathOperator.CloseParenthesis;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = esIzquierdaOArriba ? spriteAbrir : spriteCerrar;
        }
    }
}

    public string GetSymbol()
    {
        return op switch
        {
            MathOperator.Add => "+",
            MathOperator.Subtract => "-",
            MathOperator.Multiply => "×",
            MathOperator.Divide => "÷",
            MathOperator.OpenParenthesis => "(",
            MathOperator.CloseParenthesis => ")",
            _ => "?"
        };
    }
}