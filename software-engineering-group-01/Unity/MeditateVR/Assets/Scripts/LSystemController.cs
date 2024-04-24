using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using LSystemAlphabet;
using UnityEngine;
using Random = System.Random;

public struct LSystemComponentStruct
{
    public LSystemComponent lSystemComponent;
    public GameObject gameObject;
}

public class LSystemController : MonoBehaviour
{
    public float initialGrowthValue;
    public float maxGrowthValue;
    public float growthRate;
    LSystem myLSystem;
    // Start is called before the first frame update
    void Start()
    {
        myLSystem = LSystem.BasicTree(this, initialGrowthValue: initialGrowthValue, maxGrowthValue: maxGrowthValue, growthRate: growthRate * StaticGameManager.MainManager.plantGrowthRate);
    }

    private void FixedUpdate()
    {
        myLSystem.Grow();
    }
}

public class LSystem
{
    public LSystemController myController;
    public string axiom;
    public Dictionary<string,LSystemSymbol[][]> rules;
    public float lsystemGrowthValue;
    public float maxGrowthValue;
    public float growthRate;

    public LSystem(LSystemController controller, string axiom, Dictionary<string,LSystemSymbol[][]> rules, float initialGrowthValue = 0, float maxGrowthValue = 700, float growthRate = 2)
    {
        this.myController = controller;
        this.axiom = axiom;
        this.rules = rules;
        this.lsystemGrowthValue = initialGrowthValue;
        
        LSystemSymbol startingSymbol = new LSystemSymbol(axiom, destroyOnGrowth:true);
        LSystemComponentStruct startingComponent = startingSymbol.GetInstance(myController.transform, this);
        
        this.maxGrowthValue = maxGrowthValue;
        this.growthRate = growthRate;
        
    }

    public static LSystem BasicTree(LSystemController controller, float initialGrowthValue, float maxGrowthValue, float growthRate)
    {
        Dictionary<string, LSystemSymbol[][]> rules = new Dictionary<string, LSystemSymbol[][]>();
        LSystemSymbol[][] branchRule = new LSystemSymbol[2][];
        branchRule[0] = new LSystemSymbol[]{new Scale(0.8f, growthForFullSize:70), new Branch(), new Leaves("branch", 100)};
        branchRule[1] = new LSystemSymbol[] {new  LSystemSymbol((gameObject =>
        {
            int numberOfBranches = UnityEngine.Random.Range(1, 4);
            return GetNewBranchRotations(numberOfBranches, 0.3f, 0.7f, 85, 0.8f/((numberOfBranches + 2)/6.0f), gameObject);
        }) )};
        rules.Add("branch", branchRule);
        
        LSystemSymbol[][] trunkRule = new LSystemSymbol[1][];
        trunkRule[0] = new LSystemSymbol[]{new Branch(), new Leaves("branch", 100)};
        rules.Add("trunk", trunkRule);

        return new LSystem(controller, "trunk", rules, initialGrowthValue,  maxGrowthValue: maxGrowthValue, growthRate: growthRate);
    }
    
    public static LSystem PalmTree(LSystemController controller, float initialGrowthValue, float maxGrowthValue, float growthRate)
    {
        Dictionary<string, LSystemSymbol[][]> rules = new Dictionary<string, LSystemSymbol[][]>();
        LSystemSymbol[][] stemRule = new LSystemSymbol[1][];
        stemRule[0] = new LSystemSymbol[]{new CreatedGlobalRotation(Quaternion.Euler(5,0,0)), new Scale(new Vector3(1,0.2f, 1), growthForFullSize:30), new Branch(), new Scale(new Vector3(0.90f, (1.0f/0.2f), 0.90f), "stem", 50)};
        rules.Add("stem", stemRule);
        
        LSystemSymbol[][] baseRule = new LSystemSymbol[1][];
        baseRule[0] = new LSystemSymbol[]{new CreatedGlobalRotation(Quaternion.Euler(0,UnityEngine.Random.Range(0.0f, 360.0f), 0)), new LSystemSymbol("stem", 0), new Leaves()};
        rules.Add("base", baseRule);

        return new LSystem(controller, "base", rules, initialGrowthValue,  maxGrowthValue: maxGrowthValue, growthRate: growthRate);
    }

    //Rotations are a pain it took me as long to write this function as it did to write a functioning LSystem
    public static LSystemSymbol[][] GetNewBranchRotations(int numberOfBranches, float verticalAboveRotationPercentage, float verticalBelowRotationPercentage, float maxVerticalRotation, float horizontalRotationPercentage, GameObject startingGameObject)
    {
        /*
         *  numberOfBranches is how many rotations you want to get
         *  verticalAboveRotationPercentage is how far above the current rotation you can rotate to as a percentage of the angle between the current rotation and straight up
         *  verticalBelowRotationPercentage is how far below the current rotation you can rotate to as a percentage of the angle between the current rotation and the maxVerticalRotation
         *  maxVerticalRotation is the maximum angle that a branch can have from straight up
         *  horizontalRotationPercentage is how far around the y axis you can rotate as a percentage of 180 degrees
         *  startingGameObject is the parent of the new branches
         */
        
        LSystemSymbol[][] newSymbols = new LSystemSymbol[numberOfBranches][];
        Quaternion currentRotation = startingGameObject.transform.rotation;
        float angleFromUp = Vector3.Angle(Vector3.up, currentRotation * Vector3.up);
        for (int i = 0; i < numberOfBranches; i++)
        {
            
            Quaternion newRotation = currentRotation;
            if (angleFromUp < 5)
            {
                newRotation = Quaternion.Euler(UnityEngine.Random.Range(40, maxVerticalRotation * verticalBelowRotationPercentage), UnityEngine.Random.Range(360 * ((float)i/numberOfBranches - (0.8f/numberOfBranches)), 360 * ((float)i/numberOfBranches + (0.8f/numberOfBranches))), 0);
            }
            else
            {
                //Horizontal rotation
                newRotation *= Quaternion.Euler(0, UnityEngine.Random.Range(-angleFromUp * verticalAboveRotationPercentage, (maxVerticalRotation - angleFromUp) * verticalBelowRotationPercentage), 0);
                //Vertical rotation
                newRotation *= Quaternion.Euler(UnityEngine.Random.Range(-90 * horizontalRotationPercentage, 90 * horizontalRotationPercentage), 0, 0);
            }

            newSymbols[i] = new LSystemSymbol[] {new CreatedGlobalRotation(newRotation), new Scale(0.7f, growthForFullSize:70), new LSystemSymbol("trunk")};
        }

        return newSymbols;
    }

    public void Grow()
    {
        if (lsystemGrowthValue < maxGrowthValue + 500)
        {
            lsystemGrowthValue += growthRate + StaticGameManager.MainManager.relaxationLevel * 0.5f; 
        }
    }
}






namespace LSystemAlphabet
{
    public class LSystemSymbol
    {
        public delegate LSystemSymbol[][] GetNextStep(GameObject startingGameObject);
        
        public string nextStep;
        public float growthForNextStep;
        public bool destroyOnGrowth;
        public GameObject prefab;
        public GetNextStep getNextStep;
        

        public LSystemSymbol(string nextStep, float growthForNextStep = 0, bool destroyOnGrowth = false)
        {
            this.nextStep = nextStep;
            this.growthForNextStep = growthForNextStep;
            this.destroyOnGrowth = destroyOnGrowth;
        }
        
        public LSystemSymbol(GetNextStep getNextStep , int growthForNextStep = 0)
        {
            this.getNextStep = getNextStep;
            this.growthForNextStep = growthForNextStep;
        }

        public LSystemComponentStruct GetInstance(Transform parent, LSystem myLSystem)
        {
            if (prefab is null)
            {
                prefab = StaticGameManager.MainManager.PrefabAlphabet.Empty;
            }
            GameObject newGameObject = GameObject.Instantiate(prefab, parent);
            LSystemComponent myComponent = newGameObject.AddComponent<LSystemComponent>();
            myComponent.growthForNextStep = growthForNextStep * UnityEngine.Random.Range(0.8f, 1.2f);
            myComponent.destroyOnGrowth = destroyOnGrowth;
            myComponent.nextStep = nextStep;
            myComponent.getNextStep = getNextStep;
            myComponent.myLSystem = myLSystem;
            Start(newGameObject, myLSystem);
            LSystemComponentStruct myStruct = new LSystemComponentStruct();
            myStruct.lSystemComponent = myComponent;
            myStruct.gameObject = newGameObject;
            return myStruct;
        }
        
        public virtual void Start(GameObject gameObject, LSystem myLSystem)
        {
            
        }
    }

    public class Branch : LSystemSymbol
    {
        public Branch(string nextStep = "", int growthForNextStep = 0, bool destroyOnGrowth = false) : base(nextStep, growthForNextStep, destroyOnGrowth)
        {
            prefab = StaticGameManager.MainManager.PrefabAlphabet.Branch;
        }
    }
    
    public class Leaves : LSystemSymbol
    {
        public Leaves(string nextStep = "", int growthForNextStep = 0, bool destroyOnGrowth = false) : base(nextStep, growthForNextStep, destroyOnGrowth)
        {
            prefab = StaticGameManager.MainManager.PrefabAlphabet.Leaf;
        }
    }
    
    public class Scale : LSystemSymbol
    {
        public Vector3 fullSize;
        public float growthForFullSize;
        public Scale(float scale, string nextStep = "", int growthForNextStep = 0, bool destroyOnGrowth = false, float growthForFullSize = 0) : base(nextStep, growthForNextStep, destroyOnGrowth)
        {
            this.fullSize = new Vector3(scale, scale, scale);
            this.growthForFullSize = growthForFullSize;
        }
        
        public Scale(Vector3 scale, string nextStep = "", int growthForNextStep = 0, bool destroyOnGrowth = false, float growthForFullSize = 0) : base(nextStep, growthForNextStep, destroyOnGrowth)
        {
            this.fullSize = scale;
            this.growthForFullSize = growthForFullSize;
        }

        public override void Start(GameObject gameObject, LSystem myLSystem)
        {
            if (growthForFullSize > 0)
            {
                LSystemScaleGrowthController myComponent = gameObject.AddComponent<LSystemScaleGrowthController>();
                gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                myComponent.myLSystem = myLSystem;
                myComponent.startingGrowth = myLSystem.lsystemGrowthValue;
                myComponent.fullSize = fullSize;
                myComponent.growthForFullSize = myLSystem.lsystemGrowthValue + growthForFullSize * UnityEngine.Random.Range(0.8f, 1.2f);
            }
            else
            {
                gameObject.transform.localScale = fullSize;
            }
        }
    }

    public class CreatedGlobalRotation : LSystemSymbol
    {
        //public delegate Quaternion RotationStep(float rotationSpeed, float maxRotation, Quaternion globalRotation);
        
        //public float rotationSpeed;
        public Quaternion rotation;
        //public RotationStep rotationStep;
        
        
        public CreatedGlobalRotation(Quaternion rotation, string nextStep = "", int growthForNextStep = 0, bool destroyOnGrowth = false) : base(nextStep, growthForNextStep, destroyOnGrowth)
        {
            //this.rotationSpeed = rotationSpeed;
            this.rotation = rotation;
            //this.rotationStep = rotationStep;
        }

        public override void Start(GameObject gameObject, LSystem myLSystem)
        {
            gameObject.transform.localRotation = rotation;
        }
    }

}

