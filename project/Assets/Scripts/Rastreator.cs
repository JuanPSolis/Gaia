using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rastreator : MonoBehaviour {

	// VARIABLES:

	// Velocidades normal y cuando esta en persecucion
	public float Velocidad_Patrulla;
	public float Velocidad_Persecucion;
	// Vector3 que rellenamos para el movimiento
	Vector3 Velocidad;
	// Gravedad
	public float Gravedad;
	// Variable para ajustar la distancia que necesita el enemigo para lanzar raycast
	float Distacia_Persecucion;
	// Distancia a la que esta la presa
	float Distancia;
	// Distancia de vision del enemigo
	public float Distancia_Vista;
	//Array de vectores para la ruta
	public Vector3[] PuntosDeRuta_Local;
	// Array con los puntos de ruta pero de manera local
	Vector3[] PuntosDeRuta_Global;
	// Tiempo que espera en cada punto
	public float TiempoDeEspera;
	// Tiempo para comparar cuando a de moverse hacia el siguiente punto
	float TiempoParaSiguienteMovimiento;
	// Auxiliar para la flexibilidad del movimiento ajustandolo entre 0 y 2
	[Range (0, 2f)]
	public float _Flex;
	// Auxiliar para el index de los puntos de ruta, se usara en el calculo del objetivo
	int Desde_Index;
	// Porcentaje del movimiento hacia el objetivo, se usara para ajustar la velocidad de movimiento por distancia
	float PorcentajeEntrePuntosRuta;
	float PorcentajeHaciaPresa;
	// Mascara de capa, asi solo impactara el raycast en el jugador 
	int Layer_Mask = 1<<8;
	// Auxiliar para el suavizado de x
	float Suavizado;
	// Referencia a el punto de vista del object
	float Vista;
	// Referencia al manager del juego
	Game_Manager mi_Manager;
	Game_Manager G_M;
	// Referencia a la pposicion del object
	Vector2 mi_Posicion;
	// Variable para asociar la presa cuando es vista por el object
	GameObject mi_Presa;
	// Referencia al player
	GameObject mi_Llama;
	// Referencia al raycast controler
	Controler Control_2D;
	//Boleanos como maquina de estados
	public bool Muerto;
	public bool Persiguiendo;
	public bool Esperando;
	public bool Regresando;
	public bool En_Alerta;
	// Boleano por si queremos que el array de puntos globales vaya de adelante a atras o en ciclos
	public bool MovimientoCiclico;

	//-------------------------------------------------------------------------------------------

	// Inicializo diferentes variables privadas:
	void Start () {
		// Inicializamos el game manager
		G_M = FindObjectOfType <Game_Manager> ();
		mi_Manager = G_M.GetComponent <Game_Manager> ();
		En_Alerta = true;
		// La distancia para que persiga sera la misma a la que vea al jugador
		Distacia_Persecucion = Distancia_Vista;
		// Referencia a el raycast controler
		Control_2D = GetComponent<Controler> ();
		mi_Llama = GameObject.FindGameObjectWithTag ("Gaia");
		// Inicializamos los puntos de ruta globales con la longitud de los puntos de ruta que pongamos en el inspector
		PuntosDeRuta_Global = new Vector3[PuntosDeRuta_Local.Length];
		// Rellenamos los puntos de ruta globales
		for (int i = 0; i < PuntosDeRuta_Local.Length; i++) {
			// Los puntos de ruta globales seran igual a los locales mas la posicion del transform
			PuntosDeRuta_Global [i] = PuntosDeRuta_Local [i] + transform.position;
		}
	}

	//-------------------------------------------------------------------------------------------

	// En Update lanzo los rayos desde el object
	void Update () {
			if (!Muerto) {
				if (En_Alerta) {
					// Miramos si toca un suelo o techo
					if (Control_2D.Info_Col.Encima || Control_2D.Info_Col.Debajo) {
						// Si es verdad la velocidad en el eje y del vector Velocidad sera 0, eso dara mas estabilidad al ejecutar cadaframe
						Velocidad.y = 0;
					} else {
					Velocidad.y = Gravedad * Time.fixedDeltaTime;
					}
					Velocidad.x = Patrulla ();
					Control_2D.Movimiento (Velocidad);
				}
				if (Esperando) {
				// Miramos si toca un suelo o techo
				if (Control_2D.Info_Col.Encima || Control_2D.Info_Col.Debajo) {
					// Si es verdad la velocidad en el eje y del vector Velocidad sera 0, eso dara mas estabilidad al ejecutar cadaframe
					Velocidad.y = 0;
				} else {
					Velocidad.y = Gravedad * Time.fixedDeltaTime;
				}
				Velocidad.x = Espera ();
				Control_2D.Movimiento (Velocidad);
				}
				if (Persiguiendo) {
				// Miramos si toca un suelo o techo
				if (Control_2D.Info_Col.Encima || Control_2D.Info_Col.Debajo) {
					// Si es verdad la velocidad en el eje y del vector Velocidad sera 0, eso dara mas estabilidad al ejecutar cadaframe
					Velocidad.y = 0;
				} else {
					Velocidad.y = Gravedad * Time.deltaTime;
				}
				Velocidad.x = Persecucion ();
				Control_2D.Movimiento (Velocidad);
				}
				if (Regresando) {
					// Miramos si toca un suelo o techo
					if (Control_2D.Info_Col.Encima || Control_2D.Info_Col.Debajo) {
						// Si es verdad la velocidad en el eje y del vector Velocidad sera 0, eso dara mas estabilidad al ejecutar cadaframe
						Velocidad.y = 0;
					} else {
					Velocidad.y = Gravedad * Time.fixedDeltaTime;
					}
					Velocidad.x = Regreso ();
					Control_2D.Movimiento (Velocidad);
				}
			}
		if (!Muerto) {
			// Mido la distancia del enemigo con el jugador
			Distancia = Vector2.Distance (transform.position, mi_Manager.Cuerpo_Jugador.transform.position);
			if (En_Alerta) {
				// Cuando esa distancia sea menor que la que hemos ajustado lanzara el raycast
				if (Distancia < Distancia_Vista) {
					// La direccion del rayo
					if (Vista < transform.position.x) {
						Lanza_Rayos (Vector2.left);
					} else {
						Lanza_Rayos (Vector2.right);
					}
				}
			}
		}
	}
		
	//-------------------------------------------------------------------------------------------

	// Funcion que hace mas flexible el movimiento de llegada a un punto
	float Flex (float _X) {
		float _A = _Flex + 1f;
		return Mathf.Pow (_X, _A) / (Mathf.Pow (_X, _A) +  Mathf.Pow (1 - _X, _A));
	}
	// Funcion que calculara la velocidad entre las diferentes posiciones de ruta
	float Patrulla () {
		// Primero vemos si no hemos pasado el tiempo de espera
		if (Time.time < TiempoParaSiguienteMovimiento) {
			// Devolvemos un movimientode 0
			return Vector2.zero.x;
		}
		// Recogemos el index del punto de ruta hacia al que se mueve el object
		// Dicho index sera el index del punto del que viene + 1, ya que ira al siguiente punto del array
		// Usaremos el modulo que calcula el resto para hacer que el index vuelva a 0
		Desde_Index %= PuntosDeRuta_Global.Length;
		int Hacia_Index = (Desde_Index + 1) % PuntosDeRuta_Global.Length;
		// Recogemos la distancia entre los puntos de ruta globales seleccionados con sus respectivos index
		float DistanciaEntrePuntos = Vector2.Distance (PuntosDeRuta_Global [Desde_Index], PuntosDeRuta_Global [Hacia_Index]);
		// El porcentaje entre puntos sera igual a si mismo x velocidad / de la distancia entre puntos
		// Esto hara que el porcentaje no crezca de forma progresiva si el object esta de camino a un punto lejano
		PorcentajeEntrePuntosRuta += Time.fixedDeltaTime * Velocidad_Patrulla/ DistanciaEntrePuntos;
		// Hacemos que sea entre 0 y 1
		PorcentajeEntrePuntosRuta = Mathf.Clamp01 (PorcentajeEntrePuntosRuta);
		// Y lo flexibilizamos
		float Porcentaje_Flex = Flex (PorcentajeEntrePuntosRuta);
		// Recogemos la posicion (el porcentaje del camino recorrido) en la que esta el object entre el punto a y el punto b 
		Vector2 Nueva_Pos = Vector2.Lerp (PuntosDeRuta_Global [Desde_Index] ,PuntosDeRuta_Global [Hacia_Index], Porcentaje_Flex);
		// Si el porcetaje llega a 1 significa que ha llegado al objetivo
		if (PorcentajeEntrePuntosRuta >= 1) {
			// Lo igualamos a 0 para que vuelva a contar desde el principio
			PorcentajeEntrePuntosRuta = 0;
			// Sumamos uno al index
			Desde_Index ++;
			// Si el movimiento no es ciclico:
			if (!MovimientoCiclico) {
				// Ahora queremos que el index no se salga de la extension del array
				if (Desde_Index >= PuntosDeRuta_Global.Length - 1) {
					// Lo igualamos a 0 para que empiece por el primer puesto del array
					Desde_Index = 0;
					// Revertimos el conjunto de puntos
					System.Array.Reverse (PuntosDeRuta_Global);
				}
			}
			TiempoParaSiguienteMovimiento = Time.time + TiempoDeEspera;
		}
		// Mantenemos el punto de vista para marcar hacia donde esta mirando 
		if (transform.position.x < PuntosDeRuta_Global [Hacia_Index].x) {
			Vista = transform.position.x + Distancia_Vista;
		} else {
			Vista = transform.position.x - Distancia_Vista;
		}
		// Devolvemos la nueva posicion menos la posicion del object
		// Esa sera la cantidad de movimientoque daremos al object cada frame
		return Nueva_Pos.x - transform.position.x;
	}
	// Funcion para perseguir al jugador
	float Persecucion () {
		if (Distancia < Distacia_Persecucion && (mi_Presa.transform.position.y < transform.position.y + 1f && mi_Presa.transform.position.y > transform.position.y -1f)) {
			// Primero vemos si no1 hemos pasado el tiempo de espera
			if (Time.time < TiempoParaSiguienteMovimiento) {
				// Devolvemos un movimientode 0
				return Vector2.zero.x;
			}
			Vector2 mi_Posicion = transform.position;
			Vector2 Posicion_De_Presa = mi_Presa.transform.position;
			Vector2 Nueva_Pos = Vector2.MoveTowards (mi_Posicion, Posicion_De_Presa, Velocidad_Persecucion);
			// Mantenemos el punto de vista para marcar hacia donde esta mirando 
			if (transform.position.x < Posicion_De_Presa.x) {
				Vista = transform.position.x + Distancia_Vista;
			} else {
				Vista = transform.position.x - Distancia_Vista;
			}
			return Nueva_Pos.x - transform.position.x;
		} else {
			mi_Presa = null;
			Persiguiendo = false;
			Esperando = true;
			TiempoParaSiguienteMovimiento = Time.time + TiempoDeEspera;
			return Vector2.zero.x;
		}
	}
	// Funcion para que el enemigo espere entre la persecucion y el regreso
	float Espera () {
		if (Time.time < TiempoParaSiguienteMovimiento) {
			return Vector2.zero.x;
		} else {
			Esperando = false;
			mi_Posicion = transform.position;
			TiempoParaSiguienteMovimiento = Time.time + TiempoDeEspera;
			Regresando = true;
		}
		return Vector2.zero.x;
	}
	// Funcion para que el personaje regrese al punto de inicio
	float Regreso () {
		// Primero vemos si no hemos pasado el tiempo de espera
		if (Time.time < TiempoParaSiguienteMovimiento) {
			// Devolvemos un movimientode 0
			return Vector2.zero.x;
		}
		// Recogemos la distancia entre los puntos de ruta globales seleccionados con sus respectivos index
		float DistanciaEntrePuntos = Vector2.Distance (mi_Posicion, PuntosDeRuta_Global [0]);
		// El porcentaje entre puntos sera igual a si mismo x velocidad / de la distancia entre puntos
		// Esto hara que el porcentaje no crezca de forma progresiva si el object esta de camino a un punto lejano
		PorcentajeEntrePuntosRuta += Time.fixedDeltaTime * Velocidad_Patrulla / DistanciaEntrePuntos;
		// Hacemos que sea entre 0 y 1
		PorcentajeEntrePuntosRuta = Mathf.Clamp01 (PorcentajeEntrePuntosRuta);
		// Y lo flexibilizamos
		float Porcentaje_Flex = Flex (PorcentajeEntrePuntosRuta);
		// Recogemos la posicion (el porcentaje del camino recorrido) en la que esta el object y el punto de patrulla al que se dirigia antes
		Vector2 Nueva_Pos = Vector2.Lerp (mi_Posicion, PuntosDeRuta_Global [0], Porcentaje_Flex);
		// Si el porcetaje llega a 1 significa que ha llegado al objetivo
		if (PorcentajeEntrePuntosRuta >= 1) {
			PorcentajeEntrePuntosRuta = 0;
			Regresando = false;
			En_Alerta = true;
			TiempoParaSiguienteMovimiento = Time.time + TiempoDeEspera;
			return Vector2.zero.x;
		} else {
			// Mantenemos el punto de vista para marcar hacia donde esta mirando 
			if (transform.position.x < PuntosDeRuta_Global [0].x) {
				Vista = transform.position.x + Distancia_Vista;
			} else {
				Vista = transform.position.x - Distancia_Vista;
			}
		}
		// Devolvemos la nueva posicion menos la posicion del object
		// Esa sera la cantidad de movimientoque daremos al object cada frame
		return Nueva_Pos.x - transform.position.x;
	}
	// Funcion para disparar raycast
	void Lanza_Rayos (Vector2 Direccion) {
		RaycastHit2D hit = Physics2D.Raycast (transform.position, Direccion, Distancia_Vista, Layer_Mask);
		Debug.DrawRay (transform.position, Direccion, Color.blue, 5f);
		// Si colisiona con el jugador empezara a perseguirle
		if (hit) {
			if (hit.collider.tag == "Gaia") {
				mi_Presa = hit.transform.gameObject;
				En_Alerta = false;
				mi_Posicion = transform.position;
				Persiguiendo = true;
			}
		}
	}
	// Dibujamos os puntos de rota para poder verlo y controlarlo en el editor de forma mas visual
	void OnDrawGizmos () {
		// Creamos una variable para el tamaño
		float Tamaño = 0.3f;
		// Damos color a los gizmos
		Gizmos.color = Color.red;
		// Dibujamos el punto para que se pueda ver hasta donde llega la vista del enemigo
		Vector3 PuntoDeVista = new Vector3 (Vista, transform.position.y, 0f);
		Gizmos.DrawWireSphere (PuntoDeVista, Tamaño);
		// Ponemos de condicion que las rutas no esten vacias
		if (PuntosDeRuta_Local != null) {
			// Abrimos un bucle
			for (int i = 0; i < PuntosDeRuta_Local.Length; i++) {
				// Creamos una variable para contener los puntos de ruta pero esta vez con coordenadas globales
				// Sera el punto de ruta local + la posicion del transform  
				// Cuando estemos con la aplicacion en marcha se usaran los puntos globales antes especificados
				// Asi cuando estemos en el editor los puntos de ruta se moveran con el jugador 
				// Mientras que en marcha se fijaran en una posicion
				Vector3 PuntosDeRuta_G = (Application.isPlaying)? PuntosDeRuta_Global[i] : PuntosDeRuta_Local [i] + transform.position;
				// Dibujamos los gizmos en forma de cruz, uno en vertical y otro en horizontal
				Gizmos.DrawLine (PuntosDeRuta_G - Vector3.up * Tamaño, PuntosDeRuta_G + Vector3.up * Tamaño);
				Gizmos.DrawLine (PuntosDeRuta_G - Vector3.left * Tamaño, PuntosDeRuta_G + Vector3.left * Tamaño);
			}
		}
	}
}
