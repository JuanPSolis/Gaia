using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plantilla2 : MonoBehaviour {

	// VARIABLES:

	// Velocidad por la que multiplicamos el input del eje x del vector Velocidad
	public float VelocidadMov;
	// Tiempo que tardara en alcanzar el punto algido del salto
	public float TiempoDeSalto;
	// Altura del salto
	public float AlturaSalto;
	// Aceleracion para el suavizado del eje x en suelo
	public float AceleracionSuelo;
	// Aceleracion para el suavizado del eje x en el aire
	public float AceleracionAire;
	// Gravedad generada en Start()
	float Gravedad;
	// Velocidad para el eje y del vector Velocidad al ejecutar el salto, se generara en Start()
	float VelocidadSalto;
	// Auxiliar para el suavizado de x
	float Suavizado;
	// Vector3 que recoge el input del axxis
	Vector3 Velocidad;
	// Referencia al raycast controler
	Controller2D Control_2D;

	void Start() {
		// Referencia a el raycast controler
		Control_2D = GetComponent<Controller2D> ();
		// La gravedad sera -2 x Altura / Tiempo^2
		Gravedad = -(2 * AlturaSalto) / Mathf.Pow (TiempoDeSalto, 2);
		// La velocidad sera el valor absoluto de la Gravedad x Tiempo
		VelocidadSalto = Mathf.Abs(Gravedad * TiempoDeSalto);

		print ("Gravity: " + Gravedad + "  Jump Velocity: " + VelocidadSalto);
	}

	void Update() {
		// Miramos si toca un suelo o techo
		if (Control_2D.Info_Col.Encima || Control_2D.Info_Col.Debajo) {
			// Si es verdad la velocidad en el eje y del vector Velocidad sera 0, eso dara mas estabilidad al ejecutar cadaframe
			Velocidad.y = 0;
		}
		// los ejes x e y del vector Velocidad se rellenan con los axxis
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		// Funcion de salto
		if (Input.GetKeyDown (KeyCode.Space) && Control_2D.Info_Col.Debajo) {
			// Se aplica la velocidad al eje y del vector Velocidad
			Velocidad.y = VelocidadSalto;
		}
		// Creamos una variable con el movimiento del eje x	
		float _X = input.x * VelocidadMov;
		// Aplicamos el (input x velocidad de movimiento) al eje x del vector Velocidad ahora suavizandola con Mathf.SmoothDamp
		// La aceleracion hara que se llegue de un punto a otro del suavizado mas rapido o mas lento
		Velocidad.x = Mathf.SmoothDamp (Velocidad.x, _X, ref Suavizado, (Control_2D.Info_Col.Debajo) ? AceleracionSuelo : AceleracionAire);
		Velocidad.y += Gravedad * Time.deltaTime;
		// Llamamos a la funcion movimiento mandoles el vector3 Velocidad
		// El eje y estara formado con la gravedad o la fuerza del salto
		// El eje x estara formado por el input multiplicado por la velocidad y despues suavizado
		Control_2D.Movimiento (Velocidad * Time.deltaTime);
	}
}
