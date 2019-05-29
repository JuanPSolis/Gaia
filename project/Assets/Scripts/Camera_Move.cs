using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Move : MonoBehaviour {

	Camera esta_Cam;
	public GameObject mi_player;
	Vector2 Vel;
	public float Velocidad;
	public float _y;

	//-------------------------------------------------------------------------------------------

	void Start () {
		mi_player = GameObject.FindGameObjectWithTag ("Gaia");
		esta_Cam = gameObject.GetComponent <Camera> ();

	}

	void FixedUpdate () {
		float Suave_x = Mathf.SmoothDamp (transform.position.x, mi_player.transform.position.x, ref Vel.x, Velocidad);
		float Suave_y = Mathf.SmoothDamp (transform.position.y, mi_player.transform.position.y + _y, ref Vel.y, Velocidad); 
		transform.position = new Vector3 (Suave_x, Suave_y, transform.position.z);
	}

	//-------------------------------------------------------------------------------------------

}
