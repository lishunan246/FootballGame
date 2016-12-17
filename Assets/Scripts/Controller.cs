using UnityEngine;

public class Controller : MonoBehaviour
{
    private CharacterController _myController;
    public float gravity = 9.81f;

    // public variables
    public float moveSpeed = 3.0f;

    // Use this for initialization
    private void Start()
    {
        // store a reference to the CharacterController component on this gameObject
        // it is much more efficient to use GetComponent() once in Start and store
        // the result rather than continually use etComponent() in the Update function
        _myController = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Determine how much should move in the z-direction
        var movementZ = Input.GetAxis("Vertical")*Vector3.forward*moveSpeed*Time.deltaTime;

        // Determine how much should move in the x-direction
        var movementX = Input.GetAxis("Horizontal")*Vector3.right*moveSpeed*Time.deltaTime;

        // Convert combined Vector3 from local space to world space based on the position of the current gameobject (player)
        var movement = transform.TransformDirection(movementZ + movementX);

        // Apply gravity (so the object will fall if not grounded)
        movement.y -= gravity*Time.deltaTime;

        Debug.Log("Movement Vector = " + movement);

        // Actually move the character controller in the movement direction
        _myController.Move(movement);
    }
}