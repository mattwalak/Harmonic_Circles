using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoiseGameManager : MonoBehaviour
{
    public Stage1_Manager stage1Manager;
    public Stage2_Manager stage2Manager;
    public Stage3_Manager stage3Manager;
    
    public int NUM_CIRCLES = 20;
    public int NUM_KEYS = 6;

    public TMP_Text progressTextCounter;

    public OSC osc;
    public GameObject laserCollection;
    public StonePlacer stonePlacer;
    public CircleBounds circleBounds;
    public Cover frontFrontCover;
    public Cover frontCover;
    public Cover backCover;

    public GameObject airParticlePrefab;

    private NetworkManager netManager;

    public Camera mainCamera;

    // 0 = initializing
    // 1 = impulse mode
    // 2 = rhythmic impulse mode
    // 3 = laser mode
    public int gameState = 0;

    public const int NUM_PARTICLES_PER_CLICK = 10;
    public const float PARTICLE_VELOCITY = 3.0f;
    
    void Start(){
        mainCamera = Camera.main;

        netManager = (NetworkManager)FindObjectOfType(typeof(NetworkManager));
        if(netManager != null){
            netManager.LoadNoiseGameScene();
        }

        NextScene();
    }

    public void blastParticlesAtLocation(Vector2 pos){
        for(int i = 0; i < NUM_PARTICLES_PER_CLICK; i++){
            float rad = i * (2f * Mathf.PI / NUM_PARTICLES_PER_CLICK);
            Vector2 dir = new Vector2(
                Mathf.Cos(rad), 
                Mathf.Sin(rad)
            );
                
            GameObject p = Instantiate(airParticlePrefab, transform);
            p.transform.position = pos;
            Rigidbody2D p2d = p.GetComponent<Rigidbody2D>();
            p2d.velocity = dir * PARTICLE_VELOCITY;
        }    
    }

    public void blastParticlesAtMouseLocation(){
        Vector2 mousePos = Input.mousePosition;
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        blastParticlesAtLocation(worldPos);
    }

    public void NextScene(){
        int nextGameStageTemp = gameState + 1;
        if(nextGameStageTemp == 1){
            stage1Manager.EnterStage();
            backCover.SetColor(Color.HSVToRGB(180.0f/360.0f, 1.0f, 0.4f));
        }else if(nextGameStageTemp == 2){
            stage1Manager.ExitStage();
            stage2Manager.EnterStage();
            backCover.SetColor(Color.HSVToRGB(230.0f/360.0f, 1.0f, 0.4f));
        }else if(nextGameStageTemp == 3){
            stage2Manager.ExitStage();
            stage3Manager.EnterStage();
            backCover.SetColor(Color.HSVToRGB(280.0f/360.0f, 1.0f, 0.4f));
        }

        gameState = nextGameStageTemp;
    }

    public void UpdateProgressCounter(int newNumber){
        if(newNumber >= 0){
            progressTextCounter.text = newNumber.ToString();
        }else{
            progressTextCounter.text = "";
        }
        
    }

    public void SendOscMessage(OscMessage msg){
        osc.Send(msg);
    }

    public void SendNetworkMessage(NetworkMessage msg){
        netManager.SendMessage(msg);
    }


    void Update(){

    }

    public void RegisterNewActivatedStone(){
        if(gameState == 1){
            stage1Manager.RegisterNewActivatedStone();
        }else if(gameState == 2){
            // Do nothing
        }else if(gameState == 3){

        }
    }

    public void RegisterGenericStoneHit(int stoneNum, float particle_t){
        if(gameState == 1){
            // Do nothing
        }else if(gameState == 2){
            stage2Manager.RegisterGenericStoneHit(stoneNum, particle_t);
        }else if(gameState == 3){

        }
    }

    public void HandleCircleButtonClick(NetworkMessage msg){
        if(gameState == 1){
            stage1Manager.HandleCircleButtonClick(msg);
        }else if(gameState == 2){
            stage2Manager.HandleCircleButtonClick(msg);
        }else if(gameState == 3){

        }
    }

    public void HandleSendTouchPositionData(NetworkMessage msg){
        if(gameState == 1){
            stage1Manager.HandleSendTouchPositionData(msg);
        }else if(gameState == 2){
            stage2Manager.HandleSendTouchPositionData(msg);
        }else if(gameState == 3){
            stage3Manager.HandleSendTouchPositionData(msg);
        }
    }

    public void HandleKeyChange(NetworkMessage msg){
        if(gameState == 1){
            // Do nothing
        }else if(gameState == 2){
            stage2Manager.HandleKeyChange(msg);
        }else if(gameState == 3){

        }
    }

    public void HandleSendKeyComplete(NetworkMessage msg){
        if(gameState == 1){
            // Do nothing
        }else if(gameState == 2){
            stage2Manager.HandleSendKeyComplete(msg);
        }else if(gameState == 3){

        }
    }

}
