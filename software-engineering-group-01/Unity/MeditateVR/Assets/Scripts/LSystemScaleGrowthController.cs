using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemScaleGrowthController : MonoBehaviour
{
    public float growthForFullSize;
    public float startingGrowth;
    public LSystem myLSystem;
    public Vector3 fullSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (growthForFullSize > startingGrowth)
        {   
            
            if (myLSystem.lsystemGrowthValue >= growthForFullSize)
            {
                transform.localScale = fullSize;
            }
            else
            {
                
                transform.localScale = fullSize * ((myLSystem.lsystemGrowthValue-startingGrowth)/(growthForFullSize-startingGrowth));
            }
        }
    }
}
