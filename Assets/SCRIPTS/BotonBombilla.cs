using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonBombilla : MonoBehaviour {

	public Renderer[] bombillas;
	public Material encendido, apagado;
	public GestorBombillas GestorBombillas;

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
