using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_Jugador : MonoBehaviour {

	// VARIABLES:

	// Variable auxiliar para la forma que tiene el jugador en ese momento
	private GameObject mi_Player;
	// Variable auxiliar publica donde se incorporara la forma que quiere poseer Gaia
	public GameObject Nuevo_Cuerpo;

	// Variables publicas con las formas posibles del jugador
	public GameObject Gaia_porDefecto;
	public GameObject Conejo;

	//------------------------------------------------------------------------------------------------

	void Start () {
		//Alempezar se elegira a Gaia y se instanciara el object
		mi_Player = Instantiate (Gaia_porDefecto, transform.position, transform.rotation) as GameObject;
		// Hago que la nueva forma de Gaia sea padre del Control_Jugador
		// Asi este objetc se movera a la par del objeto que poseee el movimiento  
		// Por ejemplo ahora Gaia es padre de Control_Jugador y este ira a donde vaya Gaia
		transform.parent = mi_Player.transform;
	}

	//-----------------------------------------------------------------------------------------------

	// FUNCONES PUBLICAS PARA PODER SER LAMADAS DESDE EL RESTO DE LOS OBJECTS:

	// Funcion publica para el cambio de personaje
	// Esta funcion solo sera llamada por Gaia
	public void Posesion () {
		// Desenparento el transform para que el object "mi_Player" pueda ser eliminado/desactivado
		transform.parent = null;
		// Destruyo/desactivo el object
		Destroy (mi_Player);
		// Igualo de nuevo al nuevo object (personaje) que manda Gaia
		mi_Player = Nuevo_Cuerpo;
		// Vuelvo a emparentar el object para que arrastre este transform con su movimiento
		transform.parent = mi_Player.transform;
		// Mando un mensaje para que el nuevo personaje active su movimiento
		mi_Player.SendMessage ("NoEsCadaver");
	}

	// Funcion para volver a ser Gaia
	// Esta funcion podra ser llamada por el resto de personajes
	public void VuelveGaia () {
		// Desenparento el transform para que el object no arrastre a este transform
		transform.parent = null;
		// Mando un mensaje para que el personaje que abandonas ejecute la funcion de abandonar el movimiento
		mi_Player.SendMessage ("EsCadaver");
		// Instancio/activo el transform de Gaia
		mi_Player = Instantiate (Gaia_porDefecto, transform.position, transform.rotation) as GameObject;
		// Vuelvo a emparentar el objetc "mi_Player" para que arrastre a este transform
		transform.parent = mi_Player.transform;
		// Anulo el cuerpoenviado por Gaia momentos antes para que pueda volver a igualarlo despues
		// con cualquiera de los personajes
		Nuevo_Cuerpo = null;
	}
}
