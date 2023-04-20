using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirParticleManager : MonoBehaviour
{
    public GameObject airParticlePrefab;

    private float screenHeight, screenWidth;

    private const int INIT_NUM_PARTICLES = 5000;

    void Start(){
        screenHeight = Camera.main.orthographicSize;
        screenWidth = screenHeight * Camera.main.aspect;

        for(int i = 0; i < INIT_NUM_PARTICLES; i++){
            GameObject air = Instantiate(airParticlePrefab, transform);
            air.transform.position = new Vector2(
                Random.Range(-screenWidth, screenWidth),
                Random.Range(-screenHeight, screenHeight)
            );
        }
    }
}
