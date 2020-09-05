using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFeature : MonoBehaviour {

    private int containedWater = 0;
    private int capacity = 100;

    public float waterDragCoefficient = 2;

    public int width;
    public int length;

    public bool AddWater(int amount) {
        if (capacity > containedWater) {
            containedWater += amount;
            return true;
        } else {
            return false;
        }
    }
    


}
