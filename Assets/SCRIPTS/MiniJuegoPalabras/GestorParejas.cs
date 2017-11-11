using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestorParejas : MonoBehaviour {

    public float xInicial;
    public float yInicialVerbos, yInicialNombres;
    public float incrementoX, incrementoY;
    public int numeroPanelesPorFila;

    bool panelCogido; //inica si hay un panel cogido
    SecuenciasAcciones secuencias;
    Secuencia secuenciaActiva;
    int indiceSecuenciaActiva; //indica por que pareja de la secuancia vamos

    //paneles base
    GameObject soporteNombre;
    GameObject soporteVerbo;

    GameObject canvas;

    //paneles activos
    List<GameObject> panelesNombresActivos;
    List<GameObject> panelesVerbosActivos;

    //panel que se esta llevando
    GameObject carrying;

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

    public bool ComprobarPareja(GameObject obj1)
    {
        print(carrying.transform.GetChild(0).GetComponent<Text>().text + obj1.transform.GetChild(0).GetComponent<Text>().text);
        if (carrying.GetComponent<MovimientoPalabra>().palabra)
        {
            if (carrying.transform.GetChild(0).GetComponent<Text>().text == secuenciaActiva.nombres[indiceSecuenciaActiva] && obj1.transform.GetChild(0).GetComponent<Text>().text == secuenciaActiva.verbos[indiceSecuenciaActiva])
            {
                //si la pareja es correcta

                //borramos de la lista
                panelesNombresActivos.Remove(carrying);
                panelesVerbosActivos.Remove(obj1);
                GameObject.Destroy(carrying);
                GameObject.Destroy(obj1);
                panelCogido = false;
                carrying = null;
                indiceSecuenciaActiva++;
                return true;
            }
        }
        else
        {

            if (obj1.transform.GetChild(0).GetComponent<Text>().text == secuenciaActiva.nombres[indiceSecuenciaActiva] && carrying.transform.GetChild(0).GetComponent<Text>().text == secuenciaActiva.verbos[indiceSecuenciaActiva])
            {
                //si la pareja es correcta

                //borramos de las listas
                panelesNombresActivos.Remove(obj1);
                panelesVerbosActivos.Remove(carrying);

                GameObject.Destroy(carrying);
                GameObject.Destroy(obj1);
                panelCogido = false;
                carrying = null;
                indiceSecuenciaActiva++;
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
        GameObject aux;

        //generar paneles para los nombres
        for (int i = 0; i < secuenciaActiva.nombres.Length; i++)
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
}
