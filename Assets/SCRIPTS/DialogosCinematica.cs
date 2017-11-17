using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogosCinematica : MonoBehaviour {


	public Text texto;
	public string[] frases;
	public GameObject PersonajeEntero, PersonajeSinPiernas, PersonajeSinBrazos;
	public GameObject Hoja, BocadilloTexto;
	private AudioSource audio;
	private int contador = 0;
	private float tiempo;
	private bool contarTiempoTexto;

	// Use this for initialization
	void Start () {
		texto.text = frases [0];
		audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (contador < frases.Length) {

			if (contador == 1) {
				PersonajeEntero.SetActive (true);
			}

			if (contador == 4) {
				PersonajeEntero.SetActive (false);
				PersonajeSinPiernas.SetActive (true);
				BocadilloTexto.SetActive (true);
			}

			if (contador == 5) {
				PersonajeSinPiernas.SetActive (false);
				PersonajeSinBrazos.SetActive (true);
			}

			if (Input.GetKeyDown ("space")) {
				Hoja.SetActive (false);
				contarTiempoTexto = true;
				audio.Play ();

			}

			if (Hoja.activeSelf == false) {
				Hoja.SetActive (true);

			}

			if (contarTiempoTexto) {
				tiempo += Time.deltaTime;
				if (tiempo > 1) {
					contador++;
					texto.text = frases [contador];
					tiempo = 0;
					contarTiempoTexto = false;
				}
			}
		}
	}
}