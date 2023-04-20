using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicResonanceTest : MonoBehaviour
{
    public OSC osc;

    private float[] buffer;
    private int BUFFER_LENGTH = 128;
    private float timeElapsed = 0;

    private bool isFirstUpdate = true;

    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        buffer = new float[BUFFER_LENGTH];
    }

    void Update(){
        timeElapsed += Time.deltaTime;

        for(int i = 0; i < BUFFER_LENGTH; i++){
            buffer[i] = 0.5f + (0.5f * Mathf.Sin((2 * Mathf.PI / 5.0f) + timeElapsed + (i * 0.1f)));
        }

        meshRenderer.material.mainTexture = CreateTexture();

        if(isFirstUpdate){
            isFirstUpdate = false;

            OscMessage msg = new OscMessage();
            msg.address = "/updateBufferData";
            msg.values.Add(BUFFER_LENGTH);
            for(int i = 0; i < BUFFER_LENGTH; i++){
                msg.values.Add(buffer[i]);
            }
            osc.Send(msg);
        }
    }

    public Texture2D CreateTexture()
    {
        int width = BUFFER_LENGTH;
        int height = 1;

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;

        for(int x = 0; x < width; x++){
            texture.SetPixel(x, 0, new Color(buffer[x], buffer[x], buffer[x], 1.0f));
        }

        /*for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                texture.SetPixel(i, j, new Color(i/10.0f, j/10.0f, 0f, 1f));
            }
        }*/

        texture.Apply();
        return texture;
    }
}
