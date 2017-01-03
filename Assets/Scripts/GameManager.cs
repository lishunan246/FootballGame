using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // make game manager public static so can access this from other scripts
    public static GameManager gm;

    public int beatLevelScore = 0;
    public bool OffBorder = false;

    public int PlayerScore = 0;
    public int ComputerScore = 0;

    public bool canBeatLevel = false;

    private float currentTime;

    public bool gameIsOver;

    public GameObject gameOverScoreOutline;
    public GameObject Football;

    public Text mainScoreDisplay;
    public Text mainTimerDisplay;

    public AudioSource musicAudioSource;

    public GameObject nextLevelButtons;
    public string nextLevelToLoad;

    public GameObject playAgainButtons;
    public string playAgainLevelToLoad;

    // public variables
    public int score;

    public float startTime = 5.0f;

    // setup the game
    private void Start()
    {
        // set the current time to the startTime specified
        currentTime = startTime;

        // get a reference to the GameManager component for use by other scripts
        if (gm == null)
            gm = gameObject.GetComponent<GameManager>();

        // init scoreboard to 0
        mainScoreDisplay.text = "ready";

        // inactivate the gameOverScoreOutline gameObject, if it is set
        if (gameOverScoreOutline)
            gameOverScoreOutline.SetActive(false);

        // inactivate the playAgainButtons gameObject, if it is set
        if (playAgainButtons)
            playAgainButtons.SetActive(false);

        // inactivate the nextLevelButtons gameObject, if it is set
        if (nextLevelButtons)
            nextLevelButtons.SetActive(false);
    }

    // this is the main game event loop
    private void Update()
    {
        if (!gameIsOver)
            if (canBeatLevel && (score >= beatLevelScore))
            {
                // check to see if beat game
                BeatLevel();
            }
            else if (currentTime < 0)
            {
                // check to see if timer has run out
                EndGame();
            }
            else if(OffBorder)
            {
                if (Football.GetComponent<Rigidbody>().velocity.magnitude<1)
                {
                    RestartGame();
                    gm.OffBorder = false;
                }
            }
            else
            {
                // game playing state, so update the timer
                currentTime -= Time.deltaTime;
                mainTimerDisplay.text = currentTime.ToString("0.00");
                mainScoreDisplay.text = PlayerScore.ToString() + ":" + ComputerScore.ToString();
            }
    }

    private void EndGame()
    {
        // game is over
        gameIsOver = true;

        // repurpose the timer to display a message to the player
        mainTimerDisplay.text = "GAME OVER";

        // activate the gameOverScoreOutline gameObject, if it is set
        if (gameOverScoreOutline)
            gameOverScoreOutline.SetActive(true);

        // activate the playAgainButtons gameObject, if it is set
        if (playAgainButtons)
            playAgainButtons.SetActive(true);

        // reduce the pitch of the background music, if it is set
        if (musicAudioSource)
            musicAudioSource.pitch = 0.5f; // slow down the music
    }

    private void BeatLevel()
    {
        // game is over
        gameIsOver = true;

        // repurpose the timer to display a message to the player
        mainTimerDisplay.text = "LEVEL COMPLETE";

        // activate the gameOverScoreOutline gameObject, if it is set
        if (gameOverScoreOutline)
            gameOverScoreOutline.SetActive(true);

        // activate the nextLevelButtons gameObject, if it is set
        if (nextLevelButtons)
            nextLevelButtons.SetActive(true);

        // reduce the pitch of the background music, if it is set
        if (musicAudioSource)
            musicAudioSource.pitch = 0.5f; // slow down the music
    }

    // public function that can be called to update the score or time
    public void targetHit(int scoreAmount, float timeAmount)
    {
        // increase the score by the scoreAmount and update the text UI
        score += scoreAmount;
        mainScoreDisplay.text = score.ToString();

        // increase the time by the timeAmount
        currentTime += timeAmount;

        // don't let it go negative
        if (currentTime < 0)
            currentTime = 0.0f;

        // update the text UI
        mainTimerDisplay.text = currentTime.ToString("0.00");
    }

    // public function that can be called to restart the game
    public void RestartGame()
    {
        // we are just loading a scene (or reloading this scene)
        // which is an easy way to restart the level
        Application.LoadLevel(playAgainLevelToLoad);
    }

    // public function that can be called to go to the next level of the game
    public void NextLevel()
    {
        // we are just loading the specified next level (scene)
        Application.LoadLevel(nextLevelToLoad);
    }
}