using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Log : MonoBehaviour
{
    public TMP_InputField textField;

    public void Add(string str){
        if(NetworkManager.DEBUG){
            Debug.Log(str);
        }

        textField.text += str + "\n";
    }
}
