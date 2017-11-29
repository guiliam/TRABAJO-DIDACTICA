using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestorBombillas : MonoBehaviour {	
	public GameObject[] Niveles;
	public string[] FrasesRey;
	public GameObject CanvasNivelCompletado, CanvasFinal;
	public Text TextRey, TextNivelesCompletados;
	public Light GeneralLight;

	Transform[] Bombillas;
	Transform[] Botones;
	Transform[] posicionesBombillas;
	Transform[] posicionesBotones;
	Vector3[] posicionesBombillasVector;
	Vector3[] posicionesBotonesVector;
	int nivelActual, bombillasActivas;
	bool completado, finDelJuego;
	float timeAux;

	void Awake(){
		inicializarSiguienteNivel ();
	}

	void Update(){
		if (bombillasActivas == Bombillas.Length) {
			completado = true;
			CanvasNivelCompletado.SetActive (true);
		}
		if (completado) {
			timeAux += Time.deltaTime;
		}
		if(timeAux > 3 && !finDelJuego){
			timeAux = 0;
			nivelActual++;
			if (nivelActual < Niveles.Length)
				inicializarSiguienteNivel ();
			else {
				finDelJuego = true;
			}
		}
		if (finDelJuego) {
			CanvasFinal.SetActive (true);
			timeAux += Time.deltaTime;
			if (timeAux >= 8)
				Application.LoadLevel ("UnirPalabras");
		}
	}

	Transform[] desordenar(Transform[] value)
	{
		Transform[] newValue = new Transform[value.Length];
		bool[] visited = new bool[value.Length];
		int actualPosition;

		for (int i = 0; i < value.Length; i++) {
			while (newValue [i] == null) {
				actualPosition = Random.Range (0, value.Length);
				if(!visited[actualPosition]){
					newValue[i] = value[actualPosition];
					visited[actualPosition] = true;
				}
			}
		}
		return newValue;
	}

	void inicializarSiguienteNivel(){
		GeneralLight.intensity = 0.1f * (nivelActual - 1);
		TextNivelesCompletados.text = "GENERADORES ACTIVADOS:\n" + nivelActual + "/10";
		TextRey.text = FrasesRey [nivelActual];
		CanvasNivelCompletado.SetActive (false);
		bombillasActivas = 0;
		completado = false;
		if (nivelActual > 0)
			Niveles [nivelActual - 1].SetActive (false);
		Niveles [nivelActual].SetActive (true);

		GameObject[] bombillasGameObjects = GameObject.FindGameObjectsWithTag ("bombilla");
		Bombillas = new Transform[bombillasGameObjects.Length];
		for (int i = 0; i < Bombillas.Length; i++) {
			Bombillas [i] = bombillasGameObjects [i].transform;
		}
		GameObject[] botonesGameObjects = GameObject.FindGameObjectsWithTag ("boton");
		Botones = new Transform[botonesGameObjects.Length];
		for (int i = 0; i < Botones.Length; i++) {
			Botones [i] = botonesGameObjects [i].transform;
		}
		posicionesBombillas = desordenar (Bombillas);
		posicionesBotones = desordenar (Botones);
		posicionesBombillasVector = new Vector3[posicionesBombillas.Length];
		posicionesBotonesVector = new Vector3[posicionesBotones.Length];
		getPositions (posicionesBombillas, posicionesBombillasVector);
		getPositions (posicionesBotones, posicionesBotonesVector);
		setPositions (Bombillas, posicionesBombillasVector);
		setPositions (Botones, posicionesBotonesVector);
	}

	void getPositions(Transform[] transforms, Vector3[] vectores){
		for (int i = 0; i < vectores.Length; i++) {
			vectores [i] = transforms [i].position;
		}
	}

	void setPositions(Transform[] array1, Vector3[] array2){
		for (int i = 0; i < array1.Length; i++) {
			array1 [i].position = array2 [i];
		}		
	}

	public void SumarBombilla(){
		bombillasActivas++;
	}

	public void ReiniciarBombillasEncendidas(){
		bombillasActivas = 0;
	}

	public void RestarBombilla(){
		bombillasActivas--;
	}

}



/*
	const int NIVELES_HARDCODED = 4;

	public GameObject PrefabBombilla, PrefabBoton;
	public GameObject[] HardcodedLvls;
	public float limiteDer, limiteIzq;


	List<int> numBombillasNiveles;
	private GameObject[] Bombillas, Botones;
	int actualLvl;

	void Awake(){
		actualLvl = 1;
		StartLvl ();
	}

	void StartLvl(){
		if (actualLvl <= 4) {
			HardcodedLvls [actualLvl - 1].SetActive (true);
			if (actualLvl >= 2)
				HardcodedLvls [actualLvl - 2].SetActive (false);
		} else {
			//RandomLights ();
		}
	}


	public void RandomLights(int numeroBombillas,int numeroBotones){

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
	}*/