using UnityEngine;
using System.Collections;

public class TumbleweedScript : MonoBehaviour {

	[Space(5)]
	[Header("Size")]
	[Range(0, 1)]
	public float SizeRandomize = 0.5f;
	private float _scale;

	[Space(5)]
	[Header ("Direction")]
	[Range (0, 180)]
	public float WindDirection = 180;
	private float _windDirection;

	[Range (0, 1)]
	public float DirectionRandomize = 1;

	private Vector3 direction = new Vector3();
	private Rigidbody PhysicalObj;

	[Space(5)]
	[Header ("Force")]
	[Range(0, 100)]
	public float WindForce = 30;
	private float _windForce;
	[Range (0, 1)]
	public float ForceRandomize = 0.5f;

	[Space(5)]
	[Header ("Interval")]
	[Range (0, 10)]
	public float ForceInterval = 2;
	private float _interval;
	[Range (0, 1)]
	public float IntervalRandomize = 0.5f;


	// Use this for initialization
	void Start () {

		_scale = Random.Range (transform.localScale.x * (1 - SizeRandomize * 0.3f), transform.localScale.x * (1 + SizeRandomize));
		transform.localScale = new Vector3 (_scale, _scale, _scale);

		PhysicalObj = GetComponent<Rigidbody> ();
		Invoke ("AddForce", 0);
	}

	void AddForce (){
		if(WindForce != 0){
		_windForce = Random.Range (WindForce * (1 - ForceRandomize), WindForce);

		_windDirection = Random.Range (WindDirection * (1 - DirectionRandomize), WindDirection);

		direction.x = Mathf.Cos (_windDirection);
		direction.z = Mathf.Sin (_windDirection);
		direction.y = 0.5f;

		_interval = Random.Range (ForceInterval * (1 - IntervalRandomize), ForceInterval);
	
		PhysicalObj.AddForce (direction * _windForce, ForceMode.Acceleration);
		Invoke ("AddForce", _interval);
	}
	}
}
