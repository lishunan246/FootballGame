using UnityEngine;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public enum Stratagy
    {
        Pass,
        Shoot
    }

    public bool isActive = false;

    // Use this for initialization
    private readonly Random rnd = new Random();

    private GameObject _ball;
    private Vector3 _tempDestination;
    public float AngleCosMax = 0.8f;
    public Stratagy AI_Stratagy = Stratagy.Pass;
    public float BallDistance = 1.5f;
    public float PassForce = 10.0f;
    public float ShootForce = 300.0f;
    public float ShootDistance = 20.0f;
    public GameManager.Side side = GameManager.Side.Computer;
    public float ActiveSpeed = 3.0f;
    public float NonActiveSpeed = 0.3f;
    public Vector3 TargetGoal = 55 * Vector3.back;
    public float up = 1.0f;
    public float X_MAX;
    public float distanceToGoal;
    public float distanceToPlayer;
    public float PassDistance = 10.0f;

    private void Start()
    {
        _ball = GameObject.FindGameObjectWithTag("Football");
        _tempDestination = GetDestination();
    }

    private Vector3 GetDestination()
    {
        var result = Vector3.zero;
        result[0] = rnd.Next((int) -X_MAX, (int) X_MAX);
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
        var rb = gameObject.GetComponent<Rigidbody>();
        var sp = isActive ? ActiveSpeed : NonActiveSpeed;
        rb.MovePosition(gameObject.transform.position + transformForward.normalized * sp * Time.deltaTime);
    }

    private bool NearTargetGoal()
    {
        
        return distanceToGoal < ShootDistance;
    }

    private void rotateRigidBodyAroundPointBy(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);
        rb.MovePosition(q * (rb.transform.position - origin) + origin);
        rb.MoveRotation(rb.transform.rotation * q);
    }

    // Update is called once per frame
    private void Update()
    {
        distanceToPlayer = (GameObject.FindGameObjectWithTag("Player").transform.position-transform.position).magnitude;
        AI_Stratagy = distanceToPlayer < PassDistance ? Stratagy.Shoot : Stratagy.Pass; 
        distanceToGoal = (TargetGoal - gameObject.transform.position).magnitude;
        if (DistanceToBall() > BallDistance + 0.2)
        {
            MoveBehind(_tempDestination);
        }
        else
        {
            if (NearTargetGoal()||Vector3.Dot(gameObject.transform.forward.normalized, (TargetGoal - gameObject.transform.position).normalized) > AngleCosMax)
            {
                MoveTo(_ball.transform.position);
            }
            else
            {
                var t = Vector3.Dot(gameObject.transform.forward, Vector3.right)>0?1:-1;
                var rb = gameObject.GetComponent<Rigidbody>();
                rotateRigidBodyAroundPointBy(rb, gameObject.transform.position, Vector3.up, 180 * Time.deltaTime * t);
                rotateRigidBodyAroundPointBy(_ball.GetComponent<Rigidbody>(), gameObject.transform.position, Vector3.up, 180 * Time.deltaTime * t);
           
            }
        }
    }

    private void MoveBehind(Vector3 tempDestination)
    {
        var d = _tempDestination - _ball.transform.position;
        d.y = 0;

        var des = _ball.transform.position - d.normalized * BallDistance;
        MoveTo(des);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Football")
        {
            _ball = collision.gameObject;
            var distance = (_ball.transform.position - gameObject.transform.position).magnitude;
            
            var kickDirection = gameObject.transform.forward;

            kickDirection[1] = up;
            var kickForce = (float) rnd.NextDouble();
            Vector3 tf;
            if (distanceToGoal<ShootDistance)
            {
                kickDirection = (TargetGoal - gameObject.transform.position).normalized;
                kickDirection[1] = up;
                tf = kickDirection * (kickForce + 0.2f) * ShootForce;
            }
            else if (AI_Stratagy==Stratagy.Shoot)
            {
                tf = kickDirection * (kickForce + 0.2f) * ShootForce;
                
            }
            else
            {
                kickDirection[1] = 0.5f;
                tf = kickDirection * 1.0f * PassForce;
            }

            _ball.GetComponent<Rigidbody>().AddForce(tf);
            GameManager.gm.LastBallTouch = side;
            _tempDestination = GetDestination();
        }
    }
}