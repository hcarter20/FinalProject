using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    // Singleton declaration
    public static TransitionManager S;

    // The animator in control of the page flip animation
    public Animator flipAnimator;
    // For convenience, manually set how long the page flip takes
    public float pageFlipTime = 1.0f;

    // The screens for each chapter select
    public List<InGameScreen> Screens;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        // Set the current level's screen to be active
        for (int i = 0; i < Screens.Count; i++)
            Screens[i].gameObject.SetActive(i == LevelManager.S.levelIndex);
    }

    public void PrevPage(int currIndex)
    {
        if (currIndex == 0)
        {
            // Prev of the first chapter is just title menu
            LevelManager.S.ReturnToTitle();
        }
        else
        {
            // Animate the current menu fading out, page flipping back, new menu fading in
            StartCoroutine(FlipPage(currIndex, currIndex - 1, false));
        }
    }

    public void NextPage(int currIndex)
    {
        if (currIndex < LevelManager.S.levelIndex)
        {
            // Animate the current menu fading out, page flipping back, new menu fading in
            StartCoroutine(FlipPage(currIndex, currIndex + 1, true));
        }
        else
        {
            // Load the gameplay level
            LevelManager.S.NextLevel();
        }
    }

    private IEnumerator FlipPage(int curr, int next, bool forward)
    {
        // Pause for 1 second to allow for button fade out animation
        Screens[curr].HideScreen();
        yield return new WaitForSecondsRealtime(1.0f);
        Screens[curr].gameObject.SetActive(false);

        // Animate the page flipping
        if (forward)
            flipAnimator.Play("FlipPage");
        else
            flipAnimator.Play("FlipPageBack");
        AudioManager.S.Play("FlipPage");
        yield return new WaitForSecondsRealtime(pageFlipTime);

        // Trigger the button fade in animation
        Screens[next].gameObject.SetActive(true);
    }
}
