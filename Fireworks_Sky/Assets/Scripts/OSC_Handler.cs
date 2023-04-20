using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSC_Handler : MonoBehaviour
{
    public OSC osc;

    public void SendTestMessage(){
        OscMessage msg = new OscMessage();
        msg.address = "/sinNote";
        msg.values.Add(Random.Range(2000f, 20000f));
        osc.Send(msg);

        Debug.Log("Send test message");
    }
}
