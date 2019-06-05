using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prohibited_area : MonoBehaviour {

	public static float collision2 = 0;   // bool false-0
    public Texture2D texture2D;
    public GameObject obj;
    public Renderer rend;
	public GameObject obj_1;
	public static int time_limit;


    private void Start()
     {
	    obj = GameObject.Find("Prohibited_area");
	    rend = obj.GetComponent<Renderer>();

		obj_1 = GameObject.Find ("red");
		obj_1.GetComponent<Renderer>().enabled = false;
		obj_1.transform.position = new Vector2(0, 10);

     }


   void Update () {

    }
		



   private void OnCollisionEnter2D(Collision2D collision)
    {
		

	    if(collision.gameObject.name=="MoveRacket_force")
	    {
			collision2 = 1;   //true-1	
	
//		  StartCoroutine(Delay.run(() =>{
//		   collision2 = 0;     //false-0
//			}, 10));
	    }
			
    }


   private void OnGUI()
   {
		time_limit = TargetRacket_force.time_flag;

	
		if (collision2 == 1)                  //if (collision2)   collision=true  
	   {
			
		obj_1.GetComponent<Renderer>().enabled = true;


		GUIStyle bb = new GUIStyle();
		bb.normal.background = texture2D;
		bb.normal.textColor = Color.yellow;   //设置字体颜色的
		bb.fontSize = 150;       //这是字体大小
		bb.alignment = TextAnchor.MiddleCenter;
		GUI.Button(new Rect(Screen.width * 0.3f, Screen.height*0.1f , 300, 500), "Game Over", bb);
	   }
			
		if (time_limit >= 1450) {

			collision2 = 0;
			obj_1.GetComponent<Renderer>().enabled = false;
		}
		//Debug.Log ("time_limit" + time_limit);


	//if (ishint)
	//{
	//    GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.75f, 200, 30), "请上移木条");
	//}

   }
		
}
