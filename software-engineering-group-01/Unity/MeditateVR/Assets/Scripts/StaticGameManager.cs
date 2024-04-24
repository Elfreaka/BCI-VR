using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class StaticGameManager : MonoBehaviour
{
    public static StaticGameManager MainManager;
    public LSystemPrefabAlphabet PrefabAlphabet;
    public int[] breathingPattern = new int[] { 4, 4, 4, 4 };
    public string environment;
    public string biometric;
    public float plantGrowthRate;
    public string detail;
    public bool music;

    public float attentionLevel;
    public float engagementLevel;
    public float excitementLevel;
    public float interestLevel;
    public float relaxationLevel;
    public float stressLevel;
    
    float timeAtLastDataCapture;
    public float captureRate = 5;
    public SessionData sessionData;
    
    Transform mainCameraTransform;
    
    
    // Awake makes sure there is only one instance of this class and gameobject
    private void Awake()
    {
        if (MainManager == null)
        {
            MainManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        sessionData = new SessionData();
    }

    // Start is called before the first frame update
    void Start()
    {
        //log to console
        Debug.Log(music);
        //if music is true, play music
        if (music == false)
        {
            //set volume to 0
            AudioListener.volume = 0;               
        }
        else
        {
            //set volume to 1
            AudioListener.volume = 1;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeAtLastDataCapture > captureRate)
        {
            timeAtLastDataCapture = Time.time;
            CaptureData();
        }
        
    }

    private void OnApplicationQuit()
    {
        SaveMeditationData();
    }

    public void SaveMeditationData()
    {
        string json = JsonConvert.SerializeObject(sessionData);
        System.IO.File.WriteAllText((Application.dataPath + "\\MeditationLog\\MeditationData" + System.DateTime.Now.ToString("yyyyMMddHmm") + ".json"), json);
    }

    public void CaptureData()
    {
        MeditationFrameData data = new MeditationFrameData();
        data.time = Time.time;
        if (mainCameraTransform == null)
        {
            SetMainCameraTransform();
        }
        data.playerHeadPosition = new float[] {mainCameraTransform.position.x, mainCameraTransform.position.y, mainCameraTransform.position.z};
        data.playerHeadRotation = new float[] {mainCameraTransform.rotation.x, mainCameraTransform.rotation.y, mainCameraTransform.rotation.z};
        data.attentionLevel = attentionLevel;
        data.engagementLevel = engagementLevel;
        data.excitementLevel = excitementLevel;
        data.interestLevel = interestLevel;
        data.relaxationLevel = relaxationLevel;
        data.stressLevel = stressLevel;
        
        sessionData.meditationData.Add(data);

    }

    public void SetMainCameraTransform()
    {
        mainCameraTransform = GameObject.Find("Main Camera").transform;
    }
}




public class SessionData
{
    public Dictionary<string, string> userConfig;

    public List<MeditationFrameData> meditationData;
    
    public SessionData()
    {
        userConfig = new Dictionary<string, string>();
        meditationData = new List<MeditationFrameData>();
    }

}

public class MeditationFrameData
{
    public float time;
    public float[] playerHeadPosition;
    public float[] playerHeadRotation;
    public float attentionLevel;
    public float engagementLevel;
    public float excitementLevel;
    public float interestLevel;
    public float relaxationLevel;
    public float stressLevel;
    
}
