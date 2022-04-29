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
    public string SelectScene = "TransitionScene";
    public string GameScene = "Bedroom";
    public string VictoryScene = "Victory";
    public string LossScene = "GameOver";

    // How many total levels? (So know when to go to victory)
    public int LevelCount = 5;

    // Used to modify the gameplay each time the level is loaded
    public int levelIndex = 0;

    // All of the values which change for each level
    public List<float> stackTimes;
    public List<float> sleepTimes;
    public List<GameObject> princessPrefabs;
    public List<float> princessHeights;
    public List<float> princessMaxFalls;
    public List<GameObject> closetPrefabs;
    public List<GameObject> minionPrefabs;
    public List<float> minionTimes;
    public List<float> slideSpeeds;
    public List<float> speedIncrs;
    // egchan adding for transition? 
    public List<string> chapterName;
    public List<Sprite> chapterIcon;

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
    private void Start()
    {
        // In normal usage, we start at the title scene
        if (SceneManager.GetActiveScene().name.Equals(TitleScene))
        {
            // Start playing the title music
            AudioManager.S.PlayTitleMusic();
        }
    }

    /* Called by the Start button on the Title Menu */
    public void StartFromTitle()
    {
        AudioManager.S.StopAllSounds();

        if (debug)
            LoadScene(SelectScene);
        else
            LoadScene(IntroScene);
    }

    /* Triggered when we want to return to the Title Menu */
    public void ReturnToTitle()
    {
        // Start playing the title music
        AudioManager.S.StopAllSounds();
        AudioManager.S.PlayTitleMusic();

        LoadScene(TitleScene);
    }

    /* Triggered when player passes gameplay, move to level select */
    public void PassLevel()
    {
        AudioManager.S.StopAllSounds();

        levelIndex++;

        if (levelIndex >= LevelCount)
        {
            // Reset the level index, to avoid bugs
            levelIndex = 0;
            LoadScene(VictoryScene);
        }
        else
        {
            LoadScene(SelectScene);
        }
    }

    /* Triggered when player fails gameplay, moves to game over */
    public void FailLevel()
    {
        AudioManager.S.StopAllSounds();
        LoadScene(LossScene);
    }

    /* Load the next level of gameplay from the level select scene */
    public void NextLevel()
    {
        AudioManager.S.StopAllSounds();
        LoadScene(GameScene);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
