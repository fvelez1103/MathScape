using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events; 

public class RadicalLevelManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshPro equationDisplay;
    public TextMeshPro equationDisplay1;
    public TextMeshPro equationDisplay2;
    public TextMeshProUGUI equationDisplayCanvas;

    [Header("Eventos Físicos del Nivel")]
    public UnityEvent OnLevelWon;     
    public UnityEvent OnFatalError;   

    [Header("Configuración del Nivel")]
    public string startNodeID;
    public List<EquationNode> levelNodes = new List<EquationNode>();

    private EquationNode currentNode;
    private bool isAnimating = false; 

    void Start()
    {
        currentNode = levelNodes.Find(n => n.nodeID == startNodeID);
        UpdateUI();
    }

    public bool ProcessOperator(OperatorType appliedOp)
    {
        if (isAnimating) 
        {
            Debug.LogWarning("Ignorando input: Animación de tubería en progreso.");
            return false;
        }

        NodeTransition transition = currentNode.transitions.Find(t => t.requiredOperator == appliedOp);

        if (transition != null)
        {
            EquationNode nextNode = levelNodes.Find(n => n.nodeID == transition.targetNodeID);
            if (nextNode != null)
            {
                if (transition.pipeToRotate != null)
                {
                    StartCoroutine(RotatePipeSmoothly(transition, nextNode));
                }
                else
                {
                    CommitTransition(nextNode);
                }
                return true;
            }
        }
        
        Debug.LogWarning("Operación inválida para este estado.");
        return false;
    }

    private IEnumerator RotatePipeSmoothly(NodeTransition transition, EquationNode nextNode)
    {
        isAnimating = true;
        Transform pipe = transition.pipeToRotate;
        
        Quaternion startRot = pipe.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(transition.rotationAngles);
        
        float elapsed = 0f;
        float duration = transition.rotationDuration > 0f ? transition.rotationDuration : 0.5f;

        while (elapsed < duration)
        {
            pipe.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        pipe.localRotation = endRot; 
        isAnimating = false;
        
        CommitTransition(nextNode);
    }

    private void CommitTransition(EquationNode nextNode)
    {
        currentNode = nextNode;
        UpdateUI();
        EvaluateState();
    }

    private void UpdateUI()
    {
        if (currentNode == null) return;

        if (equationDisplay != null) equationDisplay.text = currentNode.equationText;
        if (equationDisplay1 != null) equationDisplay1.text = currentNode.equationText;
        if (equationDisplay2 != null) equationDisplay2.text = currentNode.equationText;
        
        if (equationDisplayCanvas != null) equationDisplayCanvas.text = currentNode.equationText;
    }

    private void EvaluateState()
    {
        if (currentNode.isFatalError)
        {
            OnFatalError.Invoke(); 
        }
        else if (currentNode.isWinState)
        {
            OnLevelWon.Invoke();
        }
    }
}