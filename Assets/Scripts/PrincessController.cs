using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessController : MonoBehaviour
{
    // How far can the princess rotate (in degrees) before the player fails?
    public float maxRotateZ = 30.0f;

    // The rigidbody component on this game object
    private Rigidbody2D rb;
    // The 2d collider component on this game object
    private Collider2D coll;
    // The renderer component on this game object
    private SpriteRenderer sr;

    // Is the princess currently asleep (looking for disruptions)
    private bool isSleeping;

    // The initial rotation of the princess (failure check)
    private float initialRotationZ;

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
    }

    public void Activate()
    {
        rb.isKinematic = false;
        coll.enabled = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1.0f);
        initialRotationZ = transform.eulerAngles.z;
        isSleeping = true;
    }

    private void FixedUpdate()
    {
        if (isSleeping)
        {
            // Check if the princess has rotated too far from original position
            if (Mathf.Abs(transform.eulerAngles.z - initialRotationZ) > maxRotateZ)
                WakeUp();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSleeping)
        {
            if (collision.transform.parent != null)
                Debug.Log("Princess collided with " + collision.transform.parent.gameObject.name);
            else
                Debug.Log("Princess collided with " + collision.gameObject.name);

            // Check if the item princess collided with is bedding
            Stackable bedding = collision.gameObject.GetComponentInParent<Stackable>();

            // Colliding with non-bedding (the bed frame) or a hazardous item is failure
            if (bedding == null)
                WakeUp();
            else if (bedding.isHazard)
                WakeUp();
        }
    }

    private void WakeUp()
    {
        isSleeping = false;
        GameManager.S.FailLevel();
    }
}