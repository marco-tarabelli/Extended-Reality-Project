using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExperimentResult
{
    public List<LiquidDataRange> liquids;
    public string resultName;
}

[System.Serializable]
public class LiquidDataRange
{
    public string name;
    public float minAmount;
    public float maxAmount;
}

[System.Serializable]
public class ExperimentDatabase
{
    public List<ExperimentResult> experimentResults;
}

public class ExperimentDatabaseManager : MonoBehaviour
{
    public TextAsset jsonFile;
    private ExperimentDatabase experimentDatabase;

    private void Awake()
    {
        LoadDatabase();
    }

    private void LoadDatabase()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("experimentDatabase");
        if (jsonFile == null)
        {
            Debug.LogError("File JSON non trovato nella cartella Resources!");
            return;
        }
        Debug.Log("Loaded JSON file length: " + jsonFile.text.Length);

        Debug.Log("JSON successfully loaded: " + jsonFile.text);

        experimentDatabase = JsonUtility.FromJson<ExperimentDatabase>(jsonFile.text);
        if (experimentDatabase == null || experimentDatabase.experimentResults == null)
        {
            Debug.LogError("Error loading the experiment database!");
        }
        else
        {
            Debug.Log($"Experiment database loaded with {experimentDatabase.experimentResults.Count} results.");
        }
    }

    public string GetExperimentResult(List<LiquidData> liquids)
    {
        Debug.Log("Method getexperimentresult is called");
        if (experimentDatabase == null || experimentDatabase.experimentResults == null)
        {
            Debug.LogError("db doesnt work.");
            return "Db dint found";
        }
        Debug.Log($"Experiment with {liquids.Count} liquids:");
        foreach (var liquid in liquids)
        {
            Debug.Log($"- {liquid.name}: {liquid.amount}ml");
        }
        foreach (var entry in experimentDatabase.experimentResults)
        {
            if (AreCombinationsEqual(liquids, entry.liquids))
            {
                Debug.Log($"Experiment found: {entry.resultName}");
                return entry.resultName;
            }
        }

        return "Unknown Experimento";
    }

    private bool AreCombinationsEqual(List<LiquidData> userLiquids, List<LiquidDataRange> dbLiquids)
    {
        Debug.Log($"User Liquids Count: {userLiquids.Count}, DB Liquids Count: {dbLiquids.Count}");

        if (userLiquids.Count != dbLiquids.Count)
        {
            Debug.Log("Mismatch in number of liquids.");
            return false;
        }

        // Sort both lists by name
        userLiquids.Sort((a, b) => a.name.CompareTo(b.name));
        dbLiquids.Sort((a, b) => a.name.CompareTo(b.name));

        for (int i = 0; i < userLiquids.Count; i++)
        {
            LiquidData userLiquid = userLiquids[i];
            LiquidDataRange dbLiquid = dbLiquids[i];

            Debug.Log($"Comparing User Liquid: {userLiquid.name} (Amount: {userLiquid.amount}) with DB Liquid: {dbLiquid.name} (Min: {dbLiquid.minAmount}, Max: {dbLiquid.maxAmount})");

            // Compare names case-insensitively
            if (!userLiquid.name.Equals(dbLiquid.name, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Name mismatch: {userLiquid.name} != {dbLiquid.name}");
                return false;
            }

            // Round the value before comparing
            float roundedAmount = Mathf.Round(userLiquid.amount * 100f) / 100f;
            if (roundedAmount < dbLiquid.minAmount || roundedAmount > dbLiquid.maxAmount)
            {
                Debug.Log($"Amount out of range: {roundedAmount} is not in [{dbLiquid.minAmount}, {dbLiquid.maxAmount}]");
                return false;
            }
        }

        Debug.Log("Match found!");
        return true;
    }
}
    
