using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3_Manager : MonoBehaviour
{
    public NoiseGameManager noiseGameManager;
    public GameObject laserCollectionObj;

    private Vector2 laserSourcePos;
    private Vector2 harmonyPos;
    private Vector2 harmonyReferenceDir = new Vector2(0, 1);

    private float levelTimer_t = -1;
    private float stageDuration = 100;

    private bool isInFadeOut = false;
    private float fadeOut_t = -1;
    private float fadeOutLength = 10;

    private bool gameIsOver = false;

    private bool stageIsActive = false;

    public void EnterStage(){
        stageIsActive = true;
        laserCollectionObj.SetActive(true);

        // SOUND - Activate laser mode!
        OscMessage oscMsg = new OscMessage();
        oscMsg.address = "/activateLaserMode";
        noiseGameManager.SendOscMessage(oscMsg);

        levelTimer_t = stageDuration;

        noiseGameManager.UpdateProgressCounter(-1);
    }

    public void ExitStage(){
        stageIsActive = false;
    }

    void Update(){
        if(gameIsOver){
            return;
        }

        if(stageIsActive){
            levelTimer_t -= Time.deltaTime;

            if(!isInFadeOut && (levelTimer_t < 0)){
                isInFadeOut = true;
                fadeOut_t = fadeOutLength;
            }

            if(levelTimer_t >= 0){
                Debug.Log("levelTimer_t = " + levelTimer_t);
                // int counter = (int)(levelTimer_t);
                // noiseGameManager.UpdateProgressCounter(counter);
                
                float counter_frac = 1.0f - (levelTimer_t / stageDuration);
                noiseGameManager.frontCover.SetOpacity(counter_frac);

                OscMessage oscMsg = new OscMessage();
                oscMsg.address = "/updateStage3Progression";
                oscMsg.values.Add(counter_frac);
                noiseGameManager.SendOscMessage(oscMsg);
            }

            if(isInFadeOut){
                
                fadeOut_t -= Time.deltaTime;
                Debug.Log("fadeOut_t = " + fadeOut_t);

                if(fadeOut_t < 0){
                    gameIsOver = true;
                    Debug.Log("GAME OVER");
                }else{
                    float fadeOutFrac = fadeOut_t / fadeOutLength;
                    noiseGameManager.frontFrontCover.SetOpacity(1 - fadeOutFrac);
                    Debug.Log("fadeOutFrac = " + fadeOutFrac);

                    OscMessage oscMsg = new OscMessage();
                    oscMsg.address = "/updateStage3FinalFade";
                    oscMsg.values.Add(fadeOutFrac);
                    noiseGameManager.SendOscMessage(oscMsg);
                }

                
            }

            
        }
    }

    public void HandleSendTouchPositionData(NetworkMessage msg){
        if(msg.playerNumber == 1){
            laserSourcePos = new Vector2(msg.touchPosX, msg.touchPosY);
            laserSourcePos = laserSourcePos * noiseGameManager.circleBounds.radius;
            laserCollectionObj.transform.position = laserSourcePos;
        }else if(msg.playerNumber == 2){

            // We do the calculations on the javascript end, so no need to do them again
            /*
            harmonyPos = new Vector2(msg.touchPosX, msg.touchPosY);
            float angleDeg = Vector2.Angle(harmonyReferenceDir, harmonyPos);
            if(harmonyPos.x > 0){
                angleDeg = 360 - angleDeg;
            }
            float magnitude = harmonyPos.magnitude;*/

            OscMessage oscMsg = new OscMessage();
            oscMsg.address = "/updateHarmonyCircleData";
            oscMsg.values.Add(msg.touchPosX/*angleDeg*/);
            oscMsg.values.Add(msg.touchPosY/*magnitude*/);
            noiseGameManager.SendOscMessage(oscMsg);
        }
    }
    
}
