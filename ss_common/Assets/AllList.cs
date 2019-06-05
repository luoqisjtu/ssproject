using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllList : MonoBehaviour {
    public  List<float> listToHoldData=new List<float>();
    public  List<float> listToHoldTime = new List<float>();
    public  List<float> listToHoldID = new List<float>();
    public  List<float> listToHoldBarHeight = new List<float>();
    public  List<bool> listToHoldcollision = new List<bool>();
   
    

    public void allList( float IDrecord, float time, float barHeight, bool collisionrecord, float barForceInMilliNewton)
    {   
        listToHoldID.Add(IDrecord);
        listToHoldBarHeight.Add(barHeight);
        listToHoldTime.Add(time);
        listToHoldcollision.Add(collisionrecord);
        listToHoldData.Add(barForceInMilliNewton / 1000);
    }
}
