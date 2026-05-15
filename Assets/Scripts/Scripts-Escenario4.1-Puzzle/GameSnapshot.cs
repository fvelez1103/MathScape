using System.Collections.Generic;
using UnityEngine;

public class GameSnapshot
{
    private Vector3 playerPos;
    private List<Vector3> entityPositions = new();
    private List<Transform> chain = new();
    private List<bool> lockStates = new();

    private PlayerController player;
    private List<Transform> entities;
    private LockDoor[] locks;

    public GameSnapshot(PlayerController p, List<Transform> allEntities, LockDoor[] allLocks)
    {
        player = p;
        entities = allEntities;
        locks = allLocks;

        playerPos = player.transform.position;

        foreach (var t in entities)
            entityPositions.Add(t.position);

        chain.AddRange(player.GetChain());

        foreach (var l in locks)
            lockStates.Add(l.IsUnlocked);
    }

    public void Restore()
    {
        player.transform.position = playerPos;
        player.RestoreChain(chain);

        for (int i = 0; i < entities.Count; i++)
            entities[i].position = entityPositions[i];

        for (int i = 0; i < locks.Length; i++)
        {
            if (lockStates[i])
                locks[i].ForceUnlock();
            else
                locks[i].ResetLock();
        }
    }
}
