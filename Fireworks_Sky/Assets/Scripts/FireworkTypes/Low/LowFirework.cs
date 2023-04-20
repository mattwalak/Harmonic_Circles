using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowFirework : MonoBehaviour, IExplosion
{
    public GameObject particlePrefab;

    private GameObject heroShapeObj;
    private Particle heroShapeParticle;
    private GameObject ambientShapeObj;
    private Particle ambientShapeParticle;

    private const float AMBIENT_EXPLOSION_TIME = 2f;
    private const float AMBIENT_PEAK_TIME_FRAC = 0.7f;
    private const float AMBIENT_PEAK_ALPHA = 0.2f;

    private const float HERO_EXPLOSION_TIME = 0.3f;
    private const float HERO_PEAK_TIME_FRAC = 0.3f;
    private const float HERO_PEAK_ALPHA = 0.5f;

    /*
    public GameObject initialExplosion;
    public GameObject secondaryExplosion;

    public Color color = new Color(1, 1, 0, 1);
    public float explosionSize = 5;
    
    private const float EXPLOSION_TIME = 2f;
    private const float ALPHA_PEAK_PERCENT = 0.7f;
    private const float INITIAL_PERCENT_SCALE = 0.2f;
    private const float INITIAL_PERCENT_TIME = 0.2f;
    private const float MAX_ALPHA = 0.3f;
    private SpriteRenderer secondaryRenderer;*/
    private OSC osc;
    private FwkData data;

    // Start is called before the first frame update
    public void Init(FwkData data)
    {
        osc = (OSC) FindObjectOfType<OSC>();

        heroShapeObj = Instantiate(particlePrefab, transform);
        heroShapeParticle = heroShapeObj.GetComponent<Particle>();
        heroShapeParticle.Init(data.shape, Color.HSVToRGB(data.hue, 1.0f, 1.0f));
        heroShapeParticle.SetAlpha(0);
        ambientShapeObj = Instantiate(particlePrefab, transform);
        ambientShapeParticle = ambientShapeObj.GetComponent<Particle>();
        ambientShapeParticle.Init(data.shape, Color.HSVToRGB(data.hue, 0.70f, 1.0f));
        ambientShapeParticle.SetAlpha(0);

        this.data = data;
    }

    public void Explode(){
        // Hero
        float heroScale = 0.5f + data.scale * 2.5f;
        LeanTween.scale(heroShapeObj, new Vector3(heroScale, heroScale, heroScale), HERO_EXPLOSION_TIME * HERO_PEAK_TIME_FRAC)
            .setEase(LeanTweenType.easeOutExpo);
        LeanTween.alpha(heroShapeObj, HERO_PEAK_ALPHA, HERO_EXPLOSION_TIME * HERO_PEAK_TIME_FRAC)
            .setEase(LeanTweenType.easeOutExpo)
            .setOnComplete(HeroPeak);

        // Ambient
        float ambientScale = 3.0f + data.scale * 10.0f;
        LeanTween.scale(ambientShapeObj, new Vector3(ambientScale, ambientScale, ambientScale), AMBIENT_EXPLOSION_TIME * AMBIENT_PEAK_TIME_FRAC)
            .setEase(LeanTweenType.easeOutExpo);
        LeanTween.alpha(ambientShapeObj, AMBIENT_PEAK_ALPHA, AMBIENT_EXPLOSION_TIME * AMBIENT_PEAK_TIME_FRAC)
            .setEase(LeanTweenType.easeOutExpo)
            .setOnComplete(AmbientPeak);

        /*
        // Initial Explosion
        initialExplosion.transform.localScale = new Vector2(0, 0);
        float initialSize = explosionSize * INITIAL_PERCENT_SCALE;
        float initialTime = EXPLOSION_TIME * INITIAL_PERCENT_TIME;
        LeanTween.scale(initialExplosion, new Vector3(initialSize, initialSize, initialSize), initialTime)
            .setEase(LeanTweenType.easeOutExpo);
        LeanTween.alpha(initialExplosion, MAX_ALPHA, initialTime * ALPHA_PEAK_PERCENT)
            .setEase(LeanTweenType.easeOutSine)
            .setOnComplete(InitialAlphaDown);

        // Secondary Explosion
        secondaryExplosion.transform.localScale = new Vector2(0, 0);
        LeanTween.scale(secondaryExplosion, new Vector3(explosionSize, explosionSize, explosionSize), EXPLOSION_TIME)
            .setEase(LeanTweenType.easeOutExpo);
        LeanTween.alpha(secondaryExplosion, MAX_ALPHA, EXPLOSION_TIME * ALPHA_PEAK_PERCENT)
            .setEase(LeanTweenType.easeOutSine)
            .setOnComplete(SecondaryAlphaDown);*/

        PlaySound();
    }

    private void AmbientPeak(){
        LeanTween.alpha(ambientShapeObj, 0, AMBIENT_EXPLOSION_TIME * (1 - AMBIENT_PEAK_TIME_FRAC))
            .setEase(LeanTweenType.easeInSine)
            .setOnComplete(DestroySelf);
    }

    private void HeroPeak(){
        LeanTween.alpha(heroShapeObj, 0, HERO_EXPLOSION_TIME * (1 - HERO_PEAK_TIME_FRAC))
            .setEase(LeanTweenType.easeInSine);
    }

    private void PlaySound(){
        OscMessage msg = new OscMessage();
        msg.address = "/bassNote";
        msg.values.Add(data.scale);
        osc.Send(msg);
    }

    

    /*
    private void InitialAlphaDown(){
        float initialTime = EXPLOSION_TIME * INITIAL_PERCENT_TIME; 
        LeanTween.alpha(initialExplosion, 0.0f, initialTime * (1f - ALPHA_PEAK_PERCENT))
            .setEase(LeanTweenType.easeInSine);
    }

    private void SecondaryAlphaDown(){
        LeanTween.alpha(secondaryExplosion, 0.0f, EXPLOSION_TIME * (1f - ALPHA_PEAK_PERCENT))
            .setEase(LeanTweenType.easeInSine)
            .setOnComplete(DestroySelf);
    }*/

    private void DestroySelf(){
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
