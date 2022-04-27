using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessController : MonoBehaviour
{
    // Singleton declaration
    public static PrincessController princess;

    // How far can the princess rotate (in degrees) before the player fails?
    public float maxRotateZ = 20.0f;
    // How far can the princess fall (from initial spawn position) before the player fails?
    public float maxFallDist;

    // The child 2d capsule collider of this game object
    public CapsuleCollider2D outlineCollider;

    // The rigidbody component on this game object
    private Rigidbody2D rb;
    // The 2d collider component on this game object
    private Collider2D coll;
    // The renderer component on this game object
    private SpriteRenderer sr;

    // Is the princess currently asleep (looking for disruptions)
    private bool isSleeping;

    // Position/rotation of princess when physics triggered (failure check)
    private float initialRotationZ;
    private float initialPositionY;
    private bool noCollision = true;

    private void Awake()
    {
        if (princess == null)
            princess = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb == null || coll == null || sr == null)
            Debug.LogError("Princess is missing components.");

        rb.isKinematic = true;
        coll.enabled = false;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
        isSleeping = false;

        // Customize the princess for this level
        sr.sprite = LevelManager.S.princessSprites[LevelManager.S.levelIndex];
        maxFallDist = LevelManager.S.princessMaxFalls[LevelManager.S.levelIndex];
        float newHeight = LevelManager.S.princessHeights[LevelManager.S.levelIndex];
        transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
    }

    public void Activate()
    {
        // Check if the player has placed an overlapping object, prevent clipping
        Vector2 origin = new Vector2(transform.position.x + outlineCollider.offset.x, transform.position.y + outlineCollider.offset.y);
        Vector2 point = origin;
        Vector2 scaledSize = new Vector2(outlineCollider.size.x * transform.lossyScale.x, outlineCollider.size.y * transform.lossyScale.y);
        CapsuleDirection2D direction = outlineCollider.direction;
        LayerMask defaultLayer = LayerMask.GetMask(new string[] { "Default" });

        Collider2D col;
        while ((col = Physics2D.OverlapCapsule(point, scaledSize, direction, 0.0f, defaultLayer)) != null)
        {
            /*
            Debug.LogError("Collider " + col.gameObject.name + " is in this area: "
                + col.bounds.ToString() + " vs our capsule at " + point.ToString() + " with size " + scaledSize.ToString());
            */
            // Move the princess up and try again
            point += new Vector2(0.0f, 0.2f);
        }

        //Debug.Log("It's safe to add princess with capsule at " + point.ToString() + " with size " + scaledSize.ToString());

        float yDiff = point.y - origin.y;
        if (yDiff != 0.0f)
            transform.position += new Vector3(0.0f, yDiff, 0.0f);

        coll.enabled = true;
        rb.isKinematic = false;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1.0f);
        initialRotationZ = ToDegrees(transform.eulerAngles.z);
        initialPositionY = transform.position.y;
        isSleeping = true;
    }

    private void FixedUpdate()
    {
        if (isSleeping)
        {
            // Check if the princess has rotated too far from original position
            if (Mathf.Abs(ToDegrees(transform.eulerAngles.z) - initialRotationZ) > maxRotateZ)
            {
                Debug.Log("Princess wakes up because of rotation: initial rotation " 
                    + initialRotationZ + " vs. eulerAngles " + transform.eulerAngles.z + ".");
                WakeUp();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSleeping)
        {
            // If this is the first collision, check fall distance
            if (noCollision)
            {
                noCollision = false;
                float fallDist = Mathf.Abs(initialPositionY - transform.position.y);
                Debug.Log("Princess has fallen " + fallDist + " units.");

                if (fallDist > maxFallDist)
                {
                    Debug.Log("Princess fell too far down!");
                    WakeUp();
                }
            }

            // Check if the item princess collided with is bedding
            Stackable bedding = collision.gameObject.GetComponentInParent<Stackable>();

            // Colliding with non-bedding (the bed frame) or a hazardous item is failure
            if (bedding == null)
            {
                Debug.Log("Princess wakes up because she collided with non-bedding (couldn't find Stackable in parent).");
                if (collision.transform.parent != null)
                    Debug.Log("Princess collided with " + collision.transform.parent.gameObject.name);
                else
                    Debug.Log("Princess collided with " + collision.gameObject.name);

                WakeUp();

                AudioManager.S.Play("Bonk");
            }
            else if (bedding.isHazard)
            {
                Debug.Log("Princess wakes up because she collided with a hazard.");
                if (collision.transform.parent != null)
                    Debug.Log("Princess collided with " + collision.transform.parent.gameObject.name);
                else
                    Debug.Log("Princess collided with " + collision.gameObject.name);

                WakeUp();
            }
        }
    }

    public void WakeUp()
    {
        isSleeping = false;
        GameManager.S.FailLevel();
    }

    /* Converts rotations from [0, 360) to (-180, 180]. */
    private float ToDegrees(float rotation)
    {
        if (rotation > 180.0f)
            return rotation - 360.0f;
        return rotation;
    }
}
