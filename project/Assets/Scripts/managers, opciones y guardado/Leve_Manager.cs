using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leve_Manager : MonoBehaviour {

	// VARIABLES:
	public string Nivel;
	GameObject[] Muros_en_Nivel;
	GameObject[] Jaulas_en_Nivel;

	//-------------------------------------------------------------------------------------------

	void Start () {
		Muros_en_Nivel = GameObject.FindGameObjectsWithTag ("Muro");
		Jaulas_en_Nivel = GameObject.FindGameObjectsWithTag ("Jaula");
	}

	void Update () {
		
	}

	//-------------------------------------------------------------------------------------------
}
