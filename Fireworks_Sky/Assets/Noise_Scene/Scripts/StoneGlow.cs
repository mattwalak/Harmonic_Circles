using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGlow : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color col){
        spriteRenderer.color = col;
    }
}
