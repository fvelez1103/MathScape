using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GestorFormulario : MonoBehaviour
{
    [Header("Elementos de la UI")]
    public GameObject panelFormulario;
    public TMP_InputField inputNombre;
    public TMP_InputField inputEdad;
    public Slider sliderConocimiento; 
    public TextMeshProUGUI textoValorSlider; 

    [Header("Conexión al Juego")]
    public MonoBehaviour scriptMovimientoJugador;

    void Start()
    {
        panelFormulario.SetActive(false);
        
        if(sliderConocimiento != null)
        {
            sliderConocimiento.onValueChanged.AddListener(delegate { ActualizarTextoSlider(); });
        }
    }

    public void MostrarFormulario()
    {
        panelFormulario.SetActive(true);
        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.None;
    }

    private void ActualizarTextoSlider()
    {
        if(textoValorSlider != null)
        {
            textoValorSlider.text = sliderConocimiento.value.ToString();
        }
    }

    public void GuardarYContinuar()
    {
        string nombre = inputNombre.text;
        int edad = 0;
        int.TryParse(inputEdad.text, out edad); 
        int conocimiento = (int)sliderConocimiento.value;

        if (string.IsNullOrWhiteSpace(nombre) || edad <= 0)
        {
            Debug.LogWarning("Faltan datos por llenar");
            return; 
        }

        if (FirebaseManager.Instancia != null)
        {
            FirebaseManager.Instancia.RegistrarPerfilInicial(nombre, edad, conocimiento);
        }


        panelFormulario.SetActive(false);

        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
    }
}