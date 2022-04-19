using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    // Also acts as index for the princess
    public int LevelIndex;

    // How many hazards are required to be placed?
    public int HazardCount;
}

/* What changes for each level?
 * 
 * Which items are available? (Certain pillows/items only given when ace prev princess?),
 *  prob handle by having a linen closet prefab which gets instantiated during setup
 *  (behind the ui panel which initially hides everything when level starts).
 * Which princess? Her sprite, her name, what items she wants in the bed, any traits she has, her gift,
 *  prob handle by storing this info in princesscontroller and prefab which switches by index.
 * Flags on which prev gifts/powerups the player currently has,
 *  prob just a bunch of bools in the level manager.
 * Straight up the day count, just so the player knows which level their on?
 */
