using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private int harmonyIndex = 0; // for now, 0 = C, 1 = F

    private int[] bassOptions_C = new int[]{0, 7};
    private int[] harmonyOptions_C = new int[]{0, 4, 7, 11};
    private int[] melodyOptions_C = new int[]{0, 2, 4, 5, 7, 9, 11};

    private int[] bassOptions_F = new int[]{0, 5};
    private int[] harmonyOptions_F = new int[]{0, 4, 5, 9};
    private int[] melodyOptions_F = new int[]{0, 2, 4, 5, 7, 9, 11};

    private const float HARMONY_CHANGE_TIME = 10f;
    private float harmonyTimer = 0;


    private OSC osc;

    private void Start(){
        osc = (OSC) FindObjectOfType<OSC>();
    }

    public void OnToggleHarmony(){
        if(harmonyIndex == 0){
            harmonyIndex = 1;
        }else{
            harmonyIndex = 0;
        }

        SendBassOptions();
        SendHarmonyOptions();
        SendMelodyOptions();
    }

    public void SendBassOptions(){
        OscMessage msg = new OscMessage();
        msg.address = "/setBassOptions";
        if(harmonyIndex == 0){
            msg.values.Add(bassOptions_C.Length);
            foreach(int i in bassOptions_C){
                msg.values.Add(i);
            }
        }else{
            msg.values.Add(bassOptions_F.Length);
            foreach(int i in bassOptions_F){
                msg.values.Add(i);
            }
        }

        osc.Send(msg);
    }

    public void SendHarmonyOptions(){
        OscMessage msg = new OscMessage();
        msg.address = "/setHarmonyOptions";
        if(harmonyIndex == 0){
            msg.values.Add(harmonyOptions_C.Length);
            foreach(int i in harmonyOptions_C){
                msg.values.Add(i);
            }
        }else{
            msg.values.Add(harmonyOptions_F.Length);
            foreach(int i in harmonyOptions_F){
                msg.values.Add(i);
            }
        }

        osc.Send(msg);
    }

    public void SendMelodyOptions(){
        OscMessage msg = new OscMessage();
        msg.address = "/setMelodyOptions";
        if(harmonyIndex == 0){
            msg.values.Add(melodyOptions_C.Length);
            foreach(int i in melodyOptions_C){
                msg.values.Add(i);
            }
        }else{
            msg.values.Add(melodyOptions_F.Length);
            foreach(int i in melodyOptions_F){
                msg.values.Add(i);
            }
        }

        osc.Send(msg);
    }

    private void Update(){
        harmonyTimer += Time.deltaTime;
        if(harmonyTimer > HARMONY_CHANGE_TIME){
            OnToggleHarmony();
            harmonyTimer = 0;
        }
    }
}
