using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CreacionCodigoAleatorio : MonoBehaviour {

    private const int TOTAL_BOTONES_SOLUCION = 8;

    struct operacion
    {
        int operador; //indice del operador
        int variable; //indice a la variable a modificar
        int valor; //valor de la operacion

        //casos especiales
        bool operacionNoConmutativaConValorPrimero; //operaciones tipo x = 5 - x \\\ x = 5 / x \\\ x = 5 % x

        public operacion(int o, int v, int val, bool opNoConmu)
        {
            operador = o;
            variable = v;
            valor = val;
            operacionNoConmutativaConValorPrimero = opNoConmu;
        }

        public int GetOperador()
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

        public bool GetOperacionNoConmutativaConValorPrimero()
        {
            return operacionNoConmutativaConValorPrimero;
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
    Text textoPregunta;
    GameObject[] botonesSolucion;
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
        textoPregunta = GameObject.Find("TextoPregunta").GetComponent<Text>();
        botonesSolucion = new GameObject[TOTAL_BOTONES_SOLUCION];

        for (int i = 0; i < TOTAL_BOTONES_SOLUCION; i++)
        {
            botonesSolucion[i] = GameObject.Find("Boton" + i);
        }

        inicializarVariablesRelacionadasConCodigo();
        dificultad = MUY_FACIL;
        bool codigoBueno = false; //para evitar divisiones entre 0
        while (!codigoBueno)
        {
            decidirCodigo();
            generarLineasCodigo();
            codigoBueno = resolverCodigo();
        }
        prepararBotonesYPregunta();
        print(valoresVariables[0]);
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
                operadoresAdmitidos = 6;
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
            //print("generandoLinea" + maximo);
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
        //print("Generando linea normal");
        //int operador = UnityEngine.Random.Range(1, operadoresAdmitidos);
        float auxRand = 0;
        bool operacionNoConmutativaConValorDelante = false;

        int indiceOperador, indiceVariable, valor;
        indiceOperador = UnityEngine.Random.Range(1, operadoresAdmitidos - 1);
        indiceVariable = UnityEngine.Random.Range(0, numeroDeVariables - 1);
        valor = UnityEngine.Random.Range(1, 10);


        //print("operador, variable, valor / " + indiceOperador + indiceVariable + valor); 
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
            soporteCodigo.text += operacionSimple(identificadoresVariables[indiceVariable],
                OPERADORES[indiceOperador],
                valor);

        }

        else
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                if (indiceOperador == 2 || indiceOperador == 4 || indiceOperador == 5)
                {
                    print("op no conmu " + indiceOperador);
                    operacionNoConmutativaConValorDelante = true;
                }
                soporteCodigo.text +=  operacionSimpleSinOperadoresContraidosPrimeroValor(identificadoresVariables[indiceVariable],
                OPERADORES[indiceOperador],
                valor);
            }
            else
            {
                soporteCodigo.text +=  operacionSimpleSinOperadoresContraidosPrimeroVariable(identificadoresVariables[indiceVariable],
                OPERADORES[indiceOperador],
                valor);
            }
        }

        //se anade la operacion a la lista
        operaciones.Add(new operacion(indiceOperador, indiceVariable, valor, operacionNoConmutativaConValorDelante));
    }

    private void inicializarVariablesRelacionadasConCodigo()
    {
        identificadoresVariables.Clear();
        valoresVariables.Clear();
        soporteCodigo.text = "";
    }

    private bool resolverCodigo()
    {
        try
        {
            foreach (operacion op in operaciones)
            {
                    switch (OPERADORES[op.GetOperador()])
                    {
                        case '='://ASIGNACION
                            valoresVariables[op.GetVariable()] = valoresVariables[op.GetValor()];
                            break;
                        case '+':
                            valoresVariables[op.GetVariable()] += op.GetValor();
                            break;
                        case '-':
                            if (op.GetOperacionNoConmutativaConValorPrimero())
                            {
                                valoresVariables[op.GetVariable()] = op.GetValor() - valoresVariables[op.GetVariable()];
                            }
                            else
                                valoresVariables[op.GetVariable()] -= op.GetValor();
                            break;
                        case '*':
                            valoresVariables[op.GetVariable()] *= op.GetValor();
                            break;
                        case '/':
                            if (op.GetOperacionNoConmutativaConValorPrimero())
                            {
                                valoresVariables[op.GetVariable()] = op.GetValor() / valoresVariables[op.GetVariable()];
                            }
                            else
                                valoresVariables[op.GetVariable()] /= op.GetValor();
                            break;
                        case '%':
                            if (op.GetOperacionNoConmutativaConValorPrimero())
                            {
                                valoresVariables[op.GetVariable()] = op.GetValor() % valoresVariables[op.GetVariable()];
                            }
                            else
                                valoresVariables[op.GetVariable()] %= op.GetValor();
                            break;
                    }

                    print(valoresVariables[0]);
                }
            return true;
        }
        catch(DivideByZeroException ex)
        {
            return false;
        }

    }

    private void prepararBotonesYPregunta()
    {
        int indiceVariable = UnityEngine.Random.Range(0, identificadoresVariables.Count - 1);
        prepararPregunta(indiceVariable);
        int botonCorrecto =  UnityEngine.Random.Range(0, dificultad + 1);
        List<int> aux = new List<int>();

        for(int i = valoresVariables[indiceVariable] - 20 ; i < valoresVariables[indiceVariable] + 20; i++)
        {
            aux.Add(i);
        }

        int selecion;
        for (int i = 0; i < dificultad + 2; i++)
        {
            selecion = UnityEngine.Random.Range(0, aux.Count - 1);
            botonesSolucion[i].SetActive(true);
            botonesSolucion[i].GetComponentInChildren<Text>().text = Convert.ToString(aux[selecion]);
            aux.Remove(aux[selecion]);
        }

        for (int i = dificultad + 2; i < TOTAL_BOTONES_SOLUCION; i++)
        {
            botonesSolucion[i].SetActive(false);
        }
        //elegimos el boton con la respuesta correcta
        GameObject.Find("Boton" + botonCorrecto).GetComponentInChildren<Text>().text = Convert.ToString(valoresVariables[indiceVariable]);
    }

    private void prepararPregunta(int variableParaPreguntar)
    {
        textoPregunta.text = "¿Qué valor tiene la variable \'" + identificadoresVariables[variableParaPreguntar] + "\'?";
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
