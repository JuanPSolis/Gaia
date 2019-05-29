using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointsSelector : MonoBehaviour {

	//Array de vectores para la ruta
	public Vector3[] PuntosDeRuta_Local;
	// Array con los puntos de ruta pero de manera local
	Vector3[] PuntosDeRuta_Global;
	// Velocidad de movimiento
	public float Velocidad;
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
	// Boleano por si queremos que el array de puntos globales vaya de adelante a atras o en ciclos
	public bool MovimientoCiclico;


	void Start () {
		// Inicializamos los puntos de ruta globales con la longitud de los puntos de ruta que pongamos en el inspector
		PuntosDeRuta_Global = new Vector3[PuntosDeRuta_Local.Length];
		// Rellenamos los puntos de ruta globales
		for (int i = 0; i < PuntosDeRuta_Local.Length; i++) {
			// Los puntos de ruta globales seran igual a los locales mas la posicion del transform
			PuntosDeRuta_Global [i] = PuntosDeRuta_Local [i] + transform.position;
		}
	}

	void Update () {
		
	}

	// Funcion que hace mas flexible el movimiento de llegada a un punto
	float Flex (float _X) {
		float _A = _Flex + 1f;
		return Mathf.Pow (_X, _A) / (Mathf.Pow (_X, _A) +  Mathf.Pow (1 - _X, _A));
	}

	// Funcion que calculara la velocidad entre las diferentes posiciones de ruta
	Vector3 CalculoMovimientoEnemigo () {
		// Primero vemos si no hemos pasado el tiempo de espera
		if (Time.time < TiempoParaSiguienteMovimiento) {
			// Devolvemos un movimientode 0
			return Vector3.zero;
		}
		// Recogemos el index del punto de ruta hacia al que se mueve el object
		// Dicho index sera el index del punto del que viene + 1, ya que ira al siguiente punto del array
		// Usaremos el modulo que calcula el resto para hacer que el index vuelva a 0
		Desde_Index %= PuntosDeRuta_Global.Length;
		int Hacia_Index = (Desde_Index + 1) % PuntosDeRuta_Global.Length;
		// Recogemos la distancia entre los puntos de ruta globales seleccionados con sus respectivos index
		float DistanciaEntrePuntos = Vector3.Distance (PuntosDeRuta_Global [Desde_Index], PuntosDeRuta_Global [Hacia_Index]);
		// El porcentaje entre puntos sera igual a si mismo x velocidad / de la distancia entre puntos
		// Esto hara que el porcentaje no crezca de forma progresiva si el object esta de camino a un punto lejano
		PorcentajeEntrePuntosRuta += Time.deltaTime * Velocidad / DistanciaEntrePuntos;
		// Hacemos que sea entre 0 y 1
		PorcentajeEntrePuntosRuta = Mathf.Clamp01 (PorcentajeEntrePuntosRuta);
		// Y lo flexibilizamos
		float Porcentaje_Flex = Flex (PorcentajeEntrePuntosRuta);
		// Recogemos la posicion (el porcentaje del camino recorrido) en la que esta el object entre el punto a y el punto b 
		Vector3 Nueva_Pos = Vector3.Lerp (PuntosDeRuta_Global [Desde_Index] ,PuntosDeRuta_Global [Hacia_Index], Porcentaje_Flex);
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
		// Devolvemos la nueva posicion menos la posicion del object
		// Esa sera la cantidad de movimientoque daremos al object cada frame
		return Nueva_Pos - transform.position;
	}

	// Dibujamos os puntos de rota para poder verlo y controlarlo en el editor de forma mas visual
	void OnDrawGizmos () {
		// Ponemos de condicion que las rutas no esten vacias
		if (PuntosDeRuta_Local != null) {
			// Damos color a los gizmos
			Gizmos.color = Color.red;
			// Creamos una variable para el tamaño
			float Tamaño = 0.3f;
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
