using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara2 : MonoBehaviour {

	public GameObject mi_player;

	void Start () {
		Cambio_Player ();
	}

	void Update () {
		transform.position = new Vector3 (mi_player.transform.position.x, mi_player.transform.position.y, transform.position.z);
	}

	public void Cambio_Player () {
		mi_player = GameObject.FindGameObjectWithTag ("Control");
	}
}
