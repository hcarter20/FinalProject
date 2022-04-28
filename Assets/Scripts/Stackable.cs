using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StackItem { pillow, cushion, mattress, pea, watermelon, melonslice, icecube, crown, flan };

public class Stackable : MonoBehaviour
{
    // Is this a hazard object? (Immediately wakes up princess on contact)
    public bool isHazard;

    // What type of stack item is this?
    public StackItem itemType;

    /* Specialized variables, for specific stackable interactions */
    // Used to melt the ice cube
    private bool isMelting = false;

    private void FixedUpdate()
    {
        if (itemType == StackItem.icecube && GameManager.S.gameState == GameState.sleeping && !isMelting)
        {
            isMelting = true;
            StartCoroutine(Melt());
        }
    }

    private IEnumerator Melt()
    {
        float meltTime = 0.0f;
        float totalTime = 4.0f;
        Vector3 initialScale = transform.localScale;
        Vector3 finalScale = new Vector3(0.1f, 0.1f, 0.1f);

        while (meltTime < totalTime)
        {
            meltTime += Time.fixedDeltaTime;
            transform.localScale = Vector3.LerpUnclamped(initialScale, finalScale, meltTime / totalTime);
            yield return new WaitForFixedUpdate();
        }

        // Once it reaches such a small size, it's destroyed
        Destroy(gameObject);
    }

    public bool Overlaps(Bounds bounds)
    {
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in childColliders)
        {
            if (col.bounds.Intersects(bounds))
                return true;
        }

        return false;
    }

    public bool Inside(Bounds bounds)
    {
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in childColliders)
        {
            if (!col.bounds.Intersects(bounds))
                return false;
        }

        return true;
    }
}
