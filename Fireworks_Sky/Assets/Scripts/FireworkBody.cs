using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkBody : MonoBehaviour
{
    public GameObject particlePrefab;

    public int numParticles;
    public int particleShape;
    public Color particleColor;

    private float screenHeight;
    private float screenWidth;

    private const float FIREWORK_SPRITE_OFFSET = 1f;
    private const float launchVelocity = 6f;
    private float explode_y;

    private bool isTravelingUp = false;
    private bool isExploding = false;

    private SpriteRenderer renderer;
    private OSC osc;

    private void Start(){
        osc = (OSC) FindObjectOfType<OSC>();
    }

    public void Launch(Vector2 explodePosition){
        renderer = GetComponent<SpriteRenderer>();

        screenHeight = Camera.main.orthographicSize;
        screenWidth = screenHeight * Camera.main.aspect;

        explode_y = explodePosition.y;
        transform.position = new Vector2(explodePosition.x, -screenHeight - FIREWORK_SPRITE_OFFSET);

        isTravelingUp = true;
    }

    public void Explode(){
        Debug.Log("Explode function");
        isTravelingUp = false;
        isExploding = true;
        renderer.enabled = false;

        for(int i = 0; i < numParticles; i++){
            float rad = i * (2f * Mathf.PI / numParticles);
            Vector2 dir = new Vector2(
                Mathf.Cos(rad /*+ Random.Range(0f, 0.1f)*/), 
                Mathf.Sin(rad /*+ Random.Range(0f, 0.1f)*/)
            );

            Particle p = Instantiate(particlePrefab).GetComponent<Particle>();
            p.Init(particleShape, particleColor);
            //float scale = Random.Range(0.5f, 0.6f);
            p.SetScale(new Vector2(0.5f, 0.5f));
            p.SetPosition(transform.position);
            p.SetLookAt(dir);
            p.AddForce(6f * dir /** Random.Range(.93f, 1.07f)*/);
            p.SetAngularVelocity(360f * Random.Range(.8f, 1.2f));
            p.SetFadeout(1f * Random.Range(.8f, 1.2f), 1f*Random.Range(.8f, 1.2f));
        }

        PlaySound();
        Destroy(gameObject);
    }

    private void PlaySound(){
        OscMessage msg = new OscMessage();
        msg.address = "/melodyNote";
        msg.values.Add(0.5f);
        osc.Send(msg);
    }

    private void Update(){
        if(isTravelingUp){
            transform.position = new Vector2(
                transform.position.x,
                transform.position.y + launchVelocity * Time.deltaTime
            );

            if(transform.position.y >= explode_y){
                Explode();
            }
        }else if(isExploding){

        }
    }
}
