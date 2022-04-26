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
    // Does the clickable icon continue to exist after click, spawning a duplicate to follow the mouse?
    public bool respawn;

    // The spriter renderer component of this game object: for hiding object
    public SpriteRenderer sprite;

    // A prefab for the physics object preview
    public GameObject previewPrefab;
    // A prefab for the physics object
    public GameObject physicsPrefab;

    // The preview object spawned from this
    private PreviewObject currentPreview;
    // Is this object currently in hiding/should ignore clicks?
    private bool ignoreClick;


    private void Start()
    {
        if (sprite == null)
        {
            Debug.LogError("Clickable objects not fully setup in editor.");
            sprite = GetComponent<SpriteRenderer>();
        }
    }

    /* OnMouseUpAsButton triggers when mouse released after being pressed on object's collider (i.e. clicked).
     * On first click, begin tracking with the cursor. On second click, place physics object at current location. */
    private void OnMouseUpAsButton()
    {
        if (ignoreClick)
            return;

        if (SlidingSpawner.S != null && SlidingSpawner.S.isOpen)
        {
            if (SlidingSpawner.S.AddObject(this, physicsPrefab) && !respawn)
            {
                // Make this invisible, stop accepting clicks
                sprite.enabled = false;
                ignoreClick = true;
            }
        }
        else if (currentPreview == null)
        {
            // Abort early if the GameManager already has a clickable
            if (GameManager.S != null && GameManager.S.hasClickable)
                return;

            // Abort early if the slider already has an object
            if (SlidingSpawner.S != null && !SlidingSpawner.S.isOpen)
                return;

            // If not set to "respawn", hide this in the selection area
            if (!respawn)
            {
                // Make this invisible, stop accepting clicks
                sprite.enabled = false;
                ignoreClick = true;
            }

            // Create a preview object to follow cursor position
            if (GameManager.S != null)
                GameManager.S.hasClickable = true;

            currentPreview = Instantiate(previewPrefab).GetComponent<PreviewObject>();
            currentPreview.clickableParent = this;
        }
    }

    public void ReEnable()
    {
        sprite.enabled = true;
        ignoreClick = false;
    }
}
