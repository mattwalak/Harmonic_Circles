using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DEBUG_ChromaticTester : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public NoiseGameManager gameManager;

    public void OnSendNoteClicked(){
        NetworkMessage msg = new NetworkMessage();
        msg.newKey = dropdown.value;
        gameManager.HandleKeyChange(msg);
        /*
        msg.circleButtonID = dropdown.value;
        msg.circleButtonState = 1;
        gameManager.HandleCircleButtonClick(msg);*/
    }
}
