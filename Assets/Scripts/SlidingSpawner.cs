using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingSpawner : MonoBehaviour
{
    // Singleton declaration
    public static SlidingSpawner S;

    // What positions the physics object should slide between
    public Vector3 leftPosition, rightPosition, startPosition;
    private Vector3 targetPosition;

    // The speed for this level of the object when sliding
    private float speed;

    // The game object which is currently sliding back and forth
    [HideInInspector]
    public GameObject currObject;

    // The clickable script which spawned it
    private Clickable clickableParent;


    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        // Setup the speed for this level
        speed = LevelManager.S.slideSpeeds[LevelManager.S.levelIndex];
    }

    private void Update()
    {
        // Check if stacking phase has ended
        if (GameManager.S.gameState != GameState.stacking && currObject != null)
            CancelObject();

        // When spacebar is pressed, drop the object
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropObject();
        }
        // When Left Shift is pressed, cancel the currently selected object
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CancelObject();
        }
    }

    private void FixedUpdate()
    {
        if (currObject != null)
        {
            // Update the position of the object
            Vector3 currPosition = currObject.transform.position;

            // If we're at destination (within float rounding)
            if (Vector3.Distance(targetPosition, currPosition) <= 0.05f)
            {
                if (targetPosition == rightPosition)
                    targetPosition = leftPosition;
                else
                    targetPosition = rightPosition;
            }
            else
            {
                currPosition = Vector3.MoveTowards(currPosition, targetPosition, speed * Time.deltaTime);
                currObject.transform.position = currPosition;
            }
        }
    }

    public bool AddObject(Clickable newParent, GameObject newPrefab)
    {
        // Return failure if there's already an object
        if (currObject != null)
            return false;

        // Instantiate the physics prefab, turn on kinematic, start moving
        GameObject newObject = Instantiate(newPrefab);
        foreach (Rigidbody2D rb in newObject.GetComponentsInChildren<Rigidbody2D>())
            rb.isKinematic = true;
        newObject.transform.position = startPosition;
        currObject = newObject;

        // Setup new variables
        clickableParent = newParent;
        targetPosition = rightPosition;

        // Signal successful placement
        return true;
    }

    private void DropObject()
    {
        // Re-enable physics for the object
        foreach (Rigidbody2D rb in currObject.GetComponentsInChildren<Rigidbody2D>())
            rb.isKinematic = false;

        // Indicate that the object has been placed in scene
        ClosetController.S.ItemPlaced(currObject);

        // Tell the parent that we've placed the object
        if (clickableParent != null && !clickableParent.respawn)
            Destroy(clickableParent);

        // Stop controlling this object
        currObject = null;
    }

    private void CancelObject()
    {
        // Tell the parent that we didn't place the object
        if (clickableParent != null && !clickableParent.respawn)
            clickableParent.ReEnable();

        // Destroy the object
        Destroy(currObject);
    }

    // OnDrawGizmos only affects the Unity editor, draws the sliding path
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leftPosition, 0.2f);
        Gizmos.DrawSphere(rightPosition, 0.2f);
        Gizmos.DrawLine(leftPosition, rightPosition);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(startPosition, 0.2f);
    }
}
