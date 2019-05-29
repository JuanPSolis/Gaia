using System.Collections;
using UnityEngine;

public class Controler : Raycaster {

	// VARIABLES:

	// Maximo angulo que puede tener el suelo para que el personaje suba cuestas
	public float MaximoAnguloSubida;
	// Maximo angulo que puede tener el suelo para descender
	public float MaximoAnguloDescenso;
	// Referencia a la struct con los boleanos de las colisiones
	public InfoColisiones Info_Col;

	//-------------------------------------------------------------------------------------

	public override void Start () {
		base.Start ();
	}

	//-------------------------------------------------------------------------------------

	// Funcion para el movimiento, recibe el vector3 del personaje
	public void Movimiento(Vector3 Velocidad) {
		ActualizarRaycastOrigenes ();
		// Y reseteamos el estado de las comisiones
		Info_Col.ResetearInfoColl ();
		// Guardamos la velocidad para compararla despues
		Info_Col.VelocidadAntigua = Velocidad;
		// Ajustamos que eje se mueve buscando que sea diferente de 0
		// Llamamos a su funcion de recoger raycast correspondiente 
		if (Velocidad.y < 0) {
			BajandoCuestas(ref Velocidad);
		}
		if (Velocidad.x != 0) {
			ColisionesHorizontales (ref Velocidad);
		}
		if (Velocidad.y != 0) {
			ColisionesVerticales (ref Velocidad);
		}
		// Movemos el transform
		transform.Translate (Velocidad);
	}

	//-------------------------------------------------------------------------------------

	// Detector de colisiones horizontales
	void ColisionesHorizontales(ref Vector3 Velocidad) {
		// Creamos dos variables, una que sea la direccion del rayo que sera el valor de signo del eje X
		// que le entre de los axis del jugador y otra la longitud que igualara el valor absoluto de
		// el movimiento en el eje X mas el espacio que hemos dado entre el collider y el object
		float Direccion_X = Mathf.Sign (Velocidad.x);
		float LongitudRaycast = Mathf.Abs (Velocidad.x) + Espacio_Coll;
		// Iniciamos un bucle
		for (int i = 0; i < ContadorRay_H; i ++) {
			// Creamos una variable (exclusiva de esta funcion) para el origen del raycast
			// Si la direccion es negativa el origen sera abajo a la izquierda, si es positiva 
			// el origen sera abajo a la derecha
			Vector2 OrigenRaycast = (Direccion_X == -1)?Origenes_Ray.Abajo_Izq:Origenes_Ray.Abajo_Der;
			// El origen se igualara a si mismo mas la direccion a la derecha por el resultado
			// del espacio de distancia de raycast horizontales por el index
			OrigenRaycast += Vector2.up * (EspacioRay_H * i);
			// Disparamos un raycast por cada paso del bucle desde el origen seleccionado
			// La direccion por defecto sera derecha aunque se multiplicara por la direccion para corregir el sentido
			// La longitud sera la elegida antes y solo actuara en la capa que hemos creado especificamente
			RaycastHit2D Golpe = Physics2D.Raycast(OrigenRaycast, Vector2.right * Direccion_X, LongitudRaycast, CapaDeColisiones);
			Debug.DrawRay(OrigenRaycast, Vector2.right * Direccion_X * LongitudRaycast,Color.red);
			if (Golpe) {
				// Creamos una variable con vecto2.angle que recoge el angulo del suelo
				// Esto determina si estamon en cuesta
				float AnguloPendiente = Vector2.Angle(Golpe.normal, Vector2.up);
				// Si el angulo es menor que el maximo
				if (i == 0 && AnguloPendiente <= MaximoAnguloSubida) {
					// Primero miramos si se estaba descendiendo una pendiente
					if (Info_Col.DescendiendoCuestas) {
						// Si es asi se pondra en false
						Info_Col.DescendiendoCuestas = false;
						// Y la Velocidad sera la que guardamos al principio del update 
						Velocidad = Info_Col.VelocidadAntigua;
					}
					// Float para distancia para que el collider se acerque totalmente al collider de la pendiente
					float DistanciaParaEmpezarSubir = 0;
					// Si el angulo que ha recogido ahora es diferente 
					// al que hemos almacenado en el ultimo movimiento del object
					if (AnguloPendiente != Info_Col.AnguloSubidoAntes) {
						// Le restamos el espacio entre colliders a la distancia que hay hasta el objeto
						// Asi se acercara lo suficiente a el suelo de la pendiente
						DistanciaParaEmpezarSubir = Golpe.distance-Espacio_Coll;
						Velocidad.x -= DistanciaParaEmpezarSubir * Direccion_X;
					}
					// Llamamos a la funcion que ajusta los parametros de subida
					SubiendoCuestas(ref Velocidad, AnguloPendiente);
					Velocidad.x += DistanciaParaEmpezarSubir * Direccion_X;
				}
				if (!Info_Col.SubiendoCuestas || AnguloPendiente > MaximoAnguloSubida) {
					// La velocidad sera igual a la distancia al objeto por el vector Direccion_X que sera positivo 
					// o negativo segun la direccion del object
					// La longitud sera la distancia entre el object colisionado y este object
					Velocidad.x = (Golpe.distance - Espacio_Coll) * Direccion_X;
					LongitudRaycast = Golpe.distance;
					// Usamos la tangente del angulo de pendiente para ajustar la velocidad de "y"
					if (Info_Col.SubiendoCuestas) {
						Velocidad.y = Mathf.Tan (Info_Col.AnguloSubidaNueva * Mathf.Deg2Rad) * Mathf.Abs(Velocidad.x);
					}
					// Ajustamos el boleano correspondiente a la direccion de donde llega el golpe
					Info_Col.Izqda = Direccion_X == -1;
					Info_Col.Derecha = Direccion_X == 1;
				}
			}
		}
	}
	// Detector de colisiones verticales
	void ColisionesVerticales(ref Vector3 Velocidad) {
		// Creamos dos variables, una que sea la direccion del rayo que sera el valor del signo del eje Y
		// que le entre de los axis del jugador y otra la longitud que igualara el valor absoluto de
		// el movimiento en el eje Y mas el espacio que hemos dado entre el collider y el object 
		float Direccion_Y = Mathf.Sign (Velocidad.y);
		float LongitudRaycast = Mathf.Abs (Velocidad.y) + Espacio_Coll;
		// Iniciamos un bucle
		for (int i = 0; i < ContadorRay_V; i ++) {
			// Creamos una variable (exclusiva de esta funcion) para el origen del raycast
			// Si la direccion es negativa el origen sera abajo a la izquierda, si es positiva 
			// el origen sera arriba
			Vector2 OrigenRaycast = (Direccion_Y == -1)?Origenes_Ray.Abajo_Izq:Origenes_Ray.Arriba_Izq;
			// El origen se igualara a si mismo mas la direccion a la derecha por el resultado
			// del espacio de distancia de raycast verticales por el index por la velocidad del eje x
			OrigenRaycast += Vector2.right * (EspacioRay_V * i + Velocidad.x);
			// Disparamos un raycast por cada paso del bucle desde el origen seleccionado
			// La direccion por defecto sera arriba aunque se multiplicara por la direccion para corregir el sentido
			// La longitud sera la elegida antes y solo actuara en la capa que hemos creado especificamente
			RaycastHit2D Golpe = Physics2D.Raycast(OrigenRaycast, Vector2.up * Direccion_Y, LongitudRaycast, CapaDeColisiones);
			Debug.DrawRay(OrigenRaycast, Vector2.up * Direccion_Y * LongitudRaycast,Color.red);
			// Si hay un golpe la velocidad sera igual a la distancia al objeto por el vector Direccion_Y que sera positivo 
			// o negativo segun la direccion del object
			// La longitud sera la distancia entre el object colisionado y este object
			if (Golpe) {
				Velocidad.y = (Golpe.distance - Espacio_Coll) * Direccion_Y;
				LongitudRaycast = Golpe.distance;
				// Ajustamos la direccion de "x" con la tangente del angulo
				if (Info_Col.SubiendoCuestas) {
					Velocidad.x = Velocidad.y / Mathf.Tan(Info_Col.AnguloSubidaNueva * Mathf.Deg2Rad) * Mathf.Sign(Velocidad.x);
				}
				// Ajustamos el boleano correspondiente a la direccion de donde llega el golpe 
				Info_Col.Debajo = Direccion_Y == -1;
				Info_Col.Encima = Direccion_Y == 1;
			}
		}
		// Si nos encontramos con otra pendiente diferente mientras subimos una
		if (Info_Col.SubiendoCuestas) {
			// Repetimos cogiendo otra vez el signo de "x", una longitud y un origen
			float Direccion_X = Mathf.Sign(Velocidad.x);
			LongitudRaycast = Mathf.Abs(Velocidad.x) + Espacio_Coll;
			Vector2 OrigenRaycast = ((Direccion_X == -1)?Origenes_Ray.Abajo_Izq:Origenes_Ray.Abajo_Der) + Vector2.up * Velocidad.y;
			// Lanzamos el raycast
			RaycastHit2D Golpe = Physics2D.Raycast(OrigenRaycast,Vector2.right * Direccion_X,LongitudRaycast,CapaDeColisiones);
			if (Golpe) {
				float AnguloPendiente = Vector2.Angle(Golpe.normal,Vector2.up);
				if (AnguloPendiente != Info_Col.AnguloSubidaNueva) {
					// significa que hay que cambiar la velocidad de "x" porque es una nueva pendiente
					Velocidad.x = (Golpe.distance - Espacio_Coll) * Direccion_X;
					// Guardamos esta nueva pendiente para contratarla mas adelante
					Info_Col.AnguloSubidaNueva = AnguloPendiente;
				}
			}
		}
	}

	//-------------------------------------------------------------------------------------

	// Funcion para ajustar parametros en la subida de cuestas
	void SubiendoCuestas(ref Vector3 Velocidad, float AnguloSubida) {
		// Float con el valor absoluto (sin signo) del movimiento de x
		float EnSubida_X = Mathf.Abs (Velocidad.x);
		// Float para acumular la velocidad del eje y subiendo una pendiente
		// La velocidad en el eje y es:
		// Seno("angulosubida" pasado a radianes) x valor absoluto (valor de eje "x")
		float EnSubida_Y = Mathf.Sin (AnguloSubida * Mathf.Deg2Rad) * EnSubida_X;
		// Si la velocidad de "y" es menor o igual que la velocidad de "y" ensubida
		if (Velocidad.y <= EnSubida_Y) {
			// Significa que no estamos saltando
			// El eje "y" es igual a la velocidad de subida
			Velocidad.y = EnSubida_Y;
			// La velocidad en el eje x es:
			// Coseno("angulosubida" pasado a radianes) x valor absoluto (valor de eje "x") x el signo de el eje "x"
			Velocidad.x = Mathf.Cos (AnguloSubida * Mathf.Deg2Rad) * EnSubida_X * Mathf.Sign (Velocidad.x);
			// actualizamos que esta en el suelo a pesar de que el movimiento "y" es positivo
			Info_Col.Debajo = true;
			Info_Col.SubiendoCuestas = true;
			Info_Col.AnguloSubidaNueva = AnguloSubida;
		}
	}
	// Funcion para mantener pegado al suelo mientras se desciende una pendiente
	void BajandoCuestas(ref Vector3 Velocidad) {
		float Direccion_X = Mathf.Sign (Velocidad.x);
		Vector2 OrigenRaycast = (Direccion_X == -1) ? Origenes_Ray.Abajo_Der : Origenes_Ray.Abajo_Izq;
		RaycastHit2D Golpe = Physics2D.Raycast (OrigenRaycast, -Vector2.up, Mathf.Infinity, CapaDeColisiones);
		if (Golpe) {
			float AnguloPendiente = Vector2.Angle(Golpe.normal, Vector2.up);
			if (AnguloPendiente != 0 && AnguloPendiente <= MaximoAnguloDescenso) {
				if (Mathf.Sign(Golpe.normal.x) == Direccion_X) {
					if (Golpe.distance - Espacio_Coll <= Mathf.Tan (AnguloPendiente * Mathf.Deg2Rad) * Mathf.Abs (Velocidad.x)) {
						float Descenso_X = Mathf.Abs(Velocidad.x);
						float Descenso_Y = Mathf.Sin (AnguloPendiente * Mathf.Deg2Rad) * Descenso_X;
						Velocidad.x = Mathf.Cos (AnguloPendiente * Mathf.Deg2Rad) * Descenso_X * Mathf.Sign (Velocidad.x);
						Velocidad.y -= Descenso_Y;
						Info_Col.AnguloSubidaNueva = AnguloPendiente;
						Info_Col.DescendiendoCuestas = true;
						Info_Col.Debajo = true;
					}
				}
			}
		}
	}

	//-------------------------------------------------------------------------------------


	// Struct con la informacion de las colisiones
	public struct InfoColisiones {
		public bool Encima, Debajo;
		public bool Izqda, Derecha;
		public bool SubiendoCuestas;
		public bool DescendiendoCuestas;
		// Float para guardar el angulo de una pendiente
		// Float para guardar el angulo de una pendiente subida
		public float AnguloSubidaNueva, AnguloSubidoAntes;
		// Vector3 para guardar la velocidad usada
		public Vector3 VelocidadAntigua;
		// Una funcion para resetear todos en falso
		public void ResetearInfoColl() {
			Encima = Debajo = false;
			Izqda = Derecha = false;
			SubiendoCuestas = false;
			DescendiendoCuestas = false;
			// Recogemos el angulo de pendiente enuna variable y devolvemos a cero la variable original
			AnguloSubidoAntes = AnguloSubidaNueva;
			AnguloSubidaNueva = 0;
		}
	}
}
