using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticData : MonoBehaviour
{
    public static IEnumerator RunWithDelay(float delay, Action run){
        yield return new WaitForSeconds(delay);
        run();
    }
}
