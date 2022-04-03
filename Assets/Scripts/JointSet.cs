using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointSet : MonoBehaviour
{
    public Joint[] joints;

    public void UpdateValues(float[] dampValues, float[] stiffValues)
    {
        for (int i = 0; i < dampValues.Length; i++)
        {
            foreach (Joint joint in joints)
            {
                if (joint.setNum == i)
                    joint.jointComponent.dampingRatio = dampValues[i];
            }
        }

        for (int i = 0; i < stiffValues.Length; i++)
        {
            foreach (Joint joint in joints)
            {
                if (joint.setNum == i)
                    joint.jointComponent.frequency = stiffValues[i];
            }
        }
    }
}

[System.Serializable]
public class Joint
{
    public SpringJoint2D jointComponent;
    public int setNum;
}