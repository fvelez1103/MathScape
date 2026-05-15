using UnityEngine;

public class OperatorButton : MonoBehaviour
{
    public void OnButtonPressed(int operatorIndex)
    {
        OperatorType chosenOp = (OperatorType)operatorIndex;
        
        if (PipeSocket.ActiveSocket != null)
        {
            PipeSocket.ActiveSocket.ReceivePlayerChoice(chosenOp);
        }
        else
        {
            Debug.LogError("Ningún socket está activo, pero se presionó un botón.");
        }
    }
}