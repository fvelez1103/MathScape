using System.Collections.Generic;
using UnityEngine;

public enum OperatorType { None, Square, Sub4, Div2, SplitRoot }

[System.Serializable]
public class EquationNode 
{
    public string nodeID;
    public string equationText;
    public bool isWinState;
    public bool isFatalError;
    public List<NodeTransition> transitions = new List<NodeTransition>();
}

[System.Serializable]
public class NodeTransition 
{
    public OperatorType requiredOperator;
    public string targetNodeID;
    public bool isSuboptimal;

    [Header("Feedback Visual 3D")]
    [Tooltip("La tubería 3D que rotará al elegir esta opción")]
    public Transform pipeToRotate; 
    
    [Tooltip("Rotación a aplicar en los ejes X, Y, Z (ej. 0, 90, 0)")]
    public Vector3 rotationAngles;
    
    [Tooltip("Duración de la animación en segundos")]
    public float rotationDuration = 0.5f;
}