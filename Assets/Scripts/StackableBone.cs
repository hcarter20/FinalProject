using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackableBone : MonoBehaviour
{
    // The Breakable script on the parent object of this bone
    public Breakable parentBreak;

    private void Start()
    {
        if (parentBreak == null)
            parentBreak = GetComponentInParent<Breakable>();
    }

    // TODO: Setup to detect collisions from the children, use those forces to determine when to break:
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Note: collision.otherRigidbody corresponds to rigidbody of this object,
        // collision.rigidbody is the incoming object, may be null if bed frame.
        Rigidbody2D rb = collision.rigidbody != null ? collision.rigidbody : collision.otherRigidbody;
        float force = Vector3.Dot(collision.GetContact(0).normal, collision.relativeVelocity) * rb.mass;
        // Debug.Log("Collided with " + collision.gameObject.name + " with force " + force);
        parentBreak.AccumulateForce(force);
    }
}
