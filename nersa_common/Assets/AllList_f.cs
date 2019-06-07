using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllList_f : MonoBehaviour {

    public List<float> listToHoldTime = new List<float>();
    public List<float> listToHoldID = new List<float>();
	public List<float> listToHoldrawEmg = new List<float>();
    public List<float> listToHoldaveragefilter = new List<float>();
	public List<float> listToHoldbutterworthfilter = new List<float>();
    public List<float> listToHoldbayesfilter = new List<float>();
    public List<float> listToHoldemg_send = new List<float>();


    public void allList( float time, float IDrecord_f, float rawEmg, float averagefilter, float butterworthfilter, float bayesfilter, float emg_send)
    {   
        
        listToHoldTime.Add(Time.time);
        listToHoldID.Add(IDrecord_f);
		listToHoldrawEmg.Add(rawEmg);
        listToHoldaveragefilter.Add(averagefilter);
		listToHoldbutterworthfilter.Add(butterworthfilter);
        listToHoldbayesfilter.Add(bayesfilter);
        listToHoldemg_send.Add(emg_send);
    }
}
