using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    // Used to manipulate the physics of the children objects
    public List<StackableBone> bones;
    // Used to determine the center of this shape, since parent doesn't track
    public StackableBone centerBone;

    // The prefab of the broken version to spawn at this location
    public GameObject brokenPrefab;
    // The force threshold for breaking this object
    public float breakingForce = 3.5f;

    // Used to track the forces on the children
    private float cumulativeForces = 0.0f;

    private void Start()
    {
        // Load the physics bones in the children of this object
        bones = new List<StackableBone>();
        foreach (StackableBone bone in GetComponentsInChildren<StackableBone>())
            bones.Add(bone);
    }

    private void FixedUpdate()
    {
        if (cumulativeForces != 0.0f)
        {
            if (cumulativeForces > breakingForce)
                Break();

            //Debug.Log("Current cumulative forces is " + cumulativeForces);
            cumulativeForces = 0.0f;
        }
    }

    private void Break()
    {
        // Create the prefab object at the location of the watermelon
        Transform center = centerBone.transform;
        GameObject brokenObject = Instantiate(brokenPrefab, center.position, center.rotation);

        // Tell the ClosetController that we've been replaced
        ClosetController.S.ItemBroken(brokenObject);

        // Destroy this game object
        Destroy(gameObject);
    }

    // Accumulate collisions from the children, use those forces to determine when to break
    public void AccumulateForce(float newForce)
    {
        cumulativeForces += newForce;
    }
}
