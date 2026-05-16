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
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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

        float centroViewportY = viewport.TransformPoint(viewport.rect.center).y;

        foreach (Transform hijo in content)
        {
            RectTransform rectHijo = hijo as RectTransform;
            if (!hijo.gameObject.activeSelf) continue;

            float centroHijoY = rectHijo.TransformPoint(rectHijo.rect.center).y;
            float distancia = Mathf.Abs(centroViewportY - centroHijoY);

            if (distancia < distanciaMasCorta)
            {
                distanciaMasCorta = distancia;
                float offset = centroViewportY - centroHijoY;
                Vector3 objetivoMundial = content.position + new Vector3(0, offset, 0);
                objetivoLocal = content.parent.InverseTransformPoint(objetivoMundial);
            }
        }

        while (Vector3.Distance(content.localPosition, objetivoLocal) > 0.1f)
        {
            if (Input.GetMouseButtonDown(0)) { estaSnapeando = false; yield break; }

            content.localPosition = Vector3.Lerp(content.localPosition, objetivoLocal, Time.deltaTime * velocidadSnap);
            yield return null;
        }

        content.localPosition = objetivoLocal;
        estaSnapeando = false;
    }
public void EnfocarPaso(RectTransform pasoObjetivo)
{
    StopAllCoroutines();
    StartCoroutine(SnapAObjetivo(pasoObjetivo));
}

IEnumerator SnapAObjetivo(RectTransform objetivo)
{
    yield return new WaitForEndOfFrame();
    
    Canvas.ForceUpdateCanvases();

    float tiempo = 0;
    Vector3 posInicial = scrollRect.content.localPosition;
    
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