using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using UnityEngine.UI;



public class MoveRacket : MonoBehaviour
{

//    public float speed = 30;

	UdpClient acq_pressureClient = new UdpClient();         // pressure acquisition system
    //IPEndPoint object will allow us to read datagrams sent from any source.
    IPEndPoint RemoteIpEndPoint1 = new IPEndPoint(IPAddress.Any, 0);

    UdpClient neuromorphicClient = new UdpClient();     //neuromorphic system
    IPEndPoint RemoteIpEndPoint2 = new IPEndPoint(IPAddress.Any, 0);

    EndPoint Remote1;
    EndPoint Remote2;

    public GameObject obj;
    public Renderer rend;


    List<float> listToHoldData;
    List<float> listToHoldTime;



	private SerialPort sp1 = new SerialPort();    //CCH 
//	private SerialPort sp2 = new SerialPort();

	static int byteBufferLength1 = 9;
	byte[] byteBuffer1 = new byte[byteBufferLength1];

	static int byteBufferLength2 = 9;
	byte[] byteBuffer2 = new byte[byteBufferLength2];

//	static int receivedDataLength = 8;
//	byte[] receivedData = new byte[receivedDataLength];//声明一个临时数组存储当前来的串口数据   /创建接收字节数组



	public static float muscle_force = 0.0f;
    public static float barForceInMilliNewton = 0.0f;
    int n = 0;
	float k = 0.05f;
	float b = 0.9f;
	double Lce = 0.0f;
	int positioncur = 0;



    void Start()
    {
		Console.WriteLine("This is a Client, host name is {0}", Dns.GetHostName());//获取本地计算机的主机名

        try
        {
			acq_pressureClient.Connect("127.0.0.1", 8001);   //注意端口保持一致
            neuromorphicClient.Connect("192.168.0.100", 5000);   //注意IP和port
        }

        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }



        listToHoldData = new List<float>();
        listToHoldTime = new List<float>();


        obj = GameObject.Find("MoveRacket");
        //rend = obj.GetComponent<Renderer>();


		sp1 = new SerialPort("\\\\.\\COM2", 921600, Parity.None, 8, StopBits.One);
//		sp2 = new SerialPort("\\\\.\\COM11", 921600, Parity.None, 8, StopBits.One);

		sp1.Open();
//		sp2.Open();

    }




    void FixedUpdate()
    {


        //Send something to Motor

        double positionangle1 = Math.Sin(n * Math.PI / 180) * 2000;      //Write position 
		n++;     //n = n + 1; 
		int position1 = (int)positionangle1;
		//position = Math.Abs(position);

		double positionangle2 = Math.Cos(n * Math.PI / 180) * 2000;      
		n++;     
		int position2 = (int)positionangle2;


		byte ID = 0x01;
		byte Instuction = 0x02;
		byte MemAddr = 0x37;
		//int position = 1300;
		byte positioncur_L = (byte)(positioncur & 0xFF);
		byte positioncur_H = (byte)(positioncur >> 8);
		byte position2_L = (byte)(position2 & 0xFF);
		byte position2_H = (byte)(position2 >> 8);
		byte nLen = 0x02;

		byte msgLen = (byte)(nLen + 0x02);
		byte CheckSum1 = (byte)(msgLen + ID + Instuction + MemAddr + positioncur_L + positioncur_H);
		byte CheckSum2 = (byte)(msgLen + ID + Instuction + MemAddr + position2_L + position2_H);

		byteBuffer1[0] = 0x55;           //帧头
		byteBuffer1[1] = 0xAA;
		byteBuffer1[2] = msgLen;     //帧长度
		byteBuffer1[3] = ID;
		byteBuffer1[4] = Instuction;
		byteBuffer1[5] = MemAddr;
		byteBuffer1[6] = positioncur_L;
		byteBuffer1[7] = positioncur_H;
		byteBuffer1[8] = CheckSum1;


		byteBuffer2[0] = 0x55;           
		byteBuffer2[1] = 0xAA;
		byteBuffer2[2] = msgLen;     
		byteBuffer2[3] = ID;
		byteBuffer2[4] = Instuction;
		byteBuffer2[5] = MemAddr;
		byteBuffer2[6] = position2_L;
		byteBuffer2[7] = position2_H;
		byteBuffer2[8] = CheckSum2;

		sp1.Write(byteBuffer1, 0, byteBuffer1.Length);
		System.Threading.Thread.Sleep(1);

        //		sp2.Write(byteBuffer2, 0, byteBuffer2.Length);
        //		System.Threading.Thread.Sleep(8);




		try
		{

//            Lce1 = Math.Sin(n * Math.PI / 180);    // n= angle
//            n++;
//            Lce1 = Math.Abs(Lce1) + 1;
//            float Lce = (float)Lce1;
//	     	  print("Lce"+Lce);

            // Sends a message to the host to which you have connected.
            Byte[] sendBytes1 = Encoding.ASCII.GetBytes("H");
			acq_pressureClient.Send(sendBytes1, sendBytes1.Length);
            // Blocks until a message returns on this socket from a remote host.
			Byte[] receiveBytes1 = acq_pressureClient.Receive(ref RemoteIpEndPoint1);	
			string recMsg1 = Encoding.ASCII.GetString(receiveBytes1);  //receive message from DAQ
			barForceInMilliNewton = float.Parse(recMsg1);

			//			Lce = k * barForceInMilliNewton + b;  //1~2   ，
			Lce = 1 + barForceInMilliNewton/20000;     

			Byte[] sendBytes2 = Encoding.ASCII.GetBytes(Lce.ToString());
			neuromorphicClient.Send(sendBytes2, sendBytes2.Length);  //send spring length change to neuromorphic system
			// Blocks until a message returns on this socket from a remote host.
            Byte[] receiveBytes2 = neuromorphicClient.Receive(ref RemoteIpEndPoint2);
            string recMsg2 = Encoding.ASCII.GetString(receiveBytes2);   //receive message from neuromorphic system
			muscle_force = float.Parse(recMsg2);    //muscle_force-muscle_force     muscle_force——0~18

			positioncur = (int)(200 + muscle_force*80);   


		}

		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}



        float barHeight = 0.004f * barForceInMilliNewton;
        GetComponent<Rigidbody2D>().position = new Vector2(0, barHeight);
        //obj.transform.position = new Vector2(0, barHeight);


        listToHoldData.Add(barForceInMilliNewton);
        //float t = Time.time;
        listToHoldTime.Add(Time.time);
        //Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //GetComponent<Rigidbody2D>().position = new Vector2(0, mousePosition.y);

    }



    private void OnApplicationQuit()
    {

        string data = "";
        StreamWriter writer = new StreamWriter("test.csv", false, Encoding.UTF8);
                  
        writer.WriteLine(string.Format("{0},{1}", "Time", "Pressure"));

        
        using (var e1 = listToHoldTime.GetEnumerator())
        using (var e2 = listToHoldData.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext())
            {
                var item1 = e1.Current;
                var item2 = e2.Current;

                data += item1.ToString();
                data += ",";
                data += item2.ToString();
                data += "\n";
                // use item1 and item2
            }
        }


        writer.Write(data);

        writer.Close();
    }

}
