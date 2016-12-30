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

    // Reference to projectile prefab to shoot
    public GameObject projectile;

    // Reference to AudioClip to play
    public AudioClip shootSFX;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Football")
        {
            var ball = collision.gameObject;
            var f = ball.transform.position - transform.position;
            f[1] = up;

            ball.GetComponent<Rigidbody>().AddForce(f*(_kickForce+0.2f)*force);
            _kickForce = 0;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Detect if fire button is pressed
        if (Input.GetButtonDown("Fire1"))
            if (projectile)
            {
                // Instantiante projectile at the camera + 1 meter forward with camera rotation
                var newProjectile =
                    Instantiate(projectile, transform.position + transform.forward, transform.rotation);

                // if the projectile does not have a rigidbody component, add one
                if (!newProjectile.GetComponent<Rigidbody>())
                    newProjectile.AddComponent<Rigidbody>();
                // Apply force to the newProjectile's Rigidbody component if it has one
                newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward*power, ForceMode.VelocityChange);

                // play sound effect if set
                if (shootSFX)
                    if (newProjectile.GetComponent<AudioSource>())
                        newProjectile.GetComponent<AudioSource>().PlayOneShot(shootSFX);
                    else
                        AudioSource.PlayClipAtPoint(shootSFX, newProjectile.transform.position);
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