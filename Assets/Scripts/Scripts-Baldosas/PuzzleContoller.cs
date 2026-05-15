using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PuzzleController : MonoBehaviour
{
    [Header("Configuración")]
    public List<BaldosaData> puzzlePieces;
    public GameObject mainCamera;
    public GameObject puzzleCamera;
    public GameObject puzzleCanvas; 
    public MonoBehaviour playerScript;

    [Header("Límites del Nivel")]
    public float limiteMinX = -5f;
    public float limiteMaxX = 5f;
    public float limiteMinY = -5f;
    public float limiteMaxY = 5f;

    [Header("Eventos")]
    public UnityEvent OnPuzzleSolved;

    [Header("Métricas DDA")]
    public MetricasCuadrado scriptMetricas;

    [Header("Referencia a la Palanca")]
    public BaldosasTrigger scriptPalanca;

    [Header("Sistema de Ayuda Temporal")]
    [Tooltip("El objeto Text o Panel dentro del Canvas que contiene la pista.")]
    public GameObject textoAyuda;
    [Tooltip("Tiempo en segundos antes de mostrar la ayuda (15 para testing, 60 para final).")]
    public float tiempoParaAyuda = 45f;

    // --- NUEVO: EFECTOS DE SONIDO ---
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoPuertaAbriendose;
    [Range(0f, 1f)] public float volumenSonido = 1f;
    // --------------------------------

    private int currentIndex = 0;
    private bool isPuzzleActive = false;
    private bool yaGano = false; 
    private Dictionary<BaldosaData, Vector3> initialPositions = new Dictionary<BaldosaData, Vector3>();
    
    private float tiempoTranscurrido = 0f;
    private bool ayudaMostrada = false;

    private void Start()
    {
        foreach (var piece in puzzlePieces)
        {
            if (piece != null) initialPositions[piece] = piece.transform.position;
        }
        isPuzzleActive = false;
        
        if (puzzleCanvas != null) puzzleCanvas.SetActive(false);
        if (textoAyuda != null) textoAyuda.SetActive(false); 
    }

    public void StartPuzzle()
    {
        if (yaGano) return;

        isPuzzleActive = true;
        mainCamera.SetActive(false);
        puzzleCamera.SetActive(true);
        if(puzzleCanvas != null) puzzleCanvas.SetActive(true);

        tiempoTranscurrido = 0f;
        ayudaMostrada = false;
        if (textoAyuda != null) textoAyuda.SetActive(false);

        if (playerScript != null) playerScript.enabled = false;
        
        if (scriptMetricas != null) scriptMetricas.IniciarCronometro();
        
        if (puzzlePieces.Count > 0) puzzlePieces[currentIndex].Select();
        
        Debug.Log("<color=green>M4:</color> Puzzle iniciado.");
    }

    void Update()
    {
        if (!isPuzzleActive) return;

        if (!ayudaMostrada)
        {
            tiempoTranscurrido += Time.deltaTime;
            if (tiempoTranscurrido >= tiempoParaAyuda)
            {
                MostrarAyuda();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) { CancelPuzzle(); return; }
        if (Input.GetKeyDown(KeyCode.R)) { ResetPuzzle(); return; }
        if (Input.GetKeyDown(KeyCode.Q)) ChangeSelection();

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) TryMovePiece(Vector2.up);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) TryMovePiece(Vector2.down);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) TryMovePiece(Vector2.left);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) TryMovePiece(Vector2.right);

        if (Input.GetKeyDown(KeyCode.E)) CheckWin();
    }

    private void MostrarAyuda()
    {
        ayudaMostrada = true;
        if (textoAyuda != null)
        {
            textoAyuda.SetActive(true);
            Debug.Log("<color=yellow>M4 Ayuda:</color> Se excedió el tiempo límite. Mostrando pista visual.");
            if (FirebaseManager.Instancia != null) 
            {
                FirebaseManager.Instancia.M4_RegistrarAyuda();
            }
        }
    }

    void CheckWin()
    {
        if (isPuzzleActive && FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.M4_RegistrarComprobar();
        }

        if (GameManager.Instance.CheckSolution(puzzlePieces))
        {
            // --- REPRODUCIR SONIDO DE ÉXITO (PUERTA) ---
            if (fuenteSonido != null && sonidoPuertaAbriendose != null)
            {
                fuenteSonido.PlayOneShot(sonidoPuertaAbriendose, volumenSonido);
            }
            // -------------------------------------------

            EndPuzzle(true);
        }
    }

    void EndPuzzle(bool isWin)
    {
        isPuzzleActive = false;
        
        if (puzzleCamera != null) puzzleCamera.SetActive(false);
        if (puzzleCanvas != null) puzzleCanvas.SetActive(false);
        if (textoAyuda != null) textoAyuda.SetActive(false); 
        
        if (mainCamera != null) mainCamera.SetActive(true);
        if (playerScript != null) playerScript.enabled = true;

        if (isWin)
        {
            yaGano = true; 
            
            if (scriptPalanca != null)
            {
                scriptPalanca.enabled = false;
                Debug.Log("<color=red>Palanca desactivada para evitar bucle de cámara.</color>");
            }

            if (scriptMetricas != null) scriptMetricas.RegistrarVictoriaCuadrado();
            OnPuzzleSolved.Invoke();
        }
        else
        {
            if (scriptMetricas != null) scriptMetricas.PausarCronometro();
        }
    }

    #region Movimiento
    void ChangeSelection() { puzzlePieces[currentIndex].Deselect(); currentIndex = (currentIndex + 1) % puzzlePieces.Count; puzzlePieces[currentIndex].Select(); }
    
    void TryMovePiece(Vector2 dir) {
        BaldosaData piece = puzzlePieces[currentIndex];
        Vector3 targetPos = piece.transform.position + (Vector3)dir;
        float hW = piece.dimensions.x / 2f, hH = piece.dimensions.y / 2f;
        
        if (targetPos.x - hW < limiteMinX || targetPos.x + hW > limiteMaxX || targetPos.y - hH < limiteMinY || targetPos.y + hH > limiteMaxY) return;
        if (WouldOverlap(piece, targetPos.x - hW, targetPos.x + hW, targetPos.y - hH, targetPos.y + hH)) return;
        
        piece.Move(dir);

        if (FirebaseManager.Instancia != null) 
        {
            FirebaseManager.Instancia.M4_RegistrarMovimientoPieza();
        }
    }
    
    private bool WouldOverlap(BaldosaData movingPiece, float tL, float tR, float tB, float tT) {
        foreach (BaldosaData other in puzzlePieces) {
            if (other == movingPiece) continue; 
            float oW = other.dimensions.x / 2f, oH = other.dimensions.y / 2f;
            if (tL < other.transform.position.x + oW && tR > other.transform.position.x - oW && tB < other.transform.position.y + oH && tT > other.transform.position.y - oH) return true; 
        }
        return false;
    }
    
    void ResetPuzzle() { foreach (var p in puzzlePieces) p.transform.position = initialPositions[p]; }
    void CancelPuzzle() { EndPuzzle(false); }
    #endregion
}