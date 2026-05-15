using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{
    public List<Camera> camaras;
    public int index;
    public Camera currentCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCamera = camaras[0];
        camaras[index].gameObject.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            index = index < camaras.Count -1 ? index +1 :0;

            for(int i = 0; i< camaras.Count; i++)
            {
                camaras[i].gameObject.SetActive(i==index);
            }

            currentCamera = camaras[index];
        }
    }
}
