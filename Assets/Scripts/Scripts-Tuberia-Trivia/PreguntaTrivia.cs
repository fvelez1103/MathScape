[System.Serializable]
public class PreguntaTrivia
{
    public string id;
    public string dificultad;
    public bool esImagen;
    public string contenido; 
    public string[] opciones; 
    public int indiceRespuestaCorrecta;
}