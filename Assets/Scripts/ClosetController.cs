using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetController : MonoBehaviour
{
    // List of the closet gameobject prefabs to be instantiated
    public List<GameObject> closetPrefabs;

    private GameObject closet;

    private void Start()
    {
        if (LevelManager.S != null)
        {
            // Instiate the appropriate closet prefab
            int index = LevelManager.S.Levels[LevelManager.S.levelIndex].LevelIndex;
            closet = Instantiate(closetPrefabs[index], transform);
        }
        else
        {
            closet = Instantiate(closetPrefabs[0], transform);
        }
    }
}
