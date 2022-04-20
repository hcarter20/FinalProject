using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { setup, stacking, sleeping, gameOverLose, gameOverWin };

public class GameManager : MonoBehaviour
{
    // Singleton declaration
    public static GameManager S;
    public GameState gameState = GameState.setup;

    // The amount of time the player gets for the stacking section
    public float stackTime = 30.0f;
    // The amount of time the player gets for sleep section
    public float sleepTime = 5.0f;
    // The amount of time to transition between states (in seconds)
    public float transitionTime = 1.0f;

    // The princess character controller
    public PrincessController princess;
    // The linen closet controller
    public ClosetController closet;
    // The minion spawner
    public MinionSpawner minionSpawner;
    // The collider which defines the bed bounds
    public BoxCollider2D bedBounds;
    // The collider which defines the linen closet bounds
    public BoxCollider2D closetBounds;

    // Text UI Element that marks timer for sleep section
    public Text timerText;
    // Text UI Element that gives the princess's requirements
    public Text infoText;
    // Button which can end the stacking phase early
    public Button stackButton;

    // Used to track time left during gameplay
    private float timeLeft;
    // Boolean used to track whether the timer is counting down
    private bool isCountdown = false;

    // The level settings which control how the level is initialized
    private Level levelSettings;

    // Temp solution: tracks whether an object is trailing the mouse
    public bool hasClickable = false;

    private void Awake()
    {
        S = this;
    }

    public void Start()
    {
        if (LevelManager.S != null)
            LoadLevel(LevelManager.S.Levels[LevelManager.S.levelIndex]);
    }

    /* Called by the LevelManager: specifies which level of gameplay to initialize */
    public void LoadLevel(Level levelInfo)
    {
        Debug.Log("Loading the level " + levelInfo.LevelIndex);
        levelSettings = levelInfo;

        // Load the linen closet


        // Setup the minion spawner


        // temp: Prepare for actual gameplay
        Setup();
    }

    /* Triggered by the button on the initial intro menu */
    public void Setup()
    {
        // Initialize gameplay variables
        timerText.text = "";
        timeLeft = stackTime;

        // Start the stacking phase
        gameState = GameState.stacking;
        isCountdown = true;
        if (minionSpawner != null)
            minionSpawner.StartSpawning();
    }

    private void Update()
    {
        // Temp: Hit the escape key to exit the game
        // if (Input.GetKeyDown(KeyCode.Escape))
        //    Application.Quit();

        // Update the timer through the update method
        if (isCountdown && (gameState == GameState.stacking || gameState == GameState.sleeping))
        {
            timeLeft -= Time.deltaTime;
            DisplayTimer();

            if (timeLeft <= 0.0f)
            {
                timeLeft = -1.0f;
                isCountdown = false;

                if (gameState == GameState.stacking)
                    StartCoroutine(BeginNighttime());
                else if (gameState == GameState.sleeping)
                    StartCoroutine(Victory());
            }
        }
    }

    public void HazardSpawned(StackItem itemType)
    {
        // In future, check against the actual object,
        // for now just count down the total num required

        if (itemType != StackItem.melonslice)
        {
            levelSettings.HazardCount--;
        }
    }

    /* Triggered by UI button press directly from game */
    public void FinishStacking()
    {
        if (gameState != GameState.stacking || !isCountdown)
        {
            Debug.LogError("Stacking button activated when not currently stacking.");
            return;
        }

        isCountdown = false;
        timeLeft = -1.0f;
        DisplayTimer();

        StartCoroutine(BeginNighttime());
    }

    private IEnumerator BeginNighttime()
    {
        // TODO: Play sound effect? Some signal of the transition?

        // Stop the minions from spawning anymore
        if (minionSpawner != null)
            minionSpawner.StopSpawning();
        // Remove the button for the end of stacking
        if (stackButton.gameObject.activeInHierarchy)
            stackButton.gameObject.SetActive(false);

        // If player didn't manage to place all required items, fail early
        if (levelSettings.HazardCount > 0)
        {
            timerText.text = "Items are missing!";
            yield return new WaitForSeconds(transitionTime);
            FailLevel();
        }
        else
        {
            // Give a moment of transition from day to night
            yield return new WaitForSeconds(transitionTime);

            // Move into the night time gameplay
            princess.Activate();
            timeLeft = sleepTime;
            isCountdown = true;
            gameState = GameState.sleeping;
        }
    }

    public void FailLevel()
    {
        // Freeze the countdown timer, instantly fail level
        isCountdown = false;
        StartCoroutine(Failure());
    }

    private IEnumerator Failure()
    {
        // TODO: Play sound effect?
        gameState = GameState.gameOverLose;
        timerText.text = "You Lose";

        // Give a moemnt of transition after you win
        yield return new WaitForSeconds(transitionTime);

        // Move to game over screen?
        LevelManager.S.FailLevel();
    }

    private IEnumerator Victory()
    {
        // TODO: Play sound effect? Fanfare?
        gameState = GameState.gameOverWin;
        timerText.text = "You Win!";

        // Give a moemnt of transition after you win
        yield return new WaitForSeconds(transitionTime);

        // Move on to the level select
        LevelManager.S.PassLevel();
    }

    private void DisplayTimer()
    {
        // So the last second displays as 0:01, instead of 0:00.
        float timeDisplay = timeLeft + 1;
        float minutes = Mathf.FloorToInt(timeDisplay / 60);
        float seconds = Mathf.FloorToInt(timeDisplay % 60);
        seconds = Mathf.Max(0.0f, seconds);
        timerText.text = "Time Left: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
