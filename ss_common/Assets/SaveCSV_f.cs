using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveCSV_f : MonoBehaviour {

    public static string filename;

   
    public static void createfile() {
        //  string filename;

		string path = @"D:\Luoqi\Code\local_ssproject\Fitts_law_emg_data";
        //获取当前系统的时间命名文件夹
        DateTime dt = DateTime.Now;
       
       filename = dt.Hour.ToString() + "_" + dt.Minute.ToString() + "_" + dt.Second.ToString();
       //Debug.Log("time" + filename);
        //如果是创建子文件夹
        DirectoryInfo dir = new DirectoryInfo(path);
        dir.CreateSubdirectory(filename);

    }
    // 将保存数据的代码进行封装
	public static void savedata(string CSVname, List<float> listToHoldTime, List<float> listToHoldID, List<float> listToHoldrawEmg, List<float> listToHoldaveragefilter, List<float> listToHoldbutterworthfilter, List<float> listToHoldbayesfilter, List<float> listToHoldemg_send)  
    {

        string data = "";
 
		FileStream fs = new FileStream(@"D:\Luoqi\Code\local_ssproject\Fitts_law_emg_data\" + filename +"\\"+ CSVname, FileMode.Create, FileAccess.Write);//创建写入文件 
 
        StreamWriter writer = new StreamWriter(fs);
      //  StreamWriter writer = new StreamWriter(CSVname, false);//。如果此值为false，则创建一个新文件，如果存在原文件，则覆盖。如果此值为true，则打开文件保留原来数据，如果找不到文件，则创建新文件。
		writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}", "Time", "ID", "rawEmg", "averagefilter", "butterworthfilter", "bayesfilter", "emg_send"));

        using (var e1 = listToHoldTime.GetEnumerator())
		using (var e2 = listToHoldID.GetEnumerator())
		using (var e3 = listToHoldrawEmg.GetEnumerator())
        using (var e4 = listToHoldaveragefilter.GetEnumerator())
		using (var e5 = listToHoldbutterworthfilter.GetEnumerator())
        using (var e6 = listToHoldbayesfilter.GetEnumerator())
        using (var e7 = listToHoldemg_send.GetEnumerator())
        {
			while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext() && e4.MoveNext() && e5.MoveNext() && e6.MoveNext()&& e7.MoveNext())
            {
                var item1 = e1.Current;
                var item2 = e2.Current;
                var item3 = e3.Current;
                var item4 = e4.Current;
                var item5 = e5.Current;
                var item6 = e6.Current;
				var item7 = e7.Current;
                data += item1.ToString();
                data += ",";
                data += item2.ToString();
                data += ",";
                data += item3.ToString();
                data += ",";
                data += item4.ToString();
                data += ",";
                data += item5.ToString();
                data += ",";
                data += item6.ToString();
				data += ",";
				data += item7.ToString();
                data += "\n";
                // use item1 and item2
            }
        }

        writer.Write(data);
        writer.Close();


    }
		
}
