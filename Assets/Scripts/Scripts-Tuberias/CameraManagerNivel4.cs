using UnityEngine;

public class CameraManagerNivel4 : MonoBehaviour
{
    public Camera camaraPlayer;
    public Camera camaraAgua;
    public Camera camaraPanel;

    void Start()
    {
        camaraPlayer.gameObject.SetActive(true);
        camaraAgua.gameObject.SetActive(false);
        camaraPanel.gameObject.SetActive(false);
    }

    public void ActivarZonaPanel()
    {
        camaraPlayer.gameObject.SetActive(false);
        camaraAgua.gameObject.SetActive(true);
        camaraPanel.gameObject.SetActive(true);
    }

    public void VolverAlPlayer()
    {
        camaraAgua.gameObject.SetActive(false);
        camaraPanel.gameObject.SetActive(false);
        camaraPlayer.gameObject.SetActive(true);
    }
}
