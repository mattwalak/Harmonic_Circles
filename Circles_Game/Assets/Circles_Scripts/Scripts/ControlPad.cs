using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPad : MonoBehaviour
{
    public NoisePanel parentPanel;
    public GameObject controlDotObj;
    private RectTransform rectTransform;

    private float padSize = 0f;

    void Start(){
        rectTransform = GetComponent<RectTransform>();

        Vector3[] v = new Vector3[4];
        rectTransform.GetWorldCorners(v);
        padSize = v[2].x - v[0].x;
        Debug.Log("Pad size = " + padSize);
    }

    void Update(){
        
    }

    public void OnMouseDrag(){
        Vector2 mousePos = Input.mousePosition - transform.position;
        Vector2 normPos = (mousePos / padSize) + new Vector2(0.5f, 0.5f);
        normPos = new Vector2(
            Mathf.Clamp(normPos.x, 0.0f, 1.0f),
            Mathf.Clamp(normPos.y, 0.0f, 1.0f)
        );

        controlDotObj.transform.position = 
            new Vector2(transform.position.x, transform.position.y) 
            + (normPos - new Vector2(0.5f, 0.5f)) * padSize;

        parentPanel.OnPadValueChanged(normPos);
    }

    public void Decoy(){

    }

}
