using UnityEngine;
public class CameraTriggerNiveles : MonoBehaviour 
{ public Camera camaraAsignada; 
private CameraManagerNiveles cameraManager; 
[System.Obsolete] void Start() { cameraManager = FindObjectOfType<CameraManagerNiveles>(); 
} 
void OnTriggerEnter(Collider other) 
{ if (other.CompareTag("Player")) 
{ cameraManager.ActivarCamara(camaraAsignada); } } }