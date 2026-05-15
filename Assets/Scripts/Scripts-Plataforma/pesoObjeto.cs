using UnityEngine;
using TMPro; 

public class pesoObjeto : MonoBehaviour
{
    public float peso;
    
    public SnapPoint origenLogico;

    void Start()
    {
        TextMeshPro texto3D = GetComponentInChildren<TextMeshPro>();
        
        if (texto3D != null)
        {
            texto3D.text = peso.ToString();
        }
    }    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("objeto"))
        {
            this.transform.SetParent(collision.transform);
        }
    }
}