using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class LiquidData
{
    public string name;
    public float amount; // Amount of liquid
}

[System.Serializable]
public class Experiment
{
    public int experimentID;
    public List<LiquidData> liquids = new List<LiquidData>();
}

public class ExperimentManager : MonoBehaviour
{   
    /*
    private string savePath;
    private List<Experiment> experimentHistory = new List<Experiment>();

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "experiments.json");
        LoadExperiments(); // Load saved experiments on startup
    }

    // Adds a new experiment
    public void SaveExperiment(List<LiquidData> liquids)
    {
        int newID = experimentHistory.Count + 1;
        Experiment newExperiment = new Experiment { experimentID = newID, liquids = liquids };

        experimentHistory.Add(newExperiment);
        SaveToFile();
        Debug.Log("Experiment saved!");
    }*/
    private string savePath;
    private List<Experiment> experimentHistory = new List<Experiment>();

    public ExperimentDatabaseManager databaseManager; // Reference al database

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "experiments.json");
        LoadExperiments();

        if (databaseManager == null)
        {
            databaseManager = FindObjectOfType<ExperimentDatabaseManager>();
        }
    }

    // Adds a new experiment
    public void SaveExperiment(List<LiquidData> liquids)
    {
        
        int newID = experimentHistory.Count + 1;
        string resultName = databaseManager != null ? databaseManager.GetExperimentResult(liquids) : "Unknown Experiment";

        Experiment newExperiment = new Experiment { experimentID = newID, liquids = liquids };

        experimentHistory.Add(newExperiment);
        SaveToFile();

        Debug.Log($"Experiment saved! Result: {resultName}");
    }   
    // Saves to JSON file
    private void SaveToFile()
    {
        string json = JsonUtility.ToJson(new ExperimentListWrapper { experiments = experimentHistory }, true);
        File.WriteAllText(savePath, json);
        Debug.Log("JSON file path: " + Path.Combine(Application.persistentDataPath, "experiments.json"));
    }

    // Loads saved experiments
    private void LoadExperiments()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            ExperimentListWrapper wrapper = JsonUtility.FromJson<ExperimentListWrapper>(json);
            experimentHistory = wrapper.experiments ?? new List<Experiment>();
            Debug.Log("JSON content:\n" + json);
        }
        else
        {
            Debug.Log("JSON path: " + Application.persistentDataPath);
            Debug.LogWarning("The JSON file does not exist!");
        }
    }

    // Gets the list of saved experiments
    public List<Experiment> GetExperiments()
    {
        return experimentHistory;
    }

    [System.Serializable]
    private class ExperimentListWrapper
    {
        public List<Experiment> experiments;
    }
}
