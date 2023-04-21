using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirParticle : MonoBehaviour
{
    private float screenHeight, screenWidth;
    private Rigidbody2D rb2D;

    private SpriteRenderer spriteRenderer;

    private float expirationTime = 5f;
    private float t = 0f;

    void Start(){
        rb2D = GetComponent<Rigidbody2D>();
        screenHeight = Camera.main.orthographicSize;
        screenWidth = screenHeight * Camera.main.aspect;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void SetOpacity(float a){
        Color col = Color.HSVToRGB(a, 0.8f, 1.0f);
        col.a = a;
        spriteRenderer.color = col;
    }

    public float GetGain(){
        float frac = t / expirationTime;
        return 1 - frac;
    }

    void Update(){
        if(transform.position.x < -screenWidth || transform.position.x > screenWidth
            || transform.position.y < -screenHeight || transform.position.y > screenHeight){
            Destroy(gameObject);
        }

        t += Time.deltaTime;
        if(t > expirationTime){
            Destroy(gameObject);
        }else{
            float frac = t / expirationTime;
            SetOpacity(1 - frac);
        }
    }
}
