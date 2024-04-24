using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class VisualGuidanceScript : MonoBehaviour
{

    public Vector3 localScaleMin = new Vector3(0.5f, 0.5f, 0.125f);
    public Vector3 localScaleMax = new Vector3(1f, 1f, 0.25f);
    public Vector3 localPositionMin = new Vector3(0, 0, 0);
    public Vector3 localPositionMax = new Vector3(0, 0, 0);
    public Vector3 startingLocalPosition;
    public Vector3 startingLocalScale;
    public bool changeColour = false;
    
    float sinAdjust(float x){
        return 0.5f+Mathf.Sin((x-0.5f) * Mathf.PI)/2;
    }

    void Start()
    {
        if (changeColour)
        {
            //set the object to be transparent
            Color color = GetComponent<Renderer>().material.color;
            color.a = 0.5f;
            GetComponent<Renderer>().material.color = color;
        }
        startingLocalPosition = transform.localPosition;
        startingLocalScale = transform.localScale;
    }

    void Update()
    {
        //declaring an array
        int breathingSum = StaticGameManager.MainManager.breathingPattern[0] + StaticGameManager.MainManager.breathingPattern[1] + StaticGameManager.MainManager.breathingPattern[2] + StaticGameManager.MainManager.breathingPattern[3];


        float scale = 0;
        //different patterns of breathing 4-2-4 4-4-4 4-7-8
        float time = (Time.time % breathingSum);

        //grow
        if (time < StaticGameManager.MainManager.breathingPattern[0])
        {
            scale = sinAdjust(time/StaticGameManager.MainManager.breathingPattern[0]);
        }
        //hold
        else if (time >= StaticGameManager.MainManager.breathingPattern[0] && time < StaticGameManager.MainManager.breathingPattern[1]+StaticGameManager.MainManager.breathingPattern[0])
        {
            scale = 1;
        }
        //shrink
        else if (time >= StaticGameManager.MainManager.breathingPattern[1]+StaticGameManager.MainManager.breathingPattern[0] && time < StaticGameManager.MainManager.breathingPattern[2]+StaticGameManager.MainManager.breathingPattern[1]+StaticGameManager.MainManager.breathingPattern[0])
        {
            scale = sinAdjust((StaticGameManager.MainManager.breathingPattern[2]+StaticGameManager.MainManager.breathingPattern[1]+StaticGameManager.MainManager.breathingPattern[0]-time)/StaticGameManager.MainManager.breathingPattern[2]);
        }
        //hold
        else if (time >= StaticGameManager.MainManager.breathingPattern[2]+StaticGameManager.MainManager.breathingPattern[1]+StaticGameManager.MainManager.breathingPattern[0] && time < StaticGameManager.MainManager.breathingPattern[3]+StaticGameManager.MainManager.breathingPattern[2]+StaticGameManager.MainManager.breathingPattern[1]+StaticGameManager.MainManager.breathingPattern[0])
        {
            scale = 0;
        }
        Vector3 newScale = (localScaleMin + (localScaleMax - localScaleMin) * scale);
        transform.localScale = new Vector3(startingLocalScale.x * newScale.x, startingLocalScale.y * newScale.y,
            startingLocalScale.z * newScale.z);
        transform.localPosition = startingLocalPosition + localPositionMin + (localPositionMax - localPositionMin) * scale;

        if (changeColour)
        {
            // Interpolating between light blue and white based on the scale
            Color lightBlue = new Color(143 / 255f, 186 / 255f, 255 / 255f); // Light blue color
            Color white = new Color(207 / 255f, 225 / 255f, 255 / 255f); // White color

    
            // Interpolate between light blue and white using the scale
            Color lerpedColor = Color.Lerp(white, lightBlue, scale);

            // Applying the color to the object's material
            GetComponent<Renderer>().material.color = lerpedColor;
        }
        



        

        
    }

}
