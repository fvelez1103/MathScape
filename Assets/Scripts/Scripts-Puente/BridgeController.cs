using System;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    private bool bridgeAlreadyValidated = false;

    public Transform[] segments;
    public Transform leftTarget;
    public Transform rightTarget;
    public float tolerance = 0.2f;


    [Header("Quadratic values")]
    public float a = 0.05f;
    public float b = 0f;
    public float c = 0f;

    public float spacing = 1f;

    void Start()
    {
        UpdateBridge();
        Debug.Log("Bienvenido");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
            a -= Time.deltaTime * 0.05f;

        if (Input.GetKey(KeyCode.E))
            a += Time.deltaTime * 0.05f;

        if (Input.GetKey(KeyCode.Z))
            c -= Time.deltaTime;

        if (Input.GetKey(KeyCode.X))
            c += Time.deltaTime;

        UpdateBridge();

        if (BridgeIsValid() && !bridgeAlreadyValidated)
        {
            bridgeAlreadyValidated = true;
            Debug.Log("Puente correcto");
        }

    }

    void UpdateBridge()
    {
        int count = segments.Length;

        for (int i = 0; i < count; i++)
        {
            // x lógico centrado
            float x = (i - count / 2f) * spacing;

            float y = a * x * x + b * x + c;

            Vector3 pos = segments[i].position;
            pos.y = y;
            segments[i].position = pos;
        }
    }

    bool BridgeIsValid()
{
    int count = segments.Length;

    // X lógico del primer y último segmento
    float xLeft = (0 - count / 2f) * spacing;
    float xRight = ((count - 1) - count / 2f) * spacing;

    // Y calculado por la ecuación
    float yLeft = a * xLeft * xLeft + b * xLeft + c;
    float yRight = a * xRight * xRight + b * xRight + c;

    // Y real de los targets
    float leftTargetY = leftTarget.position.y;
    float rightTargetY = rightTarget.position.y;

    bool leftOK = Mathf.Abs(yLeft - leftTargetY) < tolerance;
    bool rightOK = Mathf.Abs(yRight - rightTargetY) < tolerance;

    return leftOK && rightOK;
}


    void SetBridgeColliders(bool enabled)
    {
        foreach (Transform seg in segments)
        {
            seg.GetComponent<Collider>().enabled = enabled;
        }
    }

}
