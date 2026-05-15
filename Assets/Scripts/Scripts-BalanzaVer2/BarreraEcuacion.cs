using UnityEngine;
using System.Collections.Generic;

public class BarreraEcuacion : MonoBehaviour
{
    [Header("Conexión con la Balanza")]
    [Tooltip("Arrastra aquí los scripts de texto de la izquierda")]
    public List<PlatformValueDisplay> ladoIzquierdo;
    [Tooltip("Arrastra aquí los scripts de texto de la derecha")]
    public List<PlatformValueDisplay> ladoDerecho;

    [Header("El Muro a Controlar")]
    [Tooltip("Arrastra aquí el objeto que servirá de pared (el que tiene el Collider)")]
    public GameObject muroInvisible;

    void Update()
    {
        if (muroInvisible == null) return;

        int pesoIzquierdo = ObtenerPeso(ladoIzquierdo);
        int pesoDerecho = ObtenerPeso(ladoDerecho);
        bool estaBalanceado = (pesoIzquierdo == pesoDerecho && pesoIzquierdo > 0);
        if (muroInvisible.activeSelf == estaBalanceado)
        {
            muroInvisible.SetActive(!estaBalanceado);
        }
    }

    int ObtenerPeso(List<PlatformValueDisplay> lado)
    {
        int total = 0;
        foreach (var plat in lado)
        {
            if (plat != null) total += plat.GetValue();
        }
        return total;
    }
}