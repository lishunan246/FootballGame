using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public float Speed =3.0f;
    public Vector3 TargetGoal = 55*Vector3.back;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var ball = GameObject.FindGameObjectWithTag("Football");
	    var d = ball.transform.position - gameObject.transform.position;
	    d.y = 0;
	    gameObject.transform.position += d.normalized * Speed * Time.deltaTime;
	}
}
