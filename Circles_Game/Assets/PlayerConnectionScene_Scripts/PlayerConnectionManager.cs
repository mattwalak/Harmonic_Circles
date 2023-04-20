using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerConnectionManager : MonoBehaviour
{
    public NetworkManager netManager;

    public GameObject openNewGamePanel;
    public GameObject waitForPlayersPanel;

    public TMP_Text player1Text;
    public TMP_Text player2Text;

    public Button startGameButton;

    private bool hasPlayer1 = false;
    private bool hasPlayer2 = false;

    void Start(){
        DontDestroyOnLoad(netManager);
    }

    public void OnOpenGameClicked(){
        netManager.EstablishConnection();

        NetworkMessage msg = new NetworkMessage();
        msg.source = "Game";
        msg.command = "OpenNewGame";

        netManager.SendMessage(msg);

        openNewGamePanel.SetActive(false);
        waitForPlayersPanel.SetActive(true);
    }

    public void OnStartGameClicked(){
        SceneManager.LoadScene("Circles_Game");
    }

    public void OnResetConnectionsClicked(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HandlePlayerInputData(NetworkMessage msgObj){
        Debug.Log("msgObj = " + msgObj);
        if(msgObj.newPlayerConnected == 1){
            player1Text.text = "Connected!";
            hasPlayer1 = true;
        }else if(msgObj.newPlayerConnected == 2){
            player2Text.text = "Connected!";
            hasPlayer2 = true;
        }

        if(hasPlayer1 && hasPlayer2){
            startGameButton.interactable = true;
        }
    }
}
