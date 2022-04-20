using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    // Where is the minion trying to get to, to drop the item?
    public Vector2 targetPosition, exitPosition, fleePosition;
    // The movement speed of the minion, when moving normally or after startle (fleeing)
    public float defaultSpeed = 1.0f;
    public float fleeSpeed = 2.0f;

    // Used to load based on level
    public List<GameObject> hazardPrefabs;
    public List<GameObject> hazardPropPrefabs;

    // The prefab for the actual physics object corresponding to the hazard we're carrying
    private GameObject hazardPrefab;
    // The hazard object which we're carrying with us
    private GameObject hazardProp;

    // The current state of the minion (used to control its behavior)
    private enum MoveState { approach, drop, leave, startle, flee };
    private MoveState moveState = MoveState.approach;

    // The animator and sprite renderer components on this game object
    private Animator animator;
    private SpriteRenderer sprite;

    private void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        int index = 0;

        int count = hazardPropPrefabs.Count;
        if (count > 1)
            index = Random.Range(0, count);

        hazardPrefab = hazardPrefabs[index];
        hazardProp = Instantiate(hazardPropPrefabs[index], transform);
    }

    /* OnMouseUpAsButton triggers when mouse released after being pressed on object's collider (i.e. clicked).
     * When clicked, interrupts the minion and forces them to fly off. */
    private void OnMouseUpAsButton()
    {
        // Trigger the click animation
        if (animator != null)
            animator.SetTrigger("Click");

        // Minion has been startled, transition to fleeing
        StartCoroutine(Flee());
    }

    private IEnumerator DropObject()
    {
        moveState = MoveState.drop;

        Transform hazardTransform = hazardProp.transform;
        Destroy(hazardProp);
        Instantiate(hazardPrefab, hazardTransform.position, hazardTransform.rotation, null);

        yield return new WaitForSeconds(0.1f);
        moveState = MoveState.leave;
    }

    private IEnumerator Flee()
    {
        moveState = MoveState.startle;
        yield return new WaitForSeconds(0.5f);
        moveState = MoveState.flee;
    }

    private void FixedUpdate()
    {
        if (moveState == MoveState.approach)
        {
            if (Vector2.Distance(AsVector2(transform.position), targetPosition) < 0.1f)
                StartCoroutine(DropObject());
            else
                MovePosition(targetPosition, defaultSpeed);
        }
        else if (moveState == MoveState.leave)
        {
            if (Vector2.Distance(AsVector2(transform.position), exitPosition) < 0.1f)
                Destroy(gameObject);
            else
                MovePosition(exitPosition, defaultSpeed);
        }
        else if (moveState == MoveState.flee)
        {
            if (Vector2.Distance(AsVector2(transform.position), fleePosition) < 0.1f)
                Destroy(gameObject);
            else
                MovePosition(fleePosition, fleeSpeed);
        }
    }

    private void MovePosition(Vector2 goal, float speed)
    {
        // Flip the sprite depending on direction of motion
        sprite.flipX = (transform.position.x <= goal.x);

        // Move the minion towards the goal position
        Vector2 currPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPosition = Vector2.MoveTowards(currPosition, goal, speed * Time.fixedDeltaTime);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    private Vector2 AsVector2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
}
