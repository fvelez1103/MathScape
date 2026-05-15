using UnityEngine;
using System.Collections.Generic;

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance;

    [System.Serializable]
    public struct ObjectState
    {
        public Transform transform;
        public Vector3 position;
        public Quaternion rotation;
        public Transform parent;
        public bool isAttached;
    }

    public struct TurnState
    {
        public List<ObjectState> objectStates;
        public List<Transform> currentChain;
        public List<bool> lockStates; 
        public Vector3 playerPosAtThisState; 
    }

    private Stack<TurnState> history = new Stack<TurnState>();
    private List<MagneticEntity> allEntities = new List<MagneticEntity>();
    private List<LockDoor> allLocks = new List<LockDoor>(); 
    private Transform playerTransform;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [System.Obsolete]
    public void Initialize(Transform player)
    {
        playerTransform = player;
        history.Clear();
        allEntities.Clear();
        allEntities.AddRange(FindObjectsByType<MagneticEntity>(FindObjectsSortMode.None));
        allLocks.Clear();
        allLocks.AddRange(FindObjectsOfType<LockDoor>(true)); 
    }

    public void RecordState(List<Transform> currentChain)
    {
        TurnState state = new TurnState();
        state.objectStates = new List<ObjectState>();
        state.currentChain = new List<Transform>(currentChain);
        state.lockStates = new List<bool>();
        state.playerPosAtThisState = playerTransform.position;

        state.objectStates.Add(new ObjectState
        {
            transform = playerTransform,
            position = playerTransform.position,
            rotation = playerTransform.rotation,
            parent = null,
            isAttached = false
        });

        foreach (var entity in allEntities)
        {
            state.objectStates.Add(new ObjectState
            {
                transform = entity.transform,
                position = entity.transform.position,
                rotation = entity.transform.rotation,
                parent = entity.transform.parent,
                isAttached = entity.IsAttached
            });
        }

        foreach (var lockDoor in allLocks)
        {
            state.lockStates.Add(lockDoor.IsUnlocked);
        }

        history.Push(state);
    }

    public void Undo(PlayerController playerController)
    {
        if (history.Count == 0) return;

        TurnState targetState = history.Pop();

        if (Vector3.Distance(targetState.playerPosAtThisState, playerTransform.position) < 0.1f)
        {
            if (history.Count > 0)
            {
        
                targetState = history.Pop();
                Debug.Log("Undo Especial: Saltando dos pasos para despegar y retroceder casilla.");
            }
        }

        ApplyState(targetState, playerController);
    }

    private void ApplyState(TurnState state, PlayerController playerController)
    {
        foreach (var objState in state.objectStates)
        {
            if (objState.transform == null) continue;

            objState.transform.position = objState.position;
            objState.transform.rotation = objState.rotation;
            
            MagneticEntity entity = objState.transform.GetComponent<MagneticEntity>();
            if (entity != null)
            {
                if (!objState.isAttached) entity.Detach();
                else entity.transform.SetParent(objState.parent);
            }
        }

        playerController.RestoreChain(state.currentChain);

        if (state.lockStates != null && state.lockStates.Count == allLocks.Count)
        {
            for (int i = 0; i < allLocks.Count; i++)
            {
                allLocks[i].RestoreState(state.lockStates[i]);
            }
        }
    }
}