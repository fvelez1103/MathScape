using UnityEngine;

public class LectorMemoriaJugador : MonoBehaviour
{
    [Header("Conexión con la Memoria")]
    public MemoriaDelJuego memoria;

    void Awake()
    {
        if (memoria != null && memoria.ultimaPosicionJugador != Vector3.zero)
        {
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            transform.position = memoria.ultimaPosicionJugador;
            
            if (cc != null) cc.enabled = true;

            if (!string.IsNullOrEmpty(memoria.nombreCamaraRetorno))
            {
                Transform[] todosLosObjetos = Resources.FindObjectsOfTypeAll<Transform>();
                foreach (Transform t in todosLosObjetos)
                {
                    if (t.gameObject.scene.IsValid() && t.gameObject.name == memoria.nombreCamaraRetorno)
                    {
                        if (Camera.main != null && Camera.main.gameObject != t.gameObject)
                        {
                            Camera.main.gameObject.SetActive(false);
                        }
                        
                        t.gameObject.SetActive(true);
                        break;
                    }
                }
            }

            memoria.ultimaPosicionJugador = Vector3.zero; 
            memoria.nombreCamaraRetorno = "";
        }
    }
}