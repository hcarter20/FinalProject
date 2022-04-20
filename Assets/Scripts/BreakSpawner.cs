using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakSpawner : MonoBehaviour
{
    private float initializationTime;
 
    // Start is called before the first frame update
    void Start()
    {
        initializationTime = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad - initializationTime > 1)
        {
            transform.DetachChildren();
            Destroy(gameObject);
        }
    }
}
