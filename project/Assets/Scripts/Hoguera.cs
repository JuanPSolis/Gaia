using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoguera : MonoBehaviour {

	// Esprites para el apagado/encendido
	public Sprite Hoguera_Off;
	public Sprite Hoguera_On;
	private Light mi_Luz;
	private SpriteRenderer mi_spr;

	void Start () {
		mi_Luz = GetComponent <Light> ();
		mi_spr = GetComponent <SpriteRenderer> ();
	}

	void Update () {
		if (!mi_Luz.enabled) {
			mi_spr.sprite = Hoguera_Off;
		} else {
			mi_spr.sprite = Hoguera_On;
		}
	}
}
