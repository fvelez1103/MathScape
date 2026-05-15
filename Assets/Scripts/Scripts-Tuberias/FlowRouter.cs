using UnityEngine;

public class FlowRouter : MonoBehaviour
{
    public RadicalProcessor module;

    public GameObject realFloor;
    public GameObject fakeFloor;

    public GameObject waterCorrect;
    public GameObject waterWrong;

    private bool executed = false;

    public void Execute()
    {
        if (executed) return;
        executed = true;

        if (module.IsValid())
        {
            realFloor.SetActive(true);
            fakeFloor.SetActive(false);
            waterCorrect.SetActive(true);
        }
        else
        {
            realFloor.SetActive(false);
            fakeFloor.SetActive(true);
            waterWrong.SetActive(true);
        }
    }
}
