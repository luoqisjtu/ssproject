using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClick : MonoBehaviour {
	public static bool flag = true;
	public void Click(){
		flag = !flag;
	}
}