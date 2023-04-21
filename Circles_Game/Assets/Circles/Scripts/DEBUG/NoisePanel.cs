using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoisePanel : MonoBehaviour
{
    public int panelIndex;
    public OSC osc;
    public Slider slider;

    public void OnImpulseReceive(){
        OscMessage msg = new OscMessage();
        msg.address = "/impulseChange";
        msg.values.Add(panelIndex);
        osc.Send(msg);
    }

    public void OnSliderValueChanged(){
        OscMessage msg = new OscMessage();
        msg.address = "/sliderChange";
        msg.values.Add(panelIndex);
        msg.values.Add(slider.value);
        osc.Send(msg);
    }

    public void OnPadValueChanged(Vector2 val){
        OscMessage msg = new OscMessage();
        msg.address = "/padChange";
        msg.values.Add(panelIndex);
        msg.values.Add(val.x);
        msg.values.Add(val.y);
        osc.Send(msg);
    }


}
