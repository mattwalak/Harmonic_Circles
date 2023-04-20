using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Vector2[] points;
    public Vector2[] outControl;
    public Vector2[] inControl;
    public bool isClosed = false;

    private int numSegments;
    private float totalLength;
    private float[] segmentLengths;
    private float[] segmentTFracLengths; // Same as above, just divided by total length
                                         // Equal to how many counts of T we are in this segment

    private float[,] segmentArcLengths; // Literally the same thing, just one step deeper
    private float[,] segmentArcTFracLengths;

    private int arcTableResolution = 5;

    public Vector2 GetPositionAtT(float t){
        int targetSegment = 0;

        // Locate target segment
        while((t > segmentTFracLengths[targetSegment]) && (targetSegment < numSegments)){
            targetSegment++;
            t -= segmentTFracLengths[targetSegment];
        }
        if(targetSegment == numSegments){
            Debug.Log("ERROR - Path.cs - target segment exceeded numSegments");
            return Vector2.zero;
        }
        t = t/segmentTFracLengths[targetSegment]; // Should be back in 0,1 range

        // Locate target arc
        
        int targetArc = 0;
        while((t > segmentArcTFracLengths[targetSegment, targetArc]) && (targetArc < arcTableResolution)){
            targetArc++;
            t -= segmentArcTFracLengths[targetSegment, targetArc];
        }
        if(targetArc == arcTableResolution){
            Debug.Log("ERROR - Path.cs - target arc exceeded table resolution");
            return Vector2.zero;
        }
        t = t/segmentArcTFracLengths[targetSegment, targetArc];

        float start_t = (float)(targetArc / arcTableResolution);
        float end_t = (float)(targetArc + 1) / arcTableResolution;
        float real_t = Mathf.Lerp(start_t, end_t, t);
        return InterpolateAtPoint(targetSegment, real_t);

        return Vector2.zero;
    }

    public void RecalculateArcLengthTables(){
        numSegments = (isClosed ? points.Length + 1 : points.Length);

        // Populate arc length tables
        segmentArcLengths = new float[numSegments, arcTableResolution];
        segmentArcTFracLengths = new float[numSegments, arcTableResolution];
        for(int i = 0; i < numSegments; i++){
            Vector2 lastPoint = points[i];
            for(int j = 0; j < arcTableResolution; j++){
                float t = (float)(j + 1) / arcTableResolution;
                Vector2 nextPoint = InterpolateAtPoint(i, t);
                float dist = Vector2.Distance(lastPoint, nextPoint);
                segmentArcTFracLengths[i, j] = dist;
                lastPoint = nextPoint;
            }
        }

        // Calculate segment lengths and total length
        segmentLengths = new float[numSegments];
        totalLength = 0;
        for(int i = 0; i < numSegments; i++){
            float sum = 0;
            for(int j = 0; j < arcTableResolution; j++){
                sum += segmentArcLengths[i, j];
            }
            segmentLengths[i] = sum;
            totalLength += sum;
        }

        // Use total length to calculate segment TFrac lengths
        segmentTFracLengths = new float[numSegments];
        for(int i = 0; i < numSegments; i++){
            segmentTFracLengths[i] = segmentLengths[i] / totalLength;
        }

        // Use segment lengths to calculate arcLength TFracs
        segmentArcTFracLengths = new float[numSegments, arcTableResolution];
        for(int i = 0; i < numSegments; i++){
            for(int j = 0; j < arcTableResolution; j++){
                segmentArcTFracLengths[i, j] = segmentArcLengths[i, j]/segmentLengths[i];
            }
        }
    }

    // Interpolate between two specific points 
    public Vector2 InterpolateAtPoint(int i, float t){
        if((i == points.Length - 1 && !isClosed) || 
            (i > points.Length || i < 0)){
                Debug.Log("ERROR - Path.cs - Can't interpolate at point i = " + i);
                return Vector2.zero;
            }
        
        Vector2 P0 = points[i];
        Vector2 P1 = outControl[i];
        Vector2 P2 = inControl[(i + 1) % points.Length];
        Vector2 P3 = outControl[(i + 1) % points.Length];

        return Mathf.Pow(1-t, 3) * P0 
            + 3 * Mathf.Pow(1-t, 2) * t * P1 
            + 3 * (1-t)*Mathf.Pow(t, 2)*P2 
            + Mathf.Pow(t, 3) * P3;
    }

}
