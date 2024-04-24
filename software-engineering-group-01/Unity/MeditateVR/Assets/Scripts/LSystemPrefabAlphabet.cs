using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemPrefabAlphabet : MonoBehaviour
{
    public GameObject Empty;
    public GameObject Branch;
    public GameObject Leaf;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    void Start()
    {
        if (StaticGameManager.MainManager.PrefabAlphabet == null)
        {
            StaticGameManager.MainManager.PrefabAlphabet = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
