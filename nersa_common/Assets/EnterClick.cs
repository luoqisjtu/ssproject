using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterClick : MonoBehaviour {
	public static bool verify = false;
	public static bool verify_flag = false;
	public static bool cont_b = true;
	public Button Button_enter;
	public Texture2D texture2D;

	void Start(){

		GameObject button_neuromorphic = GameObject.Find("Canvas/Button_enter");
		Button_enter = (Button)button_neuromorphic.GetComponent<Button>();
		//Button_enter.gameObject.SetActive(true);
	}



	void FixedUpdate(){

		cont_b = MoveRacket.cont_a;
		Button_enter.gameObject.SetActive(cont_b);

	}
		

	public void Click(){
		verify = true;
		verify_flag = true;
	}
}
