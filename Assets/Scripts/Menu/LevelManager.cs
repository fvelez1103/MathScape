using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Transform spawnPoint;

    void Awake()
    {
        Instance = this;
    }

    public void LevelCompleted()
    {
        Debug.Log("Nivel completado");
        Time.timeScale = 0f;
    }
}