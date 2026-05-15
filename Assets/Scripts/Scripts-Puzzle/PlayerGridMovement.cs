using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerGridMovement : MonoBehaviour
{
    [SerializeField] TextMeshPro equationText;
    [SerializeField] TextMeshPro resultText;

    Vector2Int gridPos;

    List<MagneticCube> attachedCubes = new();
    Dictionary<MagneticCube, Vector2Int> cubeOffsets = new();

    Stack<IUndoAction> undoStack = new();

    bool isRewinding = false;
    bool blockAutoAttach = false;

    public int currentValue = 0;


    interface IUndoAction { void Undo(PlayerGridMovement p); }

    class MoveAction : IUndoAction
    {
        Vector2Int prev;
        public MoveAction(Vector2Int p) { prev = p; }
        public void Undo(PlayerGridMovement p) => p.SetGridPosition(prev);
    }

    class AttachAction : IUndoAction
    {
        MagneticCube cube;
        Vector2Int cell;

        public AttachAction(MagneticCube c, Vector2Int cell)
        {
            cube = c;
            this.cell = cell;
        }

        public void Undo(PlayerGridMovement p)
        {
            p.DetachCube(cube, cell);
        }
    }

    class RotateAction : IUndoAction
    {
        Dictionary<MagneticCube, Vector2Int> snapshot;
        public RotateAction(Dictionary<MagneticCube, Vector2Int> s)
        {
            snapshot = new Dictionary<MagneticCube, Vector2Int>(s);
        }

        public void Undo(PlayerGridMovement p)
        {
            p.RestoreOffsets(snapshot);
        }
    }


    void Start()
    {
        gridPos = WorldToGrid(transform.position);
        transform.position = GridToWorld(gridPos);
    }

    void Update()
    {
        if (isRewinding) return;

        if (!blockAutoAttach)
            TryAttachAdjacentCubes();

        if (Input.GetKeyDown(KeyCode.Z))
            Undo();

        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S)) dir = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A)) dir = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D)) dir = Vector2Int.right;

        if (attachedCubes.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.E)) Rotate(false);
            if (Input.GetKeyDown(KeyCode.Q)) Rotate(true);
        }

        if (dir != Vector2Int.zero)
            Move(dir);
    }


    void Move(Vector2Int dir)
    {
        Vector2Int next = gridPos + dir;
        if (!GridManager.Instance.IsValidCell(next)) return;

        foreach (var c in attachedCubes)
            if (!GridManager.Instance.IsValidCell(next + cubeOffsets[c]))
                return;

        undoStack.Push(new MoveAction(gridPos));
        blockAutoAttach = false;

        gridPos = next;
        transform.position = GridToWorld(gridPos);

        foreach (var c in attachedCubes)
            c.SetGridPosition(gridPos + cubeOffsets[c]);

        UpdateEquationFeedback();
    }


    void TryAttachAdjacentCubes()
    {
        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var dir in dirs)
        {
            Vector2Int cell = gridPos + dir;

            MagneticCube cube = GridManager.Instance.GetCubeAt(cell);
            if (cube == null || cube.isAttached) continue;

            cube.isAttached = true;
            undoStack.Push(new AttachAction(cube, cell));

            attachedCubes.Add(cube);
            cubeOffsets[cube] = dir;

            cube.transform.SetParent(transform);
            cube.transform.localPosition = new Vector3(dir.x, 0, dir.y);

            UpdateEquationFeedback();
            return;
        }
    }

    void DetachCube(MagneticCube cube, Vector2Int cell)
    {
        cube.isAttached = false;
        attachedCubes.Remove(cube);
        cubeOffsets.Remove(cube);

        cube.transform.SetParent(null);
        cube.transform.position = GridToWorld(cell);

        UpdateEquationFeedback();
    }


    void Rotate(bool clockwise)
    {
        var snapshot = new Dictionary<MagneticCube, Vector2Int>(cubeOffsets);
        Dictionary<MagneticCube, Vector2Int> rotated = new();

        foreach (var kv in cubeOffsets)
        {
            Vector2Int o = kv.Value;
            Vector2Int r = clockwise
                ? new Vector2Int(o.y, -o.x)
                : new Vector2Int(-o.y, o.x);

            if (!GridManager.Instance.IsValidCell(gridPos + r))
                return;

            rotated[kv.Key] = r;
        }

        undoStack.Push(new RotateAction(snapshot));

        cubeOffsets = rotated;
        foreach (var kv in cubeOffsets)
            kv.Key.transform.localPosition = new Vector3(kv.Value.x, 0, kv.Value.y);

        UpdateEquationFeedback();
    }

    void RestoreOffsets(Dictionary<MagneticCube, Vector2Int> snap)
    {
        cubeOffsets = new Dictionary<MagneticCube, Vector2Int>(snap);
        foreach (var kv in cubeOffsets)
            kv.Key.transform.localPosition = new Vector3(kv.Value.x, 0, kv.Value.y);
    }


    void UpdateEquationFeedback()
    {
        if (attachedCubes.Count == 0)
        {
            equationText.text = "";
            resultText.text = "";
            currentValue = 0;
            return;
        }

        List<MagneticCube> aligned = GetMainLine();

        string eq = "";
        int result = aligned[0].value;
        eq = aligned[0].value.ToString();

        for (int i = 1; i < aligned.Count; i++)
        {
            eq += aligned[i].value.ToString();
            result = int.Parse(eq); 
        }

        currentValue = result;
        equationText.text = eq;
        resultText.text = "= " + result;
    }

    List<MagneticCube> GetMainLine()
    {
        attachedCubes.Sort((a, b) =>
            cubeOffsets[a].sqrMagnitude.CompareTo(cubeOffsets[b].sqrMagnitude));



{
    attachedCubes.Sort((a, b) =>
        cubeOffsets[a].sqrMagnitude.CompareTo(cubeOffsets[b].sqrMagnitude));

    Vector2Int baseDir = GetDirection(cubeOffsets[attachedCubes[0]]);
    List<MagneticCube> line = new() { attachedCubes[0] };

    for (int i = 1; i < attachedCubes.Count; i++)
    {
        Vector2Int dir = GetDirection(cubeOffsets[attachedCubes[i]]);
        if (dir == baseDir || dir == -baseDir)
            line.Add(attachedCubes[i]);
    }

    return line;
}

    }


Vector2Int GetDirection(Vector2Int v)
{
    if (v.x != 0) return new Vector2Int(v.x > 0 ? 1 : -1, 0);
    if (v.y != 0) return new Vector2Int(0, v.y > 0 ? 1 : -1);
    return Vector2Int.zero;
}



    void Undo()
    {
        if (undoStack.Count == 0) return;
        isRewinding = true;
        undoStack.Pop().Undo(this);
        isRewinding = false;
        UpdateEquationFeedback();
    }

    void SetGridPosition(Vector2Int p)
    {
        gridPos = p;
        transform.position = GridToWorld(p);
        foreach (var c in attachedCubes)
            c.SetGridPosition(gridPos + cubeOffsets[c]);
    }

    Vector2Int WorldToGrid(Vector3 p) => new(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
    Vector3 GridToWorld(Vector2Int c) => new(c.x, transform.position.y, c.y);
}
