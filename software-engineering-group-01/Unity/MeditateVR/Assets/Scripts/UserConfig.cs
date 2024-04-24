using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class UserConfig : MonoBehaviour
{
    // Dictionary to store key-value pairs
    private Dictionary<string, string> configData = new Dictionary<string, string>();

    void Start()
    {
        // Specify the path to the text file
        string filePath = "Assets/config.txt";

        try
        {
            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read the contents of the file
                string[] lines = File.ReadAllLines(filePath);

                // Loop through each line in the file
                foreach (string line in lines)
                {
                    // Ignore lines starting with '#'
                    if (!line.StartsWith("#") && line.Contains("="))
                    {
                        // Split the line by '=' character
                        string[] parts = line.Split('=');

                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            // Store key-value pair in the dictionary
                            configData[key] = value;
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid line format: {line}");
                        }
                    }
                }

            }
            else
            {
                Debug.LogError("File does not exist.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
        
        SetBreathingPattern();
        SetEnvironment();
        setBiometric();
		setPlantGrowthRate();
        
        StaticGameManager.MainManager.sessionData.userConfig = configData;

    }
    
    void SetBreathingPattern()
    {
        
        // Check if the dictionary contains the key "breathingPattern"
        if (configData.TryGetValue("BreathingPattern", out var value))
        {
            switch (value)
            {
                case "4-2-4":
                    StaticGameManager.MainManager.breathingPattern = new int[] { 4, 2, 4, 2 };
                    return;
                case "4-4-4":
                    StaticGameManager.MainManager.breathingPattern = new int[] { 4, 4, 4, 4 };
                    return;
                case "4-7-8":
                    StaticGameManager.MainManager.breathingPattern = new int[] { 4, 7, 8, 7 };
                    return;
                default:
                    StaticGameManager.MainManager.breathingPattern = new int[] { 4, 4, 4, 4 };
                    return;
            }
        }
        
        StaticGameManager.MainManager.breathingPattern = new int[] { 4, 4, 4, 4 };
    }
    
    void SetEnvironment()
    {
        // Check if the dictionary contains the key "environment"
        if (configData.TryGetValue("Environment", out var value))
        {
            // if value not in list of environments, default to Forest
            if (value != "Forest" && value != "Beach")
            {
                StaticGameManager.MainManager.environment = "Forest";
                return;
            }
            StaticGameManager.MainManager.environment = value;
        } else
        {
            // defailt to Forest
            StaticGameManager.MainManager.environment = "Forest";
        }
    }

    void setBiometric()
    {
        // Check if the dictionary contains the key "biometric"
        if (configData.TryGetValue("Biometric", out var value))
        {
            if (value != "BrainWave" && value != "HeartRate")
            {
                StaticGameManager.MainManager.biometric = "None";
                return;
            }
            StaticGameManager.MainManager.biometric = value;
        } else
        {
            // defailt to None
            StaticGameManager.MainManager.biometric = "None";
        }
    }

    void setPlantGrowthRate()
    {
        // Check if the dictionary contains the key "plantGrowthRate"
        if (configData.TryGetValue("PlantGrowthRate", out var value))
        {
            // if value is not a float, default to 0.5
            if (!float.TryParse(value, out float result))
            {
                StaticGameManager.MainManager.plantGrowthRate = 0.5f;
                return;
            }
            StaticGameManager.MainManager.plantGrowthRate = float.Parse(value);
        } else
        {
            // defailt to 0.5
            StaticGameManager.MainManager.plantGrowthRate = 0.5f;
        }
    }

    void setDetail()
    {
        // Check if the dictionary contains the key "detail"
        if (configData.TryGetValue("Detail", out var value))
        {
            if (value != "Low" && value != "Medium" && value != "High")
            {
                StaticGameManager.MainManager.detail = "Low";
                return;
            }
            StaticGameManager.MainManager.detail = value;
        } else
        {
            // defailt to Low
            StaticGameManager.MainManager.detail = "Low";
        }
    }

    void setMusic()
    {
        // Check if the dictionary contains the key "music"
        if (configData.TryGetValue("Music", out var value))
        {
            //log
            Debug.Log(value);
            //if value not 0 or 1, default to 0
            if (value != "0" && value != "1")
            {
                StaticGameManager.MainManager.music = false;
                return;
            }
            StaticGameManager.MainManager.music = bool.Parse(value);
        } else
        {
            // defailt to None
            StaticGameManager.MainManager.music = false;
        }
    }
}
