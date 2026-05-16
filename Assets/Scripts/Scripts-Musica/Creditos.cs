using UnityEngine;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    void Start()
    {
        Invoke("WaitToEnd", 73f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        
    }

    public void WaitToEnd()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
