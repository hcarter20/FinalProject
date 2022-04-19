using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles the behavior for clicking and moving objects in the scene:
 *  Player clicks on an object in the selection area, and it begins to trail their mouse
 *   (without requiring player to continue holding down the button).
 *  If the player clicks in the selection area again, the object is deleted (stops trailing).
 *  When player clicks again in the placement area, the object is spawned in the world,
 *   i.e. the image version is replaced with the real physics prefab version.
 *  On click, checks for collisions and prevents player from placing the object in illegal position.
 */

public class Clickable : MonoBehaviour
{
    // The spriter renderer component of this game object: for transparency effects
    public SpriteRenderer sprite;
    // The physics object prefab to instantiate when placed
    public GameObject physicsObject;
    // Does the clickable icon continue to exist after click, spawning a duplicate to follow the mouse?
    public bool respawn;

    // The BoxCollider2D component attached to this objecet
    public BoxCollider2D boxCol;

    // Is the game object currently tracking with the player's cursor?
    private bool tracking = false;

    private void Start()
    {
        if (sprite == null || physicsObject == null)
            Debug.LogError("Clickable objects not fully setup in editor.");
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Automatically destroy when the gameplay transition happens
        // if (GameManager.S.gameState != GameState.stacking)
        //    Destroy(gameObject);

        // When following the cursor, update position to stay with it
        if (tracking)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 currPosition = transform.position;
            transform.position = new Vector3(mousePosition.x, mousePosition.y, currPosition.z);
        }
    }

    /* OnMouseUpAsButton triggers when mouse released after being pressed on object's collider (i.e. clicked).
     * On first click, begin tracking with the cursor. On second click, place physics object at current location. */
    private void OnMouseUpAsButton()
    {
        // Temp: check if this object is receiving the mouse click raycast
        // Debug.Log("OnMouseUp on object " + gameObject.name);

        if (!tracking)
        {
            // If set to respawn, create a duplicate in the selection area
            if (respawn)
                Instantiate(gameObject, transform.parent);

            // Begin updating to follow cursor position
            tracking = true;

            // Set the scale of this game object to match the physics object
            transform.parent = null;
            transform.localScale = physicsObject.transform.localScale;

            // Make the sprite partially transparent
            Color currColor = sprite.color;
            sprite.color = new Color(currColor.r, currColor.g, currColor.b, 0.5f);
        }
        else
        {
            // Are we within the linen closet area (i.e. trying to unselect)?
            if (boxCol.bounds.Intersects(GameManager.S.closetBounds.bounds))
            {
                /*
                Debug.Log("Unselecting this item: " + boxCol.bounds.ToString()
                    + " vs " + GameManager.S.bedBounds.bounds.ToString());
                */
                Destroy(gameObject);
                return;
            }

            // Are we within the bed bounds area?
            if (!boxCol.bounds.Intersects(GameManager.S.bedBounds.bounds))
            {
                /*
                Debug.LogError("Not within bounds of the bed: " + boxCol.bounds.ToString()
                    + " vs " + GameManager.S.bedBounds.bounds.ToString());
                */
                return;
            }

            // Check the collision of trying to place the object here
            Vector2 origin = new Vector2(transform.position.x, transform.position.y);
            Vector2 scaledSize = new Vector2(boxCol.size.x * transform.lossyScale.x, boxCol.size.y * transform.lossyScale.y);
            Collider2D col = Physics2D.OverlapBox(origin, scaledSize, 0.0f, LayerMask.GetMask(new string[] { "Default" }));
            if (col != null)
            {
                /*
                Debug.LogError("Collider " + col.gameObject.name + " is in this area: " 
                    + col.bounds.ToString() + " vs our box at " + origin.ToString() + " with size " + boxCol.size.ToString());
                Debug.LogError("Mouse click is at " + Camera.main.ScreenToWorldPoint(Input.mousePosition).ToString());
                */
                return;
            }

            // Create a physics object at this place
            Instantiate(physicsObject, transform.position, physicsObject.transform.rotation);

            // Self destruct this image object
            Destroy(gameObject);
        }
    }
}
