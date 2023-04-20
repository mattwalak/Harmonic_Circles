using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighFirework : MonoBehaviour, IExplosion
{
    public GameObject lightBurstObj;
    public GameObject heroShapePrefab;

    private OSC osc;
    private float screenHeight;
    private float screenWidth;

    private bool hasStarted = false;
    private FwkData data;

    public void Init(FwkData data){
        osc = (OSC) FindObjectOfType<OSC>();
        screenHeight = Camera.main.orthographicSize;
        screenWidth = screenHeight * Camera.main.aspect;
        this.data = data;
    }

    public void Explode(){
        GameObject hero = Instantiate(heroShapePrefab, transform);
        hero.transform.position = transform.position;
        hero.GetComponent<LightHeroShape>().Init(data);
        LightBurst();
        for(int i = 1; i <= 10; i++){
            if(data.scale >= i * 0.1f){
                StartCoroutine(StaticData.RunWithDelay(i * 0.15f, LightBurst));
            }
        }

    }

    private void LightBurst(){
        GameObject obj = Instantiate(lightBurstObj, transform);
        if(!hasStarted){
            hasStarted = true;
        }
        
        obj.transform.localPosition = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );
        BurstSound();
    }

    private void BurstSound(){
        OscMessage msg = new OscMessage();
        msg.address = "/highNote";
        msg.values.Add(data.scale);
        osc.Send(msg);
    }

    // Update is called once per frame
    void Update()
    {
        if(hasStarted){
            if(gameObject.transform.childCount == 0){
                Destroy(gameObject);
            }
        }
    }
}
