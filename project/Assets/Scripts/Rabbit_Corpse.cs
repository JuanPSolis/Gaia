using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit_Corpse : Controler {
 
	// VARIABLES:

	// Velocidad por la que multiplicamos el input del eje x del vector Velocidad
	public float VelocidadMov;
	// Tiempo que tardara en alcanzar el punto algido del salto
	public float TiempoDeSalto;
	// Altura del salto
	public float AlturaSalto;
	// Aceleracion para el suavizado del eje x en suelo
	public float AceleracionSuelo;
	// Aceleracion para el suavizado del eje x en el aire
	public float AceleracionAire;
	// Gravedad generada en Start()
	float Gravedad;
	// Velocidad para el eje y del vector Velocidad al ejecutar el salto, se generara en Start()
	float VelocidadSalto;
	// Auxiliar para el suavizado de x
	float Suavizado;
	// Vector3 que recoge el input del axxis
	Vector3 Velocidad;
	// Variable para inicializar a Gaia
	Gaia_Flame mi_Llama;
	Gaia_Flame Ll;
	// Variables con componentes
	BoxCollider2D mi_BoxCol;
	SpriteRenderer mi_Rend;
	Rigidbody2D mi_Rig;
	//Boleanos como maquina de estados
	public bool Conejo_EsCadaver;
	public bool Saltando = false;
	public bool Tocando_Suelo = false;

	//---------------------------------------------------------------------------------------------

	public override void Start() {
		// La gravedad sera -2 x Altura / Tiempo^2
		Gravedad = -(2 * AlturaSalto) / Mathf.Pow (TiempoDeSalto, 2);
		// La velocidad sera el valor absoluto de la Gravedad x Tiempo
		VelocidadSalto = Mathf.Abs(Gravedad * TiempoDeSalto);
		Ll = FindObjectOfType <Gaia_Flame> ();
		mi_Llama = Ll.GetComponent <Gaia_Flame> ();
		mi_BoxCol = GetComponent <BoxCollider2D> ();
		mi_Rend = GetComponent <SpriteRenderer> ();
		mi_Rig = GetComponent <Rigidbody2D> ();
		EsCadaver ();
		base.Start ();

		print ("Gravity: " + Gravedad + "  Jump Velocity: " + VelocidadSalto);
	}

	void Update() {
		if (!Conejo_EsCadaver) {
			// Miramos si toca un suelo o techo
			if (Info_Col.Encima || Info_Col.Debajo) {
				// Si es verdad la velocidad en el eje y del vector Velocidad sera 0, eso dara mas estabilidad al ejecutar cadaframe
				Velocidad.y = 0;
			}
			// los ejes x e y del vector Velocidad se rellenan con los axxis
			Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			// Funcion de salto
			if (Input.GetKeyDown (KeyCode.Space) && Info_Col.Debajo) {
				// Se aplica la velocidad al eje y del vector Velocidad
				Velocidad.y = VelocidadSalto;
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
		}
	}

	//-----------------------------------------------------------------------------------------------

	// FUNCIONES PARA LOS SKILLS DEL PERSONAJE:

	void Desposesion () {
		// No podra abandonar su cuerpo en un salto
		if (Tocando_Suelo) {
			// Traslado a Gaia a este punto
			mi_Llama.transform.position = transform.position;
			// Ahora puedo ejecutar la funcion dentro de Gaia
			mi_Llama.StartCoroutine ("VuelveGaia");
		}
	}

	// Creo dos funciones para que puedan ser llamadas con un mensaje desde otros object 
	public void EsCadaver () {
		// Se ajustara para que el object no se pueda mover
		Conejo_EsCadaver = true;
		mi_BoxCol.isTrigger = true;
		// Con este tag sera reconocido como un posible cuerpo
		transform.tag = "Cadaver";
	}
	public void NoEsCadaver () {
		// Se ajustara para que el object se pueda mover
		Conejo_EsCadaver = false;
		mi_BoxCol.isTrigger = false;
		// Con este tag aseguramos que mientra sea object activo no sea un posible cuerpo
		transform.tag = "Gaia";
	}
	// Funcion al morir el jugador
	public void Muerte () {
		mi_Llama.mi_Game.Cargar_Partida ();
	}

	//-----------------------------------------------------------------------------------------------

	//TRIGGERS Y COLLISIONS PARA CONTROLAR CONTACTOS:

	// Busco los collider sin trigger
	void OnCollisionEnter2D (Collision2D col) {
		if (col.transform.tag == "Enemy") {
			Muerte ();
		}
	}
	// Colisiones entrantes 
	void OnTriggerEnter2D (Collider2D col) {
		// Si cae por un precipicio habra un trigger
		if (col.tag == "Muerte") {
			Debug.Log ("ahhh");
			Muerte ();
		}
	}
	// Miro que esta en contacto con el porsonaje
	void OnCollisionStay2D (Collision2D col) {
		// Si el tag del object en colision es el "Suelo" diremos que lo esta tocando y negaremos el salto
		if (col.gameObject.tag == "Suelo") {
			// En caso de que haya doble salto, habra que modificar estas lineas
			Saltando = false;
			Tocando_Suelo = true;
		}
	}
	// Miro que ha dejado de estar en contacto con el object
	void OnCollisionExit2D (Collision2D col) {
		// Si el tag es "Suelo" negaremos que este en contacto con el
		if (col.gameObject.tag == "Suelo") {
			Tocando_Suelo = false;
		}
	}
}
