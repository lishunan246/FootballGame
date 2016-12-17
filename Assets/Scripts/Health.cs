using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public enum DeathAction { LoadLevelWhenDead, DoNothingWhenDead };

    public float HealthPoints = 1f;
    public float respawnHealthPoints = 1f;      //base health points

    public int NumberOfLives = 1;                   //lives and variables for respawning
    public bool IsAlive = true;

    public GameObject explosionPrefab;

    public DeathAction onLivesGone = DeathAction.DoNothingWhenDead;

    public string LevelToLoad = "";

    private Vector3 _respawnPosition;
    private Quaternion _respawnRotation;

    // Use this for initialization
    private void Start()
    {
        // store initial position as respawn location
        _respawnPosition = transform.position;
        _respawnRotation = transform.rotation;

        if (LevelToLoad == "") // default to current scene
        {
            LevelToLoad = SceneManager.GetActiveScene().name; 
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (HealthPoints <= 0)
        {               // if the object is 'dead'
            NumberOfLives--;                    // decrement # of lives, update lives GUI

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            if (NumberOfLives > 0)
            { // respawn
                transform.position = _respawnPosition;   // reset the player to respawn position
                transform.rotation = _respawnRotation;
                HealthPoints = respawnHealthPoints; // give the player full health again
            }
            else
            { // here is where you do stuff once ALL lives are gone)
                IsAlive = false;

                switch (onLivesGone)
                {
                    case DeathAction.LoadLevelWhenDead:
                        //Application.LoadLevel(LevelToLoad);
                        SceneManager.LoadScene(LevelToLoad);
                        break;

                    case DeathAction.DoNothingWhenDead:
                        // do nothing, death must be handled in another way elsewhere
                        break;
                }
                Destroy(gameObject);
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        HealthPoints = HealthPoints - amount;
    }

    public void ApplyHeal(float amount)
    {
        HealthPoints = HealthPoints + amount;
    }

    public void ApplyBonusLife(int amount)
    {
        NumberOfLives = NumberOfLives + amount;
    }

    public void UpdateRespawn(Vector3 newRespawnPosition, Quaternion newRespawnRotation)
    {
        _respawnPosition = newRespawnPosition;
        _respawnRotation = newRespawnRotation;
    }
}