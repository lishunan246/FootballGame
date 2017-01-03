using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // make game manager public static so can access this from other scripts
    public static GameManager gm;

    public int beatLevelScore = 0;

    public bool canBeatLevel = false;
    public int ComputerScore;

    private float currentTime;
    public GameObject Football;

    public bool gameIsOver;

    public GameObject gameOverScoreOutline;

    public Text mainScoreDisplay;
    public Text mainTimerDisplay;

    public AudioSource musicAudioSource;

    public GameObject nextLevelButtons;
    public string nextLevelToLoad;
    public bool OffBorder;
    private bool offBorderHandled;

    public GameObject playAgainButtons;
    public string playAgainLevelToLoad;
    public GameObject Player;

    public int PlayerScore;

    private Vector3 pos = Vector3.zero;

    // public variables
    public int score;

    public float startTime = 5.0f;

    public float OffBorderTimeLeft = 3.0f;
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
            if (canBeatLevel && score >= beatLevelScore)
            {
                // check to see if beat game
                BeatLevel();
            }
            else if (currentTime < 0)
            {
                // check to see if timer has run out
                EndGame();
            }
            else
            {
                if (OffBorder)
                {
                    if (!offBorderHandled)
                    {
                        var lastPosition = Football.gameObject.transform.position;
                        if (pos == Vector3.zero)
                            pos = lastPosition;

                        if (Mathf.Abs(pos.x) < 7.5 && Mathf.Abs(pos.y) < 4.8 && Mathf.Abs(pos.z) > 54.7)
                            if (pos.z > 0)
                                PlayerScore++;
                            else
                                ComputerScore++;
                        offBorderHandled = true;
                    }
                    OffBorderTimeLeft -= Time.deltaTime;
                    if (OffBorderTimeLeft < 0)
                    {
                        ResumeGame();
                    }

              
                        
                }
                // game playing state, so update the timer
                currentTime -= Time.deltaTime;
                mainTimerDisplay.text = currentTime.ToString("0.00");
                mainScoreDisplay.text = PlayerScore + ":" + ComputerScore;
            }
    }

    private void ResumeGame()
    {
        var v = new Vector3(0.0f, 0.25f, 0.0f);

        Football.transform.position = v;
        Football.GetComponent<Rigidbody>().Sleep();
        Player.transform.position = new Vector3(0, 1, -3);
        Player.transform.rotation.Set(0, 0, 0, 0);
        gm.OffBorder = false;
        pos = Vector3.zero;
        offBorderHandled = false;
        OffBorderTimeLeft = 3.0f;
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