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

    private void Start()
    {
        rb.isKinematic = true;
        coll.enabled = false;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
    }

    public void Activate()
    {
        rb.isKinematic = false;
        coll.enabled = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1.0f);
    }
}
