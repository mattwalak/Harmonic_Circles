using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StandardShapes{
    TRIANGLE = 0,
    SQUARE = 1,
    CIRCLE = 2,
    STAR = 3
}

// All shapes are centered at (0, 0) and are contained in a unit square
public class Shape
{
    private Mesh mesh;

    public Shape(StandardShapes shapePrototype){
        switch(shapePrototype){
            case StandardShapes.TRIANGLE:
                break;
            case StandardShapes.SQUARE:
                break;
            case StandardShapes.CIRCLE:
                break;
            case StandardShapes.STAR:
                break;
            default:
                Debug.Log("ERROR - Shape.cs - Unknown shapePrototype");
                break;
        }
    }

    // public Vector2 Get

    public Mesh GetMesh(){
        return mesh;
    }


}
