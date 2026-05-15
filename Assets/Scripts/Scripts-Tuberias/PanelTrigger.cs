using UnityEngine;

public class PanelTrigger : MonoBehaviour
{
    private CameraManagerNivel4 cameraManager;

    [System.Obsolete]
    void Start()
    {
        cameraManager = FindObjectOfType<CameraManagerNivel4>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        cameraManager.ActivarZonaPanel();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        cameraManager.VolverAlPlayer();
    }
}
