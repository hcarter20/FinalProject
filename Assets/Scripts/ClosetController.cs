using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetController : MonoBehaviour
{
    // Singleton declaration
    public static ClosetController S;

    // The collider which defines the bed bounds
    public BoxCollider2D bedBounds;
    // Toggles between strict & lenient placement: must be entirely in bed vs partially in bed
    public bool strictBounds = false;

    // Enum can't be shown in editor: edit directly in code
    // TODO: As of now, all items required for level 5 (change to 3 out of 5?)
    public List<StackItem>[] requiredSets =
        {
            new List<StackItem>() { StackItem.pea },
            new List<StackItem>() { StackItem.watermelon },
            new List<StackItem>() { StackItem.icecube },
            new List<StackItem>() { StackItem.crown, StackItem.flan },
            new List<StackItem>() { StackItem.pea, StackItem.watermelon, StackItem.icecube, StackItem.crown, StackItem.flan }
        };

    // The set of required items for this level
    private List<StackItem> requiredSet;
    private List<Stackable> stackItems;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        // Check which items are required for this level
        requiredSet = requiredSets[LevelManager.S.levelIndex];
        stackItems = new List<Stackable>();

        // Instantiate the appropriate closet prefab
        GameObject closetPrefab = LevelManager.S.closetPrefabs[LevelManager.S.levelIndex];
        Instantiate(closetPrefab, transform);
    }

    public void ItemPlaced(GameObject gameObject)
    {
        // Get the stackable component on this object
        Stackable s = gameObject.GetComponent<Stackable>();

        // Check if it's one of the required items we need to keep track of
        if (s.isHazard && requiredSet.Remove(s.itemType))
        {
            // Add it to the list, check at the end of the stack phase
            stackItems.Add(s);
        }
    }

    public void ItemBroken(GameObject brokenObject)
    {
        // Look for a null stackable to remove
        if (stackItems.Remove(null))
            Debug.Log("Removed a null item.");

        // Get the stackable components in the broken object
        foreach(Stackable s in brokenObject.GetComponentsInChildren<Stackable>())
            stackItems.Add(s);
    }

    /// <summary>
    /// Return values:
    /// 0 - All required items are in bed (fully valid)
    /// 1 - Items are in the scene, but not in bed area
    /// 2 - Not all items are in the scene
    /// </summary>
    public int ValidateItems()
    {
        if (requiredSet.Count > 0)
            return 2;

        // Check that each of the required items is actually in the bed
        foreach (Stackable s in stackItems)
        {
            // s == null if watermelon and was deleted
            if (s != null)
            {
                // Temp: keep both for testing which is better
                if (strictBounds && !s.Inside(bedBounds.bounds))
                    return 1;
                else if (!strictBounds && !s.Overlaps(bedBounds.bounds))
                    return 1;
            }
        }

        return 0;
    }
}
