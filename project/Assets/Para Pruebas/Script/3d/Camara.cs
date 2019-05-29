using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour {

	Control_Jugador mi_player;

	void Start () {
		mi_player = FindObjectOfType <Control_Jugador> ();
	}

	void Update () {
		transform.position = new Vector3 (mi_player.transform.position.x, mi_player.transform.position.y, transform.position.z);
	}
}
