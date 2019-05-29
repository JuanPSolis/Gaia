using System.Collections;
using UnityEngine;

public class Raycaster : MonoBehaviour {

	// VARIABLES:

	// Espacio que habra entre este objeto y su objeto colisionador
	public const float Espacio_Coll = .015f;
	// Contador de raycast, contra mas alto el numero mas raycast lanzara
	public int ContadorRay_H = 4;
	public int ContadorRay_V = 4;
	// Contador de raycast, contra mas alto el numero mas raycast lanzara
	[HideInInspector]
	public float EspacioRay_H;
	[HideInInspector]
	public float EspacioRay_V;
	// Capa nueva para las collisiones del raycast
	public LayerMask CapaDeColisiones;
	[HideInInspector]
	// Referencia a el collider
	public BoxCollider2D mi_Coll;
	// Referencia a la struct declarada mas abajo
	public OrigenesRaycast Origenes_Ray;

	//-------------------------------------------------------------------------------------

	public virtual void Start() {
		mi_Coll = GetComponent<BoxCollider2D> ();
		CalcularEspacioRay ();
	}

	//-------------------------------------------------------------------------------------

	// Funcion para actualizar los origenes de los raycast
	public void ActualizarRaycastOrigenes() {
		// Referencia a los bordes del box collider
		Bounds Limites = mi_Coll.bounds;
		Limites.Expand (Espacio_Coll * -2);
		// Rellenamos los origenes declarados en la struc con los minimos y 
		// maximos segun la esquina que queramos rellenar
		Origenes_Ray.Abajo_Izq = new Vector2 (Limites.min.x, Limites.min.y);
		Origenes_Ray.Abajo_Der = new Vector2 (Limites.max.x, Limites.min.y);
		Origenes_Ray.Arriba_Izq = new Vector2 (Limites.min.x, Limites.max.y);
		Origenes_Ray.Arriba_Der = new Vector2 (Limites.max.x, Limites.max.y);
	}
	// Funcion para recalcular el espacio de los rayos segun el tamaño del object
	public void CalcularEspacioRay() {
		// Referencia a los bordes del box collider
		Bounds Limites = mi_Coll.bounds;
		Limites.Expand (Espacio_Coll * -2);
		// Ajustamos los valores de los contadores a un minimo (2) y un maximo 
		ContadorRay_H = Mathf.Clamp (ContadorRay_H, 2, int.MaxValue);
		ContadorRay_V = Mathf.Clamp (ContadorRay_V, 2, int.MaxValue);
		// Calculamos el espacio entre raycast dividiendo el tamaño de cada lado 
		// del box collider entre su contador menos 1
		EspacioRay_H = Limites.size.y / (ContadorRay_H - 1);
		EspacioRay_V = Limites.size.x / (ContadorRay_V - 1);
	}

	//-------------------------------------------------------------------------------------

	// Struct declarada para contener todos las esquinas del box collider
	public struct OrigenesRaycast {
		public Vector2 Arriba_Izq, Arriba_Der;
		public Vector2 Abajo_Izq, Abajo_Der;
	}
}
