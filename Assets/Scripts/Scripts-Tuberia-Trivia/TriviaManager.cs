using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class TriviaManager : MonoBehaviour
{
    public static TriviaManager Instancia;
    
    private List<PreguntaTrivia> baseDeDatosPreguntas = new List<PreguntaTrivia>();
    
    private List<PreguntaTrivia> facilesPendientes = new List<PreguntaTrivia>();
    private List<PreguntaTrivia> mediasPendientes = new List<PreguntaTrivia>();
    private List<PreguntaTrivia> dificilesPendientes = new List<PreguntaTrivia>();

    private void Awake()
    {
        if (Instancia == null) 
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            CargarPreguntasDesdeCSV();
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void CargarPreguntasDesdeCSV()
    {
        TextAsset archivoCSV = Resources.Load<TextAsset>("Preguntas"); 
        if (archivoCSV == null) {
            Debug.LogError("No se encontró el archivo Preguntas.csv en Resources");
            return;
        }

        string[] filas = archivoCSV.text.Split('\n');

        for (int i = 1; i < filas.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(filas[i])) continue;

            string[] columnas = filas[i].Split(';');

            PreguntaTrivia nuevaPregunta = new PreguntaTrivia
            {
                id = columnas[0],
                dificultad = columnas[1].Trim(), 
                esImagen = columnas[2].Trim().ToLower() == "true",
                contenido = columnas[3],
                opciones = ObtenerOpcionesValidas(columnas[4], columnas[5], columnas[6], columnas[7]),
                indiceRespuestaCorrecta = int.Parse(columnas[8].Trim())
            };

            baseDeDatosPreguntas.Add(nuevaPregunta);
        }

        ReiniciarListasPendientes();
        Debug.Log($"Base de datos lista con {baseDeDatosPreguntas.Count} preguntas.");
    }

    private void ReiniciarListasPendientes()
    {
        facilesPendientes = baseDeDatosPreguntas.Where(p => p.dificultad == "Facil").ToList();
        mediasPendientes = baseDeDatosPreguntas.Where(p => p.dificultad == "Medio").ToList();
        dificilesPendientes = baseDeDatosPreguntas.Where(p => p.dificultad == "Dificil").ToList();
    }

    public PreguntaTrivia ObtenerPregunta(string dificultadBuscada)
    {
        List<PreguntaTrivia> listaObjetivo;

        switch (dificultadBuscada)
        {
            case "Facil": listaObjetivo = facilesPendientes; break;
            case "Medio": listaObjetivo = mediasPendientes; break;
            case "Dificil": listaObjetivo = dificilesPendientes; break;
            default: listaObjetivo = facilesPendientes; break;
        }

        if (listaObjetivo.Count == 0)
        {
            Debug.Log($"Recargando preguntas de dificultad: {dificultadBuscada}");
            RefrescarDificultad(dificultadBuscada);
            return ObtenerPregunta(dificultadBuscada); 
        }

        int indexAleatorio = Random.Range(0, listaObjetivo.Count);
        PreguntaTrivia preguntaSeleccionada = listaObjetivo[indexAleatorio];

        listaObjetivo.RemoveAt(indexAleatorio);

        return preguntaSeleccionada;
    }

    private void RefrescarDificultad(string dif)
    {
        List<PreguntaTrivia> frescas = baseDeDatosPreguntas.Where(p => p.dificultad == dif).ToList();
        if (dif == "Facil") facilesPendientes = frescas;
        else if (dif == "Medio") mediasPendientes = frescas;
        else if (dif == "Dificil") dificilesPendientes = frescas;
    }

    private string[] ObtenerOpcionesValidas(string o1, string o2, string o3, string o4)
    {
        List<string> opcionesValidas = new List<string>();
        if (!string.IsNullOrWhiteSpace(o1)) opcionesValidas.Add(o1);
        if (!string.IsNullOrWhiteSpace(o2)) opcionesValidas.Add(o2);
        if (!string.IsNullOrWhiteSpace(o3)) opcionesValidas.Add(o3);
        if (!string.IsNullOrWhiteSpace(o4)) opcionesValidas.Add(o4);
        return opcionesValidas.ToArray();
    }
}