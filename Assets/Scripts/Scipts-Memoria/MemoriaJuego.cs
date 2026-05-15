using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DatosMemoria", menuName = "Sistema/Memoria Del Juego")]
public class MemoriaDelJuego : ScriptableObject
{
    [Header("Navegación Espacial")]
    public Vector3 ultimaPosicionJugador;
    public string nombreEscenaPrincipal;
    
    [Tooltip("El nombre exacto del GameObject de la cámara a reactivar")]
    public string nombreCamaraRetorno; 

    [Header("Estado de las Puertas")]
    public string idPuertaEnProgreso;
    public List<string> puertasResueltas = new List<string>();

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    public void ReiniciarMemoria()
    {
        ultimaPosicionJugador = Vector3.zero;
        idPuertaEnProgreso = "";
        nombreCamaraRetorno = ""; 
        nombreEscenaPrincipal = ""; 
        puertasResueltas.Clear();
    }
}