using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorBombillas : MonoBehaviour {

	const int NIVELES_HARDCODED = 4;


	public bool[] BombillasEncendidas;
	public GameObject PrefabBombilla, PrefabBoton;
	public GameObject[] HardcodedLvls;
	public float limiteDer, limiteIzq;


	private GameObject[] Bombillas, Botones;
	int actualLvl;

	void Start(){
		print (Mathf.Ceil (5 / 2 + 0.1f));
		actualLvl = 1;
		StartLvl ();
	}

	void StartLvl(){
		if (actualLvl <= 4) {
			BombillasEncendidas = new bool[GameObject.Find ("Bombillas" + actualLvl).GetComponentsInChildren<Light> ().Length];
			HardcodedLvls [actualLvl - 1].SetActive (true);
			if (actualLvl > 2)
				HardcodedLvls [actualLvl - 2].SetActive (false);
		} else {
			/*if (Random.value < 0.5)
				MetodoCristian();
			else
				MetodoJapend ();*/
		}
	}

	public void MetodoCristian(int numeroBombillas,int numeroBotones){
		for (int i = 0; i < numeroBombillas; i++) {
			Bombillas [i] = Instantiate (PrefabBombilla);
		}
		int numBombillasPrimerBoton = (int) Mathf.Round(numeroBombillas / 2);


	}

	public void MetodoJapend(int numeroBombillas,int numeroBotones){

		bool[] encendidas = new bool[numeroBombillas];
		for (int i = 0; i < numeroBombillas; i++)
			encendidas [i] = true;
		
		int[] bombillasAEnlazar;

		for (int i = 0; i < numeroBotones - 1; i++) {
			bombillasAEnlazar = elegirBombillasAleatorio (numeroBombillas);

			foreach (int num in bombillasAEnlazar)
				encendidas [num] = !encendidas [num];

			enlazarBombillasABoton (i, bombillasAEnlazar);
		}

		List<int> aux = new List<int>();
		for (int i = 0; i < encendidas.Length; i++) {
			if(encendidas[i])
				aux.Add(i);
		}

		bombillasAEnlazar = aux.ToArray();
		enlazarBombillasABoton(numeroBotones-1, bombillasAEnlazar);
	}

	void colocarBombillasYBotones()
	{
		float aumento = Mathf.Abs (limiteDer - limiteIzq) / Bombillas.Length;
		for (int i = 0; i < Bombillas.Length; i++) {
			Bombillas [i].transform.position = new Vector3 (limiteDer + aumento * i, Bombillas [i].transform.position.y, Bombillas [i].transform.position.z);
		}

		aumento = Mathf.Abs (limiteDer - limiteIzq) / Botones.Length;
		for (int i = 0; i < Botones.Length; i++) {
			Botones [i].transform.position = new Vector3 (limiteDer + aumento * i, Botones [i].transform.position.y, Botones [i].transform.position.z);
		}
	}

	//devuelve un array de indices aleatorios sin repetirse
	int[] elegirBombillasAleatorio(int longitud)
	{
		List<int> listaCandidatos;
		listaCandidatos = new List<int> ();

		for (int i = 0; i < longitud; i++) {
			listaCandidatos.Add (i);
		}

		int necesarios = Random.Range (1, longitud / 2);
		int seleccion;
		int[] resultado = new int[necesarios];
		while (necesarios > 0) {
			seleccion = Random.Range (0, listaCandidatos.Count - 1);
			resultado [necesarios - 1] = listaCandidatos [seleccion];
			listaCandidatos.Remove (seleccion);
			necesarios--;
		}

		return resultado;
	}

	void enlazarBombillasABoton(int numBoton, int[] bombillas)
	{
		//esta funcion modifica el script de los botones y les añade las bombillas que deben cambiar al ser pulsados
		BotonBombilla boton = Botones[numBoton].GetComponent<BotonBombilla>();
		boton.bombillas = new Renderer[bombillas.Length];

		int aux = 0;
		foreach (int num in bombillas) {
			boton.bombillas [aux] = Bombillas [num].gameObject.GetComponent<Renderer> ();
		}

		boton.Inicializar ();
	}
}
