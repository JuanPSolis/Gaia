using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	// variable para la forma que tieneel jugador en ese momento y para la que va a cambiar
	private GameObject mi_Player;
	private GameObject Cambio_Player;

	// Variables con las formas posibles del jugador
	public GameObject Gaia_porDefecto;
	public GameObject Conejo;

	// Variable para desactivar el "cadaver" del animal que poseemos
	//public GameObject Cadaver;

	void Start () {
	//Alempezar se elegira a gaia
		mi_Player = Instantiate (Gaia_porDefecto, transform.position, transform.rotation) as GameObject;
		transform.parent = mi_Player.transform;
	}
		

	// Funcion publica para el cambiode personaje
	public void CambioPosesion (string Personaje) {
		mi_Player.transform.parent = transform;
		Destroy (mi_Player);
		if (Personaje == "Gaia") {
			Cambio_Player = Instantiate (Gaia_porDefecto, transform.position, transform.rotation) as GameObject;
			//Cadaver.SetActive (true);
			//Cadaver.transform.position = new Vector3 (transform.position.x, Cadaver.transform.position.y, Cadaver.transform.position.z);
		}
		if (Personaje == "Conejo") {
			Cambio_Player = Instantiate (Conejo, transform.position, transform.rotation) as GameObject;
			//Cadaver.SetActive (false);
		}
		mi_Player = Cambio_Player;
		transform.parent = mi_Player.transform;
	}
}
