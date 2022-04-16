using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public static InGameMenu menu;
    
    public GameObject PauseScreen;
    public GameObject ControlsScreen;
    public GameObject SettingsScreen;

    public enum Menu { pauseMenu, controlsMenu, settingsMenu };

    // The animator in control of the page flip animation
    public Animator flipAnimator;

    private void Awake()
    {
        // Prevent the UI from being destroyed when we reload the scene
        if (menu == null)
            menu = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update() {
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
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
        GamePaused = false;
    }

    public void Pause()
    {
        PauseScreen.SetActive(true);
        Time.timeScale = 0;
        GamePaused = true;
    }

    public void FlipPage()
    {
        // TODO: Fade out the buttons

        // Animate the page flipping
        flipAnimator.SetBool("isFlipped", true);

        // TODO: Fade in the buttons on new screen
    }

    public void FlipPageBack()
    {
        // TODO: Fade out the buttons

        // Mark the page flipping bool as false
        flipAnimator.SetBool("isFlipped", false);

        // TODO: Fade in the buttons on main pause screen
    }

    public void LoadMenu(int menuType)
    {
        // Turn off all the screens
        if (PauseScreen != null) PauseScreen.SetActive(false);
        if (ControlsScreen != null) ControlsScreen.SetActive(false);
        if (SettingsScreen != null) SettingsScreen.SetActive(false);

        // Turn on the requested screen
        switch ((Menu)menuType)
        {
            case Menu.pauseMenu:
                if (PauseScreen != null)
                    PauseScreen.SetActive(true);
                else
                    Debug.Log("PauseScreen variable not set in script.");
                break;
            case Menu.controlsMenu:
                if (ControlsScreen != null)
                    ControlsScreen.SetActive(true);
                else
                    Debug.Log("ControlsScreen variable not set in script.");
                break;
            case Menu.settingsMenu:
                if (SettingsScreen != null)
                    SettingsScreen.SetActive(true);
                else
                    Debug.Log("SettingsScreen variable not set in script.");
                break;
            default:
                Debug.LogError("Menu Type is undefined.");
                break;
        }
    }

    /*
    public void ControlsLoad()
    {
        PauseScreen.SetActive(false);
        ControlsScreen.SetActive(true);
    }

    public void ControlsReturn()
    {
        ControlsScreen.SetActive(false);
        PauseScreen.SetActive(true);
    }

    public void SettingsLoad()
    {
        PauseScreen.SetActive(false);
        SettingsScreen.SetActive(true);
    }

    public void SettingsReturn()
    {
        SettingsScreen.SetActive(false);
        PauseScreen.SetActive(true);
    }

    public void ReturnMenu()
    {
        Time.timeScale = 1;
        GamePaused = false;
    }
    */
}
