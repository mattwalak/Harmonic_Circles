using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePlacer : MonoBehaviour
{
    public List<GameObject> stones;
    public List<Stone> stoneScripts;
    public float radius = 3f;
    public float stoneSizeMul = 10.0f;

    public float stoneMulMax = 10.0f;
    public float stoneMulMin = 0.3f;

    void EqualSpaceEqualSize(){
        for(int i = 0; i < stones.Count; i++){
            float rad = i * (2f * Mathf.PI / stones.Count);
            Vector2 pos = new Vector2(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius
            );

            stones[i].transform.position = pos;
        }        
    }

    void HarmonicRadiusConstantSpacing(){
        float[] fudge = new float[stones.Count];
        float fudgeAllotment = 0;
        for(int i = 0; i < stones.Count; i++){
            fudge[i] = stones.Count - i;
            fudgeAllotment += fudge[i];
            /*
            int toAdd = 0;
            if(i < stones.Count / 2){
                toAdd = stones.Count - i;
            }else{
                toAdd = (stones.Count / 2) + (i - (stones.Count / 2) + 1);
            }

            fudge[i] = toAdd;
            fudgeAllotment += toAdd;*/
        }

        for(int i = 0; i < stones.Count; i++){
            fudge[i] = fudge[i] / fudgeAllotment;
        }

        float circumferenceOfRing = 2.0f * Mathf.PI * radius;
        float sumOfAllDiameters = 0;
        for(int i = 0; i < stones.Count; i++){
            sumOfAllDiameters += stoneSizeMul / (i + 1);
        }

        float availablePadding = circumferenceOfRing - sumOfAllDiameters - 0.1f;
        // float padding = availablePadding / (stones.Count - 1); 

        float usedCircumference = 0;
        for(int i = 0; i < stones.Count; i++){
            float frac = usedCircumference / circumferenceOfRing;
            float rad = frac * 2.0f * Mathf.PI;
            float diameter = stoneSizeMul / (i + 1);

            float padding = fudge[i] * availablePadding;
            usedCircumference += (i == 0 ? (diameter/2) + padding : diameter + padding);

            Vector2 pos = new Vector2(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius
            );

            Vector2 scale = new Vector2(diameter, diameter);

            stones[i].transform.position = pos;
            stones[i].transform.localScale = scale;
        }
    }


    void Start()
    {
          HarmonicRadiusConstantSpacing();

          for(int i = 0; i < stones.Count; i++){
            stoneScripts.Add(stones[i].GetComponent<Stone>());
          }
    }

}
