using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private SpriteRenderer renderer;

    void Start(){
        renderer = GetComponent<SpriteRenderer>();
    }

    public void SetOpacity(float t){
        Color c = renderer.color;
        c.a = t;
        renderer.color = c;
    }

    public void SetColor(Color c){
        renderer.color = c;
    }


}
