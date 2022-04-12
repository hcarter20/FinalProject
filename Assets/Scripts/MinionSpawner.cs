using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    // The game object prefab for the minion
    public GameObject minionPrefab;
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
        minionCoroutine = StartCoroutine(MinionTimer());
    }

    public void StopSpawning()
    {
        StopCoroutine(minionCoroutine);
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