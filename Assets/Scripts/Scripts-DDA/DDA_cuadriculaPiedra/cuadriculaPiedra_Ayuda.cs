using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class cuadricullaPiedra_Ayuda : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El GameObject que tiene la imagen de ayuda.")]
    public GameObject imagenSolucion; 
    
    private Image imagenUI; 

    [Header("Configuración de Tiempo")]
    public float tiempoVisible = 3f;
    public float tiempoDesvanecimiento = 1.5f;

    private void Awake()
    {
        if (imagenSolucion != null)
        {
            imagenUI = imagenSolucion.GetComponent<Image>();
            
            if (imagenUI == null)
            {
                Debug.LogWarning("DDA Puzzle: Tu 'imagenSolucion' no tiene un componente Image.");
            }
        }
    }

    private void OnEnable()
    {
        DDA_cuadriculaPiedra.OnMostrarAyuda += MostrarPista;

        if (imagenSolucion != null)
        {
            imagenSolucion.SetActive(false);
        }

        if (imagenUI != null)
        {
            Color c = imagenUI.color;
            c.a = 0f;
            imagenUI.color = c;
        }
    }

    private void OnDisable() 
    {
        DDA_cuadriculaPiedra.OnMostrarAyuda -= MostrarPista;
    }

    private void MostrarPista()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(RutinaPista());
        }
    }

    IEnumerator RutinaPista()
{
    if (imagenSolucion == null || imagenUI == null) yield break; 

    if (DDA_cuadriculaPiedra.Instancia != null)
    {
        DDA_cuadriculaPiedra.Instancia.RegistrarUsoDeAyuda();
    }
    Debug.Log("<color=yellow>DDA Puzzle:</color> Mostrando solución. Marcando ayuda en DDA.");
    
    imagenSolucion.SetActive(true);
    
    Color colorActual = imagenUI.color;
    colorActual.a = 1f; 
    imagenUI.color = colorActual;
    yield return new WaitForSeconds(tiempoVisible);
        
        float tiempoAnimacion = 0f;
        while (tiempoAnimacion < tiempoDesvanecimiento)
        {
            tiempoAnimacion += Time.deltaTime;
            float nuevoAlpha = Mathf.Lerp(1f, 0f, tiempoAnimacion / tiempoDesvanecimiento);
            
            colorActual.a = nuevoAlpha;
            imagenUI.color = colorActual;
            
            yield return null; 
        }

        colorActual.a = 0f;
        imagenUI.color = colorActual;
        imagenSolucion.SetActive(false);
    }
}