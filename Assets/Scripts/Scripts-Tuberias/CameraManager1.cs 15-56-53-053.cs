using System.Collections.Generic;
using UnityEngine;

public class CameraManager12 : MonoBehaviour
{
    public List<Camera> camaras; private Camera currentCamera;
    void Start()
    { ActivarCamara(camaras[0]); }
    public void ActivarCamara(Camera nuevaCamara)
    {
        if (currentCamera == nuevaCamara)
            return;
        foreach (Camera cam in camaras)
        { cam.gameObject.SetActive(false); }
        nuevaCamara.gameObject.SetActive(true);
        currentCamera = nuevaCamara;
    }
}