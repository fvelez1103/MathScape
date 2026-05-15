using JetBrains.Annotations;
using UnityEngine;

public class controlCamara : MonoBehaviour
{
    public Transform jugador;
    public Vector3 offset;
    public float cameraSpeed = 10f;
  
    void LateUpdate()
    {
        Vector3 desiredPosition = jugador.position + offset;

        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, cameraSpeed * Time.deltaTime);


        transform.position = jugador.position + offset;
    
        transform.LookAt(jugador);
    }
}