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

    // The renderer component on this game object
    private SpriteRenderer sr;

    // Is the princess currently asleep (looking for disruptions)
    [HideInInspector]
    public bool isSleeping;

    // Position/rotation of princess when physics triggered (failure check)
    private float initialRotationZ;
    private float initialPositionY;
    [HideInInspector]
    public bool noCollision = true;

    private void Awake()
    {
        if (princess == null)
            princess = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr == null)
            Debug.LogError("Princess is missing components.");

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
        isSleeping = false;

        // Customize the princess for this level
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

        // Enable physics on the sprite bones
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D childCol in childColliders)
            childCol.enabled = true;
        Rigidbody2D[] childRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D childRb in childRigidbodies)
            childRb.isKinematic = false;

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
            if (Mathf.Abs(ToDegrees(outlineCollider.transform.eulerAngles.z) - initialRotationZ) > maxRotateZ)
            {
                Debug.Log("Princess wakes up because of rotation: initial rotation " 
                    + initialRotationZ + " vs. eulerAngles " + transform.eulerAngles.z + ".");
                WakeUp();
            }
        }
    }

    public void CheckInitialCollision(PrincessBone bone)
    {
        if (!noCollision)
            return;

        noCollision = false;
        float fallDist = Mathf.Abs(initialPositionY - bone.transform.position.y);
        if (fallDist > maxFallDist)
        {
            Debug.Log("Princess fell too far down! Distance of " + fallDist + "units.");
            WakeUp();
        }
    }

    public void WakeUp()
    {
        if (!isSleeping)
            return;

        isSleeping = false;
        AudioManager.S.Play("Yawn");
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
