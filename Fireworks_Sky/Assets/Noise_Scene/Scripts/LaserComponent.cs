using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserComponent : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public void Init(int index, int maxIndex){
        lineRenderer = GetComponent<LineRenderer>();
        float frac = (float)index / maxIndex;

        Color c = Color.HSVToRGB(frac, 0.8f, 1.0f);
        c.a = 1.0f - frac;
        lineRenderer.material.color = c;
    }

    public LineRenderer GetLineRenderer(){
        return lineRenderer;
    }

    public void DisableRenderer(){
        lineRenderer.enabled = false;
    }

    public void EnableRenderer(){
        lineRenderer.enabled = true;
    }
}
