using System;


[Serializable]
public class PerfilJugador
{
    public string nombre;
    public int edad;
    public int conocimientoPrevio; 

    public PerfilJugador(string nombre, int edad, int conocimiento)
    {
        this.nombre = nombre;
        this.edad = edad;
        this.conocimientoPrevio = conocimiento;
    }
}

[Serializable]
public class MetricasRendimiento
{
    public float tiempoTotalSesion;
    public int aciertosMatematicos;
    public int erroresMatematicos;
    public int nivelAlcanzado;

    public MetricasRendimiento(float tiempo, int aciertos, int errores, int nivel)
    {
        this.tiempoTotalSesion = tiempo;
        this.aciertosMatematicos = aciertos;
        this.erroresMatematicos = errores;
        this.nivelAlcanzado = nivel;
    }
}