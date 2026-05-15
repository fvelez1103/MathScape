using UnityEngine;

public class ValveControl : MonoBehaviour
{
    public RadicalProcessor module;
    public bool controlsX;
    public float step = 1f;

    private bool playerNear = false;

    void Update()
    {
        if (!playerNear || module == null) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (controlsX) module.x += step;
            else module.y += step;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (controlsX) module.x -= step;
            else module.y -= step;
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
