using System.Collections;
using UnityEngine;

public class DialogoSorpresa : MonoBehaviour
{
    [Header("Configuración del Diálogo")]
    public Dialogue scriptDialogoFinal;    
    private bool yaHablo = false;

    [Header("Conexión con el Jugador")]
    public MonoBehaviour scriptMovimientoJugador; 
    public Animator animadorJugador; 

    [Header("Cinemática UI (Bordes Negros)")]
    [Tooltip("Arrastra las imágenes negras del Canvas aquí")]
    public RectTransform bordeIzquierdo; 
    public RectTransform bordeDerecho;   
    public float anchuraBorde = 150f; 
    public float tiempoAnimacionBordes = 0.5f;

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaHablo)
        {
            yaHablo = true;
            StartCoroutine(SecuenciaSorpresa());
        }
    }

    [System.Obsolete]
    IEnumerator SecuenciaSorpresa()
    {
        if (scriptMovimientoJugador != null) 
        {
            scriptMovimientoJugador.enabled = false;
            
            Rigidbody rbJugador = scriptMovimientoJugador.GetComponent<Rigidbody>();
            if (rbJugador != null) rbJugador.velocity = Vector3.zero;
        }

        if (animadorJugador != null) 
        {
            animadorJugador.Rebind();
            animadorJugador.speed = 0f;
        }

        yield return StartCoroutine(AnimarBordes(anchuraBorde));

        scriptDialogoFinal.StartDialogue();

        yield return new WaitUntil(() => scriptDialogoFinal.didDialogueStart == false);

        yield return StartCoroutine(AnimarBordes(0f));

        if (animadorJugador != null) animadorJugador.speed = 1f; 
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
    }

    IEnumerator AnimarBordes(float anchuraObjetivo)
    {
        if (bordeIzquierdo == null || bordeDerecho == null) yield break;

        float tiempo = 0;
        Vector2 sizeIzq = bordeIzquierdo.sizeDelta;
        Vector2 sizeDer = bordeDerecho.sizeDelta;

        while (tiempo < tiempoAnimacionBordes)
        {
            tiempo += Time.deltaTime;
            float interpolacion = tiempo / tiempoAnimacionBordes;

            bordeIzquierdo.sizeDelta = new Vector2(Mathf.Lerp(sizeIzq.x, anchuraObjetivo, interpolacion), sizeIzq.y);
            bordeDerecho.sizeDelta = new Vector2(Mathf.Lerp(sizeDer.x, anchuraObjetivo, interpolacion), sizeDer.y);

            yield return null;
        }

        bordeIzquierdo.sizeDelta = new Vector2(anchuraObjetivo, sizeIzq.y);
        bordeDerecho.sizeDelta = new Vector2(anchuraObjetivo, sizeDer.y);
    }
}