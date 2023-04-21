using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkMessage
{
    public string source;

    public string command;

    public int playerNumber; // 1 or 2
    public int updatedConnection; // 1 = found player, -1 = lost player
    public int newPlayerConnected;
    public int circleButtonID;
    public int circleButtonState; // 1 = pressed, -1 if released 
    public int touchState;
    public float touchPosX;
    public float touchPosY;

    public int changeSceneTo; // 2 = change to continuous impulse mode
    public int newKey;

    public string Serialized(){
        return JsonUtility.ToJson(this);
    }
}

