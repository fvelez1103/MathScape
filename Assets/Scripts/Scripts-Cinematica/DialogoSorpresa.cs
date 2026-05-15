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
        // 1. Congelar al jugador (Físicas y Animación) apenas entra al trigger
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

        // 2. Animar bordes
        yield return StartCoroutine(AnimarBordes(anchuraBorde));

        // 3. Mostrar el diálogo
        scriptDialogoFinal.StartDialogue();

        // 4. Esperar a que el diálogo termine
        yield return new WaitUntil(() => scriptDialogoFinal.didDialogueStart == false);

        // 5. Quitar los bordes
        yield return StartCoroutine(AnimarBordes(0f));

        // 6. Descongelar al jugador
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