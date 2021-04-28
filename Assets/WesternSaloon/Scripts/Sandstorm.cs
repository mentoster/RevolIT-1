using UnityEngine;
using System.Collections;

public class Sandstorm : MonoBehaviour {
	[Range(0, 50)]
	public float WindForce = 5;
	private Vector3 WindDirection;

	void Start () {
		WindDirection = transform.forward;
	}

	void OnTriggerStay (Collider other){
		other.attachedRigidbody.AddForce (WindDirection * WindForce, ForceMode.Acceleration);
	}


}
