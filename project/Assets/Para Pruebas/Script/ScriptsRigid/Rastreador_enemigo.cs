using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rastreador_enemigo : MonoBehaviour {

	// VARIABLES:

	// Variable para controlar la velocidad
	public float Velocidad;
	public float Velocidad_Persecucion;
	// Variable para ajustar la distancia que necesita el enemigo para lanzar raycast
	public float Distacia_Persecucion;
	private float Distancia;
	public float Distancia_Vista;
	// Variable para ajustar el temporizador a 0 y cuanto tiempo esta parado el object
	private float Temporizador = 0;
	public float Fin_Temporizador;
	// Referencia a el puno de vista del object
	private GameObject mi_Presa;
	public GameObject Vista;
	public GameObject Punto_Inicio;
	public GameObject Punto_Final;
	// Mascara de capa, asi solo impactara el raycast en el jugador 
	private int Layer_Mask = 1<<8;
	// Variables con componentes
	private Rigidbody2D mi_RB;
	//private Animator mi_Anim;
	private BoxCollider2D mi_BoxCol;
	//private SpriteRenderer mi_Rend;
	private Llama_Jugador mi_Llama;
	//Boleanos como maquina de estados
	public bool Muerto = false;
	public bool En_Inicio = true;
	public bool En_Final = false;
	public bool Persiguiendo = false;
	public bool En_Alerta = true;

	//-------------------------------------------------------------------------------------------

	// Inicializo diferentes variables privadas:
	void Start () {
		transform.position = Punto_Inicio.transform.position; 
		mi_RB = GetComponent <Rigidbody2D> ();
		//mi_Anim = GetComponent <Animator> ();
		mi_BoxCol = GetComponent <BoxCollider2D> ();
		//mi_Rend = GetComponent <SpriteRenderer> ();
		mi_Llama = FindObjectOfType <Llama_Jugador> ();
	}

	//-------------------------------------------------------------------------------------------

	// En FixedUpdate (mejor para usar fisicas) compruebo si esta activa y le doy movimiento
	void FixedUpdate () {
		if (!Muerto) {
			// Mido la distancia del enemigo con el jugador
			Distancia = Vector2.Distance (transform.position, mi_Llama.transform.position);
			if (!Persiguiendo) {
				if (En_Inicio) {
					Patrulla (Punto_Final);
				} else if (En_Final) {
					Patrulla (Punto_Inicio);
				}
			} else {
				Persecucion ();
			}
		}
	}

	// En Update lanzo los rayos desde el object
	void Update () {
		if (!Muerto) {
			if (En_Alerta) {
				// Cuando esa distancia sea menor que la que hemos ajustado lanzara el raycast
				if (Distancia < Distancia_Vista) {
					// La direccion del rayo
					if (Vista.transform.position.x < transform.position.x) {
						Lanza_Rayos (Vector2.left);
					} else {
						Lanza_Rayos (Vector2.right);
					}
				}
			}
		}
	}
		

	//-------------------------------------------------------------------------------------------

	// Funcion para disparar raycast
	void Lanza_Rayos (Vector2 Direccion) {
		RaycastHit2D hit = Physics2D.Raycast (transform.position, Direccion, Distancia_Vista, Layer_Mask);
		// Si colisiona con el jugador empezara a perseguirle
		if (hit) {
			if (hit.collider.tag == "Control") {
				mi_Presa = hit.transform.gameObject;
				En_Inicio = false;
				En_Final = false;
				En_Alerta = false;
				Persiguiendo = true;
			}
		}
	}

	// Funcion para que el enemigo patrulle entre dos puntos
	void Patrulla (GameObject Objetivo) {
			// Inicio el temporizador
			Temporizador += 1f * Time.deltaTime;
			// Cuando acabe le doy movimiento hacia un lado y mantengo el objetc "vista" delante del enemigo
			if (Temporizador > Fin_Temporizador) {
			if (transform.position.x < Objetivo.transform.position.x) {
					Vista.transform.position = new Vector2 (transform.position.x + 2f, transform.position.y);
					mi_RB.velocity = new Vector2 (Velocidad, mi_RB.velocity.y);
				} else {
					Vista.transform.position = new Vector2 (transform.position.x - 2f, transform.position.y);
					mi_RB.velocity = new Vector2 (-Velocidad, mi_RB.velocity.y);
				}
			}
	}
	// Funcion para perseguir al jugador
	void Persecucion () {
		if (Distancia < Distacia_Persecucion) {
			// Inicio el temporizador
			Temporizador += 1f * Time.deltaTime;
			// Cuando acabe le doy movimiento hacia un lado
			if (Temporizador > Fin_Temporizador - 1f) {
				mi_RB.position = Vector2.MoveTowards (mi_RB.transform.position, mi_Presa.transform.position, Velocidad_Persecucion * Time.deltaTime);
			}
		} else {
			En_Alerta = true;
			Persiguiendo = false;
			mi_Presa = null;
			En_Final = true;
		}
	}

	//-------------------------------------------------------------------------------------------

	//TRIGGERS Y COLLISIONS PARA CONTROLAR CONTACTOS:

	// He colocado dos objects vacios con triggers que haran las veces de puntos de patrulla
	void OnTriggerEnter2D (Collider2D col) {
		if (!Persiguiendo) {
			// Simplemente ajusto cuando ha llegado a cada destino y pongo el temporizador a 0 otra vez
			if (col.transform.name == "Punto1") {
				En_Final = false;
				En_Inicio = true;
				Temporizador = 0f;
			} else if (col.transform.name == "Punto2") {
				En_Inicio = false;
				En_Final = true;
				Temporizador = 0f;
			}
		}
	}
}
