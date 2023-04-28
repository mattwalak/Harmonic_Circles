using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DEBUG_Progress_Slider : MonoBehaviour
{
    public Slider slider;

    private OSC osc;
    private NoiseGameManager gameManager;

    void Start(){
        osc = (OSC)FindObjectOfType(typeof(OSC));
        gameManager = (NoiseGameManager)FindObjectOfType(typeof(NoiseGameManager));
    }

    public void OnSliderUpdate(){
        OscMessage oscMsg = new OscMessage();
        oscMsg.address = "/updateStage3Progression";
        oscMsg.values.Add(slider.value);
        osc.Send(oscMsg);

        gameManager.frontCover.SetOpacity(slider.value);
    }
}
