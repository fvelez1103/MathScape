using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class CanvasTriviaController : MonoBehaviour
{
    [Header("UI Elementos")]
    [SerializeField] private TMP_Text textoPregunta;
    [SerializeField] private Image imagenPregunta;
    [SerializeField] private Button[] botonesRespuestas; 
    [SerializeField] private TMP_Text[] textosRespuestas; 

    [Header("Referencias de Entidades (Para congelar)")]
    [SerializeField] private GameObject jugador;
    [SerializeField] private GameObject villano;

    private TriviaScreen pantallaActual;
    private int respuestaCorrectaIndex;

    void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            ConfirmarSeleccionActual();
        }
    }

    private void ConfirmarSeleccionActual()
    {
        GameObject seleccionado = EventSystem.current.currentSelectedGameObject;

        if (seleccionado != null)
        {
            Button boton = seleccionado.GetComponent<Button>();
            if (boton != null)
            {
                boton.onClick.Invoke();
            }
        }
    }

    [System.Obsolete]
    public void ConfigurarTrivia(PreguntaTrivia data, TriviaScreen pantalla)
    {
        pantallaActual = pantalla;
        respuestaCorrectaIndex = data.indiceRespuestaCorrecta;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
        AlternarEstadoEntidades(0f);

        ConfigurarContenidoPregunta(data);

        for (int i = 0; i < botonesRespuestas.Length; i++)
        {
            if (i < data.opciones.Length)
            {
                botonesRespuestas[i].gameObject.SetActive(true);
                textosRespuestas[i].text = data.opciones[i];
                
                int index = i; 
                botonesRespuestas[i].onClick.RemoveAllListeners();
                botonesRespuestas[i].onClick.AddListener(() => EvaluarRespuesta(index));
            }
            else
            {
                botonesRespuestas[i].gameObject.SetActive(false);
            }
        }

        botonesRespuestas[0].Select();
    }

    private void ConfigurarContenidoPregunta(PreguntaTrivia data)
    {
        if (data.esImagen)
        {
            textoPregunta.gameObject.SetActive(false);
            imagenPregunta.gameObject.SetActive(true);
            Sprite loadedSprite = Resources.Load<Sprite>(data.contenido);
            if (loadedSprite != null) imagenPregunta.sprite = loadedSprite;
        }
        else
        {
            imagenPregunta.gameObject.SetActive(false);
            textoPregunta.gameObject.SetActive(true);
            textoPregunta.text = data.contenido;
        }
    }

    public void Aplicar5050()
    {
        int eliminados = 0;
        int opcionesVisibles = 0;

        foreach (var btn in botonesRespuestas) {
            if (btn.gameObject.activeSelf) opcionesVisibles++;
        }
        int limiteEliminar = (opcionesVisibles > 3) ? 2 : ((opcionesVisibles == 3) ? 1 : 0);

        for (int i = 0; i < botonesRespuestas.Length; i++)
        {
            if (i != respuestaCorrectaIndex && eliminados < limiteEliminar && botonesRespuestas[i].gameObject.activeSelf)
            {
                botonesRespuestas[i].gameObject.SetActive(false);
                eliminados++;
            }
        }

        foreach (var btn in botonesRespuestas)
        {
            if (btn.gameObject.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null); 
                btn.Select(); 
                break; 
            }
        }
    }

    [System.Obsolete]
    private void EvaluarRespuesta(int indexSeleccionado)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        bool esCorrecta = (indexSeleccionado == respuestaCorrectaIndex);
        
        string textoElegido = textosRespuestas[indexSeleccionado].text;

        if (pantallaActual != null)
        {
            pantallaActual.RegistrarIntento(esCorrecta, textoElegido);
        }

        if (esCorrecta)
        {
            Debug.Log("<color=green>¡Correcto!</color>");
            Time.timeScale = 1f;
            AlternarEstadoEntidades(1f);
            IniciarRotacionTuberia();
            gameObject.SetActive(false); 
        }
        else
        {
            Debug.Log("<color=red>Incorrecto...</color>");
            Time.timeScale = 1f;
            AlternarEstadoEntidades(1f);
            DesaparecerSuelo();
            gameObject.SetActive(false); 
        }
    }

    private void IniciarRotacionTuberia()
    {
        if (pantallaActual.tuberiaARotar != null)
        {
            pantallaActual.StartCoroutine(AnimarRotacion(pantallaActual.tuberiaARotar, pantallaActual.rotacionFinalTuberia));
        }
    }

    IEnumerator AnimarRotacion(Transform objeto, Vector3 gradosFinales)
    {
        float duracion = 1.5f; 
        float tiempo = 0;
        Quaternion rotacionInicial = objeto.localRotation;
        Quaternion rotacionObjetivo = Quaternion.Euler(gradosFinales);
        while (tiempo < duracion)
        {
            objeto.localRotation = Quaternion.Slerp(rotacionInicial, rotacionObjetivo, tiempo / duracion);
            tiempo += Time.unscaledDeltaTime; 
            yield return null;
        }
        objeto.localRotation = rotacionObjetivo; 
    }

    private void DesaparecerSuelo()
    {
        if (pantallaActual.sueloADesaparecer != null)
        {
            pantallaActual.sueloADesaparecer.SetActive(false);
        }
    }

    private void AlternarEstadoEntidades(float speed)
    {
        if (jugador != null) {
            Animator a = jugador.GetComponent<Animator>();
            if (a != null) a.speed = speed;
        }
        if (villano != null && villano.activeInHierarchy) {
            Animator a = villano.GetComponent<Animator>();
            if (a != null) a.speed = speed;
            UnityEngine.AI.NavMeshAgent nav = villano.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (nav != null) nav.isStopped = (speed == 0f);
        }
    }
}