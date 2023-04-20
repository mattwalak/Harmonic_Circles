using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkRocket : MonoBehaviour
{
    public GameObject explosionObj;
    
    public Vector2 destination;

    private float screenHeight;
    private float screenWidth;

    private const float FIREWORK_SPRITE_OFFSET = 1f;
    private const float launchVelocity = 6f;

    private bool isInit = false;
    private FwkData data;

    public void Init(FwkData data){
        screenHeight = Camera.main.orthographicSize;
        screenWidth = screenHeight * Camera.main.aspect;

        transform.position = new Vector2(destination.x, -screenHeight - (FIREWORK_SPRITE_OFFSET * data.scale));
        transform.localScale = new Vector2(
            0.5f + data.scale * 2.5f,
            0.5f + data.scale * 2.5f
        );

        this.data = data;
    }

    private void Update(){
        transform.position = new Vector2(
            transform.position.x,
            transform.position.y + Time.deltaTime * launchVelocity
        );

        if(transform.position.y >= destination.y){
            GameObject obj = Instantiate(explosionObj, transform.parent);
            obj.transform.position = destination;            
            IExplosion explosion = obj.GetComponent<IExplosion>();
            explosion.Init(data);
            explosion.Explode();
            Destroy(gameObject);
        }
    }
}
