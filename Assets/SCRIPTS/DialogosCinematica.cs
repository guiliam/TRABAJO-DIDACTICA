using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogosCinematica : MonoBehaviour {


	public Text texto;
	public string[] frases;
	private int contador = 0;

	// Use this for initialization
	void Start () {
		texto.text = frases [0];
	}
	
	// Update is called once per frame
	void Update () {
		if (contador < frases.Length) {
			if (Input.GetKeyDown ("space")) {
				contador++;
				texto.text = frases [contador];
			}
		} 

		else{
			//carga siguiente nivel
		}
	}
}
