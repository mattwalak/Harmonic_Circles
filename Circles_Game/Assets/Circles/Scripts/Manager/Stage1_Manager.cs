using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_Manager : MonoBehaviour
{
    private int circlesLeft;
    private bool stageIsActive = false;

    public NoiseGameManager noiseGameManager;

    public void EnterStage(){
        circlesLeft = noiseGameManager.NUM_CIRCLES;
        noiseGameManager.UpdateProgressCounter(circlesLeft);
        stageIsActive = true;
    }

    public void ExitStage(){
        stageIsActive = false;
    }

    public void HandleCircleButtonClick(NetworkMessage msg){
        if(msg.circleButtonState == -1){
            noiseGameManager.stonePlacer.stoneScripts[msg.circleButtonID].DisableGlowState();
        }else{
            noiseGameManager.stonePlacer.stoneScripts[msg.circleButtonID].EnableGlowState();
        }
    }

    public void HandleSendTouchPositionData(NetworkMessage msg){
        Debug.Log("In stage1_Manager handleSendTouchPosition thing");
        Vector2 pos = new Vector2(msg.touchPosX, msg.touchPosY);
        noiseGameManager.blastParticlesAtLocation(pos * noiseGameManager.circleBounds.radius);
    }

    public void RegisterNewActivatedStone(){
        circlesLeft--;
        noiseGameManager.UpdateProgressCounter(circlesLeft);
        if(circlesLeft == 0){
            foreach(Stone s in noiseGameManager.stonePlacer.stoneScripts){
                s.ResetForNewGameState(1);
            }

            NetworkMessage msg = new NetworkMessage();
            msg.source = "Game";
            msg.command = "SceneChange";
            msg.changeSceneTo = 2;
            noiseGameManager.SendNetworkMessage(msg);

            noiseGameManager.NextScene();
        }
    }
    
    void Update(){
        if(stageIsActive){
            if(Input.GetMouseButtonDown(0)){
                noiseGameManager.blastParticlesAtMouseLocation();
            }
        }
    }



}
