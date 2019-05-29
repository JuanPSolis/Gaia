using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Game_Manager : MonoBehaviour {

	private Llama_Jugador mi_Llama;
	private Light[] Lista_Luces;
	public GameObject[] Cadaveres;
	public Rastreator[] Enemigos;
	public GameObject Cuerpo_Jugador;


	// Funcion que crea una instancia de la clase partida guardada y rellena 
	// esta con datos necesarios para cargar partida
	private Plantilla_Guardado Crear_Partida_Guardada () {	
		mi_Llama = FindObjectOfType <Llama_Jugador> ();
		Lista_Luces = FindObjectsOfType <Light> ();
		// Creo una instancia de la clase Partida_Guardada
		Plantilla_Guardado mi_Partida = new Plantilla_Guardado ();
		// Relleno las variables de esta nueva instancia
		// Guardo la posicion de Gaia y si esta activa
		mi_Partida.Respawn_Llama_x = mi_Llama.transform.position.x;
		mi_Partida.Respawn_Llama_y = mi_Llama.transform.position.y;
		// Recorro el array de los cadaveres y guardo sus posiciones
		for (int i = 0; i < Cadaveres.Length; i++) {
			mi_Partida.Posicion_Cadaveres_x.Add (Cadaveres [i].transform.position.x);
			mi_Partida.Posicion_Cadaveres_y.Add (Cadaveres [i].transform.position.y);
		}
		// Recorro el array de enemigos y añado posicion y un numero: 1 si esta vivo y 0 si esta muerto
		for (int i = 0; i < Enemigos.Length; i++) {
			mi_Partida.Posicion_Rastreadores_x.Add (Enemigos [i].transform.position.x);
			mi_Partida.Posicion_Rastreadores_y.Add (Enemigos [i].transform.position.y);
			if (Enemigos [i].Muerto) {
				mi_Partida.Rastreadores_Vivos.Add (0);
			} else {
				mi_Partida.Rastreadores_Vivos.Add (1);
			}
		}
		// Recorro el array de luces y añado un 0 a la lista si esta apagada y un 1 si esta encendida
		for (int i = 0; i < Lista_Luces.Length; i++) {
			if (Lista_Luces [i].enabled == false) {
				mi_Partida.Estado_Luces.Add (0);
			} else {
				mi_Partida.Estado_Luces.Add (1);
			}
		}
		// Devuelvo esta instancia
		return mi_Partida;
	}

	// Con esta funcion guardaremos la instancia permanente
	public void Guardar_Partida () {
		// Ejecuto la funcion que me devuelve la instancia de Partida_Guardada rellena y la guardo en una variable
		Plantilla_Guardado Nuevo_Guardado = Crear_Partida_Guardada ();
		// Creo un archivo binario
		BinaryFormatter bf = new BinaryFormatter ();
		// Creo un archivo FileStream con una ruta en donde se guardara en el pc (la extension del archivo la elijo aqui y le doy nombre)
		FileStream Ficha_Partida = File.Create (Application.persistentDataPath + "/savedata.sd");
		// Serializo en el binario la instancia rellena en la ruta marcada
		bf.Serialize (Ficha_Partida, Nuevo_Guardado);
		// Cierro el archivo
		Ficha_Partida.Close ();
		// Avisamos que ha guardado
		Debug.Log ("Partida guardada");
	}

	// Con esta funcion cargaremos la partidda guardada
	public void Cargar_Partida () {
		mi_Llama = FindObjectOfType <Llama_Jugador> ();
		Lista_Luces = FindObjectsOfType <Light> ();
		// Miro si existe un archivo guardado en la ubicacion dada 
		if (File.Exists (Application.persistentDataPath + "/savedata.sd")) {
			// Si existe creo un archivo binario
			BinaryFormatter bf = new BinaryFormatter ();
			// Creo un FileStream y abro el anteriormente guardado en la ubicacion en el
			FileStream Ficha_Partida = File.Open (Application.persistentDataPath + "/savedata.sd", FileMode.Open);
			// Deserializo el FileStream en el binario y lo guardo en una nueva instancia Partida_Guardada
			Plantilla_Guardado Ultimo_Guardado = (Plantilla_Guardado)bf.Deserialize (Ficha_Partida);
			// Cierro el archivo
			Ficha_Partida.Close ();

			// MONTAMOS LA ESCENA DEL JUEGO:
			// Recorro los cadaveres que hay en escena 
			for (int i = 0; i < Cadaveres.Length; i++) {
				// Cada cadaver se colocara en los puntos guardados
				Cadaveres [i].transform.position = new Vector2 (Ultimo_Guardado.Posicion_Cadaveres_x [i], Ultimo_Guardado.Posicion_Cadaveres_y [i]);
			}
			mi_Llama.transform.position = new Vector2 (Ultimo_Guardado.Respawn_Llama_x, Ultimo_Guardado.Respawn_Llama_y);
			// Recorro el array de enemigos y añado posicion y un numero: 1 si esta vivo y 0 si esta muerto
			for (int i = 0; i < Enemigos.Length; i++) {
				Enemigos [i].transform.position = new Vector2 (Ultimo_Guardado.Posicion_Rastreadores_x [i], Ultimo_Guardado.Posicion_Rastreadores_y [i]);
				if (Ultimo_Guardado.Rastreadores_Vivos [i] == 0) {
					Enemigos [i].Muerto = true;
				} else if (Ultimo_Guardado.Rastreadores_Vivos [i] == 1) {
					Enemigos [i].Muerto = false;
				}
			}
			// Recorro el array de las luces de la escena
			for (int i = 0; i < Lista_Luces.Length; i++) {
				// Ire comparando cada luz del array con el int guardado en las celdas homologas
				// Si es un cero la luz estaba apagada, si es un 1 significa que estaba encendida
				if (Ultimo_Guardado.Estado_Luces [i] == 0) {
					Lista_Luces [i].enabled = false;
				} else {
					Lista_Luces [i].enabled = true;
				}
			}
			// Avisamos que ha cargado
			Debug.Log ("Partida Cargada");
		} else {
			// Avisamos de que no hay partidas guardadas
			Debug.Log ("No hay partidas");
		}
	}

	// Esta funcion borra la partida guardada
	public void Borrar_Partida () {
		// Busca la partida y si existe la borra
		if (File.Exists (Application.persistentDataPath + "/savedata.sd")) {
			File.Delete (Application.persistentDataPath + "/savedata.sd");
		}
	}
}
