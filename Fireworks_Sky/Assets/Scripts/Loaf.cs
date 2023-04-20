using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loaf : MonoBehaviour
{
    public void Emphasize(){
        Debug.Log("Emphasize called");
        LeanTween.scale(gameObject, new Vector3(.2f, .2f, .2f), .32f).setOnComplete(DeEmphasize);
        
    }

    public void DeEmphasize(){
        LeanTween.scale(gameObject, new Vector3(.1f, .1f, .1f), .2f);
    }
}
