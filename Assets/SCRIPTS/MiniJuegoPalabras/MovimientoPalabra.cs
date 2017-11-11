using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoPalabra : MonoBehaviour {

    private const float DISTANCE_TO_CAMERA = 7.5f;
    private const float DISTANCE_STICK = 5f;
    private const float OFFSET_POSICION = 20;

    public bool palabra;

    GestorParejas gestor;
    bool cogido;
    GameObject candidato; //objeto con el que se intentara unir


	// Use this for initialization
	void Awake () {
        gestor = GameObject.Find("Gestor").GetComponent<GestorParejas>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (cogido)
        {
            //movemos con el raton
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, DISTANCE_TO_CAMERA));

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
                aux = Vector3.Distance(obj.transform.transform.position, transform.position);
                //si esta lo bastante cerca, se pega
                if ( aux <= DISTANCE_STICK)
                {
                    //si ya habia un candidato, se coge el mas ceracno
                    if (aux < distanciaCandidato)
                    {
                        candidato = obj;
                        distanciaCandidato = aux;
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
	}


    public void OnClick()
    {
        //si no hemos cogido este y nno hay ninguno cogido
        if (!cogido && !gestor.GetPanelCogido())
        {
            cogido = true;
            gestor.SetPanelCogido(true, this.gameObject);
        }

            //si esta cogido
        else if(cogido && gestor.GetPanelCogido())
        {
            cogido = false;
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
}
