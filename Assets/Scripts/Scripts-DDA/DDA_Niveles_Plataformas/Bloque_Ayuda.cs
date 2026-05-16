using UnityEngine;
using System.Collections.Generic;

public class BloqueAyuda : MonoBehaviour
{
    private List<Material> materialesBloque = new List<Material>();
    private Color colorOriginalDeLaRoca = Color.black; 

    [Header("Configuración de Brillo")]
    public Color colorResaltado = Color.yellow;
    public float intensidadBrillo = 1.5f;

    private float desfaseAleatorio;
    private bool debeBrillar = false; 

    void Awake()
    {
        MeshRenderer[] todosLosRenderers = GetComponentsInChildren<MeshRenderer>(true);
        
        foreach (MeshRenderer r in todosLosRenderers)
        {
            if (r == null || r.gameObject.name.Contains("Text") || r.gameObject.name.Contains("numero")) continue;

            if (r.sharedMaterial != null)
            {
                colorOriginalDeLaRoca = r.sharedMaterial.GetColor("_EmissionColor");

                Material mat = r.material; 
                mat.EnableKeyword("_EMISSION"); 
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive; 
                materialesBloque.Add(mat);
            }
        }
        
        desfaseAleatorio = Random.Range(0f, 2f);
    }

    void Start() 
    {
        this.enabled = true;

        if(DDA_DifusoPuro.Instancia != null) 
            EscucharDDA(DDA_DifusoPuro.Instancia.estadoActual);
    }

    void OnEnable() 
    {
        DDA_DifusoPuro.OnCambioEstado += EscucharDDA;
        if(DDA_DifusoPuro.Instancia != null) EscucharDDA(DDA_DifusoPuro.Instancia.estadoActual);
    }

    void OnDisable() => DDA_DifusoPuro.OnCambioEstado -= EscucharDDA;

    private void EscucharDDA(EstadoJugador nuevoEstado)
    {
        debeBrillar = (nuevoEstado == EstadoJugador.Frustrado || nuevoEstado == EstadoJugador.Bloqueado);
    }

    void Update()
    {
        if (materialesBloque.Count == 0) return;

        if (debeBrillar)
        {
            float resplandor = Mathf.PingPong((Time.time + desfaseAleatorio) * 2f, 1f);
            Color estadoEncendido = colorResaltado * intensidadBrillo;
            Color finalColor = Color.Lerp(colorOriginalDeLaRoca, estadoEncendido, resplandor);

            foreach (Material mat in materialesBloque)
                mat.SetColor("_EmissionColor", finalColor);
        }
        else
        {
            foreach (Material mat in materialesBloque)
                mat.SetColor("_EmissionColor", colorOriginalDeLaRoca);
        }
    }
}