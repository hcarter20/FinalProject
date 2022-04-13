using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : MonoBehaviour
{
    // The panel game objects, used to switch between screens
    public GameObject TitleScreen;
    public GameObject SettingScreen;

    // Should start with title menu open, others closed
    private void Start()
    {
        TitleScreen.SetActive(true);
        SettingScreen.SetActive(false);
    }

    /// <summary>
    /// Load the first scene of the game
    /// </summary>
    public void PlayGame()
    {
        LevelManager.S.StartFromTitle();
    }

    /// <summary>
    /// Close the game application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /* We could try something more flexible here, if we plan to expand beyond title/settings */

    /// <summary>
    /// Open the settings screen
    /// </summary>
    public void SettingsOpen()
    {
        TitleScreen.SetActive(false);
        SettingScreen.SetActive(true);
    }

    /// <summary>
    /// Close the settings screen, return to main title
    /// </summary>
    public void SettingClose()
    {
        SettingScreen.SetActive(false);
        TitleScreen.SetActive(true);
    }
}
