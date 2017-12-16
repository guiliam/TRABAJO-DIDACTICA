using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoPalabra : MonoBehaviour {

    private const float DISTANCE_TO_CAMERA = 7.5f;
    private const float DISTANCE_STICK = 13f;
    private const float OFFSET_POSICION = 20;
    private const float VELOCIDAD_VUELTA = 50f;
    private const float MARGEN_POSICION_INICIAL = 0.1f;

    public bool palabra;

    GestorParejas gestor;
    bool cogido;
    GameObject candidato; //objeto con el que se intentara unir
    Vector3 posInicial;


	// Use this for initialization
	void Awake () {
        gestor = GameObject.Find("Gestor").GetComponent<GestorParejas>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //si esta cogido
        if (cogido)
        {
            //movemos con el raton
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 40));

            //si esta cerca de un punto de union, se pegara a el
            candidato = null;
            float distanciaCandidato = float.PositiveInfinity, aux;
            List<GameObject> lista;
            if (palabra)
                lista = gestor.GetVerbos();
            else
                lista = gestor.GetNombres();

            //recorremos posibles puntos y buscamos el mas cercano
            foreach (GameObject obj in lista)
            {
                if (lista[lista.IndexOf(obj)].active)
                {
                    aux = Vector3.Distance(obj.transform.position, transform.position);
                    //si esta lo bastante cerca, se pega
                    if (aux <= DISTANCE_STICK)
                    {
                        print("Cerca");
                        //si ya habia un candidato, se coge el mas ceracno
                        if (aux < distanciaCandidato)
                        {
                            candidato = obj;
                            distanciaCandidato = aux;
                            //activamos highlight
                            obj.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        else //desactivamos highlight
                            obj.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    else//desactivamos highlight
                    {
                        obj.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }

            if (candidato != null)
            {
                if (palabra) //las palabras van a la ->
                {
                    transform.position = new Vector3(candidato.transform.position.x + OFFSET_POSICION, transform.position.y, candidato.transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(candidato.transform.position.x - OFFSET_POSICION, transform.position.y, candidato.transform.position.z);
                }
            }
        }

        //si no esta cogido, vuelve a su posicion
        else
        {
            if (Vector3.Distance(this.transform.position, posInicial) > MARGEN_POSICION_INICIAL)
            {
                Vector3 direccion = Vector3.Normalize(posInicial - transform.position);
                transform.position += direccion * VELOCIDAD_VUELTA * Time.fixedDeltaTime * (Vector3.Distance(this.transform.position, posInicial) / VELOCIDAD_VUELTA);

            }

        }
	}


    public void OnClick()
    {
        //si no hemos cogido este y nno hay ninguno cogido
        if (!cogido && !gestor.GetPanelCogido())
        {
            cogido = true;
            this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            gestor.SetPanelCogido(true, this.gameObject);
        }

            //si esta cogido
        else if(cogido && gestor.GetPanelCogido())
        {
            cogido = false;
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gestor.SetPanelCogido(false, null);
            /*if (palabra)
            {
                if (gestor.ComprobarPareja(this.gameObject, candidato))
                {
                    GameObject.Destroy(candidato);
                    GameObject.Destroy(this.gameObject);
                }
            }
            else
            {
                if (gestor.ComprobarPareja(candidato,this.gameObject))
                {
                    GameObject.Destroy(candidato);
                    GameObject.Destroy(this.gameObject);
                }

            }*/
        }

        else if (!cogido && gestor.GetPanelCogido())
        {
            if (gestor.ComprobarPareja(this.gameObject))
                return;
        }

    }

    public void Liberar()
    {
        cogido = false;
    }

    public void SetPosInicial(Vector3 pos)
    {
        posInicial = pos;
    }
}
