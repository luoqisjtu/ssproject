using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveCSV : MonoBehaviour {

    // 将保存数据的代码进行封装
    public static void savedata(string CSVname, List<float> listToHoldTime, List<float> listToHoldData, List<float> listToHoldID, List<float> listToHoldBarHeight, List<bool> listToHoldcollision)
    {

        string data = "";
        StreamWriter writer = new StreamWriter(CSVname, false);//。如果此值为false，则创建一个新文件，如果存在原文件，则覆盖。如果此值为true，则打开文件保留原来数据，如果找不到文件，则创建新文件。
        writer.WriteLine(string.Format("{0},{1},{2},{3},{4}", "Time", "Pressure", "ID", "barHeight", "collision"));


        using (var e1 = listToHoldTime.GetEnumerator())
        using (var e2 = listToHoldData.GetEnumerator())
        using (var e3 = listToHoldID.GetEnumerator())
        using (var e4 = listToHoldBarHeight.GetEnumerator())
        using (var e5 = listToHoldcollision.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext() && e4.MoveNext() && e5.MoveNext())
            {
                var item1 = e1.Current;
                var item2 = e2.Current;
                var item3 = e3.Current;
                var item4 = e4.Current;
                var item5 = e5.Current;
               // var item6 = e6.Current;

                data += item1.ToString();
                data += ",";
                data += item2.ToString();
                data += ",";
                data += item3.ToString();
                data += ",";
                data += item4.ToString();
                data += ",";
                data += item5.ToString();
                data += "\n";
                // use item1 and item2
            }
        }

        writer.Write(data);

        writer.Close();
    }


}
