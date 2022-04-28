using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessBone : MonoBehaviour
{
    // Since PrincessController is singleton, assume no conflict
    private PrincessController princess;

    private void Start()
    {
        princess = PrincessController.princess;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!princess.isSleeping)
            return;

        // If this is the first collision, check fall distance
        if (princess.noCollision)
            princess.CheckInitialCollision(this);

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

            princess.WakeUp();

            AudioManager.S.Play("Bonk");
        }
        else if (bedding.isHazard)
        {
            Debug.Log("Princess wakes up because she collided with a hazard.");
            if (collision.transform.parent != null)
                Debug.Log("Princess collided with " + collision.transform.parent.gameObject.name);
            else
                Debug.Log("Princess collided with " + collision.gameObject.name);

            princess.WakeUp();
        }
    }
}
