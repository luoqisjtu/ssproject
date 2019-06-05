using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRacket: MonoBehaviour
{
    private int sum;
    private float[,] trial = new float[100, 10];
    private int count = 0;
    private bool collision = false;
    private int flag = 0;

    // Use this for initialization

    public GameObject obj;
    public Renderer rend;
    void Start()
    {

        CSVHelper.Instance().ReadCSVFile("config", (table) => {
            sum = int.Parse(table["0"]["Count"]);
//            Debug.Log(sum);
            for (int i = 0; i <= sum; i++)
            {
                trial[i, 0] = float.Parse((table[i.ToString()])["id"]);
                trial[i, 1] = float.Parse((table[i.ToString()])["Distance"]);
                trial[i, 2] = float.Parse((table[i.ToString()])["Width"]);
            }
        });

        obj = GameObject.Find("TargetRacket");
        rend = obj.GetComponent<Renderer>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {

            obj.transform.localPosition = new Vector3(-8, trial[count, 1], 0);
            obj.transform.localScale = new Vector3(26, trial[count, 2], 0);

            if (collision)
            {
                flag++;

            }
            else
            {
                flag = 0;
            }
            if (flag > 30)
            {
                count++;
                flag = 0;
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