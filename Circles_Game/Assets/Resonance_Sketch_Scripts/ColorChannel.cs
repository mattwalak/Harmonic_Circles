using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChannel : MonoBehaviour
{
    public float springConstant = 1;
    public float simulatedMass = 1;
    public float damping = 0;
    
    private const int NUM_STRING_SEGMENTS = 100;

    private float[] positions;
    private float[] velocities;
    private float[] acceleration;
    private float t = 0;
    public float freq = 2;

    private MeshRenderer meshRenderer;

    void Start(){
        positions = new float[NUM_STRING_SEGMENTS];
        velocities = new float[NUM_STRING_SEGMENTS];
        acceleration = new float[NUM_STRING_SEGMENTS];

        for(int i = 0; i < NUM_STRING_SEGMENTS; i++){
            positions[i] = 0;
            velocities[i] = 0;
            acceleration[i] = 0;
        }

        meshRenderer = GetComponent<MeshRenderer>();
    }

    public Texture2D CreateTexture(){
        int width = NUM_STRING_SEGMENTS;
        int height = 1;

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;

        for(int x = 0; x < width; x++){
            texture.SetPixel(x, 0, new Color(positions[x], 0.0f, 0.0f, 1.0f));
        }

        texture.Apply();
        return texture;
    }

    void Update(){
        meshRenderer.material.mainTexture = CreateTexture();


    }

    void FixedUpdate()
    {
        /*
        for(int i = NUM_STRING_SEGMENTS - 1; i > 0; i--){
            positions[i] = positions[i-1];      
        } */

        for(int i = 1; i < NUM_STRING_SEGMENTS; i++){
            float dy = positions[i] - positions[i-1];
            float accel = (-springConstant * dy);
            velocities[i] = velocities[i] + (accel * Time.fixedDeltaTime);
            positions[i] = positions[i] + (velocities[i] * Time.fixedDeltaTime);
            
            // acceleration[i] = -springConstant * dy;
        }   

        t += Time.fixedDeltaTime;
        
        positions[0] = 0.2f * (0.5f + (0.5f * Mathf.Sin(t * 2 * Mathf.PI * freq)));
    }

    public void OnImpulse(){
        // positions[0] = 0.5f;
    }

}
