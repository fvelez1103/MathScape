using UnityEngine;
using TMPro;
using System.Collections;

public class DialogosPuzzle : MonoBehaviour
{
    [Header("Configuración UI")]
    public GameObject canvasDialogo;
    public TextMeshProUGUI textoDialogo;
    public GameObject promptE;

    [Header("Ajustes de Escritura")]
    [Range(0.01f, 0.1f)] public float velocidadEscritura = 0.05f;

    [Header("Tutorial: Diálogo de Inicio")]
    [TextArea(4, 6)]
    public string[] dialogueLines;
    
    [Header("Mecánica: Al Adherirse (Secuenciales)")]
    public bool requiereAdherirse = false;
    
    [TextArea(4, 6)]
    public string[] listaDialogosAdhesion;

    private int indiceFraseActual = 0;
    private int indiceBloqueAdhesion = 0; 
    private bool estaHablando = false;
    private bool esperandoAdhesion = false;
    private bool escribiendo = false;
    private string[] frasesActuales;
    private Coroutine corrutinaEscritura;

    void Start()
    {
        if (dialogueLines != null && dialogueLines.Length > 0)
        {
            IniciarSecuencia(dialogueLines);
        }
    }

    void Update()
    {
        if (estaHablando && Input.GetKeyDown(KeyCode.E))
        {
            if (escribiendo)
            {
                CompletarTextoInmediato();
            }
            else
            {
                ContinuarDialogo();
            }
        }
    }

    public void IniciarSecuencia(string[] frases)
    {
        if (frases == null || frases.Length == 0) return;

        frasesActuales = frases;
        indiceFraseActual = 0;
        estaHablando = true;
        
        if (canvasDialogo != null) canvasDialogo.SetActive(true);
        
        EmpezarEscritura();
        BloquearJugador(true); 
    }

    void EmpezarEscritura()
    {
        if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
        corrutinaEscritura = StartCoroutine(EscribirTexto());
    }

    IEnumerator EscribirTexto()
    {
        escribiendo = true;
        textoDialogo.text = "";
        if (promptE != null) promptE.SetActive(false);

        foreach (char letra in frasesActuales[indiceFraseActual].ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
        if (promptE != null) promptE.SetActive(true);
    }

    void CompletarTextoInmediato()
    {
        if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
        textoDialogo.text = frasesActuales[indiceFraseActual];
        escribiendo = false;
        if (promptE != null) promptE.SetActive(true);
    }

    void ContinuarDialogo()
    {
        indiceFraseActual++;

        if (indiceFraseActual < frasesActuales.Length)
        {
            EmpezarEscritura();
        }
        else
        {
            FinalizarDialogo();
        }
    }

    void FinalizarDialogo()
    {
        estaHablando = false;
        if (canvasDialogo != null) canvasDialogo.SetActive(false);
        if (promptE != null) promptE.SetActive(false);

        if (requiereAdherirse && indiceBloqueAdhesion < listaDialogosAdhesion.Length)
        {
            esperandoAdhesion = true;
        }
        
        BloquearJugador(false); 
    }

    public void NotificarAdhesion()
    {
        if (requiereAdherirse && esperandoAdhesion && !estaHablando)
        {
            if (indiceBloqueAdhesion < listaDialogosAdhesion.Length)
            {
                esperandoAdhesion = false;
                
                string[] siguienteMensaje = { listaDialogosAdhesion[indiceBloqueAdhesion] };
                IniciarSecuencia(siguienteMensaje);
                
                indiceBloqueAdhesion++; 
            }
        }
    }

    private void BloquearJugador(bool bloquear)
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.SetMovementEnabled(!bloquear);
        }
    }
}