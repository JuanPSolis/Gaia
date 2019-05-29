using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conejo_Prefab : MonoBehaviour {

	// VARIABLES:

	// Variables float publicas para controlar fisicas
	public float VelocidadSuelo;
	public float AlturaSalto;
	// Un objetc auxiliar "Player" para buscarlo cuand sea necesario
	private Control_Jugador mi_Player;
	// Variables con componentes
	private Rigidbody mi_RB;
	private BoxCollider mi_Col;
	private Animator mi_Anim;
	// Vectores auxiliares para ayudart en las fisicas
	private Vector3 Movi;
	//Boleanos como maquina de estados
	public bool Conejo_EsCadaver;
	public bool Saltando = true;
	public bool TocandoSuelo = false;

	//---------------------------------------------------------------------------------------------

	// Inicializamos diferentes variables privadas
	void Start () {
		mi_RB = GetComponent <Rigidbody> ();
		mi_Col = GetComponent <BoxCollider> ();
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
		} else {
			
		}
	}
		
	//----------------------------------------------------------------------------------------------

	// FUNCIONES PARA EL PERSONAJE "Conejo":

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
		if (Saltando == false && TocandoSuelo == true) {
			// No se por que, pero si no  anulo el bool "Saltando" nada mas ejecutar la accion, funciona mal
			Saltando = true;
			// Simplemente aplico una fuerza de impulso al eje Y en positivo
			mi_RB.AddForce (0f, AlturaSalto, 0f, ForceMode.Impulse);
			// Ejecuto la animaion
			mi_Anim.SetBool ("Saltando", true);
		}
	}
	// Funcion para recuperar el aspecto de Gaia
	// Tambien se podria mandar un mensaje
	void Desposesion () {
		// Busco al object que controla la partida del jugador
		mi_Player = FindObjectOfType <Control_Jugador> ();
		// Ahora puedo ejecutar la funcion dentro de "Player"
		mi_Player.VuelveGaia ();
	}

	// Creo dos funciones para que puedan ser llamadas con un mensaje desde otros object 
	public void EsCadaver () {
		// Se ajustara para que el object no se pueda mover
		Conejo_EsCadaver = true;
		// Hago que no detecte la colision con Gaia solo el trigger
		// Lo hago cinematico y le hago verdadero el istrigger para que no choque contra Gaia
		mi_RB.isKinematic = true;
		mi_Col.isTrigger = true;
		// Con este tag sera reconocido como un posible cuerpo
		transform.tag = "Cadaver";
	}
	public void NoEsCadaver () {
		// Se ajustara para que el object se pueda mover
		Conejo_EsCadaver = false;
		// Hago que detecte las colisiones
		// Le desactivo el istrigger y la cinematica para que colisione normalmente
		mi_RB.isKinematic = false;
		mi_Col.isTrigger = false;
		// Con este tag aseguramos que mientra sea object activo no sea un posible cuerpo
		transform.tag = "Conejo";
	}

	//-----------------------------------------------------------------------------------------------

	//TRIGGERS Y COLLISIONS PARA CONTROLAR CONTACTOS:

	// Miro que esta en contacto con el porsonaje
	void OnCollisionStay (Collision col) {
		// Si el tag del object en colision es el "Suelo" diremos que lo esta tocando y negaremos el salto
		if (col.gameObject.tag == "Suelo") {
			// En caso de que haya doble salto, habra que modificar estas lineas
			Saltando = false;
			TocandoSuelo = true;
		}
	}
	// Miro que ha dejado de estar en contacto con el object
	void OnCollisionExit (Collision col) {
		// Si el tag es "Suelo" negaremos que este en contacto con el
		if (col.gameObject.tag == "Suelo") {
			TocandoSuelo = false;
		}
	}
}
