using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlataformaControlada
{
    [Tooltip("El modelo 3D (Transform) de esta plataforma")]
    public Transform modelo;
    
    [Tooltip("La altura 'Y' exacta a la que debe quedar cuando el jugador gane (Equilibrio)")]
    public float alturaEquilibrio = 5.0f;
    
    [Tooltip("Cuántos metros baja o sube esta plataforma por cada punto de desbalance. (Pon 0 si no quieres que se mueva)")]
    public float sensibilidadPeso = 0.5f;
}

public class BalanzaModular : MonoBehaviour
{
    [Header("Matemática: ¿Qué suma el peso?")]
    public List<PlatformValueDisplay> ladoIzquierdo;
    public List<PlatformValueDisplay> ladoDerecho;

    [Header("Física y Control de Alturas INDEPENDIENTE")]
    public List<PlataformaControlada> plataformasIzquierdas;
    public List<PlataformaControlada> plataformasDerechas;

    [Header("Ajustes Globales")]
    public float velocidadMovimiento = 3f;

    [Header("Estado del Nivel")]
    [Tooltip("Se marcará automáticamente cuando la balanza esté igualada")]
    public bool nivelResuelto = false;

    // --- NUEVO: EFECTOS DE SONIDO ---
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoAlineacion;
    [Range(0f, 1f)] public float volumenSonido = 1f;

    void Update()
    {
        int pesoIzquierdo = ObtenerPeso(ladoIzquierdo);
        int pesoDerecho = ObtenerPeso(ladoDerecho);

        int diferencia = pesoIzquierdo - pesoDerecho;

        MoverLado(plataformasIzquierdas, diferencia, true);
        MoverLado(plataformasDerechas, diferencia, false);

        // ¿Están equilibradas y tienen algo de peso encima?
        if (pesoIzquierdo == pesoDerecho && pesoIzquierdo > 0)
        {
            // Solo ejecutamos el sonido si acaba de resolverse en este exacto frame
            if (!nivelResuelto)
            {
                nivelResuelto = true;
                ReproducirSonidoExito();
            }
        }
        else
        {
            // Si el jugador quita un bloque y desbalancea la ecuación, 
            // reseteamos la variable para que el sonido pueda volver a sonar si lo vuelve a resolver.
            nivelResuelto = false;
        }
    }

    int ObtenerPeso(List<PlatformValueDisplay> lado)
    {
        int total = 0;
        foreach (var plat in lado)
        {
            if (plat != null) total += plat.GetValue();
        }
        return total;
    }

    void MoverLado(List<PlataformaControlada> plataformas, int diferencia, bool esIzquierda)
    {
        foreach (var plat in plataformas)
        {
            if (plat.modelo == null) continue;

            float targetY;
            if (esIzquierda)
            {
                targetY = plat.alturaEquilibrio - (diferencia * plat.sensibilidadPeso);
            }
            else
            {
                targetY = plat.alturaEquilibrio + (diferencia * plat.sensibilidadPeso);
            }

            Vector3 posicionActual = plat.modelo.position;
            Vector3 targetPos = new Vector3(posicionActual.x, targetY, posicionActual.z);

            plat.modelo.position = Vector3.MoveTowards(posicionActual, targetPos, velocidadMovimiento * Time.deltaTime);
        }
    }

    void ReproducirSonidoExito()
    {
        if (fuenteSonido != null && sonidoAlineacion != null)
        {
            fuenteSonido.PlayOneShot(sonidoAlineacion, volumenSonido);
        }
    }
}