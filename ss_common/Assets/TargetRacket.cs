using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRacket : MonoBehaviour
{
    private int sum;
    private float[,] trial = new float[100, 10];
    private int count = 0;
	public static bool collision = false;
	public static bool temp = true;
    private int flag = 0;
	private int time_flag = 0;
	public static float ID = 0;

	public static int levelnumber;
	public string[][] Array;

    public GameObject obj;
    public Renderer rend;

	public  void Awake()
	{
		obj = GameObject.Find("TargetRacket");
		rend = obj.GetComponent<Renderer>();

		//读取csv二进制文件  
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
		// Debug.Log(Array.Length-1);
		levelnumber = Array.Length - 1;
		//Debug.Log("TargetRacket"+ levelnumber); //（为9个ID）0-8 9+1=10

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



    // Update is called once per frame
    void Update()
    {   
		if (temp){
       // obj.transform.localPosition = new Vector3(1, trial[count, 1], 0);
        //obj.transform.localScale = new Vector3(26, trial[count, 2], 0);

		obj.transform.localPosition = new Vector2(-8, float.Parse(GetDataByIdAndName(count,"Distance")));  //x轴方向position(float.Parse将string强制转换成float)
		obj.transform.localScale = new Vector2(26, float.Parse(GetDataByIdAndName(count, "Width")));     //x轴方向宽度

		ID = float.Parse(GetDataByIdAndName(count, "id"));

		time_flag++;

        if (collision)
        {
            flag++;

        }
        else
        {
            flag = 0;
        }
        if(flag>60 || time_flag>1000)
        {
            count++;
            flag = 0;
			time_flag = 0;
        }

		if (count == levelnumber)
		{   
				
			temp = false;
		}

      }
	}
    void OnCollisionEnter2D(Collision2D col)
    {
        rend.material.color = Color.red;
        collision = true;

    }
    void OnCollisionExit2D(Collision2D col)
    {
        rend.material.color = Color.green;
        collision = false;
    }

}