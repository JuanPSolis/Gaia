using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oso_Definitivo : MonoBehaviour {

	// VARIABLES:

	// Variables float publicas para controlar fisicas
	public float VelocidadSuelo;
	// Variable para inicializar a Gaia
	private Gaia_Definitiva mi_Gaia;
	// Variables con componentes
	private Rigidbody mi_RB;
	private BoxCollider mi_BoxCol;
	private Animator mi_Anim;
	// Vectores auxiliares para ayudart en las fisicas
	private Vector3 Movi;
	//Boleanos como maquina de estados
	public bool Oso_EsCadaver;
	public bool Atacando = false;
	public bool Defendiendo = false;

	//---------------------------------------------------------------------------------------------

	// Inicializamos diferentes variables privadas
	void Start () {
		mi_Gaia = FindObjectOfType <Gaia_Definitiva> ();
		mi_RB = GetComponent <Rigidbody> ();
		mi_BoxCol = GetComponent <BoxCollider> ();
		mi_Anim = GetComponent <Animator> ();
		EsCadaver ();
	}

	//----------------------------------------------------------------------------------------------

	// En FixedUpdate (mejor para usar fisicas) compruebo si es o no un cadaver
	void FixedUpdate () {		
		if (Oso_EsCadaver == false) {
			// Si no es un cadaver se permitira el movimiento, por lo tanto el jugador pasara a 
			// controlar el personaje
			Mov ();
		}
	}

	//----------------------------------------------------------------------------------------------

	// FUNCIONES PARA EL PERSONAJE "Oso":

	// Funcion que controla el movimiento y los inputs
	void Mov () {
		// Inicializo un float con el input axis que corresponde a las teclas direccionales y A,S,D,W
		float xAxis = Input.GetAxis ("Horizontal") * VelocidadSuelo;
		// Las incluyo en un vector3
		Movi = new Vector3 (xAxis, 0, 0) * Time.fixedDeltaTime;
		// Y muevo el Rigidbody
		mi_RB.MovePosition (transform.position + Movi);
		// Creo un grupo de condicionales que generaran acciones en juego
		// De momento uso funciones aparte para el mejor manejo de ellas		 
		// Con Z 
		if (Input.GetKey (KeyCode.Z)) {
			Salto ();
		}
		// Con C hare que Gaia "desposea" este cuerpo
		if (Input.GetKey (KeyCode.C)) {
			Desposesion ();
		}
	}
	// Funciones de las acciones del personajes
	// Salto del conejo
	void Salto () {
		
	}
	// Funcion para recuperar el aspecto de Gaia
	// Tambien se podria mandar un mensaje
	void Desposesion () {
		// No podra abandonar su cuerpo en un salto
		if (Atacando == false && Defendiendo == false) {
			// Ahora puedo ejecutar la funcion dentro de Gaia
			mi_Gaia.StartCoroutine ("VuelveGaia");
		}
	}

	// Creo dos funciones para que puedan ser llamadas con un mensaje desde otros object 
	public void EsCadaver () {
		// Se ajustara para que el object no se pueda mover
		Oso_EsCadaver = true;
		// Lo hago cinematico y le hago verdadero el istrigger para que no choque contra Gaia
		mi_RB.isKinematic = true;
		mi_BoxCol.isTrigger = true;
		// Con este tag sera reconocido como un posible cuerpo
		transform.tag = "Cadaver";
	}
	public void NoEsCadaver () {
		// Se ajustara para que el object se pueda mover
		Oso_EsCadaver = false;
		// Le desactivo el istrigger y la cinematica para que colisione normalmente
		mi_RB.isKinematic = false;
		mi_BoxCol.isTrigger = false;
		// Con este tag aseguramos que mientra sea object activo no sea un posible cuerpo
		transform.tag = "Oso";
	}

	//-----------------------------------------------------------------------------------------------

	//TRIGGERS Y COLLISIONS PARA CONTROLAR CONTACTOS:

	// Miro que esta en contacto con el porsonaje
	void OnCollisionStay (Collision col) {
		
	}
	// Miro que ha dejado de estar en contacto con el object
	void OnCollisionExit (Collision col) {
		
	}
}
