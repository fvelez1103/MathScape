using UnityEngine;
using UnityEngine.EventSystems;

public class MenuFocusFix : MonoBehaviour
{
    public GameObject firstSelectedButton;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
}