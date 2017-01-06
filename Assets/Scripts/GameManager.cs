using System;
using System.Linq.Expressions;
using Boo.Lang.Environments;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameStatus
    {
        Running,
        OffBorder,
        Goal,
        Over,
        ToStart
    }
    // make game manager public static so can access this from other scripts
    public static GameManager gm;
    public Camera MainCamera;
    public int ComputerScore;
    private float currentTime;
    public GameObject Football;

    public GameStatus status;

    public GameObject gameOverScoreOutline;

    public Text mainScoreDisplay;
    public Text mainTimerDisplay;

    public AudioSource musicAudioSource;

    public GameObject nextLevelButtons;
    public string nextLevelToLoad;
    public bool OffBorder;
    

    public float OffBorderTimeLeft = 3.0f;

    public GameObject playAgainButtons;
    public string playAgainLevelToLoad;
    public GameObject Player;

    public int PlayerScore;

    private Vector3 PositionOnBorder = Vector3.zero;
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
        if (status == GameStatus.Running)
        {
            if (currentTime < 0)
            {
                // check to see if timer has run out
                EndGame();
            }
            else
            {
                if (OffBorder)
                {
                    if (status == GameStatus.Running)
                    {
                        var lastPosition = Football.gameObject.transform.position;
                        if (PositionOnBorder == Vector3.zero)
                            PositionOnBorder = lastPosition;

                        if (Mathf.Abs(PositionOnBorder.x) < 7.5 && Mathf.Abs(PositionOnBorder.y) < 4.8 && Mathf.Abs(PositionOnBorder.z) > 54.7)
                        {
                            status = GameStatus.Goal;

                            if (PositionOnBorder.z > 0)
                                PlayerScore++;
                            else
                                ComputerScore++;
                        }
                        else
                        {
                            status = GameStatus.OffBorder;
                        }
                    }
                }
                // game playing state, so update the timer
                currentTime -= Time.deltaTime;
                mainTimerDisplay.text = currentTime.ToString("0.00");
                mainScoreDisplay.text = PlayerScore + ":" + ComputerScore;
            }
        }
        else if(status==GameStatus.OffBorder||status==GameStatus.Goal)
        {
            OffBorderTimeLeft -= Time.deltaTime;
            if (OffBorderTimeLeft < 0)
                ResumeGame();
        }
    }

    private void ResumeGame()
    {
        Vector3 newBallPos;
        Vector3 newPlayerPos;
        if (status == GameStatus.Goal)
        {
            newBallPos = 0.25f*Vector3.up;
            newPlayerPos = new Vector3(0, 1, -3);
        }
        else if(status==GameStatus.OffBorder)
        {
            var d = Vector3.zero - PositionOnBorder;
            var n = PositionOnBorder + d.normalized;
            n.y = 0.25f;
            newBallPos = n;
            var m = PositionOnBorder - d.normalized*2;
            m.y = 1;
            newPlayerPos = m;
        }
        else
        {
            throw new Exception("bad status: "+status.ToString());
        }

        
        Football.GetComponent<Rigidbody>().Sleep();
        Player.GetComponent<Rigidbody>().Sleep();
        Football.transform.position = newBallPos;
        Player.transform.position = newPlayerPos;
        var lookPos = newPlayerPos- newBallPos;
        var rotation = Quaternion.LookRotation(lookPos);
        Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, rotation, Time.deltaTime);
//        Player.transform.LookAt(newBallPos);
        gm.OffBorder = false;
        PositionOnBorder = Vector3.zero;
        
        OffBorderTimeLeft = 3.0f;
        status = GameStatus.Running;
    }

    private void EndGame()
    {
        // game is over
        status=GameStatus.Over;

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