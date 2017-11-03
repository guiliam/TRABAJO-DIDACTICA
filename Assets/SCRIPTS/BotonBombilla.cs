using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonBombilla : MonoBehaviour {

	public Renderer[] bombillas;
	public Material encendido, apagado;

	private LineRenderer[] lineas;
	private GameObject lineaBase;

	public void Inicializar(){

		lineaBase = GameObject.Find ("LineaBase");

		foreach (Renderer bombilla in bombillas) {
				bombilla.material = apagado;
				bombilla.gameObject.GetComponentInChildren<Light> ().enabled = false;
		}

		lineas = new LineRenderer[bombillas.Length];

		for (int i = 0; i < lineas.Length; i++) {
			lineas [i] = GameObject.Instantiate (lineaBase).GetComponent<LineRenderer>();
			lineas[i].SetPositions( new Vector3[]{
				new Vector3(this.gameObject.transform.position.x, 1, this.gameObject.transform.position.z),
				new Vector3(bombillas[i].gameObject.transform.position.x, 1, bombillas[i].gameObject.transform.position.z)}
			);
			lineas [i].enabled = false;
		}
	}

	public void Clickar(){
		foreach (Renderer bombilla in bombillas) {
			if (bombilla.material.color == encendido.color) {
				bombilla.material = apagado;
				bombilla.gameObject.GetComponentInChildren<Light> ().enabled = false;
			}
			else {
				bombilla.material = encendido;
				bombilla.gameObject.GetComponentInChildren<Light> ().enabled = true;
			}
				
		}
	}

	public void EntradaRaton()
	{
		foreach (LineRenderer rend in lineas) {
			rend.enabled = true;
		}
	}

	public void SalidaRaton()
	{
		foreach (LineRenderer rend in lineas) {
			rend.enabled = false;
		}
	}
}
