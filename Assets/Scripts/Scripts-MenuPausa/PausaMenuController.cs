using UnityEngine;
using UnityEngine.SceneManagement; 

public class PauseMenuController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pauseCanvas; 
    public MonoBehaviour playerMovementScript; 
    [Header("Configuración")]
    [Tooltip("El nombre exacto o el índice de tu escena de Menú Principal en los Build Settings")]
    public string mainMenuSceneName = "MenuPrincipal"; 

    private bool isPaused = false;

    void Start()
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else if (playerMovementScript != null && playerMovementScript.enabled == true)
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseCanvas.SetActive(true);
        
        Time.timeScale = 0f; 

        playerMovementScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseCanvas.SetActive(false);
        
        Time.timeScale = 1f; 

        playerMovementScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
}