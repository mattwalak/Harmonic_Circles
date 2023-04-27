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
    public int NUM_KEYS = 1;

    public TMP_Text progressTextCounter;

    public OSC osc;
    public GameObject laserCollection;
    public StonePlacer stonePlacer;
    public CircleBounds circleBounds;

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

        netManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
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
        }else if(nextGameStageTemp == 2){
            stage1Manager.ExitStage();
            stage2Manager.EnterStage();
        }else if(nextGameStageTemp == 3){
            stage2Manager.ExitStage();
            stage3Manager.EnterStage();
        }

        gameState = nextGameStageTemp;
    }

    public void UpdateProgressCounter(int newNumber){
        progressTextCounter.text = newNumber.ToString();
    }

    public void SendOscMessage(OscMessage msg){
        osc.Send(msg);
    }

    public void SendNetworkMessage(NetworkMessage msg){
        netManager.SendMessage(msg);
    }


    void Update(){
        /*
            // Laser mode
            if(Input.GetMouseButton(0)){
                Vector2 mousePos = Input.mousePosition;
                Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
                ballSourcePosition = worldPos;
            }

            laserCollection.transform.position = ballSourcePosition;
        */
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
