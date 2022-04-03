using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JointUpdater : MonoBehaviour
{
    public JointSet[] jointSets;
    public float[] dampValues;
    public float[] stiffValues;

    public void Start()
    {
        foreach (JointSet set in jointSets)
        {
            set.UpdateValues(dampValues, stiffValues);
        }
    }
}