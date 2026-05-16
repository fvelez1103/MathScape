using UnityEngine;
using TMPro;
using System.Collections;

public class LockDoor : MonoBehaviour
{
    [SerializeField] private int targetValue = 10;
    [SerializeField] private TextMeshPro valueText;

    [Header("Efectos de Desbloqueo")]
    public AudioClip sonidoDesbloqueo;
    [Tooltip("Tiempo en segundos que tarda en desaparecer tras resolverse")]
    public float retrasoDesbloqueo = 1.0f; 
    public float volumenSonido = 1.0f;

    private bool isUnlocked = false;

    public bool IsUnlocked => isUnlocked; 

    private void Awake()
    {
        if (valueText != null)
            valueText.text = targetValue.ToString();
    }

    private void UpdateText()
    {
        if (valueText != null)
        {
            valueText.text = isUnlocked ? "" : "= " + targetValue.ToString();
        }
    }

    public void TryUnlock(int currentValue)
    {
        if (isUnlocked) return;

        if (currentValue == targetValue)
        {
            isUnlocked = true;
            
            StartCoroutine(ProcesoDeDesbloqueo());
        }
    }

    private IEnumerator ProcesoDeDesbloqueo()
    {
        UpdateText();

        if (sonidoDesbloqueo != null)
        {
            AudioSource.PlayClipAtPoint(sonidoDesbloqueo, transform.position, volumenSonido);
        }

        yield return new WaitForSeconds(retrasoDesbloqueo);

        gameObject.SetActive(false); 
        CheckAllLocksCleared();
    }

    private void CheckAllLocksCleared()
    {
        LockDoor[] allLocks = Object.FindObjectsByType<LockDoor>(FindObjectsSortMode.None);
        bool allClear = true;

        foreach (LockDoor lockItem in allLocks)
        {
            if (lockItem.gameObject.activeSelf && !lockItem.IsUnlocked)
            {
                allClear = false;
                break;
            }
        }

        if (allClear)
        {
            Debug.Log("<color=yellow>Todos los candados abiertos. El nivel sigue activo hasta que el jugador cruce la meta.</color>");
        }
    }

    public void ResetLock()
    {
        isUnlocked = false;

        if (valueText != null)
            valueText.text = targetValue.ToString();

        gameObject.SetActive(true);
    }

    public void ForceUnlock()
    {
        isUnlocked = true;

        if (valueText != null)
            valueText.text = "";

        gameObject.SetActive(false);
        CheckAllLocksCleared(); 
    }

    public void RestoreState(bool wasUnlocked)
    {
        isUnlocked = wasUnlocked;
        gameObject.SetActive(!isUnlocked);
        UpdateText();
    }
}