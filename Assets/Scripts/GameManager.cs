using System;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

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

    public enum Side
    {
        Player,
        Computer
    }

    // make game manager public static so can access this from other scripts
    public static GameManager gm;
    public GameObject AI_Active;
    public int ComputerScore;
    private float currentTime;
    public GameObject Football;
    public GameObject Enemies;
    public GameObject gameOverScoreOutline;

    public Side LastBallTouch;
    public Camera MainCamera;

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

    private Vector3 _positionOnBorder = Vector3.zero;
    public float startTime = 5.0f;
    public GameStatus status;
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
        switch (status)
        {
            case GameStatus.Running:
                if (currentTime < 0)
                {
                    // check to see if timer has run out
                    EndGame();
                }
                else
                {
                    if (OffBorder)
                        if (status == GameStatus.Running)
                        {
                            var lastPosition = Football.gameObject.transform.position;
                            if (_positionOnBorder == Vector3.zero)
                                _positionOnBorder = lastPosition;

                            if (Mathf.Abs(_positionOnBorder.x) < 7.5 && Mathf.Abs(_positionOnBorder.y) < 4.8 &&
                                Mathf.Abs(_positionOnBorder.z) > 54.7)
                            {
                                status = GameStatus.Goal;

                                if (_positionOnBorder.z > 0)
                                    PlayerScore++;
                                else
                                    ComputerScore++;
                            }
                            else
                            {
                                status = GameStatus.OffBorder;
                            }
                        }
                    // game playing state, so update the timer
                    currentTime -= Time.deltaTime;
                    mainTimerDisplay.text = currentTime.ToString("0.00");
                    mainScoreDisplay.text = PlayerScore + ":" + ComputerScore;
                }
                break;
            case GameStatus.OffBorder:
            case GameStatus.Goal:
                OffBorderTimeLeft -= Time.deltaTime;
                if (OffBorderTimeLeft < 0)
                    ResumeGame();
                break;
        }
    }

    private void ResumeGame()
    {
        Vector3 newBallPos;
        Vector3 newPlayerPos;
        if (status == GameStatus.Goal)
        {
            newBallPos = 0.25f * Vector3.up;
            newPlayerPos = new Vector3(0, 0, -3);
            Player.transform.position = newPlayerPos;
        }
        else if (status == GameStatus.OffBorder)
        {
            var d = Vector3.zero - _positionOnBorder;
            var n = _positionOnBorder + d.normalized;
            n.y = 0.25f;
            newBallPos = n;
            var m = _positionOnBorder - d.normalized * 2;
            m.y = 1;
            newPlayerPos = m;
            switch (gm.LastBallTouch)
            {
                case Side.Player:
                {
                    AI_Active.transform.position = newPlayerPos;
                    break;
                }
                case Side.Computer:
                {
                    newPlayerPos.y = 0;
                    Player.transform.position = newPlayerPos;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            throw new Exception("bad status: " + status);
        }


        Football.GetComponent<Rigidbody>().Sleep();
        Player.GetComponent<Rigidbody>().Sleep();
        Football.transform.position = newBallPos;

        var lookPos = newPlayerPos - newBallPos;
        var rotation = Quaternion.LookRotation(lookPos);
        Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, rotation, Time.deltaTime);
//        Player.transform.LookAt(newBallPos);
        gm.OffBorder = false;
        _positionOnBorder = Vector3.zero;
        status = GameStatus.Running;
        OffBorderTimeLeft = 3.0f;
        var sc = Enemies.GetComponent("EnemyManager") as EnemyManager;
        sc.ResetPosition();
        
    }

    private void EndGame()
    {
        // game is over
        status = GameStatus.Over;

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