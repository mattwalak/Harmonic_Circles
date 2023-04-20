using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBounds : MonoBehaviour
{
    public float radius = 1.0f;
    public int numPoints = 100;

    public Color color;
    public MeshRenderer meshRenderer;

    private EdgeCollider2D edgeCollider;

    void Start(){
        CreateCircle();

        meshRenderer.materials[0].color = color;
    }

    void CreateCircle()
    {
        Vector2[] edgePoints = new Vector2[numPoints + 1];
        edgeCollider = GetComponent<EdgeCollider2D>();
       
        for(int loop = 0; loop <= numPoints; loop++)
        {
            float angle = (Mathf.PI * 2.0f / numPoints) * loop;
            edgePoints[loop] = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
        }
       
        edgeCollider.points = edgePoints;
    }


}
