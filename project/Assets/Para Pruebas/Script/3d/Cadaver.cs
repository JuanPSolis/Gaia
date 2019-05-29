using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cadaver : MonoBehaviour {

	// Vamor a hacer un script sencillo para que el collider del cuerpo que vaya a poseer Gaia
	// no entorpezca con el propio collider de Gaia. Asi mismo servira para ser buscado en el editor 
	// via script si hay que mandar algun mensaje.

	//Variable para el collider
	private BoxCollider mi_Col;


	void Start () {
		// inicializamos el collider
		mi_Col = GetComponent <BoxCollider> ();
		mi_Col.isTrigger = false;
	}

	void OnCollisionEnter (Collision col) {
		if (col.transform.tag == "Gaia") {
			mi_Col.isTrigger = true;
			Debug.Log (col.transform.tag);
		}
	}
	void OnCollisionStay (Collision col) {
		if (col.transform.tag == "Gaia") {
			mi_Col.isTrigger = true;
			Debug.Log (col.transform.tag);
		}
	}
	void OnCollisionExit (Collision col) {
		if (col.transform.tag == "Gaia") {
			mi_Col.isTrigger = false;
		}
	}
}
