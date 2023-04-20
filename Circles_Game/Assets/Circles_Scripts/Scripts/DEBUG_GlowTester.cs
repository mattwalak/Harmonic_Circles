using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DEBUG_GlowTester : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public NoiseGameManager gameManager;

    public void OnActivateGlowClicked(){
        NetworkMessage msg = new NetworkMessage();
        msg.circleButtonID = dropdown.value;
        msg.circleButtonState = 1;
        gameManager.HandleCircleButtonClick(msg);
    }

    public void OnDeactivateGlowClicked(){
        NetworkMessage msg = new NetworkMessage();
        msg.circleButtonID = dropdown.value;
        msg.circleButtonState = -1;
        gameManager.HandleCircleButtonClick(msg);
    }
}
