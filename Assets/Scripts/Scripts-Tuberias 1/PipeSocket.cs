using UnityEngine;

public class PipeSocket : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject operatorMenuUI;
    public MonoBehaviour playerMovementScript; 
    public RadicalLevelManager levelManager;
    private bool isUsed = false;

    private bool isPlayerNear = false;
    private float menuOpenTime;
    
    public static PipeSocket ActiveSocket; 

    void Update()
    {
        if (isUsed) return; 

        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !operatorMenuUI.activeSelf)
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        ActiveSocket = this;
        playerMovementScript.enabled = false;
        operatorMenuUI.SetActive(true);
        menuOpenTime = Time.time; 
    }

    public void ReceivePlayerChoice(OperatorType choice)
    {
        float decisionTime = Time.time - menuOpenTime;
        Debug.Log($"[Métrica DDA] Op: {choice} | Tiempo: {decisionTime:F2}s");

        operatorMenuUI.SetActive(false);
        playerMovementScript.enabled = true;
        ActiveSocket = null;

        bool operationSuccess = levelManager.ProcessOperator(choice);

        if (operationSuccess)
        {
            ShutDownSocket();
        }
    }

    private void ShutDownSocket()
    {
        isUsed = true;
        
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            mesh.material.color = Color.gray; 
        }
        else
        {
            Debug.LogWarning("No se encontró MeshRenderer en la tubería.", this);
        }

        Debug.Log("Socket bloqueado permanentemente tras su uso.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = false;
    }
}