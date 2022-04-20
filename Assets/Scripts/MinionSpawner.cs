using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    // The game object prefabs for the minion
    public List<GameObject> minionPrefabs;
    private GameObject minionPrefab;
    // Where is the minion trying to get to, to drop the item?
    public Transform target, exit, flee;
    // How long between minions appearing? (Give or take 1 second)
    public float minionTime = 10.0f;

    // Reference to the currently instantiated minion
    private GameObject minion;

    // The coroutine which spawns minions
    private Coroutine minionCoroutine;

    public void StartSpawning()
    {
        // Setup the minion prefab for this level
        if (LevelManager.S == null)
            minionPrefab = minionPrefabs[0];
        else
        {
            int index = LevelManager.S.Levels[LevelManager.S.levelIndex].LevelIndex;
            minionPrefab = minionPrefabs[index];
        }

        // TODO: On final level, minions appear more often

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

            // Create a new minion
            minion = Instantiate(minionPrefab, transform);
            MinionController cont = minion.GetComponent<MinionController>();
            cont.targetPosition = new Vector2(target.position.x, target.position.y);
            cont.exitPosition = new Vector2(exit.position.x, exit.position.y);
            cont.fleePosition = new Vector2(flee.position.x, flee.position.y);

            // Check in with the minion, to see when it's finished
            while (minion != null)
                yield return new WaitForSeconds(1.0f);
        }
    }
}