using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Esta clase se encarga de mostrar los tutoriales en pantalla y esperar a que se pulse espacio para que desaparezcan
/// </summary>
public class TutorialesCodigo : MonoBehaviour {

    public const int TUTORIAL_ASIGNACIONES = 0;
    public const int TUTORIAL_OPERADORES = 1;
    public const int TUTORIAL_CONDICIONALES = 2;
    public const int TUTORIAL_BUCLES = 3;


    private const float VELOCIDAD = 500f;



    public Sprite tutoAsignaciones, tutoOperadores, tutoCondicionales, tutoBucles;

    //para que solo se muestren una vez
    private bool asign, oper, condi, bucle;

    private Vector3 posInicial, posEscondido;
    public Image imagenTutoriales;
    public GameObject soporteTutoriales;

    //indican el estado de la accion
    private bool mostrandose, espacioPulsado;

	// Use this for initialization
	void Awake () {
        imagenTutoriales = GameObject.Find("ImagenSoporteTutoriales").GetComponent<Image>();
        soporteTutoriales = GameObject.Find("SoporteTutoriales");
        posInicial = soporteTutoriales.transform.position;
        posEscondido = new Vector3(posInicial.x, posInicial.y -3000, posInicial.z);
	}

    void Update()
    {
        if (mostrandose)
        {
            if (!espacioPulsado)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    espacioPulsado = true;
            }
        }
    }

    public void MostrarTutorial(int tuto)
    {
        switch (tuto)
        {
            case TUTORIAL_ASIGNACIONES:
                if (asign)
                    return;
                imagenTutoriales.sprite = tutoAsignaciones;
                asign = true;
                break;

            case TUTORIAL_BUCLES:
                if (bucle)
                    return;
                imagenTutoriales.sprite = tutoBucles;
                bucle = true;
                break;

            case TUTORIAL_CONDICIONALES:
                if (condi)
                    return;
                imagenTutoriales.sprite = tutoCondicionales;
                condi = true;
                break;

            case TUTORIAL_OPERADORES:
                if (oper)
                    return;
                imagenTutoriales.sprite = tutoOperadores;
                oper = true;
                break;
        }

        mostrandose = true;
        espacioPulsado = false;
        StopCoroutine("MostrarYEsconder");
        StartCoroutine("MostrarYEsconder");

    }


    IEnumerator MostrarYEsconder()
    {
        Vector3 posActual = soporteTutoriales.transform.localPosition;


        //se suben
        while (soporteTutoriales.transform.localPosition.y > posInicial.y)
        {
            posActual.y -= VELOCIDAD * Time.deltaTime;
            soporteTutoriales.transform.localPosition = posActual;
            yield return null;
        }
        //se colocan en la posicion definitiva
        posActual = posInicial;
        soporteTutoriales.transform.localPosition = posInicial;

        //esperamos a que se pulse espacio
        while(!espacioPulsado)
        {
            yield return null;
        }

        //escondemos cartel
        while (soporteTutoriales.transform.localPosition.y > posEscondido.y)
        {
            posActual.y -= VELOCIDAD * Time.deltaTime;
            soporteTutoriales.transform.localPosition = posActual;
            yield return null;
        }
        //se colocan en la posicion definitiva
        soporteTutoriales.transform.localPosition = posEscondido;

        mostrandose = false;
    }


    public bool HayTutorialActivo()
    {
        return mostrandose;
    }
}
