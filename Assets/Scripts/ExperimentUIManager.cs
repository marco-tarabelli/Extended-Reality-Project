using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExperimentUIManager : MonoBehaviour
{
    public Transform contentPanel; // Container inside the Scroll View
    public GameObject experimentPrefab; // Prefab to instantiate
    public ExperimentDatabaseManager databaseManager;

    private string savePath;
    private ExperimentManager experimentManager;
    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "experiments.json");
        experimentManager = FindObjectOfType<ExperimentManager>();
        LoadExperiments();
    }

    private void LoadExperiments()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("JSON file does not exist!");
            return;
        }

        string json = File.ReadAllText(savePath);
        ExperimentListWrapper wrapper = JsonUtility.FromJson<ExperimentListWrapper>(json);

        if (wrapper == null || wrapper.experiments == null || wrapper.experiments.Count == 0)
        {
            Debug.LogWarning("No experiments found in the JSON file!");
            return;
        }

        Debug.Log($"Number of experiments loaded: {wrapper.experiments.Count}");

        foreach (Experiment exp in wrapper.experiments)
        {
            Debug.Log($"Creating experiment {exp.experimentID}");
            GameObject entry = Instantiate(experimentPrefab, contentPanel);
            entry.SetActive(true);

            RectTransform entryRect = entry.GetComponent<RectTransform>();
            entryRect.localScale = Vector3.one;
            entryRect.localPosition = Vector3.zero;

            Transform backgroundTransform = entry.transform.Find("Background");
            if (backgroundTransform == null)
            {
                Debug.LogError("Error: The prefab does not contain an object named 'Background'!");
                continue;
            }

            Transform textTransform = backgroundTransform.Find("ExperimentText");
            if (textTransform == null)
            {
                Debug.LogError("Error: 'Background' does not contain 'ExperimentText'!");
                continue;
            }

            TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent == null)
            {
                Debug.LogError("Error: 'ExperimentText' does not have a TextMeshProUGUI component!");
                continue;
            }

            // **Get the experiment result by comparing the liquids in the database**
            string resultName = databaseManager != null ? databaseManager.GetExperimentResult(exp.liquids) : "Unknown Experiment";

            // **Update the experiment text**
            textComponent.text = $"Result: {resultName}\n" +
                                 string.Join("\n", exp.liquids.ConvertAll(l => $"{l.name}: {l.amount}ml"));

            // **Force text size recalculation**
            LayoutRebuilder.ForceRebuildLayoutImmediate(textComponent.rectTransform);

            Transform deleteButtonTransform = backgroundTransform.Find("DeleteButton");
            if (deleteButtonTransform == null)
            {
                Debug.LogError("Error: 'Background' does not contain 'DeleteButton'!");
                continue;
            }

            Button deleteButton = deleteButtonTransform.GetComponent<Button>();
            if (deleteButton == null)
            {
                Debug.LogError("Error: 'DeleteButton' does not have a Button component!");
                continue;
            }

            deleteButton.onClick.AddListener(() => DeleteExperimentEntry(entry, exp.experimentID));
        }

        // Force UI update
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel.GetComponent<RectTransform>());

        // **Re-enable `VerticalLayoutGroup` to refresh the UI**
        VerticalLayoutGroup layoutGroup = contentPanel.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }

        Debug.Log("UI updated!");
    }

    public void DeleteExperimentEntry(GameObject entry, int experimentID)
    {
        Debug.Log($"Deleting experiment {experimentID}");

        string json = File.ReadAllText(savePath);
        ExperimentListWrapper wrapper = JsonUtility.FromJson<ExperimentListWrapper>(json);

        if (wrapper != null && wrapper.experiments != null)
        {
            wrapper.experiments.RemoveAll(exp => exp.experimentID == experimentID);

            string updatedJson = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(savePath, updatedJson);

            Debug.Log($"Experiment removed from file json {experimentID} .");
        }

        Destroy(entry);

        Debug.Log($"Experiment {experimentID} removed");
    }

    [System.Serializable]
    private class ExperimentListWrapper
    {
        public List<Experiment> experiments;
    }
}


