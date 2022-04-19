using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* LevelManager is responsible for tracking player's progress in the game
 * and which scene should come next, as well as what the level parameters are.
 */

public class LevelManager : MonoBehaviour
{
    // Singleton declaration
    public static LevelManager S;

    // Temp: So I can toggle between cutscenes or straight to gameplay during development
    public bool debug = true;

    // List of scene names: so we can modify through the Unity editor
    public string TitleScene = "TitleScreen";
    public string IntroScene = "Intro";
    public string SelectScene = "LevelSelect";
    public string GameScene = "Bedroom";
    public string VictoryScene = "Victory";
    public string LossScene = "GameOver";

    // How many total levels? (So know when to go to victory)
    public int TotalLevelCount = 5;

    // Used to modify the gameplay each time the level is loaded
    public int levelIndex = 0;

    public List<Level> Levels;

    private void Awake()
    {
        // Setup the singleton
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    /* Called by the Start button on the Title Menu */
    public void StartFromTitle()
    {
        if (debug)
            NextLevel();
        else
            LoadScene(IntroScene);
    }

    /* Triggered when we want to return to the Title Menu */
    public void ReturnToTitle()
    {
        LoadScene(TitleScene);
    }

    /* Triggered when player passes gameplay, move to level select */
    public void PassLevel()
    {
        levelIndex++;

        if (levelIndex >= TotalLevelCount)
        {
            LoadScene(VictoryScene);
            return;
        }

        if (debug)
            NextLevel();
        else
            LoadScene(SelectScene);
    }

    /* Triggered when player fails gameplay, moves to game over */
    public void FailLevel()
    {
        LoadScene(LossScene);
    }

    /* Load the next level of gameplay from the level select scene */
    public void NextLevel()
    {
        LoadScene(GameScene);
        // GameManager will call when it's awakened
        // GameManager.S.LoadLevel(Levels[levelIndex]);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
