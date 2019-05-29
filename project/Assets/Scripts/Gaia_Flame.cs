using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaia_Flame : Controler {

	// VARIABLES:

	// Velocidad por la que multiplicamos el input del eje x del vector Velocidad
	public float VelocidadMov;
	// Aceleracion para el suavizado del eje x en suelo
	public float AceleracionSuelo;
	// Aceleracion para el suavizado del eje x en el aire
	public float AceleracionAire;
	// Gravedad generada en Start()
	public float Gravedad;
	// Auxiliar para el suavizado de x
	float Suavizado;
	// Vector3 que recoge el input del axxis
	Vector3 Velocidad;
	//Variables float publicas para controlar fisicas
	public float Velocidad_Liana;
	// Variable con el manager del juego
	Game_Manager G_M;
	public Game_Manager mi_Game;
	// Variables con componentes
	BoxCollider2D mi_BoxCol;
	SpriteRenderer mi_Rend;
	// Variable auxiliar para la funcion de apagar/encender luces
	Light Luz;
	// Variable para la liana
	Liana Liana;
	// Variable auxiliar para ejecutar la posesion
	GameObject Posible_Cuerpo;
	public GameObject Cuerpo_Poseido;
	// Variable para el muro que tene que desaparecer
	SpriteRenderer Muro_Rend;
	//Boleanos como maquina de estados
	public bool _IsActive = true;
	public bool Liana_Pos = false;
	public bool Cambio_Pos = false;
	public bool En_Muro_Pos = false;

	//-------------------------------------------------------------------------------------------

	// Inicializamos diferentes variables privadas:
	void Awake () {
//		G_M = FindObjectOfType <Game_Manager> ();
//		mi_Game = G_M.GetComponent <Game_Manager> ();
	}
	public override void Start() {
		mi_BoxCol = GetComponent <BoxCollider2D> ();
		mi_Rend = GetComponent <SpriteRenderer> ();
		base.Start ();

		print ("Gravity: " + Gravedad);
	}

	void Update() {
		if (_IsActive) {
			// Miramos si toca un suelo o techo
			if (Info_Col.Encima || Info_Col.Debajo || Liana_Pos == true) {
				// Si es verdad la velocidad en el eje y del vector Velocidad sera 0, eso dara mas estabilidad al ejecutar cadaframe
				Velocidad.y = 0;
			}
			// los ejes x e y del vector Velocidad se rellenan con los axxis
			Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			// Funcion de salto
			if (Input.GetKeyDown (KeyCode.Z) && Info_Col.Debajo) {
				StartCoroutine ("Posesion");
			}
			if (Input.GetKeyDown (KeyCode.X)) {
				if (Luz != null) {
					ApagaEnciendeLuz ();
				}
				if (Liana_Pos) {
					SubirLiana ();
				}
			}
			// Creamos una variable con el movimiento del eje x	
			float _X = input.x * VelocidadMov;
			// Aplicamos el (input x velocidad de movimiento) al eje x del vector Velocidad ahora suavizandola con Mathf.SmoothDamp
			// La aceleracion hara que se llegue de un punto a otro del suavizado mas rapido o mas lento
			Velocidad.x = Mathf.SmoothDamp (Velocidad.x, _X, ref Suavizado, (Info_Col.Debajo) ? AceleracionSuelo : AceleracionAire);
			Velocidad.y += Gravedad * Time.deltaTime;
			// Llamamos a la funcion movimiento mandoles el vector3 Velocidad
			// El eje y estara formado con la gravedad o la fuerza del salto
			// El eje x estara formado por el input multiplicado por la velocidad y despues suavizado
			Movimiento (Velocidad * Time.deltaTime);
		} else {
			transform.position = Cuerpo_Poseido.transform.position;
		}
	}

	//-------------------------------------------------------------------------------------------

	// FUNCIONES PARA LAS SKILLS DEL PERSONAJE:

	// Posesion de los cuerpos
	public IEnumerator Posesion () {
		// Condicional para saber si esta en el lugar correcto
		// Intento hacer una funcion que valga para todos los cuerpos
		// Y que sea la colision la que defina que personaje es
		if (Cambio_Pos) {
			// Igualo el posible cuerpo al cuerpo poseido
			Cuerpo_Poseido = Posible_Cuerpo;
			// Desactivo el movimiento de Gaia y sus componentes
			_IsActive = false;
			mi_BoxCol.enabled = false;
			mi_Rend.enabled = false;
			// Espero unos segundos a que la animacion termine
			yield return new WaitForSeconds (2);
			// Mando un mensaje para que el personaje que abandonas ejecute la funcion de abandonar el movimiento
			Cuerpo_Poseido.SendMessage ("NoEsCadaver");
		}
	}

	void ApagaEnciendeLuz () {
		// Creo una serie de condicionales para saber que respuesta tiene el boton en cada momento
		// Si hay una inicializacion desde el collision de una luz esta dejara de ser null
		// Si no es null podremos encender/apagar dicha luz
		if (Luz.enabled == false) {
			Luz.enabled = true;
			if (Luz.transform.name == "Hoguera") {
				mi_Game.Guardar_Partida ();
			}
		} else {
			Luz.enabled = false;
		}
	}

	void SubirLiana () {
		// Si la posicion coincide con una liana se ejecutara la accion de subir por ella
		for (int i = 0; i < Liana.Trozos_Liana.Length; i++) {
			transform.position = new Vector2 (transform.position.x, Liana.Trozos_Liana [i].transform.position.y);
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
		//		mi_Anim.SetTrigger ("NoIsPoseso");
		// Espero unos segundos a que la animacion termine
		yield return new WaitForSeconds (2);
		// Activo el movimiento de Gaia y sus componentes
		_IsActive = true;
		mi_BoxCol.enabled = true;
		mi_Rend.enabled = true;
		// Anula el cuerpo poseido
		Cuerpo_Poseido = null;
	}

	// Funcion al morir el jugador
	public void Muerte () {
		mi_Game.Cargar_Partida ();
	}

	//-----------------------------------------------------------------------------------------------

	//TRIGGERS Y COLLISIONS PARA CONTROLAR CONTACTOS:

	// Busco los collider sin trigger
	void OnCollisionEnter2D (Collision2D col) {
		if (col.transform.tag == "Enemy") {
			Muerte ();
		}
	}

	// Miro que triggers entran en contacto con el personaje
	void OnTriggerEnter2D (Collider2D col) {
		// Si es una liana
		if (col.tag == "Liana") {
			// Ajusto a verdad que esta en una posicion con una liana y cojo referencia
			Liana_Pos = true;
			Liana = col.GetComponent <Liana> ();
			Debug.Log ("liana " + Liana.name);
		}
		// Si entra en contacto con un cadaver
		if (col.tag == "Cadaver") {
			// Cambio el bool para avisar de que puede poseer el cuerpo
			Cambio_Pos = true;
			// Inicializo el object auxiliar con el object de cuerpo que esta tocando
			// Asi podra mandarlo al "Control_Jugador"
			Posible_Cuerpo = col.gameObject;
			Debug.Log ("posible cuerpo " + Posible_Cuerpo.name);
		}
		// Si esta en contacto con el trigger de una luz 
		if (col.tag == "Luz") {
			// Inicializo la luz con la que esta en contacto para poder acceder a ella
			Luz = col.GetComponent <Light> ();
			Debug.Log ("luz " + Luz.name);
		}
		// Si cae por un precipicio habra un trigger
		if (col.tag == "Muerte") {
			Muerte ();
		}
		if (col.tag == "Muro") {
			En_Muro_Pos = true;
			Muro_Rend = col.gameObject.GetComponent <SpriteRenderer> ();
			Muro_Rend.enabled = false;
		}
	}
	// Los Exits se usan para dar salida a las cosas que se han inicializado en los Enter
	// Los boleanos se ajustan a False y los object se anulan
	void OnTriggerExit2D (Collider2D col) {
		if (col.tag == "Liana") {
			Liana_Pos = false;
			Liana = null;
		} 
		if (col.tag == "Cadaver") {
			Cambio_Pos = false;
			Posible_Cuerpo = null;
		} 
		if (col.gameObject.tag == "Luz") {
			Luz = null;
		}
		if (col.tag == "Muro") {
			En_Muro_Pos = false;
			Muro_Rend.enabled = true;
			Muro_Rend = null;
		}
	}
}


