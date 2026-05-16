using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ExponentCanvasController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject canvasUI;
    public MonoBehaviour playerMovementScript;
    public TextMeshProUGUI equationText; 
    public GameObject primerBoton;

    [Header("Diseño del Puzzle")]
    [Tooltip("El texto de la ecuación en cada etapa. Ej: [0] x^2 * x^3, [1] x^(2+3), [2] x^5")]
    public string[] equationSteps;
    
    [Tooltip("La contraseña. El ID numérico de los botones que deben presionarse en orden.")]
    public int[] correctButtonSequence;

    private int currentStepIndex = 0;

    public void AbrirCanvas()
    {
        canvasUI.SetActive(true);
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        currentStepIndex = 0;
        ActualizarTexto();
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(primerBoton);
    }

    public void EvaluarBoton(int buttonID)
    {
        if (currentStepIndex >= correctButtonSequence.Length) return;

        if (buttonID == correctButtonSequence[currentStepIndex])
        {
            currentStepIndex++;
            
            if (currentStepIndex >= correctButtonSequence.Length)
            {
                ActualizarTexto();
                CompletarMinijuego();
            }
            else
            {
                ActualizarTexto();
            }
        }
        else
        {
            Debug.LogWarning("El jugador falló la secuencia. Castigo aplicado.");
            currentStepIndex = 0;
            ActualizarTexto();
        }
    }

    private void ActualizarTexto()
    {
        if (currentStepIndex < equationSteps.Length && equationText != null)
        {
            equationText.text = equationSteps[currentStepIndex];
        }
    }

    private void CompletarMinijuego()
    {
        Debug.Log("Minijuego superado.");
        
        canvasUI.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }
}