﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GestorParejas : MonoBehaviour {

    private const float INCREMENTO_VERTICAL_TEXTO_PAREJAS_RESUELTAS = 7F;
    private const float POSICION_Y_DESAPARICION_IMAGEN_TUTORIAL = -1055f;
    private const float VELOCIDAD_MOVIMIENTO_IMAGEN_TUTO = 400f;
    private const float TIEMPO_PARA_PODER_PASAR_TUTORIAL = 4F;

    public float xInicial;
    public float yInicialVerbos, yInicialNombres;
    public float incrementoX, incrementoY;
    public int numeroPanelesPorFila;
	public Text TextMision, TextRey, TextContador, TextPistas;
    public GameObject imagenTuto;

    bool panelCogido; //inica si hay un panel cogido
    SecuenciasAcciones secuencias;
    Secuencia secuenciaActiva;
    int indiceSecuenciaActiva; //indice de la secuencia activa
    int parejaActual; //indica por que pareja de la secuancia vamos
	int misionActual;
	int fraseReyActual;

    //paneles base
    GameObject soporteNombre;
    GameObject soporteVerbo;

    GameObject canvas;
    GameObject canvasSecuencia;

    //paneles activos
    List<GameObject> panelesNombresActivos;
    List<GameObject> panelesVerbosActivos;

    //panel que se esta llevando
    GameObject carrying;

    //texto base para mostrar parejas ya hechas
    GameObject textoSecuenciaBase;
	public GameObject CanvasNivelCompletado, CanvasFinal;
	float timeAux, tiempoInicioEscena;
	bool completado, finDelJuego, tutoMostrandose;

	void Update(){

        //si el tutorial se esta mostrando, lo apartamos
        if (tutoMostrandose && Input.GetKeyDown(KeyCode.Space) && Time.time - tiempoInicioEscena > TIEMPO_PARA_PODER_PASAR_TUTORIAL)
        {
            StartCoroutine("DesaparicionImagenTutorial");
            tutoMostrandose = false;
        }

		if (indiceSecuenciaActiva >= secuencias.Secuencias.Length) {
			CanvasNivelCompletado.SetActive (true);
		}
		if (completado) {
			timeAux += Time.deltaTime;
		}
		if(timeAux > 3 && !finDelJuego){
			timeAux = 0;
			cargarSecuencia ();
			completado = false;
		}
		if (finDelJuego) {
			CanvasFinal.SetActive (true);
			timeAux += Time.deltaTime;
			if (timeAux >= 8)
				Application.LoadLevel ("GeneradorCodigoAleatorio");
		}
	}

    // Use this for initialization
    void Awake () {
        tiempoInicioEscena = Time.time;
        secuencias = SecuenciasAcciones.Getinstance();

        //soportes palabras
        soporteNombre = GameObject.Find("SoporteNombre");
        soporteNombre.SetActive(false);
        soporteVerbo = GameObject.Find("SoporteVerbo");
        soporteVerbo.SetActive(false);

        //canvas, para que sea el padre de los soportes de palbras
        canvas = GameObject.Find("CanvasSoportes");

        //canvas parejas ya hechas
        canvasSecuencia = GameObject.Find("CanvasSecuencia");

        //texto de parejas ya hechas
        textoSecuenciaBase = GameObject.Find("TextoBaseSecuencia");
        textoSecuenciaBase.SetActive(false);

        indiceSecuenciaActiva = 0;
        secuenciaActiva = secuencias.Secuencias[indiceSecuenciaActiva];

        panelesNombresActivos = new List<GameObject>();
        panelesVerbosActivos = new List<GameObject>();

        cargarSecuencia();

        tutoMostrandose = true;
	}

    public bool ComprobarPareja(GameObject obj1)
    {
        print("ComprobarPareja");
        print(carrying.transform.GetChild(0).GetComponent<Text>().text + obj1.transform.GetChild(0).GetComponent<Text>().text);
        if (carrying.GetComponent<MovimientoPalabra>().palabra)
        {
            if (carrying.transform.GetChild(0).GetComponent<Text>().text == secuenciaActiva.nombres[parejaActual] && obj1.transform.GetChild(0).GetComponent<Text>().text == secuenciaActiva.verbos[parejaActual])
            {
                //si la pareja es correcta
                //se genera texto para la pareja resuelta
                generarTextoParejaResuelta();

                //desactivamos
                panelesNombresActivos[panelesNombresActivos.IndexOf(carrying)].SetActive(false);
                panelesVerbosActivos[panelesVerbosActivos.IndexOf(obj1)].SetActive(false);
                /*GameObject.Destroy(carrying);
                GameObject.Destroy(obj1);*/
                panelCogido = false;
                carrying = null;
                parejaActual++;
                //actualizamos pista
                if(parejaActual < secuenciaActiva.pistas.Length)
                    TextPistas.text = secuenciaActiva.pistas[parejaActual];
                comprobarVictoria();
                return true;
            }
        }
        else
        {

            if (obj1.transform.GetChild(0).GetComponent<Text>().text == secuenciaActiva.nombres[parejaActual] && carrying.transform.GetChild(0).GetComponent<Text>().text == secuenciaActiva.verbos[parejaActual])
            {
                //si la pareja es correcta

                //se genera texto para la pareja resuelta
                generarTextoParejaResuelta();

                //desactivamos
                panelesNombresActivos[panelesNombresActivos.IndexOf(obj1)].SetActive(false);
                panelesVerbosActivos[panelesVerbosActivos.IndexOf(carrying)].SetActive(false);

                panelCogido = false;
                carrying = null;
                parejaActual++;
                //actualizamos pista
                if (parejaActual < secuenciaActiva.pistas.Length)
                    TextPistas.text = secuenciaActiva.pistas[parejaActual];
                comprobarVictoria();
                return true;
            }

        }

        return false;
        //comprueba si la pareja introducida es la que se esperaba
        /*if (nombre.GetComponent<Text>().text == secuenciaActiva.nombres[indiceSecuenciaActiva] && verbo.GetComponent<Text>().text == secuenciaActiva.verbos[indiceSecuenciaActiva])
        {
            //si la pareja es correcta
            indiceSecuenciaActiva++;
            return true;
        }*/
    }

    void cargarSecuencia()
    {
		completado = false;
		CanvasNivelCompletado.SetActive (false);
        GameObject aux;
        List<int> listaPosAux = new List<int>(); //para mezclar los paneles
        int ultimoIndiceNombres;

        //reiniciamos la pareja actual
        parejaActual = 0;

        //generar paneles para los nombres
        for (int i = 0; i < secuenciaActiva.nombres.Length; i++)
        {
            //si el soporte ya existe, se actualiza el texto
            if (i < panelesNombresActivos.Count)
            {
                panelesNombresActivos[i].GetComponentInChildren<Text>().text = secuenciaActiva.nombres[i];
                panelesNombresActivos[i].SetActive(true);
                panelesNombresActivos[i].GetComponent<MovimientoPalabra>().Liberar();
                panelesNombresActivos[i].transform.GetChild(1).gameObject.SetActive(false);
            }

            //si no existe, se crea uno nuevo 
            else
            {
                aux = GameObject.Instantiate(soporteNombre, canvas.transform);
                aux.GetComponentInChildren<Text>().text = secuenciaActiva.nombres[i];
                aux.SetActive(true);
                panelesNombresActivos.Add(aux);
            }

            listaPosAux.Add(i);
           /* panelesNombresActivos[i].transform.localPosition = new Vector3(xInicial + (i % numeroPanelesPorFila * incrementoX),
                                                       yInicialNombres - (incrementoY * (i / numeroPanelesPorFila)), -2f);*/
        }

        ultimoIndiceNombres = listaPosAux[listaPosAux.Count - 1];

        //si han sobrado paneles, se desactivan
        for (int i = panelesNombresActivos.Count - 1; i >= secuenciaActiva.nombres.Length; i--)
        {
            panelesNombresActivos[i].SetActive(false);
        }


        //hacemos lo mismo con los verbos
        for (int i = 0; i < secuenciaActiva.verbos.Length; i++)
        {
            //si el soporte ya existe, se actualiza el texto
            if (i < panelesVerbosActivos.Count)
            {
                panelesVerbosActivos[i].GetComponentInChildren<Text>().text = secuenciaActiva.verbos[i];
                panelesVerbosActivos[i].SetActive(true);
                panelesVerbosActivos[i].GetComponent<MovimientoPalabra>().Liberar();
                panelesVerbosActivos[i].transform.GetChild(1).gameObject.SetActive(false);
            }

            //si no existe, se crea uno nuevo 
            else
            {
                aux = GameObject.Instantiate(soporteVerbo, canvas.transform);
                aux.GetComponentInChildren<Text>().text = secuenciaActiva.verbos[i];
                aux.SetActive(true);
                panelesVerbosActivos.Add(aux);
            }

            /*panelesVerbosActivos[i].transform.localPosition = new Vector3(xInicial + (i % numeroPanelesPorFila * incrementoX),
                                                        yInicialVerbos - (incrementoY * (i / numeroPanelesPorFila)), -2f);*/
            listaPosAux.Add(i + ultimoIndiceNombres + 1);
        }

        //si han sobrado paneles, se desactivan
        for (int i = panelesVerbosActivos.Count - 1; i >= secuenciaActiva.verbos.Length; i--)
        {
            panelesVerbosActivos[i].SetActive(false);
        }

        int posicionExtraida;
        //colocamos paneles aleatoriamente
        for (int i = 0; i < secuenciaActiva.nombres.Length; i++)
        {
            posicionExtraida = listaPosAux[UnityEngine.Random.Range(0, listaPosAux.Count - 1)];
            listaPosAux.Remove(posicionExtraida);
            panelesNombresActivos[i].transform.localPosition = new Vector3(xInicial + (posicionExtraida % numeroPanelesPorFila * incrementoX),
                                                       yInicialNombres - (incrementoY/2 * (posicionExtraida / numeroPanelesPorFila)), -2f);
            panelesNombresActivos[i].GetComponent<MovimientoPalabra>().SetPosInicial(panelesNombresActivos[i].transform.position);
        }

        for (int i = 0; i < secuenciaActiva.verbos.Length; i++)
        {
			posicionExtraida = listaPosAux[UnityEngine.Random.Range(0, listaPosAux.Count - 1)];
            listaPosAux.Remove(posicionExtraida);
            panelesVerbosActivos[i].transform.localPosition = new Vector3(xInicial + (posicionExtraida % numeroPanelesPorFila * incrementoX),
				yInicialNombres - (incrementoY/2 * (posicionExtraida / numeroPanelesPorFila)), -2f);
            panelesVerbosActivos[i].GetComponent<MovimientoPalabra>().SetPosInicial(panelesVerbosActivos[i].transform.position);
        }

        TextMision.text = secuenciaActiva.Descripcion;
        TextRey.text = secuenciaActiva.Ayuda;
        TextContador.text = "ACCIONES COMPLETADAS\n " + (misionActual + 1) + "/" + secuencias.Secuencias.Length;

        //actualizamos pista
        TextPistas.text = secuenciaActiva.pistas[0];
    }


    private void generarTextoParejaResuelta()
    {
        GameObject nuevoTexto = GameObject.Instantiate(textoSecuenciaBase);
        nuevoTexto.SetActive(true);
        nuevoTexto.transform.SetParent(canvasSecuencia.transform);
		nuevoTexto.GetComponent<Text>().text = secuenciaActiva.verbos[parejaActual] + " " + secuenciaActiva.nombres[parejaActual];
        nuevoTexto.transform.localPosition = new Vector3(textoSecuenciaBase.transform.localPosition.x, textoSecuenciaBase.transform.localPosition.y - INCREMENTO_VERTICAL_TEXTO_PAREJAS_RESUELTAS * parejaActual,
            textoSecuenciaBase.transform.localPosition.z);
        nuevoTexto.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }


    private void comprobarVictoria()
    {		
        if (parejaActual >= secuenciaActiva.parejas + 1)
        {
			completado = true;
            print("NIVEL COMPLETADO");
			CanvasNivelCompletado.SetActive (true);
            ////CODIGO VICTORIA AQUI
            //carrying.GetComponent<MovimientoPalabra>().Liberar();
            indiceSecuenciaActiva++;
            if (indiceSecuenciaActiva >= secuencias.Secuencias.Length)
            {                
                //Application.LoadLevel("GeneradorCodigoAleatorio");
				finDelJuego = true;
                return;
            }
            secuenciaActiva = secuencias.Secuencias[indiceSecuenciaActiva];
            //cargarSecuencia();
			//cambiar dialogo y mision
			misionActual++;
			fraseReyActual++;            
            //borrar parejas hechas
            for (int i = canvasSecuencia.transform.GetChildCount() - 1; i > 0; i--)
            {
                GameObject.Destroy(canvasSecuencia.transform.GetChild(i).gameObject);
            }
        }

    }

    public List<GameObject> GetNombres()
    {
        return panelesNombresActivos;
    }


    public List<GameObject> GetVerbos()
    {
        return panelesVerbosActivos;
    }

    public void SetPanelCogido(bool valor, GameObject obj)
    {
        if (valor && panelCogido)
        {
            print("Error en set panel cogido: se ha intentado coger cuando ya habia un panel cogido");
        }

        else
        {
            panelCogido = valor;
            carrying = obj;
        }
    }

    public bool GetPanelCogido()
    {
        return panelCogido;
    }

    public GameObject GetCarriying()
    {
        return carrying;
    }


    IEnumerator DesaparicionImagenTutorial()
    {
        Vector3 aux;

        while (imagenTuto.transform.localPosition.y > POSICION_Y_DESAPARICION_IMAGEN_TUTORIAL)
        {
            aux = imagenTuto.transform.localPosition;
            aux.y -= Time.deltaTime * VELOCIDAD_MOVIMIENTO_IMAGEN_TUTO;
            imagenTuto.transform.localPosition = aux;
            yield return null;
        }

        imagenTuto.SetActive(false);

    }
}
