using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHeroShape : MonoBehaviour
{
    public GameObject particlePrefab;

    private const float EXPLOSION_TIME = 1f;
    private const float PEAK_TIME_PERCENT = 0.7f;
    private const float MAX_ALPHA = 0.3f;
    private const float ROTATION = 360.0f;
    private float calculatedEndRotation = 0;

    private Particle heroShape;
    private FwkData data;

    public void Init(FwkData data){
        Color col = Color.HSVToRGB(data.hue, 0.5f, 1.0f);
        
        GameObject heroShapeObj = Instantiate(particlePrefab, transform);
        heroShape = heroShapeObj.GetComponent<Particle>();
        heroShape.Init(data.shape, col);
        heroShape.SetScale(Vector2.zero);
        heroShape.SetAlpha(0);

        float rotationStart = Random.value * 360.0f;
        float calculatedEndRotation = rotationStart + ROTATION;
        float calculatedMidRotation = rotationStart + PEAK_TIME_PERCENT * ROTATION;
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            rotationStart
        );


        LeanTween.scale(heroShapeObj, new Vector3(
                0.5f + 2.5f * data.scale, 
                0.5f + 2.5f * data.scale,
                1.0f), EXPLOSION_TIME * PEAK_TIME_PERCENT)
            .setEase(LeanTweenType.easeOutExpo)
            .setOnComplete(AnimPeak);
        LeanTween.alpha(heroShapeObj, MAX_ALPHA, EXPLOSION_TIME * PEAK_TIME_PERCENT)
            .setEase(LeanTweenType.easeOutSine);
        LeanTween.rotateZ(heroShapeObj, calculatedMidRotation, EXPLOSION_TIME * PEAK_TIME_PERCENT)
            .setEase(LeanTweenType.easeOutExpo);

        this.data = data;
    }

    public void AnimPeak(){
        GameObject heroShapeObj = heroShape.GetGameObject();
        LeanTween.scale(heroShapeObj, new Vector3(0f, 0f, 0f), EXPLOSION_TIME * (1.0f - PEAK_TIME_PERCENT))
            .setEase(LeanTweenType.easeInSine)
            .setOnComplete(AnimEnd);
        LeanTween.alpha(heroShapeObj, 0.0f, EXPLOSION_TIME * (1.0f - PEAK_TIME_PERCENT))
            .setEase(LeanTweenType.easeInSine);
        LeanTween.rotateZ(heroShapeObj, calculatedEndRotation, EXPLOSION_TIME * (1 - PEAK_TIME_PERCENT))
            .setEase(LeanTweenType.easeInSine);
    }

    public void AnimEnd(){
        Destroy(gameObject);
    }

}
