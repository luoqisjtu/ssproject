using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRacket_force : MonoBehaviour
{
	private int sum;
	private float[,] trial = new float[100, 10];
	public static int count = 0;
	private int counttmp = 0;       // 第一个起始ID数据不需要，故不需弹出“开始”，定义counttmp=0；   若第一个ID需要弹出“开始”,定义counttmp=大于ID个数的数
	public static float collision1 = 0;   //public static bool collision1 = false
	public static bool temp = true;
	private int flag = 0;
	public static int time_flag = 0;
	public static float ID = 0;

	public static int levelnumber;
	public string[][] Array;

	public GameObject obj;
	public Renderer rend;

	public bool ishint = false;
	public Texture2D texture2D;
	Color[] changecolor;
	public static bool destroy_mark = false;


	public  void Awake()
	{
		
		obj = GameObject.Find ("TargetRacket_force");
		rend = obj.GetComponent<Renderer>();
		rend.material.color = Color.green;

		//读取text二进制文件  
		TextAsset binAsset = Resources.Load("config", typeof(TextAsset)) as TextAsset;
		//Debug.Log("文件读取");
		//读取每一行的内容  
		string[] lineArray = binAsset.text.Split("\r"[0]);  

		//创建二维数组  
		Array = new string[lineArray.Length][];
		//Debug.Log("lineArray.Length" + lineArray.Length);
		//把csv中的数据储存在二位数组中  
		for (int i = 0; i < lineArray.Length; i++)
		{
			Array[i] = lineArray[i].Split(',');   // Array.Length =5 表示行数、Array[0].Length=3 表示列数
		}

		levelnumber = Array.Length - 1;

	}

	// 以行和列读取数据
	public string GetDataByRowAndCol(int nRow, int nCol)
	{
		if (Array.Length <= 0 || nRow >= Array.Length)  //行越界的判断
			return "";
		if (nCol >= Array[0].Length)  //列越界的判断
			return ""; 

		return Array[nRow][nCol];
	}

	//以ID和名称读取数据
	public string GetDataByIdAndName(int nId, string strName)
	{
		if (Array.Length <= 0)
			return "";

		int nRow = Array.Length;
		int nCol = Array[0].Length;
		for (int i = 0; i < nRow; ++i)
		{
			string strId = string.Format("\n{0}", nId);
			if (Array[i][0] == strId)
			{
				for (int j = 0; j < nCol; ++j)
				{
					if (Array[0][j] == strName)
					{
						return Array[i][j];
					}
				}
			}
		}

		return "";



	}

	void FixedUpdate()
	{
		if (TestClick.flag)
		{

//			if (EnterClick.verify_flag) {           
//				time_flag++;
//			}

			time_flag++;

			obj.transform.localPosition = new Vector2(15, float.Parse(GetDataByIdAndName(count, "Distance")));  //x轴方向position(float.Parse将string强制转换成float)
			obj.transform.localScale = new Vector2(26, float.Parse(GetDataByIdAndName(count, "Width")));     //x轴方向宽度

			ID = float.Parse(GetDataByIdAndName(count, "id"));

			if (collision1 == 1)      //if (collision1)  
			{
				flag++;

			}
			else
			{
				flag = 0;

			}


			if (flag == 100 || time_flag == 1500)     //时间设置1.0s
			{
				//   Debug.Log("delay测试开始");
				time_flag = 0;
				// 3s之后目标物体改变，给测试者休息的时间 3s
				StartCoroutine(Delay.run(() =>
					{
						flag = 0;
						count++;
						time_flag = 0;  // 当下一个目标出现的时候，time-flag重新开始记录

						if (count == levelnumber)
						{
							GameObject.Destroy(obj);  //将物体销毁
							//Debug.Log("物体被销毁");
							destroy_mark = true;
						}

					}, 5));
			}


			//       if (count == levelnumber)
			// { 	
			////  temp = false;
			//           GameObject.Destroy(obj);  //将物体销毁
			//       }
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{

		if (col.gameObject.name == "MoveRacket_force")
		{
			rend.material.color = Color.red;
			collision1 = 1;    //true-1
		}

	}
	void OnCollisionExit2D(Collision2D col)
	{
		if (col.gameObject.name == "MoveRacket_force")
		{
			//if (count % 2 == 0) { rend.material.color = Color.green; }
			//if (count % 2 != 0) { rend.material.color = Color.blue; }
			rend.material.color = Color.green;
			collision1 = 0;  //false-0
		}
	}

	private void OnGUI()
	{
		
		if (count!=counttmp && time_flag >= 20)   //ID变化时（新目标出现），提示"开始"
			{
				
				GUIStyle aa = new GUIStyle();
				aa.normal.background = texture2D;
				aa.normal.textColor = Color.red;   //设置字体颜色
				aa.fontSize = 200;       //这是字体大小
				aa.alignment = TextAnchor.MiddleCenter;
			    GUI.Button (new Rect (Screen.width * 0.34f, Screen.height * 0.4f, 1500, -100), "开始", aa);					    
			    
			    StartCoroutine(Delay.run(() =>
				{
					counttmp = count;
					//Debug.Log("counttmp" + counttmp);
				}, 1));
			 }


		//Debug.Log("time_flag"+time_flag);
		if (flag >= 100|| time_flag >= 1470)          //1970
		{
			GUIStyle bb = new GUIStyle();
			bb.normal.background = texture2D;
			bb.normal.textColor = Color.red;   //设置字体颜色
			bb.fontSize = 200;       //这是字体大小
			bb.alignment = TextAnchor.MiddleCenter;
			GUI.Button(new Rect(Screen.width * 0.34f, Screen.height * 0.4f, 1500, -100), "请放手", bb);

		}
		//if (ishint)
		//{
		//    GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.75f, 200, 30), "请上移木条");
		//}
	 }

}