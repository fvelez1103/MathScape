using UnityEngine;
using System.IO;

public class DDA_Logger : MonoBehaviour
{
    private string rutaArchivo;

    void Start()
    {
        // Define la ruta
        rutaArchivo = Application.dataPath + "/Log_DDA_Fuzzy.csv";
        
        // Escribe el encabezado si el archivo no existe
        if (!File.Exists(rutaArchivo))
        {
            File.WriteAllText(rutaArchivo, "Tiempo;ErrorMat;Caidas;NivelFrustracion;Estado\n");
        }

        // --- EL RASTREADOR ---
        Debug.Log($"<color=cyan>DDA Logger Activo:</color> Guardando datos en: {rutaArchivo}");

        DDA_DifusoPuro.OnCambioEstado += RegistrarCambio;
    }

    private void RegistrarCambio(EstadoJugador nuevoEstado)
    {
        if (DDA_DifusoPuro.Instancia == null) return;

        var dda = DDA_DifusoPuro.Instancia;
        string nuevaLinea = $"{Time.time};{dda.erroresMatematicos};{dda.caidasAlVacio};{dda.nivelFrustracionCrisp};{nuevoEstado}\n";
        
        File.AppendAllText(rutaArchivo, nuevaLinea);
        Debug.Log($"<color=white>DDA Logger:</color> Nuevo estado ({nuevoEstado}) registrado en CSV.");
    }

    private void OnDestroy()
    {
        DDA_DifusoPuro.OnCambioEstado -= RegistrarCambio;
    }
}