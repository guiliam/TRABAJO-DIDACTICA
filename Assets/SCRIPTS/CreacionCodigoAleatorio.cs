using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CreacionCodigoAleatorio : MonoBehaviour {

    struct operacion
    {
        char operador;
        int variable; //indice a la variable a modificar
        int valor; //valor de la operacion

        public operacion(char o, int v, int val)
        {
            operador = o;
            variable = v;
            valor = val;
        }

        public char GetOperador()
        {
            return operador;
        }

        public int GetVariable()
        {
            return variable;
        }

        public int GetValor()
        {
            return valor;
        }
    }

    public const int MUY_FACIL = 0;
    public const int FACIL = 1;
    //aparecen condiciones
    public const int MEDIO = 2;
    //bucles while
    public const int MEDIO_DIFICIL = 3;
    //bucles for
    public const int DIFICIL = 4;
    //bucles anidados
    public const int MUY_DIFICIL = 5;

    private char[] OPERADORES = {'=', '+' , '-', '*', '/', '%'};

    private int dificultad;
    private int operadoresAdmitidos;
    private int numeroDeVariables;
    private bool operadoresReducidos; //decide si podran usarse operadores como +=, -=, etc.

    #region VARIABLES UNITY
    Text soporteCodigo;
    #endregion

    //para almacenar variables
    List<char> identificadoresVariables = new List<char>();
    List<int> valoresVariables = new List<int>();




    #region FUNCIONES UNITY
    void Awake()
    {
        soporteCodigo = GameObject.Find("SoporteCodigo").GetComponent<Text>();
        inicializarVariablesRelacionadasConCodigo();
        dificultad = MUY_FACIL;
        establecerParametrosSegunDificultad();
        decidirCodigo();
    }

    #endregion



    void establecerParametrosSegunDificultad()
    {
        switch (dificultad)
        {
            case MUY_FACIL:
                operadoresAdmitidos = 2;
                numeroDeVariables = 10;
                break;
        }

    }

    private void decidirCodigo()
    {
        //decide cuantas lineas (APROXIMADAMENTE EN EL CASO DE LOS BUCLES) habra
        int maxLineas = (dificultad + 1) * 3;
        maxLineas = (int) Mathf.Ceil(maxLineas * UnityEngine.Random.Range(dificultad, maxLineas));

        //se generan las variables
        obtenerVariables();

        for (int iterador = 0; iterador < maxLineas; iterador++)
        {
        }
    }

    private void generarLineasCodigo()
    {


    }

    private void obtenerVariables()
    {
        //escoge caracteres para representar las variables
        char candidatoAVariable;
        bool repetir = false;
        for (int iterador = 0; iterador < numeroDeVariables; iterador++)
        {
            //se ecoge una letra al zara para las variables
            candidatoAVariable = Convert.ToChar(UnityEngine.Random.Range(0, 24) + 97);

            //se compruba que no haya sido usada ya
            

            foreach (char c in identificadoresVariables)
            {
                if (c == candidatoAVariable)
                {
                    //si la variable ha sido usada, descartamos y volvemos a empezar
                    repetir = true;
                    break;
                }
            }

            if (repetir)
            {
                repetir = false;
                iterador--;
                continue;
            }

            //si no esta repetida, la variable se anade a la lista
            identificadoresVariables.Add(candidatoAVariable);

            //se le asigna un valor aleatorio
            valoresVariables.Add(UnityEngine.Random.Range(0, 10));
        }

        generarLineasDeclaracionVariables();
    }


    private void generarLineasDeclaracionVariables()
    {
        //recorre las id de variables y los valores de estas y las inicializa a dichos valores
        for (int iterador = 0; iterador < identificadoresVariables.Count; iterador++)
        {
            soporteCodigo.text = soporteCodigo.text +
                declaracionVariable(identificadoresVariables[iterador], valoresVariables[iterador]) + "\n";
        }
    }

    private void inicializarVariablesRelacionadasConCodigo()
    {
        identificadoresVariables.Clear();
        valoresVariables.Clear();
        soporteCodigo.text = "";
    }


    #region GENERADORES LINEAS

    private string declaracionVariable(char var, int valor)
    {
        return var + " = " + Convert.ToString(valor) + ';';
    }

    private string asignacion(char var1, char var2)
    {
        return var1 + " = " + var2 + ';';
    }

    private string operacionSimpleSinOperadoresContraidosPrimeroVariable(char var, char operador, int valor)
    {
        return var + " = " + var + " " + operador + " " + Convert.ToString(valor) + ';';
    }

    private string operacionSimpleSinOperadoresContraidosPrimeroValor(char var, char operador, int valor)
    {
        return var + " = " + Convert.ToString(valor) + " " + operador + " " + var + ';';
    }

    private string operacionSimple(char var, char operador, int valor)
    {
        return var + operador + "= " + Convert.ToString(valor) + ';';
    }

    #endregion


    #region FUNCIONES_OPEACIONES

    private void asignacion(ref int var1, ref int var2)
    {
        var1 = var2;
    }

    private void suma(ref int resultado, int valor)
    {
        resultado += valor;
    }

    private void resta(ref int resultado, int valor)
    {
        resultado -= valor;
    }

    private void multiplicacion(ref int resultado, int valor)
    {
        resultado *= valor;
    }

    private void divisionEntera(ref int resultado, int valor)
    {
        resultado /= valor;
    }

    private void restoDivisionEntera(ref int resultado, int valor)
    {
        resultado %= valor;
    }
    #endregion
}
