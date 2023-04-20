using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public GameObject laserComponentPrefab;
    public int[] hitLog;

    private Vector2 direction;
    private LaserComponent[] laserComponents;
    private LayerMask mask;

    private const float MAX_RANGE = 15f;
    private const float EPSILON = 0.0001f;

    public void Init(Vector2 dir, int maxNumComponents){
        direction = dir;

        laserComponents = new LaserComponent[maxNumComponents];
        for(int i = 0; i < maxNumComponents; i++){
            laserComponents[i] = 
                Instantiate(laserComponentPrefab, transform).GetComponent<LaserComponent>();
            laserComponents[i].Init(i, maxNumComponents);
        }

        hitLog = new int[maxNumComponents];
        for(int i = 0; i < maxNumComponents; i++){
            hitLog[i] = -1;
        }

        mask = ~LayerMask.GetMask("AirParticles");
    }
    void Update(){
        for(int i = 0; i < laserComponents.Length; i++){
            hitLog[i] = -1;
            laserComponents[i].DisableRenderer();
        }

        if(laserComponents != null){
            FireLaserComponent(transform.position, direction, 0);
        }
    }

    private void FireLaserComponent(Vector2 origin, Vector2 direction, int depth){
        if(depth >= laserComponents.Length){
            return; // max reflections reached
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, MAX_RANGE, mask);
        float distance = MAX_RANGE;
        if(hit.collider != null){
            distance = Vector2.Distance(origin, hit.point);
            Vector2 newDirection = Vector2.Reflect(direction, hit.normal);
            FireLaserComponent(hit.point - EPSILON * direction, newDirection, depth + 1);

            Stone stone = hit.transform.gameObject.GetComponent<Stone>();
            if(stone != null){
                hitLog[depth] = stone.stoneNum;
            }
        }

        LineRenderer line = laserComponents[depth].GetLineRenderer();
        line.enabled = true;
        Vector3[] positions = new Vector3[2];
        positions[0] = origin;
        positions[1] = origin + (distance * direction);
        line.SetPositions(positions);
    }


}
