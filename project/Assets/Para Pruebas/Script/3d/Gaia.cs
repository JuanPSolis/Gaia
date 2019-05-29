using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaia : MonoBehaviour {

	//Variables necesarias:
	public float VelocidadSuelo;
	public float VelocidadLiana;
	private Player mi_Player;
	private Rigidbody mi_RB;
	private Vector3 Movi;
	private Light Luz;

	//Boleanos para controlar los movimientos:
	public bool Gaia_IsActive = true;
	public bool LianaPos = false;
	public bool ConejoPos = false;

	void Start () {
		mi_RB = GetComponent <Rigidbody> ();
	}

	void FixedUpdate () {		
		if (Gaia_IsActive == true) {
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
		if (Input.GetKey (KeyCode.Z)) {
			Accion2 ();
		}
		if (Input.GetKey (KeyCode.X)) {
			Accion1 ();
		}
		if (Input.GetKey (KeyCode.C)) {
			
		}
	}

	void Accion1 () {
		if (ConejoPos == true) {
			mi_Player = FindObjectOfType <Player> ();
			mi_Player.CambioPosesion ("Conejo");
		}
	}
	void Accion2 () {
		if (Luz != null) {
			Luz.enabled = true;
		}
		if (LianaPos == true) {
			Vector3 Subir = new Vector3 (0, VelocidadLiana, 0);
			mi_RB.MovePosition (transform.position + Subir);
		}
	}


	//Diferentes triger donde se selecciona que accion puede ejecutar en ese momento
	void OnTriggerEnter (Collider col) {
		if (col.tag == "Liana") {
			LianaPos = true;
		}
		if (col.tag == "Conejo") {
			ConejoPos = true;
		}
		// Usamos el collider para ajustar la luz que queremos encender/apagar
		if (col.gameObject.tag == "Luz") {
			Luz = col.GetComponent <Light> ();
		}
	}

	void OnTriggerExit (Collider col) {
		if (col.tag == "Liana") {
			LianaPos = false;
		}
		if (col.tag == "Conejo") {
			ConejoPos = false;
		}
		// Usamos el collider para ajustar la luz que queremos encender/apagar
		if (col.gameObject.tag == "Luz") {
			Luz = null;
		}
	}
}
