using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidFirework : MonoBehaviour, IExplosion
{
    public GameObject particlePrefab;

    private GameManager gameManager;
    private OSC osc;

    private FwkData data;
    private bool hasExploded = false;

    public void Init(FwkData data){
        gameManager = (GameManager) FindObjectOfType<GameManager>();
        osc = (OSC) FindObjectOfType<OSC>();

        this.data = data;
    }

    public void Explode(){
        int numParticles = 6 + (int)(data.scale * 4.0f);

        for(int i = 0; i < numParticles; i++){
            float rad = i * (2f * Mathf.PI / numParticles);
            Vector2 dir = new Vector2(
                Mathf.Cos(rad /*+ Random.Range(0f, 0.1f)*/), 
                Mathf.Sin(rad /*+ Random.Range(0f, 0.1f)*/)
            );

            Particle p = Instantiate(particlePrefab, transform).GetComponent<Particle>();
            p.Init(data.shape, Color.HSVToRGB(data.hue, 1.0f, 0.8f));
            Vector2 scale = new Vector2(0.25f, 0.25f) + (data.scale * new Vector2(0.75f, 0.75f));
            p.SetScale(scale);
            p.SetPosition(transform.position);
            p.SetRotation(((float)i/numParticles) * 360f);
            float force = 3f + data.scale * 5f;
            p.AddForce(force * dir);
            p.SetAngularVelocity(360f * Random.Range(.8f, 1.2f));
            p.SetFadeout(1f * Random.Range(.8f, 1.2f), 1f*Random.Range(.8f, 1.2f));
            p.GetGameObject().name = "i = " + i;
        }

        hasExploded = true;
        PlaySound();
    }

    private void PlaySound(){
        OscMessage msg = new OscMessage();
        msg.address = "/melodyNote";
        msg.values.Add(data.scale);
        osc.Send(msg);
    }

    void Update(){
        if(hasExploded){
            if(gameObject.transform.childCount == 0){
                Destroy(gameObject);
            }
        }
    }
}
