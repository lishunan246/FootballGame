using UnityEngine;

public class Shooter : MonoBehaviour
{
    public float power = 10.0f;

    // Reference to projectile prefab to shoot
    public GameObject projectile;

    // Reference to AudioClip to play
    public AudioClip shootSFX;

    // Update is called once per frame
    private void Update()
    {
        // Detect if fire button is pressed
        if (Input.GetButtonDown("Fire1"))
            if (projectile)
            {
                // Instantiante projectile at the camera + 1 meter forward with camera rotation
                var newProjectile =
                    Instantiate(projectile, transform.position + transform.forward, transform.rotation) as GameObject;

                // if the projectile does not have a rigidbody component, add one
                if (!newProjectile.GetComponent<Rigidbody>())
                    newProjectile.AddComponent<Rigidbody>();
                // Apply force to the newProjectile's Rigidbody component if it has one
                newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * power, ForceMode.VelocityChange);

                // play sound effect if set
                if (shootSFX)
                    if (newProjectile.GetComponent<AudioSource>())
                        newProjectile.GetComponent<AudioSource>().PlayOneShot(shootSFX);
                    else
                        AudioSource.PlayClipAtPoint(shootSFX, newProjectile.transform.position);
            }
    }
}