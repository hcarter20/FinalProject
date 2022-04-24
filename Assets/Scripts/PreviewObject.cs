using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    // The physics object prefab to instantiate when placed
    public GameObject physicsObject;

    // The BoxCollider2D component attached to this objecet
    private BoxCollider2D boxCol;

    // Toggle for testing purposes:
    public bool restrictPlacement = true;

    // The Clickable which spawned us
    [HideInInspector]
    public Clickable clickableParent;

    private void Start()
    {
        if (physicsObject == null)
            Debug.LogError("PreviewObject missing its physics object.");

        // Set the scale of this game object to match the physics object
        // transform.parent = null;
        // transform.localScale = physicsObject.transform.localScale;
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // When following the cursor, update position to stay with it
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 currPosition = transform.position;
        transform.position = new Vector3(mousePosition.x, mousePosition.y, currPosition.z);
    }

    private void OnMouseUpAsButton()
    {
        // Are we within the linen closet area (i.e. trying to unselect)?
        if (boxCol.bounds.Intersects(GameManager.S.closetBounds.bounds))
        {
            // Testing:
            Debug.Log("Unselecting this item: " + boxCol.bounds.ToString()
                + " vs " + GameManager.S.bedBounds.bounds.ToString());

            // Indicate that the mouse will be free, before self destruct
            if (GameManager.S != null)
                GameManager.S.hasClickable = false;

            // Replace this item at original position if unselect, to prevent disappearing
            if (!clickableParent.respawn)
                clickableParent.ReEnable();
            
            Destroy(gameObject);
            return;
        }

        // Are we within the bed bounds area?
        if (restrictPlacement && !boxCol.bounds.Intersects(GameManager.S.bedBounds.bounds))
        {
            // Testing:
            Debug.LogError("Not within bounds of the bed: " + boxCol.bounds.ToString()
                + " vs " + GameManager.S.bedBounds.bounds.ToString());
            return;
        }

        // Check the collision of trying to place the object here
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector2 scaledSize = new Vector2(boxCol.size.x * transform.lossyScale.x, boxCol.size.y * transform.lossyScale.y);
        Collider2D col = Physics2D.OverlapBox(origin, scaledSize, 0.0f, LayerMask.GetMask(new string[] { "Default" }));
        if (col != null)
        {
            // Testing:
            Debug.LogError("Collider " + col.gameObject.name + " is in this area: "
                + col.bounds.ToString() + " vs our box at " + origin.ToString() + " with size " + boxCol.size.ToString());
            Debug.LogError("Mouse click is at " + Camera.main.ScreenToWorldPoint(Input.mousePosition).ToString());

            return;
        }

        // Create the physics object at this place
        Instantiate(physicsObject, transform.position, physicsObject.transform.rotation);

        // Indicate that the mouse will be free, before self destruct
        if (GameManager.S != null)
            GameManager.S.hasClickable = false;

        // Self destruct this preview & its clickable object
        if (!clickableParent.respawn)
            Destroy(clickableParent);
        Destroy(gameObject);
    }
}
