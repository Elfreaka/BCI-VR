using System.Collections;
using System.Collections.Generic;
using LSystemAlphabet;
using UnityEngine;

public class LSystemComponent : MonoBehaviour
{
    public LSystem myLSystem;
    public float growthForNextStep;
    public bool destroyOnGrowth;
    bool grown;
    public string nextStep;
    public LSystemSymbol.GetNextStep getNextStep;
    // Start is called before the first frame update
    void Start()
    {
        if (growthForNextStep > myLSystem.maxGrowthValue)
        {
            grown = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!grown && myLSystem.lsystemGrowthValue >= growthForNextStep)
        {
            Grow();
        }
    }

    void Grow()
    {
        //Check if it actually should grow
        if (nextStep == null)
        {
            if (getNextStep == null)
            {
                grown = true;
                Debug.Log("LSystemComponent nextStep and getNextStep are both null");
                return;
            }
        }
        else
        {
            if (nextStep == "")
            {
                grown = true;
                return;
            }
        }
        
        LSystemSymbol[][] newSymbols = null;
        if (nextStep != null)
        {
            if(myLSystem.rules.ContainsKey(nextStep))
            {
                newSymbols = myLSystem.rules[nextStep];
            }
            else
            {
                Debug.Log("LSystem rule not found: " + nextStep);
            }
        }
        else
        {
            newSymbols = getNextStep(gameObject);
        }
        foreach(LSystemSymbol[] symbolArray in newSymbols)
        {
            GameObject previousGameObject;
            if (destroyOnGrowth)
            {
                previousGameObject = gameObject.transform.parent.gameObject;
            }
            else
            {
                previousGameObject = gameObject;
            }
                
            foreach (LSystemSymbol symbol in symbolArray)
            {
                LSystemComponentStruct newComponent = symbol.GetInstance(previousGameObject.transform, myLSystem);
                GameObject newGameObject = newComponent.gameObject;
                LSystemComponent newLSystemComponent = newComponent.lSystemComponent;
                newLSystemComponent.myLSystem = myLSystem;
                newLSystemComponent.growthForNextStep += myLSystem.lsystemGrowthValue;
                    
                previousGameObject = newGameObject;
            }

            foreach (Transform child in transform)
            {
                //child.parent = previousGameObject.transform;
            }
            
        }
        
        
        if (destroyOnGrowth)
        {
            Destroy(gameObject);
        }
        grown = true;
    }
}
