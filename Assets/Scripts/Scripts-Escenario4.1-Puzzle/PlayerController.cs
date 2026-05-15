using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Data; 
using System;

[RequireComponent(typeof(GridMover))]
[RequireComponent(typeof(MagneticChain))]
[RequireComponent(typeof(GridDetector))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput input;
    private GridMover mover;
    private MagneticChain magneticChain;
    private GridDetector gridDetector;

    [Header("Métricas (M2)")]
    [HideInInspector] public int contadorZ = 0;
    [HideInInspector] public int contadorR = 0;

    [Header("Configuración")]
    public int nivelID; 

    [Header("Undo System")]
    [SerializeField] private UndoManager undoManager; 

    [Header("Layer Masks")]
    [SerializeField] private LayerMask boxLayer;
    [SerializeField] private LayerMask operatorLayer;
    [SerializeField] private LayerMask wallLayer; 

    [Header("Candados")]
    [SerializeField] private LockDoor[] allLocks;

    [Header("Operador del Player (PRINCIPAL)")]
    [SerializeField] private PlayerOperator playerOperator = PlayerOperator.Add;

    [Header("Visualización")]
    [SerializeField] private TextMeshPro chainText;
    [SerializeField] private TextMeshPro resultText;

    // --- NUEVO: SONIDO DE MOVIMIENTO ---
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoPasoGrid;
    [Range(0f, 1f)] public float volumenPaso = 1f;

    private bool canMoveByTutorial = true;
    private bool wasMoving = false;
    private DataTable mathParser = new DataTable(); 

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        mover = GetComponent<GridMover>();
        magneticChain = GetComponent<MagneticChain>();
        gridDetector = GetComponent<GridDetector>();
        
        if (undoManager == null) 
            undoManager = FindAnyObjectByType<UndoManager>();
    }

    [Obsolete]
    private void Start()
    {
        if (undoManager != null) 
            undoManager.Initialize(transform);
            
        AttachAdjacentEntities();
    }

    private void Update()
    {
        HandleRestart();
        HandleUndo(); 

        if (!canMoveByTutorial) return; 

        if (wasMoving && !mover.IsMoving)
        {
            wasMoving = false;
            AttachAdjacentEntities();
        }

        if (mover.IsMoving)
        {
            wasMoving = true;
            return;
        }

        HandleMovement();
        HandleRotation();
        
        UpdateChainText();
        UpdateResultText();
    }

    #region Inputs Globales (Restart / Undo)
   private void HandleRestart() 
    { 
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            if (DDA_cuadriculaPiedra.Instancia != null) 
            {
                DDA_cuadriculaPiedra.Instancia.NotificarReinicioLocal();
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
    }

    private void HandleUndo() 
    { 
        if (mover.IsMoving) return; 
        if (Input.GetKeyDown(KeyCode.Z)) 
        { 
            if (undoManager != null) 
            { 
                undoManager.Undo(this); 
                CheckLocks(); 
                contadorZ++;
                if(DDA_cuadriculaPiedra.Instancia != null) DDA_cuadriculaPiedra.Instancia.totalUndos++;
            } 
        }
    }
    #endregion

    #region Movimiento y Auto-Adherencia Omnidireccional
   private void HandleMovement() 
    { 
        if (mover.IsMoving) return; 
        Vector3 dir = input.ReadMovement(); 
        if (dir == Vector3.zero) return; 
        
        // AQUÍ ES DONDE SE VALIDA QUE NO HAYA PARED
        if (IsValidMove(dir)) 
        { 
            // --- REPRODUCCIÓN DE SONIDO ---
            if (fuenteSonido != null && sonidoPasoGrid != null)
            {
                fuenteSonido.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                fuenteSonido.PlayOneShot(sonidoPasoGrid, volumenPaso);
            }
            // ------------------------------

            RecordUndoState(); 
            mover.TryMove(dir, new List<Transform>(magneticChain.Chain)); 
            
            if (FirebaseManager.Instancia != null && nivelID != 0) 
            {
                FirebaseManager.Instancia.M2_RegistrarMovimiento(nivelID);
            }

            CheckLocks(); 
        } 
    }
    
    private bool AttachAdjacentEntities()
    {
        bool attached = false;
        int conteoAntes = magneticChain.Count;
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        List<Transform> currentChain = new List<Transform> { transform };
        currentChain.AddRange(magneticChain.Chain);

        for (int i = 0; i < currentChain.Count; i++)
        {
            Transform part = currentChain[i];
            foreach (Vector3 dir in directions)
            {
                Vector3 checkPos = part.position + dir;
                Collider2D[] boxHits = Physics2D.OverlapBoxAll(checkPos, Vector2.one * 0.4f, 0f, boxLayer);
                foreach (Collider2D c in boxHits) { 
                    NumberBox nb = c.GetComponent<NumberBox>(); 
                    if (nb != null && !nb.IsAttached) { 
                        if (!attached) RecordUndoState(); 
                        nb.AttachTo(part, dir); 
                        magneticChain.AddToChain(nb.transform); 
                        currentChain.Add(nb.transform); 
                        attached = true; 
                    } 
                }
                
                Collider2D[] opHits = Physics2D.OverlapBoxAll(checkPos, Vector2.one * 0.4f, 0f, operatorLayer);
                foreach (Collider2D c in opHits) { 
                    OperatorBox op = c.GetComponent<OperatorBox>(); 
                    if (op != null && !op.IsAttached) { 
                        if (!attached) RecordUndoState(); 
                        op.AttachTo(part, dir); 
                        magneticChain.AddToChain(op.transform); 
                        currentChain.Add(op.transform); 
                        attached = true; 
                    } 
                }
            }
        }

        if (attached) 
        {
            CheckLocks();
            if (magneticChain.Count > conteoAntes) 
            {
                DialogosPuzzle tutorial = FindFirstObjectByType<DialogosPuzzle>();
                if (tutorial != null) tutorial.NotificarAdhesion();
            }
        }
        return attached;
    }

    private bool IsValidMove(Vector3 dir)
    {
        List<Transform> allParts = new List<Transform> { transform }; allParts.AddRange(magneticChain.Chain);
        foreach (Transform part in allParts) 
        { 
            Vector3 target = part.position + dir; 
            if (!gridDetector.HasGridAt(target)) return false; 
            
            if (Physics2D.OverlapBox(target, Vector2.one * 0.4f, 0f, wallLayer) != null) return false; 
            Collider2D[] hits = Physics2D.OverlapBoxAll(target, Vector2.one * 0.4f, 0f, boxLayer | operatorLayer); 
            foreach (var hit in hits) { if (!allParts.Contains(hit.transform)) return false; } 
        }
        return true;
    }

    private void RecordUndoState() { if (undoManager != null) undoManager.RecordState(new List<Transform>(magneticChain.Chain)); }
    #endregion

    #region Rotación
    private void HandleRotation() 
    { 
        if (mover.IsMoving) return; 
        int rot = input.ReadRotation(); 
        if (rot == 0 || magneticChain.Count == 0) return; 
        
        if (CanRotate(rot)) 
        { 
            RecordUndoState(); 
            ApplyRotation(rot); 
            CheckLocks(); 
        } 
    }
    
    private bool CanRotate(int rotDir) 
    { 
        List<Transform> allParts = new List<Transform> { transform }; allParts.AddRange(magneticChain.Chain); 
        foreach (Transform t in magneticChain.Chain) 
        { 
            Vector3 targetPos = CalculateRotatedPosition(t.position, rotDir); 
            if (!gridDetector.HasGridAt(targetPos)) return false; 
            
            if (Physics2D.OverlapBox(targetPos, Vector2.one * 0.4f, 0f, wallLayer) != null) return false; 
            Collider2D[] hits = Physics2D.OverlapBoxAll(targetPos, Vector2.one * 0.4f, 0f, boxLayer | operatorLayer); 
            foreach(var hit in hits) { if (!allParts.Contains(hit.transform)) return false; } 
        } 
        return true; 
    }

    private void ApplyRotation(int rotDir) 
    { 
        foreach (Transform t in magneticChain.Chain) 
        {
            t.position = CalculateRotatedPosition(t.position, rotDir); 
        }
        RefreshParenthesisSprites();
    }  

    private void RefreshParenthesisSprites()
    {
        foreach (Transform t in magneticChain.Chain)
        {
            if (t.TryGetComponent(out OperatorBox ob))
            {
                if (ob.Operator == MathOperator.OpenParenthesis || ob.Operator == MathOperator.CloseParenthesis)
                {
                    Vector3 relativa = t.position - transform.position;
                    bool esVertical = Mathf.Abs(relativa.y) > Mathf.Abs(relativa.x);
                    bool debeAbrir = esVertical ? relativa.y > 0 : relativa.x < 0;
                    ob.ActualizarApariencia(debeAbrir);
                }
            }
        }
    }

    private Vector3 CalculateRotatedPosition(Vector3 currentPos, int rotDir)
    {
        Vector3 relative = currentPos - transform.position;
        Vector3 newRelative = rotDir > 0
            ? new Vector3(-relative.y, relative.x, 0)
            : new Vector3(relative.y, -relative.x, 0);

        return transform.position + newRelative;
    }
    #endregion

    #region Evaluación Matemática
    private List<Transform> GetSortedMainChain()
    {
        List<Transform> allParts = new List<Transform> { transform };
        allParts.AddRange(magneticChain.Chain);

        List<Transform> hChain = GetContiguousLine(allParts, Vector3.right, Vector3.left);
        List<Transform> vChain = GetContiguousLine(allParts, Vector3.up, Vector3.down);

        bool isHorizontal = hChain.Count >= vChain.Count;
        List<Transform> mainChain = isHorizontal ? hChain : vChain;

        mainChain.Sort((a, b) => isHorizontal ? a.position.x.CompareTo(b.position.x) : b.position.y.CompareTo(a.position.y));
        return mainChain;
    }

    private List<Transform> GetContiguousLine(List<Transform> pool, Vector3 dir1, Vector3 dir2)
    {
        List<Transform> line = new List<Transform> { transform };
        Vector3 currentPos = transform.position + dir1;
        while (true) { Transform found = FindAtPosition(pool, currentPos); if (found != null) { line.Add(found); currentPos += dir1; } else break; }
        currentPos = transform.position + dir2;
        while (true) { Transform found = FindAtPosition(pool, currentPos); if (found != null) { line.Add(found); currentPos += dir2; } else break; }
        return line;
    }

    private Transform FindAtPosition(List<Transform> pool, Vector3 pos)
    {
        foreach (var t in pool) if (Mathf.Round(t.position.x) == Mathf.Round(pos.x) && Mathf.Round(t.position.y) == Mathf.Round(pos.y)) return t;
        return null;
    }

    private void CheckLocks()
    {
        int value = EvaluateChain();
        if (value == int.MinValue) return; 
        foreach (var l in allLocks) l.TryUnlock(value);
    }

    public int EvaluateChain()
    {
        List<Transform> visual = GetSortedMainChain();
        if (visual.Count <= 1) return int.MinValue; 

        string computeExpression = "";
        foreach (Transform t in visual)
        {
            if (t == transform) computeExpression += GetComputeSymbol(playerOperator);
            else if (t.TryGetComponent(out NumberBox nb)) computeExpression += nb.Value.ToString();
            else if (t.TryGetComponent(out OperatorBox ob)) computeExpression += GetComputeSymbol(ob.Operator); 
        }

        string finalExpr = computeExpression.Trim();
        if (finalExpr.EndsWith("+") || finalExpr.EndsWith("-") || finalExpr.EndsWith("*") || finalExpr.EndsWith("/"))
            finalExpr = finalExpr.Substring(0, finalExpr.Length - 1);
        
        if (finalExpr.StartsWith("*") || finalExpr.StartsWith("/")) return int.MinValue;

        try {
            var result = mathParser.Compute(finalExpr, "");
            return Convert.ToInt32(result);
        } catch { return int.MinValue; }
    }

    private string GetComputeSymbol(MathOperator op) => op switch { MathOperator.Add => "+", MathOperator.Subtract => "-", MathOperator.Multiply => "*", MathOperator.Divide => "/", MathOperator.OpenParenthesis => "(", MathOperator.CloseParenthesis => ")", _ => "" };
    private string GetComputeSymbol(PlayerOperator op) => op switch { PlayerOperator.Add => "+", PlayerOperator.Subtract => "-", PlayerOperator.Multiply => "*", PlayerOperator.Divide => "/", _ => "" };
    #endregion

    #region UI & Helpers
    private void UpdateChainText()
    {
        if (chainText == null) return;
        List<Transform> visual = GetSortedMainChain();
        
        if (visual.Count <= 2) 
        { 
            chainText.text = ""; 
            return; 
        }

        string display = "";
        foreach (Transform t in visual)
        {
            if (t == transform) display += " " + GetUISymbol(playerOperator) + " ";
            else if (t.TryGetComponent(out NumberBox nb)) display += nb.Value;
            else if (t.TryGetComponent(out OperatorBox ob)) display += " " + ob.GetSymbol() + " "; 
        }
        chainText.text = display.Trim();
    }
    
    private void UpdateResultText()
    {
        if (resultText == null) return;

        List<Transform> visual = GetSortedMainChain();
        
        if (visual.Count <= 2) 
        { 
            resultText.text = ""; 
            return; 
        }

        int resultado = EvaluateChain();
        if (resultado == int.MinValue) 
        { 
            resultText.text = "= ?"; 
            resultText.color = Color.red; 
        }
        else 
        { 
            resultText.text = "= " + resultado.ToString(); 
            resultText.color = Color.white; 
        }
    }

    private string GetUISymbol(PlayerOperator op) => op switch { PlayerOperator.Add => "+", PlayerOperator.Subtract => "-", PlayerOperator.Multiply => "×", PlayerOperator.Divide => "÷", _ => "+" };
    
    public void RestoreChain(List<Transform> newChain) { magneticChain.Clear(); foreach (var t in newChain) magneticChain.AddToChain(t); UpdateChainText(); UpdateResultText(); }
    public void ClearChain() { magneticChain.Clear(); }
    public List<Transform> GetChain() => new List<Transform>(magneticChain.Chain);
    public void SetMovementEnabled(bool enabled) => canMoveByTutorial = enabled;
    #endregion
}

public enum PlayerOperator { Add, Subtract, Multiply, Divide }