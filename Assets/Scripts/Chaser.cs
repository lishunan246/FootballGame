using UnityEngine;

//[RequireComponent(typeof(CharacterController))]

public class Chaser : MonoBehaviour
{
    public float interval = 2.0f;
    public float minDist = 1f;
    public Rigidbody rb;

    public float force = 20.0f;
    public Transform target;

    private float time;

    // Use this for initialization
    private void Start()
    {
        time = 0;
        rb = GetComponent<Rigidbody>();
        // if no target specified, assume the player
        if (target == null)
            if (GameObject.FindWithTag("Player") != null)
                target = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (target == null)
            return;

        // face the target
        transform.LookAt(target);

        //get the distance between the chaser and the target
        var distance = Vector3.Distance(transform.position, target.position);

        //so long as the chaser is farther away than the minimum distance, move towards it at rate speed.
        if (time > interval)
        {
            if (distance > minDist)
                rb.AddForce(transform.forward*force);

            time = 0;
        }
        time += Time.deltaTime;
        //transform.position += transform.forward * speed * Time.deltaTime;
    }

    // Set the target of the chaser
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}