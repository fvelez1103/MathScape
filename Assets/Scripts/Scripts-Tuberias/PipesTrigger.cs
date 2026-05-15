using UnityEngine;

public class PipesTrigger : MonoBehaviour
{
    public FlowRouter junction;

    private bool playerNear = false;
    private bool used = false;

    void Update()
    {
        if (!playerNear || used) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;
            junction.Execute();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}
