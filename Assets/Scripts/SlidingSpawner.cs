using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingSpawner : MonoBehaviour
{
    // Singleton declaration
    public static SlidingSpawner S;

    // How fast should the object move when sliding
    public float speed;

    // What positions the physics object should slide between
    public Vector3 leftPosition, rightPosition;
    private Vector3 targetPosition;

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

    private void Update()
    {
        // When spacebar is pressed, drop the object
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (clickableParent != null && !clickableParent.respawn)
                Destroy(clickableParent);
            DropObject();
        }
        // When Left Shift is pressed, cancel the currently selected object
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (clickableParent != null && !clickableParent.respawn)
                clickableParent.ReEnable();
            Destroy(currObject);
        }
    }

    private void FixedUpdate()
    {
        if (currObject != null)
            MoveObject();
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
        newObject.transform.position = leftPosition;
        currObject = newObject;

        // Setup new variables
        clickableParent = newParent;
        targetPosition = rightPosition;

        // Signal successful placement
        return true;
    }

    private void MoveObject()
    {
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

    private void DropObject()
    {
        // Re-enable physics for the object
        foreach (Rigidbody2D rb in currObject.GetComponentsInChildren<Rigidbody2D>())
            rb.isKinematic = false;

        // Stop controlling this object
        currObject = null;
    }

    // OnDrawGizmos only affects the Unity editor, draws the sliding path
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leftPosition, 0.3f);
        Gizmos.DrawSphere(rightPosition, 0.3f);
        Gizmos.DrawLine(leftPosition, rightPosition);
    }
}
