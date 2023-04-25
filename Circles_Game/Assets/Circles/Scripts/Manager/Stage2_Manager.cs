using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2_Manager : MonoBehaviour
{
    public NoiseGameManager noiseGameManager;
    private bool stageIsActive = false;

    private const float TIME_BETWEEN_BLASTS_MAX = 0.5f;
    private const float TIME_BETWEEN_BLASTS_MIN = 0.03f;
    private int[] KEY_TABLE = {0, 2, 4, 6, 8, 10};

    private int keysLeft;
    private float currentTimeBetweenBlasts;

    private float blastCounter_t = 0f;

    private float focusOnPartialsCountdown_t = -1f;
    private float focusOnPartialsResetValue;
    private bool[] focusOnPartialsTrackingBuffer;

    public Vector2 ballSourcePosition;
    private bool sourceEmitsParticles = false;

    public void EnterStage(){
        keysLeft = noiseGameManager.NUM_KEYS;
        noiseGameManager.UpdateProgressCounter(keysLeft);
        currentTimeBetweenBlasts = TIME_BETWEEN_BLASTS_MAX;
        ballSourcePosition = new Vector2(0, 0);
        focusOnPartialsResetValue = noiseGameManager.airParticlePrefab.GetComponent<AirParticle>().expirationTime;
        focusOnPartialsTrackingBuffer = new bool[noiseGameManager.NUM_CIRCLES];
        for(int i = 0; i < focusOnPartialsTrackingBuffer.Length; i++){
            focusOnPartialsTrackingBuffer[i] = false;
        }

        stageIsActive = true;

        // Initiate the first keychange with a fake network message
        NetworkMessage msg = new NetworkMessage();
        msg.newKey = 0;
        HandleKeyChange(msg);

        NetworkMessage msg2 = new NetworkMessage();
        msg2.touchPosX = 0;
        msg2.touchPosY = 0;
        HandleSendTouchPositionData(msg2);
    }

    public void ExitStage(){
        stageIsActive = false;
    }

    void Update(){
        if(stageIsActive){
            blastCounter_t += Time.deltaTime;
            if(focusOnPartialsCountdown_t > 0){
                focusOnPartialsCountdown_t -= Time.deltaTime;
                if(focusOnPartialsCountdown_t <= 0){
                    // NETWORK - Send end of focusOnPartials signal
                    NetworkMessage msg = new NetworkMessage();
                    msg.source = "Game";
                    msg.command = "EndOfFocusOnPartial";
                    noiseGameManager.SendNetworkMessage(msg);
                }
            }

            if(Input.GetMouseButton(0)){
                Vector2 mousePos = Input.mousePosition;
                Vector2 worldPos = noiseGameManager.mainCamera.ScreenToWorldPoint(mousePos);
                ballSourcePosition = worldPos;
            }

            if(sourceEmitsParticles){
                while(blastCounter_t > currentTimeBetweenBlasts){
                    noiseGameManager.blastParticlesAtLocation(ballSourcePosition);
                    blastCounter_t -= currentTimeBetweenBlasts;
                }
            }
            
        }
    }

    public void HandleSendTouchPositionData(NetworkMessage msg){
        ballSourcePosition = new Vector2(msg.touchPosX, msg.touchPosY);
        ballSourcePosition = ballSourcePosition * noiseGameManager.circleBounds.radius;
        
        // NETWORK - Send message to start FocusOnPartials
        NetworkMessage netMsg = new NetworkMessage();
        netMsg.source = "Game";
        netMsg.command = "StartOfFocusOnPartial";
        noiseGameManager.SendNetworkMessage(netMsg);

        // Turn on particle source
        // Reset focus on partials counter
        focusOnPartialsCountdown_t = focusOnPartialsResetValue;
        sourceEmitsParticles = true;
    }

    public void HandleKeyChange(NetworkMessage msg){
        OscMessage oscMessage = new OscMessage();
        oscMessage.address = "/keyChange";
        oscMessage.values.Add(KEY_TABLE[msg.newKey]);
        noiseGameManager.SendOscMessage(oscMessage);

        // Turn off particle source
        sourceEmitsParticles = false;

        // Reset focusOnPartial buffer
        for(int i = 0; i < focusOnPartialsTrackingBuffer.Length; i++){
            focusOnPartialsTrackingBuffer[i] = false;
        }

        // Reset stones
        foreach(Stone s in noiseGameManager.stonePlacer.stoneScripts){
            s.DisableGlowState();
            s.ResetColorState();
        }
    }

    public void RegisterGenericStoneHit(int stoneNum, float particle_t){
        if((focusOnPartialsCountdown_t > 0)
            && (particle_t < (focusOnPartialsResetValue - focusOnPartialsCountdown_t))){
            
            if(!focusOnPartialsTrackingBuffer[stoneNum]){
                focusOnPartialsTrackingBuffer[stoneNum] = true;

                // NETWORK - Send message that this partial should be focused
                NetworkMessage msg = new NetworkMessage();
                msg.source = "Game";
                msg.command = "FocusOnPartial";
                msg.circleButtonID = stoneNum;
                noiseGameManager.SendNetworkMessage(msg);
            }
        }
    }

    public void HandleCircleButtonClick(NetworkMessage msg){
        noiseGameManager.stonePlacer.stoneScripts[msg.circleButtonID].EnableGlowState();
    }

    public void HandleSendKeyComplete(NetworkMessage msg){
        keysLeft--;
        noiseGameManager.UpdateProgressCounter(keysLeft);
    }

}
