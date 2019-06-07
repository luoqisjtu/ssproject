using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using UnityEngine.UI;
//using System.math;
using System.Threading;
using DisruptorUnity3d;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;

public class MoveRacket : MonoBehaviour
{
    public InputField inputvariate1;
    public InputField inputvariate2;
    public float input1 = 1;
    public float input2 = -1;     //给input1、input2设定初始值
    //int count = 0;
	public float IDrecord_f;
	public int levelcount_f;
	AllList_f[] Levels;


//	UdpClient acq_pressureClient = new UdpClient();         // pressure acquisition system
//	//IPEndPoint object will allow us to read datagrams sent from any source.
//	IPEndPoint RemoteIpEndPoint1 = new IPEndPoint(IPAddress.Any, 0);

    UdpClient neuromorphicClient = new UdpClient();     //neuromorphic system
    IPEndPoint RemoteIpEndPoint2 = new IPEndPoint(IPAddress.Any, 0);

    EndPoint Remote1;
    EndPoint Remote2;


	private Thread _queueThread;

	public GameObject obj;
	public Renderer rend;
	public GameObject obj_2;
	public static bool cont_a = true;



	private SerialPort sp1 = new SerialPort();    //CCH 
	//	private SerialPort sp2 = new SerialPort();

	static int byteBufferLength1 = 9;
	byte[] byteBuffer1 = new byte[byteBufferLength1];

	static int byteBufferLength2 = 9;
	byte[] byteBuffer2 = new byte[byteBufferLength2];

	//	static int receivedDataLength = 8;
	//	byte[] receivedData = new byte[receivedDataLength];//声明一个临时数组存储当前来的串口数据   /创建接收字节数组


	public static float muscle_force = 0.0f;    //motorCommand
//	public static float barForceInMilliNewton = 0.0f;
    int n = 0;
//	public static float Lce = 0.0f;
    //float Lce1 = 0.0f;
	public float Lce = 0.0f;
	int positioncur = 0;
	int neuromorphic_off_send = 0;
	int neuromorphic_on_send = 0;

//    float rawEmg = 0.0f;
	float averagefilter = 0.0f;
	float butterworthfilter = 0.0f;  
//	float bayesfilter = 0.0f;
	float emg_send = 0.0f;  

	public float rawEmg;
	public float bayesfilter;
	private bool running = false;

	//ring buffer
	static readonly RingBuffer<float> Queue = new RingBuffer<float>(10);


    EmgModule myEmg = new EmgModule();
	BayesFilter myBayesFilter = new BayesFilter();
    Simulator mySimulator = new Simulator();




    float[] a1 = { 1f, 1.7600f, 1.1829f, 0.2781f };           // 高通滤波
	float[] b1 = { 0.0181f, -0.0543f, 0.0543f, -0.0181f };    // 高通滤波  
    //float[] a2 = {1f,-0.5772f, 0.4218f, -0.0563f};
    //float[] b2 = {0.0985f, 0.2956f, 0.2956f, 0.0985f};//20Hz低通滤波
    //float[] a2 = { 1f, -1.7600f, 1.1829f, -0.2781f };
    //float[] b2 = { 0.0181f, 0.0543f, 0.0543f, 0.0181f };//10Hz低通滤波
    //float[] a2 = { 1f, -2.3741f, 1.9294f, -0.5321f };
    //float[] b2 = { 0.0029f, 0.0087f, 0.0087f, 0.0029f };//5Hz低通滤波
    //float[] a2 = { 1f, -2.7488f, 2.5282f, -0.7776f };
    //float[] b2 = { 0.0002f, 0.0007f, 0.0007f, 0.0002f };//2Hz低通滤波
    float[] a2 = { 1f, -2.8744f, 2.7565f, -0.8819f };
    float[] b2 = { 0.00003f, 0.00009f, 0.00009f, 0.00003f };//1Hz低通滤波
    float[] x = new float[4];
    float[] y1 = new float[4];//高通
    float[] y2 = new float[4];//整流
    float[] y3 = new float[4];//低通

    float[] aa = new float[10];      //均值滤波阶数（oder）-10阶



	void CreateList(int n)
	{
		for (int i = 0; i < n; i++)
		{
			Levels[i] = new AllList_f();
		}
	}



    void Start()
    {

        Console.WriteLine("This is a Client, host name is {0}", Dns.GetHostName());//获取本地计算机的主机名
        try
        {
//			acq_pressureClient.Connect("127.0.0.1", 8001);   //注意端口保持一致
			neuromorphicClient.Connect("192.168.0.100", 5000);   //注意IP和port
        }

        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }


		running = true;
		_queueThread = new Thread(queueWorker);
		_queueThread.IsBackground = true;
		_queueThread.Start();

        myEmg.startEmg();


		obj = GameObject.Find("MoveRacket");
		obj_2 = GameObject.Find ("Interface");
		//.GetComponent<Renderer>().enabled = false;
		obj_2.GetComponent<Renderer>().enabled = false;
		obj_2.transform.position = new Vector2(280, 260);

		GameObject inputvariate1 = GameObject.Find("Canvas/inputvariate1");
		GameObject inputvariate2 = GameObject.Find("Canvas/inputvariate2");


		levelcount_f = TargetRacket_force.levelnumber;  
		Levels = new AllList_f[levelcount_f];
		CreateList(levelcount_f);



		sp1 = new SerialPort("\\\\.\\COM2", 921600, Parity.None, 8, StopBits.One);
		//		sp2 = new SerialPort("\\\\.\\COM11", 921600, Parity.None, 8, StopBits.One);

		sp1.Open();
		//		sp2.Open();



	
    }



    private float EmgAverage(int order, float emg)   //   均值滤波   order(阶数-order)
    {

        float sum = 0;
        float filterEmg = 0;
        for (int i = order - 1; i > 0; i--)
        {
            aa[i] = aa[i - 1];
            sum += aa[i];
        }
        aa[0] = emg;
        //if (a[order - 1] == 0) myEmg.emgData[0] = 0;
        filterEmg = (sum + aa[0]) / order;
        return filterEmg;

    }



	private float ButterworthFilter(float emg) //  巴特沃斯滤波-3阶
    {
        for (int i = 3; i > 0; i--)
        {
            x[i] = x[i - 1];
            y1[i] = y1[i - 1];
        }

        x[0] = emg;
        y1[0] = (b1[3] * x[3] + b1[2] * x[2] + b1[1] * x[1] + b1[0] * x[0] - (a1[3] * y1[3] + a1[2] * y1[2] + a1[1] * y1[1])) / a1[0];

        for (int i = 3; i > 0; i--)
        {
            y2[i] = y2[i - 1];
            y3[i] = y3[i - 1];
        }

        y2[0] = Math.Abs(y1[0]);
        y3[0] = (b2[3] * y2[3] + b2[2] * y2[2] + b2[1] * y2[1] + b2[0] * y2[0] - (a2[3] * y3[3] + a2[2] * y3[2] + a2[1] * y3[1])) / a2[0];

        return y3[0];

    }



    public float ChooseMode(string aa)
    {
        float chemg;
        switch (aa)
        {
            case "real":
                chemg = myEmg.emgData[0];
                //return chemg;
                break;

            case "sim":
                chemg = (float)mySimulator.storedata();
                break;

            default:
                chemg = 0;
                break;
        }
        return chemg;

    }



    void FixedUpdate()         // 设为0.01s
    {
		
		float rawEmg = 0.0f;

		IDrecord_f = TargetRacket_force.ID;
		Lce = MoveRacket_force.Lce_mo;

        if (TestClick.flag)
        {
			
            if (EnterClick.verify)
            {
                input1 = float.Parse(inputvariate1.text);    // string to float(字符串转浮点型)
                input2 = float.Parse(inputvariate2.text);
                EnterClick.verify = false;
            }
            //print (input1);
            //print (input2);

			//print ("rawEmg");
			rawEmg = ChooseMode("real");

			while (Queue.TryDequeue(out rawEmg))
			{
				bayesfilter = myBayesFilter.UpdateEst(Math.Abs(rawEmg) * 6e2f);
				//			Debug.Log(bayesfilter.ToString());
			}

			  

			averagefilter = EmgAverage(10, Math.Abs(rawEmg * 80000 / 40));       //均值滤波
			butterworthfilter = ButterworthFilter(rawEmg* 80000/4);                     //巴特沃斯滤波
			emg_send = Mathf.Max(0, bayesfilter * input1 + input2);   //butterworthfilter    bayesfilter



            //for (double Lce = 1.0; Lce < 2.0; Lce += 0.01)
            try
            {
                //Lce = Math.Sin(n * Math.PI / 180);    // n= angle
                //n++;
                //Lce = Math.Abs(Lce) + 1;
                //float Lce1 = (float)Lce;   


				// Sends a message to the host to which you have connected.
//				Byte[] sendBytes1 = Encoding.ASCII.GetBytes("H");
//				acq_pressureClient.Send(sendBytes1, sendBytes1.Length);
				// Blocks until a message returns on this socket from a remote host.
//				Byte[] receiveBytes1 = acq_pressureClient.Receive(ref RemoteIpEndPoint1);	
//				string recMsg1 = Encoding.ASCII.GetString(receiveBytes1);  //receive message from DAQ
//				barForceInMilliNewton = float.Parse(recMsg1);

//				//			Lce = k * barForceInMilliNewton + b;  //1~2   ，
//				Lce = 1 + barForceInMilliNewton/20000;     

				Byte[] sendBytes2 = Encoding.ASCII.GetBytes(Lce.ToString());
				neuromorphicClient.Send(sendBytes2, sendBytes2.Length);  //send spring length change to neuromorphic system
				// Blocks until a message returns on this socket from a remote host.
				Byte[] receiveBytes2 = neuromorphicClient.Receive(ref RemoteIpEndPoint2);
				string recMsg2 = Encoding.ASCII.GetString(receiveBytes2);   //receive message from neuromorphic system
				muscle_force = float.Parse(recMsg2);    //motorCommand-muscle_force     muscle_force——0~18

				neuromorphic_on_send = (int)((2000 - emg_send*50)  - muscle_force*80);   
				neuromorphic_off_send = (int)((2000 - emg_send*50) - Lce/100);



				if (NeuromorphicClick.enter_neuromorphic)
				{

					positioncur = neuromorphic_on_send;         //send EMG + muscle force to linear motor
				}

				else
				{
					positioncur = neuromorphic_off_send;      //send (EMG + 0) or (EMG + feedback_number) to linear motor
				}

				if (positioncur >= 2000)

					positioncur=2000;
//				    print (positioncur);

            }


            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


//			float barHeight = 0.008f * barForceInMilliNewton;
//			GetComponent<Rigidbody2D>().position = new Vector2(0, barHeight);

            GetComponent<Rigidbody2D>().position = new Vector2(30, emg_send);

            //Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //GetComponent<Rigidbody2D>().position = new Vector2(0, mousePosition.y);




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
			System.Threading.Thread.Sleep(5);

			//		sp2.Write(byteBuffer2, 0, byteBuffer2.Length);
			//		System.Threading.Thread.Sleep(8);


        }




		if(TargetRacket_force.destroy_mark == false)
		{
			Levels[(int)IDrecord_f].allList(Time.time, IDrecord_f, rawEmg, averagefilter, butterworthfilter, bayesfilter, emg_send); 

		}



		if(Input.GetKeyDown(KeyCode.Z)){

			cont_a = !cont_a;

			obj_2.GetComponent<Renderer>().enabled = cont_a;
			inputvariate1.gameObject.SetActive(!cont_a);
			inputvariate2.gameObject.SetActive(!cont_a);
		}
	
    }



	private void queueWorker()
	{
		while (running)
		{
			//			Debug.Log ("Here");

			float newEmg = myEmg.getOneSample();

			//			Debug.Log ("Here2");


			Queue.Enqueue(newEmg);
		}

	}



    private void OnApplicationQuit()
    {

		running = false;

		//_queueThread.Join ();
		myEmg.stopEmg();
        myEmg.stopEmg();

        // *** Shut down nanoTecServer
        try
        {
//			acq_pressureClient.Close();
            neuromorphicClient.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

 

		SaveCSV_f.createfile();

		for (int n = 0; n < levelcount_f; n++)
		{
			SaveCSV_f.savedata("emg_data" + n.ToString() + ".csv", Levels[n].listToHoldTime, Levels[n].listToHoldID, Levels[n].listToHoldrawEmg, Levels[n].listToHoldaveragefilter, Levels[n].listToHoldbutterworthfilter, Levels[n].listToHoldbayesfilter, Levels[n].listToHoldemg_send);  
		}

                
    }

}
