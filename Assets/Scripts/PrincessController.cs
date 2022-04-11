using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessController : MonoBehaviour
{
    // The rigidbody component on this game object
    public Rigidbody2D rb;
    // The 2d collider component on this game object
    public Collider2D coll;
    // The renderer component on this game object
    public SpriteRenderer sr;

    // Is the princess currently asleep (looking for disruptions)
    private bool isSleeping;

    private void Start()
    {
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
        isSleeping = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSleeping)
        {
            Debug.Log("Princess collided with " + collision.gameObject.name);

            // Princess is woken up by touching certain hazard objects
            Bedding bedding = collision.gameObject.GetComponentInParent<Bedding>();
            if (bedding != null && bedding.isHazard)
            {
                WakeUp();
            }
        }
    }

    private void WakeUp()
    {
        isSleeping = false;
        GameManager.S.FailLevel();
    }
}
