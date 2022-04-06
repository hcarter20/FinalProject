using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { setup, sleeping, gameOver };

public class GameManager : MonoBehaviour
{
    // Singleton declaration
    public static GameManager S;
    public GameState gameState = GameState.setup;

    // Text UI Element that marks timer for sleep section
    public Text timerText;
    // The amount of time the player gets for sleep section
    public float sleepTime = 5.0f;
    // The princess character controller
    public PrincessController princess;
    // The collider which defines the bed bounds
    public BoxCollider2D bedBounds;

    // Used to track time left and bedding left during gameplay
    private int beddingLeft;
    public float timeLeft;

    // Coroutine which updates the timer
    private Coroutine timerCoroutine;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        // Initialize gameplay variables
        Clickable[] allClickable = FindObjectsOfType<Clickable>();
        beddingLeft = allClickable.Length;
    }

    public void SpawnBedding()
    {
        beddingLeft--;

        if (beddingLeft == 0)
        {
            princess.Activate();
            gameState = GameState.sleeping;
            timerCoroutine = StartCoroutine(SleepCountdown());
        }
    }

    private IEnumerator SleepCountdown()
    {
        timeLeft = sleepTime;

        while (timeLeft > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            timeLeft -= 1.0f;
            timerText.text = "Time Left: 0:0" + Mathf.FloorToInt(timeLeft).ToString("0");
        }

        gameState = GameState.gameOver;
        timerText.text = "You Win!";
    }
}
