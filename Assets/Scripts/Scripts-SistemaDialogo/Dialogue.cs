using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.AI;

public class Dialogue : MonoBehaviour
{
    [Header("UI Dialogo")]
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] public TMP_Text dialogueText;
    [SerializeField, TextArea(4,6)] private string[] dialogueLines;
    [SerializeField] private float typingTime = 0.05f;

    [Header("Entidades a Congelar (Opcional)")]
    [SerializeField] private GameObject jugadorObjeto;
    [SerializeField] private GameObject villanoObjeto;

    [Header("Configuración Extra")]
    [Tooltip("Si es true, el diálogo no se podrá volver a activar después de terminar.")]
    public bool esUnSoloUso = false; 

    private bool isPlayerInRange;
    public bool didDialogueStart;
    private int lineIndex;
    
    private bool dialogoCompletado = false; 

    void Update()
    {
        // Si el diálogo es de un solo uso y ya terminó, bloqueamos la tecla E por completo
        if (dialogoCompletado) return; 

        if((isPlayerInRange || didDialogueStart) && Input.GetKeyDown(KeyCode.E))
        {
            if(!didDialogueStart)
            {
                StartDialogue();
            }
            else if(dialogueText.text == dialogueLines[lineIndex])
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
            }
        }
    }

    public void StartDialogue()
    {
        didDialogueStart = true;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueMark != null) dialogueMark.SetActive(false); 
        
        lineIndex = 0;
        Time.timeScale = 0f;

        CongelarEntidad(jugadorObjeto, true);
        CongelarEntidad(villanoObjeto, true);

        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if(lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            FinalizarDialogo();
        }
    }

    private void FinalizarDialogo()
    {
        didDialogueStart = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        
        Time.timeScale = 1f;

        CongelarEntidad(jugadorObjeto, false);
        CongelarEntidad(villanoObjeto, false);

        // --- LÓGICA DEL CANDADO ---
        if (esUnSoloUso)
        {
            dialogoCompletado = true; // Sellamos el diálogo para siempre
            if (dialogueMark != null) dialogueMark.SetActive(false); // Apagamos la marca de ayuda
        }
        else
        {
            // Si no es de un solo uso (ej. un NPC normal), volvemos a mostrar la E
            if (dialogueMark != null && isPlayerInRange) dialogueMark.SetActive(true); 
        }
    }

    private void CongelarEntidad(GameObject obj, bool congelar)
{
    if (obj == null) return;

    Animator anim = obj.GetComponent<Animator>();
    if (anim != null) anim.speed = congelar ? 0f : 1f;

    NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
    if (agent != null && agent.isActiveAndEnabled) agent.isStopped = congelar;

    MonoBehaviour villanoScript = obj.GetComponent("VillanoIA") as MonoBehaviour;
    if (villanoScript != null) villanoScript.enabled = !congelar;

    // --- EL ARREGLO PARA EL SONIDO ---
    MovimientoJugador mov = obj.GetComponent<MovimientoJugador>();
    if (mov != null)
    {
        mov.estaEnDialogo = congelar;
        if (congelar && mov.fuentePasos != null)
        {
            mov.fuentePasos.Stop(); // Cortamos el sonido de raíz aquí
        }
    }
}

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        foreach(char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Si ya se completó, no hacemos nada para no mostrar la "E" por error
        if (dialogoCompletado) return; 

        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (dialogueMark != null) dialogueMark.SetActive(true); 
        }
    }
 
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (dialogueMark != null) dialogueMark.SetActive(false);
        }
    }
}