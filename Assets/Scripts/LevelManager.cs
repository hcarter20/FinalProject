using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Singleton declaration
    public static LevelManager S;

    private void Awake()
    {
        // Setup the singleton
        if (S == null)
            S = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// Load a new scene
    /// </summary>
    /// <param name="sceneName">Name of the scene in build</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
