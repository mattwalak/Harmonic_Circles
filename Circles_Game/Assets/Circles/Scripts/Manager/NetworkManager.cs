using UnityEngine;
using WebSocketSharp;
using System.Collections;
using System.Collections.Generic;
using System;

public class NetworkManager : MonoBehaviour
{
    public bool isPlayerConnectionScene;
    public bool isNoiseGameScene;

    public PlayerConnectionManager playerConnectionManager;
    public NoiseGameManager noiseGameManager;

    public static bool DEBUG = false;
    
    string host = "ws://18.215.177.114:42742";
    WebSocket ws;

    List<Action<NetworkMessage>> handlers;
    List<NetworkMessage> messages;

    public void EstablishConnection(){
        ws = new WebSocket(host);
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            string str = "Message Received from "+((WebSocket)sender).Url+", Data : "+e.Data;
            Debug.Log("Received message: " + str);
            NetworkMessage msgObj = JsonUtility.FromJson<NetworkMessage>(e.Data);
            if(!msgObj.source.Equals("Server")){
                Debug.Log("ERROR - Unknown source");
            }else{
                switch(msgObj.command){
                    case "PlayerInputData":
                        Debug.Log("PlayerInputData Initial Response");
                        handlers.Add(HandlePlayerInputData);
                        messages.Add(msgObj);
                        break;
                    case "CircleButtonClick":
                        Debug.Log("CircleButtonClick Initial Response");
                        handlers.Add(HandleCircleButtonClick);
                        messages.Add(msgObj);
                        break;
                    case "SendTouchPositionData":
                        handlers.Add(HandleSendTouchPositionData);
                        messages.Add(msgObj);
                        break;
                    case "SendKeyChange":
                        handlers.Add(HandleSendKeyChange);
                        messages.Add(msgObj);
                        break;
                    default:
                        Debug.Log("ERROR - Unknown command");
                        break;
                }
            }
        };
    }

    public void SendMessage(NetworkMessage msg){
        string str = msg.Serialized();
        Debug.Log("Sending Serialized obj on Network: " + str);
        ws.Send(str);
    }
    
    // ---------------------------------- HANDLERS --------------------------------
    public void HandleSendKeyChange(NetworkMessage msgObj){
        if(isNoiseGameScene){
            noiseGameManager.HandleKeyChange(msgObj);
        }
    }
    
    public void HandleSendTouchPositionData(NetworkMessage msgObj){
        if(isNoiseGameScene){
            noiseGameManager.HandleSendTouchPositionData(msgObj);
        }
    }
    
    public void HandleCircleButtonClick(NetworkMessage msgObj){
        Debug.Log("Circle Button Click handler");
        if(isNoiseGameScene){
            noiseGameManager.HandleCircleButtonClick(msgObj);
        }
    }

    public void HandlePlayerInputData(NetworkMessage msgObj){
        Debug.Log("Player Input Data handler");
        if(isPlayerConnectionScene){
            playerConnectionManager.HandlePlayerInputData(msgObj);
        }else if(isNoiseGameScene){

        }
    }

    // ----------------------------------- UNITY STUFF -----------------------------

    private void Start()
    {
        if (Application.isEditor)
        {
            DEBUG = true;
            Debug.Log("Running in DEBUG");
        }else{
            Debug.Log("for real this time");
        }

        if(DEBUG){
            host = "ws://127.0.0.1:42742";
        }

        handlers = new List<Action<NetworkMessage>>();
        messages = new List<NetworkMessage>();
    }

    public void LoadNoiseGameScene(){
        isPlayerConnectionScene = false;
        isNoiseGameScene = true;

        noiseGameManager = (NoiseGameManager) FindObjectOfType(typeof(NoiseGameManager));
    }

    private void Update(){
        while(handlers.Count > 0){
            handlers[0](messages[0]);
            handlers.RemoveAt(0);
            messages.RemoveAt(0);
        }
    }
}
