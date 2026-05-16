using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cámara principal (una sola activa)")]
    public List<Camera> camarasPrincipales;
    private Camera currentCamera;

    [Header("Cámaras secundarias (pueden coexistir)")]
    public List<Camera> camarasSecundarias;

    void Start()
    {
        ActivarCamaraPrincipal(camarasPrincipales[0]);

        foreach (Camera cam in camarasSecundarias)
        {
            cam.gameObject.SetActive(false);
        }
    }

    public void ActivarCamaraPrincipal(Camera nuevaCamara)
    {
        if (currentCamera == nuevaCamara) return;

        foreach (Camera cam in camarasPrincipales)
        {
            cam.gameObject.SetActive(false);
        }

        nuevaCamara.gameObject.SetActive(true);
        currentCamera = nuevaCamara;
    }

    public void ActivarCamaraSecundaria(Camera camara)
    {
        camara.gameObject.SetActive(true);
    }

    public void DesactivarCamaraSecundaria(Camera camara)
    {
        camara.gameObject.SetActive(false);
    }
}
