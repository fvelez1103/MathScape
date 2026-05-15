using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SnapVertical : MonoBehaviour, IEndDragHandler
{
    public ScrollRect scrollRect;
    public float velocidadSnap = 10f;

    private bool estaSnapeando = false;

    void Start()
    {
        // Si no asignaste el ScrollRect manualmente, lo busca en este objeto
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
    }

    // Se ejecuta cuando el usuario suelta el click/dedo
    public void OnEndDrag(PointerEventData eventData)
    {
        // Si no se está moviendo muy rápido, iniciamos el snap
        if (Mathf.Abs(scrollRect.velocity.y) < 500f && !estaSnapeando)
        {
            scrollRect.StopMovement(); // Frenamos el scroll
            StartCoroutine(SnapAlMasCercano());
        }
    }

    IEnumerator SnapAlMasCercano()
    {
        estaSnapeando = true;
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        float distanciaMasCorta = float.MaxValue;
        Vector3 objetivoLocal = content.localPosition;

        // Calculamos el centro del viewport en el mundo
        float centroViewportY = viewport.TransformPoint(viewport.rect.center).y;

        // Buscamos cuál hijo (paso) tiene su centro más cerca del centro del viewport
        foreach (Transform hijo in content)
        {
            RectTransform rectHijo = hijo as RectTransform;
            // Ignoramos objetos desactivados
            if (!hijo.gameObject.activeSelf) continue;

            // Calculamos el centro del hijo en el mundo
            float centroHijoY = rectHijo.TransformPoint(rectHijo.rect.center).y;
            float distancia = Mathf.Abs(centroViewportY - centroHijoY);

            if (distancia < distanciaMasCorta)
            {
                distanciaMasCorta = distancia;
                // Calculamos cuánto hay que mover el contenido para alinear los centros
                float offset = centroViewportY - centroHijoY;
                // Convertimos ese movimiento a coordenadas locales del content
                Vector3 objetivoMundial = content.position + new Vector3(0, offset, 0);
                objetivoLocal = content.parent.InverseTransformPoint(objetivoMundial);
            }
        }

        // Animamos suavemente hacia la posición objetivo
        while (Vector3.Distance(content.localPosition, objetivoLocal) > 0.1f)
        {
            // Si el usuario vuelve a tocar, cancelamos el snap
            if (Input.GetMouseButtonDown(0)) { estaSnapeando = false; yield break; }

            content.localPosition = Vector3.Lerp(content.localPosition, objetivoLocal, Time.deltaTime * velocidadSnap);
            yield return null;
        }

        content.localPosition = objetivoLocal;
        estaSnapeando = false;
    }
    // Agrega esto dentro de tu clase SnapVertical
public void EnfocarPaso(RectTransform pasoObjetivo)
{
    StopAllCoroutines();
    StartCoroutine(SnapAObjetivo(pasoObjetivo));
}

IEnumerator SnapAObjetivo(RectTransform objetivo)
{
    // Esperamos un frame para que los Layout Groups terminen de acomodarse
    yield return new WaitForEndOfFrame();
    
    // Volvemos a forzar actualización por seguridad
    Canvas.ForceUpdateCanvases();

    float tiempo = 0;
    Vector3 posInicial = scrollRect.content.localPosition;
    
    // Calculamos el destino final basándonos en la posición real del objeto
    // (Esto es mucho más preciso que el NormalizedPosition)
    float objetivoY = -objetivo.localPosition.y - (objetivo.rect.height / 2f);
    Vector3 posFinal = new Vector3(posInicial.x, objetivoY, posInicial.z);

    while (tiempo < 1f)
    {
        tiempo += Time.deltaTime * velocidadSnap;
        scrollRect.content.localPosition = Vector3.Lerp(posInicial, posFinal, tiempo);
        yield return null;
    }
    scrollRect.content.localPosition = posFinal;
}
}