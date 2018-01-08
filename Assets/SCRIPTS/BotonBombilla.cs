using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonBombilla : MonoBehaviour {

	public Renderer[] bombillas;
	GameObject[] indicadores;
	public GameObject PrefabIndicador;
	public Material encendido, apagado;
	public GestorBombillas GestorBombillas;

	void Start(){
		indicadores = new GameObject[bombillas.Length];
		for (int i = 0; i < bombillas.Length; i++){
			GameObject indicador = Instantiate (PrefabIndicador, bombillas[i].transform.position, PrefabIndicador.transform.rotation, bombillas[i].transform);
			indicador.transform.localPosition = new Vector3 (0, 0, 0.09f);
			indicadores [i] = indicador;
			indicador.SetActive (false);
		}
	}

	public void Inicializar(){
		foreach (Renderer bombilla in bombillas) {
				bombilla.material = apagado;
				bombilla.gameObject.GetComponentInChildren<Light> ().enabled = false;
		}
	}

	public void Clickar(){
		foreach (Renderer bombilla in bombillas) {
			if (bombilla.material.color == encendido.color) {
				bombilla.material = apagado;
				bombilla.gameObject.GetComponentInChildren<Light> ().enabled = false;
				GestorBombillas.RestarBombilla ();
			}
			else {
				bombilla.material = encendido;
				bombilla.gameObject.GetComponentInChildren<Light> ().enabled = true;
				GestorBombillas.SumarBombilla ();
			}
				
		}
	}

	public void ActivarIndicador(){
		foreach (GameObject ind in indicadores) {
			ind.SetActive (true);
		}
	}

	public void DesactivarIndicador(){
		foreach (GameObject ind in indicadores) {
			ind.SetActive (false);
		}
	}



	/*public void EntradaRaton()
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
	}*/
}
