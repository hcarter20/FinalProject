using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockController : MonoBehaviour
{
    // Singleton Declaration
    public static ClockController S;

    // The Clock hand UI transform
    public RectTransform hand;

    // The time control buttons
    public Button pauseButton;
    public Button fastForwardButton;
    public Button skipButton;

    // For the fastForward sprites
    private Image fastButtonImage;
    public Sprite normalSpeed, fastSpeed;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        fastButtonImage = fastForwardButton.GetComponent<Image>();
    }

    public void UpdateClock(float timeLeft, float totalTime)
    {
        float angle = Mathf.LerpUnclamped(0.0f, 359.0f, timeLeft / totalTime);
        hand.localEulerAngles = new Vector3(0.0f, 0.0f, angle);
    }

    /* The methods called by the time control buttons */
    public void PressPause()
    {
        if (!InGameMenu.GamePaused) 
            InGameMenu.menu.Pause();
        else
            InGameMenu.menu.Resume();
    }

    public void PressFastForward()
    {
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 2.0f;
            fastButtonImage.sprite = normalSpeed;
        }
        else
        {
            Time.timeScale = 1.0f;
            fastButtonImage.sprite = fastSpeed;
        }
    }

    public void PressSkip()
    {
        // Call method in game manager for the finished UI button
        GameManager.S.FinishStacking();
    }

    /* Accessed from another class */
    public void StartNightTime()
    {
        // When night time starts (princess is sleeping), disable skip function
        skipButton.interactable = false;
    }
}
