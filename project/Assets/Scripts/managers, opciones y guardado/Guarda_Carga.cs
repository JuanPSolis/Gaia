using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Guarda_Carga : MonoBehaviour {

	private Gaia_Definitiva mi_Gaia;
	private Light[] Lista_Luces;
	public GameObject[] Cadaveres;



	// Funcion que crea una instancia de la clase partida guardada y rellena 
	// esta con datos necesarios para cargar partida
	private PartidaGuardada Crear_Partida_Guardada () {	
		mi_Gaia = FindObjectOfType <Gaia_Definitiva> ();
		Lista_Luces = FindObjectsOfType <Light> ();
		// Creo una instancia de la clase Partida_Guardada
		PartidaGuardada mi_Partida = new PartidaGuardada ();
		// Relleno las variables de esta nueva instancia
		// Guardo la posicion de Gaia y si esta activa
		mi_Partida.Respawn_Gaia_x = mi_Gaia.transform.position.x;
		mi_Partida.Respawn_Gaia_y = mi_Gaia.transform.position.y;
		mi_Partida.Respawn_Gaia_z = mi_Gaia.transform.position.z;
		mi_Partida.Gaia_Esta_Activa = mi_Gaia.Gaia_IsActive;
		// Si Gaia habia poseido a algun animal guardamos su nombre en un string
		if (mi_Gaia.Cuerpo_Poseido != null) {
			mi_Partida.Cuerpo_Poseido = mi_Gaia.Posible_Cuerpo.name;
		} else {
			mi_Partida.Cuerpo_Poseido = "";
		}
		// Recorro el array de los cadaveres y guardo sus posiciones
		for (int i = 0; i < Cadaveres.Length; i++) {
			mi_Partida.Posicion_Cadaveres_x.Add (Cadaveres [i].transform.position.x);
			mi_Partida.Posicion_Cadaveres_y.Add (Cadaveres [i].transform.position.y);
			mi_Partida.Posicion_Cadaveres_z = 0f;
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
		PartidaGuardada Nuevo_Guardado = Crear_Partida_Guardada ();
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
		mi_Gaia = FindObjectOfType <Gaia_Definitiva> ();
		Lista_Luces = FindObjectsOfType <Light> ();
		// Miro si existe un archivo guardado en la ubicacion dada 
		if (File.Exists (Application.persistentDataPath + "/savedata.sd")) {
			// Si existe creo un archivo binario
			BinaryFormatter bf = new BinaryFormatter ();
			// Creo un FileStream y abro el anteriormente guardado en la ubicacion en el
			FileStream Ficha_Partida = File.Open (Application.persistentDataPath + "/savedata.sd", FileMode.Open);
			// Deserializo el FileStream en el binario y lo guardo en una nueva instancia Partida_Guardada
			PartidaGuardada Ultimo_Guardado = (PartidaGuardada)bf.Deserialize (Ficha_Partida);
			// Cierro el archivo
			Ficha_Partida.Close ();

			// MONTAMOS LA ESCENA DEL JUEGO:
			// Igualamos si Gaia estaba o no activa
			mi_Gaia.Gaia_IsActive = Ultimo_Guardado.Gaia_Esta_Activa;
			// Recorro los cadaveres que hay en escena 
			for (int i = 0; i < Cadaveres.Length; i++) {
				// Cada cadaver se colocara en los puntos guardados
				Cadaveres [i].transform.position = new Vector3 (Ultimo_Guardado.Posicion_Cadaveres_x [i], Ultimo_Guardado.Posicion_Cadaveres_y [i], Ultimo_Guardado.Posicion_Cadaveres_z); 
				// Compruebo si el nombre de alguno coincide con el que hemos guardado en caso de que el jugador grabara
				// mientras controla a un cuerpo poseido. Se podria hacer despues pero hay que recorrer el array en este punto
				if (Cadaveres [i].transform.name == Ultimo_Guardado.Cuerpo_Poseido) {
					// Si coincide lo igualo al de Gaia para que pueda ejecutar la posesion desde aqui
					mi_Gaia.Posible_Cuerpo = Cadaveres [i];
				}
			}
			if (mi_Gaia.Gaia_IsActive == true) {
				// Si Gaia estaba activa se iniciara en el ultimo checkpoint guardado
				mi_Gaia.transform.position = new Vector3 (Ultimo_Guardado.Respawn_Gaia_x, Ultimo_Guardado.Respawn_Gaia_y, Ultimo_Guardado.Respawn_Gaia_z);
			} else {
				// Si Gaia no estaba activa pondre que el bool para la posicion de cambio es verdad, ajustare su posicion a la del cuerpo
				// poseido e iniciare la posesion para pasar a controlar el cuerpo poseido
				mi_Gaia.Cambio_Pos = true;
				mi_Gaia.transform.position = mi_Gaia.Posible_Cuerpo.transform.position;
				mi_Gaia.StartCoroutine ("Posesion");
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
