using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonBorrado : MonoBehaviour {

	public Renderer[] bombillas;
	public Material apagado;

	public void Clickar(){
		foreach (Renderer bombilla in bombillas) {
				bombilla.material = apagado;
				bombilla.gameObject.GetComponentInChildren<Light> ().enabled = false;
			}
		}
}
