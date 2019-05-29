using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaia_Definitiva : MonoBehaviour {

	// VARIABLES: 

	//Variables float publicas para controlar fisicas
	public float Velocidad_Suelo;
	public float Velocidad_Liana;
	// Variables con componentes
	private Rigidbody mi_RB;
	private Animator mi_Anim;
	private BoxCollider mi_BoxCol;
	private MeshRenderer mi_Rend;
	// Vectores auxiliares para ayudart en las fisicas
	private Vector3 Movi;
	// Variable auxiliar para la funcion de apagar/encender luces
	private Light Luz;
	// Variable auxiliar para ejecutar la posesion
	public GameObject Posible_Cuerpo;
	public GameObject Cuerpo_Poseido;
	//Boleanos como maquina de estados
	public bool Gaia_IsActive = true;
	public bool Liana_Pos = false;
	public bool Cambio_Pos = false;

	//-------------------------------------------------------------------------------------------

	// Inicializamos diferentes variables privadas:
	void Start () {
		mi_RB = GetComponent <Rigidbody> ();
		mi_Anim = GetComponent <Animator> ();
		mi_BoxCol = GetComponent <BoxCollider> ();
		mi_Rend = GetComponent <MeshRenderer> ();
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
	public void Mov () {
		// Inicializo un float con el input axis que corresponde a las teclas direccionales y A,S,D,W
		float xAxis = Input.GetAxis ("Horizontal") * Velocidad_Suelo;
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
			StartCoroutine ("Posesion");
		}
	}

	// Funciones de las acciones del personajes
	// Posesion de los cuerpos
	public IEnumerator Posesion () {
		// Condicional para saber si esta en el lugar correcto
		// Intento hacer una funcion que valga para todos los cuerpos
		// Y que sea la colision la que defina que personaje es
		if (Cambio_Pos == true) {
			// Cambio el tag
			transform.tag = "Jugador";
			// Igualo el posible cuerpo al cuerpo poseido
			Cuerpo_Poseido = Posible_Cuerpo;
			// Desactivo el movimiento de Gaia y sus componentes
			Gaia_IsActive = false;
			mi_RB.isKinematic = true;
			mi_BoxCol.isTrigger = true;
			// Tiro la animacion de la accion
			mi_Anim.SetTrigger ("IsPoseso");
			// Espero unos segundos a que la animacion termine
			yield return new WaitForSeconds (2);
			// Mando un mensaje para que el personaje que abandonas ejecute la funcion de abandonar el movimiento
			Cuerpo_Poseido.SendMessage ("NoEsCadaver");
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
		if (Liana_Pos == true) {
			Vector3 Subir = new Vector3 (0, Velocidad_Liana, 0);
			mi_RB.MovePosition (transform.position + Subir);
		}
	}

	//------------------------------------------------------------------------------------------------

	// FUNCONES PUBLICAS PARA PODER SER LAMADAS DESDE EL RESTO DE LOS OBJECTS:

	// Funcion para volver a ser Gaia
	// Esta funcion podra ser llamada por el resto de personajes
	public IEnumerator VuelveGaia () {
		// Mando un mensaje para que el personaje que abandonas ejecute la funcion de abandonar el movimiento
		Cuerpo_Poseido.SendMessage ("EsCadaver");
		// Tiro la animacion de la accion
		mi_Anim.SetTrigger ("NoIsPoseso");
		// Espero unos segundos a que la animacion termine
		yield return new WaitForSeconds (2);
		// Activo el movimiento de Gaia y sus componentes
		Gaia_IsActive = true;
		mi_RB.isKinematic = false;
		mi_BoxCol.isTrigger = false;
		// Anula el cuerpo poseido
		Cuerpo_Poseido = null;
		// Cambio el tag
		transform.tag = "Control";
		// Busco la camara pra que siga al cuerpo poseido
		Camara2 cam = FindObjectOfType <Camara2> ();
		cam.Cambio_Player ();
	}

	//-----------------------------------------------------------------------------------------------

	//TRIGGERS Y COLLISIONS PARA CONTROLAR CONTACTOS:

	// Miro que triggers entran en contacto con el personaje
	void OnTriggerEnter (Collider col) {
		// Si es una liana
		if (col.tag == "Liana") {
			// Ajusto a verdad que esta en una posicion con una liana
			Liana_Pos = true;
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
			Liana_Pos = false;
		} if (col.tag == "Cadaver") {
			Cambio_Pos = false;
			Posible_Cuerpo = null;
		} if (col.gameObject.tag == "Luz") {
			Luz = null;
		}
	}
}
