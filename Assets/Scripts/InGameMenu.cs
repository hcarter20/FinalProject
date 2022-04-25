using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Menu { pauseMenu, controlsMenu, settingsMenu };

public class InGameMenu : MonoBehaviour
{
    // Singleton declaration
    public static InGameMenu menu;
    public static bool GamePaused = false;
    
    // The animator in control of the page flip animation
    public Animator flipAnimator;
    // For convenience, manually set how long the page flip takes
    public float pageFlipTime;

    // The overlay which is always active when game is paused
    public GameObject PauseOverlay;
    // The elements which are applied over top of the book, for each screen
    public InGameScreen PauseScreen;
    public InGameScreen ControlsScreen;
    public InGameScreen SettingsScreen;

    // Which screen is currently open
    private InGameScreen currScreen;

    private void Awake()
    {
        if (menu == null)
            menu = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        PauseOverlay.SetActive(false);
    }

    private void Update() 
    {
        // Listens to the pause menu controls
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        currScreen.HideScreen();
        currScreen.gameObject.SetActive(false);
        PauseOverlay.SetActive(false);
        Time.timeScale = 1;
        GamePaused = false;
    }

    public void Pause()
    {
        PauseOverlay.SetActive(true);
        Time.timeScale = 0;
        GamePaused = true;

        // Load the pause menu
        currScreen = PauseScreen;
        PauseScreen.gameObject.SetActive(true);
    }

    public void QuitLevel()
    {
        // Delegate to the level manager
        LevelManager.S.ReturnToTitle();
    }

    // Called by button action: flip to a new menu, with full animations
    public void OpenMenu(int menuType)
    {
        // Convert from enum to game object
        Menu newMenu = (Menu)menuType;
        bool forward = newMenu != Menu.pauseMenu;
        InGameScreen newScreen = EnumToScreen(newMenu);

        // Animate the current menu fading out, page flipping, new menu fading in
        StartCoroutine(FlipPage(currScreen, newScreen, forward));

        // Update our current screen variable
        currScreen = newScreen;
    }

    private IEnumerator FlipPage(InGameScreen prevScreen, InGameScreen newScreen, bool forward)
    {
        // Pause for 1 second to allow for button fade out animation
        prevScreen.HideScreen();
        yield return new WaitForSecondsRealtime(1.0f);
        prevScreen.gameObject.SetActive(false);

        // Animate the page flipping
        flipAnimator.SetBool("isFlipped", forward);
        yield return new WaitForSecondsRealtime(pageFlipTime);

        // Trigger the button fade in animation
        newScreen.gameObject.SetActive(true);
    }

    private InGameScreen EnumToScreen(Menu menuType)
    {
        switch (menuType)
        {
            case Menu.pauseMenu:
                return PauseScreen;
            case Menu.controlsMenu:
                return ControlsScreen;
            case Menu.settingsMenu:
                return SettingsScreen;
            default:
                Debug.LogError("Menu Type is undefined.");
                return null;
        }
    }
}
