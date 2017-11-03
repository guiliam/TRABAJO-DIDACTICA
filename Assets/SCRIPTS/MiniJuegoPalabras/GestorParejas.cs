using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestorParejas : MonoBehaviour {

    public float xInicial;
    public float yInicialVerbos, yInicialNombres;
    public float incrementoX, incrementoY;
    public int numeroPanelesPorFila;


    SecuenciasAcciones secuencias;
    Secuencia secuenciaActiva;
    int indiceSecuenciaActiva;

    //paneles base
    GameObject soporteNombre;
    GameObject soporteVerbo;

    GameObject canvas;

    //paneles activos
    List<GameObject> panelesNombresActivos;
    List<GameObject> panelesVerbosActivos;

	// Use this for initialization
	void Awake () {
        secuencias = SecuenciasAcciones.Getinstance();

        //soportes palabras
        soporteNombre = GameObject.Find("SoporteNombre");
        soporteNombre.SetActive(false);
        soporteVerbo = GameObject.Find("SoporteVerbo");
        soporteVerbo.SetActive(false);

        //canvas, para que sea el padre de los soportes de palbras
        canvas = GameObject.Find("Canvas");

        indiceSecuenciaActiva = 0;
        secuenciaActiva = secuencias.secuencias[indiceSecuenciaActiva];

        panelesNombresActivos = new List<GameObject>();
        panelesVerbosActivos = new List<GameObject>();

        cargarSecuencia();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void cargarSecuencia()
    {
        GameObject aux;

        //generar paneles para los nombres
        for(int i = 0; i < secuenciaActiva.nombres.Length; i++)
        {
            //si el soporte ya existe, se actualiza el texto
            if (i < panelesNombresActivos.Count)
            {
                panelesNombresActivos[i].GetComponentInChildren<Text>().text = secuenciaActiva.nombres[i];
                panelesNombresActivos[i].SetActive(true);
            }

            //si no existe, se crea uno nuevo 
            else
            {
                aux = GameObject.Instantiate(soporteNombre, canvas.transform);
                aux.GetComponentInChildren<Text>().text = secuenciaActiva.nombres[i];
                aux.SetActive(true);
                panelesNombresActivos.Add(aux);
            }

            panelesNombresActivos[i].transform.localPosition = new Vector3(xInicial + (i % numeroPanelesPorFila * incrementoX),
                                                       yInicialNombres - (incrementoY * (i / numeroPanelesPorFila)), -2f);
        }

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
            }

            //si no existe, se crea uno nuevo 
            else
            {
                aux = GameObject.Instantiate(soporteVerbo, canvas.transform);
                aux.GetComponentInChildren<Text>().text = secuenciaActiva.verbos[i];
                aux.SetActive(true);
                panelesVerbosActivos.Add(aux);
            }

            panelesVerbosActivos[i].transform.localPosition = new Vector3(xInicial + (i % numeroPanelesPorFila * incrementoX),
                                                        yInicialVerbos - (incrementoY * (i / numeroPanelesPorFila)), -2f);
                                                                   
        }

        //si han sobrado paneles, se desactivan
        for (int i = panelesVerbosActivos.Count - 1; i >= secuenciaActiva.verbos.Length; i--)
        {
            panelesVerbosActivos[i].SetActive(false);
        }

    }
}
