using System;
using UnityEngine;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public enum Status
    {
        Attack,
        Assist,
        Idle,
        Return
    }

    public enum Stratagy
    {
        Pass,
        Shoot,
        Goal
    }

    // Use this for initialization
    private readonly Random _rnd = new Random();

    private GameObject _ball;

    private Vector3 _tempDestination;
    public float ActiveSpeed = 3.0f;
    public Stratagy AiStratagy = Stratagy.Pass;
    public float AngleCosMax = 0.8f;
    public float BallDistance = 1.5f;
    public float CurrentSpeed;

    public Vector3 DefaultPosition;
    public float DistanceToBall;
    public float DistanceToGoal;
    public float DistanceToPlayer;
    public float NonActiveSpeed = 0.3f;
    public float PassDistance = 10.0f;
    public float PassForce = 3.0f;
    public float ShootDistance = 20.0f;
    public float ShootForce = 20.0f;
    public GameManager.Side Side = GameManager.Side.Computer;
    public Status status = Status.Idle;
    public Vector3 TargetGoal = 55 * Vector3.back;
    public float Up = 1.0f;
    public float XMax;

    private void Start()
    {
        _ball = GameObject.FindGameObjectWithTag("Football");
        _tempDestination = GetDestination();
        DefaultPosition = transform.position;
    }


    private Vector3 GetDestination()
    {
        var result = Vector3.zero;
        result[0] = _rnd.Next((int) -XMax, (int) XMax);
        result[2] = _rnd.Next(-10000, (int) TargetGoal.z);
        return result;
    }

    private void MoveTo(Vector3 pos)
    {
        pos.y = 0;
        transform.LookAt(pos);
        transform.position = Vector3.MoveTowards(transform.position, pos, CurrentSpeed * Time.deltaTime);
        //        var transformForward = pos - gameObject.transform.position;
        //        transformForward.y = 0;
        //        gameObject.transform.forward = transformForward;
        //        var rb = GetComponent<Rigidbody>();
        //        //Quaternion q=Quaternion.FromToRotation(transform.forward,transformForward);
        //        //gameObject.transform.LookAt(pos);
        //        //rb.MoveRotation(q);
        //        
        //        rb.MovePosition(gameObject.transform.position + transformForward.normalized * sp * Time.deltaTime);
    }

    private bool ShouldGoal()
    {
        return DistanceToGoal < ShootDistance || Mathf.Abs(gameObject.transform.position.y) > 40;
    }

    private void rotateRigidBodyAroundPointBy(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
    {
        var q = Quaternion.AngleAxis(angle, axis);
        rb.MovePosition(q * (rb.transform.position - origin) + origin);
        rb.MoveRotation(rb.transform.rotation * q);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (GameManager.gm.status == GameManager.GameStatus.Paused)
            return;
        UpdateStatus();

        switch (status)
        {
            case Status.Attack:
            {
                if (DistanceToBall > BallDistance + 1.1)
                {
                    MoveBehind(_tempDestination);
                }
                else
                {
                    if (ShouldGoal() ||
                        Vector3.Dot(gameObject.transform.forward.normalized,
                            (TargetGoal - gameObject.transform.position).normalized) > AngleCosMax)
                    {
                        MoveTo(_ball.transform.position);
                    }
                    else
                    {
                        var t = Vector3.Dot(gameObject.transform.forward, Vector3.right) > 0 ? 1 : -1;
                        var tt = Side == GameManager.Side.Computer ? 1 : -1;
                        var rb = gameObject.GetComponent<Rigidbody>();
                        rotateRigidBodyAroundPointBy(rb, gameObject.transform.position, Vector3.up,
                            180 * Time.deltaTime * t * tt);
                        rotateRigidBodyAroundPointBy(_ball.GetComponent<Rigidbody>(), gameObject.transform.position,
                            Vector3.up, 180 * Time.deltaTime * t * tt);
                    }
                }
                break;
            }
            case Status.Assist:
            {
                var d = GameManager.gm.LastBallTouch == Side ? TargetGoal : -TargetGoal;

                MoveTo(d * 0.8f + 0.2f * DefaultPosition);
                break;
            }
            case Status.Idle:
            {
                if ((transform.position - DefaultPosition).magnitude > 0.2)
                    MoveTo(_ball.transform.position);
                break;
            }
            case Status.Return:
                if ((transform.position - DefaultPosition).magnitude < 0.1)
                    status = Status.Idle;
                else
                    MoveTo(DefaultPosition);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (DistanceToBall < BallDistance)
            Kick();
    }


    private void UpdateStatus()
    {
        if (GameManager.gm.status == GameManager.GameStatus.OffBorder ||
            GameManager.gm.status == GameManager.GameStatus.Goal)
            status = Status.Return;
        DistanceToPlayer =
            (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).magnitude;
        AiStratagy = ShouldGoal()
            ? Stratagy.Goal
            : (DistanceToPlayer < PassDistance ? Stratagy.Shoot : Stratagy.Pass);
        DistanceToGoal = (TargetGoal - gameObject.transform.position).magnitude;
        DistanceToBall = (_ball.transform.position - gameObject.transform.position).magnitude;
        CurrentSpeed = status != Status.Idle ? ActiveSpeed : NonActiveSpeed;
        if (GameManager.gm.status == GameManager.GameStatus.Wait &&
            (gameObject != GameManager.gm.AI_Active || Side == GameManager.gm.LastBallTouch))
            if (status != Status.Idle || status != Status.Return)
                status = Status.Return;
    }

    private void MoveBehind(Vector3 tempDestination)
    {
        var d = tempDestination - _ball.transform.position;
        d.y = 0;

        var des = _ball.transform.position - d.normalized * BallDistance;
        MoveTo(des);
    }

    private void Kick()
    {
        if (GameManager.gm.status == GameManager.GameStatus.Wait)
            GameManager.gm.status = GameManager.GameStatus.Running;
        Vector3 kickDirection;

        var kickForce = (float) _rnd.NextDouble();
        Vector3 tf;
        switch (AiStratagy)
        {
            case Stratagy.Goal:
                kickDirection = (TargetGoal - gameObject.transform.position).normalized;
                kickDirection[1] = 0.5f;
                tf = kickDirection * (kickForce + 0.2f) * ShootForce;
                gameObject.GetComponent<Animation>().Play("tiro");
                break;
            case Stratagy.Shoot:
                kickDirection = gameObject.transform.forward.normalized;

                kickDirection[1] = 0.2f;
                tf = kickDirection * (kickForce + 0.2f) * ShootForce;
                gameObject.GetComponent<Animation>().Play("pass");
                break;
            default:
                kickDirection = gameObject.transform.forward;
                kickDirection[1] = 0.2f;
                tf = kickDirection * 1.0f * PassForce;
                break;
        }

        _ball.GetComponent<Rigidbody>().AddForce(tf, ForceMode.Impulse);

        GetComponent<AudioSource>().Play();

        GameManager.gm.LastBallTouch = Side;
        GetComponent<Rigidbody>().Sleep();
        _tempDestination = GetDestination();
    }
}