using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    // Singleton declaration
    public static MinionSpawner S;

    // Where is the minion trying to get to, to drop the item?
    public Transform startLeft, startRight, target;
    // Minion flees straight up after startle
    public float fleeHeight = 6.5f;

    // The game object prefab for the minion
    private GameObject minionPrefab;
    // The time between minion spawns (give or take 1 second)
    private float minionTime;

    // Reference to the currently instantiated minion
    private GameObject minion;

    // The coroutine which spawns minions
    private Coroutine minionCoroutine;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this);
    }

    public void StartSpawning()
    {
        // Setup for this level
        minionPrefab = LevelManager.S.minionPrefabs[LevelManager.S.levelIndex];
        minionTime = LevelManager.S.minionTimes[LevelManager.S.levelIndex];

        // If minion prefab is null, means that minions shouldn't appear this level
        if (minionPrefab != null)
            minionCoroutine = StartCoroutine(MinionTimer());
    }

    public void StopSpawning()
    {
        if (minionCoroutine != null)
            StopCoroutine(minionCoroutine);
        if (minion != null)
            Destroy(minion);
    }

    private IEnumerator MinionTimer()
    {
        // TODO: Would it be better to set this as a flag?
        while (true)
        {
            // Generate a random time until the next minion appears
            // TODO: For now the same every time, add in randomness
            yield return new WaitForSeconds(minionTime);

            // Randomly decide whether moves left to right, or right to left
            bool fromLeft = Random.Range(0, 2) == 0;
            Transform start = fromLeft ? startLeft : startRight;
            Transform exit = fromLeft ? startRight : startLeft;
            
            // Create a new minion
            minion = Instantiate(minionPrefab, start);
            MinionController cont = minion.GetComponent<MinionController>();
            cont.fleeHeight = fleeHeight;
            cont.targetPosition = new Vector2(target.position.x, target.position.y);
            cont.exitPosition = new Vector2(exit.position.x, exit.position.y);

            // Check in with the minion, to see when it's finished
            while (minion != null)
                yield return new WaitForSeconds(1.0f);
        }
    }
}