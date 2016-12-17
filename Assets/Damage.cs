using UnityEngine;

public class Damage : MonoBehaviour
{
    public bool continuousDamage = false;
    public float continuousTimeBetweenHits = 0;

    public float damageAmount = 10.0f;
    public bool damageOnCollision = false;

    public bool damageOnTrigger = true;
    public float delayBeforeDestroy = 0.0f;

    public bool destroySelfOnImpact = false; // variables dealing with exploding on impact (area of effect)
    public GameObject explosionPrefab;

    private float savedTime;

    private void OnTriggerEnter(Collider collision) // used for things like bullets, which are triggers.
    {
        if (damageOnTrigger)
        {
            if ((tag == "PlayerBullet") && (collision.gameObject.tag == "Player"))
                // if the player got hit with it's own bullets, ignore it
                return;

            if (collision.gameObject.GetComponent<Health>() != null)
            {
                // if the hit object has the Health script on it, deal damage
                collision.gameObject.GetComponent<Health>().ApplyDamage(damageAmount);

                if (destroySelfOnImpact)
                    Destroy(gameObject, delayBeforeDestroy); // destroy the object whenever it hits something

                if (explosionPrefab != null)
                    Instantiate(explosionPrefab, transform.position, transform.rotation);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
        // this is used for things that explode on impact and are NOT triggers
    {
        if (damageOnCollision)
        {
            if ((tag == "PlayerBullet") && (collision.gameObject.tag == "Player"))
                // if the player got hit with it's own bullets, ignore it
                return;

            if (collision.gameObject.GetComponent<Health>() != null)
            {
                // if the hit object has the Health script on it, deal damage
                collision.gameObject.GetComponent<Health>().ApplyDamage(damageAmount);

                if (destroySelfOnImpact)
                    Destroy(gameObject, delayBeforeDestroy); // destroy the object whenever it hits something

                if (explosionPrefab != null)
                    Instantiate(explosionPrefab, transform.position, transform.rotation);
            }
        }
    }

    private void OnCollisionStay(Collision collision) // this is used for damage over time things
    {
        if (continuousDamage)
            if ((collision.gameObject.tag == "Player") && (collision.gameObject.GetComponent<Health>() != null))
                if (Time.time - savedTime >= continuousTimeBetweenHits)
                {
                    savedTime = Time.time;
                    collision.gameObject.GetComponent<Health>().ApplyDamage(damageAmount);
                }
    }
}