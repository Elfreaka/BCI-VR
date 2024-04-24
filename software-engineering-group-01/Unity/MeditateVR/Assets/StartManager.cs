using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int nextSceneIndex = 1;
        switch (StaticGameManager.MainManager.environment)
        {
            case "Forest":
                nextSceneIndex = 1;
                break;
            case "Beach":
                nextSceneIndex = 2;
                break;
            default:
                nextSceneIndex = 1;
                break;
        }

        SceneManager.LoadScene(nextSceneIndex, LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
