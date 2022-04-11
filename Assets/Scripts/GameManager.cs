using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { setup, sleeping, gameOverLose, gameOverWin };

public class GameManager : MonoBehaviour
{
    // Singleton declaration
    public static GameManager S;
    public GameState gameState = GameState.setup;

    // The amount of time the player gets for sleep section
    public float sleepTime = 5.0f;
    // The princess character controller
    public PrincessController princess;

    // The collider which defines the bed bounds
    public BoxCollider2D bedBounds;
    // The collider which defines the linen closet bounds
    public BoxCollider2D closetBounds;

    // Temp: Total amount of bedding items you can place
    public int beddingLeft = 12;

    // Used to track time left during gameplay
    private float timeLeft;

    // Text UI Element that marks timer for sleep section
    public Text timerText;
    // Text UI Element that gives the princess's requirements
    public Text infoText;

    // Boolean used to track the timer
    private bool isCountdown = false;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        // Initialize gameplay variables
        timerText.text = "";
    }

    private void Update()
    {
        // Temp: Hit the escape key to exit the game
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        // Update the timer when princess is sleeping
        if (gameState == GameState.sleeping)
        {
            if (isCountdown)
            {
                timeLeft -= Time.deltaTime;
                DisplayTimer();
            }
            else
            {
                timeLeft = 0.0f;
                isCountdown = false;
                gameState = GameState.gameOverWin;
                DisplayTimer();
            }
        }
    }

    public void SpawnBedding(Bedding bedding)
    {
        beddingLeft--;

        if (beddingLeft <= 0)
        {
            princess.Activate();
            gameState = GameState.sleeping;
            timeLeft = sleepTime;
            isCountdown = true;
        }
    }

    public void FailLevel()
    {
        // Freeze the countdown timer, instantly fail level
        gameState = GameState.gameOverLose;
        Debug.Log("You lose the round.");
    }

    private void DisplayTimer()
    {
        // So the last second displays as 0:01, instead of 0:00.
        float timeDisplay = timeLeft + 1;
        float seconds = Mathf.FloorToInt(timeDisplay % 60);
        timerText.text = "Time Left: 0:" + seconds.ToString("00");
    }
}
