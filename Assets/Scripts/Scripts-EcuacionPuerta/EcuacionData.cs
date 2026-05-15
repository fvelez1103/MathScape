using UnityEngine;

[CreateAssetMenu(fileName = "NuevaEcuacion", menuName = "Minijuego/Ecuacion")]
public class EcuacionData : ScriptableObject 
{
    [Header("Ecuación Original")]
    public Sprite[] imagenesProgreso;
    
    [Header("Configuración de Pasos")]
    [Tooltip("Lo que se le pide al jugador (ej. 4x / 3 =)")]
    public string[] acciones;
    
    [Tooltip("La respuesta correcta que debe escribir el jugador (ej. 20)")]
    public string[] resultados;
    
    [Tooltip("Feedback visual intermedio (ej. 4x / 3 = 20)")]
    public string[] estadosIntermedios;

    [Header("Pedagogía y Ayudas")]
    [Tooltip("Explicación que aparece al ACERTAR el paso")]
    public string[] explicacionesMatematicas;
    
    [Tooltip("Pista que aparecerá si el jugador falla mucho (Para futuro sistema DDA)")]
    public string[] pistas; 
}