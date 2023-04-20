using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Sprite SquareSprite;
    public Sprite CircleSprite;
    public Sprite TriangleSprite;
    public Sprite BlobSprite;
    public Sprite LightningSprite;
    public Sprite CloverSprite;
    public Sprite SpikeSprite;
    public Sprite StarSprite;
    public Sprite HeartSprite;
    public Sprite CrescentSprite;

    private int shape;
    private Color color;

    private SpriteRenderer renderer;
    private Rigidbody2D body;


    public void Init(int shape_in, Color color_in){
        renderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        renderer.enabled = true;

        shape = shape_in;
        color = color_in;

        switch(shape){
            case 0:
                renderer.sprite = SquareSprite;
                break;
            case 1:
                renderer.sprite = CircleSprite;
                break;
            case 2:
                renderer.sprite = TriangleSprite;
                break;
            case 3:
                renderer.sprite = BlobSprite;
                break;
            case 4:
                renderer.sprite = LightningSprite;
                break;
            case 5:
                renderer.sprite = CloverSprite;
                break;
            case 6:
                renderer.sprite = SpikeSprite;
                break;
            case 7:
                renderer.sprite = StarSprite;
                break;
            case 8:
                renderer.sprite = HeartSprite;
                break;
            case 9:
                renderer.sprite = CrescentSprite;
                break;
        }

        renderer.color = color;
    }

    public void SetPosition(Vector2 pos){
        transform.position = pos;
    }

    public void SetRotation(float rot){
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            rot
        );
    }

    public void SetLocalPosition(Vector2 localPos){
        transform.localPosition = localPos;
    }

    public void SetScale(Vector2 scale){
        transform.localScale = scale;
    }

    public void SetLinearDrag(float drag){
        body.drag = drag;
    }

    public void SetAngularDrag(float angularDrag){
        body.angularDrag = angularDrag;
    }

    public void AddForce(Vector2 force){
        body.AddForce(force, ForceMode2D.Impulse);
    }

    public void SetLookAt(Vector2 look){
        transform.right = look;
    }

    public void SetAngularVelocity(float vel){
        body.angularVelocity = vel;
    }

    public void SetAlpha(float val){
        Color col = renderer.material.color;
        col = new Color(col.r, col.g, col.b, val);
        renderer.material.color = col;
    }

    public GameObject GetGameObject(){
        return gameObject;
    }

    public void SetFadeout(float untilFadeout, float duringFadeout){
        StartCoroutine(FadeoutWithDelay(untilFadeout, duringFadeout));
    }

    private IEnumerator FadeoutWithDelay(float untilFadeout, float duringFadeout){
        yield return new WaitForSeconds(untilFadeout);
        LeanTween.alpha(gameObject, 0f, duringFadeout).setOnComplete(DestroySelf);
    }

    public void DestroySelf(){
        Destroy(gameObject);
    }
}
