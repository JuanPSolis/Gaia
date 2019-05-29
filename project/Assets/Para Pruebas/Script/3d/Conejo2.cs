using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conejo2 : MonoBehaviour {

	//Variables necesarias:
	public float VelocidadSuelo;
	public float AlturaSalto;
	private Player mi_Player;
	private Rigidbody mi_RB;
	private Vector3 Movi;

	//Boleanos para controlar los movimientos:
	public bool Conejo_IsActive = true;
	public bool Saltando = true;
	public bool TocandoSuelo = false;

	void Start () {
		mi_RB = GetComponent <Rigidbody> ();
	}

	void FixedUpdate () {		
		if (Conejo_IsActive == true) {
			Mov ();
		}
	}

	void Mov () {
		//funcion que controla el movimiento
		//------------------------------------------------------------------------
		/*if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Translate (Vector3.left * Velocidad * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.Translate (Vector3.right * Velocidad * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.UpArrow) && LianaPos == true) {
			transform.Translate (Vector3.up * Velocidad * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.DownArrow) && LianaPos == true) {
			transform.Translate (Vector3.down * Velocidad * Time.deltaTime);
		}*/
		//------------------------------------------------------------------------
		/*if (Input.GetKey (KeyCode.LeftArrow)) {
			mi_RB.velocity = new Vector3 (-Velocidad, mi_RB.velocity.y, mi_RB.velocity.z);
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			mi_RB.velocity = new Vector3 (Velocidad, mi_RB.velocity.y, mi_RB.velocity.z);
		}
		if (Input.GetKey (KeyCode.UpArrow) && LianaPos == true) {
			mi_RB.velocity = new Vector3 (mi_RB.velocity.x, Velocidad, mi_RB.velocity.z);
		}
		if (Input.GetKey (KeyCode.DownArrow) && LianaPos == true) {
			mi_RB.velocity = new Vector3 (mi_RB.velocity.x, -Velocidad, mi_RB.velocity.z);
		}*/
		//--------------------------------------------------------------------------
		float xAxis = Input.GetAxis ("Horizontal") * VelocidadSuelo;
		Movi = new Vector3 (xAxis, 0, 0) * Time.fixedDeltaTime;
		mi_RB.MovePosition (transform.position + Movi);
		if (Input.GetKey (KeyCode.UpArrow)) {
			Accion ();
		}
		if (Input.GetKey (KeyCode.C)) {
			mi_Player = FindObjectOfType <Player> ();
			mi_Player.CambioPosesion ("Gaia");
		}
	}

	void Accion () {
		if (Saltando == false && TocandoSuelo == true) {
			Saltando = true;
			mi_RB.AddForce (0f, AlturaSalto, 0f, ForceMode.Impulse);
		}
	}


	//Diferentes triger y collision donde se selecciona que accion puede ejecutar en ese momento
	void OnCollisionStay (Collision col) {
		if (col.gameObject.tag == "Suelo") {
			Saltando = false;
			TocandoSuelo = true;
		}
	}

	void OnCollisionExit (Collision col) {
		if (col.gameObject.tag == "Suelo") {
			TocandoSuelo = false;
		}
	}
}
