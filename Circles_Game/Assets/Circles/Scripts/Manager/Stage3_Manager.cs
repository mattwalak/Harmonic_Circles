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

    private float levelTimer_t = 0;
    private bool stageIsActive = false;

    public void EnterStage(){
        stageIsActive = true;
        laserCollectionObj.SetActive(true);

        // SOUND - Activate laser mode!
        OscMessage oscMsg = new OscMessage();
        oscMsg.address = "/activateLaserMode";
        noiseGameManager.SendOscMessage(oscMsg);
    }

    public void ExitStage(){
        stageIsActive = false;
    }

    void Update(){
        if(stageIsActive){
            levelTimer_t += Time.deltaTime;
            noiseGameManager.UpdateProgressCounter((int)(levelTimer_t/5));
        }
    }

    public void HandleSendTouchPositionData(NetworkMessage msg){
        if(msg.playerNumber == 1){
            laserSourcePos = new Vector2(msg.touchPosX, msg.touchPosY);
            laserSourcePos = laserSourcePos * noiseGameManager.circleBounds.radius;
            laserCollectionObj.transform.position = laserSourcePos;
        }else if(msg.playerNumber == 2){
            harmonyPos = new Vector2(msg.touchPosX, msg.touchPosY);
            float angleDeg = Vector2.Angle(harmonyReferenceDir, harmonyPos);
            if(harmonyPos.x > 0){
                angleDeg = 360 - angleDeg;
            }
            float magnitude = harmonyPos.magnitude;

            OscMessage oscMsg = new OscMessage();
            oscMsg.address = "/updateHarmonyCircleData";
            oscMsg.values.Add(angleDeg);
            oscMsg.values.Add(magnitude);
            noiseGameManager.SendOscMessage(oscMsg);
        }
    }
    
}
