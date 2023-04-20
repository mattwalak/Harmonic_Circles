using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollection : MonoBehaviour
{
    public GameObject laserPrefab;
    public Color color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    public OSC osc;
    public int numStones = 20;
    
    private LineRenderer[] lines;
    private Laser[] lasers;
    private LayerMask mask;
    private float[] amplitudeValues;

    private const int NUM_LASERS = 20;
    private const int MAX_REFLECTIONS = 3;


    void Start(){
        lasers = new Laser[NUM_LASERS];
        for(int i = 0; i < NUM_LASERS; i++){
            float rad = i * (2f * Mathf.PI / NUM_LASERS);
            Vector2 dir = new Vector2(
                Mathf.Cos(rad /*+ Random.Range(0f, 0.1f)*/), 
                Mathf.Sin(rad /*+ Random.Range(0f, 0.1f)*/)
            );

            lasers[i] = Instantiate(laserPrefab, transform).GetComponent<Laser>();
            lasers[i].Init(dir, MAX_REFLECTIONS);
        }

        amplitudeValues = new float[numStones];
        for(int i = 0; i < numStones; i++){
            amplitudeValues[i] = 0.0f;
        }
    }

    void Update(){
        for(int i = 0; i < numStones; i++){
            amplitudeValues[i] = 0.0f;
        }

        for(int laserNum = 0; laserNum < NUM_LASERS; laserNum++){
            Laser l = lasers[laserNum];
            for(int componentNum = 0; componentNum < MAX_REFLECTIONS; componentNum++){
                int hitStone = l.hitLog[componentNum];
                if(hitStone >= 0){
                    amplitudeValues[hitStone - 1] += (1.0f/(componentNum + 1));
                }
            }
        }

        SendAmplitudeValues();
    }

    void SendAmplitudeValues(){
        float[] values = new float[numStones];
        float max = 0;
        for(int i = 0; i < numStones; i++){
            if(amplitudeValues[i] > max){
                max = amplitudeValues[i];
            }
        }

        if(max == 0){
            max = 1; // none of the stones have been it so it dosent matter anyway
        }

        for(int i = 0; i < numStones; i++){
            values[i] = (float) amplitudeValues[i] / max;

            /*
            if(amplitudeValues[i] > 0){
                values[i] = 1;
            }else{
                values[i] = 0;
            }*/
        }

        string s = "";
        for(int i = 0; i < values.Length; i++){
            s  = s + values[i].ToString() + " ";
        }
        Debug.Log("amp vals:" + s);

        OscMessage msg = new OscMessage();
        msg.address = "/updateLaserData";
        msg.values.Add(numStones);
        for(int i = 0; i < numStones; i++){
            msg.values.Add(values[i]);
        }

        osc.Send(msg);
    }
}
