using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara_Movimiento : MonoBehaviour {

	public GameObject mi_player;
	private Vector2 Vel;
	public float Velocidad;
	public float _y = 2.27f;

	void Start () {
		Cambio_Player ();
	}

	void FixedUpdate () {
		float Suave_x = Mathf.SmoothDamp (transform.position.x, mi_player.transform.position.x, ref Vel.x, Velocidad);
		float Suave_y = Mathf.SmoothDamp (transform.position.y, mi_player.transform.position.y + _y, ref Vel.y, Velocidad); 
		transform.position = new Vector3 (Suave_x, Suave_y, transform.position.z);
	}
		

	public void Cambio_Player () {
		mi_player = GameObject.FindGameObjectWithTag ("Control");
	}
}
