using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaia_Prefab : MonoBehaviour {

	// VARIABLES: 

	//Variables float publicas para controlar fisicas
	public float VelocidadSuelo;
	public float VelocidadLiana;
	// Un objetc auxiliar "Player" para buscarlo cuand sea necesario
	private Control_Jugador mi_Player;
	// Variables con componentes
	private Rigidbody mi_RB;
	private Animator mi_Anim;
	// Vectores auxiliares para ayudart en las fisicas
	private Vector3 Movi;
	// Variable auxiliar para la funcion de apagar/encender luces
	private Light Luz;
	// Variable auxiliar para ejecutar la posesion
	public GameObject Posible_Cuerpo;
	//Boleanos como maquina de estados
	public bool Gaia_IsActive = true;
	public bool LianaPos = false;
	public bool Cambio_Pos = false;

	//-------------------------------------------------------------------------------------------

	// Inicializamos diferentes variables privadas:
	void Start () {
		mi_RB = GetComponent <Rigidbody> ();
		mi_Anim = GetComponent <Animator> ();
	}

	//-------------------------------------------------------------------------------------------

	// En FixedUpdate (mejor para usar fisicas) compruebo si esta activa
	void FixedUpdate () {		
		if (Gaia_IsActive == true) {
			Mov ();
		}
	}

	//--------------------------------------------------------------------------------------------

	//funcion que controla el movimiento y los inputs
	void Mov () {
		// Inicializo un float con el input axis que corresponde a las teclas direccionales y A,S,D,W
		float xAxis = Input.GetAxis ("Horizontal") * VelocidadSuelo;
		// Las incluyo en un vector3
		Movi = new Vector3 (xAxis, 0, 0) * Time.fixedDeltaTime;
		// Y muevo el Rigidbody
		mi_RB.MovePosition (transform.position + Movi);
		// Creo un grupo de condicionales que generaran acciones en juego
		// De momento uso funciones aparte para el mejor manejo de ellas		 
		// Con Z subira lianas, pudrira vegetacion y encendera/apagara las luces
		if (Input.GetKey (KeyCode.Z)) {
			Luz_Liana_Pudrir ();
		}
		// Con X poseera otrs cuerpos
		if (Input.GetKey (KeyCode.X)) {
			Posesion ();
		}
	}

	// Funciones de las acciones del personajes
	// Posesion de los cuerpos
	void Posesion () {
		// Condicional para saber si esta en el lugar correcto
		// Intento hacer una funcion que valga para todos los cuerpos
		// Y que sea la colision la que defina que personaje es
		if (Cambio_Pos == true) {
			// Busco el object de control
			mi_Player = FindObjectOfType <Control_Jugador> ();
			// Inicio la variable del cambio de cuerpo del control con el que recojo de la colision de este object
			mi_Player.Nuevo_Cuerpo = Posible_Cuerpo;
			// Tiro la animacion de la accion
			mi_Anim.SetBool ("IsPoseso", true);
			// Ejecuto la funcion de posesion del "Control_Juagador" desde aqui
			mi_Player.Posesion ();
		}
	}
	// Funcion para ejecutar la accion de principal de Gaia
	void Luz_Liana_Pudrir () {
		// Creo una serie de condicionales para saber que respuesta tiene el boton en cada momento
		// Si hay una inicializacion desde el collision de una luz esta dejara de ser null
		// Si no es null podremos encender/apagar dicha luz
		if (Luz != null) {
			if (Luz.enabled == false) {
				Luz.enabled = true;
			} else if (Luz.enabled == true) {
				Luz.enabled = false;
			}
		}
		// Si la posicion coincide con una liana se ejecutara la accion de subir por ella
		if (LianaPos == true) {
			Vector3 Subir = new Vector3 (0, VelocidadLiana, 0);
			mi_RB.MovePosition (transform.position + Subir);
		}
	}

	//-----------------------------------------------------------------------------------------------

	//TRIGGERS Y COLLISIONS PARA CONTROLAR CONTACTOS:

	// Miro que triggers entran en contacto con el personaje
	void OnTriggerEnter (Collider col) {
		// Si es una liana
		if (col.tag == "Liana") {
			// Ajusto a verdad que esta en una posicion con una liana
			LianaPos = true;
		}
		// Si entra en contacto con un cadaver
		if (col.tag == "Cadaver") {
			// Cambio el bool para avisar de que puede poseer el cuerpo
			Cambio_Pos = true;
			// Inicializo el object auxiliar con el object de cuerpo que esta tocando
			// Asi podra mandarlo al "Control_Jugador"
			Posible_Cuerpo = col.gameObject;
		}
		// Si esta en contacto con el trigger de una luz 
		if (col.gameObject.tag == "Luz") {
			// Inicializo la luz con la que esta en contacto para poder acceder a ella
			Luz = col.GetComponent <Light> ();
		}
	}
	// Los Exits se usan para dar salida a las cosas que se han inicializado en los Enter
	// Los boleanos se ajustan a False y los object se anulan
	void OnTriggerExit (Collider col) {
		if (col.tag == "Liana") {
			LianaPos = false;
		} if (col.tag == "Cadaver") {
			Cambio_Pos = false;
			Posible_Cuerpo = null;
		} if (col.gameObject.tag == "Luz") {
			Luz = null;
		}
	}
}
