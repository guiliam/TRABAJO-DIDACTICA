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

    int tabuladores;
    string lineaActual;

    //variables que definen el codigo que va a generarase
    int maxLineas,
        condiciones,
        buclesWhile,
        buclesFor;
    bool elsePendiente;

    #region VARIABLES UNITY
    Text soporteCodigo;
    #endregion

    //para almacenar variables
    List<char> identificadoresVariables = new List<char>();
    List<int> valoresVariables = new List<int>();

    //almacena el codigo creado
    List<operacion> operaciones = new List<operacion>();



    #region FUNCIONES UNITY
    void Awake()
    {
        soporteCodigo = GameObject.Find("SoporteCodigo").GetComponent<Text>();
        inicializarVariablesRelacionadasConCodigo();
        dificultad = MUY_FACIL;
        decidirCodigo();
        generarLineasCodigo();
    }

    #endregion


    private void decidirCodigo()
    {
        //decide cuantas lineas (APROXIMADAMENTE EN EL CASO DE LOS BUCLES) habra
        maxLineas = (dificultad + 1) * 3;
        maxLineas = (int) Mathf.Ceil(maxLineas * UnityEngine.Random.Range(dificultad, maxLineas));

        //se generan las variables
        obtenerVariables();

        //decidimos que aparecera en el codigo segun la dificultad
        switch (dificultad)
        {
            case MUY_FACIL:
                operadoresAdmitidos = 3;
                numeroDeVariables = 1;
                operadoresReducidos = false;
                break;

            case FACIL:
                operadoresAdmitidos = 3;
                numeroDeVariables = 2;
                operadoresReducidos = true;
                break;

            case MEDIO:
                //generamos condiciones
                operadoresReducidos = true;
                condiciones = UnityEngine.Random.Range(1, 3);
                break;

            case MEDIO_DIFICIL:
                break;

            case DIFICIL:
                break;

            case MUY_DIFICIL:
                break;

        }
    }

    private void generarLineasCodigo()
    {
        int maximo = dificultad - 1;
        if (maximo < 0) maximo = 0;
        int seleccion = 0;

        obtenerVariables();
        

        for (int iterador = 0; iterador < maxLineas; iterador++)
        {
            print("generandoLinea" + maximo);
            seleccion = UnityEngine.Random.Range(0, maximo);

            switch (seleccion)
            {
                case 0: //linea normal
                    generarLineaNormal();
                    break;
            }

        }

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

    private void generarLineaNormal()
    {
        print("Generando linea normal");
        int operador = UnityEngine.Random.Range(0, operadoresAdmitidos);
        float auxRand = 0;

        if (operadoresReducidos)
        {
            auxRand = UnityEngine.Random.value;
        }
        else
        {
            auxRand = 1;
        }


        if (auxRand < 0.5)
        {
            soporteCodigo.text += operacionSimple(identificadoresVariables[UnityEngine.Random.Range(0, numeroDeVariables - 1)],
                OPERADORES[UnityEngine.Random.Range(0, operadoresAdmitidos - 1)],
                UnityEngine.Random.Range(0, 10));

        }

        else
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                soporteCodigo.text +=  operacionSimpleSinOperadoresContraidosPrimeroValor(identificadoresVariables[UnityEngine.Random.Range(0, numeroDeVariables - 1)],
                OPERADORES[UnityEngine.Random.Range(0, operadoresAdmitidos - 1)],
                UnityEngine.Random.Range(0, 10));
            }
            else
            {
                soporteCodigo.text +=  operacionSimpleSinOperadoresContraidosPrimeroVariable(identificadoresVariables[UnityEngine.Random.Range(0, numeroDeVariables - 1)],
                OPERADORES[UnityEngine.Random.Range(0, operadoresAdmitidos - 1)],
                UnityEngine.Random.Range(0, 10));
            }
        }
    }

    private void inicializarVariablesRelacionadasConCodigo()
    {
        identificadoresVariables.Clear();
        valoresVariables.Clear();
        soporteCodigo.text = "";
    }


    #region GENERADORES LINEAS

    private string tabular()
    {
        string cadena = "";
        for (int i = 0; i < tabuladores; i++)
        {
            cadena += "\t";
        }
        return cadena;
    }

    private string declaracionVariable(char var, int valor)
    {
        return tabular() + var + " = " + Convert.ToString(valor) + "; \n";
    }

    private string asignacion(char var1, char var2)
    {
        return tabular() +  var1 + " = " + var2 + "; \n";
    }

    private string operacionSimpleSinOperadoresContraidosPrimeroVariable(char var, char operador, int valor)
    {
        return tabular() +  var + " = " + var + " " + operador + " " + Convert.ToString(valor) + "; \n";
    }

    private string operacionSimpleSinOperadoresContraidosPrimeroValor(char var, char operador, int valor)
    {
        return tabular() + var + " = " + Convert.ToString(valor) + " " + operador + " " + var + "; \n";
    }

    private string operacionSimple(char var, char operador, int valor)
    {
        return tabular() + var + operador + "= " + Convert.ToString(valor) + "; \n";
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
