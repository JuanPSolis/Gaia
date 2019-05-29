using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conejo_Definitivo : MonoBehaviour {

	// VARIABLES:

	// Variables float publicas para controlar fisicas
	public float Velocidad_Suelo;
	public float Altura_Salto;
	// Variable para inicializar a Gaia
	private Gaia_Definitiva mi_Gaia;
	// Variables con componentes
	private Rigidbody mi_RB;
	private BoxCollider mi_BoxCol;
	private Animator mi_Anim;
	// Vectores auxiliares para ayudart en las fisicas
	private Vector3 Movi;
	//Boleanos como maquina de estados
	public bool Conejo_EsCadaver;
	public bool Saltando = false;
	public bool Tocando_Suelo = false;

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
		if (Conejo_EsCadaver == false) {
			// Si no es un cadaver se permitira el movimiento, por lo tanto el jugador pasara a 
			// controlar el personaje
			Mov ();
		}
	}

	//----------------------------------------------------------------------------------------------

	// FUNCIONES PARA EL PERSONAJE "Conejo":

	// Funcion que controla el movimiento y los inputs
	void Mov () {
		// Inicializo un float con el input axis que corresponde a las teclas direccionales y A,S,D,W
		float xAxis = Input.GetAxis ("Horizontal") * Velocidad_Suelo;
		// Las incluyo en un vector3
		Movi = new Vector3 (xAxis, 0, 0) * Time.fixedDeltaTime;
		// Y muevo el Rigidbody
		mi_RB.MovePosition (transform.position + Movi);
		// Creo un grupo de condicionales que generaran acciones en juego
		// De momento uso funciones aparte para el mejor manejo de ellas		 
		// Con Z saltara
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
		// Compruebo que no este saltando ya y que este tocando suelo
		if (Saltando == false && Tocando_Suelo == true) {
			// No se por que, pero si no  anulo el bool "Saltando" nada mas ejecutar la accion, funciona mal
			Saltando = true;
			// Simplemente aplico una fuerza de impulso al eje Y en positivo
			mi_RB.AddForce (0f, Altura_Salto, 0f, ForceMode.Impulse);
			// Ejecuto la animaion
			mi_Anim.SetBool ("Saltando", true);
		}
	}
	// Funcion para recuperar el aspecto de Gaia
	// Tambien se podria mandar un mensaje
	void Desposesion () {
		// No podra abandonar su cuerpo en un salto
		if (Tocando_Suelo == true) {
			// Traslado a Gaia a este punto
			mi_Gaia.transform.position = transform.position;
			// Ahora puedo ejecutar la funcion dentro de Gaia
			mi_Gaia.StartCoroutine ("VuelveGaia");
		}
	}

	// Creo dos funciones para que puedan ser llamadas con un mensaje desde otros object 
	public void EsCadaver () {
		// Se ajustara para que el object no se pueda mover
		Conejo_EsCadaver = true;
		// Lo hago cinematico y le hago verdadero el istrigger para que no choque contra Gaia
		mi_RB.isKinematic = true;
		mi_BoxCol.isTrigger = true;
		// Con este tag sera reconocido como un posible cuerpo
		transform.tag = "Cadaver";
	}
	public void NoEsCadaver () {
		// Se ajustara para que el object se pueda mover
		Conejo_EsCadaver = false;
		// Le desactivo el istrigger y la cinematica para que colisione normalmente
		mi_RB.isKinematic = false;
		mi_BoxCol.isTrigger = false;
		// Con este tag aseguramos que mientra sea object activo no sea un posible cuerpo
		transform.tag = "Control";
		// Busco la camara pra que siga al cuerpo poseido
		Camara2 cam = FindObjectOfType <Camara2> ();
		cam.Cambio_Player ();
	}

	//-----------------------------------------------------------------------------------------------

	//TRIGGERS Y COLLISIONS PARA CONTROLAR CONTACTOS:

	// Miro que esta en contacto con el porsonaje
	void OnCollisionStay (Collision col) {
		// Si el tag del object en colision es el "Suelo" diremos que lo esta tocando y negaremos el salto
		if (col.gameObject.tag == "Suelo") {
			// En caso de que haya doble salto, habra que modificar estas lineas
			Saltando = false;
			Tocando_Suelo = true;
		}
	}
	// Miro que ha dejado de estar en contacto con el object
	void OnCollisionExit (Collision col) {
		// Si el tag es "Suelo" negaremos que este en contacto con el
		if (col.gameObject.tag == "Suelo") {
			Tocando_Suelo = false;
		}
	}
}
