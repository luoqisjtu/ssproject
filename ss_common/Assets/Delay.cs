using UnityEngine;
using System.Collections;
using System;

public class Delay
{
    public static IEnumerator run(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
}