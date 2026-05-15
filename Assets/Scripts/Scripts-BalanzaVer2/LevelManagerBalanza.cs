using UnityEngine;
using System.Collections.Generic;

public class LevelManagerBalanza : MonoBehaviour
{
    [System.Serializable]
    public struct ObjetoPesoCSV
    {
        public string idCSV; 
        public GameObject prefab;
        public float alturaFijaOffset; 
    }

    [Header("Configuración del Nivel")]
    public string archivoCSV = "NivelesBalanza"; 
    public string nivelActivo = "Nivel1"; 
    public string nivelBase = "Nivel2"; 
    public bool usarMacroDDA = true;

    [Header("DDA: Configuración de Ayuda")]
    public List<string> idsSolucionNivel; 

    [Header("Diccionario de Prefabs")]
    public List<ObjetoPesoCSV> diccionarioPrefabs;

    [Header("Referencias de la Escena")]
    public List<Transform> plataformasBase; 
    public Transform zonaPesosLibres;

    // --- LA MAGIA: Cambiamos Awake por Start para esperar al DDA ---
    void Start() 
    {
        CargarNivel();
    }

    // --- DEVOLVEMOS LA LIMPIEZA PARA BORRAR BLOQUES FANTASMAS ---
    void LimpiarNivel()
    {
        foreach (var plat in plataformasBase)
        {
            if (plat == null) continue;
            foreach (Transform hijo in plat)
            {
                // Borra todo lo que parezca un bloque viejo antes de cargar
                if (hijo.CompareTag("objeto") || hijo.name.Contains("Clone") || hijo.name.Contains("Contenedor"))
                {
                    Destroy(hijo.gameObject);
                }
            }
        }
        if (zonaPesosLibres != null)
        {
            foreach (Transform hijo in zonaPesosLibres) Destroy(hijo.gameObject);
        }
    }

    void CargarNivel()
    {
        LimpiarNivel(); // 1. Limpiamos la escena de bloques manuales

        TextAsset csvData = Resources.Load<TextAsset>(archivoCSV);
        if (csvData == null) 
        { 
            Debug.LogError("No se encontró el archivo CSV en la carpeta Resources."); 
            return; 
        }

        string nivelACargar = nivelActivo; 

        // 2. Le preguntamos al DDA (que ahora sí tiene la memoria actualizada)
        if (usarMacroDDA && DDA_DifusoPuro.Instancia != null)
        {
            EstadoJugador historial = DDA_DifusoPuro.Instancia.estadoNivelAnterior;
            
            switch (historial)
            {
                case EstadoJugador.Bloqueado:
                case EstadoJugador.Frustrado:
                    nivelACargar = "NivelFacil";
                    break;
                case EstadoJugador.Enganchado:
                    nivelACargar = "NivelMedio";
                    break;
                case EstadoJugador.Optimo:
                    nivelACargar = "NivelDificil";
                    break;
            }
            Debug.Log($"<color=magenta>Macro-DDA:</color> Jugador terminó {historial}. Redirigiendo a: {nivelACargar}");
        }

        // 3. Leemos el Excel
        string[] lineas = csvData.text.Split('\n');
        
        for(int i = 1; i < lineas.Length; i++) 
        {
            if (string.IsNullOrWhiteSpace(lineas[i])) continue; 

            string[] celdas = lineas[i].Trim().Split(';');

            if (celdas[0] == nivelACargar)
            {
                GenerarEcuacion(celdas[2]); 
                GenerarPesosLibres(celdas[3]); 

                // 4. Inicializamos TODO
                PlatformValueDisplay[] todasLasPlats = Object.FindObjectsByType<PlatformValueDisplay>(FindObjectsSortMode.None);
                foreach (var plat in todasLasPlats)
                {
                    plat.InicializarValor();
                }
                
                return; // Terminamos exitosamente
            }
        }
        Debug.LogError("Macro-DDA Error: No se encontró la fila " + nivelACargar + " en el CSV. Revisa mayúsculas y minúsculas.");
    }

    void GenerarEcuacion(string datosEcuacion)
    {
        string[] grupos = datosEcuacion.Split('|');
        
        for(int i = 0; i < grupos.Length; i++)
        {
            if (i >= plataformasBase.Count) break; 
            
            string[] elementos = grupos[i].Split('+');
            float alturaAcumulada = 0f; 
            
            SnapPoint[] snapPoints = plataformasBase[i].GetComponentsInChildren<SnapPoint>();
            int snapIndex = 0;

            foreach(string elem in elementos)
            {
                if (elem == "0" || string.IsNullOrWhiteSpace(elem)) continue; 

                bool esFijo = elem.StartsWith("F");
                string idReal = esFijo ? elem.Substring(1) : elem; 

                float alturaIdeal = ObtenerOffsetPorID(idReal);
                if (alturaAcumulada == 0f) alturaAcumulada = alturaIdeal;

                GameObject obj = InstanciarPorID(idReal, Vector3.zero);
                if (obj != null)
                {
                    if (!esFijo && snapIndex < snapPoints.Length)
                    {
                        snapPoints[snapIndex].ColocarObjeto(obj);
                        snapIndex++;
                    }
                    else
                    {
                        obj.transform.SetParent(plataformasBase[i]);
                        obj.transform.position = plataformasBase[i].position + new Vector3(0, alturaAcumulada, 0);
                        obj.tag = "Untagged"; 
                        alturaAcumulada += alturaIdeal + 0.1f; 
                    }
                }
            }
        }
    }

    void GenerarPesosLibres(string datosPesos)
    {
        string[] elementos = datosPesos.Split('|');
        int maxPorFila = 3; 
        float separacion = 1.2f; 
        int contador = 0;
        
        foreach(string elem in elementos)
        {
            if (elem == "0" || string.IsNullOrWhiteSpace(elem)) continue;

            float posX = (contador % maxPorFila) * separacion;
            float posZ = (contador / maxPorFila) * separacion;

            GameObject obj = InstanciarPorID(elem, zonaPesosLibres.position);
            if (obj != null)
            {
                obj.transform.SetParent(zonaPesosLibres);
                obj.transform.localPosition = new Vector3(posX, 0, posZ);
                contador++;
            }
        }
    }

    GameObject InstanciarPorID(string idBuscado, Vector3 posicion)
    {
        string idLimpio = idBuscado.Replace("_S", "");
        foreach(var map in diccionarioPrefabs)
        {
            if (map.idCSV == idLimpio)
            {
                GameObject nuevoObjeto = Instantiate(map.prefab, posicion, Quaternion.identity);
                if (idsSolucionNivel.Contains(idBuscado)) nuevoObjeto.AddComponent<BloqueAyuda>();
                return nuevoObjeto;
            }
        }
        return null;
    }

    float ObtenerOffsetPorID(string idBuscado)
    {
        string idLimpio = idBuscado.Replace("_S", "");
        foreach(var map in diccionarioPrefabs)
        {
            if (map.idCSV == idLimpio) return map.alturaFijaOffset;
        }
        return 0.5f;
    }
}