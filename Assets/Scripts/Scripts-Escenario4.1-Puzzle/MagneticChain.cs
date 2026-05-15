using System.Collections.Generic;
using UnityEngine;

public class MagneticChain : MonoBehaviour
{
    private readonly List<Transform> chain = new();

    public IReadOnlyList<Transform> Chain => chain;

    public void AddToChain(Transform t)
    {
        if (!chain.Contains(t))
            chain.Add(t);
    }

    public int Count => chain.Count;

    public void Clear()
{
    chain.Clear();
}

}

