using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameStatus
    {
        Paused,
        Running,
        OffBorder,
        Goal,
        Over,
        ToStart,
        Wait
    }

    public enum Side
    {
        Player,
        Computer
    }

    // make game manager public static so can access this from other scripts

    public static GameManager gm;

    private Vector3 _positionOnBorder = Vector3.zero;
    public GameObject AI_Active;

    public GameObject ComGoalKeeper;
    public int ComputerScore;
    private float currentTime;
    public Text EndGameResultText;
    public Text EndGameScoreText;
    public GameObject EndGameUI;
    public GameObject Enemies;
    public GameObject Football;
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
    // setup the game
    public GameObject OnGameUI;
    public GameObject OutGameUI;

    public GameObject playAgainButtons;
    public string playAgainLevelToLoad;
    public GameObject Player;
    public GameObject PlayerGoalKeeper;

    public int PlayerScore;
    public Button QuitButton;
    public Button RestartButton;
    public Button ResumeButton;

    public float ResumeGameBallDistance = 2.0f;
    private Vector3 savedAngularVelocity;
    private Vector3 savedVelocity;
    public float startTime = 5.0f;
    public GameStatus status;
    public GameObject Teammates;

    private void Start()
    {
        RestartButton.onClick.AddListener(RestartGame);
        QuitButton.onClick.AddListener(Application.Quit);
        ResumeButton.onClick.AddListener(UnPauseGame);
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

    private void PauseGame()
    {
        var rb = Football.GetComponent<Rigidbody>();
        savedVelocity = rb.velocity;
        savedAngularVelocity = rb.angularVelocity;
        rb.isKinematic = true;
        OnGameUI.SetActive(false);
        OutGameUI.SetActive(true);
        status = GameStatus.Paused;
    }

    private void UnPauseGame()
    {
        var rb = Football.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(savedVelocity, ForceMode.VelocityChange);
        rb.AddTorque(savedAngularVelocity, ForceMode.VelocityChange);
        OnGameUI.SetActive(true);
        OutGameUI.SetActive(false);
        status = GameStatus.Running;
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
                    if (Input.GetKeyDown(KeyCode.Escape))
                        PauseGame();
                }
                break;
            case GameStatus.OffBorder:
            case GameStatus.Goal:
                OffBorderTimeLeft -= Time.deltaTime;
                if (OffBorderTimeLeft < 0)
                    ResumeGame();
                break;
            case GameStatus.Over:
                if (Input.GetKey(KeyCode.Return))
                    Application.LoadLevel("Intro");
                break;
            case GameStatus.ToStart:
                break;
            case GameStatus.Paused:
                break;
            case GameStatus.Wait:
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
            var n = _positionOnBorder + d.normalized * ResumeGameBallDistance;
            n.y = 0.25f;
            newBallPos = n;


            var z = _positionOnBorder.z;
            if (Mathf.Abs(z) > 54) //出底线
                if (z > 0) //出对面
                {

                    LastBallTouch = Side.Player; //角球


                    newBallPos = 45 * Vector3.forward;
                    var t = ComGoalKeeper.GetComponent("GoalKeeper_Script") as GoalKeeper_Script;
                    t.state = GoalKeeper_Script.GoalKeeper_State.KICK_BALL;

                }
                else //自己出
                {
                    LastBallTouch = Side.Computer;//门球

                    newBallPos = -45 * Vector3.forward;
                    var t = PlayerGoalKeeper.GetComponent("GoalKeeper_Script") as GoalKeeper_Script;
                    t.state = GoalKeeper_Script.GoalKeeper_State.KICK_BALL;
                }


            var m = _positionOnBorder - d.normalized * 2;
            m.y = 0;
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
        newBallPos.y = 0.25f;
        Football.transform.position = newBallPos;

        var lookPos = newPlayerPos - newBallPos;
        var rotation = Quaternion.LookRotation(lookPos);
        Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, rotation, Time.deltaTime);
//        Player.transform.LookAt(newBallPos);
        gm.OffBorder = false;
        _positionOnBorder = Vector3.zero;
        status = GameStatus.Wait;
        OffBorderTimeLeft = 3.0f;
        var sc = Enemies.GetComponent("EnemyManager") as EnemyManager;
        sc.ResetPosition();
        var sc2 = Teammates.GetComponent("EnemyManager") as EnemyManager;

		if (sc2) {
			sc2.ResetPosition();
		}
    }

    private void EndGame()
    {
        // game is over
        status = GameStatus.Over;
        OnGameUI.SetActive(false);
        OutGameUI.SetActive(false);

        EndGameScoreText.text = PlayerScore + " : " + ComputerScore;
        if (PlayerScore > ComputerScore)
            EndGameResultText.text = "You Win!";
        else if (PlayerScore < ComputerScore)
            EndGameResultText.text = "You Lose!";
        else
            EndGameResultText.text = "Draw!";
        EndGameUI.SetActive(true);
        // repurpose the timer to display a message to the player
        mainTimerDisplay.text = "GAME OVER";
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