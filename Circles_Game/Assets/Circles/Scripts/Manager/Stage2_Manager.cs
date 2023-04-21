using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2_Manager : MonoBehaviour
{
    public NoiseGameManager noiseGameManager;
    private bool stageIsActive = false;

    private const float TIME_BETWEEN_BLASTS_MAX = 0.5f;
    private const float TIME_BETWEEN_BLASTS_MIN = 0.03f;

    private int keysLeft;
    private float currentTimeBetweenBlasts;

    private float blastCounter_t = 0f;
    

    public Vector2 ballSourcePosition;

    public void EnterStage(){
        keysLeft = noiseGameManager.NUM_KEYS;
        noiseGameManager.UpdateProgressCounter(keysLeft);
        currentTimeBetweenBlasts = TIME_BETWEEN_BLASTS_MAX;
        ballSourcePosition = new Vector2(0, 0);

        stageIsActive = true;
    }

    public void ExitStage(){
        stageIsActive = false;
    }

    void Update(){
        if(stageIsActive){
            blastCounter_t += Time.deltaTime;

            if(Input.GetMouseButton(0)){
                Vector2 mousePos = Input.mousePosition;
                Vector2 worldPos = noiseGameManager.mainCamera.ScreenToWorldPoint(mousePos);
                ballSourcePosition = worldPos;
            }

            while(blastCounter_t > currentTimeBetweenBlasts){
                noiseGameManager.blastParticlesAtLocation(ballSourcePosition);
                blastCounter_t -= currentTimeBetweenBlasts;
            }
        }
    }

    public void HandleSendTouchPositionData(NetworkMessage msg){
        ballSourcePosition = new Vector2(msg.touchPosX, msg.touchPosY);
        ballSourcePosition = ballSourcePosition * noiseGameManager.circleBounds.radius;
    }

    public void HandleKeyChange(NetworkMessage msg){
        OscMessage oscMessage = new OscMessage();
        oscMessage.address = "/keyChange";
        oscMessage.values.Add(msg.newKey);
        noiseGameManager.SendOscMessage(oscMessage);
    }

}
