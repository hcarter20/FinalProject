using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { setup, stacking, sunset, sleeping, gameOverLose, gameOverWin };

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

    // Text UI Element that marks timer for sleep section
    public Text timerText;

    // Used to track time left during gameplay
    private float timeLeft;
    // Boolean used to track whether the timer is counting down
    private bool isCountdown = false;

    // Temp: For free placement of objects with mouse
    // Tracks whether an object is trailing the mouse
    public bool hasClickable = false;
    // The collider which defines the bed bounds
    public BoxCollider2D bedBounds;
    // The collider which defines the linen closet bounds
    public BoxCollider2D closetBounds;


    private void Awake()
    {
        S = this;
    }

    public void Start()
    {
        // Load the values for this level
        stackTime = LevelManager.S.stackTimes[LevelManager.S.levelIndex];
        sleepTime = LevelManager.S.sleepTimes[LevelManager.S.levelIndex];

        if (LevelManager.S.debug)
            Setup();
    }

    /* Triggered by the button on the initial intro menu */
    public void Setup()
    {
        // Just in case we ended a previous level on fast-forward
        Time.timeScale = 1.0f;

        // Initialize gameplay variables
        timerText.text = "";
        timeLeft = stackTime;

        // Start the stacking phase
        gameState = GameState.stacking;
        isCountdown = true;
        MinionSpawner.S.StartSpawning();
        if (SkyController.S != null)
            SkyController.S.StartStacking(stackTime);

        // Start the music for the stacking phase
        AudioManager.S.Play("Stacking BGM");
    }

    private void Update()
    {
        // Update the timer through the update method
        if (isCountdown && (gameState == GameState.stacking || gameState == GameState.sleeping))
        {
            timeLeft -= Time.deltaTime;
            DisplayTimer();
            if (gameState == GameState.stacking)
                ClockController.S.UpdateClock(timeLeft, stackTime);
            else
                ClockController.S.UpdateClock(timeLeft, sleepTime);

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
        ClockController.S.UpdateClock(0.0f, stackTime);
        DisplayTimer();
        if (SkyController.S != null)
            SkyController.S.FinishStacking();

        StartCoroutine(BeginNighttime());
    }

    private IEnumerator BeginNighttime()
    {
        // TODO: Play sound effect? Some signal of the transition?
        gameState = GameState.sunset;

        // Stop the minions from spawning anymore
        MinionSpawner.S.StopSpawning();

        // Remove the button for the end of stacking
        ClockController.S.StartNightTime();

        // Check for failure in making the bed
        int validate = ClosetController.S.ValidateItems();
        if (validate == 2)
        {
            timerText.text = "Items are missing!";
            yield return new WaitForSeconds(transitionTime);
            FailLevel();
        }
        else if (validate == 1)
        {
            timerText.text = "Items aren't in bed!";
            yield return new WaitForSeconds(transitionTime);
            FailLevel();
        }
        else
        {
            // Give a moment of transition from day to night
            yield return new WaitForSeconds(transitionTime);

            // Move into the night time gameplay
            PrincessController.princess.Activate();
            timeLeft = sleepTime;
            isCountdown = true;
            gameState = GameState.sleeping;

            // Tell the sky to start moving again
            if (SkyController.S != null)
                SkyController.S.StartSleeping(sleepTime);
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
        // Just in case we ended a previous level on fast-forward
        Time.timeScale = 1.0f;

        // TODO: Play sound effect?
        gameState = GameState.gameOverLose;
        timerText.text = "You Lose";

        // Give a moment of transition after you win
        yield return new WaitForSeconds(transitionTime);

        // Move to game over screen?
        LevelManager.S.FailLevel();
    }

    private IEnumerator Victory()
    {
        // Just in case we ended a previous level on fast-forward
        Time.timeScale = 1.0f;

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
