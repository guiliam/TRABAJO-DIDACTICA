#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CreacionCodigoAleatorio : MonoBehaviour {

    private const int TOTAL_BOTONES_SOLUCION = 8;

    //TIPOS DE LINEA
    private const int LINEA_NORMAL = 0;
    private const int LINEA_CONDICION = 1;
    private const int BUCLE_WHILE = 2;
    private const int BUCLE_FOR = 3;
    List<int> listaReferenciaTiposLineas = new List<int>(new int[] {0,1,2,3});

    struct operacion
    {
        int operador; //indice del operador
        int variable; //indice a la variable a modificar
        int valor; //valor de la operacion

        //casos especiales
        bool operacionNoConmutativaConValorPrimero; //operaciones tipo x = 5 - x \\\ x = 5 / x \\\ x = 5 % x

        //bucles y condiciones
        bool bucle;
        bool condicional;
        bool condicionalConElse;

        List<operacion> instrucciones;
        List<operacion> instruccionesAlternativa;

        //constructor para operaciones normales y condicionales
        //si es una condicion, l operador sera el comparador, el valor aquello con lo que se comparara y la variable lo que sera comparado
        public operacion(int o, int v, int val, bool opNoConmu, bool condicional = false, bool condicionalElse = false, List<operacion> lista = null, List<operacion> listaLAter = null)
        {
            operador = o;
            variable = v;
            valor = val;
            operacionNoConmutativaConValorPrimero = opNoConmu;
            bucle = false;
            this.condicional = condicional;
            condicionalConElse = condicionalElse;
            instrucciones = lista;
            instruccionesAlternativa = listaLAter;
        }

        //constructor para bucles (la variable es la que decide cuando salir del bucle, el operador la condicion a satisfacer y el valor el valor con el que se va a comparar
        public operacion(int o, int v, int val, bool opNoConmu, bool bucle, List<operacion> lista = null)
        {
            operador = o;
            variable = v;
            valor = val;
            operacionNoConmutativaConValorPrimero = opNoConmu;
            this.bucle = bucle;
            condicional = false;
            condicionalConElse = false;
            instrucciones = lista;
            instruccionesAlternativa = null;
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

        public bool GetBucle()
        {
            return bucle;
        }

        public bool GetCondicional()
        {
            return condicional;
        }

        public bool GetCondicionalConElse()
        {
            return condicionalConElse;
        }

        public List<operacion> GetListaInstrucciones()
        {
            return instrucciones;
        }

        public List<operacion> GetListaInstruccionesAlternatva()
        {
            return instruccionesAlternativa;
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
    private const int SUMA = 1;
    private const int RESTA = 2;
    private const int MULTIPLICACION = 3;
    private const int DIVISION = 4;
    private const int RESTO = 5;

    private string[] COMPARADORES = {"==", "!=", ">=", "<=", "<", ">"};
    private const int IGUAL = 0;
    private const int DISTINTO = 1;
    private const int MAYOR_IGUAL = 2;
    private const int MENOR_IGUAL = 3;
    private const int MENOR = 4;
    private const int MAYOR = 5;

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

    //esta lista se usa para asignar los tipos de lineas y repetar las cantidades
    List<int> tiposLineasRestantes;

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

        //limpia las variables
        inicializarVariablesRelacionadasConCodigo();
        dificultad = MEDIO_DIFICIL;
        bool codigoBueno = false; //para evitar divisiones entre 0
        while (!codigoBueno)
        {
            //se decide parametros del codigo (lineas, bucles...)
            decidirCodigo();
            //se generan las lineas
            generarLineasCodigo();
            //se resuelve el codigo (true si se resuelve / false si hay problemas)
            codigoBueno = resolverCodigo(operaciones);
        }
        //se activan los botones correspondientes y se prepara la pregunta
        prepararBotonesYPregunta();

#if DEBUG
        imprimeValoresVariables();
#endif
    }

    #endregion


    private void decidirCodigo()
    {
        //decide cuantas lineas (APROXIMADAMENTE EN EL CASO DE LOS BUCLES) habra
        maxLineas = (dificultad + 1) * 3;
        maxLineas = 3;// (int)Mathf.Max(Mathf.Ceil(maxLineas + UnityEngine.Random.Range(dificultad, maxLineas - 10)), 7);

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
                operadoresAdmitidos = 4;
                numeroDeVariables = 2;
                operadoresReducidos = true;
                break;

            case MEDIO:
                //generamos condiciones
                operadoresAdmitidos = 5;
                numeroDeVariables = 1;
                operadoresReducidos = true;
                condiciones = UnityEngine.Random.Range(1, 2);
                break;

            case MEDIO_DIFICIL:
                operadoresAdmitidos = 5;
                numeroDeVariables = 2;
                operadoresReducidos = true;
                condiciones = 1;//UnityEngine.Random.Range(0, 2);
                buclesWhile = UnityEngine.Random.Range(1, 1);
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

        //la lista deoperaciones disponibles se actualiza
        if (buclesFor <= 0)
            tiposLineasRestantes.Remove(BUCLE_FOR);
        if (buclesWhile <= 0)
            tiposLineasRestantes.Remove(BUCLE_WHILE);
        if (condiciones <= 0)
            tiposLineasRestantes.Remove(LINEA_CONDICION);

        for (int iterador = 0; iterador < maxLineas; iterador++)
        {
            //print("generandoLinea" + maximo);
            seleccion = UnityEngine.Random.Range(0, tiposLineasRestantes.Count);
            //print(seleccion);
            switch (tiposLineasRestantes[seleccion])
            {
                case LINEA_NORMAL:
                    generarLineaNormal();
                    break;

                case LINEA_CONDICION:
                    generarCondicion();
                    condiciones--;
                    if (condiciones <= 0)
                        tiposLineasRestantes.Remove(LINEA_CONDICION);
                    break;

                case BUCLE_WHILE:
                    generarBucleWhile();
                    buclesWhile--;
                    if (buclesWhile <= 0)
                        tiposLineasRestantes.Remove(BUCLE_WHILE);
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

    private void inicializarVariablesRelacionadasConCodigo()
    {
        identificadoresVariables.Clear();
        valoresVariables.Clear();
        soporteCodigo.text = "";
        condiciones = 0;
        buclesWhile = 0;
        buclesFor = 0;
        //se inicializa la lista
        tiposLineasRestantes = new List<int>(new int[] { 0, 1, 2, 3});
    }

    private bool resolverCodigo(List<operacion> operaciones)
    {
        try
        {
            foreach (operacion op in operaciones)
            {
                //bucles
                if(op.GetBucle())
                {
                    print("Hay bucle");
                    //evaluamos condicion de entrada
                    bool ejecucion = false;

                    switch (op.GetOperador())
                    {
                        case IGUAL:
                            if (valoresVariables[op.GetVariable()] == op.GetValor())
                                ejecucion = true;
                            break;
                        case MAYOR:
                            if (valoresVariables[op.GetVariable()] > op.GetValor())
                                ejecucion = true;
                            break;
                        case MAYOR_IGUAL:
                            if (valoresVariables[op.GetVariable()] >= op.GetValor())
                                ejecucion = true;
                            break;
                        case MENOR:
                            if (valoresVariables[op.GetVariable()] < op.GetValor())
                                ejecucion = true;
                            break;
                        case MENOR_IGUAL:
                            if (valoresVariables[op.GetVariable()] <= op.GetValor())
                                ejecucion = true;
                            break;
                        default:
                            print("Operador extrano en bucle");
                            break;

                    }

                    while(ejecucion)
                    {
                        resolverCodigo(op.GetListaInstrucciones());

                            //volvemos a comprobar la condicion
                        ejecucion = false;
                        switch (op.GetOperador())
                        {
                            case IGUAL:
                                if (valoresVariables[op.GetVariable()] == op.GetValor())
                                    ejecucion = true;
                                break;
                            case MAYOR:
                                if (valoresVariables[op.GetVariable()] > op.GetValor())
                                    ejecucion = true;
                                break;
                            case MAYOR_IGUAL:
                                if (valoresVariables[op.GetVariable()] >= op.GetValor())
                                    ejecucion = true;
                                break;
                            case MENOR:
                                if (valoresVariables[op.GetVariable()] < op.GetValor())
                                    ejecucion = true;
                                break;
                            case MENOR_IGUAL:
                                if (valoresVariables[op.GetVariable()] <= op.GetValor())
                                    ejecucion = true;
                                break;
                            default:
                                print("Operador extrano en bucle");
                                break;

                        }

                    }
                    continue;

                }

                //condiciones if else
                if (op.GetCondicional() || op.GetCondicionalConElse())
                {
                   // print(COMPARADORES[op.GetOperador()] + " " + valoresVariables[op.GetVariable()] + " " + op.GetValor() + " " + op.GetCondicionalConElse());
                    switch (COMPARADORES[op.GetOperador()])
                    {
                        case "==":
                            if (valoresVariables[op.GetVariable()] == op.GetValor())
                                resolverCodigo(op.GetListaInstrucciones());
                            else if (op.GetCondicionalConElse())
                                resolverCodigo(op.GetListaInstruccionesAlternatva());
                            break;

                        case "!=":
                            if (valoresVariables[op.GetVariable()] != op.GetValor())
                                resolverCodigo(op.GetListaInstrucciones());
                            else if (op.GetCondicionalConElse())
                                resolverCodigo(op.GetListaInstruccionesAlternatva());
                            break;

                        case ">=":
                            if (valoresVariables[op.GetVariable()] >= op.GetValor())
                                resolverCodigo(op.GetListaInstrucciones());
                            else if (op.GetCondicionalConElse())
                                resolverCodigo(op.GetListaInstruccionesAlternatva());
                            break;

                        case "<=":
                            if (valoresVariables[op.GetVariable()] == op.GetValor())
                                resolverCodigo(op.GetListaInstrucciones());
                            else if (op.GetCondicionalConElse())
                                resolverCodigo(op.GetListaInstruccionesAlternatva());
                            break;

                        case ">":
                            if (valoresVariables[op.GetVariable()] == op.GetValor())
                                resolverCodigo(op.GetListaInstrucciones());
                            else if (op.GetCondicionalConElse())
                                resolverCodigo(op.GetListaInstruccionesAlternatva());
                            break;

                        case "<":
                            if (valoresVariables[op.GetVariable()] == op.GetValor())
                                resolverCodigo(op.GetListaInstrucciones());
                            else if (op.GetCondicionalConElse())
                                resolverCodigo(op.GetListaInstruccionesAlternatva());
                            break;
                    }

                    continue;
                }
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
        int indiceVariable = UnityEngine.Random.Range(0, identificadoresVariables.Count);
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


    private operacion generarLineaNormal(bool anadir = true, int indiceVariableExcluida = -1, int variableParaUsar = -1, int operadorParaUsar = -1)
    {
        //la variable excluida es una variable que NO podra ser usada en las operaciones que se generen
        //si se proporciona variableParaUsar, la linea que se genere modificara dicha variable con el operador que se pase en operadorParaUsar

        //anadir decide si la operacion se anade a la lista general de operaciones y se devuelve o si solo se devuelve
        //print("Generando linea normal");
        //int operador = UnityEngine.Random.Range(1, operadoresAdmitidos);
        float auxRand = 0;
        bool operacionNoConmutativaConValorDelante = false;

        int indiceOperador, indiceVariable, valor;

        //se selecciona el operador
        if (operadorParaUsar != -1)
            indiceOperador = operadorParaUsar;
        else
            indiceOperador = UnityEngine.Random.Range(1, operadoresAdmitidos);

        //se selecciona la variable
        if (variableParaUsar != -1)
            indiceVariable = variableParaUsar;
        else
        {
            indiceVariable = UnityEngine.Random.Range(0, numeroDeVariables);

            if (indiceVariableExcluida != -1)
            {
                while (indiceVariable == indiceVariableExcluida)
                    indiceVariable = UnityEngine.Random.Range(0, numeroDeVariables);
            }
        }

        valor = UnityEngine.Random.Range(1, 5);


        //print("operador, variable, valor / " + indiceOperador + indiceVariable + valor); 

        // si se permiten operadores reducidos, se decide si esta linea tendra uno
        if (operadoresReducidos)
        {
            auxRand = UnityEngine.Random.value;
        }
        else
        {
            auxRand = 1;
        }


        //si lo tiene se genera
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
                soporteCodigo.text += operacionSimpleSinOperadoresContraidosPrimeroValor(identificadoresVariables[indiceVariable],
                OPERADORES[indiceOperador],
                valor);
            }
            else
            {
                soporteCodigo.text += operacionSimpleSinOperadoresContraidosPrimeroVariable(identificadoresVariables[indiceVariable],
                OPERADORES[indiceOperador],
                valor);
            }
        }

        //se anade la operacion a la lista
        operacion aux = new operacion(indiceOperador, indiceVariable, valor, operacionNoConmutativaConValorDelante);
        if(anadir)
            operaciones.Add(aux);
        return aux;
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


    private void generarCondicion()
    {
        //primero se decide si habra o no else
        bool habraElse = true;
        if (UnityEngine.Random.value > 0.5f)
            habraElse = true;

        //se crea la lista auxiliar que contendra las operaciones a realizar si la operacion se cumple (o si no lo hace si hay un else);
        List<operacion> instruccionesIf = new List<operacion>();

        //se decide la condicion (igual, distinto, mayor, igual)
        int condicion = UnityEngine.Random.Range(0, COMPARADORES.Length - 1);
        //se decide cuantas lineas habra dentro
        int numeroLineas = UnityEngine.Random.Range(1, 3);
        //se decide que variable va a compararse y con que valor
        int indiceVariable = UnityEngine.Random.Range(0, numeroDeVariables);
        int valor = UnityEngine.Random.Range(1, 10);

        //se genera la linea de condicion
        soporteCodigo.text += sentenciaIf(identificadoresVariables[indiceVariable], COMPARADORES[condicion], valor);

        for (int i = 0; i < numeroLineas; i++)
        {
            instruccionesIf.Add(generarLineaNormal(false));
        }

        //cerramos bloque condicion
        soporteCodigo.text += cierreSentenciaCondicional();

        if (!habraElse)
        {
            //anadimos y terminamos
            operaciones.Add(new operacion(condicion, indiceVariable, valor, false, true, habraElse, instruccionesIf));
            return;
        }
        else
        {
            List<operacion> listaElse = new List<operacion>();
            numeroLineas = UnityEngine.Random.Range(1, 3);
            soporteCodigo.text += sentenciaElse();

            for (int i = 0; i < numeroLineas; i++)
            {
                listaElse.Add(generarLineaNormal(false));
            }

            soporteCodigo.text += cierreSentenciaCondicional();

            operaciones.Add(new operacion(condicion, indiceVariable, valor, false, true, habraElse, instruccionesIf, listaElse));
        }
    }

    private void generarBucleWhile()
    {
        List<operacion> operacionesBucle = new List<operacion>();

        //se elige la variable que servira para controlar la ejecucion del bucle
        //para evitar problemas, esa variable se excluira de las operaciones de dentro del bucle
        //a excepcion de una linea al final que modificara su valor para salir eventualmente del bucle
        int indiceVariableCondicion = UnityEngine.Random.Range(0, identificadoresVariables.Count);
        //se decide la condicion (igual, distinto, mayor, igual)
        int condicion = UnityEngine.Random.Range(0, COMPARADORES.Length - 1);
        //para reducir complejidad, eliminamos distinto
        if(condicion == DISTINTO)
            condicion++;
        //se decide cuantas lineas habra dentro
        int numeroLineas = UnityEngine.Random.Range(1, 3);
        //se decide que valor va a compararse
        int valor = UnityEngine.Random.Range(1, 10);

        soporteCodigo.text += bucleWhile(identificadoresVariables[indiceVariableCondicion], COMPARADORES[condicion], valor);

        for (int i = 0; i < numeroLineas; i++)
        {
            operacionesBucle.Add(generarLineaNormal(false, indiceVariableCondicion));
        }

        //se anade la operacion que modificara la variable que controla la ejecucion ddel bucle

        //si la comparacion es 'MIENTRAS SEA MAYOR', la variable se decrementa
        if (condicion == MAYOR || condicion == MAYOR_IGUAL)
        {
            operacionesBucle.Add(generarLineaNormal(false, -1, indiceVariableCondicion, RESTA));
        }
        //si la comparacion es 'MIENTRAS SEA MENOR', la variable se incrementa
        else if(condicion == MENOR || condicion == MENOR_IGUAL)
        {
            operacionesBucle.Add(generarLineaNormal(false, -1, indiceVariableCondicion, SUMA));
        }
        //si la comparacion es "mientras sea igual", no importa
        else
        {
            operacionesBucle.Add(generarLineaNormal(false, -1, indiceVariableCondicion));
        }

        //anadimos el bucle
        //condicion sera el comparador, variable la variable a comparar y valor la variable con la que se comparara
        operaciones.Add(new operacion(condicion, indiceVariableCondicion, valor, false, true, operacionesBucle));
        soporteCodigo.text += cierreSentenciaCondicional();
    }

    #endregion

    #region GENERADORES LINEAS TEXTO

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

    private string sentenciaIf(char var, string operador, int valor)
    {
        string aux = tabular() + "SI (" + var + " " + operador + " " + valor + ") \n { \n";
        tabuladores++;
        return aux;

    }

    private string sentenciaElse()
    {
        string aux =  tabular() + "SI NO \n { \n";
        tabuladores++;
        return aux;
    }

    private string cierreSentenciaCondicional()
    {
        tabuladores--;
        return tabular() + "} \n";
    }

    private string bucleWhile(char var, string operador, int valor)
    {
        string aux = tabular() + "MIENTRAS (" + var + " " + operador + " " + valor + ") \n { \n";
        tabuladores++;
        return aux;
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

    #region DEBUG_FUNCTIONS

    private void imprimeValoresVariables()
    {
        for (int i = 0; i < identificadoresVariables.Count; i++)
        {
            print("Variable " + identificadoresVariables[i] + " vale " + valoresVariables[i]);
        }

    }

    #endregion
}
