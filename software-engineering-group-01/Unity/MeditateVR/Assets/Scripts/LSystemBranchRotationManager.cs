using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemBranchRotationManager : MonoBehaviour
{
    LSystemComponent myLSystemComponent;
    public float rotationSpeed = 0.5f;
    float maxRotation = 80;
    public int growthForNextRoatation = 50;
    // Start is called before the first frame update
    void Start()
    {
        myLSystemComponent = GetComponent<LSystemComponent>();
        gameObject.transform.Rotate(0, Random.Range(0, 360), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (myLSystemComponent.myLSystem.lsystemGrowthValue >= myLSystemComponent.growthForNextStep)
        {
            float t1 = (transform.localRotation.eulerAngles.z - maxRotation) * rotationSpeed;
            gameObject.transform.Rotate(0, 0, (transform.localRotation.eulerAngles.z - maxRotation) * rotationSpeed);
            myLSystemComponent.growthForNextStep += growthForNextRoatation;
        }
    }
}
