using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject openNewSkyButton;

    public NetworkManager netManager;
    public GameObject fireworkPrefab;
    public OSC osc;
    public GameObject demoHighExplosionPrefab;
    public GameObject demoMidExplosionPrefab;
    public GameObject demoLowExplosionPrefab;

    public GameObject fireworkRocketPrefab;
    public GameObject highExplosionPrefab;
    public GameObject midExplosionPrefab;
    public GameObject lowExplosionPrefab;


    private float screenHeight;
    private float screenWidth;
    private const float SCREEN_EXPLODE_FRAC = 0.7f;

    private void Start(){
        screenHeight = Camera.main.orthographicSize;
        screenWidth = screenHeight * Camera.main.aspect;
    }

    public void DebugOSC(){
        OscMessage msg = new OscMessage();
        msg.address = "/pants";
        osc.Send(msg);
    }

    public void OnOpenNewSkyClicked(){
        netManager.EstablishConnection();

        float aspect = Camera.main.aspect;

        NetworkMessage msg = new NetworkMessage();
        msg.source = "Sky";
        msg.command = "OpenNewSky";
        msg.skyAspect = aspect;

        netManager.SendMessage(msg);

        openNewSkyButton.SetActive(false);
    }

    public void OnSendPantsOptionalClicked(){
        NetworkMessage msg = new NetworkMessage();
        msg.source = "Sky";
        msg.command = "PantsOptional";

        netManager.SendMessage(msg);
    }

    public void OnLaunchReceived(){
        FireworkBody fwk = Instantiate(fireworkPrefab).GetComponent<FireworkBody>();

        float random_x = Random.Range(
            -SCREEN_EXPLODE_FRAC * screenWidth, 
            SCREEN_EXPLODE_FRAC * screenWidth);
        float random_y = Random.Range(
            -SCREEN_EXPLODE_FRAC * screenHeight, 
            SCREEN_EXPLODE_FRAC * screenHeight);

        fwk.numParticles = Random.Range(5, 10)*2 + 1;
        fwk.particleShape = Random.Range(1, 3);
        fwk.particleColor = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 0.8f);

        fwk.Launch(new Vector2(random_x, random_y));
    }

    public void NetworkLaunch(FwkData data){
        Debug.Log(data.ToString());
        GameObject explosionObj = highExplosionPrefab;

        if(data.type == 0){
            explosionObj = highExplosionPrefab;
        }else if(data.type == 1){
            explosionObj = midExplosionPrefab;
        }else if(data.type == 2){
            explosionObj = lowExplosionPrefab;
        }

        GameObject fwk = Instantiate(fireworkRocketPrefab, transform);
        FireworkRocket rocket = fwk.GetComponent<FireworkRocket>();
        rocket.explosionObj = explosionObj;
        rocket.destination = new Vector2(
            (data.normPosX * 2 * screenWidth) - screenWidth,
            ((1 - data.normPosY) * 2 * screenHeight) - screenHeight
        );
        rocket.Init(data);

        /*
        FireworkBody fwk = Instantiate(fireworkPrefab).GetComponent<FireworkBody>();

        fwk.numParticles = Random.Range(5, 10)*2 + 1;
        fwk.particleShape = (ParticleShape)data.shape;
        fwk.particleColor = Color.HSVToRGB(data.hue, 1f, 0.8f);

        float posX = data.posX * screenWidth;
        float posY = data.posY * screenHeight;

        fwk.Launch(new Vector2(posX, posY));

        Debug.Log("End of network launch");*/
    }

    public void DemoHigh(){
        FwkData data = new FwkData();
        data.type = 0;
        data.shape = 2;
        data.hue = 0.5f;
        data.scale = 0.5f;
        data.normPosX = 0.9f;
        data.normPosY = 0.1f;
        
        NetworkLaunch(data);
    }

    public void DemoMid(){
        FwkData data = new FwkData();
        data.type = 1;
        data.shape = 2;
        data.hue = 0.5f;
        data.scale = 0.5f;
        data.normPosX = 0.2f;
        data.normPosY = 0.5f;
        
        NetworkLaunch(data);
    }

    public void DemoLow(){
        FwkData data = new FwkData();
        data.type = 2;
        data.shape = 8;
        data.hue = 0.22f;
        data.scale = 0.5f;
        data.normPosX = 0.2f;
        data.normPosY = 0.9f;
        
        NetworkLaunch(data);
    }
}
