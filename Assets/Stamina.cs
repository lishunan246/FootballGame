using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Stamina : MonoBehaviour
{
//    public float PlayerStamina = 1.0f;
    public Slider StaminaBarSlider;
    public float TimeToConsume = 5;
    public float TimeToRecover = 10;
    private RigidbodyFirstPersonController controller;
    private float defaultspeed;
    // Use this for initialization
    void Start ()
	{
        controller = gameObject.GetComponent("RigidbodyFirstPersonController") as RigidbodyFirstPersonController;
	    StaminaBarSlider.value = 1.0f;
	    defaultspeed = controller.movementSettings.ForwardSpeed;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey(KeyCode.LeftShift))
	    {
            StaminaBarSlider.value -= Time.deltaTime / TimeToConsume;
            StaminaBarSlider.value = StaminaBarSlider.value <0 ? 0 : StaminaBarSlider.value;
        }
	    else
	    {
	        StaminaBarSlider.value += Time.deltaTime / TimeToRecover;
	        StaminaBarSlider.value = StaminaBarSlider.value > 1 ? 1 : StaminaBarSlider.value;

	    }
	    if (StaminaBarSlider.value > 0.5)
	    {
            controller.movementSettings.ForwardSpeed=defaultspeed;
        }
	    else
	    {
            controller.movementSettings.ForwardSpeed = defaultspeed*StaminaBarSlider.value*2;
        }
	}
}
