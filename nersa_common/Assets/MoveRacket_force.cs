using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class MoveRacket_force : MonoBehaviour
{
    public float speed = 30;

    byte[] data = new byte[1024];
    string input, stringData;

    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);  //实现 Berkeley 套接字接口
    public IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);  //定义服务端

    EndPoint Remote;
    int recv;


    public GameObject obj;
    public Renderer rend;

	private float IDrecord;
	private float collisionrecord1;  //  bool
	private float collisionrecord2;  //  bool
	public int levelcount;
	AllList[] Levels;
	public List<float> FromZeroIDtime;
	public int IDframe;
	//public bool isShow;
	public float barForceInMilliNewton;  
	public static float Lce_mo;
	public float force_mo;

	void CreateList(int n)
	{
		for (int i = 0; i < n; i++)
		{
			Levels[i] = new AllList();
		}
	}

    void Start()
    {
        Console.WriteLine("This is a Client, host name is {0}", Dns.GetHostName());//获取本地计算机的主机名
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);


        string welcome = "K";
        data = Encoding.ASCII.GetBytes(welcome);  //数据类型转换
        server.SendTo(data, data.Length, SocketFlags.None, ip);  //发送给指定服务端

        Remote = (EndPoint)sender;
        recv = server.ReceiveFrom(data, ref Remote);//获取客户端，获取客户端数据，用引用给客户端赋值 
        data = new byte[1024];

		levelcount = TargetRacket_force.levelnumber;  
		//Debug.Log("f"+ levelcount); 

		Levels = new AllList[levelcount];
		CreateList(levelcount);

        obj = GameObject.Find("MoveRacket_force");
        //rend = obj.GetComponent<Renderer>();

    }


	void FixedUpdate()
    {
		
		IDrecord = TargetRacket_force.ID;
		collisionrecord1 = TargetRacket_force.collision1;
		collisionrecord2 = Prohibited_area.collision2;
		IDframe = TargetRacket_force.time_flag;
	    force_mo = MoveRacket.muscle_force;
		//Debug.Log (IDrecord);

		if (TestClick.flag) {
			//print ("force");
			server.SendTo(Encoding.ASCII.GetBytes("H"), Remote);//发送信息
			data = new byte[1024];//对data清零
			recv = server.ReceiveFrom(data, ref Remote);//获取客户端，获取服务端端数据，用引用给服务端赋值，实际上服务端已经定义好并不需要赋值
			stringData = Encoding.ASCII.GetString(data, 0, recv);//字节数组转换为字符串  //输出接收到的数据 
			Console.WriteLine(stringData);

		    barForceInMilliNewton = (float)Convert.ToInt32(stringData);

			//			Lce = k * barForceInMilliNewton + b;  //1~2   ，
//			Lce_mo = 1 + barForceInMilliNewton/(20*1000);   //18
			Lce_mo = 2 - barForceInMilliNewton/(20*1000);   //18
//			print(Lce_mo);
			                
			float barHeight = 0.003f * barForceInMilliNewton;                   
			GetComponent<Rigidbody2D>().position = new Vector2(0, barHeight);
            //obj.transform.position = new Vector2(0, barHeight);


			if(TargetRacket_force.destroy_mark == false)
			{
				Levels[(int)IDrecord].allList(Time.time, barForceInMilliNewton, IDrecord, barHeight, collisionrecord1, collisionrecord2, Lce_mo, force_mo, IDframe); 

			}

			//if (barHeight <= 0.06f)
			//{
			//	isShow = true;
			//}
			//else
			//{
			//	isShow = false;
			//}

		}

    }
		

    private void OnApplicationQuit()
    {

		SaveCSV.createfile();
		//for (int n = 0; n < levelcount; n++)
		//{
		//    //// 将listToHoldTime列表的每一个值都减去此列表的第一个值
		//  //  List<float> AdjustIDtime = Levels[n].listToHoldTime;


		//    for (int m = 0; m < Levels[n].listToHoldTime.Count; m++)
		//    {
		//        FromZeroIDtime.Add(Levels[n].listToHoldTime[m] - Levels[n].listToHoldTime[0]);

		//    }
		//    SaveCSV.savedata("Force_bar" + n.ToString() + ".csv", FromZeroIDtime, Levels[n].listToHoldData, Levels[n].listToHoldID, Levels[n].listToHoldBarHeight, Levels[n].listToHoldtimeflag);
		//}


		for (int n = 0; n < levelcount; n++)
		{
			SaveCSV.savedata("Force_bar" + n.ToString() + ".csv", Levels[n].listToHoldTime, Levels[n].listToHoldData, Levels[n].listToHoldID, Levels[n].listToHoldBarHeight,Levels[n].listToHoldcollision1, Levels[n].listToHoldcollision2, Levels[n].listToHoldLce, Levels[n].listToHoldmuscle_force, Levels[n].listToHoldtimeflag);  

		}
			

	}


}
