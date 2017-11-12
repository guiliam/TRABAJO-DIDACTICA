using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pair<T>
{
    private T first;
    private T second;

    Pair(T f, T s)
    {
        first = f;
        second = s;
    }

    public T First
    {
      get { return first; }
    }
    public T Second
    {
      get { return second; }
    }
}

public struct Secuencia
{
    /*
     * Almacena todas las palabras de cada secuencia. Las parejas validas sona las correspondientes a los indices entre 0 y'parejas' (incluido)
     * es decir, si nombres y verbos tienen una longitud de 5 y parejas = 3, las parejas validas seran (0,0), (1,1), (2,2) y (3,3). el resto de combinaciones no seran validas 
     */

    public string[] nombres;
    public string[] verbos;

    public int parejas;

    public Secuencia(string[] palsA, string[] palsB, int parejas)
    {
        nombres = palsA;
        verbos = palsB;
        this.parejas = parejas;
    }
}

public class SecuenciasAcciones{

    static SecuenciasAcciones instanciaActiva;

    public Secuencia[] Secuencias = new Secuencia[]
        {
            //tirar palanca
             new Secuencia(
                new string[]{"coger", "tirar"},
                new string[]{"palanca", "palanca"},
                1),
            
            //encender antorcha
            new Secuencia(
                new string[]{"prender", "acercar", "encender"},
                new string[]{"fuego", "antorcha", "antorcha"},
                2),
            //abrir puerta
            new Secuencia(
                new string[]{"portal", "pomo", "pomo", "puerta", "escalera", "visillo"},
                new string[]{"acercarse", "coger", "girar", "empujar", "golpear", "sacar"},
                3),
            //sacar DNI
             new Secuencia(
                new string[]{"cartera", "cartera", "DNI", "DNI", "DNI", "tarjeta", "carné", "monedas", "bolsillo"},
                new string[]{"sacar", "abrir", "buscar", "coger", "sacar", "contar", "observar", "leer"},
                4),

        };



    public static SecuenciasAcciones Getinstance()
    {
        if (instanciaActiva == null)
            instanciaActiva = new SecuenciasAcciones();
        return instanciaActiva;

    }
}
