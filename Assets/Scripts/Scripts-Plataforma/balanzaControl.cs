using UnityEngine;

public class BalanzaMovimiento : MonoBehaviour
{
    [Header("Plataformas")]
    public Transform plataformaA;
    public Transform plataformaB;

    [Header("Valores")]
    public PlatformValueDisplay valorA;
    public PlatformValueDisplay valorB;

    [Header("Configuración")]
    public float factorMovimiento = 0.2f; 
    public float velocidad = 5f;
    public float alturaEquilibrio = 3.08f; 
    private bool igualdadAlcanzada = false;
    private cogerObjetos scriptCogerObjetos;

   void Start()
    {
        scriptCogerObjetos = Object.FindFirstObjectByType<cogerObjetos>();
    }

    void Update()
    {
        int a = valorA.GetValue();
        int b = valorB.GetValue();

        bool jugadorHaceTrampa = false;
        if (scriptCogerObjetos != null)
        {
            jugadorHaceTrampa = scriptCogerObjetos.EstaSosteniendoObjeto();
        }

        if (a == b && a != 0 && !igualdadAlcanzada && !jugadorHaceTrampa)
        {
            AlcanzarIgualdad();
        }

        if (!igualdadAlcanzada)
        {
            float diferencia = a - b;
            float targetYA = alturaEquilibrio - (diferencia * factorMovimiento);
            float targetYB = alturaEquilibrio + (diferencia * factorMovimiento);
            Vector3 targetA = new Vector3(plataformaA.localPosition.x, targetYA, plataformaA.localPosition.z);
            Vector3 targetB = new Vector3(plataformaB.localPosition.x, targetYB, plataformaB.localPosition.z);
            plataformaA.localPosition = Vector3.Lerp(plataformaA.localPosition, targetA, Time.deltaTime * velocidad);
            plataformaB.localPosition = Vector3.Lerp(plataformaB.localPosition, targetB, Time.deltaTime * velocidad);
        }
        else
        {
            Vector3 targetFinalA = new Vector3(plataformaA.localPosition.x, alturaEquilibrio, plataformaA.localPosition.z);
            Vector3 targetFinalB = new Vector3(plataformaB.localPosition.x, alturaEquilibrio, plataformaB.localPosition.z);
            plataformaA.localPosition = Vector3.Lerp(plataformaA.localPosition, targetFinalA, Time.deltaTime * velocidad);
            plataformaB.localPosition = Vector3.Lerp(plataformaB.localPosition, targetFinalB, Time.deltaTime * velocidad);
        }
    }

   void AlcanzarIgualdad()
    {
        igualdadAlcanzada = true;
        if (scriptCogerObjetos != null)
        {
            scriptCogerObjetos.BloquearAgarrarNuevos(); 
        }
    }

}
