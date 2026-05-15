using System.Collections;
using UnityEngine;

public class DirectorCinematica : MonoBehaviour
{
    [Header("Conexión con tu Juego")]
    public Dialogue scriptDialogoNPC;     
    public MonoBehaviour scriptMovimientoJugador; 
    public Animator animadorJugador; 
    public Transform npc; 
    
    [Header("Villano (Opcional)")]
    public VillanoIA scriptVillano; 

    [Header("Destino del NPC")]
    public Transform puntoDestinoNPC; 
    public float velocidadNPC = 3f;

    [Header("Cinemática UI (Bordes Negros)")]
    public RectTransform bordeIzquierdo; 
    public RectTransform bordeDerecho;   
    public float anchuraBorde = 150f;
    public float tiempoAnimacionBordes = 0.5f; 

    private bool cinematicaYaSeJugo = false;

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cinematicaYaSeJugo)
        {
            StartCoroutine(SecuenciaCinematica());
        }
    }

    [System.Obsolete]
    IEnumerator SecuenciaCinematica()
    {
        cinematicaYaSeJugo = true;

        if (scriptVillano != null) 
        {
            scriptVillano.enabled = false;

            Rigidbody rbVillano = scriptVillano.GetComponent<Rigidbody>();
            if (rbVillano != null) rbVillano.velocity = Vector3.zero;

            Animator animVillano = scriptVillano.GetComponent<Animator>();
            if(animVillano != null) animVillano.SetBool("estaCorriendo", false);
        }

        Dialogue[] todosLosDialogos = GetComponents<Dialogue>();
        foreach (Dialogue d in todosLosDialogos)
        {
            if (d != scriptDialogoNPC) d.enabled = false;
        }

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

        scriptDialogoNPC.StartDialogue();
        yield return new WaitUntil(() => scriptDialogoNPC.didDialogueStart == false);

        scriptDialogoNPC.enabled = false;

        if (npc != null && puntoDestinoNPC != null)
        {
            while (Vector3.Distance(npc.position, puntoDestinoNPC.position) > 0.1f)
            {
                if (scriptVillano != null) scriptVillano.enabled = false;
                
                if (animadorJugador != null) animadorJugador.speed = 0f;

                npc.position = Vector3.MoveTowards(npc.position, puntoDestinoNPC.position, velocidadNPC * Time.deltaTime);
                Vector3 lookPos = new Vector3(puntoDestinoNPC.position.x, npc.position.y, puntoDestinoNPC.position.z);
                npc.LookAt(lookPos);
                yield return null; 
            }
        }

        yield return StartCoroutine(AnimarBordes(0f));

        if (animadorJugador != null) animadorJugador.speed = 1f; 
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;

        if (scriptVillano != null) 
        {
            scriptVillano.enabled = true;
        }
        
        GetComponent<Collider>().enabled = false;
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
    }
}