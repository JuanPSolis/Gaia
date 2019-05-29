using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartidaGuardada {

	// Script para usar en el guardado de partida
	// Necesitara guardar diferentes variables
	// Mas adelante se iran añadiendo diferentes

	// Variable para registrar el nivel
	public float Nivel_Actual = 0;
	// Variables para Gaia, guardaremos su posicion y si esta activa
	public float Respawn_Gaia_x = 0f;
	public float Respawn_Gaia_y = 0f;
	public float Respawn_Gaia_z = 0f;
	public bool Gaia_Esta_Activa = true;
	// Tambien tendra que registrar el cuerpo poseido del cual registraremos como se llama
	// para compaarlo despues y su posicion
	public string Cuerpo_Poseido;
	public List <float> Posicion_Cadaveres_x = new List <float> ();
	public List <float> Posicion_Cadaveres_y = new List <float> ();
	public float Posicion_Cadaveres_z = 0;
	// Posiciones de los enemigos
	//public List <Vector3> Respawn_Enemigos = new List <Vector3> ();
	// Luces y hogueras encendidas
	// Se guarda un listado de enteros, 1 significa encendido y 0 apagado
	public List <int> Estado_Luces = new List <int> ();
}
