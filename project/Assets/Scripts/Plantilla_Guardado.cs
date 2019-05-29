using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Plantilla_Guardado  {

	// Script para usar en el guardado de partida
	// Necesitara guardar diferentes variables
	// Mas adelante se iran añadiendo diferentes

	// Variable para registrar el nivel
	public float Nivel_Actual = 0;
	// Variables para Gaia, guardaremos su posicion
	public float Respawn_Llama_x = 0f;
	public float Respawn_Llama_y = 0f;
	// Tambien tendra que registrar el cuerpo poseido del cual registraremos como se llama
	// para compararlo despues y su posicion
	public string Cuerpo_Poseido;
	public List <float> Posicion_Cadaveres_x = new List <float> ();
	public List <float> Posicion_Cadaveres_y = new List <float> ();
	// Posiciones de los enemigos
	// Se guardara 1 si esta muerto y 0 si esta vivo
	public List <int> Rastreadores_Vivos = new List<int> ();
	public List <float> Velocidad_Rastreadores = new List<float> ();
	public List <float> Posicion_Rastreadores_x = new List <float> ();
	public List <float> Posicion_Rastreadores_y = new List <float> ();
	// Luces y hogueras encendidas
	// Se guarda un listado de enteros, 1 significa encendido y 0 apagado
	public List <int> Estado_Luces = new List <int> ();
}
