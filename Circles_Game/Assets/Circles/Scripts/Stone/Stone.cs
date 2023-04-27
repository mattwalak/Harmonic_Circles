using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int stoneNum;
    private OSC osc;

    private NoiseGameManager gameManager;
    private NetworkManager netManager;
    private SpriteRenderer spriteRenderer;

    public StoneGlow glow;

    private Color normalColor = new Color(160.0f/255.0f, 160.0f/255.0f, 160.0f/255.0f, 1.0f);
    private Color glowColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    private Color lockedColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);


    private bool isInGlowState = false;
    private bool isInColoredState = false;
    private int numGlowHits = 0;

    void Start(){
        gameManager = (NoiseGameManager)FindObjectOfType<NoiseGameManager>();
        netManager = (NetworkManager)FindObjectOfType<NetworkManager>();
        osc = (OSC)FindObjectOfType<OSC>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateColor();

        // DEBUG
        EnableGlowState();
    }

    public void ResetForNewGameState(int newGameState){
        if(newGameState == 2){
            DisableGlowState();
            isInColoredState = false;
            numGlowHits = 0;
            UpdateColor();
        }else if(newGameState == 3){
            DisableGlowState();
            isInColoredState = false;
            numGlowHits = 0;
            UpdateColor();
        }
    }

    private void UpdateColor(){
        if(numGlowHits != 0){
            float hue = (120.0f + numGlowHits * 60.0f) % 360.0f;
            Color col = Color.HSVToRGB(hue/360.0f, 1.0f, 0.8f);
            spriteRenderer.color = col;
            glow.SetColor(col);
        }else{
            spriteRenderer.color = normalColor;
        }
    }    

    public void EnableGlowState(){
        isInGlowState = true;
        glow.gameObject.SetActive(true);
    }

    public void DisableGlowState(){
        isInGlowState = false;
        glow.gameObject.SetActive(false);
    }

    public void ResetColorState(){
        numGlowHits = 0;
        isInColoredState = false;
        UpdateColor();
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(gameManager.gameState == 2){
            AirParticle particle = collisionInfo.gameObject.GetComponent<AirParticle>();
            gameManager.RegisterGenericStoneHit(stoneNum, particle.t);
        }

        if(isInGlowState){
            if(gameManager.gameState == 1){
                // Send glow hit osc only in game state 1
                OscMessage msg = new OscMessage();
                msg.address = "/playStoneNoteGlowHit";
                msg.values.Add(stoneNum);
                msg.values.Add(collisionInfo.gameObject.GetComponent<AirParticle>().GetGain());
                osc.Send(msg);
            }else if(gameManager.gameState == 2){
                // Do not send glow hit osc in game state 2
                OscMessage msg = new OscMessage();
                msg.address = "/playStoneNote";
                msg.values.Add(stoneNum);
                msg.values.Add(collisionInfo.gameObject.GetComponent<AirParticle>().GetGain());
                osc.Send(msg);
            }

            // Send networked glow hit only if in gamemode 1
            if(numGlowHits == 0 && gameManager.gameState == 1){
                // First glow hit
                gameManager.RegisterNewActivatedStone();
                NetworkMessage netMsgObj = new NetworkMessage();
                netMsgObj.circleButtonID = stoneNum;
                netMsgObj.circleButtonState = 2;
                netMsgObj.source = "Game";
                netMsgObj.command = "SendCircleButtonUpdateFromGame";
                netManager.SendMessage(netMsgObj);
            }

            numGlowHits++;
            UpdateColor();

            isInColoredState = true;
        }else if(isInColoredState){
            OscMessage msg = new OscMessage();
            msg.address = "/playStoneNoteColoredHit";
            msg.values.Add(stoneNum);
            msg.values.Add(collisionInfo.gameObject.GetComponent<AirParticle>().GetGain());
            msg.values.Add(numGlowHits % 6);
            osc.Send(msg);

            numGlowHits++;
            UpdateColor();
        }else{
            OscMessage msg = new OscMessage();
            msg.address = "/playStoneNote";
            msg.values.Add(stoneNum);
            msg.values.Add(collisionInfo.gameObject.GetComponent<AirParticle>().GetGain());
            osc.Send(msg);
        }
    }
}
