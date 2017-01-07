using UnityEngine;
using UnityEngine.UI;

public class Shooter : MonoBehaviour
{
    public Slider ForceBarSlider;
    private float _kickForce;

    private readonly float MaxKickForce = 1.0f;
    public float force=300.0f;
    public float up = 1.0f;
    public float power = 10.0f;

    // dribble
    public float dribble_range = 5.0f;

    // Reference to projectile prefab to shoot
    public GameObject projectile;

    // Reference to AudioClip to play
    public AudioClip shootSFX;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Football")
        {
            var ball = collision.gameObject;
            var f = gameObject.transform.forward;

            f[1] = up;

            ball.GetComponent<Rigidbody>().AddForce(f*(_kickForce+0.2f)*force);
            GameManager.gm.LastBallTouch=GameManager.Side.Player;
            _kickForce = 0;
        }
    }

    // Update is called once per frame
    private void Update()
    {

        if(Input.GetButton("Fire1")){
            GameObject ball = GameObject.FindWithTag("Football");
            float dist = Vector3.Distance (gameObject.transform.position, ball.transform.position);
            // Debug.Log (string.Format ("Distance between {0} and {1} is: {2}", gameObject, ball, dist));
            if(dist<=dribble_range){
                // stop the ball
                ball.GetComponent<Rigidbody>().Sleep();
            }
        }

        if (Input.GetButton("Jump"))
        {
            _kickForce += 1.0f*Time.deltaTime;
            _kickForce = _kickForce > MaxKickForce ? MaxKickForce : _kickForce;
        }
        else
        {
            _kickForce -= 0.5f*Time.deltaTime;
            _kickForce = _kickForce < 0 ? 0 : _kickForce;
        }

        ForceBarSlider.value = _kickForce;
    }
}