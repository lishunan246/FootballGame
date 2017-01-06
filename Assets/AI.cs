using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public float Speed =3.0f;
    public Vector3 TargetGoal = 55*Vector3.back;
    public float X_MAX;
    public float BallDistance = 1.0f;
	// Use this for initialization
    private System.Random rnd =new System.Random();
    private Vector3 _tempDestination;
    private GameObject _ball;

   public enum Stratagy 
    {
        Pass,
        Shoot
    }

    public Stratagy AI_Stratagy=Stratagy.Pass;
    public float up = 1.0f;
    public float ShootForce = 300.0f;
    public float PassForce = 10.0f;
    void Start ()
	{
        _ball = GameObject.FindGameObjectWithTag("Football");
        _tempDestination = GetDestination();
	}

    private Vector3 GetDestination()
    {
        var result = Vector3.zero;
        result[0] = rnd.Next((int)-X_MAX, (int)X_MAX);
        result[2] = rnd.Next(-10000, -55);
        return result;
    }

    private float DistanceToBall()
    {
        return (_ball.transform.position - gameObject.transform.position).magnitude;
    }

    private void MoveTo(Vector3 pos)
    {
        var transformForward = pos - gameObject.transform.position;
        transformForward.y = 0;
        gameObject.transform.forward = transformForward;
        gameObject.transform.position += transformForward.normalized * Speed * Time.deltaTime;
    }
    // Update is called once per frame
    void Update ()
	{
	    if (DistanceToBall() > BallDistance+0.2)
	    {
	        MoveBehind(_tempDestination);
	    }
	    else
	    {
            MoveTo(_ball.transform.position);
	    }
	}

    private void MoveBehind(Vector3 tempDestination)
    {
        var d = (_tempDestination - _ball.transform.position);
        d.y = 0;

        var des = _ball.transform.position - d.normalized * BallDistance;
        MoveTo(des);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Football")
        {
            _ball = collision.gameObject;
            var f = gameObject.transform.forward;

            f[1] = up;
            float kickForce = (float)rnd.NextDouble();
            Vector3 tf;
            if (AI_Stratagy == Stratagy.Shoot)
            {
                tf = f * (kickForce + 0.2f) * ShootForce;
            }
            else
            {
                f[1] = 0.01f;
                tf = f * (0.2f) * PassForce;
            }
            
            _ball.GetComponent<Rigidbody>().AddForce(tf);

            _tempDestination = GetDestination();
        }
    }
}
